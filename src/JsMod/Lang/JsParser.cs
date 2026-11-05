using System.Collections.Generic;
using System.Linq;

namespace JsMod.Lang
{
    /// <summary>
    /// Recursive descent parser that builds the game's own Node tree from JS tokens, so that tick costs, stepping, breakpoints,
    /// and drones behave as they do for Python. Errors are raised as the game's ParseException with source positions.
    /// </summary>
    public sealed class JsParser
    {
        private sealed class FunctionScope
        {
            public readonly HashSet<string> Vars = new HashSet<string>();
            public readonly HashSet<string> Consts = new HashSet<string>();
            public readonly FunctionScope Parent;

            public FunctionScope(FunctionScope parent)
            {
                Parent = parent;
            }
        }

        private static readonly HashSet<string> AssignOps = new HashSet<string> { "=", "+=", "-=", "*=", "/=", "%=" };

        private static readonly Dictionary<string, string> UnsupportedSyntax = new Dictionary<string, string>
        {
            { "=>", "Arrow functions are not supported yet. Declare a named function instead." },
            { "?", "The ternary operator is not supported yet. Use if/else instead." },
            { "??", "The ?? operator is not supported yet. Use || instead." },
            { "?.", "Optional chaining (?.) is not supported yet." },
            { "...", "Spread syntax (...) is not supported yet." },
            { "&", "Bitwise operators are not supported yet." },
            { "|", "Bitwise operators are not supported yet." },
            { "^", "Bitwise operators are not supported yet." },
            { "~", "Bitwise operators are not supported yet." },
            { "<<", "Bitwise operators are not supported yet." },
            { ">>", "Bitwise operators are not supported yet." },
            { ">>>", "Bitwise operators are not supported yet." },
            { "++", "++ can only be used as a statement or in a for loop update." },
            { "--", "-- can only be used as a statement or in a for loop update." },
        };

        // JS only ends a statement at a line break when the next token could not continue it. These tokens always can.
        // ++ and -- are left out on purpose: JS treats them as starting a new statement after a line break.
        private static readonly HashSet<string> ExpressionContinuations = new HashSet<string>
        {
            ".", "(", "[", "+", "-", "*", "/", "%", "**", "==", "!=", "===", "!==", "<", ">", "<=", ">=", "&&", "||", "in",
            "=", "+=", "-=", "*=", "/=", "%=", "**=", ",",
            "?", "??", "?.", "&", "|", "^", "<<", ">>", ">>>"
        };

        private static readonly HashSet<string> DeclarationContinuations = new HashSet<string> { "=", "," };

        private readonly List<Token> tokens;
        private readonly CodeWindow window;
        private readonly HashSet<string> allVars = new HashSet<string>();
        private readonly int endOfSource;
        private FunctionScope scope = new FunctionScope(null);
        private int pos;
        private int groupDepth;
        private int loopDepth;
        private int functionDepth;
        private int lastEnd;

        private JsParser(List<Token> tokens, CodeWindow window)
        {
            this.tokens = tokens;
            this.window = window;
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].type != TokenType.NEW_LINE && tokens[i].type != JsTokenizer.Sentinel)
                {
                    endOfSource = tokens[i].startIndex + tokens[i].value.Length;
                    break;
                }
            }
        }

        public static Program Parse(TokenStream stream, CodeWindow window)
        {
            return new JsParser(stream.ToList(), window).ParseProgram();
        }

        private Program ParseProgram()
        {
            if (Current != null && Current.type == JsTokenizer.Sentinel)
            {
                pos++;
            }
            var body = new List<Node>();
            SkipNewLines();
            while (Current != null)
            {
                ParseStatement(body, topLevel: true, functionBody: false);
                SkipNewLines();
            }
            return new Program(new SequenceNode(window) { slots = body }, scope.Vars, allVars, new HashSet<string>());
        }

        // ---- Statements ----

        private void ParseStatement(List<Node> into, bool topLevel, bool functionBody)
        {
            Token token = Current;
            if (token.type == JsTokenizer.Keyword)
            {
                switch (token.value)
                {
                    case "let":
                    case "const":
                    case "var":
                        ParseDeclarations(into);
                        EndStatement();
                        return;
                    case "function":
                        into.Add(ParseFunction(topLevel, topLevel || functionBody));
                        return;
                    case "if":
                        into.Add(ParseIf());
                        return;
                    case "while":
                        into.Add(ParseWhile());
                        return;
                    case "for":
                        into.Add(ParseFor());
                        return;
                    case "return":
                        into.Add(ParseReturn());
                        return;
                    case "break":
                        into.Add(ParseBreak());
                        return;
                    case "continue":
                        into.Add(ParseContinue());
                        return;
                }
            }
            if (IsPunct(";"))
            {
                pos++;
                return;
            }
            if (IsPunct("{"))
            {
                into.Add(ParseBlock());
                return;
            }
            into.Add(ParseSimpleStatement());
            EndStatement();
        }

        private void EndStatement()
        {
            if (Current == null || IsPunct("}"))
            {
                return;
            }
            if (Current.type == TokenType.NEW_LINE)
            {
                SkipNewLines();
                if (IsPunct(";"))
                {
                    pos++;
                }
                return;
            }
            if (IsPunct(";"))
            {
                pos++;
                return;
            }
            throw Unexpected("Expected ';' or a new line");
        }

        private Node ParseBlock()
        {
            Token open = ExpectPunct("{");
            var body = new List<Node>();
            SkipNewLines();
            while (!IsPunct("}"))
            {
                if (Current == null)
                {
                    throw new ParseException("Missing '}' for the block opened here.", open.startIndex, open.startIndex + 1);
                }
                ParseStatement(body, topLevel: false, functionBody: false);
                SkipNewLines();
            }
            Token close = ExpectPunct("}");
            return Block(body, open.startIndex, close.startIndex + 1);
        }

        private Node ParseFunctionBody()
        {
            Token open = ExpectPunct("{");
            var body = new List<Node>();
            SkipNewLines();
            while (!IsPunct("}"))
            {
                if (Current == null)
                {
                    throw new ParseException("Missing '}' for the function body opened here.", open.startIndex, open.startIndex + 1);
                }
                ParseStatement(body, topLevel: false, functionBody: true);
                SkipNewLines();
            }
            Token close = ExpectPunct("}");
            return Block(body, open.startIndex, close.startIndex + 1);
        }

        /// <summary>A block is a sequence. An empty one holds a pass, because a loop charging no ops would hang the simulation.</summary>
        private Node Block(List<Node> body, int start, int end)
        {
            if (body.Count == 0)
            {
                body.Add(new PassNode(window, start, end));
            }
            return new SequenceNode(window) { slots = body };
        }

        private Node ParseBody()
        {
            SkipNewLines();
            if (IsPunct("{"))
            {
                return ParseBlock();
            }
            if (Current == null)
            {
                throw new ParseException("Expected a statement.", endOfSource, endOfSource);
            }
            int start = Current.startIndex;
            int end = start + Current.value.Length;
            var body = new List<Node>();
            ParseStatement(body, topLevel: false, functionBody: false);
            return Block(body, start, end);
        }

        private void ParseDeclarations(List<Node> into)
        {
            Token keyword = Next();
            while (true)
            {
                bool redeclared = Current != null && Current.type == TokenType.IDENTIFIER && scope.Vars.Contains(Current.value);
                Node pattern = ParseBindingPattern(keyword.value == "const");
                JoinLineIfContinued(DeclarationContinuations);
                if (IsPunct("="))
                {
                    Token eq = Next();
                    SkipNewLines();
                    Node value = ParseExpression();
                    RejectChainedAssignment();
                    into.Add(new AssignmentNode("=", window, eq.startIndex, eq.startIndex + 1) { slots = { pattern, value } });
                }
                else if (keyword.value == "const" || !(pattern is ValueNode))
                {
                    throw Unexpected(keyword.value == "const" ? "A const needs a value" : "Destructuring needs a value");
                }
                else if (!redeclared)
                {
                    // `var x;` keeps the value of an x that already exists, so only a new name starts as undefined.
                    Node value = new LiteralNode(new PyNone(), window, keyword.startIndex, keyword.startIndex + keyword.value.Length);
                    into.Add(new AssignmentNode("=", window, keyword.startIndex, keyword.startIndex + keyword.value.Length) { slots = { pattern, value } });
                }
                if (!IsPunct(","))
                {
                    return;
                }
                pos++;
                SkipNewLines();
            }
        }

        private Node ParseBindingPattern(bool isConst)
        {
            if (IsPunct("["))
            {
                Token open = Next();
                var names = new List<Node>();
                groupDepth++;
                while (!IsPunct("]"))
                {
                    names.Add(ParseBindingPattern(isConst));
                    if (!IsPunct(","))
                    {
                        break;
                    }
                    pos++;
                }
                groupDepth--;
                ExpectPunct("]");
                return new ListNode(window, open.startIndex, open.startIndex + 1) { slots = { new SequenceNode(window) { slots = names } } };
            }
            Token name = ExpectIdentifier();
            CheckAssignableName(name.value, name.startIndex);
            Declare(name.value, isConst);
            return new ValueNode(name.value, window, name.startIndex, name.startIndex + name.value.Length);
        }

        private void Declare(string name, bool isConst)
        {
            scope.Vars.Add(name);
            allVars.Add(name);
            if (isConst)
            {
                scope.Consts.Add(name);
            }
            else
            {
                scope.Consts.Remove(name);
            }
        }

        private Node ParseFunction(bool global, bool isStatic)
        {
            Token keyword = Next();
            SkipNewLines();
            if (Current == null || Current.type != TokenType.IDENTIFIER)
            {
                throw Unexpected("Expected a function name");
            }
            Token name = Next();
            CheckAssignableName(name.value, name.startIndex);
            Declare(name.value, isConst: false);

            var paramNames = new List<string>();
            var defaults = new List<Node>();
            var inner = new FunctionScope(scope);
            FunctionScope outer = scope;
            scope = inner;
            ExpectPunct("(");
            groupDepth++;
            while (!IsPunct(")"))
            {
                Token param = ExpectIdentifier();
                CheckAssignableName(param.value, param.startIndex);
                paramNames.Add(param.value);
                Declare(param.value, isConst: false);
                if (IsPunct("="))
                {
                    pos++;
                    defaults.Add(ParseExpression());
                }
                else if (defaults.Count > 0)
                {
                    throw new ParseException("A parameter without a default cannot follow one with a default.", param.startIndex, param.startIndex + param.value.Length);
                }
                if (!IsPunct(","))
                {
                    break;
                }
                pos++;
            }
            groupDepth--;
            ExpectPunct(")");
            SkipNewLines();
            if (!IsPunct("{"))
            {
                throw Unexpected("Expected '{' to start the function body");
            }
            int headerEnd = Current.startIndex + 1;

            int outerGroupDepth = groupDepth;
            int outerLoopDepth = loopDepth;
            groupDepth = 0;
            loopDepth = 0;
            functionDepth++;
            var function = new FunctionNode(paramNames, name.value, global && !ReferenceEquals(window, null), window, keyword.startIndex, headerEnd);
            function.slots.Add(ParseFunctionBody());
            function.slots.AddRange(defaults);
            function.Vars = inner.Vars;
            functionDepth--;
            groupDepth = outerGroupDepth;
            loopDepth = outerLoopDepth;
            scope = outer;

            return new DefNode(name.value, isStatic, window, keyword.startIndex, keyword.startIndex + keyword.value.Length) { slots = { function } };
        }

        private Node ParseIf()
        {
            Token keyword = Next();
            Node condition = ParseCondition();
            Node thenBody = ParseBody();
            var node = new BranchNode(window, keyword.startIndex, keyword.startIndex + keyword.value.Length) { slots = { condition, thenBody } };
            int save = pos;
            SkipNewLines();
            if (Current != null && Current.type == JsTokenizer.Keyword && Current.value == "else")
            {
                pos++;
                SkipNewLines();
                if (Current != null && Current.type == JsTokenizer.Keyword && Current.value == "if")
                {
                    node.slots.Add(ParseIf());
                }
                else
                {
                    node.slots.Add(ParseBody());
                }
            }
            else
            {
                pos = save;
            }
            return node;
        }

        private Node ParseWhile()
        {
            Token keyword = Next();
            Node condition = ParseCondition();
            loopDepth++;
            Node body = ParseBody();
            loopDepth--;
            return new BranchNode(window, keyword.startIndex, keyword.startIndex + keyword.value.Length, looping: true) { slots = { condition, body } };
        }

        private Node ParseCondition()
        {
            SkipNewLines();
            ExpectPunct("(");
            groupDepth++;
            Node condition = ParseExpression();
            if (IsPunct("="))
            {
                throw Unexpected("Assignment is not allowed in a condition. Did you mean '=='?");
            }
            groupDepth--;
            ExpectPunct(")");
            return condition;
        }

        private Node ParseFor()
        {
            Token keyword = Next();
            int start = keyword.startIndex;
            int end = start + keyword.value.Length;
            SkipNewLines();
            ExpectPunct("(");
            groupDepth++;

            Node init = null;
            if (Current != null && Current.type == JsTokenizer.Keyword && (Current.value == "let" || Current.value == "const" || Current.value == "var"))
            {
                int declStart = pos;
                var varsBefore = new HashSet<string>(scope.Vars);
                var constsBefore = new HashSet<string>(scope.Consts);
                Token decl = Next();
                Node pattern = ParseBindingPattern(decl.value == "const");
                if (IsContextual("of"))
                {
                    return FinishForOf(pattern, start, end);
                }
                if (IsKeyword("in"))
                {
                    throw Unexpected("for...in is not supported yet. Use for...of instead");
                }
                // This was only a look ahead for `of`. Undo it, so the real parse below doesn't see the name as redeclared.
                pos = declStart;
                scope.Vars.Clear();
                scope.Vars.UnionWith(varsBefore);
                scope.Consts.Clear();
                scope.Consts.UnionWith(constsBefore);
                var decls = new List<Node>();
                ParseDeclarations(decls);
                init = decls.Count == 1 ? decls[0] : new SequenceNode(window) { slots = decls };
            }
            else if (!IsPunct(";"))
            {
                Node first = ParseSimpleStatement();
                if (IsContextual("of"))
                {
                    CheckAssignmentTarget(first);
                    return FinishForOf(first, start, end);
                }
                if (first is ComparisonNode comparison && comparison.slots.Count == 2 && IsPunct(")"))
                {
                    throw new ParseException("for...in is not supported yet. Use for...of instead.", start, end);
                }
                init = first;
            }

            ExpectPunct(";");
            Node condition = IsPunct(";") ? new LiteralNode(new PyBool(true), window, start, end) : ParseExpression();
            ExpectPunct(";");
            Node update = null;
            if (!IsPunct(")"))
            {
                var updates = new List<Node> { ParseSimpleStatement() };
                while (IsPunct(","))
                {
                    pos++;
                    updates.Add(ParseSimpleStatement());
                }
                update = updates.Count == 1 ? updates[0] : new SequenceNode(window) { slots = updates };
            }
            groupDepth--;
            ExpectPunct(")");

            loopDepth++;
            Node body = ParseBody();
            loopDepth--;
            return new JsForNode(window, start, end)
            {
                slots =
                {
                    init ?? new NoOpNode(window, start, end),
                    condition,
                    update ?? new NoOpNode(window, start, end),
                    body
                }
            };
        }

        private Node FinishForOf(Node pattern, int start, int end)
        {
            pos++;
            Node iterable = ParseExpression();
            groupDepth--;
            ExpectPunct(")");
            loopDepth++;
            Node body = ParseBody();
            loopDepth--;
            return new ForNode(pattern, window, start, end) { slots = { iterable, body } };
        }

        private Node ParseReturn()
        {
            Token keyword = Next();
            if (functionDepth == 0)
            {
                throw new ParseException("'return' can only be used inside a function.", keyword.startIndex, keyword.startIndex + keyword.value.Length);
            }
            var node = new ReturnNode(window, keyword.startIndex, keyword.startIndex + keyword.value.Length);
            if (Current != null && Current.type != TokenType.NEW_LINE && !IsPunct(";") && !IsPunct("}"))
            {
                node.slots.Add(ParseExpression());
            }
            EndStatement();
            return node;
        }

        private Node ParseBreak()
        {
            Token keyword = Next();
            if (loopDepth == 0)
            {
                throw new ParseException("'break' can only be used inside a loop.", keyword.startIndex, keyword.startIndex + keyword.value.Length);
            }
            EndStatement();
            return new BreakNode(window, keyword.startIndex, keyword.startIndex + keyword.value.Length);
        }

        private Node ParseContinue()
        {
            Token keyword = Next();
            if (loopDepth == 0)
            {
                throw new ParseException("'continue' can only be used inside a loop.", keyword.startIndex, keyword.startIndex + keyword.value.Length);
            }
            EndStatement();
            return new ContinueNode(window, keyword.startIndex, keyword.startIndex + keyword.value.Length);
        }

        /// <summary>An expression, assignment, or increment. Used for statements and for loop clauses.</summary>
        private Node ParseSimpleStatement()
        {
            if (IsPunct("++") || IsPunct("--"))
            {
                Token op = Next();
                Node target = ParseUnary();
                return Increment(op, target);
            }
            Node expression = ParseExpression();
            if (Current != null && Current.type == JsTokenizer.Punct)
            {
                if (Current.value == "++" || Current.value == "--")
                {
                    return Increment(Next(), expression);
                }
                if (AssignOps.Contains(Current.value))
                {
                    Token op = Next();
                    SkipNewLines();
                    CheckAssignmentTarget(expression);
                    Node value = ParseExpression();
                    RejectChainedAssignment();
                    return new AssignmentNode(op.value, window, op.startIndex, op.startIndex + op.value.Length) { slots = { expression, value } };
                }
                if (Current.value == "**=")
                {
                    throw Unexpected("**= is not supported. Write x = x ** y instead");
                }
            }
            if (expression is ValueNode bare && (BuiltinFunctions.Functions.ContainsKey(bare.value) || BuiltinFunctions.Methods.ContainsKey(bare.value)))
            {
                throw new ParseException(bare.value + " does nothing without brackets. Did you mean " + bare.value + "()?", bare.boxedParams.wordStart, bare.boxedParams.wordEnd);
            }
            return expression;
        }

        private Node Increment(Token op, Node target)
        {
            CheckAssignmentTarget(target);
            int end = op.startIndex + op.value.Length;
            Node one = new LiteralNode(new PyNumber(1.0), window, op.startIndex, end);
            return new AssignmentNode(op.value == "++" ? "+=" : "-=", window, op.startIndex, end) { slots = { target, one } };
        }

        private void RejectChainedAssignment()
        {
            if (Current != null && Current.type == JsTokenizer.Punct && AssignOps.Contains(Current.value))
            {
                throw Unexpected("Chained assignment is not supported. Use one assignment per statement");
            }
        }

        private void CheckAssignmentTarget(Node target)
        {
            switch (target)
            {
                case ValueNode value:
                    CheckAssignableName(value.value, value.boxedParams.wordStart);
                    CheckNotConst(value);
                    allVars.Add(value.value);
                    return;
                case ListNode list:
                    foreach (Node slot in list.slots[0].slots)
                    {
                        CheckAssignmentTarget(slot);
                    }
                    return;
                case BracketNode bracket:
                    CheckAssignmentTarget(bracket.slots[0]);
                    return;
                case BinaryExprNode binary when binary.op == "." || binary.op == "[]":
                    return;
                default:
                    throw new ParseException("You can't assign to this expression.", target.boxedParams.wordStart, target.boxedParams.wordEnd);
            }
        }

        private void CheckNotConst(ValueNode target)
        {
            for (FunctionScope s = scope; s != null; s = s.Parent)
            {
                if (s.Vars.Contains(target.value))
                {
                    if (s.Consts.Contains(target.value))
                    {
                        throw new ParseException(target.value + " is a const and can't be reassigned.", target.boxedParams.wordStart, target.boxedParams.wordEnd);
                    }
                    return;
                }
            }
        }

        private static void CheckAssignableName(string name, int start)
        {
            if (Scope.IsConstant(name))
            {
                throw new ParseException(name + " is a reserved name in this game.", start, start + name.Length);
            }
        }

        // ---- Expressions, lowest precedence first ----

        private Node ParseExpression()
        {
            return ParseOr();
        }

        private Node ParseOr()
        {
            Node left = ParseAnd();
            while (IsPunct("||"))
            {
                Token op = NextOperator();
                left = new BinaryExprNode("or", TokenType.OR, window, op.startIndex, op.startIndex + 2) { slots = { left, ParseAnd() } };
            }
            return left;
        }

        private Node ParseAnd()
        {
            Node left = ParseEquality();
            while (IsPunct("&&"))
            {
                Token op = NextOperator();
                left = new BinaryExprNode("and", TokenType.AND, window, op.startIndex, op.startIndex + 2) { slots = { left, ParseEquality() } };
            }
            return left;
        }

        private Node ParseEquality()
        {
            Node left = ParseRelational();
            while (IsPunct("==") || IsPunct("!=") || IsPunct("===") || IsPunct("!=="))
            {
                Token op = NextOperator();
                string gameOp = op.value.StartsWith("!") ? "!=" : "==";
                left = Compare(left, gameOp, ParseRelational());
            }
            return left;
        }

        private Node ParseRelational()
        {
            Node left = ParseAdditive();
            while (IsPunct("<") || IsPunct(">") || IsPunct("<=") || IsPunct(">=") || IsKeyword("in"))
            {
                Token op = NextOperator();
                left = Compare(left, op.value, ParseAdditive());
            }
            return left;
        }

        private Node Compare(Node left, string op, Node right)
        {
            return new ComparisonNode(new List<string> { op }, window, left.boxedParams.wordStart, lastEnd) { slots = { left, right } };
        }

        private Node ParseAdditive()
        {
            Node left = ParseMultiplicative();
            while (IsPunct("+") || IsPunct("-"))
            {
                Token op = NextOperator();
                left = new BinaryExprNode(op.value, TokenType.ADD, window, op.startIndex, op.startIndex + 1) { slots = { left, ParseMultiplicative() } };
            }
            return left;
        }

        private Node ParseMultiplicative()
        {
            Node left = ParseUnary();
            while (IsPunct("*") || IsPunct("/") || IsPunct("%"))
            {
                Token op = NextOperator();
                left = new BinaryExprNode(op.value, TokenType.MULT, window, op.startIndex, op.startIndex + 1) { slots = { left, ParseUnary() } };
            }
            return left;
        }

        private Node ParseUnary()
        {
            if (IsPunct("-") || IsPunct("+") || IsPunct("!"))
            {
                Token op = NextOperator();
                string gameOp = op.value == "!" ? "not" : op.value;
                return new UnaryExprNode(gameOp, window, op.startIndex, op.startIndex + 1) { slots = { ParseUnary() } };
            }
            return ParseExponent();
        }

        private Node ParseExponent()
        {
            Node left = ParsePostfix();
            if (IsPunct("**"))
            {
                Token op = NextOperator();
                return new BinaryExprNode("**", TokenType.EXP, window, op.startIndex, op.startIndex + 2) { slots = { left, ParseUnary() } };
            }
            return left;
        }

        private Node ParsePostfix()
        {
            Node node = ParseAtom();
            while (true)
            {
                JoinLineIfContinued(ExpressionContinuations);
                if (IsPunct("["))
                {
                    Token open = Next();
                    groupDepth++;
                    Node index = ParseExpression();
                    groupDepth--;
                    ExpectPunct("]");
                    node = new BinaryExprNode("[]", TokenType.BRACKET_OPEN, window, open.startIndex, open.startIndex + 1)
                    {
                        slots = { node, new SequenceNode(window) { slots = { index } } }
                    };
                }
                else if (IsPunct("("))
                {
                    Token open = Next();
                    Node args = ParseSequence(")");
                    node = new CallNode(window, open.startIndex, open.startIndex + 1) { slots = { node, args } };
                }
                else if (IsPunct("."))
                {
                    Token dot = NextOperator();
                    if (Current == null || (Current.type != TokenType.IDENTIFIER && Current.type != JsTokenizer.Keyword))
                    {
                        throw Unexpected("Expected a name after '.'");
                    }
                    Token member = Next();
                    int memberEnd = member.startIndex + member.value.Length;
                    if (node is LiteralNode literal && literal.value is PyConstBag bag)
                    {
                        try
                        {
                            node = new LiteralNode(bag.Evaluate(member.value, node.boxedParams.wordStart, node.boxedParams.wordEnd).val, window, node.boxedParams.wordStart, memberEnd);
                        }
                        catch (ExecuteException ex)
                        {
                            throw new ParseException(ex.Message, ex.startIndex, ex.endIndex);
                        }
                    }
                    else
                    {
                        Node name = new ValueNode(member.value, window, member.startIndex, memberEnd);
                        node = new BinaryExprNode(".", TokenType.DOT, window, dot.startIndex, dot.startIndex + 1) { slots = { node, name } };
                    }
                }
                else
                {
                    return node;
                }
            }
        }

        /// <summary>Comma separated expressions up to the closer, which is consumed. The opener has already been consumed.</summary>
        private Node ParseSequence(string closer)
        {
            var items = new List<Node>();
            groupDepth++;
            while (!IsPunct(closer))
            {
                items.Add(ParseExpression());
                if (!IsPunct(","))
                {
                    break;
                }
                pos++;
            }
            groupDepth--;
            ExpectPunct(closer);
            return new SequenceNode(window) { slots = items };
        }

        private Node ParseAtom()
        {
            Token token = Current;
            if (token == null)
            {
                throw new ParseException("The code ended where an expression was expected.", endOfSource, endOfSource);
            }
            int end = token.startIndex + token.value.Length;
            if (token.type == JsTokenizer.Number)
            {
                pos++;
                lastEnd = end;
                if (!JsTokenizer.TryParseNumber(token.value, out double number))
                {
                    throw new ParseException("Invalid number.", token.startIndex, end);
                }
                return new LiteralNode(new PyNumber(number), window, token.startIndex, end);
            }
            if (token.type == JsTokenizer.Text)
            {
                pos++;
                lastEnd = end;
                string inner = token.value.Substring(1, token.value.Length - 2);
                if (token.value[0] == '`' && inner.Contains("${"))
                {
                    throw new ParseException("Template interpolation (${...}) is not supported yet. Use + and str() instead.", token.startIndex, end);
                }
                return new LiteralNode(new PyString(inner), window, token.startIndex, end);
            }
            if (token.type == TokenType.IDENTIFIER)
            {
                pos++;
                lastEnd = end;
                IPyObject constant = Scope.EvaluateConstant(token.value);
                return constant != null
                    ? (Node)new LiteralNode(constant, window, token.startIndex, end)
                    : new ValueNode(token.value, window, token.startIndex, end);
            }
            if (token.type == JsTokenizer.Keyword)
            {
                switch (token.value)
                {
                    case "true":
                    case "false":
                        pos++;
                        lastEnd = end;
                        return new LiteralNode(new PyBool(token.value == "true"), window, token.startIndex, end);
                    case "null":
                    case "undefined":
                        pos++;
                        lastEnd = end;
                        return new LiteralNode(new PyNone(), window, token.startIndex, end);
                    case "function":
                        throw new ParseException("Function expressions are not supported yet. Declare a named function instead.", token.startIndex, end);
                }
                throw new ParseException("'" + token.value + "' is not supported here.", token.startIndex, end);
            }
            if (token.type == JsTokenizer.Unknown)
            {
                throw new ParseException(DescribeUnknown(token), token.startIndex, end);
            }
            if (IsPunct("("))
            {
                pos++;
                groupDepth++;
                Node inner = ParseExpression();
                groupDepth--;
                ExpectPunct(")");
                return new BracketNode(window, inner.boxedParams.wordStart, inner.boxedParams.wordEnd) { slots = { inner } };
            }
            if (IsPunct("["))
            {
                pos++;
                Node items = ParseSequence("]");
                return new ListNode(window, token.startIndex, end) { slots = { items } };
            }
            if (IsPunct("{"))
            {
                return ParseObject();
            }
            if (token.type == TokenType.NEW_LINE)
            {
                throw new ParseException("The line ended where an expression was expected.", token.startIndex, token.startIndex + 1);
            }
            if (token.type == JsTokenizer.Punct && UnsupportedSyntax.TryGetValue(token.value, out string message))
            {
                throw new ParseException(message, token.startIndex, end);
            }
            throw new ParseException("Unexpected '" + token.value + "'.", token.startIndex, end);
        }

        private Node ParseObject()
        {
            Token open = Next();
            var items = new List<Node>();
            groupDepth++;
            while (!IsPunct("}"))
            {
                Token key = Current;
                if (key != null && (key.type == TokenType.IDENTIFIER || key.type == JsTokenizer.Keyword))
                {
                    pos++;
                    items.Add(new LiteralNode(new PyString(key.value), window, key.startIndex, key.startIndex + key.value.Length));
                }
                else if (IsPunct("["))
                {
                    pos++;
                    items.Add(ParseExpression());
                    ExpectPunct("]");
                }
                else
                {
                    items.Add(ParseAtom());
                }
                ExpectPunct(":");
                items.Add(ParseExpression());
                if (!IsPunct(","))
                {
                    break;
                }
                pos++;
            }
            groupDepth--;
            ExpectPunct("}");
            return new DictNode(window, open.startIndex, open.startIndex + 1) { slots = { new SequenceNode(window) { slots = items } } };
        }

        // ---- Token helpers ----

        private Token Current
        {
            get
            {
                if (groupDepth > 0)
                {
                    while (pos < tokens.Count && tokens[pos].type == TokenType.NEW_LINE)
                    {
                        pos++;
                    }
                }
                return pos < tokens.Count ? tokens[pos] : null;
            }
        }

        private Token Next()
        {
            Token token = Current;
            pos++;
            if (token != null)
            {
                lastEnd = token.startIndex + token.value.Length;
            }
            return token;
        }

        /// <summary>Consumes a binary or unary operator. A line break straight after one continues the expression.</summary>
        private Token NextOperator()
        {
            Token token = Next();
            SkipNewLines();
            return token;
        }

        /// <summary>Moves past line breaks when the next real token continues the current statement, as JS does.</summary>
        private void JoinLineIfContinued(HashSet<string> continuations)
        {
            if (groupDepth > 0 || pos >= tokens.Count || tokens[pos].type != TokenType.NEW_LINE)
            {
                return;
            }
            int ahead = pos;
            while (ahead < tokens.Count && tokens[ahead].type == TokenType.NEW_LINE)
            {
                ahead++;
            }
            if (ahead < tokens.Count
                && (tokens[ahead].type == JsTokenizer.Punct || tokens[ahead].type == JsTokenizer.Keyword)
                && continuations.Contains(tokens[ahead].value))
            {
                pos = ahead;
            }
        }

        private void SkipNewLines()
        {
            while (pos < tokens.Count && tokens[pos].type == TokenType.NEW_LINE)
            {
                pos++;
            }
        }

        private bool IsPunct(string value)
        {
            Token token = Current;
            return token != null && token.type == JsTokenizer.Punct && token.value == value;
        }

        private bool IsKeyword(string value)
        {
            Token token = Current;
            return token != null && token.type == JsTokenizer.Keyword && token.value == value;
        }

        private bool IsContextual(string value)
        {
            Token token = Current;
            return token != null && token.type == TokenType.IDENTIFIER && token.value == value;
        }

        private Token ExpectPunct(string value)
        {
            if (!IsPunct(value))
            {
                throw Unexpected("Expected '" + value + "'");
            }
            return Next();
        }

        private Token ExpectIdentifier()
        {
            if (Current == null || Current.type != TokenType.IDENTIFIER)
            {
                throw Unexpected("Expected a name");
            }
            return Next();
        }

        private ParseException Unexpected(string expectation)
        {
            Token token = Current;
            if (token == null)
            {
                return new ParseException(expectation + ", but the code ended.", endOfSource, endOfSource);
            }
            if (token.type == JsTokenizer.Unknown)
            {
                return new ParseException(DescribeUnknown(token), token.startIndex, token.startIndex + token.value.Length);
            }
            if (token.type == TokenType.NEW_LINE)
            {
                return new ParseException(expectation + ", but the line ended.", token.startIndex, token.startIndex + 1);
            }
            if (token.type == JsTokenizer.Punct && UnsupportedSyntax.TryGetValue(token.value, out string message))
            {
                return new ParseException(message, token.startIndex, token.startIndex + token.value.Length);
            }
            return new ParseException(expectation + ", found '" + token.value + "'.", token.startIndex, token.startIndex + token.value.Length);
        }

        private static string DescribeUnknown(Token token)
        {
            if (token.value.StartsWith("/*"))
            {
                return "This comment is never closed. Add */ to end it.";
            }
            if (token.value.StartsWith("\"") || token.value.StartsWith("'") || token.value.StartsWith("`"))
            {
                return "This string is never closed.";
            }
            if (char.IsDigit(token.value[0]) || token.value[0] == '.')
            {
                return "Invalid number '" + token.value + "'.";
            }
            return "Unexpected character '" + token.value + "'.";
        }
    }
}

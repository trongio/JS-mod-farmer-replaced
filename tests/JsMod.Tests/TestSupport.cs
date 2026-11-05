using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JsMod.Lang;

internal static class TestSupport
{
    public const string Marker = "// @js\n";

    public static CodeWindow FakeWindow()
    {
        var window = (CodeWindow)RuntimeHelpers.GetUninitializedObject(typeof(CodeWindow));
        window.parsedFunctions = new ConcurrentDictionary<string, FunctionNode>();
        return window;
    }

    public static Program ParseJs(string js)
    {
        return JsParser.Parse(JsTokenizer.Tokenize(Marker + js), FakeWindow());
    }

    public static Program ParsePy(string py)
    {
        Tokenizer.Tokenize(py, out TokenStream stream);
        return Parser.Parse(stream, FakeWindow());
    }

    /// <summary>Structural dump including operators, names, and flags, which Node.ToString leaves out.</summary>
    public static string Dump(Node node)
    {
        var text = new StringBuilder();
        Dump(node, text);
        return text.ToString();
    }

    private static void Dump(Node node, StringBuilder text)
    {
        if (node == null)
        {
            text.Append("null");
            return;
        }
        text.Append(node.GetType().Name);
        for (Type t = node.GetType(); t != null && t != typeof(Node); t = t.BaseType)
        {
            foreach (FieldInfo field in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                object value = field.GetValue(node);
                if (value is string || value is bool || value is TokenType)
                {
                    text.Append(' ').Append(field.Name).Append('=').Append(value);
                }
                else if (value is List<string> list)
                {
                    text.Append(' ').Append(field.Name).Append("=[").Append(string.Join(",", list)).Append(']');
                }
                else if (value is IPyObject py)
                {
                    text.Append(' ').Append(field.Name).Append('=').Append(py.GetType().Name).Append(':').Append(py is IEnumerable ? "" : py.ToString());
                    if (py is PyString s)
                    {
                        text.Append((string)s.ToString());
                    }
                }
                else if (value is Node pattern)
                {
                    text.Append(' ').Append(field.Name).Append('=');
                    Dump(pattern, text);
                }
            }
        }
        if (node is FunctionNode function)
        {
            text.Append(" vars=[").Append(string.Join(",", function.Vars.OrderBy(v => v))).Append(']');
        }
        text.Append('(');
        foreach (Node slot in node.slots)
        {
            Dump(slot, text);
            text.Append(',');
        }
        text.Append(')');
    }

    /// <summary>Runs a tree in the game's real interpreter with a Simulation that has only what language nodes touch.</summary>
    public sealed class Run
    {
        public ProgramState State;
        public double Ops => State.OpCount;

        public double Number(string name)
        {
            return (double)(PyNumber)State.moduleState.globalScope.Evaluate(name, "").val;
        }

        public IPyObject Value(string name)
        {
            return State.moduleState.globalScope.Evaluate(name, "").val;
        }
    }

    public static Simulation FakeSim()
    {
        var sim = (Simulation)RuntimeHelpers.GetUninitializedObject(typeof(Simulation));
        sim.randomRandom = new Random(1);
        typeof(Simulation).GetField("operationOps").SetValue(sim, 1.0);
        return sim;
    }

    public static Run Execute(Node tree, double opLimit = 1_000_000)
    {
        // Execution id -1 equals the unset id on nodes, which skips the unlock check that needs Unity.
        var execution = new Execution(FakeSim(), tree, -1);
        ProgramState state = execution.MainState;
        for (int guard = 0; guard < 1_000_000; guard++)
        {
            state.PerformExecutionStep(opLimit, out _);
            if (state.currentSideEffect == SideEffect.Error)
            {
                throw new InvalidOperationException("Runtime error: " + state.currentExecuteException?.Message);
            }
            if (state.currentSideEffect == SideEffect.Terminated || state.OpCount > opLimit)
            {
                break;
            }
        }
        return new Run { State = state };
    }

    public static Run RunJs(string js, double opLimit = 1_000_000)
    {
        return Execute(ParseJs(js).syntaxTree, opLimit);
    }

    public static Run RunPy(string py, double opLimit = 1_000_000)
    {
        return Execute(ParsePy(py).syntaxTree, opLimit);
    }
}

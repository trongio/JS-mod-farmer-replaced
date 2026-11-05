using System.Collections.Generic;
using System.Linq;

namespace JsMod.Lang
{
    /// <summary>
    /// C-style for loop. Slots: init, condition, update, body. Costs follow the game's while loop: one operation on the
    /// first iteration plus whatever the slots charge themselves. `continue` still runs the update, as JS requires.
    /// </summary>
    public class JsForNode : Node
    {
        public override string NodeName => "jsfor";

        public JsForNode(CodeWindow func, int startIndex, int endIndex)
            : base(func, startIndex, endIndex)
        {
        }

        private JsForNode(BoxedNodeParams boxedParams)
            : base(boxedParams)
        {
        }

        public override IEnumerable<double> Execute(ProgramState state, Execution execution, int depth)
        {
            ErrorsAndBreakpoints(state, execution, depth);
            foreach (double tick in slots[0].Execute(state, execution, depth + 1))
            {
                yield return tick;
            }
            bool first = true;
            while (true)
            {
                bool condition;
                if (LiteralCondition(slots[1]) is LiteralNode literal)
                {
                    condition = Scope.IsTrueValue(literal.value);
                    execution.sim.UnlockAchievement("INFINITE_LOOP");
                }
                else
                {
                    foreach (double tick in slots[1].Execute(state, execution, depth + 1))
                    {
                        yield return tick;
                    }
                    condition = Scope.IsTrueValue(state.ReturnValue);
                }
                Blink(state, execution);
                if (first)
                {
                    first = false;
                    if (CheckIncrementOpCount(state, execution, execution.sim.operationOps))
                    {
                        yield return 0.0;
                    }
                }
                if (!condition)
                {
                    break;
                }
                bool exitLoop = false;
                IEnumerator<double> body = slots[3].Execute(state, execution, depth + 1).GetEnumerator();
                while (true)
                {
                    double tick;
                    try
                    {
                        if (!body.MoveNext())
                        {
                            break;
                        }
                        tick = body.Current;
                    }
                    catch (BreakStatement)
                    {
                        exitLoop = true;
                        break;
                    }
                    catch (ContinueStatement)
                    {
                        break;
                    }
                    yield return tick;
                }
                if (exitLoop)
                {
                    break;
                }
                // Before the update: the update sits on the `for` line, so afterwards step mode would see no line change and never pause here.
                ErrorsAndBreakpoints(state, execution, depth);
                yield return 0.0;
                foreach (double tick in slots[2].Execute(state, execution, depth + 1))
                {
                    yield return tick;
                }
            }
            state.ReturnValue = new PyNone();
        }

        public override void CheckDependencies(ProgramState state, Execution execution)
        {
            state.currentDependencies.Add(("while", boxedParams.wordStart, boxedParams.wordEnd));
        }

        public override Node DeepCopy(Dictionary<object, object> copies)
        {
            if (copies.TryGetValue(this, out object existing))
            {
                return (Node)existing;
            }
            var copy = new JsForNode(boxedParams);
            copies[this] = copy;
            copy.slots = slots.Select(s => s.DeepCopy(copies)).ToList();
            return copy;
        }

        private static LiteralNode LiteralCondition(Node node)
        {
            while (node is BracketNode bracket && bracket.slots.Count > 0)
            {
                node = bracket.slots[0];
            }
            return node as LiteralNode;
        }
    }
}

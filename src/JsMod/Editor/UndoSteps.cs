using System;

namespace JsMod.Editor
{
    /// <summary>The undo history can hold entries equal to the current text, which would make a button click look like it did nothing.</summary>
    public static class UndoSteps
    {
        public const int MaxSteps = 8;

        /// <summary>Steps until the text visibly changes, or the limit is reached. Returns whether it changed.</summary>
        public static bool StepUntilChanged(Func<string> text, Action step)
        {
            string before = text();
            for (int i = 0; i < MaxSteps; i++)
            {
                step();
                if (text() != before)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

namespace JsMod.Editor
{
    /// <summary>
    /// Spots Shift pressed twice quickly, as in PhpStorm's Search Everywhere. Either Shift counts for either press. It's fed one frame at a
    /// time, so that it can be tested without the game. Another key or mouse button in between, or Ctrl or Alt held, starts it over.
    /// </summary>
    public sealed class DoubleShift
    {
        public const float Window = 0.3f;

        private enum State
        {
            Idle,
            FirstHeld,
            FirstReleased,
            SecondHeld
        }

        private State state;
        private float firstPress;

        public void Reset()
        {
            state = State.Idle;
        }

        /// <summary>Returns true on the frame of the second press.</summary>
        public bool Feed(float now, bool shiftPressed, bool shiftReleased, bool otherPressed, bool ctrlOrAltHeld)
        {
            if (otherPressed || ctrlOrAltHeld)
            {
                state = State.Idle;
                return false;
            }
            if (shiftPressed)
            {
                switch (state)
                {
                    case State.Idle:
                        state = State.FirstHeld;
                        firstPress = now;
                        return false;
                    case State.FirstReleased:
                        if (now - firstPress <= Window)
                        {
                            state = State.SecondHeld;
                            return true;
                        }
                        state = State.FirstHeld;
                        firstPress = now;
                        return false;
                    default:
                        // A repeat, or the other Shift while this one is still down.
                        return false;
                }
            }
            if (shiftReleased)
            {
                if (state == State.FirstHeld)
                {
                    state = State.FirstReleased;
                }
                else if (state == State.SecondHeld)
                {
                    state = State.Idle;
                }
            }
            return false;
        }
    }
}

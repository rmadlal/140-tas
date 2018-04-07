public class TasAction
{
    // Controls the player. Used for both action types
    public bool Left;
    public bool Right;

    // Used for action: Go to position
    public bool IsPosition;
    public float TargetX;

    // Used for action: Simulate key presses
    public int RemainingFrames;
    public bool Jump;

    public override string ToString()
    {
        return this.IsPosition
            ? string.Format("To {0}", this.TargetX)
            : string.Format("{0} {1} {2}", this.Left ? "Left " : "    ", this.Right ? "Right " : "     ", this.Jump ? "Jump " : "    ");
    }
}

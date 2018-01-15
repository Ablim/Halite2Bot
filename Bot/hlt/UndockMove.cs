namespace Bot.hlt
{
    public class UndockMove : Move
    {
        public UndockMove(Ship ship)
            : base(MoveType.Undock, ship) { }
    }
}

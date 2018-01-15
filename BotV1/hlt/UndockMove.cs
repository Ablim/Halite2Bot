namespace BotV1.hlt
{
    public class UndockMove : Move
    {
        public UndockMove(Ship ship)
            : base(MoveType.Undock, ship) { }
    }
}

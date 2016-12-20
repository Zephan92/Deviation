using Assets.Scripts.Enum;

namespace Assets.Scripts.Interface
{
    interface IExchangeObject
    {
        void MoveObject(Direction direction, int Distance, bool Force = false);
        void MoveObject_Instant(int column, int row);
    }
}

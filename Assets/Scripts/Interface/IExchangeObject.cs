using Assets.Scripts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Interface
{
    interface IExchangeObject
    {
        void MoveObject(Direction direction, int Distance, bool Force = false);
        void MoveObject_Instant(int column, int row);
    }
}

using RaceBike.Model.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model
{
    public class SpeedEventArgs : EventArgs
    {
        public ImmutableSpeed Speed { get; private set; }

        public SpeedEventArgs(ImmutableSpeed speed)
        {
            Speed = speed;
        }
    }
}

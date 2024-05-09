using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public struct Fuel
    {
        #region Properties
        public int Volume { get; private set; }

        #endregion

        public Fuel(int volume = 100)
        {
            Volume = volume;
        }
    }
}

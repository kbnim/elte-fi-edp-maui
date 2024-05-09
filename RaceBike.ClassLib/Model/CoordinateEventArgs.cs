using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model.Classes;

namespace RaceBike.Model
{
    public class CoordinateEventArgs : EventArgs
    {
        public SimplePoint TypeChanged { get; private set; }

        public CoordinateEventArgs(SimplePoint p)
        {
            TypeChanged = p;
        }
    }
}

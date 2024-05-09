using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Persistence;

namespace RaceBike.Model.Classes
{
    public class Bike
    {
        #region Private fields
        private readonly Tank _tank;
        private readonly MutableSpeed _speed;
        #endregion

        #region Properties
        public SimplePoint Location { get; set; }
        public ImmutableSpeed Speed => (ImmutableSpeed)_speed;
        public int MaxCapacity => _tank.MaxCapacity;
        public int TankLevel => _tank.CurrentLevel;
        public bool IsOutOfGas => _tank.IsEmpty();
        #endregion

        #region Constructors
        public Bike(int x, int y)
        {
            _speed = new MutableSpeed();
            _tank  = new Tank();
            Location = new SimplePoint(GameFieldType.Bike, x, y);
        }
        #endregion

        #region Public methods
        public void IncreaseTankLevel(Fuel fuel)
        {
            _tank.IncreaseChargeLevel(fuel);
        }

        public void DecreaseTankLevel()
        {
            switch ((int)_speed)
            {
                case 1: _tank.DecreaseChargeLevel((int)_speed); break;
                case 2: _tank.DecreaseChargeLevel((int)_speed); break;
                case 3: _tank.DecreaseChargeLevel((int)_speed); break;
            }
        }

        public void SpeedUp() { _speed.SpeedUp(); }

        public void SlowDown() { _speed.SlowDown();  }

        public void Reset()
        {
            _tank.Reset();
            _speed.Reset();
        }

        public void SetSpeed(AbstractSpeed speed)
        {
            _speed.SetSpeed(speed);
        }
        #endregion
    }
}

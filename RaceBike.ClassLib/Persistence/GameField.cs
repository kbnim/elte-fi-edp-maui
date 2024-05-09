using RaceBike.Model.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Persistence
{
    public enum GameFieldType { Empty, Bike, Fuel }

    public class GameField
    {
        #region Properties
        public GameFieldType Type { get; set; }
        public int Row
        {
            get { return _coordinate.X; }
            set { _coordinate.X = value; }
        }
        public int Column
        {
            get { return _coordinate.Y; }
            set { _coordinate.Y = value; }
        }
        #endregion

        #region Private fields
        private readonly SimplePoint _coordinate;
        #endregion

        #region Constructors
        public GameField(int row = 0, int column = 0, GameFieldType type = GameFieldType.Empty)
        {
            Type = type;
            _coordinate = new SimplePoint(type, row, column);
        }

        public GameField(SimplePoint simplePoint)
        {
            Type = simplePoint.Field;
            _coordinate = simplePoint;
        }


        #endregion

        #region Public methods
        public static GameField Parse(string line)
        {
            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != 2) throw new FormatException("The line does not contain exactly two tokens.");

            GameFieldType type;
            SimplePoint point;

            try
            {
                type = ParseField(tokens[0]);
                point = SimplePoint.Parse(type, tokens[1]);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new GameField(point);
        }
        #endregion

        #region Overridden methods
        public override string ToString()
        {
            return _coordinate.ToString();
        }
        #endregion

        #region private methods
        private static GameFieldType ParseField(string token)
        {
            return token switch
            {
                "Empty" => GameFieldType.Empty,
                "Bike" => GameFieldType.Bike,
                "Fuel" => GameFieldType.Fuel,
                _ => throw new ArgumentException($"Argument {token} is not a valid value"),
            };
        }
        #endregion
    }
}

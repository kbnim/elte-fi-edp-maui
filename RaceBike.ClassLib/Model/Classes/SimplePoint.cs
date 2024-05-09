using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Persistence;
using System.Runtime.CompilerServices;

namespace RaceBike.Model.Classes
{
    public class SimplePoint : INotifyPropertyChanged, ICloneable
    {
        #region Properties
        private int _x;
        public int X
        {
            get { return _x; }
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }
        private int _y;
        public int Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        private GameFieldType _field;
        public GameFieldType Field
        {
            get { return _field; }
            set
            {
                if (_field != value)
                {
                    _field = value;
                    OnPropertyChanged(nameof(Field));
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Constructor
        public SimplePoint(GameFieldType field, int x = 0, int y = 0)
        {
            Field = field;
            X = x;
            Y = y;
        }
        #endregion

        #region Public methods
        public static bool CheckCoordinates(SimplePoint p1, SimplePoint p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public override string ToString()
        {
            return string.Format($"{Field} ({X},{Y})");
        }

        public static SimplePoint Parse(GameFieldType field, string s)
        {
            int x, y;
            int i = 0;

            while (char.IsWhiteSpace(s[i]) && i < s.Length) i++;

            if (s[i] != '(') throw new FormatException("Character '(' was not found");
            i++;

            var builder = new StringBuilder();

            while (s[i] != ',' && i < s.Length)
            {
                builder.Append(s[i]);
                i++;
            }

            try
            {
                x = Convert.ToInt32(builder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            builder.Clear();

            while ((s[i] == ',' || char.IsWhiteSpace(s[i])) && i < s.Length) i++;

            while (s[i] != ')' && i < s.Length)
            {
                builder.Append(s[i]);
                i++;
            }

            try
            {
                y = Convert.ToInt32(builder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new SimplePoint(field, x, y);
        }

        public object Clone()
        {
            return new SimplePoint(Field, X, Y);
        }
        #endregion

        #region Protected methods
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged is not null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}

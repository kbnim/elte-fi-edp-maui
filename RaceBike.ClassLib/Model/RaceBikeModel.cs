using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model.Classes;
using RaceBike.Persistence;

namespace RaceBike.Model
{
    public class RaceBikeModel
    {
        #region Constants
        private const int COURSE_WIDTH  = 9;
        private const int COURSE_HEIGHT = 16;
        #endregion

        #region Fields
        private readonly IRaceBikeDataAccess _dataAccess;
        private readonly Bike _bike;
        private readonly List<SimplePoint> _fuels;
        private readonly GameFieldType[,] _course;
        private readonly SmartStopwatch _stopwatch;
        private readonly Random _random;
        private readonly object _lock;
        #endregion

        #region Properties
        public static int CourseWidth => COURSE_WIDTH;
        public static int CourseHeight => COURSE_HEIGHT;
        public bool IsGameOver => _bike.IsOutOfGas;
        public bool IsPaused => !_stopwatch.IsRunning;
        public TimeSpan CurrentTime => _stopwatch.Elapsed;
        public TimeSpan RecordTime  { get; private set; }
        public double CurrentTankLevel => CalculateTankLevelPercent();
        public ImmutableSpeed CurrentSpeed => _bike.Speed;
        public GameFieldType[,] Course => _course;
        #endregion

        #region Events
        public event EventHandler? GameContinues;
        public event EventHandler<GameStatsEventArgs>? GameOver;
        public event EventHandler<GameStatsEventArgs>? GameOnPause;
        public event EventHandler<CoordinateEventArgs>? CoordinateChanged;
        public event EventHandler<SpeedEventArgs>? SpeedChanged;
        #endregion

        #region Constructor
        public RaceBikeModel(IRaceBikeDataAccess dataAccess)
        {
            RecordTime = TimeSpan.Zero;

            _dataAccess = dataAccess;
            _lock = new object();
            _fuels = new();
            _stopwatch = new SmartStopwatch();
            _course = new GameFieldType[COURSE_WIDTH, COURSE_HEIGHT];

            for (int i = 0; i < COURSE_WIDTH; i++)
            {
                for (int j = 0; j < COURSE_HEIGHT; j++)
                {
                    _course[i, j] = GameFieldType.Empty;
                }
            }

            int bike_x = (COURSE_WIDTH / 2);
            int bike_y = (COURSE_HEIGHT * 7 / 8) - 1;
            _course[bike_x, bike_y] = GameFieldType.Bike;
            _bike = new Bike(bike_x, bike_y);

            _random = new Random();
        }
        #endregion

        #region Public methods
        public void Reset()
        {
            _bike.Reset();
            _stopwatch.Reset();
            OnSpeedChanged(_bike.Speed);

            for (int i = 0; i < COURSE_WIDTH; i++)
            {
                for (int j = 0; j < COURSE_HEIGHT; j++)
                {
                    _course[i, j] = GameFieldType.Empty;
                }
            }

            int bike_x = (COURSE_WIDTH / 2);
            int bike_y = (COURSE_HEIGHT * 7 / 8) - 1;
            _course[bike_x, bike_y] = GameFieldType.Bike;
            _bike.Location.X = bike_x;
            _bike.Location.Y = bike_y;
            _fuels.Clear();
        }

        public async Task LoadGameAsync(string path)
        {
            if (_dataAccess is null)
            {
                throw new InvalidOperationException("No data access was provided.");
            }

            try
            {
                RaceBikeFile fileContents = await _dataAccess.LoadAsync(path);

                // emptying out storage to load new data
                _fuels.Clear();
                for (int i = 0; i < COURSE_WIDTH; i++)
                {
                    for (int j = 0; j < COURSE_HEIGHT; j++)
                    {
                        _course[i, j] = GameFieldType.Empty;
                    }
                }

                _stopwatch.Add(fileContents.LatestBestTime);
                RecordTime = fileContents.LatestBestTime;
                _bike.SetSpeed(fileContents.Speed);
                OnSpeedChanged(_bike.Speed);

                foreach (GameField entity in fileContents.Entities)
                {
                    switch (entity.Type)
                    {
                        case GameFieldType.Bike:
                            {
                                _course[entity.Row, entity.Column] = GameFieldType.Bike;
                                _bike.Location.X = entity.Row;
                                _bike.Location.Y = entity.Column;
                                break;
                            }
                        case GameFieldType.Fuel:
                            {
                                _course[entity.Row, entity.Column] = GameFieldType.Fuel;
                                var fuel = new SimplePoint(GameFieldType.Fuel, entity.Row, entity.Column);
                                _fuels.Add(fuel);
                                break;
                            }
                        default: break;
                    }
                }

                string title, desc01, desc02, new_resume;
                title = "RaceBike 2000";
                desc01 = "Are you ready to beat your record?";
                desc02 = "Best: " + FormatTime(RecordTime);
                new_resume = "Continue";
                var e = new GameStatsEventArgs(title, desc01, desc02, new_resume);
                OnGameOnPause(e);
            }
            catch(Exception e)
            {
                throw new RaceBikeDataException(e.Message);
            }
            
        }

        public void SaveGame(string path)
        {
            var entities = new Queue<GameField>();
            entities.Enqueue(new GameField(_bike.Location));

            foreach (SimplePoint item in _fuels)
            {
                entities.Enqueue(new GameField(item));
            }

            RaceBikeFile file = new(RecordTime, CurrentSpeed, entities);
            _dataAccess?.SaveAsync(path, file);
        }

        public void MoveLeft()
        {
            if (_bike.Location.X - 1 >= 0)
            {
                // starting coordinate with new value
                SimplePoint old_p = (SimplePoint)_bike.Location.Clone();
                old_p.Field = GameFieldType.Empty;

                _course[_bike.Location.X, _bike.Location.Y] = GameFieldType.Empty;
                _course[_bike.Location.X - 1, _bike.Location.Y] = GameFieldType.Bike;
                _bike.Location.X -= 1;

                // ending coordinate with new coordinate
                SimplePoint new_p = (SimplePoint)_bike.Location.Clone();

                OnCoordinateChanged(old_p);
                OnCoordinateChanged(new_p);
            }
        }

        public void MoveRight()
        {
            if (_bike.Location.X + 1 < COURSE_WIDTH)
            {
                // starting coordinate with new value
                SimplePoint old_p = (SimplePoint)_bike.Location.Clone();
                old_p.Field = GameFieldType.Empty;

                _course[_bike.Location.X, _bike.Location.Y] = GameFieldType.Empty;
                _course[_bike.Location.X + 1, _bike.Location.Y] = GameFieldType.Bike;
                _bike.Location.X += 1;

                // ending coordinate with new coordinate
                SimplePoint new_p = (SimplePoint)_bike.Location.Clone();

                OnCoordinateChanged(old_p);
                OnCoordinateChanged(new_p);
            }
        }

        public void SpeedUp() 
        {
            _bike.SpeedUp();
            OnSpeedChanged(CurrentSpeed); 
        }

        public void SlowDown()
        {
            _bike.SlowDown();
            OnSpeedChanged(CurrentSpeed);
        }

        public void GameTimeElapsing()
        {
            if (_bike.IsOutOfGas)
            {
                _stopwatch.Stop();

                string title, desc01, desc02, new_resume;

                if (_stopwatch.Elapsed > RecordTime)
                {
                    RecordTime = _stopwatch.Elapsed;
                    desc01 = "New record!";
                    desc02 = "Time: " + FormatTime(RecordTime);
                }
                else
                {
                    desc01 = "Record time: " + FormatTime(RecordTime);
                    desc02 = "Current time: " + FormatTime(CurrentTime);
                }

                title = "Game over";
                new_resume = "New";
                var e = new GameStatsEventArgs(title, desc01, desc02, new_resume);
                OnGameOver(e);
            }
            else
            {
                lock (_lock)
                {
                    var caught = new List<SimplePoint>();
                    var lost = new List<SimplePoint>();

                    for (int i = 0; i < _fuels.Count; i++)
                    {
                        if (_fuels[i].Y + 1 < COURSE_HEIGHT)
                        {
                            // SimplePoint old_p, new_p;

                            if (_course[_fuels[i].X, _fuels[i].Y + 1] == GameFieldType.Bike)
                            {
                                caught.Add(_fuels[i]);
                                IncreaseTankLevel();
                            }
                            else
                            {
                                _course[_fuels[i].X, _fuels[i].Y + 1] = GameFieldType.Fuel;
                                var new_p = new SimplePoint(GameFieldType.Fuel, _fuels[i].X, _fuels[i].Y + 1);
                                OnCoordinateChanged(new_p);
                                
                            }

                            _course[_fuels[i].X, _fuels[i].Y] = GameFieldType.Empty;
                            var old_p = new SimplePoint(GameFieldType.Empty, _fuels[i].X, _fuels[i].Y);
                            OnCoordinateChanged(old_p);

                            _fuels[i].Y += 1;
                        }
                        else
                        {
                            _course[_fuels[i].X, _fuels[i].Y] = GameFieldType.Empty;
                            var old_p = new SimplePoint(GameFieldType.Empty, _fuels[i].X, _fuels[i].Y);
                            OnCoordinateChanged(old_p);

                            lost.Add(_fuels[i]);
                        }
                    }

                    foreach (var i in caught)
                    {
                        _fuels.Remove(i);
                    }

                    foreach (var i in lost)
                    {
                        _fuels.Remove(i);
                    }

                    RefreshRecord();
                    DecreaseTankLevel();
                    OnGameContinues();
                }
            }
        }

        public void GameTimePause()
        {
            if (!IsPaused)
            {
                _stopwatch.Stop();

                if (_stopwatch.Elapsed > RecordTime)
                {
                    RecordTime = _stopwatch.Elapsed;
                }

                string title = "RaceBike 2000";
                string desc01 = "Best: " + FormatTime(RecordTime);
                string desc02 = "Time: " + FormatTime(CurrentTime);
                string new_resume = "Resume";
                var e = new GameStatsEventArgs(title, desc01, desc02, new_resume);
                OnGameOnPause(e);
            }
        }

        public void GameTimeResume()
        {
            if (IsPaused)
            {
                _stopwatch.Start();
            }
        }

        public void GenerateNewFuelItem()
        {
            int x = _random.Next(COURSE_WIDTH);
            int y = 0;

            _course[x, y] = GameFieldType.Fuel;
            var fuel_point = new SimplePoint(GameFieldType.Fuel, x, y);
            OnCoordinateChanged(fuel_point);
            _fuels.Add(fuel_point);
        }
        #endregion

        #region Private methods
        private void RefreshRecord()
        {
            if (_stopwatch.Elapsed > RecordTime)
            {
                RecordTime = _stopwatch.Elapsed;
            }
        }

        private void IncreaseTankLevel()
        {
            _bike.IncreaseTankLevel(new Fuel(100));
        }

        private static string FormatTime(TimeSpan ts)
        {
            return ts.ToString("mm\\:ss\\.ff");
        }

        private void DecreaseTankLevel() { _bike.DecreaseTankLevel(); }

        private int CalculateTankLevelPercent()
        {
            double percent = (double)_bike.TankLevel / _bike.MaxCapacity * 100;
            return (int)percent;
        }
        #endregion

        #region Private event methods
        private void OnCoordinateChanged(SimplePoint p)
        {
            CoordinateChanged?.Invoke(this, new CoordinateEventArgs(p));
        }

        private void OnGameContinues()
        {
            GameContinues?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameOver(GameStatsEventArgs e)
        {
            GameOver?.Invoke(this, e);
        }

        private void OnGameOnPause(GameStatsEventArgs e)
        {
            GameOnPause?.Invoke(this, e);
        }

        private void OnSpeedChanged(ImmutableSpeed speed)
        {
            SpeedChanged?.Invoke(this, new SpeedEventArgs(speed));
        }
        #endregion
    }
}

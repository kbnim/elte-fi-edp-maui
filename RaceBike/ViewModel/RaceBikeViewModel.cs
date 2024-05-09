using RaceBike.Model;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model.Classes;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;

namespace RaceBike.ViewModel
{
    public class RaceBikeViewModel : ViewModelBase
    {
        #region Private fields
        private readonly RaceBikeModel _model;
        #endregion

        #region Properties (for the statistics)
        public string LatestBestTime 
        { 
            get { return _model.RecordTime.ToString("mm\\:ss\\.ff"); } 
            set { OnPropertyChanged(nameof(LatestBestTime)); }
        }
        public string CurrentTime
        {
            get { return _model.CurrentTime.ToString("mm\\:ss\\.ff"); }
            set { OnPropertyChanged(nameof(CurrentTime)); }
        } 
        public string CurrentTankLevel
        {
            get { return _model.CurrentTankLevel.ToString() + " / 100"; }
            set { OnPropertyChanged(nameof(CurrentTankLevel)); }
        }
        public string CurrentSpeed => _model.CurrentSpeed.ToString();
        public ObservableCollection<SimplePoint> Coordinates { get; private set; }
        public static int CourseWidth  => RaceBikeModel.CourseWidth;
        public static int CourseHeight => RaceBikeModel.CourseHeight;

        public RowDefinitionCollection RowDefinitions
        {
            get => new(Enumerable.Repeat(new RowDefinition(GridLength.Star), CourseHeight).ToArray());
        }
        public ColumnDefinitionCollection ColumnDefinitions
        {
            get => new(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), CourseWidth).ToArray());
        }
        #endregion

        #region Game commands
        public DelegateCommand KeyCommand_PauseResume { get; private set; }
        public DelegateCommand KeyCommand_MoveLeft { get; private set; }
        public DelegateCommand KeyCommand_MoveRight { get; private set; }
        public DelegateCommand KeyCommand_SpeedUp { get; private set; }
        public DelegateCommand KeyCommand_SlowDown { get; private set; }
        #endregion

        #region Game events
        public event EventHandler? KeyEvent_PauseResume;
        public event EventHandler? KeyEvent_MoveLeft;
        public event EventHandler? KeyEvent_MoveRight;
        public event EventHandler? KeyEvent_SpeedUp;
        public event EventHandler? KeyEvent_SlowDown;
        #endregion

        #region Constructor
        public RaceBikeViewModel(RaceBikeModel model)
        {
            _model = model;
            _model.GameContinues += Model_GameContinues;
            _model.CoordinateChanged += Model_CoordinateChanged;

            KeyCommand_PauseResume = new DelegateCommand(param => OnPauseResume());
            KeyCommand_MoveLeft = new DelegateCommand(param => OnMoveLeft());
            KeyCommand_MoveRight = new DelegateCommand(param => OnMoveRight());
            KeyCommand_SpeedUp = new DelegateCommand(param => OnSpeedUp());
            KeyCommand_SlowDown = new DelegateCommand(param => OnSlowDown());

            Coordinates = new();
            TableInit();
        }
        #endregion

        #region Public methods
        public void ResumeGame()
        {
            _model.GameTimeResume();
        }

        public void PauseGame()
        {
            _model.GameTimePause();
        }

        public void Reset()
        {
            _model.Reset();
            TableInit();
            OnPropertyChanged(nameof(Coordinates));
        }

        public void GameTimeElapsing()
        {
            _model.GameTimeElapsing();

            OnPropertyChanged(nameof(LatestBestTime));
            OnPropertyChanged(nameof(CurrentTime));
            OnPropertyChanged(nameof(CurrentTankLevel));
            OnPropertyChanged(nameof(CurrentSpeed));

            OnPropertyChanged(nameof(Coordinates));
        }

        public void RefreshTable() => TableInit();
        #endregion

        private void TableInit()
        {
            Coordinates.Clear();

            for (int i = 0; i < CourseWidth; i++)
            {
                for (int j = 0; j < CourseHeight; j++)
                {
                    Coordinates.Add(new SimplePoint(_model.Course[i, j], i, j));
                    OnPropertyChanged(nameof(Coordinates));
                }
            }
        }

        #region Private model event handlers
        private void Model_CoordinateChanged(object? sender, CoordinateEventArgs e)
        {
            Coordinates.First(f => f.X == e.TypeChanged.X && f.Y == e.TypeChanged.Y).Field = e.TypeChanged.Field;
            OnPropertyChanged(nameof(Coordinates));
        }
        private void Model_GameContinues(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(LatestBestTime));
            OnPropertyChanged(nameof(CurrentTime));
            OnPropertyChanged(nameof(CurrentTankLevel));
            OnPropertyChanged(nameof(CurrentSpeed));

            OnPropertyChanged(nameof(Coordinates));
        }
        #endregion

        #region Private game event handlers
        private void OnPauseResume()
        {
            KeyEvent_PauseResume?.Invoke(this, EventArgs.Empty);
        }

        private void OnMoveLeft()
        {
            KeyEvent_MoveLeft?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(Coordinates));
        }

        private void OnMoveRight()
        {
            KeyEvent_MoveRight?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(Coordinates));
        }

        private void OnSpeedUp()
        {
            KeyEvent_SpeedUp?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(CurrentSpeed));
        }

        private void OnSlowDown()
        {
            KeyEvent_SlowDown?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(CurrentSpeed));
        }
        #endregion
    }
}

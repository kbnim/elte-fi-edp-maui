using RaceBike.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RaceBike.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        #region Private fields
        private readonly RaceBikeModel _model;
        #endregion

        #region Properties (for the menu)
        private Visibility _menuVisibility;
        public Visibility MenuVisibility => _menuVisibility;

        private string _titleText;
        public string TitleText
        {
            get { return _titleText; }
            set
            {
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }

        private string _description01Text;
        public string Description01Text
        {
            get { return _description01Text; }
            set
            {
                _description01Text = value;
                OnPropertyChanged(nameof(Description01Text));
            }
        }

        private string _description02Text;
        public string Description02Text
        {
            get { return _description02Text; }
            set
            {
                _description02Text = value;
                OnPropertyChanged(nameof(Description02Text));
            }
        }

        private string _newResumeText;
        public string NewResumeText
        {
            get { return _newResumeText; }
            set
            {
                _newResumeText = value;
                OnPropertyChanged(nameof(NewResumeText));
            }
        }
        #endregion

        #region Menu commands
        public DelegateCommand ButtonCommand_NewResume { get; private set; }
        public DelegateCommand ButtonCommand_Load { get; private set; }
        public DelegateCommand ButtonCommand_Save { get; private set; }
        public DelegateCommand ButtonCommand_Help { get; private set; }
        #endregion

        #region Menu events
        public event EventHandler? ButtonEvent_NewResume;
        public event EventHandler? ButtonEvent_Load;
        public event EventHandler? ButtonEvent_Save;
        public event EventHandler? ButtonEvent_Help;
        #endregion

        #region Constructor
        public MenuViewModel(RaceBikeModel model)
        {
            _model = model;
            _model.GameOnPause += Model_GameOnPause;

            _menuVisibility = Visibility.Visible;
            _titleText = string.Empty;
            _description01Text = string.Empty;
            _description02Text = string.Empty;
            _newResumeText = string.Empty;

            MenuSetup_FirstLaunch();

            ButtonCommand_NewResume = new DelegateCommand(param => OnNewResume());
            ButtonCommand_Load = new DelegateCommand(param => OnLoad());
            ButtonCommand_Save = new DelegateCommand(param => OnSave());
            ButtonCommand_Help = new DelegateCommand(param => OnHelp());
        }

        private void Model_GameOnPause(object? sender, GameStatsEventArgs e)
        {
            TitleText = e.Title;
            Description01Text = e.Description01;
            Description02Text = e.Description02;
            NewResumeText = e.NewButtonText;
            RefreshProperties();
        }
        #endregion

        #region Public methods
        public void ChangeVisibility()
        {
            if (_model.IsPaused)
            {
                _menuVisibility = Visibility.Visible;
            }
            else
            {
                _menuVisibility = Visibility.Hidden;
            }
        }

        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(TitleText));
            OnPropertyChanged(nameof(Description01Text));
            OnPropertyChanged(nameof(Description02Text));
            OnPropertyChanged(nameof(NewResumeText));
        }
        private void MenuSetup_FirstLaunch()
        {
            _menuVisibility = Visibility.Visible;
            _titleText = "RaceBike 2000";
            _description01Text = "Press 'space' or";
            _description02Text = "click to start";
            _newResumeText = "New";
            RefreshProperties();
        }

        public void MenuSetup_Paused()
        {
            _description01Text = "Time: " + _model.CurrentTime.ToString("mm\\:ss\\.ff");
            _description02Text = "Best: " + _model.RecordTime.ToString("mm\\:ss\\.ff");
            _newResumeText = "Resume";
            RefreshProperties();
        }
        #endregion

        #region Private menu event handlers
        private void OnNewResume()
        {
            ButtonEvent_NewResume?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoad()
        {
            ButtonEvent_Load?.Invoke(this, EventArgs.Empty);
            // MenuSetup_FileLoaded();
        }

        private void OnSave()
        {
            ButtonEvent_Save?.Invoke(this, EventArgs.Empty);
        }

        private void OnHelp()
        {
            ButtonEvent_Help?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}

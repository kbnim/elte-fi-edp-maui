using RaceBike.Model;
using RaceBike.Store;
using RaceBike.ViewModel;
using RaceBike.View;

namespace RaceBike
{
    public partial class AppShell : Shell
    {
        private readonly RaceBikeModel _model;
        private readonly IStore _store;
        private readonly StoredGameBrowserModel _storedGameBrowserModel;

        private readonly RaceBikeViewModel _gameViewModel;
        private readonly MenuViewModel _menuViewModel;
        private readonly StoredGameBrowserViewModel _storedGameBrowserViewModel;

        private readonly IDispatcherTimer _gameTimer;
        private readonly IDispatcherTimer _fuelTimer;

        private readonly GamePage _gamePage;
        private readonly LoadPage _loadPage;
        private readonly SavePage _savePage;

        public AppShell(RaceBikeModel model, IStore store, RaceBikeViewModel gameViewModel, MenuViewModel menuViewModel)
        {
            InitializeComponent();

            _store = store;
            _model = model;
            _model.GameOver += Model_GameOver;
            _model.SpeedChanged += Model_SpeedChanged;

            _gameViewModel = gameViewModel;
            _gameViewModel.KeyEvent_MoveLeft += GameViewModel_MoveLeft;
            _gameViewModel.KeyEvent_MoveRight += GameViewModel_MoveRight;
            _gameViewModel.KeyEvent_PauseResume += GameViewModel_PauseResume;
            _gameViewModel.KeyEvent_SpeedUp += GameViewModel_SpeedUp;
            _gameViewModel.KeyEvent_SlowDown += GameViewModel_SlowDown;

            _menuViewModel = menuViewModel;
            _menuViewModel.ButtonEvent_NewResume += MenuViewModel_NewResume;
            _menuViewModel.ButtonEvent_Load += MenuViewModel_Load;
            _menuViewModel.ButtonEvent_Save += MenuViewModel_Save;
            _menuViewModel.ButtonEvent_Help += MenuViewModel_Help;

            _gameTimer = Dispatcher.CreateTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(300);
            _gameTimer.Tick += GameTimer_Tick;

            _fuelTimer = Dispatcher.CreateTimer();
            _fuelTimer.Interval = TimeSpan.FromSeconds(2);
            _fuelTimer.Tick += FuelTimer_Tick;

            _gamePage = new GamePage(gameViewModel);

            _storedGameBrowserModel = new StoredGameBrowserModel(_store);
            _storedGameBrowserViewModel = new StoredGameBrowserViewModel(_storedGameBrowserModel);
            _storedGameBrowserViewModel.GameLoading += StoredGameBrowserViewModel_GameLoading;
            _storedGameBrowserViewModel.GameSaving += StoredGameBrowserViewModel_GameSaving;

            _loadPage = new LoadPage
            {
                BindingContext = _storedGameBrowserViewModel
            };

            _savePage = new SavePage
            {
                BindingContext = _storedGameBrowserViewModel
            };
        }

        private void Model_SpeedChanged(object? sender, SpeedEventArgs e)
        {
            // StopTimers();
            // _model.GameTimePause();

            switch (e.Speed.ToString())
            {
                case "Slow": _gameTimer.Interval = TimeSpan.FromMilliseconds(300); break;
                case "Medium": _gameTimer.Interval = TimeSpan.FromMilliseconds(200); break;
                case "Fast": _gameTimer.Interval = TimeSpan.FromMilliseconds(100); break;
                default: break;
            }

            // StartTimers();
            // _model.GameTimeResume();
        }

        private void StartTimers()
        {
            _gameTimer.Start();
            _fuelTimer.Start();
        }

        private void StopTimers()
        {
            _gameTimer.Stop();
            _fuelTimer.Stop();
        }

        private async void StoredGameBrowserViewModel_GameSaving(object? sender, string e)
        {
            await Navigation.PopAsync();
            StopTimers();

            try
            {
                _model.SaveGame(e);
                await DisplayAlert("RaceBike 2000", "File has been successfully saved. :)", "OK");
            }
            catch
            {
                await DisplayAlert("RaceBike 2000", "Saving file failed awfully. :(", "OK");
            }
        }

        private async void StoredGameBrowserViewModel_GameLoading(object? sender, string e)
        {
            await Navigation.PopAsync();
            try
            {
                await _model.LoadGameAsync(e);
                await Navigation.PopAsync();
                await DisplayAlert("RaceBike 2000", "File has been successfully loaded. :)", "OK");

                // StartTimers();
            }
            catch
            {
                await DisplayAlert("RaceBike 2000", "Loading file failed awfully. :(", "OK");
            }
        }

        #region Timer events
        private void FuelTimer_Tick(object? sender, EventArgs e)
        {
            _model.GenerateNewFuelItem();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            _gameViewModel.GameTimeElapsing();
            // _gamePage.UpdateAbsoluteLayout();
        }
        #endregion

        #region Menu events
        private void MenuViewModel_Help(object? sender, EventArgs e)
        {
            string message = "A simple racing game. Your objective is to play for the " +
                             "longest time possible at the highest speed without running out of fuel.";
            DisplayAlert("RaceBike 2000", message, "OK");
        }

        private async void MenuViewModel_Save(object? sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync();
            await Navigation.PushAsync(_savePage);
        }

        private async void MenuViewModel_Load(object? sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync();
            await Navigation.PushAsync(_loadPage);
            _gameViewModel.RefreshTable();
        }

        private void MenuViewModel_NewResume(object? sender, EventArgs e)
        {
            if (_model.IsPaused)
            {
                _gameViewModel.RefreshTable();

                if (_model.IsGameOver)
                {
                    // _menuViewModel.MenuSetup_GameOver();
                    // _menuViewModel.NewResumeText = "Resume";
                    _gameViewModel.Reset();
                    StartTimers();
                    _model.GameTimeResume();
                }
                else
                {
                    // _menuViewModel
                    // _menuViewModel.TitleText = "RaceBike 2000";
                    // _menuViewModel.NewResumeText = "Resume";
                    StartTimers();
                    _model.GameTimeResume();
                }

                Navigation.PushAsync(_gamePage);
            }
        }
        #endregion

        #region Game events
        private void GameViewModel_SlowDown(object? sender, EventArgs e)
        {
            //StopTimers();
            //_model.GameTimePause();
            _model.SlowDown();
            //StartTimers();
            //_model.GameTimeResume();
        }

        private void GameViewModel_SpeedUp(object? sender, EventArgs e)
        {
            //StopTimers();
            //_model.GameTimePause();
            _model.SpeedUp();
            //StartTimers();
            //_model.GameTimeResume();
        }

        private void GameViewModel_PauseResume(object? sender, EventArgs e)
        {
            if (!_model.IsPaused && !_model.IsGameOver)
            {
                StopTimers();
                _model.GameTimePause();
                Navigation.PopAsync();
            }
        }

        private void GameViewModel_MoveRight(object? sender, EventArgs e)
        {
            if (!(_model.IsPaused || _model.IsGameOver)) _model.MoveRight();
        }

        private void GameViewModel_MoveLeft(object? sender, EventArgs e)
        {
            if (!(_model.IsPaused || _model.IsGameOver)) _model.MoveLeft();
        }
        #endregion

        #region Model events
        private void Model_GameOver(object? sender, GameStatsEventArgs e)
        {
            StopTimers();
            _model.GameTimePause();

            _menuViewModel.TitleText = e.Title;
            _menuViewModel.Description01Text = e.Description01;
            _menuViewModel.Description02Text = e.Description02;
            _menuViewModel.NewResumeText = e.NewButtonText;

            // _menuViewModel.MenuSetup_GameOver();
            Navigation.PopAsync();
        }
        #endregion
    }
}

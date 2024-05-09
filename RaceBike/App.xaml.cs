using RaceBike.Persistence;
using RaceBike.Model;
using RaceBike.Store;
using RaceBike.ViewModel;

namespace RaceBike
{
    public partial class App : Application
    {
        private const string SuspendedGameSavePath = "SuspendedGame";

        #region Fields
        private readonly AppShell _appShell;
        private Window? _window = null;

        private readonly IRaceBikeDataAccess _dataAccess;
        private readonly RaceBikeModel _model;
        private readonly IStore _store;
        private readonly RaceBikeViewModel _gameViewModel;
        private readonly MenuViewModel _menuViewModel;
        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();

            _store = new RaceBikeStore();
            _dataAccess = new RaceBikeTxtAccess(FileSystem.AppDataDirectory);
            _model = new RaceBikeModel(_dataAccess);
            _gameViewModel = new RaceBikeViewModel(_model);
            _menuViewModel = new MenuViewModel(_model);

            _appShell = new AppShell(_model, _store, _gameViewModel, _menuViewModel)
            {
                BindingContext = _menuViewModel
            };

            MainPage = _appShell;
        }
        #endregion

        #region Application life-cycle methods
        protected override Window CreateWindow(IActivationState? activationState)
        {
            _window = base.CreateWindow(activationState);

            _window.Created += async (s, e) =>
            {
                try
                {
                    await _model.LoadGameAsync(
                        Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath));
                }
                catch { }
            };

            _window.Resumed += async (s, e) =>
            {
                try
                {
                    await _model.LoadGameAsync(
                        Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath));
                }
                catch { }
            };

            _window.Stopped += (s, e) =>
            {
                try
                {
                    _model.SaveGame(
                        Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath));
                }
                catch { }
            };

            return _window;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Store;
using RaceBike.Model;

namespace RaceBike.ViewModel
{
    public class StoredGameBrowserViewModel : ViewModelBase
    {
        private readonly StoredGameBrowserModel _model;

        public event EventHandler<string>? GameLoading;
        public event EventHandler<string>? GameSaving;
        public DelegateCommand NewSaveCommand { get; private set; }
        public ObservableCollection<StoredGameViewModel> StoredGames { get; private set; }

        public StoredGameBrowserViewModel(StoredGameBrowserModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _model.StoreChanged += new EventHandler(Model_StoreChanged);

            NewSaveCommand = new DelegateCommand(param =>
            {
                string? fileName = Path.GetFileNameWithoutExtension(param?.ToString()?.Trim());
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName += ".txt";
                    OnGameSaving(fileName);
                }
            });
            StoredGames = new();
            UpdateStoredGames();
        }

        private void UpdateStoredGames()
        {
            StoredGames.Clear();

            foreach (StoredGameModel item in _model.StoredGames)
            {
                StoredGames.Add(new StoredGameViewModel
                {
                    Name = item.Name,
                    Modified = item.Modified,
                    LoadGameCommand = new DelegateCommand(param => OnGameLoading(param?.ToString() ?? "")),
                    SaveGameCommand = new DelegateCommand(param => OnGameSaving(param?.ToString() ?? ""))
                });
            }
        }

        private void Model_StoreChanged(object? sender, EventArgs e)
        {
            UpdateStoredGames();
        }

        private void OnGameLoading(string name)
        {
            GameLoading?.Invoke(this, name);
        }

        private void OnGameSaving(string name)
        {
            GameSaving?.Invoke(this, name);
        }
    }
}

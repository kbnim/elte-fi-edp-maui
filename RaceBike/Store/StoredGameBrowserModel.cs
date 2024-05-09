using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Store;

namespace RaceBike.Model
{
    public class StoredGameBrowserModel
    {
        private readonly IStore _store;

        public event EventHandler? StoreChanged;

        public List<StoredGameModel> StoredGames { get; private set; } = new();

        public StoredGameBrowserModel(IStore store)
        {
            _store = store;
        }

        public async Task UpdateAsync()
        {
            if (_store == null) return;

            StoredGames.Clear();

            foreach (string name in await _store.GetFilesAsync())
            {
                if (name == "SuspendedGame")
                    continue;

                StoredGames.Add(new StoredGameModel
                {
                    Name = name,
                    Modified = await _store.GetModifiedTimeAsync(name)
                });
            }

            StoredGames = StoredGames.OrderByDescending(item => item.Modified).ToList();

            OnSavesChanged();
        }

        private void OnSavesChanged()
        {
            StoreChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

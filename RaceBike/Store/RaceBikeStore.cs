using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Store
{
    public class RaceBikeStore : IStore
    {
        #region Methods
        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            return await Task.Run(() => Directory.GetFiles(FileSystem.AppDataDirectory)
                .Select(Path.GetFileName)
                .Where(name => name?.EndsWith(".txt") ?? false)
                .OfType<string>());
        }

        public async Task<DateTime> GetModifiedTimeAsync(string name)
        {
            var info = new FileInfo(Path.Combine(FileSystem.AppDataDirectory, name));

            return await Task.Run(() => info.LastWriteTime);
        }
        #endregion
    }
}

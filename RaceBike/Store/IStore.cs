using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Store
{
    public interface IStore
    {
        #region Methods
        Task<IEnumerable<string>> GetFilesAsync();
        Task<DateTime> GetModifiedTimeAsync(string name);
        #endregion
    }
}

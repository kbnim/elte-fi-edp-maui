using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using RaceBike.Model.Classes;

namespace RaceBike.Persistence
{
    public class RaceBikeTxtAccess : IRaceBikeDataAccess
    {
        private readonly string? _directory;

        public RaceBikeTxtAccess(string? directory = null)
        {
            _directory = directory;
        }

        #region Public methods
        public async Task<RaceBikeFile> LoadAsync(string path)
        {
            if (!string.IsNullOrEmpty(_directory))
            {
                path = Path.Combine(_directory, path);
            }  

            try
            {
                using StreamReader reader = new(path); // fájl megnyitása
                
                List<string> lines = new();

                while (!reader.EndOfStream)
                {
                    lines.Add(await reader.ReadLineAsync() ?? string.Empty);
                }

                return RaceBikeFile.Parse(lines);
            }
            catch (Exception ex)
            {
                throw new RaceBikeDataException(ex.Message, ex);
            }
        }

        public async Task SaveAsync(string path, RaceBikeFile fileContents)
        {
            if (!string.IsNullOrEmpty(_directory))
            {
                path = Path.Combine(_directory, path);
            }

            try
            {
                using StreamWriter writer = new(path);
                await writer.WriteLineAsync(fileContents.LatestBestTime.ToString());
                await writer.WriteLineAsync(fileContents.Speed.ToString());

                foreach (GameField field in fileContents.Entities)
                {
                    await writer.WriteLineAsync(field.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new RaceBikeDataException(ex.Message, ex);
            }
        }
        #endregion
    }
}

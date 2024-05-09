using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.ViewModel
{
    public class StoredGameViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        private DateTime _modified;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Modified
        {
            get { return _modified; }
            set
            {
                if (_modified != value)
                {
                    _modified = value;
                    OnPropertyChanged();
                }
            }
        }
        public DelegateCommand? LoadGameCommand { get; set; }

        public DelegateCommand? SaveGameCommand { get; set; }
    }
}

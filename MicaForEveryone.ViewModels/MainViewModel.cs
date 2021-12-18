using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MicaForEveryone.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private bool _systemBackdropIsSupported;

        public bool SystemBackdropIsSupported
        {
            get => _systemBackdropIsSupported;
            set => SetProperty(ref _systemBackdropIsSupported, value);
        }

        public ICommand Exit { get; set; }
    }
}

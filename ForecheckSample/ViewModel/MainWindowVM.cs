using ForecheckSample.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForecheckSample.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OpenVideoFileCommand
        {
            get { return new RelayCommand((object obj) => OpenVideoFile()); }
        }

        private void OpenVideoFile()
        {
            var mDialogService = new DefaultDialogService();
            if (mDialogService.OpenFileDialog())
            {

            }
        }
    }
}

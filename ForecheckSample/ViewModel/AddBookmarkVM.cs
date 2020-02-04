using ForecheckSample.Model;
using ForecheckSample.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ForecheckSample.ViewModel
{
    public class AddBookmarkVM : INotifyPropertyChanged
    {
        public BitmapSource Frame { get; set; }
        public string Description { get; set; }
        public int FrameCount { get; set; }
        public bool NeedSaveBookmark { get; set; } = false;
        public Bookmark Bookmark { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void CloseBookmarkEventHandler(object sender);
        public event CloseBookmarkEventHandler BookmarkVMClosed;

        public AddBookmarkVM(Bookmark bookmark)
        {
            Frame = bookmark.GetFrame();
            this.Bookmark = bookmark;
            this.FrameCount = bookmark.FrameCount;
        }

        public ICommand SaveBookmarkCommand
        {
            get { return new RelayCommand((object obj) => SaveBookmark()); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand((object obj) => Cancel()); }
        }

        private void SaveBookmark()
        {
            Bookmark.Description = Description;
            NeedSaveBookmark = true;
            BookmarkVMClosed(this);
        }

        private void Cancel()
        {
            BookmarkVMClosed(this);
        }
    }
}

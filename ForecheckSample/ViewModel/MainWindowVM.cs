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
using System.Windows.Threading;

namespace ForecheckSample.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private FrameProvider frameProvider = new FrameProvider();
        private bool needToPlayVideo = false;
        private DispatcherTimer timer;

        public event PropertyChangedEventHandler PropertyChanged;
        public BitmapSource Video { get; private set; }
        public bool IsVideoOpen { get; set; } = false;
        public int MaxFrameCount { get; set; } = 0;
        public int CurrentFrameCount
        {
            get => frameProvider.CurrentFrameCount;
            set
            {
                if (value >= 0 && value < MaxFrameCount)
                {
                    CurrentFrameCount = value;
                    SetNewFramePosition(value);
                }
            }
        }

        public ICommand OpenVideoFileCommand
        {
            get { return new RelayCommand((object obj) => OpenVideoFile()); }
        }

        public ICommand PlayVideoCommand
        {
            get { return new RelayCommand((object obj) => PlayVideo()); }
        }

        private void OpenVideoFile()
        {
            var DialogService = new DefaultDialogService();
            if (DialogService.OpenFileDialog())
            {
                frameProvider.DefineVideoSource(DialogService.FilePath);
                if (frameProvider.IsOpened)
                {
                    IsVideoOpen = true;
                    MaxFrameCount = frameProvider.MaxFrameCount;
                    CurrentFrameCount = frameProvider.CurrentFrameCount;
                    Video = frameProvider.GetInitialFrame();
                }
            }
        }

        private void PlayVideo()
        {
            if (!IsVideoOpen)
                return;
            needToPlayVideo = true;

            timer = new DispatcherTimer();
            timer.Tick += TimerTick; ;
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var frame = frameProvider.GetNextFrame();
            if (frame == null)
                StopTimer();

            Video = frame;
            CurrentFrameCount = frameProvider.CurrentFrameCount;
            if (CurrentFrameCount + 1 >= MaxFrameCount)
                StopTimer();
        }

        private void SetNewFramePosition(int position)
        {
            frameProvider.SetNewVideoPosition(position);
        }

        private void StopTimer()
        {
            timer.Stop();
            needToPlayVideo = false;
        }
    }
}

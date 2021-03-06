﻿using ForecheckSample.Model;
using ForecheckSample.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ForecheckSample.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private FrameProvider frameProvider = new FrameProvider();        
        private DispatcherTimer timer;
        private bool isTimerInitialized = false;
        private int currentFrameCount = 0;
        private int selectedBookmarkIndex = -1;
        private double frameDelay = 1;

        public event PropertyChangedEventHandler PropertyChanged;
        public BitmapSource Video { get; private set; }
        public bool IsVideoOpen { get; set; } = false;
        public bool NeedToPlayVideo { get; set; } = false;
        public bool IsBookmarkDialogOpen { get; set; } = false;
        public int MaxFrameCount { get; set; } = 0;
        public AddBookmarkVM DialogVM { get; set; }
        public int CurrentFrameCount
        {
            get => currentFrameCount;
            set
            {
                if (value >= 0 && value < MaxFrameCount)
                {
                    currentFrameCount = value;
                    SetNewFramePosition(value);
                }
            }
        }

        public ObservableCollection<Bookmark> Bookmarks { get; set; } = new ObservableCollection<Bookmark>();
        public int SelectedBookmarkIndex
        {
            get { return selectedBookmarkIndex; }
            set
            {
                if (value >= 0 && value < Bookmarks.Count)
                {
                    selectedBookmarkIndex = value;
                    MoveToSelectedFrame();
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

        public ICommand AddBookmarkCommand
        {
            get { return new RelayCommand((object obj) => AddBookmark()); }
        }

        public ICommand GetNextFrameCommand
        {
            get { return new RelayCommand((object obj) => GetNextFrame()); }
        }

        public ICommand GetPreviousFrameCommand
        {
            get { return new RelayCommand((object obj) => GetPreviousFrame()); }
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
                    currentFrameCount = frameProvider.CurrentFrameCount;
                    Video = frameProvider.GetInitialFrame();
                    Bookmarks = new ObservableCollection<Bookmark>();
                    frameDelay = 1000.0/frameProvider.Fps - 22; //Opencv reads and other processing per each frame at average delay is 22ms 
                }
            }
        }

        private void PlayVideo()
        {
            if (!IsVideoOpen)
                return;

            if (NeedToPlayVideo)
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }            
        }

        private void GrabFrame(BitmapSource frame)
        {
            Video = frame;
            currentFrameCount = frameProvider.CurrentFrameCount;
            OnPropertyChanged("CurrentFrameCount");
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var frame = frameProvider.GetNextFrame();
            if (frame == null)
                StopTimer();

            GrabFrame(frame);            
            if (currentFrameCount >= MaxFrameCount)
                StopTimer();
        }

        private void SetNewFramePosition(int position)
        {
            frameProvider.SetNewVideoPosition(position);
            var frame = frameProvider.GetNextFrame();
            Video = frame;
        }

        private void StopTimer()
        {
            if (NeedToPlayVideo)
            {
                timer.Stop();
                NeedToPlayVideo = false;
            }
        }

        private void StartTimer()
        {
            if (!NeedToPlayVideo)
            {
                if (!isTimerInitialized)
                    InitializeTimer();

                timer.Start();
                NeedToPlayVideo = true;
            }
        }

        private void InitializeTimer()
        {
            if (!isTimerInitialized)
            {
                timer = new DispatcherTimer();
                timer.Tick += TimerTick; 
                timer.Interval = TimeSpan.FromMilliseconds(frameDelay);
                isTimerInitialized = true;
            }
        }

        private void AddBookmark()
        {
            StopTimer();
            Bookmark bm = new Bookmark(frameProvider.CurrentFrame, currentFrameCount, "");
            DialogVM = new AddBookmarkVM(bm);
            DialogVM.BookmarkVMClosed += DialogVMBookmarVMClosed;
            IsBookmarkDialogOpen = true;
        }

        private void DialogVMBookmarVMClosed(object sender)
        {
            var bmvm = sender as AddBookmarkVM;
            bool needSave = bmvm.NeedSaveBookmark;
            if (needSave)
            {
                Bookmarks.Add(bmvm.Bookmark);
            }
            IsBookmarkDialogOpen = false;
        }

        private void GetNextFrame()
        {
            var frame = frameProvider.GetNextFrame();
            if (frame == null)
                return;

            GrabFrame(frame);
        }

        private void GetPreviousFrame()
        {
            var frame = frameProvider.GetPreviousFrame();
            if (frame == null)
                return;

            GrabFrame(frame);
        }

        private void MoveToSelectedFrame()
        {
            int index = Bookmarks[selectedBookmarkIndex].FrameCount;
            frameProvider.SetNewVideoPosition(index);
            var frame = frameProvider.GetCurrentFrame();
            if (frame == null)
                return;

            GrabFrame(frame);
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

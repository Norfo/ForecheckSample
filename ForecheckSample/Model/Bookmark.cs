using OpenCvSharp;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ForecheckSample.Model
{
    public class Bookmark : INotifyPropertyChanged
    {
        public Mat Frame { get; set; }
        public int FrameCount { get; set; }
        public string Description { get; set; } = "";
        public BitmapSource FrameBm { get { return GetFrame(); } }

        public Bookmark(Mat frame, int count, string description)
        {
            Frame = frame.Clone();
            FrameCount = count;
            Description = description;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSource GetFrame()
        {
            if (Frame == null)
                return null;
            var bmpS = BitmapSource.Create(Frame.Cols, Frame.Rows, 100, 100, PixelFormats.Bgr24, BitmapPalettes.Halftone256, Frame.Data, Frame.Cols * Frame.Rows * 3, Frame.Cols * 3);
            GC.Collect();
            return bmpS;
        }
    }
}

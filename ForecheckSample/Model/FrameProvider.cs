using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace ForecheckSample.Model
{
    public class FrameProvider : INotifyPropertyChanged
    {
        private string sourceVideoPath = "";
        private VideoCapture videoCapture;

        public bool IsOpened { get; private set; } = false;
        public int MaxFrameCount { get; private set; } = 0;
        public int CurrentFrameCount { get; private set; } = 0;
        public double Fps { get; private set; } = 0;
        public Mat CurrentFrame { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void DefineVideoSource(string sourcePath)
        {
            sourceVideoPath = sourcePath;
            videoCapture = new VideoCapture(sourcePath);

            IsOpened = videoCapture.IsOpened();
            if (IsOpened)
            {
                MaxFrameCount = videoCapture.FrameCount;
                CurrentFrameCount = 0;
                Fps = videoCapture.Fps;
            }
        }

        public BitmapSource GetInitialFrame()
        {
            Mat frame = new Mat();
            videoCapture.Read(frame);
            CurrentFrameCount = videoCapture.PosFrames;
            CurrentFrame = frame.Clone();
            return ConvertMatToBitmapSource(frame);
        }

        public BitmapSource GetNextFrame()
        {
            Mat frame = new Mat();
            if (videoCapture.PosFrames < videoCapture.FrameCount)
            {
                CurrentFrameCount = videoCapture.PosFrames;
                videoCapture.Read(frame);
                CurrentFrameCount = videoCapture.PosFrames;
                CurrentFrame = frame.Clone();
                return ConvertMatToBitmapSource(frame);
            }

            return null;
        }

        public BitmapSource GetCurrentFrame()
        {
            Mat frame = new Mat();
            if (videoCapture.PosFrames - 1 >= 0)
            {
                int prevF = videoCapture.PosFrames - 1;
                SetNewVideoPosition(prevF);
                CurrentFrameCount = videoCapture.PosFrames;
                videoCapture.Read(frame);
                CurrentFrameCount = videoCapture.PosFrames;
                CurrentFrame = frame.Clone();
                return ConvertMatToBitmapSource(frame);
            }

            return null;
        }

        public BitmapSource GetPreviousFrame()
        {
            Mat frame = new Mat();
            if (videoCapture.PosFrames - 1 >= 0)
            {
                int prevF = videoCapture.PosFrames - 2;
                SetNewVideoPosition(prevF);
                CurrentFrameCount = videoCapture.PosFrames;
                videoCapture.Read(frame);
                CurrentFrameCount = videoCapture.PosFrames;
                CurrentFrame = frame.Clone();
                return ConvertMatToBitmapSource(frame);
            }

            return null;
        }

        private BitmapSource ConvertMatToBitmapSource(Mat image)
        {
            var bmpS = BitmapSource.Create(image.Cols, image.Rows, 100, 100, PixelFormats.Bgr24, BitmapPalettes.Halftone256, image.Data, image.Cols * image.Rows * 3, image.Cols * 3);
            GC.Collect();

            return bmpS;
        }

        public void SetNewVideoPosition(int newPosition)
        {            
            CurrentFrameCount = newPosition;
            videoCapture.Set(CaptureProperty.PosFrames, newPosition);
        }
    }
}

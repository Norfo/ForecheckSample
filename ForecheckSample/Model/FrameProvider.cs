using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace ForecheckSample.Model
{
    public class FrameProvider
    {
        private string sourceVideoPath = "";
        private VideoCapture videoCapture;

        public void DefineVideoSource(string sourcePath)
        {
            sourceVideoPath = sourcePath;
            videoCapture = new VideoCapture(sourcePath);

            if (!videoCapture.IsOpened())
            {

            }
        }
    }
}

using System;
using Reviewer.Core;
using System.IO;
using AVFoundation;
using Foundation;
using System.Threading.Tasks;

namespace Reviewer.iOS
{
    public class VideoConverter : IVideoConversion
    {
        public async Task<Stream> ConvertToMP4(string videoLocation)
        {
            string finishedPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string finishedFilePath = Path.Combine(finishedPath, $"{Guid.NewGuid()}.mp4");

            var asset = AVAsset.FromUrl(NSUrl.FromFilename(videoLocation));

            AVAssetExportSession exportSession = new AVAssetExportSession(asset, AVAssetExportSession.PresetLowQuality);
            exportSession.OutputUrl = NSUrl.FromFilename(finishedFilePath);
            exportSession.OutputFileType = AVFileType.Mpeg4;
            exportSession.ShouldOptimizeForNetworkUse = true;

            await exportSession.ExportTaskAsync();

            return File.Open(finishedFilePath, FileMode.Open);
        }
    }
}

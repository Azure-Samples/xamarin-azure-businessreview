using System;
using Microsoft.WindowsAzure.Storage.Core.Util;
using Xamarin.Forms;
namespace Reviewer.Core
{
    public class UploadProgress : IProgress<StorageProgress>
    {
        public event EventHandler<double> Updated;

        public double TotalImageBytes { get; set; }

        void IProgress<StorageProgress>.Report(StorageProgress value)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Math.Abs(TotalImageBytes) < 0)
                    return;

                double updatePercentage = (double)value.BytesTransferred;

                Updated?.Invoke(this, updatePercentage);
            });
        }
    }
}

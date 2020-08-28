using System;
using Xamarin.Forms;
namespace Reviewer.Core
{
    public class UploadProgress : IProgress<long>
    {
        public event EventHandler<double> Updated;

        public double TotalImageBytes { get; set; }

        void IProgress<long>.Report(long value)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Math.Abs(TotalImageBytes) < 0)
                    return;

                double updatePercentage = (double)value;

                Updated?.Invoke(this, updatePercentage);
            });
        }
    }
}

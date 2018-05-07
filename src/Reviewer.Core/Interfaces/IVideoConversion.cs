using System;
using System.Threading.Tasks;
using System.IO;

namespace Reviewer.Core
{
    public interface IVideoConversion
    {
        Task<Stream> ConvertToMP4(string videoLocation);
    }
}

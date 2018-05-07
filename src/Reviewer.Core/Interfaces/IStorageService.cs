using System;
using System.IO;
using System.Threading.Tasks;
namespace Reviewer.Core
{
    public interface IStorageService
    {
        Task<Uri> UploadBlob(Stream blobContent, bool isVideo, string reviewId, UploadProgress progressUpdater);
    }
}

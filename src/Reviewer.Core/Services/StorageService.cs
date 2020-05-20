using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MonkeyCache.FileStore;
using Xamarin.Forms;
using Reviewer.Services;
using System.Threading;
using System.Diagnostics;
using Azure.Storage.Blobs;
using System.Collections.Generic;

namespace Reviewer.Core
{
    public class StorageService : IStorageService
    {
        enum StoragePermissionType
        {
            List,
            Read,
            Write
        }

        public async Task<Uri> UploadBlob(Stream blobContent, bool isVideo, string reviewId, UploadProgress progressUpdater)
        {
            Uri blobAddress = null;
            try
            {
                var writeCredentials = await ObtainStorageCredentials(StoragePermissionType.Write);

                var sasUri = $"https://{APIKeys.StorageAccountName}.blob.core.windows.net/" + writeCredentials;
                var blobServiceClient = new BlobServiceClient(new Uri(sasUri));

                var container = blobServiceClient.GetBlobContainerClient(APIKeys.PhotosContainerName);

                var extension = isVideo ? "mp4" : "png";
                var blockBlob = container.GetBlobClient($"{Guid.NewGuid()}.{extension}");

                IDictionary<string, string> metadata = new Dictionary<string, string>();
                metadata.Add("reviewId", reviewId);
                blockBlob.SetMetadata(metadata);
                await blockBlob.UploadAsync(blobContent, null, null, null, progressUpdater);

                blobAddress = blockBlob.Uri;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** Error {ex.Message}");

                return null;
            }

            return blobAddress;
        }

        #region Helpers

        async Task<string> ObtainStorageCredentials(StoragePermissionType permissionType)
        {
            var functionService = DependencyService.Get<IAPIService>();

            var cacheKey = permissionType.ToString();

            if (Barrel.Current.Exists(cacheKey) && !Barrel.Current.IsExpired(cacheKey))
                return  Barrel.Current.Get<string>(cacheKey);

            string storageToken = null;
            switch (permissionType)
            {
                //case StoragePermissionType.List:
                //    storageToken = await functionService.GetContainerListSasToken().ConfigureAwait(false);
                //    break;
                //case StoragePermissionType.Read:
                //storageToken = await FunctionService.GetContainerReadSASToken().ConfigureAwait(false);
                //break;
                case StoragePermissionType.Write:
                    storageToken = await functionService.GetContainerWriteSasToken().ConfigureAwait(false);
                    break;
            }

            return storageToken == null ? null : StuffCredentialsInBarrel(storageToken, cacheKey);
        }

        TimeSpan GetExpirationSpan(string tokenQueryString)
        {
            // We'll need to parse the token query string
            // easiest way is to make it ino a URI and parse it with URI.ParseQueryString

            var fakeTokenUri = new Uri($"http://localhost{tokenQueryString}");
            var queryStringParts = fakeTokenUri.ParseQueryString();

            var endDateString = queryStringParts["se"];

            // Expire one minute before we really need to
            var endTimeSpan = DateTimeOffset.Parse(endDateString) - DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1);

            return endTimeSpan;
        }

        string StuffCredentialsInBarrel(string storageToken, string cacheKey)
        {
            var credentials = storageToken;
            var expireIn = GetExpirationSpan(storageToken);

            Barrel.Current.Add<string>(cacheKey, storageToken, expireIn);

            return credentials;
        }

        #endregion
    }
}

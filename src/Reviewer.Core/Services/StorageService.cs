using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using MonkeyCache.FileStore;
using Xamarin.Forms;
using Reviewer.Services;
using System.Threading;
using System.Diagnostics;

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

                var csa = new Microsoft.WindowsAzure.Storage.CloudStorageAccount(writeCredentials, APIKeys.StorageAccountName, APIKeys.StorageAccountUrlSuffix, true);

                var blobClient = csa.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(APIKeys.PhotosContainerName);

                var extension = isVideo ? "mp4" : "png";
                var blockBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}.{extension}");

                blockBlob.Metadata.Add("reviewId", reviewId);
                await blockBlob.UploadFromStreamAsync(blobContent, null, null, null, progressUpdater, new CancellationToken());

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

        async Task<StorageCredentials> ObtainStorageCredentials(StoragePermissionType permissionType)
        {
            var functionService = DependencyService.Get<IAPIService>();

            var cacheKey = permissionType.ToString();

            if (Barrel.Current.Exists(cacheKey) && !Barrel.Current.IsExpired(cacheKey))
                return new StorageCredentials(Barrel.Current.Get<string>(cacheKey));

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

        StorageCredentials StuffCredentialsInBarrel(string storageToken, string cacheKey)
        {
            var credentials = new StorageCredentials(storageToken);
            var expireIn = GetExpirationSpan(storageToken);

            Barrel.Current.Add<string>(cacheKey, storageToken, expireIn);

            return credentials;
        }

        #endregion
    }
}

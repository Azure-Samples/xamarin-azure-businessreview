using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;
using Newtonsoft.Json;
using Reviewer.Functions.Models;

namespace Reviewer.Functions
{
    public static class AMSPublish
    {
        #region Environment Variables

        static readonly string AADTenantDomain = Environment.GetEnvironmentVariable("AMSAADTenantDomain");
        static readonly string RESTAPIEndpoint = Environment.GetEnvironmentVariable("AMSRESTAPIEndpoint");
        static readonly string mediaServicesClientId = Environment.GetEnvironmentVariable("AMSClientId");
        static readonly string mediaServicesClientSecret = Environment.GetEnvironmentVariable("AMSClientSecret");

        static readonly string storageAccountName = Environment.GetEnvironmentVariable("MediaServicesStorageAccountName");
        static readonly string storageAccountKey = Environment.GetEnvironmentVariable("MediaServicesStorageAccountKey");

        #endregion

        private static CloudMediaContext context = null;

        internal const string SignatureHeaderKey = "sha256";

        internal const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";

        [FunctionName("AMSPublish")]
        [return: Queue("review-videos")]
        public static async Task<VideoQueueMessage> Run(
            [HttpTrigger(authLevel: AuthorizationLevel.Function, methods: "post")]HttpRequestMessage req,
            TraceWriter log)
        {
            try
            {
                log.Info($"Webhook was triggered!");

                string jsonContent = await req.Content.ReadAsStringAsync();

                log.Info($"Publish Message: {jsonContent}");

                var publishMsg = JsonConvert.DeserializeObject<VideoPublishMessage>(jsonContent);

                InitializeCloudMediaContext();

                IAsset asset = null;
                foreach (var item in context.Assets)
                {
                    if (item.Name == publishMsg.AssetName)
                        asset = item;

                    break;
                }

                if (asset == null)
                {
                    return null;
                }

                log.Info($"Found asset: {asset.Name}");

                var videoQueueMsg = new VideoQueueMessage();

                // Create the on demand
                await context.Locators.CreateAsync(LocatorType.OnDemandOrigin, asset, AccessPermissions.Read, TimeSpan.FromDays(365));

                videoQueueMsg.ReviewId = publishMsg.ReviewId;

                videoQueueMsg.SmoothStreamingUri = asset.GetSmoothStreamingUri().AbsoluteUri;
                videoQueueMsg.HLSUri = asset.GetHlsUri().AbsoluteUri;
                videoQueueMsg.MpegDashUri = asset.GetMpegDashUri().AbsoluteUri;

                // Create the progressive download
                await context.Locators.CreateAsync(LocatorType.Sas, asset, AccessPermissions.Read, TimeSpan.FromDays(365));

                var imageUri = asset.AssetFiles.ToList().Where(af => af.Name.EndsWith("png", StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.GetSasUri();

                videoQueueMsg.ThumbnailUri = imageUri?.AbsoluteUri ?? "";

                // Write the message to the queue
                return videoQueueMsg;
            }
            catch (Exception ex)
            {
                log.Error($"Error during publishing! {ex.Message}");
            }

            return null;
        }

        static void InitializeCloudMediaContext()
        {
            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(AADTenantDomain,
                               new AzureAdClientSymmetricKey(mediaServicesClientId, mediaServicesClientSecret),
                               AzureEnvironments.AzureCloudEnvironment);

            AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            context = new CloudMediaContext(new Uri(RESTAPIEndpoint), tokenProvider);
        }
    }
}

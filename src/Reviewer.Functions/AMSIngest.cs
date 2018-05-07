using System;

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Reviewer.Functions.Models;

using System.Net.Http;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Reviewer.Functions
{
    public static class AMSIngest
    {
        #region Environment Variables

        static readonly string AADTenantDomain = Environment.GetEnvironmentVariable("AMSAADTenantDomain");
        static readonly string RESTAPIEndpoint = Environment.GetEnvironmentVariable("AMSRESTAPIEndpoint");
        static readonly string mediaServicesClientId = Environment.GetEnvironmentVariable("AMSClientId");
        static readonly string mediaServicesClientSecret = Environment.GetEnvironmentVariable("AMSClientSecret");

        static readonly string storageAccountName = Environment.GetEnvironmentVariable("MediaServicesStorageAccountName");
        static readonly string storageAccountKey = Environment.GetEnvironmentVariable("MediaServicesStorageAccountKey");

        static readonly string webhookEndpoint = Environment.GetEnvironmentVariable("WebhookEndpoint");

        #endregion

        private static CloudMediaContext context = null;
        private static CloudStorageAccount destinationStorageAccount = null;

        [FunctionName("AMSIngest")]
        public static async Task Run(
            [BlobTrigger("review-photos/{fileName}.mp4", Connection = "AzureWebJobsStorage")]CloudBlockBlob inputBlob,
            string fileName,
            TraceWriter log)
        {
            try
            {
                fileName = $"{fileName}.mp4";

                log.Info($"C# Blob trigger function Processed blob\n Name:{fileName}");

                if (!inputBlob.Metadata.ContainsKey("reviewId"))
                {
                    log.Warning("No review id metadata!");
                    await inputBlob.DeleteIfExistsAsync();
                }

                string reviewId = inputBlob.Metadata["reviewId"];
                string assetName = inputBlob.Name;

                InitializeCloudMediaContext();

                // 1. Copy BLOB into Input Asset
                IAsset thumbAsset = await CreateAssetFromBlobAsync(inputBlob, $"thumb-{fileName}", log);

                log.Info("Deleting the source asset from the input container");
                await inputBlob.DeleteIfExistsAsync();

                // 2. Create an encoding job

                // Declare a new encoding job with the Standard encoder
                IJob job = context.Jobs.Create("Build Reviewer - Function Kickoff");

                // Get a media processor reference, and pass to it the name of the 
                // processor to use for the specific task.
                IMediaProcessor processor = GetLatestMediaProcessorByName("Media Encoder Standard");

                // 3. Create an encoding task

                // grab preset encoding info that will create a video & a thumbnail
                var thumbnailPreset = VideoEncodingPresetGenerator.Thumbnail().ToJson();

                // create a task that does the encoding
                ITask thumbnailTask = job.Tasks.AddNew("Thumbnail encode", processor, thumbnailPreset, TaskOptions.None);
                thumbnailTask.Priority = 100;

                // Make sure there's an input asset to encode
                thumbnailTask.InputAssets.Add(thumbAsset);

                var outAssetName = $"thumbnail-{fileName}";

                // Define the output asset
                thumbnailTask.OutputAssets.AddNew(outAssetName, AssetCreationOptions.None);

                // Submit the job
                job.Submit();
                log.Info("Job Submitted");

                // 3. Monitor the job
                while (true)
                {
                    job.Refresh();

                    // Refresh every 5 seconds
                    await Task.Delay(5000);

                    log.Info($"Job: {job.Id}    State: {job.State.ToString()}");

                    if (job.State == JobState.Error || job.State == JobState.Finished || job.State == JobState.Canceled)
                        break;
                }

                if (job.State == JobState.Finished)
                {
                    log.Info($"Job {job.Id} is complete.");

                    // Call a webhook
                    var msg = new VideoPublishMessage { AssetName = outAssetName, ReviewId = reviewId };

                    var client = new HttpClient();
                    await client.PostAsJsonAsync<VideoPublishMessage>(webhookEndpoint, msg);
                }
                else if (job.State == JobState.Error)
                {
                    log.Error("Job Failed with Error. ");
                    throw new Exception("Job failed encoding .");
                }
            }
            catch (Exception ex)
            {
                log.Error($"An exception occurred: {ex.Message}");
            }

            // Write to a queue
            log.Info("All done!!");
        }

        #region Helper Functions

        public static async Task<IAsset> CreateAssetFromBlobAsync(CloudBlockBlob blob, string assetName, TraceWriter log)
        {
            //Get a reference to the storage account that is associated with the Media Services account. 
            StorageCredentials mediaServicesStorageCredentials =
                new StorageCredentials(storageAccountName, storageAccountKey);

            destinationStorageAccount = new CloudStorageAccount(mediaServicesStorageCredentials, false);

            // Create a new asset. 
            var asset = context.Assets.Create(blob.Name, AssetCreationOptions.None);

            log.Info($"Created new asset {asset.Name}");

            IAccessPolicy writePolicy = context.AccessPolicies.Create("writePolicy",
                TimeSpan.FromHours(4), AccessPermissions.Write);

            ILocator destinationLocator = context.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);

            CloudBlobClient destBlobStorage = destinationStorageAccount.CreateCloudBlobClient();

            // Get the destination asset container reference
            string destinationContainerName = (new Uri(destinationLocator.Path)).Segments[1];

            log.Info($"Destination container name: {destinationContainerName}");

            CloudBlobContainer assetContainer = destBlobStorage.GetContainerReference(destinationContainerName);

            try
            {
                assetContainer.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                log.Error("ERROR:" + ex.Message);
            }

            log.Info("Created asset.");

            // Get hold of the destination blob
            CloudBlockBlob destinationBlob = assetContainer.GetBlockBlobReference(blob.Name);

            // Copy Blob
            try
            {
                using (var stream = await blob.OpenReadAsync())
                {
                    await destinationBlob.UploadFromStreamAsync(stream);
                }

                log.Info("Copy Complete.");

                var assetFile = asset.AssetFiles.Create(blob.Name);
                assetFile.ContentFileSize = blob.Properties.Length;

                assetFile.IsPrimary = true;
                assetFile.Update();
                asset.Update();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Info(ex.StackTrace);
                log.Info("Copy Failed.");

                throw;
            }

            destinationLocator.Delete();
            writePolicy.Delete();

            return asset;
        }

        private static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
                ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        static void InitializeCloudMediaContext()
        {
            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(AADTenantDomain,
                               new AzureAdClientSymmetricKey(mediaServicesClientId, mediaServicesClientSecret),
                               AzureEnvironments.AzureCloudEnvironment);

            AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            context = new CloudMediaContext(new Uri(RESTAPIEndpoint), tokenProvider);
        }

        #endregion

    }
}

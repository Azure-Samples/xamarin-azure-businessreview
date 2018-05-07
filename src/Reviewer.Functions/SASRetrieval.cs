using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;


namespace Reviewer.Functions
{
    public static class SASRetrieval
    {
        [FunctionName("SASRetrieval")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]StoragePermissionRequest input,
            [Blob("review-photos", FileAccess.Read)]CloudBlobDirectory blobDirectory, 
            TraceWriter log)
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            var permissions = SharedAccessBlobPermissions.Read; // default to read permissions

            // if permission was supplied, check if it is a possible value
            if (!string.IsNullOrWhiteSpace(input.Permission))
            {
                if (!Enum.TryParse(input.Permission, out permissions))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Invalid value for 'permissions'") };
                }
            }

            log.Info($"**** INFO: Requesting {input.Permission} permissions");

            var container = blobDirectory.Container;

            var sasToken = string.IsNullOrWhiteSpace(input.BlobName) ? GetContainerSasToken(container, permissions) : GetBlobSasToken(container, input.BlobName, permissions);

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(sasToken) };
        }

        public static string GetBlobSasToken(CloudBlobContainer container, string blobName, SharedAccessBlobPermissions permissions, string policyName = null)
        {
            string sasBlobToken;

            // Get a reference to a blob within the container.
            // Note that the blob may not exist yet, but a SAS can still be created for it.
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            if (policyName == null)
            {
                var adHocSas = CreateAdHocSasPolicy(permissions);

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                sasBlobToken = blob.GetSharedAccessSignature(adHocSas);
            }
            else
            {
                // Generate the shared access signature on the blob. In this case, all of the constraints for the
                // shared access signature are specified on the container's stored access policy.
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            }

            return sasBlobToken;
        }

        public static string GetContainerSasToken(CloudBlobContainer container, SharedAccessBlobPermissions permissions, string storedPolicyName = null)
        {
            string sasContainerToken;

            // If no stored policy is specified, create a new access policy and define its constraints.
            if (storedPolicyName == null)
            {
                var adHocSas = CreateAdHocSasPolicy(permissions);

                // Generate the shared access signature on the container, setting the constraints directly on the signature.
                sasContainerToken = container.GetSharedAccessSignature(adHocSas, null);
            }
            else
            {
                // Generate the shared access signature on the container. In this case, all of the constraints for the
                // shared access signature are specified on the stored access policy, which is provided by name.
                // It is also possible to specify some constraints on an ad-hoc SAS and others on the stored access policy.
                // However, a constraint must be specified on one or the other; it cannot be specified on both.
                sasContainerToken = container.GetSharedAccessSignature(null, storedPolicyName);
            }

            return sasContainerToken;
        }

        private static SharedAccessBlobPolicy CreateAdHocSasPolicy(SharedAccessBlobPermissions permissions)
        {
            // Create a new access policy and define its constraints.
            // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and 
            // to construct a shared access policy that is saved to the container's shared access policies. 
            return new SharedAccessBlobPolicy()
            {
                // Set start time to five minutes before now to avoid clock skew.
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                Permissions = permissions
            };
        }
    }
}

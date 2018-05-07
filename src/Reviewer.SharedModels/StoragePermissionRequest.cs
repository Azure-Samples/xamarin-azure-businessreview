using System;
using System.Collections.Generic;
using System.Text;

namespace Reviewer.SharedModels
{
    public class StoragePermissionRequest
    {
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public string Permission { get; set; }
    }
}

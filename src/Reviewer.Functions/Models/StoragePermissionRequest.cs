using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reviewer.Functions
{
    public class StoragePermissionRequest
    {
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public string Permission { get; set; }
    }
}

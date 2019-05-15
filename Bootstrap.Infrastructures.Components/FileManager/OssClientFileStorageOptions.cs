using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Components.FileUploader
{
    public class OssClientFileStorageOptions
    {
        public string Endpoint { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string BucketName { get; set; }
        public string Domain { get; set; }
    }
}

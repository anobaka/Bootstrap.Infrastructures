namespace Bootstrap.Infrastructures.Components.FileManager.Storage
{
    public class OssFileStorageOptions
    {
        public string Endpoint { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string BucketName { get; set; }
        public string Domain { get; set; }
        public string RootPath { get; set; }
    }
}

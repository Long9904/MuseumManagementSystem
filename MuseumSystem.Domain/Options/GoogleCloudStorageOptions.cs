namespace MuseumSystem.Domain.Options
{
    public class GoogleCloudStorageOptions
    {
        public string BucketName { get; set; } = string.Empty;
        public string? CredentialsFilePath { get; set; } = string.Empty;
    }
}

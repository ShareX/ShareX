using Amazon;

namespace ShareX.UploadersLib.FileUploaders
{
    public class AmazonS3Region
    {
        public AmazonS3Region(string name)
        {
            Name = name;
        }

        public AmazonS3Region(string name, string identifier, string hostname)
        {
            Name = name;
            Identifier = identifier;
            Hostname = hostname;
        }

        public AmazonS3Region(RegionEndpoint region)
        {
            Name = region.DisplayName;
            Identifier = region.SystemName;
            AmazonRegion = region;
            Hostname = region.GetEndpointForService("s3").Hostname;
        }

        public string Name { get; private set; }
        public string Identifier { get; private set; }
        public RegionEndpoint AmazonRegion { get; private set; }
        public string Hostname { get; private set; }
    }
}
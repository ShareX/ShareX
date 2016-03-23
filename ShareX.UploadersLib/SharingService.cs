namespace ShareX.UploadersLib
{
    /// <summary>
    /// Defines a service capable of sharing a URL to (eg. Facebook, Twitter)
    /// </summary>
    public abstract class SharingService : UploaderService<URLSharingServices>
    {
        /// <summary>
        /// Shares the URL with the service
        /// </summary>
        /// <param name="url">the unencoded URL to share</param>
        /// <param name="uploadersConfig">the uploaders configuration for the task</param>
        public abstract void ShareURL(string url, UploadersConfig uploadersConfig);
    }
}
namespace AzureBlobStorageWrapper.Models;

/// <summary>
/// Represents basic information about a blob
/// </summary>
public class BlobItemInfo
{
    /// <summary>
    /// Name of the blob
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Size of the blob in bytes
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Last modified date/time of the blob
    /// </summary>
    public DateTimeOffset? LastModified { get; set; }

    /// <summary>
    /// Content type of the blob
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// ETag of the blob
    /// </summary>
    public string? ETag { get; set; }
}

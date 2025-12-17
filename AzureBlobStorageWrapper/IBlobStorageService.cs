using AzureBlobStorageWrapper.Models;

namespace AzureBlobStorageWrapper;

/// <summary>
/// Interface for Azure Blob Storage operations using Managed Identity authentication
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a blob from a stream
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="content">Stream containing the blob content</param>
    /// <param name="contentType">Optional content type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the upload operation</returns>
    Task UploadAsync(string blobName, Stream content, string? contentType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob to a stream
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="destination">Stream to write the blob content to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the download operation</returns>
    Task DownloadAsync(string blobName, Stream destination, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob and returns its content as a byte array
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Byte array containing the blob content</returns>
    Task<byte[]> DownloadBytesAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a blob
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>BlobItemInfo object containing blob metadata</returns>
    Task<BlobItemInfo?> GetBlobInfoAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a blob exists
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the blob exists, false otherwise</returns>
    Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a blob
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the blob was deleted, false if it didn't exist</returns>
    Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all blobs in the container
    /// </summary>
    /// <param name="prefix">Optional prefix to filter blobs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of BlobItemInfo objects</returns>
    Task<List<BlobItemInfo>> ListBlobsAsync(string? prefix = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file from the file system
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="filePath">Path to the file to upload</param>
    /// <param name="contentType">Optional content type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the upload operation</returns>
    Task UploadFileAsync(string blobName, string filePath, string? contentType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob to a file
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="filePath">Path where the file will be saved</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the download operation</returns>
    Task DownloadToFileAsync(string blobName, string filePath, CancellationToken cancellationToken = default);
}

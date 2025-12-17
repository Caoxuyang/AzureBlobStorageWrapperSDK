using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobStorageWrapper.Models;

namespace AzureBlobStorageWrapper;

/// <summary>
/// Implementation of Azure Blob Storage operations using Managed Identity authentication
/// </summary>
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly BlobStorageOptions _options;

    /// <summary>
    /// Initializes a new instance of BlobStorageService with Managed Identity authentication
    /// </summary>
    /// <param name="options">Configuration options for blob storage</param>
    public BlobStorageService(BlobStorageOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.AccountName))
            throw new ArgumentException("AccountName is required", nameof(options));

        if (string.IsNullOrWhiteSpace(options.ContainerName))
            throw new ArgumentException("ContainerName is required", nameof(options));

        // Create the BlobServiceClient using Managed Identity
        var credential = CreateManagedIdentityCredential(options);
        var blobServiceUri = new Uri($"https://{options.AccountName}.blob.core.windows.net");
        var blobServiceClient = new BlobServiceClient(blobServiceUri, credential);

        _containerClient = blobServiceClient.GetBlobContainerClient(options.ContainerName);
    }

    /// <summary>
    /// Creates a DefaultAzureCredential with appropriate options for Managed Identity
    /// </summary>
    private static DefaultAzureCredential CreateManagedIdentityCredential(BlobStorageOptions options)
    {
        var credentialOptions = new DefaultAzureCredentialOptions();

        if (!string.IsNullOrWhiteSpace(options.TenantId))
        {
            credentialOptions.TenantId = options.TenantId;
        }

        if (!string.IsNullOrWhiteSpace(options.ClientId))
        {
            credentialOptions.ManagedIdentityClientId = options.ClientId;
        }

        return new DefaultAzureCredential(credentialOptions);
    }

    /// <inheritdoc/>
    public async Task UploadAsync(string blobName, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        if (content == null)
            throw new ArgumentNullException(nameof(content));

        var blobClient = _containerClient.GetBlobClient(blobName);

        var options = new BlobUploadOptions();
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            options.HttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        }

        await blobClient.UploadAsync(content, options, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DownloadAsync(string blobName, Stream destination, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DownloadToAsync(destination, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<byte[]> DownloadBytesAsync(string blobName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToArray();
    }

    /// <inheritdoc/>
    public async Task<BlobItemInfo?> GetBlobInfoAsync(string blobName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        var blobClient = _containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync(cancellationToken))
            return null;

        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

        return new BlobItemInfo
        {
            Name = blobName,
            Size = properties.Value.ContentLength,
            LastModified = properties.Value.LastModified,
            ContentType = properties.Value.ContentType,
            ETag = properties.Value.ETag.ToString()
        };
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        var blobClient = _containerClient.GetBlobClient(blobName);
        return await blobClient.ExistsAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    /// <inheritdoc/>
    public async Task<List<BlobItemInfo>> ListBlobsAsync(string? prefix = null, CancellationToken cancellationToken = default)
    {
        var blobs = new List<BlobItemInfo>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            blobs.Add(new BlobItemInfo
            {
                Name = blobItem.Name,
                Size = blobItem.Properties.ContentLength ?? 0,
                LastModified = blobItem.Properties.LastModified,
                ContentType = blobItem.Properties.ContentType,
                ETag = blobItem.Properties.ETag?.ToString()
            });
        }

        return blobs;
    }

    /// <inheritdoc/>
    public async Task UploadFileAsync(string blobName, string filePath, string? contentType = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);

        using var fileStream = File.OpenRead(filePath);
        await UploadAsync(blobName, fileStream, contentType, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DownloadToFileAsync(string blobName, string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be empty", nameof(blobName));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = File.Create(filePath);
        await DownloadAsync(blobName, fileStream, cancellationToken);
    }
}

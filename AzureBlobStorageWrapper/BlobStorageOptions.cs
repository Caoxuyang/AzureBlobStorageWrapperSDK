namespace AzureBlobStorageWrapper;

/// <summary>
/// Configuration options for Azure Blob Storage connection
/// </summary>
public class BlobStorageOptions
{
    /// <summary>
    /// The Azure Storage account name
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// The blob container name
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Tenant ID for Managed Identity authentication
    /// If not provided, uses the default tenant
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Optional: Client ID for User-Assigned Managed Identity
    /// If not provided, uses System-Assigned Managed Identity
    /// </summary>
    public string? ClientId { get; set; }
}

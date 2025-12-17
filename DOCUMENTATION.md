# Azure Blob Storage Wrapper SDK

A lightweight .NET wrapper SDK for Azure Blob Storage that simplifies common blob operations using Managed Identity authentication. This library eliminates the need to work directly with the Azure Storage SDK by providing a clean, easy-to-use interface.

## Features

- **Managed Identity Authentication**: Built-in support for both System-Assigned and User-Assigned Managed Identities
- **Simple API**: Intuitive methods for common blob operations
- **Comprehensive Operations**: Upload, download, delete, list, and metadata retrieval
- **Stream and File Support**: Work with streams or files directly
- **Async/Await**: Full asynchronous support with cancellation tokens
- **.NET 8.0**: Built on the latest .NET platform

## Installation

### Using the DLL directly

1. Build the project to generate the DLL:
   ```bash
   dotnet build AzureBlobStorageWrapper.csproj --configuration Release
   ```

2. The DLL will be generated at:
   ```
   bin/Release/net8.0/AzureBlobStorageWrapper.dll
   ```

3. Reference the DLL in your project:
   - In Visual Studio: Right-click on your project → Add → Reference → Browse and select the DLL
   - In .csproj file:
     ```xml
     <ItemGroup>
       <Reference Include="AzureBlobStorageWrapper">
         <HintPath>path\to\AzureBlobStorageWrapper.dll</HintPath>
       </Reference>
     </ItemGroup>
     ```

### Using NuGet Package

The project is configured to generate a NuGet package (.nupkg) on build. You can find it in `bin/Debug` or `bin/Release` directory and install it locally.

## Prerequisites

Before using this wrapper SDK, ensure you have:

1. An Azure Storage Account
2. A blob container created in the storage account
3. Managed Identity configured and assigned appropriate permissions:
   - System-Assigned or User-Assigned Managed Identity enabled on your Azure resource (VM, App Service, Function App, etc.)
   - Managed Identity must have at least "Storage Blob Data Contributor" role on the storage account or container

## Configuration

### Azure Setup

1. **Enable Managed Identity** on your Azure resource (e.g., Azure App Service, VM, Function App)
2. **Assign RBAC role** to the Managed Identity:
   - Navigate to your Storage Account → Access Control (IAM)
   - Add role assignment: "Storage Blob Data Contributor"
   - Select your Managed Identity as the member

### Code Configuration

The SDK uses the `BlobStorageOptions` class for configuration:

```csharp
using AzureBlobStorageWrapper;

var options = new BlobStorageOptions
{
    AccountName = "mystorageaccount",      // Your storage account name
    ContainerName = "mycontainer",          // Your container name
    TenantId = null,                        // Optional: Specify for specific tenant
    ClientId = null                         // Optional: For User-Assigned Managed Identity
};
```

## Usage Examples

### Basic Setup

```csharp
using AzureBlobStorageWrapper;
using AzureBlobStorageWrapper.Models;

// Configure the service
var options = new BlobStorageOptions
{
    AccountName = "mystorageaccount",
    ContainerName = "mycontainer"
};

// Create the service instance
IBlobStorageService blobService = new BlobStorageService(options);
```

### Upload a File

```csharp
// Upload from file path
await blobService.UploadFileAsync(
    blobName: "documents/report.pdf",
    filePath: @"C:\local\path\report.pdf",
    contentType: "application/pdf"
);

// Upload from stream
using var fileStream = File.OpenRead(@"C:\local\path\data.json");
await blobService.UploadAsync(
    blobName: "data/data.json",
    content: fileStream,
    contentType: "application/json"
);

Console.WriteLine("Upload completed successfully!");
```

### Download a File

```csharp
// Download to file
await blobService.DownloadToFileAsync(
    blobName: "documents/report.pdf",
    filePath: @"C:\downloads\report.pdf"
);

// Download to stream
using var memoryStream = new MemoryStream();
await blobService.DownloadAsync(
    blobName: "data/data.json",
    destination: memoryStream
);

// Download as byte array
byte[] fileContent = await blobService.DownloadBytesAsync("images/photo.jpg");
await File.WriteAllBytesAsync(@"C:\downloads\photo.jpg", fileContent);

Console.WriteLine("Download completed successfully!");
```

### Check if Blob Exists

```csharp
bool exists = await blobService.ExistsAsync("documents/report.pdf");
if (exists)
{
    Console.WriteLine("Blob exists!");
}
else
{
    Console.WriteLine("Blob not found.");
}
```

### Get Blob Metadata

```csharp
BlobItemInfo? blobInfo = await blobService.GetBlobInfoAsync("documents/report.pdf");

if (blobInfo != null)
{
    Console.WriteLine($"Name: {blobInfo.Name}");
    Console.WriteLine($"Size: {blobInfo.Size} bytes");
    Console.WriteLine($"Content Type: {blobInfo.ContentType}");
    Console.WriteLine($"Last Modified: {blobInfo.LastModified}");
    Console.WriteLine($"ETag: {blobInfo.ETag}");
}
```

### List All Blobs

```csharp
// List all blobs in the container
List<BlobItemInfo> allBlobs = await blobService.ListBlobsAsync();

foreach (var blob in allBlobs)
{
    Console.WriteLine($"{blob.Name} - {blob.Size} bytes");
}

// List blobs with a prefix (folder simulation)
List<BlobItemInfo> documents = await blobService.ListBlobsAsync(prefix: "documents/");

Console.WriteLine("\nDocuments:");
foreach (var doc in documents)
{
    Console.WriteLine($"  {doc.Name}");
}
```

### Delete a Blob

```csharp
bool deleted = await blobService.DeleteAsync("documents/old-report.pdf");

if (deleted)
{
    Console.WriteLine("Blob deleted successfully!");
}
else
{
    Console.WriteLine("Blob not found.");
}
```

## Complete Example Application

```csharp
using AzureBlobStorageWrapper;
using AzureBlobStorageWrapper.Models;

namespace MyApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                // Initialize the blob storage service
                var options = new BlobStorageOptions
                {
                    AccountName = "mystorageaccount",
                    ContainerName = "mycontainer"
                };

                IBlobStorageService blobService = new BlobStorageService(options);

                // Upload a file
                Console.WriteLine("Uploading file...");
                await blobService.UploadFileAsync(
                    blobName: "test/sample.txt",
                    filePath: "sample.txt",
                    contentType: "text/plain"
                );
                Console.WriteLine("Upload complete!");

                // List all blobs
                Console.WriteLine("\nListing all blobs:");
                var blobs = await blobService.ListBlobsAsync();
                foreach (var blob in blobs)
                {
                    Console.WriteLine($"  - {blob.Name} ({blob.Size} bytes)");
                }

                // Download the file
                Console.WriteLine("\nDownloading file...");
                await blobService.DownloadToFileAsync(
                    blobName: "test/sample.txt",
                    filePath: "downloaded-sample.txt"
                );
                Console.WriteLine("Download complete!");

                // Get blob info
                var blobInfo = await blobService.GetBlobInfoAsync("test/sample.txt");
                if (blobInfo != null)
                {
                    Console.WriteLine($"\nBlob Info:");
                    Console.WriteLine($"  Name: {blobInfo.Name}");
                    Console.WriteLine($"  Size: {blobInfo.Size} bytes");
                    Console.WriteLine($"  Last Modified: {blobInfo.LastModified}");
                }

                // Delete the blob
                Console.WriteLine("\nDeleting blob...");
                bool deleted = await blobService.DeleteAsync("test/sample.txt");
                Console.WriteLine(deleted ? "Blob deleted!" : "Blob not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
```

## Advanced Configuration

### Using User-Assigned Managed Identity

```csharp
var options = new BlobStorageOptions
{
    AccountName = "mystorageaccount",
    ContainerName = "mycontainer",
    ClientId = "00000000-0000-0000-0000-000000000000"  // Your User-Assigned Identity Client ID
};

IBlobStorageService blobService = new BlobStorageService(options);
```

### Specifying Tenant ID

```csharp
var options = new BlobStorageOptions
{
    AccountName = "mystorageaccount",
    ContainerName = "mycontainer",
    TenantId = "00000000-0000-0000-0000-000000000000"  // Your Azure AD Tenant ID
};

IBlobStorageService blobService = new BlobStorageService(options);
```

## Error Handling

The SDK methods may throw exceptions in the following scenarios:

- **ArgumentException**: Invalid or empty parameters (blob name, file path, etc.)
- **ArgumentNullException**: Null parameters where non-null is required
- **FileNotFoundException**: When uploading a file that doesn't exist
- **Azure.RequestFailedException**: Azure-specific errors (authentication, network, storage account issues)

Example with error handling:

```csharp
try
{
    await blobService.UploadFileAsync("test.txt", "localfile.txt");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid argument: {ex.Message}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.Message}");
}
catch (Azure.RequestFailedException ex)
{
    Console.WriteLine($"Azure error: {ex.Message}");
    Console.WriteLine($"Status: {ex.Status}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## API Reference

### IBlobStorageService Interface

#### UploadAsync
Uploads a blob from a stream.
```csharp
Task UploadAsync(string blobName, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
```

#### DownloadAsync
Downloads a blob to a stream.
```csharp
Task DownloadAsync(string blobName, Stream destination, CancellationToken cancellationToken = default)
```

#### DownloadBytesAsync
Downloads a blob and returns its content as a byte array.
```csharp
Task<byte[]> DownloadBytesAsync(string blobName, CancellationToken cancellationToken = default)
```

#### GetBlobInfoAsync
Gets information about a blob.
```csharp
Task<BlobItemInfo?> GetBlobInfoAsync(string blobName, CancellationToken cancellationToken = default)
```

#### ExistsAsync
Checks if a blob exists.
```csharp
Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default)
```

#### DeleteAsync
Deletes a blob.
```csharp
Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default)
```

#### ListBlobsAsync
Lists all blobs in the container.
```csharp
Task<List<BlobItemInfo>> ListBlobsAsync(string? prefix = null, CancellationToken cancellationToken = default)
```

#### UploadFileAsync
Uploads a file from the file system.
```csharp
Task UploadFileAsync(string blobName, string filePath, string? contentType = null, CancellationToken cancellationToken = default)
```

#### DownloadToFileAsync
Downloads a blob to a file.
```csharp
Task DownloadToFileAsync(string blobName, string filePath, CancellationToken cancellationToken = default)
```

## Dependencies

- Azure.Storage.Blobs (>= 12.26.0)
- Azure.Identity (>= 1.17.1)
- .NET 8.0

## Troubleshooting

### Authentication Errors

If you encounter authentication errors:

1. Verify Managed Identity is enabled on your Azure resource
2. Check that the Managed Identity has the correct RBAC role (Storage Blob Data Contributor)
3. Ensure the role is assigned at the correct scope (Storage Account or Container level)
4. If using User-Assigned Managed Identity, verify the ClientId is correct

### Connection Errors

- Verify the storage account name is correct
- Check that the container exists
- Ensure your Azure resource has network connectivity to the storage account
- Verify firewall rules on the storage account allow access from your resource

## Building from Source

```bash
# Clone the repository
git clone https://github.com/Caoxuyang/AzureBlobStorageWrapperSDK.git
cd AzureBlobStorageWrapperSDK/AzureBlobStorageWrapper

# Restore dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# The DLL will be available at:
# bin/Release/net8.0/AzureBlobStorageWrapper.dll
```

## License

MIT

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/Caoxuyang/AzureBlobStorageWrapperSDK).

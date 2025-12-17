# AzureBlobStorageWrapperSDK

A lightweight .NET wrapper SDK for Azure Blob Storage with Managed Identity authentication support. This library provides a simple, easy-to-use interface for common blob operations without the complexity of the full Azure Storage SDK.

## Features

✅ **Managed Identity Authentication** - Supports both System-Assigned and User-Assigned Managed Identities  
✅ **Simple API** - Clean, intuitive interface for blob operations  
✅ **Comprehensive Operations** - Upload, download, delete, list, and metadata retrieval  
✅ **Stream & File Support** - Work with streams or files directly  
✅ **Async/Await** - Full asynchronous support with cancellation tokens  
✅ **Ready to Package** - Can be compiled to a private DLL for use in other applications  

## Quick Start

### Installation

1. Build the project to generate the DLL:
   ```bash
   cd AzureBlobStorageWrapper
   dotnet build --configuration Release
   ```

2. Reference the generated DLL (`bin/Release/net8.0/AzureBlobStorageWrapper.dll`) in your project.

### Basic Usage

```csharp
using AzureBlobStorageWrapper;

// Configure the service
var options = new BlobStorageOptions
{
    AccountName = "mystorageaccount",
    ContainerName = "mycontainer"
};

// Create service instance
IBlobStorageService blobService = new BlobStorageService(options);

// Upload a file
await blobService.UploadFileAsync("documents/report.pdf", @"C:\local\report.pdf");

// Download a file
await blobService.DownloadToFileAsync("documents/report.pdf", @"C:\downloads\report.pdf");

// List blobs
var blobs = await blobService.ListBlobsAsync();

// Delete a blob
await blobService.DeleteAsync("documents/report.pdf");
```

## Prerequisites

- .NET 8.0 or later
- Azure Storage Account with a blob container
- Managed Identity configured with "Storage Blob Data Contributor" role

## Documentation

For comprehensive documentation including:
- Detailed setup instructions
- API reference
- Advanced configuration
- Error handling
- Complete examples

See **[DOCUMENTATION.md](DOCUMENTATION.md)**

## API Overview

The SDK provides the following operations through the `IBlobStorageService` interface:

| Method | Description |
|--------|-------------|
| `UploadAsync` | Upload blob from stream |
| `UploadFileAsync` | Upload blob from file path |
| `DownloadAsync` | Download blob to stream |
| `DownloadToFileAsync` | Download blob to file path |
| `DownloadBytesAsync` | Download blob as byte array |
| `GetBlobInfoAsync` | Get blob metadata |
| `ExistsAsync` | Check if blob exists |
| `DeleteAsync` | Delete a blob |
| `ListBlobsAsync` | List all blobs (with optional prefix filter) |

## Project Structure

```
AzureBlobStorageWrapper/
├── BlobStorageOptions.cs      # Configuration class
├── IBlobStorageService.cs     # Service interface
├── BlobStorageService.cs      # Service implementation
└── Models/
    └── BlobItemInfo.cs        # Blob metadata model
```

## Dependencies

- Azure.Storage.Blobs (12.26.0)
- Azure.Identity (1.17.1)

## Building the Package

The project is configured to automatically generate a NuGet package (.nupkg) when built:

```bash
dotnet build --configuration Release
```

The package will be available at:
- DLL: `bin/Release/net8.0/AzureBlobStorageWrapper.dll`
- NuGet: `bin/Release/AzureBlobStorageWrapper.1.0.0.nupkg`

## License

MIT
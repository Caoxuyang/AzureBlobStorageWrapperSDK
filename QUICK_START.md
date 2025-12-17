# Quick Start: Using the Wrapper SDK DLL

This guide shows you exactly how to use the AzureBlobStorageWrapper DLL in your own application.

## Step 1: Build the DLL

```bash
cd AzureBlobStorageWrapper
dotnet build --configuration Release
```

The DLL will be generated at:
```
AzureBlobStorageWrapper/bin/Release/net8.0/AzureBlobStorageWrapper.dll
```

## Step 2: Create Your Application

Create a new console application or use an existing project:

```bash
dotnet new console -n MyBlobApp
cd MyBlobApp
```

## Step 3: Add Reference to the DLL

Option A - Using Visual Studio:
1. Right-click on your project → Add → Reference
2. Browse and select `AzureBlobStorageWrapper.dll`

Option B - Edit .csproj file directly:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="AzureBlobStorageWrapper">
      <HintPath>..\AzureBlobStorageWrapper\bin\Release\net8.0\AzureBlobStorageWrapper.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
```

## Step 4: Add Required Dependencies

The wrapper SDK depends on Azure packages. Add them to your project:

```bash
dotnet add package Azure.Storage.Blobs
dotnet add package Azure.Identity
```

## Step 5: Write Your Code

Example `Program.cs`:

```csharp
using AzureBlobStorageWrapper;
using AzureBlobStorageWrapper.Models;

// Configure the service
var options = new BlobStorageOptions
{
    AccountName = "your_storage_account_name",
    ContainerName = "your_container_name"
    // For User-Assigned Managed Identity, also set:
    // ClientId = "your_managed_identity_client_id"
};

// Create the service
IBlobStorageService blobService = new BlobStorageService(options);

try
{
    // Example: Upload a file
    Console.WriteLine("Uploading file...");
    await blobService.UploadFileAsync(
        blobName: "test/sample.txt",
        filePath: "sample.txt",
        contentType: "text/plain"
    );
    Console.WriteLine("Upload complete!");

    // Example: List blobs
    Console.WriteLine("\nListing blobs:");
    var blobs = await blobService.ListBlobsAsync();
    foreach (var blob in blobs)
    {
        Console.WriteLine($"  {blob.Name} - {blob.Size} bytes");
    }

    // Example: Download a file
    Console.WriteLine("\nDownloading file...");
    await blobService.DownloadToFileAsync(
        blobName: "test/sample.txt",
        filePath: "downloaded.txt"
    );
    Console.WriteLine("Download complete!");

    // Example: Delete a blob
    Console.WriteLine("\nDeleting blob...");
    bool deleted = await blobService.DeleteAsync("test/sample.txt");
    Console.WriteLine(deleted ? "Blob deleted!" : "Blob not found.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Step 6: Configure Azure

Before running, ensure:

1. **Managed Identity is enabled** on your Azure resource (VM, App Service, Function App, etc.)
2. **RBAC role is assigned**:
   - Go to your Storage Account → Access Control (IAM)
   - Click "Add role assignment"
   - Select "Storage Blob Data Contributor"
   - Select your Managed Identity
   - Click "Save"

## Step 7: Run Your Application

```bash
dotnet run
```

## Complete Project Structure

```
MyBlobApp/
├── MyBlobApp.csproj
├── Program.cs
└── (reference to AzureBlobStorageWrapper.dll)
```

## Distributing Your Application

When deploying your application, make sure to include:
- Your application DLL/executable
- `AzureBlobStorageWrapper.dll`
- All Azure SDK dependencies (automatically copied to output directory)

The output directory will contain all necessary files after build.

## Alternative: Using NuGet Package

Instead of referencing the DLL directly, you can use the generated NuGet package:

```bash
# Install from local .nupkg file
dotnet add package AzureBlobStorageWrapper --source /path/to/nupkg/directory

# Or publish to a private NuGet feed and install from there
```

This approach automatically handles dependencies.

## Troubleshooting

### "Could not load file or assembly" Error
- Ensure the DLL path in your .csproj is correct
- Check that all Azure SDK dependencies are present in your output directory

### Authentication Errors
- Verify Managed Identity is enabled on your Azure resource
- Check RBAC permissions on the storage account
- For User-Assigned MI, verify the ClientId is correct

### Build Errors
- Ensure you're using .NET 8.0 or later
- Make sure Azure.Storage.Blobs and Azure.Identity packages are installed

For more detailed information, see [DOCUMENTATION.md](DOCUMENTATION.md).

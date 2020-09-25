using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.Storage;
using Pulumi.Azure.Storage.Inputs;

class MyStack : Stack
{
    public MyStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("resourceGroup");


        // Create Azure Storage Accounts
        AccountArgs storageConfig = new AccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountReplicationType = "RAGRS",
            AccountTier = "Standard",
            MinTlsVersion = "TLS1_2",
            BlobProperties = new AccountBlobPropertiesArgs
            {
                DeleteRetentionPolicy = new AccountBlobPropertiesDeleteRetentionPolicyArgs { Days = 30 }
            },
            Tags = new InputMap<string> {
                {"stack", "Dev"}
            }
        };

        var mediaAccount = new Account("media", storageConfig);
        var contentAccount = new Account("content", storageConfig);

        // Export the connection strings for the storage accounts
        this.MediaConnectionString = mediaAccount.PrimaryConnectionString;
        this.ContentConnectionString = mediaAccount.PrimaryConnectionString;
    }

    [Output]
    public Output<string> MediaConnectionString { get; set; }

    [Output]
    public Output<string> ContentConnectionString { get; set; }
}

# How to Run Pulumi with an Azure Storage Backend

Use an Azure Storage account to store the stack file instead of app.pulumi.com.

<!-- TOC -->
- [How to Run Pulumi with an Azure Storage Backend](#how-to-run-pulumi-with-an-azure-storage-backend)
  - [Prerequisites](#prerequisites)
  - [Steps](#steps)
  - [Interesting Take-aways](#interesting-take-aways)

<!-- /TOC -->

## Prerequisites

* Pulumi CLI - provided by devcontainer in this project
* dotnet SDK - provided by devcontainer in this project
* Azure CLI - provided by devcontainer in this project

## Steps

1. Same setup as the pipeline example.
2. Create a storage account in Azure.
3. Setup environment
   1. AZURE_STORAGE_ACCOUNT with the name of the account from step 2
   2. AZURE_STORAGE_KEY with the access key for the storage account
4. Run `pulumi login --cloud-url azblob://<storage_container>`
   1. storage_container is the name of the blob container in the account
5. Set the location
   1. `pulumi config set azure:location centralus`
6. Pulumi up
   1. Pulumi prompts for new stack name
   2. Pulumi prompts for a passphrase to protect config secrets
   3. Pulumi builds the project, creates a plan
   4. In the storage account, pulumi creates a `.pulumi` folder
      1. In this folder, pulumi stores the stack JSON
7. Update pipeline
   1. Fix error while selecting stack (Pulumi is looking for stack on app.pulumi.com)
      1. For pipeline we will set storage account name as a variable, and use SAS token
         1. Create `pulumi-azure-backend` variable group
         2. Add AZURE_STORAGE_ACCOUNT variable (not secret) with name of the account.
         3. Update pipeline YAML and allow pipeline to use the vairable group
      2. Without the API key, Pulumi doesn't know how to login yet.  Add a script/task to create and set the SAS token.
         1. Update `pulumi-azure-backend` variable group to include AZURE_STORAGE_CONTAINER with the name of the container inside the storage account.
         2. Add AZURE_STORAGE_AUTH_MODE=login to the group
      3. Update the preview task to consume the SAS token secret with the name AZURE_STORAGE_SAS_TOKEN.
      4. There is a bug in the Pulumi task preventing it from consuming secrets.
      5. Switch to manual tasks.
         1. Must grant 'Storage Blob Data Contributor' to the service principal making the token.
         2. Add environment setup script to add Azure SP enviornment variables to the Pulumi run.

## Interesting Take-aways

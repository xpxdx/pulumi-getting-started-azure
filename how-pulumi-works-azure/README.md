# How Pulumi Works (on Azure)

An example adapted for Azure from the Pulumi documentation page "[How Pulumi Works][1]" and the "Get Started - Azure[2]" guide. A toy project to demonstrate basic developer workflow. 

<!-- TOC -->
- [How Pulumi Works (on Azure)](#how-pulumi-works-on-azure)
  - [Prerequisites](#prerequisites)
  - [Steps](#steps)
  - [Interesting Take-aways](#interesting-take-aways)
<!-- /TOC -->

## Prerequisites

* Pulumi CLI - provided by devcontainer in this project
* dotnet SDK - provided by devcontainer in this project
* Azure CLI - provided by devcontainer in this project

## Steps

1. Create a project directory and bootstrap the Pulumi project.
   1. `mkdir stack && cd stack`
   2. `pulumi new azure-csharp` - launches cli wizard
      1. Project name will be the dotnet csproj name: AzureCSharp.HowItWorks
      2. Description: accepted default
      3. Stack name: jamesrcounts/azcs-howitworks-dev
      4. Azure region: your choice, centralus for me.
2. Bring up the stack: `pulumi up`.
3. Make changes to infrastructure resources (add a tag, for example)
4. Update the stack: `pulumi up`.
5. Destroy the stack: `pulumi destroy`.

## Interesting Take-aways

* Auto-naming by default
* Seems faster than Terraform (due to no up-front refresh?)

[1]: https://www.pulumi.com/docs/intro/concepts/how-pulumi-works/
[2]: https://www.pulumi.com/docs/get-started/azure/
using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.Network;
using Pulumi.Azure.Storage;
using Pulumi.Azure.Network.Inputs;
using Pulumi.Azure.Compute;
using Pulumi.Azure.Compute.Inputs;

class MyStack : Stack
{
    public MyStack()
    {
        var suffix = "pulumi-devops";

        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup($"rg-{suffix}");
        var network = CreateVirtualNetwork(suffix, resourceGroup);
        CreateVirtualMachine(suffix, resourceGroup, network);
    }

    private static void CreateVirtualMachine(string suffix, ResourceGroup resourceGroup, CreateVirtualNetworkResult network)
    {
        var publicip = new PublicIp($"pip-{suffix}", new PublicIpArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AllocationMethod = "Static",
        });

        var nic = new NetworkInterface($"nic-{suffix}", new NetworkInterfaceArgs
        {
            ResourceGroupName = resourceGroup.Name,
            IpConfigurations =
            {
                new NetworkInterfaceIpConfigurationArgs
                {
                    Name = "nic-pip-association",
                    SubnetId = network.SubnetId,
                    PrivateIpAddressAllocation = "Dynamic",
                    PublicIpAddressId = publicip.Id,
                },
            },
        });

        var nicNsgAssociation = new NetworkInterfaceSecurityGroupAssociation($"nic-to-nsg-{suffix}", new NetworkInterfaceSecurityGroupAssociationArgs
        {
            NetworkInterfaceId = nic.Id,
            NetworkSecurityGroupId = network.NetworkSecurityGroupId
        });

        var vm = new LinuxVirtualMachine($"vm-{suffix}", new LinuxVirtualMachineArgs
        {
            ResourceGroupName = resourceGroup.Name,
            NetworkInterfaceIds = { nic.Id },
            Size = "Standard_DS1_v2",
            ComputerName = $"vm-{suffix}",
            AdminUsername = "plankton",
            AdminPassword = "password1234!",
            DisablePasswordAuthentication = false,
            OsDisk = new LinuxVirtualMachineOsDiskArgs
            {
                Name = "disk0",
                Caching = "ReadWrite",
                StorageAccountType = "Premium_LRS"
            },
            SourceImageReference = new LinuxVirtualMachineSourceImageReferenceArgs
            {
                Publisher = "Canonical",
                Offer = "UbuntuServer",
                Sku = "18_04-lts-gen2",
                Version = "latest",
            },

        });
    }

    private static CreateVirtualNetworkResult CreateVirtualNetwork(string suffix, ResourceGroup resourceGroup)
    {
        var virtualNetwork = new VirtualNetwork($"vnet-{suffix}", new VirtualNetworkArgs
        {
            AddressSpaces = { "10.0.0.0/16" },
            ResourceGroupName = resourceGroup.Name
        });

        var subnet = new Subnet("apps", new SubnetArgs
        {
            ResourceGroupName = resourceGroup.Name,
            VirtualNetworkName = virtualNetwork.Name,
            AddressPrefixes = {
                "10.0.1.0/24",
            }
        });

        var nsg = new NetworkSecurityGroup($"nsg-{suffix}", new NetworkSecurityGroupArgs
        {
            ResourceGroupName = resourceGroup.Name,
        });

        var nsgSshAllowInbound = new NetworkSecurityRule("ssh-allow-inbound", new NetworkSecurityRuleArgs
        {
            Priority = 1001,
            Direction = "Inbound",
            Access = "Allow",
            Protocol = "Tcp",
            SourcePortRange = "*",
            DestinationPortRange = "22",
            SourceAddressPrefix = "*",
            DestinationAddressPrefix = "*",
            NetworkSecurityGroupName = nsg.Name,
            ResourceGroupName = resourceGroup.Name,
        });

        return new CreateVirtualNetworkResult
        {
            NetworkSecurityGroupId = nsg.Id,
            SubnetId = subnet.Id
        };
    }
}

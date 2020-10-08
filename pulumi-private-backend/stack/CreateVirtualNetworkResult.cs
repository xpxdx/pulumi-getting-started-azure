using Pulumi;

class CreateVirtualNetworkResult{
    public Input<string> NetworkSecurityGroupId { get; set; } = null!;
    public Input<string> SubnetId { get; set; } = null!;
}
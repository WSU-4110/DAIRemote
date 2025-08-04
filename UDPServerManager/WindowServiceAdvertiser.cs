using Makaretu.Dns;

public class WindowsServiceAdvertiser : IDisposable
{
    private readonly string serviceName;
    private readonly int port;
    private MulticastService mdns;
    private ServiceDiscovery serviceDiscovery;

    public WindowsServiceAdvertiser(string serviceName = "DAIRemote Desktop", int port = 9416)
    {
        this.serviceName = serviceName;
        this.port = port;
    }

    public void StartAdvertising()
    {
        mdns = new MulticastService();
        serviceDiscovery = new ServiceDiscovery();

        var serviceProfile = new ServiceProfile(serviceName, "_daidesktop._tcp", (ushort)port);
        serviceProfile.AddProperty("description", "DAIRemote gesture control service");
        serviceProfile.AddProperty("version", "1.0");
        serviceProfile.AddProperty("platform", Environment.OSVersion.Platform.ToString());

        mdns.Start(); // Starts on background thread
        serviceDiscovery.Advertise(serviceProfile);
    }

    public void StopAdvertising()
    {
        serviceDiscovery?.Dispose();
        mdns?.Dispose();
    }

    public void Dispose()
    {
        StopAdvertising();
    }
}

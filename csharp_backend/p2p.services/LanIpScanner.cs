using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading.Channels;

namespace p2p.services;

public class LanIpScanner
{
  // method to obtain the local IP address
  public string GetIpLocal()
  {
    var host = Dns.GetHostEntry(Dns.GetHostName());

    Console.WriteLine($"Local  ip: {host.AddressList[0]}");
    Console.WriteLine($"Public ip: {host.AddressList[1]}");
    Console.WriteLine($"Addre Mac: {host.AddressList[2]}");

    foreach (var ip in host.AddressList)
    {
      if (ip.AddressFamily == AddressFamily.InterNetwork)
      {
        return $"Local ip: {ip.ToString()}";
      }
    }

    return "";
  }

  public static async Task<List<(string ipAddress, string hostName)>> GetAllIpAddressesAndHostnamesAsync(
     string subnet,
     int pingTimeout = 500,
     int maxParallelPings = 50,
     CancellationToken cancellationToken = default)
  {
    var devices = new List<(string ipAddress, string hostName)>();
    var semaphore = new SemaphoreSlim(maxParallelPings);

    var tasks = new List<Task>();

    for (int i = 1; i <= 254; i++)
    {
      await semaphore.WaitAsync(cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();

      string ipAddress = $"{subnet}.{i}";
      tasks.Add(ProcessIpAsync(ipAddress, pingTimeout, semaphore, cancellationToken)
          .ContinueWith(t =>
          {
            if (t.Result.hostName != null)
            {
              lock (devices)
              {
                devices.Add(t.Result);
              }
            }
          }, cancellationToken));
    }

    await Task.WhenAll(tasks);
    return devices;
  }

  private static async Task<(string ipAddress, string hostName)> ProcessIpAsync(
      string ipAddress,
      int timeout,
      SemaphoreSlim semaphore,
      CancellationToken cancellationToken)
  {
    try
    {
      using var ping = new Ping(); // 👈 Nueva instancia para cada ping
      var reply = await ping.SendPingAsync(ipAddress, timeout);
      if (reply.Status == IPStatus.Success)
      {
        string hostName = await GetHostnameAsync(ipAddress, cancellationToken);
        return (ipAddress, hostName);
      }
      return (ipAddress, null);
    }
    catch (PingException)
    {
      return (ipAddress, null);
    }
    finally
    {
      semaphore.Release();
    }
  }

  private static async Task ProcessResultsAsync(
      ChannelReader<(string ip, string hostname)> reader,
      List<(string ipAddress, string hostName)> devices,
      CancellationToken cancellationToken)
  {
    await foreach (var item in reader.ReadAllAsync(cancellationToken))
    {
      devices.Add((item.ip, item.hostname));
    }
  }

  private static async Task<string> GetHostnameAsync(string ipAddress, CancellationToken cancellationToken)
  {
    Console.WriteLine(ipAddress);

    if (!IsValidIp(ipAddress))
      Console.WriteLine("Invalid IP address");

    try
    {
      var hostEntry = await Dns.GetHostEntryAsync(ipAddress, cancellationToken);
      // System.Console.WriteLine(ipAddress);
      // System.Console.WriteLine(hostEntry.HostName);
      return hostEntry.HostName;
    }
    catch (SocketException)
    {
      return null;
    }
  }

  private static bool IsValidIp(string ipAddress)
  {
    return IPAddress.TryParse(ipAddress, out _);
  }
}


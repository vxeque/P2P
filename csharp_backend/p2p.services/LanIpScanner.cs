using System.Net;
using System.Net.Sockets; 
using System.Threading.Tasks;
using System.Linq;

namespace p2p.services;

public class LanIpScanner
{
  // method to obtain the local IP address
  public string GetIpLocal(){
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
  
  
  // method to obtain the all IP address 
  public static async Task<List<(string ipAddress, string hostname)>> GetAllIpAddressAndHostnames
    (
      string subnet,
      int pingTimeout = 1000,
      int maxParallelPings = 50,
      CancellationToken cancellationToken = default
    )
  {
    if (string.IsNullOrWhiteSpace(subnet))
      throw new ArgumentException("Subnet cannot be empty", nameof(subnet)); 
  
    var devices = new List<(string ipAddress, string hostname)>();
    var tasks = new List<Task<(string ip, string hostname)>>();
    var smaphore = new SemaphoreSlim(maxParallelPings); 

    using (var ping = new Ping())
    {
      for (int i = 1; i <= 254; i++)
      {
        await smaphore.WaitAsync(cancellationToken); 
        cancellationToken.ThrowIfCancellationRequested();
        string ipAddress = $"{subnet}.{i}"; 
        tasks.Add(ProcessIpAsync(ping, ipAddress, pingTimeout, smaphore, cancellationToken));  
        var result = await Task.WaitAll(tasks);
      devices.AddRange(result.Where(x => x.hostname != null)); 
    }

    return devices; 

  }

  private static async Task<(string ip, string hostname)> ProcessIpAsync
    (
      Ping ping, 
      string ipAddress, 
      int timeout, 
      SemaphoreSlim semaphore,
      CancellationToken cancellationToken
    )
    {
      try
      {
        var reply = await ping.SendPingAsync(ipAddress, timeout);
        if (reply.Status == IPStatus.Success)
        {
          string hostname = await GetHostnameAsync(ipAddress, cancellationToken); 
          return (ipAddress, hostname); 
        }
        return (ipAddress, "--null--"); 
      }
      catch (PingException)
      {
          
          return (ipAddress, "---NUll---");  
      }
      finally 
      {
        semaphore.Release();
      }
    }

  private static async Task<string> GetHostnameAsync(string ipAddress, CancellationToken cancellationToken)
  {
    try
    {
        var hostEntry = await Dns.GetHostEntryAsync(ipAddress, cancellationToken); 
        return hostEntry.HostName; 
    }
    catch (SocketException)
    {
        
        throw;
    }
  }

   
  }
}

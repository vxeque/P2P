using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Zeroconf;

namespace p2p.services; 

public class UniversalDeviceScanner
{
    public static async Task<List<(string ip, string name, string osType)>> ScanNetworkDevicesAsync(string subnet)
    {
        var devices = new List<(string, string, string)>();
        var tasks = new List<Task>();

        // Escaneo paralelo optimizado
        for (int i = 1; i <= 254; i++)
        {
            string ip = $"{subnet}.{i}";
            tasks.Add(ProcessDeviceAsync(ip, devices));
        }

        await Task.WhenAll(tasks);
        
        // Añadir dispositivos descubiertos via mDNS
        var mdnsDevices = await DiscoverViaMDNSAsync();
        devices.AddRange(mdnsDevices);

        return devices.Distinct().ToList(); // Eliminar duplicados
    }

    private static async Task ProcessDeviceAsync(string ip, List<(string ip, string name, string osType)> devices)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, 200); // Timeout corto

            if (reply.Status == IPStatus.Success)
            {
                var name = await GetDeviceNameAsync(ip);
                var osType = DetectOSByPingTtl(reply.Options.Ttl);
                
                lock (devices)
                {
                    devices.Add((ip, name, osType));
                }
            }
        }
        catch { }
    }

    private static async Task<List<(string ip, string name, string osType)>> DiscoverViaMDNSAsync()
    {
        var results = new List<(string, string, string)>();
        
        try
        {
            var domains = await ZeroconfResolver.BrowseDomainsAsync();
            var responses = await ZeroconfResolver.ResolveAsync(domains.Select(d => d.Key));

            foreach (var resp in responses)
            {
                results.Add((
                    resp.IPAddress,
                    resp.DisplayName,
                    GuessOSByServices(resp.Services.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
                ));
            }
        }
        catch { }

        return results;
    }

    private static async Task<string> GetDeviceNameAsync(string ip)
    {
        try
        {
            // 1. Intento con DNS inverso
            var hostEntry = await Dns.GetHostEntryAsync(ip);
            if (!string.IsNullOrEmpty(hostEntry.HostName))
                return hostEntry.HostName.Split('.')[0];

            // 2. Buscar en dispositivos mDNS
            var mdnsDevices = await ZeroconfResolver.ResolveAsync(new[] { "_services._dns-sd._udp.local." });
            return mdnsDevices.FirstOrDefault(d => d.IPAddress == ip)?.DisplayName ?? $"Dispositivo-{ip.Split('.')[3]}";
        }
        catch
        {
            return $"Dispositivo-{ip.Split('.')[3]}";
        }
    }

    private static string GuessOSByServices(IDictionary<string, IService> services)
    {
        if (services.Any(s => s.Key.Contains("_android"))) return "Android";
        if (services.Any(s => s.Key.Contains("_apple"))) return "iOS/MacOS";
        if (services.Any(s => s.Key.Contains("_windows"))) return "Windows";
        if (services.Any(s => s.Key.Contains("_linux"))) return "Linux";
        return "Desconocido";
    }

    private static string DetectOSByPingTtl(int ttl)
    {
        return ttl switch
        {
            64 => "Linux/Android",
            128 => "Windows",
            255 => "iOS/MacOS",
            _ => "Desconocido"
        };
    }
}
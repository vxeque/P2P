using p2p.services;
using System.Net;

class Program
{
    static async Task Main(string[] args)
    {
        // for windows 
        string filePath = @"C:\Users\victo\Desktop\files\prueba.txt";

        // for linux
        string filePathLinux = @"/home/vxeque/Escritorio/jaba.js";

        FileTransfer fileTransfer = new FileTransfer();
        LanIpScanner lanIpScanner = new LanIpScanner();

        fileTransfer.Port = 8080;

        fileTransfer.LocalIp = IPAddress.Any;

        Console.WriteLine($"Dirección de ip de la maquina local: {lanIpScanner.GetIpLocal()}");

        // List<(string ipAddress, string hostname)>? devices = await LanIpScanner.GetAllIpAddressesAndHostnamesAsync("192.168.0");

        var devicess = await UniversalDeviceScanner.ScanNetworkDevicesAsync("192.168.0");
        foreach (var device in devicess)
        {
            Console.WriteLine($"IP: {device.ip} | Nombre: {device.name} | SO: {device.osType}");
        }
    }
}












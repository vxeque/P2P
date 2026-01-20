using p2p.services;
using System.Net;

/// <summary>
/// Punto de entrada principal de la aplicación P2P.
/// Configura los servicios de transferencia de archivos, escaneo de red local
/// y descubre dispositivos disponibles en la red.
/// </summary>
class Program
{
    /// <summary>
    /// Punto de entrada de la aplicación.
    /// </summary>
    /// <param name="args">Argumentos de línea de comandos pasados a la aplicación.</param>
    /// <remarks>
    /// Este método realiza las siguientes operaciones:
    /// 1. Inicializa los servicios de transferencia de archivos y escaneo de red.
    /// 2. Configura el puerto de transferencia a 8080.
    /// 3. Asigna la dirección IP local.
    /// 4. Muestra la dirección IP de la máquina local.
    /// 5. Escanea los dispositivos disponibles en la red local (192.168.0.x).
    /// 6. Muestra información de cada dispositivo encontrado (IP, nombre y SO).
    /// </remarks>
    static async Task Main(string[] args)
    {
        // for windows 
        // string filePath = @"C:\Users\victo\Desktop\files\prueba.txt";

        // for linux
        // string filePathLinux = @"/home/vxeque/Escritorio/jaba.js";

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












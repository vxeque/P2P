using p2p.services;
using System.Net;

class Program
{
    static async Task Main(string[] args)
    {
        // for windows 
        string filePath = @"C:\Users\User\Desktop\file.txt";
        
        // for linux
        string filePathLinux = @"/home/vxeque/Escritorio/jaba.js"; 

        FileTransfer fileTransfer = new FileTransfer();
        LanIpScanner lanIpScanner = new LanIpScanner(); 

        fileTransfer.Port = 8080;
        fileTransfer.LocalIp = IPAddress.Any;
        
        Console.WriteLine($"Dirección de ip de la maquina local: {lanIpScanner.GetIpLocal()}"); 

        Console.WriteLine($"Local IP: {fileTransfer.LocalIp}:{fileTransfer.Port}");
        // si los archivos son menores a 100 MB se enviaran sin compresión
        await fileTransfer.SendFileAsync(filePathLinux, fileTransfer.LocalIp.ToString());
        
        Console.WriteLine("Enviado");
        
        // si los archivos son mayores a 100 MB se usara la compresión GZip

        // si los archivos son mayores a 1GB  se desfragmentara el archivo en partes de 100MB 
        // comprimirlos 
        
    }
}

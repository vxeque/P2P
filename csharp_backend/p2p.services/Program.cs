using p2p.services;
using System.Net;

class Program
{
    static async Task Main(string[] args)
    {
        
        string filePath = @"C:\Users\User\Desktop\file.txt";

        FileTransfer fileTransfer = new FileTransfer();

        fileTransfer.Port = 8080;
        fileTransfer.LocalIp = IPAddress.Any;

        // si los archivos son menores a 100 MB se enviaran sin compresión
        await fileTransfer.SendFileAsync(filePath, fileTransfer.LocalIp.ToString());
        Console.WriteLine("Enviado");
        
        // si los archivos son mayores a 100 MB se usara la compresión GZip

        // si los archivos son mayores a 1GB  se desfragmentara el archivo en partes de 100MB 
        // comprimirlos 
        
    }
}
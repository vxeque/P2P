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

        await fileTransfer.SendFileAsync(filePath, fileTransfer.LocalIp.ToString());
        // Console.WriteLine("Enviado");
        
    }
}
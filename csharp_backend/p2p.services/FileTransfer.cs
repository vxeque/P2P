using System.Net;
using System.Net.Sockets;
using System.Text;

namespace p2p.services
{
    public class FileTransfer
    {
        private readonly int _port;
        private readonly IPAddress? _localIp;
        private TcpListener? _listener;
        private bool _disposed;

        // properties to access the port and local IP
        public int Port { get; set; }

        // properties to access the port and local IP
        public IPAddress? LocalIp { get; set; }

        // Método para enviar archivo de forma asíncrona
        public async Task SendFileAsync(string filePath, string destinationIp,
            IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(destinationIp, _port, cancellationToken);

                using var stream = client.GetStream();
                var fileName = Path.GetFileName(filePath);

                // Enviar metadatos
                await SendMetadata(stream, fileName, cancellationToken);

                // Enviar contenido del archivo
                await SendFileContent(stream, filePath, progress, cancellationToken);

                Console.WriteLine("Archivo enviado con éxito.");
            }
            catch (Exception)
            {
                // Console.WriteLine($"Error al enviar el archivo: {ex.Message}");
                Console.Error.WriteLine($"Error al enviar el archivo...");
                // throw;
            }
        }

        // Método para recibir archivo de forma asíncrona
        public async Task ReceiveFileAsync(string? saveDirectory = null,
            IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _listener = new TcpListener(_localIp, _port);
                _listener.Start();
                Console.WriteLine("Esperando conexión...");

                using var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                using var stream = client.GetStream();

                // Recibir metadatos
                var (fileName, fileSize) = await ReceiveMetadata(stream, cancellationToken);

                // Recibir contenido del archivo
                await ReceiveFileContent(stream, fileName, fileSize, saveDirectory,
                    progress, cancellationToken);

                Console.WriteLine("Archivo recibido con éxito.");
            }
            finally
            {
                _listener?.Stop();
            }
        }

        private async Task SendMetadata(NetworkStream stream, string fileName,
            CancellationToken cancellationToken)
        {
            var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            var fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);

            await stream.WriteAsync(fileNameLength, 0, fileNameLength.Length, cancellationToken);
            await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length, cancellationToken);
        }

        private async Task<(string fileName, long fileSize)> ReceiveMetadata(NetworkStream stream,
            CancellationToken cancellationToken)
        {
            var fileNameLengthBytes = await ReadExactAsync(stream, 4, cancellationToken);
            var fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

            var fileNameBytes = await ReadExactAsync(stream, fileNameLength, cancellationToken);
            var fileName = Path.GetFileName(Encoding.UTF8.GetString(fileNameBytes));

            var fileSizeBytes = await ReadExactAsync(stream, 8, cancellationToken);
            var fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

            return (fileName, fileSize);
        }

        private async Task SendFileContent(NetworkStream stream, string filePath,
            IProgress<int> progress, CancellationToken cancellationToken)
        {
            var fileInfo = new FileInfo(filePath);
            var fileSize = fileInfo.Length;
            await stream.WriteAsync(BitConverter.GetBytes(fileSize), 0, 8, cancellationToken);

            using var fileStream = File.OpenRead(filePath);
            var buffer = new byte[8192];
            long totalBytesSent = 0;
            int bytesRead;

            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length,
                cancellationToken)) > 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesSent += bytesRead;
                progress?.Report((int)((totalBytesSent * 100) / fileSize));
            }
        }

        private async Task ReceiveFileContent(NetworkStream stream, string fileName,
            long fileSize, string saveDirectory, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            saveDirectory ??= Directory.GetCurrentDirectory();
            Directory.CreateDirectory(saveDirectory);

            var filePath = Path.Combine(saveDirectory, fileName);

            using var fileStream = File.Create(filePath);
            var buffer = new byte[8192];
            long totalBytesReceived = 0;
            int bytesToRead;

            while (totalBytesReceived < fileSize)
            {
                var remaining = fileSize - totalBytesReceived;
                bytesToRead = (int)Math.Min(buffer.Length, remaining);

                var bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead,
                    cancellationToken);
                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                totalBytesReceived += bytesRead;
                progress?.Report((int)((totalBytesReceived * 100) / fileSize));
            }
        }

        private async Task<byte[]> ReadExactAsync(NetworkStream stream, int length,
            CancellationToken cancellationToken)
        {
            var buffer = new byte[length];
            var totalRead = 0;

            while (totalRead < length)
            {
                var read = await stream.ReadAsync(buffer, totalRead, length - totalRead,
                    cancellationToken);
                if (read == 0) throw new EndOfStreamException();
                totalRead += read;
            }
            return buffer;
        }
    }
}

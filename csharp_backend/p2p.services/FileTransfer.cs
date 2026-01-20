using System.Net;
using System.Net.Sockets;
using System.Text;

namespace p2p.services
{
    /// <summary>
    /// Proporciona funcionalidades para transferir archivos entre dispositivos a través de la red
    /// utilizando TCP como protocolo de comunicación.
    /// </summary>
    public class FileTransfer
    {
        private readonly int _port; 
        private readonly IPAddress _localIp;
        private TcpListener? _listener;
        private bool _disposed;

        /// <summary>
        /// Obtiene o establece el puerto TCP utilizado para la transferencia de archivos.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección IP local donde se escucharán las conexiones entrantes.
        /// </summary>
        public IPAddress? LocalIp { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase FileTransfer con configuración predeterminada.
        /// Establece el puerto en 8080 y la dirección IP en cualquier interfaz disponible.
        /// </summary>
        public FileTransfer()
        {
            _port = 8080;
            _localIp = IPAddress.Any;
        }
        
        /// <summary>
        /// Envía un archivo de forma asíncrona a un dispositivo remoto especificado.
        /// Se utiliza para transferir archivos desde este dispositivo hacia otro en la red.
        /// </summary>
        /// <param name="filePath">La ruta local completa del archivo a enviar.</param>
        /// <param name="destinationIp">La dirección IP del dispositivo destino.</param>
        /// <param name="progress">Objeto opcional para reportar el progreso de la transferencia (0-100).</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
        public async Task SendFileAsync(string filePath, string destinationIp,
            IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(destinationIp, _port, cancellationToken);

                using var stream = client.GetStream();
                var fileName = Path.GetFileName(filePath);

                // Enviar metadatos que se envian antes del contenido real
                await SendMetadata(stream, fileName, cancellationToken);

                // Enviar contenido del archivo de forma asíncrona
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

        /// <summary>
        /// Recibe un archivo de forma asíncrona desde un dispositivo remoto que inicia la conexión.
        /// Se utiliza para escuchar conexiones entrantes y recibir archivos en este dispositivo.
        /// </summary>
        /// <param name="saveDirectory">El directorio donde se guardarán los archivos recibidos. 
        /// Si es nulo, se utiliza el directorio actual.</param>
        /// <param name="progress">Objeto opcional para reportar el progreso de la transferencia (0-100).</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
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

        /// <summary>
        /// Envía los metadatos del archivo (nombre y tamaño) al dispositivo remoto.
        /// Los metadatos se transmiten antes del contenido del archivo para preparar la recepción.
        /// </summary>
        /// <param name="stream">El flujo de red donde se escribirán los metadatos.</param>
        /// <param name="fileName">El nombre del archivo a transmitir.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
        private async Task SendMetadata(NetworkStream stream, string fileName,
            CancellationToken cancellationToken)
        {
            var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            var fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);

            await stream.WriteAsync(fileNameLength, 0, fileNameLength.Length, cancellationToken);
            await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length, cancellationToken);
        }

        /// <summary>
        /// Recibe los metadatos del archivo (nombre y tamaño) desde el dispositivo remoto.
        /// Los metadatos se reciben antes del contenido para conocer la información del archivo entrante.
        /// </summary>
        /// <param name="stream">El flujo de red desde donde se leerán los metadatos.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
        /// <returns>Una tupla con el nombre del archivo y su tamaño en bytes.</returns>
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

        /// <summary>
        /// Envía el contenido del archivo por el flujo de red de forma asíncrona.
        /// Divide el archivo en fragmentos para transferencia eficiente y reporta el progreso.
        /// </summary>
        /// <param name="stream">El flujo de red donde se escribirá el contenido.</param>
        /// <param name="filePath">La ruta local del archivo cuyo contenido se enviará.</param>
        /// <param name="progress">Objeto opcional para reportar el progreso de la transferencia.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
        private async Task SendFileContent(NetworkStream stream, string filePath,
            IProgress<int>? progress, CancellationToken cancellationToken)
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

        /// <summary>
        /// Recibe el contenido del archivo desde el flujo de red y lo guarda en el sistema local.
        /// Lee el archivo en fragmentos para eficiencia de memoria y reporta el progreso.
        /// </summary>
        /// <param name="stream">El flujo de red desde donde se leerá el contenido.</param>
        /// <param name="fileName">El nombre del archivo a crear localmente.</param>
        /// <param name="fileSize">El tamaño total del archivo en bytes.</param>
        /// <param name="saveDirectory">El directorio donde se guardará el archivo.</param>
        /// <param name="progress">Objeto opcional para reportar el progreso de la recepción.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
        private async Task ReceiveFileContent(NetworkStream stream, string fileName,
            long fileSize, string? saveDirectory, IProgress<int>? progress,
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

        /// <summary>
        /// Lee una cantidad exacta de bytes desde el flujo de red de forma asíncrona.
        /// Se utiliza para garantizar que se lean todos los bytes esperados, incluso si llegan en múltiples lecturas.
        /// </summary>
        /// <param name="stream">El flujo de red desde donde se leerán los bytes.</param>
        /// <param name="length">La cantidad exacta de bytes a leer.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
        /// <returns>Un arreglo de bytes con la cantidad exacta solicitada.</returns>
        /// <exception cref="EndOfStreamException">Se lanza cuando se alcanza el final del flujo antes de leer todos los bytes.</exception>
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

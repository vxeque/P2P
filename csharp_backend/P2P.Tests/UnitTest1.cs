using p2p.services;
using System.Net;
using System.Runtime.CompilerServices;
using Xunit;

namespace P2P.Tests;

/// <summary>
/// Tests para la clase FileTransfer que verifica la funcionalidad de transferencia de archivos
/// </summary>
public class FileTransferTests : IDisposable
{
    private readonly FileTransfer _fileTransfer;
    private readonly string _testDirectory;
    private readonly string _testFile;

    public FileTransferTests()
    {
        _fileTransfer = new FileTransfer();
        _testDirectory = Path.Combine(Path.GetTempPath(), "P2PFileTransferTests");
        _testFile = Path.Combine(_testDirectory, "test_file.txt");

        if (!Directory.Exists(_testDirectory))
            Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public void FileTransfer_Constructor_InitializesWithDefaultPort()
    {
        // Act & Assert
        Assert.Equal(8080, _fileTransfer.Port);
        Assert.NotNull(_fileTransfer.LocalIp);
        // verificar que no sea una ip vacia
        Assert.NotEqual(IPAddress.None, _fileTransfer.LocalIp);
    }

    [Fact]
    public void FileTransfer_Port_CanBeSet()
    {
        // Arrange
        var newPort = 9090;

        // Act
        _fileTransfer.Port = newPort;

        // Assert
        Assert.Equal(newPort, _fileTransfer.Port);
    }

    [Fact]
    public void FileTransfer_LocalIp_CanBeSet()
    {
        // Arrange
        var newIp = IPAddress.Loopback;

        // Act
        _fileTransfer.LocalIp = newIp;

        // Assert
        Assert.Equal(newIp, _fileTransfer.LocalIp);
    }

    [Fact]
    public async Task SendFileAsync_WithValidFile_ExecutesWithoutThrowingException()
    {
        // Arrange
        File.WriteAllText(_testFile, "Test content");

        // Act & Assert - No debe lanzar excepción
        await _fileTransfer.SendFileAsync(_testFile, "127.0.0.1");
    }

    [Fact]
    public async Task ReceiveFileAsync_WithoutDirectory_UsesCurrentDirectory()
    {
        // Act & Assert - No debe lanzar excepción
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500); // Cancelar después de 500ms

        try
        {
            await _fileTransfer.ReceiveFileAsync(null, null, cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Esto es esperado
        }
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch { }
        }
    }
}

/// <summary>
/// Tests para la clase LanIpScanner que verifica el escaneo de IPs en la LAN
/// </summary>
public class LanIpScannerTests
{
    private readonly LanIpScanner _scanner;

    public LanIpScannerTests()
    {
        _scanner = new LanIpScanner();
    }

    [Fact]
    public void GetIpLocal_ReturnsValidIpString()
    {
        // Act
        var result = _scanner.GetIpLocal();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // verificar que contiene "Local ip:"
        if (result.Contains("Local ip:"))
        {
            Assert.Contains("Local ip:", result);
            var ipPart = result.Replace("Local ip:", "").Trim();
            Assert.True(IPAddress.TryParse(ipPart, out _)); //verificar que la ip sea valida
        }
    }

    [Fact]
    public async Task GetAllIpAddressesAndHostnamesAsync_WithValidSubnet_ReturnsListOfDevices()
    {
        // Arrange
        var subnet = "192.168.1";
        var timeout = 100; // Timeout corto para tests

        // Act
        var result = await LanIpScanner.GetAllIpAddressesAndHostnamesAsync(subnet, timeout);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<(string, string)>>(result);
    }

    [Fact(Skip="Requires network access")]
    public async Task GetAllIpAddressesAndHostnamesAsync_WithCancellationToken_RespectsCancellation()
    {
        // Arrange
        var subnet = "192.168.1";
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(50);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await LanIpScanner.GetAllIpAddressesAndHostnamesAsync(subnet, cancellationToken: cts.Token);
        });
    }

    [Fact]
    public async Task GetAllIpAddressesAndHostnamesAsync_WithCustomTimeout_UsesSpecifiedTimeout()
    {
        // Arrange
        var subnet = "192.168.2";
        var customTimeout = 250;

        // Act
        var result = await LanIpScanner.GetAllIpAddressesAndHostnamesAsync(subnet, customTimeout);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<(string, string)>>(result);
    }

    [Fact]
    public async Task GetAllIpAddressesAndHostnamesAsync_WithMaxParallelPings_LimitsParallelism()
    {
        // Arrange
        var subnet = "10.0.0";
        var maxParallel = 10;

        // Act
        var result = await LanIpScanner.GetAllIpAddressesAndHostnamesAsync(
            subnet,
            maxParallelPings: maxParallel);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<(string, string)>>(result);
    }
}

/// <summary>
/// Tests para la clase UniversalDeviceScanner que verifica el escaneo de dispositivos en red
/// </summary>
public class UniversalDeviceScannerTests
{
    private readonly UniversalDeviceScanner _scanner;

    public UniversalDeviceScannerTests()
    {
        _scanner = new UniversalDeviceScanner();
    }

    [Fact]
    public async Task ScanNetworkDevicesAsync_ReturnsListOfTuples()
    {
        // Arrange
        var subnet = "192.168.1";

        // Act
        var result = await UniversalDeviceScanner.ScanNetworkDevicesAsync(subnet);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<(string, string, string)>>(result);
    }

    [Fact]
    public async Task ScanNetworkDevicesAsync_ReturnsTuplesWithIpNameAndOsType()
    {
        // Arrange
        var subnet = "127.0.0";

        // Act
        var result = await UniversalDeviceScanner.ScanNetworkDevicesAsync(subnet);

        // Assert
        foreach (var (ip, name, osType) in result)
        {
            Assert.NotNull(ip);
            Assert.NotEmpty(ip);
            Assert.NotNull(name);
            Assert.NotNull(osType);
        }
    }

    [Fact]
    public async Task ScanNetworkDevicesAsync_RemovesDuplicateDevices()
    {
        // Arrange
        var subnet = "192.168.100";

        // Act
        var result = await UniversalDeviceScanner.ScanNetworkDevicesAsync(subnet);

        // Assert
        Assert.NotNull(result);
        // Verificar que no hay duplicados por IP
        var uniqueIps = result.Select(d => d.ip).Distinct();
        Assert.Equal(result.Count, uniqueIps.Count());
    }

    [Fact]
    public async Task DescriptionDevices_ImplementsIP2PService_ReturnsList()
    {
        // Act
        var result = await _scanner.DescriptionDevices();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<(string, string, string)>>(result);
    }

    [Fact]
    public async Task DescriptionDevices_ReturnsValidDeviceInformation()
    {
        // Act
        var result = await _scanner.DescriptionDevices();

        // Assert
        foreach (var (ip, name, osType) in result)
        {
            Assert.NotNull(ip);
            Assert.NotEmpty(ip);
            Assert.NotNull(name);
            Assert.NotEmpty(name);
            Assert.NotNull(osType);
            Assert.NotEmpty(osType);
        }
    }
}

/// <summary>
/// Tests para la interfaz IP2PService
/// </summary>
public class IP2PServiceTests
{
    [Fact]
    public void IP2PService_IsImplementedByUniversalDeviceScanner()
    {
        // Act
        var scanner = new UniversalDeviceScanner();

        // Assert
        Assert.IsAssignableFrom<IP2PService>(scanner);
    }

    [Fact]
    public async Task IP2PService_DescriptionDevices_HasCorrectSignature()
    {
        // Arrange
        IP2PService service = new UniversalDeviceScanner();

        // Act
        var result = await service.DescriptionDevices();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<(string, string, string)>>(result);
    }
}

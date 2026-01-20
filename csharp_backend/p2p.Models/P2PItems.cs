using System.ComponentModel.DataAnnotations;

namespace p2p.models
{
    /// <summary>
    /// Representa un elemento o dispositivo P2P en la base de datos.
    /// Contiene información completa sobre un dispositivo con un identificador único.
    /// </summary>
    public class P2PItems
    {
        /// <summary>
        /// Obtiene o establece el identificador único del elemento P2P.
        /// </summary>
        /// <value>
        /// Identificador único generado automáticamente con formato: yyyyMMddHHmmssfff-GUID.
        /// Se genera en UTC si no se proporciona un valor.
        /// </value>
        public string? Id { get; set; } = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid()}";

        /// <summary>
        /// Obtiene o establece el nombre del dispositivo.
        /// </summary>
        /// <value>El nombre o hostname del dispositivo P2P.</value>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de dispositivo.
        /// </summary>
        /// <value>El tipo o categoría del dispositivo (ej: Computer, Mobile, IoT).</value>
        public string? DeviceType { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección IP del dispositivo.
        /// </summary>
        /// <value>La dirección IP del dispositivo en la red.</value>
        public string? DeviceIp { get; set; }

    }

    /// <summary>
    /// Objeto de transferencia de datos (DTO) para P2P Items.
    /// Utilizado para transferir información de dispositivos sin exponer el identificador único.
    /// </summary>
    public class P2PItemsDto
    {
        /// <summary>
        /// Obtiene o establece el nombre del dispositivo.
        /// </summary>
        /// <value>El nombre o hostname del dispositivo P2P.</value>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de dispositivo.
        /// </summary>
        /// <value>El tipo o categoría del dispositivo (ej: Computer, Mobile, IoT).</value>
        public string? DeviceType { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección IP del dispositivo.
        /// </summary>
        /// <value>La dirección IP del dispositivo en la red.</value>
        public string? DeviceIp { get; set; }
    }

}
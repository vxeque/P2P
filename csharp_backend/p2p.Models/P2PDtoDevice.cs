using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p2p.models
{
    /// <summary>
    /// Representa un dispositivo en la red P2P.
    /// Contiene información básica sobre un dispositivo detectado en la red local.
    /// </summary>
    public class P2PDtoDevice
    {
        /// <summary>
        /// Obtiene o establece el nombre del dispositivo.
        /// </summary>
        /// <value>El nombre o hostname del dispositivo en la red.</value>
        public string? name { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección IP del dispositivo.
        /// </summary>
        /// <value>La dirección IP del dispositivo en formato estándar (ej: 192.168.1.100).</value>
        public string? ip { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de sistema operativo del dispositivo.
        /// </summary>
        /// <value>El tipo de SO (ej: Windows, Linux, macOS).</value>
        public string? osType { get; set; }
    }
}


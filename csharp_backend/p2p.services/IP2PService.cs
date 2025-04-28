using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p2p.services
{
    public interface IP2PService
    {
        Task<List<(string ip, string name, string osType)>> DescriptionDevices();
    }
}

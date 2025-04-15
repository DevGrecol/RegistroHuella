using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaDigitalPersonRegistrar.Conexion
{
    using System;
    using System.Collections.Generic;
    public partial class Cliente
    {

        public byte[] huella { get; set; }

        public int id_cliente { get; set; }

        public string nombres { get; set; }

        public string apellidos { get; set; }

        public string numero_identificacion { get; set; }

        public int codigo_ver { get; set; }

    }

}
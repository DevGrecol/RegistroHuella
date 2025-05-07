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

        public byte[] MeñiqueIzquierdo { get; set; }
        public byte[] AnularIzquierdo { get; set; }
        public byte[] MedioIzquierdo { get; set; }
        public byte[] IndiceIzquierdo { get; set; }
        public byte[] PulgarIzquierdo { get; set; }
        public byte[] MeñiqueDerecho { get; set; }
        public byte[] AnularDerecho { get; set; }
        public byte[] MedioDerecho { get; set; }
        public byte[] IndiceDerecho { get; set; }
        public byte[] PulgarDerecho { get; set; }
    }

}
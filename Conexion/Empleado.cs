using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaDigitalPersonRegistrar.Conexion
{
    using System;
    using System.Collections.Generic;

    public partial class Empleado
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public byte[] huella { get; set; }
    }
}

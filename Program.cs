using PruebaDigitalPersonRegistrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PruebaDigitalPersonRegistrar
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //-----Cambio para no mostrar Main-----
            Application.Run(new frmRegistrar());
        }
    }
}

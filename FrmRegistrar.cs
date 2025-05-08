using PruebaDigitalPersonRegistrar.Conexion;
using PruebaDigitalPersonRegistrar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using System.Diagnostics;
using System.Threading.Tasks;
using DPFP.Verification;
using System.IO;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace PruebaDigitalPersonRegistrar
{
    public partial class frmRegistrar : Form
    {
        private DPFP.Template //Template,
            TemplateMeñiqueIzquierdo,
            TemplateAnularIzquierdo,
            TemplateMedioIzquierdo,
            TemplateIndiceIzquierdo,
            TemplatePulgarIzquierdo,
            TemplatePulgarDerecho,
            TemplateIndiceDerecho,
            TemplateMedioDerecho,
            TemplateAnularDerecho,
            TemplateMeñiqueDerecho;

        private string dedoSeleccionado;

        private ConexionBD contexto;

        public frmRegistrar()
        {
            InitializeComponent();
        }

        private static readonly Random random = new Random();
        private string ObtenerDedoAleatorio()
        {
            string[] nombresDedos = {
                "MeñiqueIzquierdo", "AnularIzquierdo", "MedioIzquierdo", "IndiceIzquierdo", "PulgarIzquierdo",
                "PulgarDerecho", "IndiceDerecho", "MedioDerecho", "AnularDerecho", "MeñiqueDerecho"
            };
            int indiceAleatorio = random.Next(nombresDedos.Length);
            return nombresDedos[indiceAleatorio];
        }

        private void btnRegistrarHuella_Click(object sender, EventArgs e)
        {
            if (txtDocumento.Text == "" || txtIdentificadorCliente.Text == "")
            {
                Show("Error al consultar el cliente: debe de ingresar una cedula de cliente registrado", "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        Color.LightCoral,
                        Color.IndianRed,
                        Color.Black);

            }
            else
            {
                dedoSeleccionado = ObtenerDedoAleatorio();
                CapturarHuella capturar1 = new CapturarHuella(dedoSeleccionado);
                capturar1.OnTemplate += (template) => OnTemplate((DPFP.Template)template, dedoSeleccionado);
                capturar1.ShowDialog();
            }
        }
        // -------- Redondear bordes de los botones --------
        // Importa la función SetWindowRgn de user32.dll
        [DllImport("gdi32.dll")]
        public static extern int CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        private void RedondearBoton(Button boton, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(boton.ClientRectangle.Left, boton.ClientRectangle.Top, radio, radio, 180, 90);
            path.AddArc(boton.ClientRectangle.Right - radio, boton.ClientRectangle.Top, radio, radio, 270, 90);
            path.AddArc(boton.ClientRectangle.Right - radio, boton.ClientRectangle.Bottom - radio, radio, radio, 0, 90);
            path.AddArc(boton.ClientRectangle.Left, boton.ClientRectangle.Bottom - radio, radio, radio, 90, 90);
            path.CloseAllFigures();

            Region region = new Region(path);
            boton.Region = region;
        }

        private void Capturar_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (TemplateMeñiqueIzquierdo != null ||
                    TemplateAnularIzquierdo != null ||
                    TemplateMedioIzquierdo != null ||
                    TemplateIndiceIzquierdo != null ||
                    TemplatePulgarIzquierdo != null ||
                    TemplatePulgarDerecho != null ||
                    TemplateIndiceDerecho != null ||
                    TemplateMedioDerecho != null ||
                    TemplateAnularDerecho != null ||
                    TemplateMeñiqueDerecho != null)
            {
                Show("La plantilla de huella dactilar está lista para la verificación de huella dactilar.",
                            "Registro de huella dactilar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            Color.LightGreen,
                            Color.LightGreen,
                            Color.Black);
            }
        }



        private void OnTemplate(DPFP.Template template, string dedo)
        {
            this.Invoke(new Function(delegate ()
            {
                if (template != null)
                {
                    txtHuella.Text = "Huella capturada correctamente";
                    btnAgregar.BackColor = Color.LightGreen;
                    btnAgregar.ForeColor = Color.DarkGreen;

                    // Mostrar el mensaje de plantilla lista para guardar


                    switch (dedo)
                    {
                        case "MeñiqueIzquierdo": TemplateMeñiqueIzquierdo = template; break;
                        case "AnularIzquierdo": TemplateAnularIzquierdo = template; break;
                        case "MedioIzquierdo": TemplateMedioIzquierdo = template; break;
                        case "IndiceIzquierdo": TemplateIndiceIzquierdo = template; break;
                        case "PulgarIzquierdo": TemplatePulgarIzquierdo = template; break;
                        case "PulgarDerecho": TemplatePulgarDerecho = template; break;
                        case "IndiceDerecho": TemplateIndiceDerecho = template; break;
                        case "MedioDerecho": TemplateMedioDerecho = template; break;
                        case "AnularDerecho": TemplateAnularDerecho = template; break;
                        case "MeñiqueDerecho": TemplateMeñiqueDerecho = template; break;
                    }

                    Show("Plantilla lista para guardar.",
                    "Registro de huella",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    Color.LightGreen,
                    Color.LightGreen,
                    Color.Black);

                }
                else
                {
                    Show("La plantilla de huella dactilar no es válida. Repita el registro de huella dactilar",
                         "Error",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Information,
                         Color.LightCoral,
                         Color.IndianRed,
                         Color.White);
                    btnAgregar.BackColor = SystemColors.Control;
                    btnAgregar.ForeColor = SystemColors.ControlText;
                }


                btnAgregar.Enabled = TemplateMeñiqueIzquierdo != null || TemplateAnularIzquierdo != null ||
                                    TemplateMedioIzquierdo != null || TemplateIndiceIzquierdo != null ||
                                    TemplatePulgarIzquierdo != null || TemplatePulgarDerecho != null ||
                                    TemplateIndiceDerecho != null || TemplateMedioDerecho != null ||
                                    TemplateAnularDerecho != null || TemplateMeñiqueDerecho != null;
            }));
        }


        private void frmRegistrar_Load(object sender, EventArgs e)
        {
            contexto = new ConexionBD();
            //Listar();


            //-----Diseño Boton-----
            btnAgregar.BackColor = SystemColors.Control;
            btnAgregar.ForeColor = SystemColors.ControlText;
            btnAgregar.Font = new Font("Verdana", 8, FontStyle.Regular);
            btnAgregar.FlatStyle = FlatStyle.Flat;
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.FlatAppearance.MouseOverBackColor = Color.LightGray;
            btnAgregar.FlatAppearance.MouseDownBackColor = Color.DimGray;

            RedondearBoton(btnAgregar, 15);
        }

        private void Limpiar()
        {
            txtIdentificadorCliente.Text = "";
        }

        /*private void Listar()
        {
            string query = "SELECT nombre FROM empleados"; // Reemplaza 'empleados' con el nombre de tu tabla
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["nombre"].ToString());
                        }
                    }
                }
            }
        } */

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                List<Tuple<string, byte[]>> nuevasHuellas = new List<Tuple<string, byte[]>>();
                if (TemplateMeñiqueIzquierdo?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Meñique Izquierdo", TemplateMeñiqueIzquierdo.Bytes));
                if (TemplateAnularIzquierdo?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Anular Izquierdo", TemplateAnularIzquierdo.Bytes));
                if (TemplateMedioIzquierdo?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Medio Izquierdo", TemplateMedioIzquierdo.Bytes));
                if (TemplateIndiceIzquierdo?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Indice Izquierdo", TemplateIndiceIzquierdo.Bytes));
                if (TemplatePulgarIzquierdo?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Pulgar Izquierdo", TemplatePulgarIzquierdo.Bytes));
                if (TemplatePulgarDerecho?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Pulgar Derecho", TemplatePulgarDerecho.Bytes));
                if (TemplateIndiceDerecho?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Indice Derecho", TemplateIndiceDerecho.Bytes));
                if (TemplateMedioDerecho?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Medio Derecho", TemplateMedioDerecho.Bytes));
                if (TemplateAnularDerecho?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Anular Derecho", TemplateAnularDerecho.Bytes));
                if (TemplateMeñiqueDerecho?.Bytes != null) nuevasHuellas.Add(Tuple.Create("Meñique Derecho", TemplateMeñiqueDerecho.Bytes));

                Cliente cliente = new Cliente()
                {
                    id_cliente = Convert.ToInt32(txtIdentificadorCliente.Text),
                    MeñiqueIzquierdo = TemplateMeñiqueIzquierdo?.Bytes,
                    AnularIzquierdo = TemplateAnularIzquierdo?.Bytes,
                    MedioIzquierdo = TemplateMedioIzquierdo?.Bytes,
                    IndiceIzquierdo = TemplateIndiceIzquierdo?.Bytes,
                    PulgarIzquierdo = TemplatePulgarIzquierdo?.Bytes,
                    PulgarDerecho = TemplatePulgarDerecho?.Bytes,
                    IndiceDerecho = TemplateIndiceDerecho?.Bytes,
                    MedioDerecho = TemplateMedioDerecho?.Bytes,
                    AnularDerecho = TemplateAnularDerecho?.Bytes,
                    MeñiqueDerecho = TemplateMeñiqueDerecho?.Bytes
                };

                contexto.GuardarHuellaCliente(cliente);
                contexto.SaveChanges();
                Show("Huellas agregadas al cliente correctamente",
                            "Éxito",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            Color.LightGreen,
                            Color.LightGreen,
                            Color.Black);
                LimpiarCamposCliente();
                LimpiarTemplatesCliente();
                BtnCerrado();

                TemplateMeñiqueIzquierdo = null;
                TemplateAnularIzquierdo = null;
                TemplateMedioIzquierdo = null;
                TemplateIndiceIzquierdo = null;
                TemplatePulgarIzquierdo = null;
                TemplatePulgarDerecho = null;
                TemplateIndiceDerecho = null;
                TemplateMedioDerecho = null;
                TemplateAnularDerecho = null;
                TemplateMeñiqueDerecho = null;
                btnAgregar.Enabled = false;
            }
            catch (Exception ex)
            {
                Show(ex.Message, "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information,
                                 Color.LightCoral,
                                 Color.IndianRed,
                                 Color.White);
            }
        }

        private void LimpiarCamposCliente()
        {
            txtDocumento.Text = "";
            txtNombreCliente.Text = "";
            txtApellidoCliente.Text = "";
            txtIdentificadorCliente.Text = "";
            txtHuella.Text = "";
        }

        private void LimpiarTemplatesCliente()
        {
            TemplateMeñiqueIzquierdo = null;
            TemplateAnularIzquierdo = null;
            TemplateMedioIzquierdo = null;
            TemplateIndiceIzquierdo = null;
            TemplatePulgarIzquierdo = null;
            TemplatePulgarDerecho = null;
            TemplateIndiceDerecho = null;
            TemplateMedioDerecho = null;
            TemplateAnularDerecho = null;
            TemplateMeñiqueDerecho = null;
            //Template = null;
        }

        private void BtnCerrado()
        {
            btnAgregar.Enabled = false;
            btnAgregar.BackColor = SystemColors.Control;
        }

        private void buttonBuscar_Click(object sender, EventArgs e)
        {
            if (txtDocumento.Text == "")
            {
                Show("Error al consultar el cliente: debe de ingresar la cedula del cliente",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            Color.LightCoral,
                            Color.IndianRed,
                            Color.White);
            }
            else
            {
                contexto = new ConexionBD();

                try
                {
                    Cliente cliente = new Cliente()
                    {
                        numero_identificacion = txtDocumento.Text,
                    };

                    // Método para consultar un usuario
                    contexto.ConsultarCliente(cliente);

                    txtNombreCliente.Text = cliente.nombres;
                    txtApellidoCliente.Text = cliente.apellidos;

                    if (cliente.MeñiqueIzquierdo != null |
                        cliente.AnularIzquierdo != null |
                        cliente.MedioIzquierdo != null |
                        cliente.IndiceIzquierdo != null |
                        cliente.PulgarIzquierdo != null |
                        cliente.PulgarDerecho != null |
                        cliente.IndiceDerecho != null |
                        cliente.MedioDerecho != null |
                        cliente.AnularDerecho != null |
                        cliente.MeñiqueDerecho != null)

                    {
                        btnRegistrar.Enabled = false;
                    }
                    else if (cliente.nombres == "0" || cliente.apellidos == "0")
                    {
                        Show("El cliente consultado no existe", "Error",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information,
                                     Color.LightCoral,
                                     Color.IndianRed,
                                     Color.White);
                    }
                    else if (cliente.MeñiqueIzquierdo != null |
                            cliente.AnularIzquierdo != null |
                            cliente.MedioIzquierdo != null |
                            cliente.IndiceIzquierdo != null |
                            cliente.PulgarIzquierdo != null |
                            cliente.PulgarDerecho != null |
                            cliente.IndiceDerecho != null |
                            cliente.MedioDerecho != null |
                            cliente.AnularDerecho != null |
                            cliente.MeñiqueDerecho != null)
                    {
                        btnRegistrar.Enabled = false;
                        btnAgregar.Enabled = false;
                    }
                    else
                    {
                        btnRegistrar.Enabled = true;
                    }

                    txtIdentificadorCliente.Text = Convert.ToString(cliente.id_cliente);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar el cliente: " + ex.Message, "Error");
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtDocumento_TextChanged(object sender, EventArgs e)
        {


        }




        private void txtDocumento_Click(object sender, EventArgs e)
        {
            txtDocumento.Text = Clipboard.GetText();

            string numero_identificacion = txtDocumento.Text;

            if (numero_identificacion != null || numero_identificacion != " ")
            {

                Task.Delay(new TimeSpan(0, 0, 1)).ContinueWith(o => {

                    //btnRegistrarHuella_Click(sender, e);

                    this.buscaClienteRegistrar(numero_identificacion, sender, e);

                });

            }
            else
            {

                Show("Falta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

            }

        }


        private void buscaClienteRegistrar(string numero_identificacion, object sender, EventArgs e)
        {
            // Elimina espacios en blanco al principio y al final del texto ingresado
            string documentoTrimmed = txtDocumento.Text.Trim();

            if (string.IsNullOrEmpty(documentoTrimmed)) // Verifica si está vacío o solo contiene espacios
            {
                Show("Error al consultar el cliente: debe de ingresar la cédula del cliente",
                         "Error",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Information,
                         Color.LightCoral,
                         Color.IndianRed,
                         Color.White);
            }
            else
            {
                contexto = new ConexionBD();

                try
                {
                    Cliente cliente = new Cliente()
                    {
                        numero_identificacion = documentoTrimmed, // Usa el valor trimmeado
                    };

                    ///metodo para consultar un usuario
                    contexto.ConsultarCliente(cliente);
                    //-------------------------------------------------------------------------------//

                    txtNombreCliente.Text = cliente.nombres;
                    txtApellidoCliente.Text = cliente.apellidos;

                    if (cliente.nombres == "0" || cliente.apellidos == "0")
                    {
                        Show("El cliente consultado no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
                    }
                    else
                    {
                        if (cliente.MeñiqueIzquierdo != null |
                            cliente.AnularIzquierdo != null |
                            cliente.MedioIzquierdo != null |
                            cliente.IndiceIzquierdo != null |
                            cliente.PulgarIzquierdo != null |
                            cliente.PulgarDerecho != null |
                            cliente.IndiceDerecho != null |
                            cliente.MedioDerecho != null |
                            cliente.AnularDerecho != null |
                            cliente.MeñiqueDerecho != null)
                        {
                            btnRegistrar.Enabled = false;
                            btnAgregar.Enabled = false;
                        }
                        else
                        {
                            btnRegistrar.Enabled = true;
                            txtIdentificadorCliente.Text = Convert.ToString(cliente.id_cliente);
                            btnRegistrarHuella_Click(sender, e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Show("Error al consultar el cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
                }
            }
        }

        private void txtApellidoCliente_TextChanged(object sender, EventArgs e)
        {

        }


        public static Color ColorIntermedio(Color color1, Color color2)
        {
            return Color.FromArgb(
                (color1.R + color2.R) / 2,
                (color1.G + color2.G) / 2,
                (color1.B + color2.B) / 2);
        }

        public static Point _mouseDownPoint = Point.Empty;

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, Color color1, Color color2, Color foreColor)
        {
            Form messageBoxForm = new Form
            {
                Text = caption,
                ForeColor = foreColor,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(400, 200),
                FormBorderStyle = FormBorderStyle.None,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Panel customTitleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = color1,
                ForeColor = foreColor,
            };
            messageBoxForm.Controls.Add(customTitleBar);

            Label titleLabel = new Label
            {
                Text = caption,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Black,
                Font = new Font("Calibri", 12, FontStyle.Bold),
                BackColor = Color.Transparent,
                Location = new Point(5, 5)
            };
            customTitleBar.Controls.Add(titleLabel);


            customTitleBar.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    _mouseDownPoint = new Point(e.X, e.Y);
                }
            };

            customTitleBar.MouseMove += (sender, e) =>
            {
                if (_mouseDownPoint != Point.Empty)
                {
                    messageBoxForm.Location = new Point(
                        messageBoxForm.Left + (e.X - _mouseDownPoint.X),
                        messageBoxForm.Top + (e.Y - _mouseDownPoint.Y));
                }
            };

            customTitleBar.MouseUp += (sender, e) =>
            {
                _mouseDownPoint = Point.Empty;
            };

            Button closeButton = new Button
            {
                Text = "X",
                AutoSize = false,
                Width = 20,
                Height = 22,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(customTitleBar.Width - 25, (customTitleBar.Height - 22) / 2),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            closeButton.Click += (sender, e) =>
            {
                messageBoxForm.DialogResult = DialogResult.Cancel;
                messageBoxForm.Close();
            };
            customTitleBar.Controls.Add(closeButton);
            customTitleBar.Width = messageBoxForm.ClientSize.Width;
            messageBoxForm.Resize += (sender, e) =>
            {
                customTitleBar.Width = messageBoxForm.ClientSize.Width;
                closeButton.Left = customTitleBar.Width - closeButton.Width - 5;
            };



            messageBoxForm.Tag = new Tuple<Color, Color>(color1, color2);


            messageBoxForm.Paint += (sender, e) =>
            {
                Form form = (Form)sender;
                if (form.Tag is Tuple<Color, Color> colors)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(form.ClientRectangle, colors.Item1, colors.Item2, LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(brush, form.ClientRectangle);
                    }
                }
            };

            Label messageLabel = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Black,
                Font = new Font("Calibri", 16),
                BackColor = Color.Transparent,
                Padding = new Padding(0, customTitleBar.Height, 0, 40)
            };
            messageBoxForm.Controls.Add(messageLabel);


            Color buttonBackColor = ColorIntermedio(color1, color2);

            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Font = new Font("Calibri", 16),
                ForeColor = Color.Black,
                BackColor = buttonBackColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Dock = DockStyle.Bottom,
                Height = 40
            };
            messageBoxForm.Controls.Add(okButton);

            return messageBoxForm.ShowDialog();
        }
    }
}

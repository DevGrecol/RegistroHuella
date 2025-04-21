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
        private DPFP.Template Template;
        private ConexionBD contexto;

        public frmRegistrar()
        {
            InitializeComponent();
        }

   
        private void btnRegistrarHuella_Click(object sender, EventArgs e)
        {
            if (txtDocumento.Text == "" || txtIdentificadorCliente.Text == "")
            {
                Show("Error al consultar el cliente: debe de ingresar una cedula de cliente registrado","Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    Color.LightCoral,
                    Color.IndianRed,
                    Color.Black);

            }
            else
            {
                CapturarHuella capturar = new CapturarHuella();
                capturar.OnTemplate += this.OnTemplate;
                capturar.FormClosed += Capturar_FormClosed;
                capturar.ShowDialog();
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


        // Replace the usage of 'CustomMessageBox.Show' with the static 'Show' method defined in the same class.  
        private void Capturar_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Template != null)
            {
                Show("La plantilla de huella dactilar está lista para la verificación de huella dactilar.",
                     "Registro de huella dactilar",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information,
                     Color.LightGreen, // Verde más claro para el fondo
                     Color.LightGreen,      // Verde un poco más oscuro para el borde
                     Color.Black);
            }
        }


        // Replacing 'CustomMessageBox.Show' with the 'Show' method already defined in the same class.  
        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Function(delegate ()
            {
                Template = template;
                btnAgregar.Enabled = (Template != null);

                if (Template != null)
                {
                    txtHuella.Text = "Huella capturada correctamente";
                    btnAgregar.BackColor = Color.LightGreen;
                    btnAgregar.ForeColor = Color.DarkGreen;
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
                byte[] streamHuella = Template.Bytes;

                Cliente cliente = new Cliente()
                {
                    id_cliente = Convert.ToInt32(txtIdentificadorCliente.Text),
                    huella = streamHuella,
                };

                contexto.GuardarHuellaCliente(cliente);
                contexto.SaveChanges();
                Show("Huella agregada al cliente correctamente",
                     "Éxito",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information,
                     Color.LightGreen,
                     Color.LightGreen,  
                     Color.Black);
                Limpiar();
                Template = null;
                btnAgregar.Enabled = false;

                txtDocumento.Text = "";
                txtNombreCliente.Text = "";
                txtApellidoCliente.Text = "";
                txtIdentificadorCliente.Text = "";
                txtHuella.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

                    if (cliente.huella != null)
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
            else {

                Show("Falta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

            }

        }


        private void buscaClienteRegistrar(string numero_identificacion, object sender, EventArgs e)
        {


            if (txtDocumento.Text == "")
            {



                Show("Error al consultar el cliente: debe de ingresar la cedula del cliente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

            }
            else
            {

                contexto = new ConexionBD();

                try
                {

                    Cliente cliente = new Cliente()
                    {
                        numero_identificacion = numero_identificacion,
                    };

                    ///metodo para consultar un usuario
                    contexto.ConsultarCliente(cliente);


                    txtNombreCliente.Text = cliente.nombres;
                    txtApellidoCliente.Text = cliente.apellidos;

                    if (cliente.nombres == "0" || cliente.apellidos == "0") {


                        Show("El cliente consultado no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

                    }
                    else {

                        if (cliente.huella != null)
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

                    //btnAgregar.Enabled = false;

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

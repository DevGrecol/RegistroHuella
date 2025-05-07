using PruebaDigitalPersonRegistrar;
using PruebaDigitalPersonRegistrar.Conexion;
using System;
using System.IO;
using DPFP;
using DPFP.Verification;
using Npgsql;
using System.Windows.Forms;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace PruebaDigitalPersonRegistrar
{
    public partial class frmVerificar : CaptureFormVerificar
    {
        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;
        private ConexionBD contexto;

        public void Verify(DPFP.Template template)
        {
            Template = template;
            ShowDialog();
        }

        protected override void Init()
        {
            base.Init();
            base.Text = "Verificación de Huella Digital";
            Verificator = new DPFP.Verification.Verification();
            //UpdateStatus(0);
        }

        //private void UpdateStatus(int FAR)
        //{
        //    SetStatus(String.Format("Tasa de Aceptación Falsa (FAR) = {0}", FAR));
        //}

        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);

            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            if (features != null)
            {
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                DPFP.Template template;
                Stream stream;
                bool huellaVerificada = false;

                try
                {
                    string query = "SELECT id_cliente, nombres, apellidos, codigo_ver, MeñiqueIzquierdo, AnularIzquierdo, MedioIzquierdo, IndiceIzquierdo, PulgarIzquierdo, PulgarDerecho, IndiceDerecho, MedioDerecho, AnularDerecho, MeñiqueDerecho , numero_identificacion FROM public.clientes order by id_cliente desc;";

                    using (NpgsqlConnection conn = new NpgsqlConnection(contexto.connectionString))
                    {
                        conn.Open();
                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    bool algunaHuellaAsignada = false;

                                    // Iterar a través de las 10 columnas de huellas
                                    for (int i = 4; i < 14; i++) // Columnas de MeñiqueIzquierdo a MeñiqueDerecho
                                    {
                                        if (!reader.IsDBNull(i))
                                        {
                                            algunaHuellaAsignada = true;
                                            byte[] huellaBytes = (byte[])reader[i];
                                            stream = new MemoryStream(huellaBytes);
                                            template = new DPFP.Template(stream);

                                            Verificator.Verify(features, template, ref result);

                                            if (result.Verified)
                                            {
                                                Random rnd = new Random();
                                                int cardPago = rnd.Next(1, 1000);
                                                string mostrarCardPago = cardPago.ToString();

                                                if (cardPago >= 1 && cardPago <= 9)
                                                {
                                                    labelNumeroPago.Text = "00" + mostrarCardPago;
                                                }
                                                else if (cardPago >= 10 && cardPago <= 99)
                                                {
                                                    labelNumeroPago.Text = "0" + mostrarCardPago;
                                                }
                                                else
                                                {
                                                    labelNumeroPago.Text = mostrarCardPago;
                                                }

                                                int nume = (int)reader.GetValue(0);
                                                int update = this.actualizarCodigo(cardPago, nume);

                                                txtEncontradoNombre.Text = reader.GetValue(1).ToString();
                                                txtEncontradoApellido.Text = reader.GetValue(2).ToString();
                                                txtEncontradoCedula.Text = reader.GetValue(14).ToString();

                                                MakeReport("La huella dactilar pertenece al cliente. " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString());
                                                huellaVerificada = true;
                                                goto VerificacionExitosa; // Salir del bucle si se encuentra una coincidencia
                                            }
                                        }
                                    }

                                    if (!algunaHuellaAsignada)
                                    {
                                        txtEncontradoNombre.Text = " ";
                                        txtEncontradoApellido.Text = " ";
                                        txtEncontradoCedula.Text = " ";
                                        labelNumeroPago.Text = "XXX";
                                        // Puedes decidir si quieres mostrar un mensaje aquí por cada cliente sin huellas asignadas.
                                    }
                                }

                            VerificacionExitosa:
                                if (!huellaVerificada)
                                {
                                    MakeReport("La huella dactilar NO fue encontrada en la base de datos.");
                                    txtEncontradoNombre.Text = " ";
                                    txtEncontradoApellido.Text = " ";
                                    txtEncontradoCedula.Text = " ";
                                    labelNumeroPago.Text = "XXX";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MakeReport("Error durante la verificación: " + ex.Message);
                }
            }
        }

        public frmVerificar()
        {
            contexto = new ConexionBD();
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }


        public int actualizarCodigo(int codigo, int id_cliente)
        {


            using (NpgsqlConnection connection = new NpgsqlConnection(contexto.connectionString))
            {
                try
                {
                    connection.Open();

                    //string sql = "INSERT INTO empleados (nombre, huella) VALUES (@nombre, @huella)";

                    string sql = "UPDATE clientes SET codigo_ver = " + codigo + " WHERE id_cliente= " + id_cliente + "";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@codigo_ver", codigo);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException ex)
                {

                    Show("Error de PostgreSQL: " + ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

                    throw;
                }
                catch (Exception ex)
                {

                    Show("Error al guardar cliente: " + ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

                    throw;
                }
            }

            return 0;

        }

        private void txtEncontradoNombre_TextChanged(object sender, EventArgs e)
        {

        }


        public static new Color ColorIntermedio(Color color1, Color color2)
        {
            return Color.FromArgb(
                (color1.R + color2.R) / 2,
                (color1.G + color2.G) / 2,
                (color1.B + color2.B) / 2);
        }

        public static new Point _mouseDownPoint = Point.Empty;

        public static new DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, Color color1, Color color2, Color foreColor)
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
using DPFP.Verification;
using Npgsql;
using NPOI.SS.UserModel;
using PruebaDigitalPersonRegistrar.Conexion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;

namespace PruebaDigitalPersonRegistrar
{
    /* NOTE: This form is a base for the EnrollmentForm and the VerificationForm,
		All changes in the CaptureForm will be reflected in all its derived forms.
	*/

    delegate void UpdateUIHandler();

    public partial class CaptureFormVerificar : Form, DPFP.Capture.EventHandler
    {
        //private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;
        //private ConexionBD contexto;

        public string connectionString = ("Host=localhost;Username=postgres;Password=31415926;Database=cali_17_03_2025;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;");

        public CaptureFormVerificar()
        {
            InitializeComponent();
        }

        protected virtual void Init()
        {

            try
            {
                Capturer = new DPFP.Capture.Capture();				// Create a capture operation.

                if (null != Capturer)
                    Capturer.EventHandler = this;					// Subscribe for capturing events.
                else
                    SetPrompt("No se pudo iniciar la operaci n de captura");
            }
            catch
            {
                Show("No se pudo iniciar la operaci n de captura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
            }
        }

        protected virtual void Process(DPFP.Sample Sample)
        {
            // Draw fingerprint sample image.



            Verificator = new DPFP.Verification.Verification();

            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            if (features != null)
            {
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                DPFP.Template template;
                Stream stream;

                try
                {
                    //string query = "SELECT nombre, huella, FROM clientes";
                    string query = "SELECT id_cliente, nombres, apellidos, codigo_ver,  meñiqueizquierdo, anularizquierdo, medioizquierdo, indiceizquierdo, pulgarizquierdo, pulgarderecho, indicederecho, medioderecho, anularderecho, meñiquederecho, numero_identificacion FROM public.clientes order by id_cliente desc;";

                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                    {

                        conn.Open();

                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {

                                    Boolean validador = reader.IsDBNull(4) ? false : true;

                                    if (validador == false)
                                    {
                                        //MessageBox.Show("La huella no esta asignada a ningun cliente");


                                        //break
                                    }
                                    else
                                    {


                                        byte[] huellaBytes = (byte[])reader["huella"];
                                        stream = new MemoryStream(huellaBytes);
                                        template = new DPFP.Template(stream);

                                        Verificator.Verify(features, template, ref result);
                                        //UpdateStatus(result.FARAchieved);

                                        if (result.Verified)
                                        {

                                            if (validador == false)
                                            {
                                                Show("Error: La huella no esta asignada a ningun cliente", "", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
                                                break;
                                            }
                                            else
                                            {

                                                //labelVerification.Text = "Error: La huella pertenece al cliente. " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString();

                                                MakeReport("********");
                                                MakeReport("**verificando coincidencia**");
                                                MakeReport("********");

                                                MakeReport("********");
                                                MakeReport("La huella dactilar pertenece al cliente " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString());
                                                MakeReport("********");

                                                Capturer.StopCapture();

                                                SetPrompt("No se puede terminar la captura - ya esta registrada");

                                                break;

                                            }
                                        }
                                    }
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


            DrawPicture(ConvertSampleToBitmap(Sample));
        }

        protected void Start()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    SetPrompt("Escanea tu huella usando el lector");
                }
                catch
                {
                    SetPrompt("No se puede iniciar la captura");
                }
            }
        }

        protected void Stop()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    SetPrompt("No se puede terminar la captura");
                }
            }
        }

        #region Form Event Handlers:

        private void CaptureForm_Load(object sender, EventArgs e)
        {
            Init();
            Start();                                                // Start capture operation.
        }

        private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }
        #endregion

        #region EventHandler Members:

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            /// en este punto captura la huella 
            ///
            MakeReport("La muestra ha sido capturada");
            SetPrompt("Escanea tu misma huella otra vez");
            Process(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            MakeReport("La huella fue removida del lector");
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            MakeReport("El lector fue tocado");
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("El Lector de huellas ha sido conectado");
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("El Lector de huellas ha sido desconectado");
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
                MakeReport("La calidad de la muestra es BUENA");
            else
                MakeReport("La calidad de la muestra es MALA");
        }
        #endregion

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();  // Create a sample convertor.
            Bitmap bitmap = null;                                                           // TODO: the size doesn't matter
            Convertor.ConvertToPicture(Sample, ref bitmap);                                 // TODO: return bitmap as a result
            return bitmap;
        }

        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();  // Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);            // TODO: return features as a result?
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }

        protected void SetStatus(string status)
        {
            this.Invoke(new Function(delegate () {
                //StatusLine.Text = status;
            }));
        }

        protected void SetPrompt(string prompt)
        {
            this.Invoke(new Function(delegate () {
                Prompt.Text = prompt;
            }));
        }
        protected void MakeReport(string message)
        {
            this.Invoke(new Function(delegate () {
                //StatusText.AppendText(message + "\r\n");
            }));
        }

        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Function(delegate () {
                Picture.Image = new Bitmap(bitmap, Picture.Size);   // fit the image into the picture box
            }));
        }
        private DPFP.Capture.Capture Capturer;
        private void label1_Click(object sender, EventArgs e)
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
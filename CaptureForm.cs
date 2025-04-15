using DPFP.Verification;
using Npgsql;
using NPOI.SS.UserModel;
using PruebaDigitalPersonRegistrar.Conexion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;

namespace PruebaDigitalPersonRegistrar
{
    /* NOTE: This form is a base for the EnrollmentForm and the VerificationForm,
		All changes in the CaptureForm will be reflected in all its derived forms.
	*/
    delegate void Function();

    public partial class CaptureForm : Form, DPFP.Capture.EventHandler
    {
        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;
        private ConexionBD contexto;

        public string connectionString = ("Host=localhost;Username=postgres;Password=31415926;Database=cali_17_03_2025;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;");

        public CaptureForm()
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
                    SetPrompt("No se pudo iniciar la operaci�n de captura");
            }
            catch
            {
                MessageBox.Show("No se pudo iniciar la operaci�n de captura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string query = "SELECT id_cliente, nombres, apellidos, codigo_ver, huella , numero_identificacion FROM public.clientes order by id_cliente desc;";
                    
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
                                                MessageBox.Show("Error: La huella no esta asignada a ningun cliente");
                                                break;
                                            }
                                            else
                                            {

                                                labelVerification.Text = "Error: La huella pertenece al cliente. " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString();

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
                StatusLine.Text = status;
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
                StatusText.AppendText(message + "\r\n");
            }));
        }

        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Function(delegate () {
                Picture.Image = new Bitmap(bitmap, Picture.Size);   // fit the image into the picture box
            }));
        }
        private DPFP.Capture.Capture Capturer;

        private void UpdateStatus(int FAR)
        {
            SetStatus(String.Format("Tasa de Aceptación Falsa (FAR) = {0}", FAR));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
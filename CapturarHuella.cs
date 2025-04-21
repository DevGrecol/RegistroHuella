using DPFP.Verification;
using Npgsql;
using Org.BouncyCastle.Bcpg.Sig;
using PruebaDigitalPersonRegistrar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;

namespace PruebaDigitalPersonRegistrar
{

    public partial class CapturarHuella : CaptureForm
    {
        public delegate void OnTemplateEventHandler(DPFP.Template template);

        public event OnTemplateEventHandler OnTemplate;

        private DPFP.Processing.Enrollment Enroller;

        protected override void Init()
        {
            base.Init();
            base.Text = "Dar de alta Huella";
            Enroller = new DPFP.Processing.Enrollment();           
            UpdateStatus();
        }

        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);

            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            if (features != null) try
                {
                    MakeReport("La huella dactilar fue creada.");
                    Enroller.AddFeatures(features);
                }
                finally
                {
                    UpdateStatus();

                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:
                            OnTemplate(Enroller.Template);
                            SetPrompt("Huella capturada correctamente. El formulario se cerrará.");
                            Stop();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:
                            Enroller.Clear();
                            Stop();
                            UpdateStatus();
                            OnTemplate(null);
                            Start();
                            break;
                    }
                }
        }

        private void UpdateStatus()
        {
            // Show number of samples needed.
            SetStatus(String.Format("Se necesitan muestras de huellas dactilares: {0}", Enroller.FeaturesNeeded));
        }

        public CapturarHuella()
        {
            InitializeComponent();
        }

       
    }
}

using PruebaDigitalPersonRegistrar;
using PruebaDigitalPersonRegistrar.Conexion;
using System;
using System.IO;
using DPFP;
using DPFP.Verification;
using Npgsql;
using System.Windows.Forms;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace PruebaDigitalPersonRegistrar
{
    public partial class frmVerificar : CaptureForm
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
            UpdateStatus(0);
        }

        private void UpdateStatus(int FAR)
        {
            SetStatus(String.Format("Tasa de Aceptación Falsa (FAR) = {0}", FAR));
        }

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
                    
                    //string query = "SELECT nombre, huella, FROM clientes";
                    string query = "SELECT id_cliente, nombres, apellidos, codigo_ver, huella , numero_identificacion FROM public.clientes order by id_cliente desc;";

                    using (NpgsqlConnection conn = new NpgsqlConnection(contexto.connectionString)) 
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

                                        txtEncontradoNombre.Text = " ";
                                        txtEncontradoApellido.Text = " ";
                                        txtEncontradoCedula.Text = " ";
                                        labelNumeroPago.Text = "XXX";
                                        //break;
                                    }
                                    else
                                    {
                                        byte[] huellaBytes = (byte[])reader["huella"];
                                        stream = new MemoryStream(huellaBytes);
                                        template = new DPFP.Template(stream);

                                        Verificator.Verify(features, template, ref result);
                                        UpdateStatus(result.FARAchieved);

                                        if (result.Verified)
                                        {

                                            if (validador == false)
                                            {
                                                MessageBox.Show("La huella no esta asignada a ningun cliente");
                                                break;
                                            }
                                            else
                                            {
                                                Random rnd = new Random();

                                                int cardPago = rnd.Next(1,1000);

                                                string mostrarCardPago = cardPago.ToString();

                                                if (cardPago >= 1 && cardPago <= 9)
                                                {

                                                    labelNumeroPago.Text = "00" + mostrarCardPago;

                                                }
                                                else if (cardPago >= 10 && cardPago <= 99)
                                                {

                                                    labelNumeroPago.Text = "0" + mostrarCardPago;

                                                }else {

                                                    labelNumeroPago.Text = mostrarCardPago;
                                                }


                                                int nume = (int)reader.GetValue(0);

                                                int update = this.actualizarCodigo(cardPago , nume);

                                                txtEncontradoNombre.Text = reader.GetValue(1).ToString();

                                                txtEncontradoApellido.Text = reader.GetValue(2).ToString();

                                                txtEncontradoCedula.Text = reader.GetValue(5).ToString();

                                                MakeReport("La huella dactilar pertenece al cliente. " + reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString());
                                                huellaVerificada = true;
                                                break;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!huellaVerificada)
                    {
                        MakeReport("La huella dactilar NO fue encontrada en la base de datos.");
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


        public int actualizarCodigo(int codigo , int id_cliente) 
        {

    
            using (NpgsqlConnection connection = new NpgsqlConnection(contexto.connectionString))
            {
                try
                {
                    connection.Open();

                    //string sql = "INSERT INTO empleados (nombre, huella) VALUES (@nombre, @huella)";

                    string sql = "UPDATE clientes SET codigo_ver = "+ codigo + " WHERE id_cliente= "+ id_cliente + "";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@codigo_ver", codigo);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException ex)
                {

                    Console.WriteLine("Error de PostgreSQL: " + ex.Message);

                    throw;
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error al guardar cliente: " + ex.Message);

                    throw;
                }
            }

            return 0;

        }

        private void txtEncontradoNombre_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
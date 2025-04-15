using DPFP;
using DPFP.Verification;
using Npgsql;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PruebaDigitalPersonRegistrar.Conexion
{

    
  

    public class ConexionBD : DbContext
    {
        public string connectionString = ("Host=localhost;Username=postgres;Password=31415926;Database=cali_17_03_2025;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;");
        public void GuardarEmpleado(Empleado empleado)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = "INSERT INTO empleados (nombre, huella) VALUES (@nombre, @huella)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", empleado.nombre);
                        command.Parameters.AddWithValue("@huella", empleado.huella);

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
                    
                    Console.WriteLine("Error al guardar empleado: " + ex.Message);
                    
                    throw;
                }
            }
        }

        public List<int> ObtenerIdEmpleadosDesdeBD()
        {
            List<int> ids = new List<int>();

            MessageBox.Show("por aca 00");

            try
            {
                this.Database.Connection.Open();
                string query = "SELECT Id FROM empleados"; 
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, (NpgsqlConnection)this.Database.Connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            ids.Add(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener IDs de empleados: {ex.Message}");
                return null;
            }
            finally
            {
                this.Database.Connection.Close();
            }

            return ids;
        }

        public byte[] ObtenerHuellaEmpleadoDesdeBD(int idEmpleado)
        {

            MessageBox.Show("por aca 1");


            try
            {
                this.Database.Connection.Open();
                string query = "SELECT huella FROM empleados WHERE Id = @Id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, (NpgsqlConnection)this.Database.Connection))
                {
                    cmd.Parameters.AddWithValue("@id", idEmpleado);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (byte[])reader.GetValue(0);
                        }
                        else
                        {
                            return null; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener huella del empleado {idEmpleado}: {ex.Message}");
                return null;
            }
            finally
            {
                this.Database.Connection.Close();
            }
        }

        public string ObtenerNombreEmpleadoDesdeBD(int idEmpleado)
        {
            MessageBox.Show("por aca 2");

            try
            {
                this.Database.Connection.Open();
                string query = "SELECT nombre FROM empleados WHERE Id = @Id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, (NpgsqlConnection)this.Database.Connection))
                {
                    cmd.Parameters.AddWithValue("@Id", idEmpleado);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                        else
                        {
                            return null; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener nombre del empleado {idEmpleado}: {ex.Message}");
                return null;
            }
            finally
            {
                this.Database.Connection.Close();
            }
        }


        public Cliente ConsultarCliente(Cliente cliente)
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {

                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = "select id_cliente,numero_identificacion,nombres, apellidos,codigo_ver,huella FROM clientes where numero_identificacion = @numero_identificacion";
                    cmd.Parameters.AddWithValue("@id_cliente", cliente.id_cliente);
                    cmd.Parameters.AddWithValue("@numero_identificacion", cliente.numero_identificacion);
                    cmd.Parameters.AddWithValue("@codigo_ver", cliente.codigo_ver);
                    //cmd.Parameters.AddWithValue("@apellidos", cliente.apellidos);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            cliente.id_cliente = reader.GetInt32(reader.GetOrdinal("id_cliente"));
                            cliente.numero_identificacion = reader.GetString(reader.GetOrdinal("numero_identificacion"));
                            cliente.nombres = reader.GetString(reader.GetOrdinal("nombres"));
                            cliente.apellidos = reader.GetString(reader.GetOrdinal("apellidos"));
                            cliente.codigo_ver = reader.GetInt32(reader.GetOrdinal("codigo_ver"));

                            Boolean validador  =  reader.IsDBNull(5) ? false : true;

                            if (validador == true)
                            {

                                byte[] bytes = (byte[])reader.GetValue(5);
                                cliente.huella = bytes;

                            }
                            else {

                                cliente.huella = null;
                            }

                            return cliente;
                        }
                        else {


                            cliente.id_cliente = 0;
                            cliente.numero_identificacion = "0";
                            cliente.nombres = "0";
                            cliente.apellidos = "0";
                            cliente.codigo_ver = 0;

                            return cliente;

                        }
                    }
                }

            }

        }



        public void GuardarHuellaCliente(Cliente cliente)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //string sql = "INSERT INTO empleados (nombre, huella) VALUES (@nombre, @huella)";

                    string sql = "UPDATE clientes SET huella = @huella WHERE id_cliente= @id_cliente";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id_cliente", cliente.id_cliente);
                        command.Parameters.AddWithValue("@huella", cliente.huella);

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
        }


        public int confirmarHuella(Cliente cliente)
        {

            


           return 1;
        
        }






    }
}

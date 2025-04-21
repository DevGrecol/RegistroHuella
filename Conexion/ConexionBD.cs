using DPFP;
using DPFP.Verification;
using Npgsql;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Drawing2D;
using System.Drawing;
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
                    
                    Show("Error de PostgreSQL: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
                    
                    throw;
                }
                catch (Exception ex)
                {
                    
                    Show("Error al guardar empleado: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
                    
                    throw;
                }
            }
        }

        public List<int> ObtenerIdEmpleadosDesdeBD()
        {
            List<int> ids = new List<int>();

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
                Show($"Error al obtener IDs de empleados: {ex.Message}","", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
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
                Show($"Error al obtener huella del empleado {idEmpleado}: {ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);
                return null;
            }
            finally
            {
                this.Database.Connection.Close();
            }
        }

        public string ObtenerNombreEmpleadoDesdeBD(int idEmpleado)
        {
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
                Show($"Error al obtener nombre del empleado {idEmpleado}: {ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White     );
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

                    Show("Error de PostgreSQL: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

                    throw;
                }
                catch (Exception ex)
                {

                    Show("Error al guardar cliente: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information, Color.LightCoral, Color.IndianRed, Color.White);

                    throw;
                }
            }
        }


        public int confirmarHuella(Cliente cliente)
        {

            


           return 1;
        
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

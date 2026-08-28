using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace EjemploSQLCommandInsertar
{
    /// <summary>
    /// Lógica de interacción para AgregarProducto.xaml
    /// </summary>
    public partial class AgregarProducto : Window
    {
        public AgregarProducto()
        {
            InitializeComponent();
            CargarDataGrid();
        }

        private void CargarDataGrid()
        {
            string cadena = ConfigurationManager.ConnectionStrings["EjemploSQLCommandInsertar.Properties.Settings.Northwind"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(cadena))
                {
                    string query = @"SELECT p.ProductName AS Producto, p.UnitPrice AS Precio, c.CategoryName AS Categoria 
                                     FROM Products p 
                                     LEFT JOIN Categories c ON p.CategoryID = c.CategoryID 
                                     ORDER BY p.ProductID DESC";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgProductos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la tabla: " + ex.Message);
            }
        }

        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            btnRegistrar.IsEnabled = false;
            string cadena = ConfigurationManager.ConnectionStrings["EjemploSQLCommandInsertar.Properties.Settings.Northwind"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(cadena))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SP_AgregarProducto";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 40).Value = txtNombre.Text;
                        cmd.Parameters.Add("@Precio", System.Data.SqlDbType.Money).Value = txtPrecio.Text;
                        cmd.Parameters.Add("@Categoria", System.Data.SqlDbType.NVarChar, 15).Value = txtCategoria.Text;
                        cmd.CommandTimeout = 60;
                        await cmd.ExecuteNonQueryAsync();

                        MessageBox.Show("Producto agregado exitosamente.");
                        Limpiar();
                        CargarDataGrid();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50001)
                {
                    MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Error SQL {ex.Number}, {ex.Message}");
                }
            }
            btnRegistrar.IsEnabled = true;
        }

        private void Limpiar()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtCategoria.Clear();
            txtNombre.Focus();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }
    }
}
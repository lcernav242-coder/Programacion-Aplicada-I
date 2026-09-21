using Microsoft.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Threading.Tasks;

namespace EliminacionRegistros
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string cn = ConfigurationManager.ConnectionStrings["EliminacionRegistros.Properties.Settings.Nortwind"].ConnectionString;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void CargarListaCategorias()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cn))
                {
                    string query = "SELECT CategoryID,CategoryName,Description FROM Categories ORDER BY CategoryID";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Categoria> lista = new List<Categoria>();

                    while (reader.Read())
                    {
                        lista.Add(new Categoria
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),

                            //aca siempre ver el error 
                            Descripcion = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                        });
                    }
                    dgCategorias.ItemsSource = lista;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error en sql {ex.Number}, {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general {ex.Message}");
            }
        }

        private async void dgCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEliminar.IsEnabled = false;

            if (dgCategorias.SelectedItem != null)
            {
                Categoria categoria =
                    (Categoria)dgCategorias.SelectedItem;

                txtIdCategoria.Text = categoria.Id.ToString();

                txtNombre.Text = categoria.Nombre;

                txtDescripcion.Text = categoria.Descripcion ?? string.Empty;

                btnEliminar.IsEnabled = await CanDeleteCategoryAsync(categoria.Id);
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            this.Nuevo();

        }
        private void Nuevo()

        {
            txtIdCategoria.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            btnEliminar.IsEnabled = false;
            txtNombre.Focus();
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string id = txtIdCategoria.Text;

                using (SqlConnection conn = new SqlConnection(cn))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();

                    if (string.IsNullOrEmpty(id))
                    {
                        cmd.CommandText = "INSERT INTO Categories(CategoryName,Description) VALUES(@Nombre,@Descripcion); select SCOPE_IDENTITY();";
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 15).Value = txtNombre.Text;
                        cmd.Parameters.Add("@Descripcion", System.Data.SqlDbType.NVarChar, -1).Value = txtDescripcion.Text;
                        int idGenerado = Convert.ToInt32(cmd.ExecuteScalar());

                        MessageBox.Show($"Categoria agregada con Id {idGenerado}");
                        this.Nuevo();
                        this.CargarListaCategorias();
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE categories SET CategoryName=@Nombre,
                     Description=@Descripcion
                     WHERE CategoryID=@Id";
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 15).Value = txtNombre.Text;
                        cmd.Parameters.Add("@Descripcion", System.Data.SqlDbType.NVarChar, -1).Value = string.IsNullOrEmpty(txtDescripcion.Text) ? (object)DBNull.Value : txtDescripcion.Text;
                        cmd.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;

                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"Categoria actualizada");
                        this.CargarListaCategorias();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error en sql {ex.Number}, {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general {ex.Message}");
            }




        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(" ¿esta seguro de eliminar la categoria? ", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta == MessageBoxResult.Yes)
            {
                this.EiminarRegistro();
            }

        }

        private void EiminarRegistro()
        {
            try
            {
                string query = "DELETE FROM Categories WHERE CategoryId = @IdCategoria";
                using (SqlConnection con = new SqlConnection(cn))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@IdCategoria", System.Data.SqlDbType.Int).Value = txtIdCategoria.Text;

                        int filaAfectadas = cmd.ExecuteNonQuery();

                        if (filaAfectadas > 0)
                        {
                            MessageBox.Show("Registro eliminado");
                            this.CargarListaCategorias();
                        }
                        else
                        {
                            MessageBox.Show("El Registro no existe");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    MessageBox.Show("No es posible eliminar el registro seleccionado, contiene productos");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CargarListaCategorias();
        }

        private async Task<bool> CanDeleteCategoryAsync(int categoryId)
        {
            string sql = @"
        SELECT COUNT(1)
        FROM Products
        WHERE CategoryID = @ID";

            using (SqlConnection connection =
                   new SqlConnection(cn))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd =
                       new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add(
                        "@ID",
                        System.Data.SqlDbType.Int
                    ).Value = categoryId;

                    int count = Convert.ToInt32(
                        await cmd.ExecuteScalarAsync()
                    );

                    return count == 0;
                }
            }
        }

    }
}
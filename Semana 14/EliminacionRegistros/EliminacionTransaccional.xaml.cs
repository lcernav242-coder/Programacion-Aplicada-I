using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace EliminacionRegistros
{
    public partial class EliminacionTransaccional : Window
    {
        string cn = ConfigurationManager.ConnectionStrings["EliminacionRegistros.Properties.Settings.Nortwind"].ConnectionString;

        public EliminacionTransaccional()
        {
            InitializeComponent();
            CargarListaCategorias();
        }

        private void Window_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            CargarListaCategorias();
        }

        private void CargarListaCategorias()
        {
            try
            {
                using (SqlConnection conn =
                       new SqlConnection(cn))
                {
                    string query = @"
                        SELECT
                            CategoryID,
                            CategoryName,
                            Description
                        FROM Categories
                        ORDER BY CategoryID";

                    conn.Open();

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader =
                               cmd.ExecuteReader())
                        {
                            List<Categoria> lista =
                                new List<Categoria>();

                            while (reader.Read())
                            {
                                lista.Add(new Categoria
                                {
                                    Id = reader.GetInt32(0),

                                    Nombre =
                                        reader.GetString(1),

                                    Descripcion =
                                        reader.IsDBNull(2)
                                        ? string.Empty
                                        : reader.GetString(2)
                                });
                            }

                            dgCategorias.ItemsSource = lista;
                        }
                    }
                }

                btnEliminar.IsEnabled = false;
            }
            catch (SqlException ex)
            {
                txtEstado.Text =
                    $"Error SQL {ex.Number}: {ex.Message}";
            }
            catch (Exception ex)
            {
                txtEstado.Text =
                    $"Error general: {ex.Message}";
            }
        }

        private void dgCategorias_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgCategorias.SelectedItem != null)
            {
                Categoria categoria =
                    (Categoria)dgCategorias.SelectedItem;

                txtIdCategoria.Text =
                    categoria.Id.ToString();

                txtNombre.Text =
                    categoria.Nombre;

                txtDescripcion.Text =
                    categoria.Descripcion ?? string.Empty;


                btnEliminar.IsEnabled = true;

                txtEstado.Text =
                    $"Categoría seleccionada: {categoria.Nombre}.";
            }
            else
            {
                btnEliminar.IsEnabled = false;
            }
        }

        private void btnNuevo_Click(
            object sender,
            RoutedEventArgs e)
        {
            Nuevo();
        }

        private void Nuevo()
        {
            txtIdCategoria.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();

            dgCategorias.SelectedItem = null;
            btnEliminar.IsEnabled = false;

            txtEstado.Text =
                "Seleccione una categoría.";

            txtNombre.Focus();
        }

        private void btnAgregar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                txtEstado.Text =
                    "Ingrese el nombre de la categoría.";

                return;
            }

            try
            {
                string id = txtIdCategoria.Text;

                using (SqlConnection conn =
                       new SqlConnection(cn))
                {
                    conn.Open();

                    using (SqlCommand cmd =
                           conn.CreateCommand())
                    {
                        if (string.IsNullOrEmpty(id))
                        {
                            cmd.CommandText = @"
                                INSERT INTO Categories(
                                    CategoryName,
                                    Description
                                )
                                VALUES(
                                    @Nombre,
                                    @Descripcion
                                );

                                SELECT SCOPE_IDENTITY();";

                            cmd.Parameters.Add(
                                "@Nombre",
                                SqlDbType.NVarChar,
                                15
                            ).Value = txtNombre.Text;

                            cmd.Parameters.Add(
                                "@Descripcion",
                                SqlDbType.NVarChar,
                                -1
                            ).Value =
                                string.IsNullOrWhiteSpace(
                                    txtDescripcion.Text)
                                ? DBNull.Value
                                : txtDescripcion.Text;

                            int idGenerado =
                                Convert.ToInt32(
                                    cmd.ExecuteScalar()
                                );

                            txtEstado.Text =
                                $"Categoría agregada con ID {idGenerado}.";
                        }
                        else
                        {
                            cmd.CommandText = @"
                                UPDATE Categories
                                SET
                                    CategoryName = @Nombre,
                                    Description = @Descripcion
                                WHERE CategoryID = @Id";

                            cmd.Parameters.Add(
                                "@Nombre",
                                SqlDbType.NVarChar,
                                15
                            ).Value = txtNombre.Text;

                            cmd.Parameters.Add(
                                "@Descripcion",
                                SqlDbType.NVarChar,
                                -1
                            ).Value =
                                string.IsNullOrWhiteSpace(
                                    txtDescripcion.Text)
                                ? DBNull.Value
                                : txtDescripcion.Text;

                            cmd.Parameters.Add(
                                "@Id",
                                SqlDbType.Int
                            ).Value = Convert.ToInt32(id);

                            int filas =
                                cmd.ExecuteNonQuery();

                            txtEstado.Text =
                                filas > 0
                                ? "Categoría actualizada correctamente."
                                : "No se encontró la categoría.";
                        }
                    }
                }

                Nuevo();
                CargarListaCategorias();
            }
            catch (SqlException ex)
            {
                txtEstado.Text =
                    $"Error SQL {ex.Number}: {ex.Message}";
            }
            catch (Exception ex)
            {
                txtEstado.Text =
                    $"Error general: {ex.Message}";
            }
        }

        private void btnEliminar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                    txtIdCategoria.Text))
            {
                txtEstado.Text =
                    "Primero seleccione una categoría.";

                return;
            }

            MessageBoxResult respuesta =
                MessageBox.Show(
                    "¿Está seguro de eliminar la categoría " +
                    "y todos sus productos?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

            if (respuesta == MessageBoxResult.Yes)
            {
                EiminarRegistro();
            }
            else
            {
                txtEstado.Text =
                    "Eliminación cancelada.";
            }
        }

        private void EiminarRegistro()
        {
            if (!int.TryParse(
                    txtIdCategoria.Text,
                    out int idCategoria))
            {
                txtEstado.Text =
                    "El ID de la categoría no es válido.";

                return;
            }

            using (SqlConnection conn =
                   new SqlConnection(cn))
            {
                conn.Open();

                SqlTransaction transaccion =
                    conn.BeginTransaction();

                try
                {

                    string eliminarProductos = @"
                        DELETE FROM Products
                        WHERE CategoryID = @IdCategoria";

                    int productosEliminados;

                    using (SqlCommand cmdProductos =
                           new SqlCommand(
                               eliminarProductos,
                               conn,
                               transaccion))
                    {
                        cmdProductos.Parameters.Add(
                            "@IdCategoria",
                            SqlDbType.Int
                        ).Value = idCategoria;

                        productosEliminados =
                            cmdProductos.ExecuteNonQuery();
                    }


                    string eliminarCategoria = @"
                        DELETE FROM Categories
                        WHERE CategoryID = @IdCategoria";

                    int categoriaEliminada;

                    using (SqlCommand cmdCategoria =
                           new SqlCommand(
                               eliminarCategoria,
                               conn,
                               transaccion))
                    {
                        cmdCategoria.Parameters.Add(
                            "@IdCategoria",
                            SqlDbType.Int
                        ).Value = idCategoria;

                        categoriaEliminada =
                            cmdCategoria.ExecuteNonQuery();
                    }

                    if (categoriaEliminada == 0)
                    {
                        throw new Exception(
                            "La categoría seleccionada no existe."
                        );
                    }


                    transaccion.Commit();

                    txtEstado.Text =
                        $"Categoría eliminada correctamente. " +
                        $"Productos eliminados: " +
                        $"{productosEliminados}.";

                    Nuevo();
                    CargarListaCategorias();
                }
                catch (Exception ex)
                {

                    try
                    {
                        transaccion.Rollback();
                    }
                    catch
                    {
                    }

                    txtEstado.Text =
                        $"No se realizó la eliminación. " +
                        $"Se ejecutó Rollback(). " +
                        $"Detalle: {ex.Message}";
                }
            }
        }
    }
}
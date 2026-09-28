using GestionOrdenesMVVM.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace GestionOrdenesMVVM.Repositories
{
    public class OrdenRepositoryImpl : IOrdenRepository
    {
        private string cn = Properties.Settings.Default.Northwind;

        public List<Orden> ListarTodos()
        {
            List<Orden> lista = new List<Orden>();
            string query = @"
                SELECT 
                    o.OrderID, 
                    c.CompanyName AS Cliente, 
                    e.FirstName + ' ' + e.LastName AS Empleado, 
                    s.CompanyName AS Transportista, 
                    o.OrderDate, 
                    o.ShipCity, 
                    o.Freight AS Monto
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID
                INNER JOIN Shippers s ON o.ShipVia = s.ShipperID";

            using (SqlConnection conn = new SqlConnection(cn))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Orden
                            {
                                OrderID = reader.GetInt32(0),
                                Cliente = reader.GetString(1),
                                Empleado = reader.GetString(2),
                                Transportista = reader.GetString(3),
                                Fecha = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                CiudadDestino = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                Monto = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void Insertar(Orden orden)
        {
            string query = @"
                INSERT INTO Orders (CustomerID, ShipName, ShipCity, Freight, ShipVia) 
                VALUES (@Cliente, @Destino, @Ciudad, @Monto, @Transportista)";

            using (SqlConnection conn = new SqlConnection(cn))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Cliente", orden.Cliente);
                    cmd.Parameters.AddWithValue("@Destino", orden.Empleado);
                    cmd.Parameters.AddWithValue("@Ciudad", orden.CiudadDestino);
                    cmd.Parameters.AddWithValue("@Monto", orden.Monto);

                    int shipVia = 1;
                    if (orden.Transportista == "United Package") shipVia = 2;
                    else if (orden.Transportista == "Federal Shipping") shipVia = 3;
                    cmd.Parameters.AddWithValue("@Transportista", shipVia);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Orden orden)
        {
            string query = @"
                UPDATE Orders 
                SET CustomerID = @Cliente, ShipName = @Destino, ShipCity = @Ciudad, Freight = @Monto, ShipVia = @Transportista
                WHERE OrderID = @Id";

            using (SqlConnection conn = new SqlConnection(cn))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", orden.OrderID);
                    cmd.Parameters.AddWithValue("@Cliente", orden.Cliente);
                    cmd.Parameters.AddWithValue("@Destino", orden.Empleado);
                    cmd.Parameters.AddWithValue("@Ciudad", orden.CiudadDestino);
                    cmd.Parameters.AddWithValue("@Monto", orden.Monto);

                    int shipVia = 1;
                    if (orden.Transportista == "United Package") shipVia = 2;
                    else if (orden.Transportista == "Federal Shipping") shipVia = 3;
                    cmd.Parameters.AddWithValue("@Transportista", shipVia);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int idOrden)
        {
            using (SqlConnection conn = new SqlConnection(cn))
            {
                conn.Open();
                SqlTransaction transaccion = conn.BeginTransaction();

                try
                {
                    // 1. Primero eliminamos los detalles para no romper la llave foránea
                    string queryDetalles = "DELETE FROM [Order Details] WHERE OrderID = @Id";
                    using (SqlCommand cmdDetalles = new SqlCommand(queryDetalles, conn, transaccion))
                    {
                        cmdDetalles.Parameters.AddWithValue("@Id", idOrden);
                        cmdDetalles.ExecuteNonQuery();
                    }

                    // 2. Luego eliminamos la orden principal
                    string queryOrden = "DELETE FROM Orders WHERE OrderID = @Id";
                    using (SqlCommand cmdOrden = new SqlCommand(queryOrden, conn, transaccion))
                    {
                        cmdOrden.Parameters.AddWithValue("@Id", idOrden);
                        cmdOrden.ExecuteNonQuery();
                    }

                    transaccion.Commit(); // Confirmamos si no hubo errores
                }
                catch (Exception)
                {
                    transaccion.Rollback(); // Revertimos si hay error
                    throw;
                }
            }
        }
        public List<string> ListarEmpleados()
        {
            List<string> empleados = new List<string>();
            using (SqlConnection conn = new SqlConnection(cn))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT FirstName + ' ' + LastName FROM Employees", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) empleados.Add(reader.GetString(0));
                }
            }
            return empleados;
        }

        public List<string> ListarCiudades()
        {
            List<string> ciudades = new List<string>();
            using (SqlConnection conn = new SqlConnection(cn))
            {
                conn.Open();
                // DISTINCT evita que salgan 50 veces "Madrid" o "London"
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT ShipCity FROM Orders WHERE ShipCity IS NOT NULL ORDER BY ShipCity", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) ciudades.Add(reader.GetString(0));
                }
            }
            return ciudades;
        }
    }
}
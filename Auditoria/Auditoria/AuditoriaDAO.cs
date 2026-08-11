using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditoria
{
    public class AuditoriaDAO
    {
        private string connectionString = "Server=.;Database=Northwind;Integrated Security=True;TrustServerCertificate=True;";

        public List<Transportista> ObtenerTransportistas()
        {
            List<Transportista> lista = new List<Transportista>();
            string sql = "SELECT ShipperID, CompanyName FROM Shippers";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, cn);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Transportista
                        {
                            ShipperID = dr.GetInt32(0),
                            CompanyName = dr.GetString(1)
                        });
                    }
                }
            }
            return lista;
        }

        public List<PedidoEnvio> BuscarPedidos(int shipperId, decimal fleteMin, decimal fleteMax)
        {
            List<PedidoEnvio> lista = new List<PedidoEnvio>();

            string sql = @"
                SELECT 
                    o.OrderID, 
                    c.CompanyName, 
                    o.OrderDate, 
                    o.Freight
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                WHERE o.ShipVia = @ShipperID 
                  AND o.Freight >= @FleteMin 
                  AND o.Freight <= @FleteMax";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@ShipperID", shipperId);
                cmd.Parameters.AddWithValue("@FleteMin", fleteMin);
                cmd.Parameters.AddWithValue("@FleteMax", fleteMax);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PedidoEnvio pedido = new PedidoEnvio();
                        pedido.NumeroOrden = dr.GetInt32(0);
                        pedido.NombreCliente = dr.GetString(1);

                        if (!dr.IsDBNull(2))
                            pedido.FechaPedido = dr.GetDateTime(2);

                        if (!dr.IsDBNull(3))
                            pedido.MontoFlete = dr.GetDecimal(3);

                        lista.Add(pedido);
                    }
                }
            }
            return lista;
        }
    }
}

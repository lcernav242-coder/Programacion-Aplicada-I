using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using MantenimientoLibros.Models;

namespace MantenimientoLibros.Repositories
{
    public class LibroRepositoryImpl : ILibroRepository
    {
        private readonly string cadenaConexion;

        public LibroRepositoryImpl()
        {
            cadenaConexion = ConfigurationManager.ConnectionStrings["EditorialDBConnection"].ConnectionString;
        }

        public IEnumerable<Libro> ObtenerTodos()
        {
            var libros = new List<Libro>();
            string query = @"SELECT t.title_id, t.title, t.type, t.pub_id, p.pub_name, p.city, t.price, t.advance, t.royalty, t.ytd_sales, t.notes, t.pubdate 
                     FROM titles t 
                     LEFT JOIN publishers p ON t.pub_id = p.pub_id";

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        libros.Add(new Libro
                        {
                            TitleId = reader["title_id"].ToString() ?? string.Empty,
                            Title = reader["title"].ToString() ?? string.Empty,
                            Type = reader["type"] != DBNull.Value ? reader["type"].ToString() : null,
                            PubId = reader["pub_id"] != DBNull.Value ? reader["pub_id"].ToString() : null,
                            PubName = reader["pub_name"] != DBNull.Value ? reader["pub_name"].ToString() : null,
                            City = reader["city"] != DBNull.Value ? reader["city"].ToString() : null,
                            Price = reader["price"] != DBNull.Value ? Convert.ToDecimal(reader["price"]) : (decimal?)null,
                            Advance = reader["advance"] != DBNull.Value ? Convert.ToDecimal(reader["advance"]) : (decimal?)null,
                            Royalty = reader["royalty"] != DBNull.Value ? Convert.ToInt32(reader["royalty"]) : (int?)null,
                            YtdSales = reader["ytd_sales"] != DBNull.Value ? Convert.ToInt32(reader["ytd_sales"]) : (int?)null,
                            Notes = reader["notes"] != DBNull.Value ? reader["notes"].ToString() : null,
                            PubDate = reader["pubdate"] != DBNull.Value ? Convert.ToDateTime(reader["pubdate"]) : (DateTime?)null
                        });
                    }
                }
            }
            return libros;
        }

        public IEnumerable<Editorial> ObtenerEditoriales()
        {
            var editoriales = new List<Editorial>();
            string query = "SELECT pub_id, pub_name, city FROM publishers";

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        editoriales.Add(new Editorial
                        {
                            PubId = reader["pub_id"].ToString() ?? string.Empty,
                            PubName = reader["pub_name"].ToString() ?? string.Empty,
                            City = reader["city"] != DBNull.Value ? reader["city"].ToString() ?? string.Empty : string.Empty
                        });
                    }
                }
            }
            return editoriales;
        }

       

        public IEnumerable<Autor> ObtenerAutores()
        {
            var autores = new List<Autor>();
            string query = "SELECT au_id, au_fname, au_lname FROM authors";

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        autores.Add(new Autor
                        {
                            AuId = reader["au_id"].ToString(),
                            AuFname = reader["au_fname"].ToString(),
                            AuLname = reader["au_lname"].ToString(),
                            IsSelected = false
                        });
                    }
                }
            }
            return autores;
        }

        public IEnumerable<string> ObtenerAutoresPorLibro(string titleId)
        {
            var autoresIds = new List<string>();
            string query = "SELECT au_id FROM titleauthor WHERE title_id = @title_id";

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@title_id", titleId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        autoresIds.Add(reader["au_id"].ToString());
                    }
                }
            }
            return autoresIds;
        }

        public void Guardar(Libro libro, List<string> autoresIds)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        string queryLibro = @"
                            IF EXISTS (SELECT 1 FROM titles WHERE title_id = @title_id)
                                UPDATE titles SET title = @title, type = @type, pub_id = @pub_id, price = @price, advance = @advance, royalty = @royalty, ytd_sales = @ytd_sales, notes = @notes, pubdate = @pubdate WHERE title_id = @title_id
                            ELSE
                                INSERT INTO titles (title_id, title, type, pub_id, price, advance, royalty, ytd_sales, notes, pubdate) 
                                VALUES (@title_id, @title, @type, @pub_id, @price, @advance, @royalty, @ytd_sales, @notes, @pubdate)";

                        using (SqlCommand cmdLibro = new SqlCommand(queryLibro, conn, tx))
                        {
                            cmdLibro.Parameters.AddWithValue("@title_id", libro.TitleId);
                            cmdLibro.Parameters.AddWithValue("@title", libro.Title);
                            cmdLibro.Parameters.AddWithValue("@type", libro.Type ?? (object)DBNull.Value);
                            cmdLibro.Parameters.AddWithValue("@pub_id", libro.PubId ?? (object)DBNull.Value);
                            cmdLibro.Parameters.AddWithValue("@price", libro.Price ?? (object)DBNull.Value);
                            cmdLibro.Parameters.AddWithValue("@advance", libro.Advance ?? (object)DBNull.Value);
                            cmdLibro.Parameters.AddWithValue("@royalty", libro.Royalty ?? (object)DBNull.Value);
                            cmdLibro.Parameters.AddWithValue("@ytd_sales", libro.YtdSales ?? (object)DBNull.Value);
                            cmdLibro.Parameters.AddWithValue("@notes", libro.Notes ?? (object)DBNull.Value);
                            cmdLibro.Parameters.AddWithValue("@pubdate", libro.PubDate ?? (object)DBNull.Value);
                            cmdLibro.ExecuteNonQuery();
                        }

                        string deleteAutores = "DELETE FROM titleauthor WHERE title_id = @title_id";
                        using (SqlCommand cmdDelete = new SqlCommand(deleteAutores, conn, tx))
                        {
                            cmdDelete.Parameters.AddWithValue("@title_id", libro.TitleId);
                            cmdDelete.ExecuteNonQuery();
                        }

                        if (autoresIds != null)
                        {
                            string insertAutor = "INSERT INTO titleauthor (au_id, title_id, au_ord, royaltyper) VALUES (@au_id, @title_id, 1, 100)";
                            foreach (var auId in autoresIds)
                            {
                                using (SqlCommand cmdInsertAutor = new SqlCommand(insertAutor, conn, tx))
                                {
                                    cmdInsertAutor.Parameters.AddWithValue("@au_id", auId);
                                    cmdInsertAutor.Parameters.AddWithValue("@title_id", libro.TitleId);
                                    cmdInsertAutor.ExecuteNonQuery();
                                }
                            }
                        }
                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Eliminar(string titleId)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        string queryDetalle = "DELETE FROM titleauthor WHERE title_id = @Id";
                        using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conn, tx))
                        {
                            cmdDetalle.Parameters.AddWithValue("@Id", titleId);
                            cmdDetalle.ExecuteNonQuery();
                        }

                        string queryPadre = "DELETE FROM titles WHERE title_id = @Id";
                        using (SqlCommand cmdPadre = new SqlCommand(queryPadre, conn, tx))
                        {
                            cmdPadre.Parameters.AddWithValue("@Id", titleId);
                            cmdPadre.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
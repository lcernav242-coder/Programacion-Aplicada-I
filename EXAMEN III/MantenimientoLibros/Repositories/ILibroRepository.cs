using System.Collections.Generic;
using MantenimientoLibros.Models;

namespace MantenimientoLibros.Repositories
{
    public interface ILibroRepository
    {
        IEnumerable<Libro> ObtenerTodos();
        IEnumerable<Editorial> ObtenerEditoriales();
        IEnumerable<Autor> ObtenerAutores();
        IEnumerable<string> ObtenerAutoresPorLibro(string titleId);
        void Guardar(Libro libro, List<string> autoresIds);
        void Eliminar(string titleId);
    }
}
using GestionOrdenesMVVM.Models;
using System.Collections.Generic;

namespace GestionOrdenesMVVM.Repositories
{
    public interface IOrdenRepository
    {
        List<Orden> ListarTodos();
        void Insertar(Orden orden);
        void Actualizar(Orden orden);
        void Eliminar(int idOrden);
        List<string> ListarEmpleados();
        List<string> ListarCiudades();

    }
}
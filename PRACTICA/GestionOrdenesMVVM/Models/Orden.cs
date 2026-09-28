using System;

namespace GestionOrdenesMVVM.Models
{
    public class Orden
    {
        public int OrderID { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Empleado { get; set; } = string.Empty;
        public string Transportista { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string CiudadDestino { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}
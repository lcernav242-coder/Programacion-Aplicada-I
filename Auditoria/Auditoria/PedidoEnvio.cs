using System;
using System.Collections.Generic;
using System.Text;

namespace Auditoria
{
    public class PedidoEnvio
    {
        public int NumeroOrden { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal MontoFlete { get; set; }
    }
}
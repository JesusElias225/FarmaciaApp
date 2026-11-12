using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaApp
{
    public class ItemPedido
    {
        public string Nombre { get; set; } = "";
        public double Precio { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal { get; set; }
    }
}
    
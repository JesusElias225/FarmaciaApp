using System;
using System.Collections.Generic;

namespace FarmaciaApp
{
    public class Ticket
    {
        public string Cliente { get; set; } = "";
        public string Fecha { get; set; } = "";
        public List<ItemPedido> Productos { get; set; } = new List<ItemPedido>();
        public double Total { get; set; }
    }
}

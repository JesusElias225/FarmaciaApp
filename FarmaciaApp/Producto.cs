using System;
using System.Collections.Generic;

namespace FarmaciaApp
{
    public class Producto
    {
        public string Nombre { get; set; } = "";
        public double Precio { get; set; }

        public override string ToString()
        {
            return Nombre + " - $" + Precio.ToString("0.00");
        }
    }
}

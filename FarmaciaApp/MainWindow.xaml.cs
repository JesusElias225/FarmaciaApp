using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace FarmaciaApp
{
    public sealed partial class MainWindow : Window
    {
        //Lista de los medicamentos 
        List<Producto> productos = new List<Producto>();

        //Lista del cliente que va agregando en el pedido
        List<ItemPedido> pedido = new List<ItemPedido>();

        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.SetIcon("Assets/farmacia.ico");
            CargarProductos();
        }

        //Productos de medicamentos
        private void CargarProductos()
        {
            productos.Add(new Producto { Nombre = "Paracetamol 500 mg", Precio = 120 });
            productos.Add(new Producto { Nombre = "Ibuprofeno 400 mg", Precio = 140 });
            productos.Add(new Producto { Nombre = "Jarabe para la tos", Precio = 110 });
            productos.Add(new Producto { Nombre = "Vitaminas c", Precio = 150 });
            productos.Add(new Producto { Nombre = "Omeprazol 20 mg 120 capsulas", Precio = 230 });
            productos.Add(new Producto { Nombre = "Naproxeno/Paracetamol", Precio = 300 });
            productos.Add(new Producto { Nombre = "Alcohol 350 ml", Precio = 50 });

            cmbProductos.ItemsSource = productos;
        }

        //Boton de agregar a la compra
        private async void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            // Validacion 1: Que haya elegido un medicamento
            if (cmbProductos.SelectedItem == null)
            {
                await MostrarMensaje("Selecciona un medicamento en la lista.");
                return;
            }

            // Validacion 2: Que la cantidad no este vacia
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                await MostrarMensaje("Escribe la cantidad.");
                return;
            }

            // Validacion 3: Que la cantidad sea numero entero
            int cantidad;
            if (!int.TryParse(txtCantidad.Text, out cantidad))
            {
                await MostrarMensaje("La cantidad debe ser un numero entero");
                return;
            }

            // Validacion 4: Que este en un rango logico
            if (cantidad <= 0 || cantidad > 10)
            {
                await MostrarMensaje("La cantidad debe ser entre 1 y 10.");
                return;
            }

            Producto productoElegido = (Producto)cmbProductos.SelectedItem;

            ItemPedido item = new ItemPedido();
            item.Nombre = productoElegido.Nombre;
            item.Precio = productoElegido.Precio;
            item.Cantidad = cantidad;
            item.Subtotal = productoElegido.Precio * cantidad;

            pedido.Add(item);
            ActualizarLista();

            txtCantidad.Text = "";
            cmbProductos.SelectedIndex = -1;
        }

        // Boton de quitar seleccionados
        private async void btnQuitar_Click(object sender, RoutedEventArgs e)
        {
            if (lstPedido.SelectedIndex == -1)
            {
                await MostrarMensaje("Selecciona el medicamento que desea quitarlo en la compra");
                return;
            }

            pedido.RemoveAt(lstPedido.SelectedIndex);
            ActualizarLista();
        }

        // Boton de generar el ticket
        private async void btnTicket_Click(object sender, RoutedEventArgs e)
        {
            // Validacion: Nombre vacio
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                await MostrarMensaje("Escribe el nombre del cliente.");
                return;
            }

            // Validacion: solo letras y espacios
            foreach (char letra in txtCliente.Text)
            {
                if (!char.IsLetter(letra) && letra != ' ')
                {
                    await MostrarMensaje("El nombre solo puede tener letras y espacios.");
                    return;
                }
            }

            // Validacion: que haya medicamentos
            if (pedido.Count == 0)
            {
                await MostrarMensaje("Agrega al menos un medicamento antes de generar el ticket.");
                return;
            }

            double total = 0;
            foreach (ItemPedido item in pedido)
            {
                total = total + item.Subtotal;
            }

            string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            string ticket = "";
            ticket += "-------- FARMACIA UNIVERSITARIA --------\n";
            ticket += "Fecha: " + fecha + "\n";
            ticket += "Cliente: " + txtCliente.Text + "\n";
            ticket += "--------------------------------\n";
            foreach (ItemPedido item in pedido)
            {
                ticket += item.Cantidad + " x " + item.Nombre + "\n";
                ticket += "     $" + item.Subtotal.ToString("0.00") + "\n";
            }
            ticket += "----------------------------\n";
            ticket += "TOTAL: $" + total.ToString("0.00") + "\n";
            ticket += "=============================\n";
            ticket += "Muchas gracias por su compra\n";
            ticket += "Que se mejore pronto.\n";

            txtTicket.Text = ticket;

            // Guardar en JSON
            Ticket datos = new Ticket();
            datos.Cliente = txtCliente.Text;
            datos.Fecha = fecha;
            datos.Productos = new List<ItemPedido>(pedido);
            datos.Total = total;

            string ruta = GuardarJson(datos);
            await MostrarMensaje("Ticket guardado en:\n" + ruta);

            pedido.Clear();
            ActualizarLista();
            txtCliente.Text = "";
        }

        // Guardar el ticket en un archivo .json en documentos
        private string GuardarJson(Ticket datos)
        {
            JsonSerializerOptions opciones = new JsonSerializerOptions();
            opciones.WriteIndented = true;
            opciones.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            string json = JsonSerializer.Serialize(datos, opciones);

            string carpeta = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string nombreArchivo = "ticket_farmacia_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
            string ruta = Path.Combine(carpeta, nombreArchivo);

            File.WriteAllText(ruta, json);
            return ruta;
        }

        // Actualizar el ListView
        private void ActualizarLista()
        {
            lstPedido.Items.Clear();
            foreach (ItemPedido item in pedido)
            {
                lstPedido.Items.Add(item.Cantidad + " x " + item.Nombre + "     $" + item.Subtotal.ToString("0.00"));
            }
        }

        // Muestra el mensaje emergente
        private async Task MostrarMensaje(string mensaje)
        {
            ContentDialog dialogo = new ContentDialog();
            dialogo.Title = "Aviso";
            dialogo.Content = mensaje;
            dialogo.CloseButtonText = "Aceptar";
            dialogo.XamlRoot = this.Content.XamlRoot;
            await dialogo.ShowAsync();
        }
    }
}
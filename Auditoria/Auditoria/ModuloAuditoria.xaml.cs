using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Auditoria
{
    /// <summary>
    /// Lógica de interacción para ModuloAuditoria.xaml
    /// </summary>
    public partial class ModuloAuditoria : Window
    {
        private AuditoriaDAO dao = new AuditoriaDAO();

        public ModuloAuditoria()
        {
            InitializeComponent();
            CargarTransportistas();
        }

        private void CargarTransportistas()
        {
            try
            {
                cbTransportistas.ItemsSource = dao.ObtenerTransportistas();
                cbTransportistas.SelectedIndex = 0; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los transportistas: " + ex.Message, "Error BD", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
           
            if (cbTransportistas.SelectedValue == null)
            {
                MessageBox.Show("Por favor, seleccione un transportista.");
                return;
            }

            if (!decimal.TryParse(txtFleteMin.Text, out decimal fleteMin) ||
                !decimal.TryParse(txtFleteMax.Text, out decimal fleteMax))
            {
                MessageBox.Show("Debe ingresar valores numéricos válidos en los campos de flete.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (fleteMin > fleteMax)
            {
                MessageBox.Show("El flete mínimo no puede ser mayor al flete máximo.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int idTransportista = (int)cbTransportistas.SelectedValue;

                List<PedidoEnvio> resultados = dao.BuscarPedidos(idTransportista, fleteMin, fleteMax);

                dgPedidos.ItemsSource = resultados;

                CalcularIndicadores(resultados);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al buscar los pedidos: " + ex.Message, "Error de Sistema", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalcularIndicadores(List<PedidoEnvio> pedidos)
        {
            int cantidad = 0;
            decimal montoTotal = 0;

            foreach (PedidoEnvio pedido in pedidos)
            {
                cantidad++;
                montoTotal += pedido.MontoFlete;
            }

            decimal promedio = 0;
            if (cantidad > 0)
            {
                promedio = montoTotal / cantidad;
            }

            lblCantidad.Text = cantidad.ToString();
            lblMontoTotal.Text = montoTotal.ToString("C2"); 
            lblPromedio.Text = promedio.ToString("C2");
        }
    }
}

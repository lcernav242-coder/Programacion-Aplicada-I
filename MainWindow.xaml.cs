
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegistroImpresiones
{
    public partial class MainWindow : Window
    {
        // 1. Contadores Globales de Páginas
        int totalEscolares = 0;
        int totalUniversitarias = 0;
        int totalOrganizaciones = 0;

        public MainWindow()
        {
            InitializeComponent(); // Esto es lo que "dibuja" el XAML de arriba
        }

        // ==========================================
        // EVENTOS PARA CAMBIAR LA TARIFA AUTOMÁTICAMENTE
        // ==========================================
        private void rdbEscolar_Checked(object sender, RoutedEventArgs e)
        {
            txtTarifa.Text = "0.10";
        }

        private void rdbUniversitario_Checked(object sender, RoutedEventArgs e)
        {
            txtTarifa.Text = "0.20";
        }

        private void rdbOrganizacion_Checked(object sender, RoutedEventArgs e)
        {
            txtTarifa.Text = "0.15";
        }

        // ==========================================
        // EVENTO DEL BOTÓN REGISTRAR
        // ==========================================
        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            // Validar que se haya llenado la información básica
            if (string.IsNullOrWhiteSpace(txtCliente.Text) || string.IsNullOrWhiteSpace(txtCantidad.Text) || string.IsNullOrWhiteSpace(txtTarifa.Text))
            {
                MessageBox.Show("Por favor, ingrese todos los datos y seleccione un tipo de cliente.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Capturar datos y convertir de Texto a Números
            string cliente = txtCliente.Text;
            string celular = txtCelular.Text;
            int cantidad = int.Parse(txtCantidad.Text);
            double tarifa = double.Parse(txtTarifa.Text);

            // Calcular importe a pagar
            double importe = cantidad * tarifa;

            // Acumular páginas usando los RadioButton de WPF (propiedad IsChecked)
            if (rdbEscolar.IsChecked == true)
            {
                totalEscolares += cantidad;
            }
            else if (rdbUniversitario.IsChecked == true)
            {
                totalUniversitarias += cantidad;
            }
            else if (rdbOrganizacion.IsChecked == true)
            {
                totalOrganizaciones += cantidad;
            }

            // Crear el texto y agregarlo al ListBox grande
            string lineaResultado = $"Cliente: {cliente} | Cel: {celular} | Cantidad: {cantidad} | Tarifa: S/. {tarifa:F2} | Importe: S/. {importe:F2}";
            lstEstadisticas.Items.Add(lineaResultado);

            // Mostrar estadísticas de abajo convertidas a texto
            txtTotalEscolar.Text = totalEscolares.ToString();
            txtTotalUniversitario.Text = totalUniversitarias.ToString();
            txtTotalOrganizacion.Text = totalOrganizaciones.ToString();

            // Limpiar las cajas de texto para el próximo cliente
            txtCliente.Clear();
            txtCantidad.Clear();
            txtCelular.Clear();
            txtCliente.Focus();
        }
    }
}
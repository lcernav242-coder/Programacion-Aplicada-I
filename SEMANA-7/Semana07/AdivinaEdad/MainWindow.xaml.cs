using System;
using System.Windows;

namespace AdivinaEdad
{
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();
        private int contadorIntentos = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnPrimerIntento_Click(object sender, RoutedEventArgs e)
        {
            contadorIntentos = 0;
            RealizarIntento();

            btnCorrecto.IsEnabled = true;
            btnIncorrecto.IsEnabled = true;
        }

        private void btnIncorrecto_Click(object sender, RoutedEventArgs e)
        {
            RealizarIntento();
        }

        private void RealizarIntento()
        {
            if (int.TryParse(txtMinimo.Text, out int min) && int.TryParse(txtMaximo.Text, out int max))
            {
                if (min <= max)
                {
                    int edadGenerada = rnd.Next(min, max + 1);

                    txtEdadAdivinada.Text = edadGenerada.ToString();
                    contadorIntentos++;
                }
                else
                {
                    MessageBox.Show("El valor mínimo no puede ser mayor al máximo.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa un intervalo de números válido.");
            }
        }

        private void btnCorrecto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Edad Correcta: {contadorIntentos} intentos");

            txtEdadAdivinada.Text = "";
            btnCorrecto.IsEnabled = false;
            btnIncorrecto.IsEnabled = false;
        }
    }
}
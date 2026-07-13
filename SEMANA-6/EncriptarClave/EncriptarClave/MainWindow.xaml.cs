using System;
using System.Windows;

namespace EncriptarClave // Cambia esto si tu proyecto se llama diferente
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEncriptar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtener la clave ingresada
            string claveOriginal = txtClaveOriginal.Text.Trim();

            // 2. Validar que no esté vacía
            if (string.IsNullOrEmpty(claveOriginal))
            {
                MessageBox.Show("Por favor, ingrese una clave para encriptar.");
                return;
            }

            string claveEncriptada = "";
            int nivelDeDesplazamiento = 5; // Puedes cambiar este número para que la encriptación sea diferente

            // 3. Recorrer la clave letra por letra para transformarla
            foreach (char letra in claveOriginal)
            {
                // Convertimos el carácter a su valor numérico (ASCII), le sumamos el desplazamiento y lo regresamos a texto
                int valorAscii = (int)letra;
                valorAscii += nivelDeDesplazamiento;

                claveEncriptada += (char)valorAscii;
            }

            // 4. Mostrar el resultado en pantalla
            txtClaveEncriptada.Text = claveEncriptada;
        }
    }
}
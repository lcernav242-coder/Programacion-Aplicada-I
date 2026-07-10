using System;
using System.Windows;

namespace OrdenarCadenas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnListar_Click(object sender, RoutedEventArgs e)
        {
            lstTodos.Items.Clear();

            string cadena = txtCadenaOriginal.Text.Trim();

            if (string.IsNullOrEmpty(cadena))
            {
                MessageBox.Show("Por favor, asegúrese de que haya nombres en la caja de texto.");
                return;
            }

            string[] nombres = cadena.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string nombre in nombres)
            {
                lstTodos.Items.Add(nombre);
            }

            txtContadorTotal.Text = nombres.Length.ToString();
        }

        private void btnPasar_Click(object sender, RoutedEventArgs e)
        {
            lstFiltrados.Items.Clear();

            string letra = txtLetraFiltro.Text.Trim();

            if (string.IsNullOrEmpty(letra))
            {
                MessageBox.Show("Por favor, ingrese la inicial que desea buscar.");
                return;
            }

            int contador = 0;

            foreach (var item in lstTodos.Items)
            {
                string nombre = item.ToString();

                if (nombre.StartsWith(letra, StringComparison.OrdinalIgnoreCase))
                {
                    lstFiltrados.Items.Add(nombre);
                    contador++;
                }
            }

            txtContadorFiltrados.Text = contador.ToString();
        }
    }
}
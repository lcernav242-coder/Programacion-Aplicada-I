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

namespace Temperaturas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TextBox[] txtTemperaturas;
        private string[] meses = new string[] {
            "Enero", "Febrero", "Marzo", "Abril",
            "Mayo", "Junio", "Julio", "Agosto",
            "Setiembre", "Octubre", "Noviembre", "Diciembre"};

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCalcular_Click(object sender, RoutedEventArgs e)
        {
            double[] temperaturas = new double[12];
            double suma = 0;
            lbMeses.Items.Clear();

            for (int i = 0; i < txtTemperaturas.Length; i++)
            {
                if (double.TryParse(txtTemperaturas[i].Text, out double temperatura))
                {
                    temperaturas[i] = temperatura;
                    suma = suma + temperatura;
                }
                else
                {
                    MessageBox.Show("Ingrese una temperatura Válida");
                    txtTemperaturas[i].Focus();
                    return;
                }
            }

            double promedio = suma / 12;
            tbPromedio.Text = promedio.ToString("N2");

            int cantidadMayorPromedio = 0;
            for (int i = 0; i < temperaturas.Length; i++)
            {
                if (temperaturas[i] > promedio)
                {
                    cantidadMayorPromedio++;
                    lbMeses.Items.Add(meses[i]);
                }
            }

            tbCantidad.Text = cantidadMayorPromedio.ToString();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtTemperaturas = new TextBox[] {
                txtEnero,txtFebrero,txtMarzo,txtAbril,
                txtMayo,txtJunio,txtJulio,txtAgosto,txtSetiembre,
                txtOctubre,txtNoviembre,txtDiciembre
            };

            Random random = new Random();
            for (int i = 0; i < txtTemperaturas.Length; i++)
            {
                txtTemperaturas[i].Text = (random.NextDouble() * 10).ToString("N2");
            }
        }
    }
}
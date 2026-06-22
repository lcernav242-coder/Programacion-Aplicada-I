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

namespace RegistroPromedios
{
    public partial class MainWindow : Window
    {
        public class AlumnoRegistro
        {
            public string NombreCompleto { get; set; }
            public string Codigo { get; set; }
            public string Nivel { get; set; }
            public double Eval1 { get; set; }
            public double Eval2 { get; set; }
            public double Eval3 { get; set; }
            public double Promedio { get; set; }
            public string PromedioFormatted => Promedio.ToString("N2");
            public string Condicion { get; set; }
        }

        private List<AlumnoRegistro> listaAlumnosFinal;

        public MainWindow()
        {
            InitializeComponent();
            listaAlumnosFinal = new List<AlumnoRegistro>();
            lvResultados.ItemsSource = listaAlumnosFinal;
            CargarNiveles();
        }

        private void CargarNiveles()
        {
            cmbNivel.Items.Add("Pregrado");
            cmbNivel.Items.Add("Posgrado");
            cmbNivel.Items.Add("Extensión");
        }

        private void btnAgregarNotas_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtApellidos.Text) || string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtCodigo.Text) || cmbNivel.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los Datos Generales.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(txtEval1.Text, out double e1) || !double.TryParse(txtEval2.Text, out double e2) || !double.TryParse(txtEval3.Text, out double e3))
            {
                MessageBox.Show("Ingrese valores numéricos válidos en las evaluaciones.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (e1 < 0 || e1 > 20 || e2 < 0 || e2 > 20 || e3 < 0 || e3 > 20)
            {
                MessageBox.Show("Las notas deben estar entre 0 y 20.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double promedio = (e1 + e2 + e3) / 3.0;
            string condicion = (promedio >= 10.5) ? "Aprobado" : "Desaprobado";

            txtPromedioOutput.Text = promedio.ToString("N2");
            txtCondicionOutput.Text = condicion;

            listaAlumnosFinal.Add(new AlumnoRegistro
            {
                NombreCompleto = $"{txtApellidos.Text.ToUpper()}, {txtNombres.Text}",
                Codigo = txtCodigo.Text,
                Nivel = cmbNivel.SelectedItem.ToString(),
                Eval1 = e1,
                Eval2 = e2,
                Eval3 = e3,
                Promedio = promedio,
                Condicion = condicion
            });

            lvResultados.Items.Refresh();

            // Se limpian solo los campos de entrada para facilitar el registro del siguiente alumno
            txtApellidos.Clear();
            txtNombres.Clear();
            txtCodigo.Clear();
            cmbNivel.SelectedIndex = -1;
            txtEval1.Clear();
            txtEval2.Clear();
            txtEval3.Clear();
            txtApellidos.Focus();
        }

        private void btnMediaPromedios_Click(object sender, RoutedEventArgs e)
        {
            if (listaAlumnosFinal.Count == 0)
            {
                MessageBox.Show("No hay estudiantes registrados para calcular la media.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Calcula la Media de los Promedios exactamente como pediste
            double media = listaAlumnosFinal.Average(x => x.Promedio);
            txtMediaPromedios.Text = media.ToString("N2");
        }
    }
}
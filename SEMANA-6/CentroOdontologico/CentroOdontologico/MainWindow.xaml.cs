using System;
using System.Windows;

namespace CentroOdontologico
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CargarOpciones();
        }

        private void CargarOpciones()
        {
            // Opciones de tratamientos
            cboTratamiento.Items.Add("Limpieza dental (Profilaxis)");
            cboTratamiento.Items.Add("Curación de caries");
            cboTratamiento.Items.Add("Extracción dental");
            cboTratamiento.Items.Add("Blanqueamiento");
            cboTratamiento.Items.Add("Ortodoncia (Evaluación)");
            cboTratamiento.Items.Add("Endodoncia");
            cboTratamiento.SelectedIndex = 0;
           
            // Opciones de piezas dentales
            cboPiezaDental.Items.Add("Incisivo Central (11)");
            cboPiezaDental.Items.Add("Incisivo Lateral (12)");
            cboPiezaDental.Items.Add("Canino (13)");
            cboPiezaDental.Items.Add("Premolar (14)");
            cboPiezaDental.Items.Add("Molar Superior (16)");
            cboPiezaDental.Items.Add("Molar Inferior (36)");
            cboPiezaDental.Items.Add("General (Toda la boca)");
            cboPiezaDental.SelectedIndex = 0;

            // Configurar ventana
            this.Title = "Registro de Citas - Centro Odontológico";
            this.Width = 650;
            this.Height = 500;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void btnCronogramar_Click(object sender, RoutedEventArgs e)
        {
            string paciente = txtPaciente.Text.Trim();
            string tratamiento = cboTratamiento.SelectedItem?.ToString() ?? "";
            string pieza = cboPiezaDental.SelectedItem?.ToString() ?? "";

            // Obtener la fecha seleccionada en el calendario
            string fecha = calFecha.SelectedDate.HasValue
                ? calFecha.SelectedDate.Value.ToShortDateString()
                : DateTime.Now.ToShortDateString();

            // Validar paciente
            if (string.IsNullOrEmpty(paciente))
            {
                MessageBox.Show("Por favor, ingrese el nombre del paciente.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Crear el registro de la cita
            string registro = $"• [FECHA: {fecha}] - PACIENTE: {paciente} | TRATAMIENTO: {tratamiento} | PIEZA: {pieza}";
            lstRegistro.Items.Add(registro);

            // Limpiar campo para siguiente cita
            txtPaciente.Clear();
            txtPaciente.Focus();

            MessageBox.Show("Cita cronogramada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
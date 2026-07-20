using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TragamonedasV2
{
    public partial class MainWindow : Window
    {
        // Declaración de variables y Timers
        private DispatcherTimer relojTimer;
        private DispatcherTimer juegoTimer;

        private Random rnd = new Random();
        private BitmapImage[] arrayImagenes;
        private int puntaje = 0;
        private int segundosTranscurridos = 0;

        public MainWindow()
        {
            InitializeComponent();
            CargarImagenes();
            ConfigurarTimers();

            // Limpiamos la interfaz al arrancar
            txtPuntaje.Text = "0";
            txtResultado.Text = "";
        }

        // Método para precargar las 6 imágenes en memoria
        private void CargarImagenes()
        {
            arrayImagenes = new BitmapImage[6];
            for (int i = 0; i < 6; i++)
            {
                // Carga las imágenes usando la ruta exacta de tu Explorador de Soluciones
                arrayImagenes[i] = new BitmapImage(new Uri($"pack://application:,,,/Imagenes/{i + 1}.png"));
            }
        }

        // Configuración de los dos DispatcherTimer
        private void ConfigurarTimers()
        {
            // Timer 1: Actualiza el reloj cada 1 segundo
            relojTimer = new DispatcherTimer();
            relojTimer.Interval = TimeSpan.FromSeconds(1);
            relojTimer.Tick += RelojTimer_Tick;
            relojTimer.Start();

            // Timer 2: Lógica del juego cada 1 segundo
            juegoTimer = new DispatcherTimer();
            juegoTimer.Interval = TimeSpan.FromSeconds(1);
            juegoTimer.Tick += JuegoTimer_Tick;
        }

        // Evento que actualiza la hora en el TextBox superior
        private void RelojTimer_Tick(object sender, EventArgs e)
        {
            txtReloj.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        // Evento del botón "Iniciar Juego"
        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            IniciarNuevoJuego();
        }

        private void IniciarNuevoJuego()
        {
            // Reiniciamos los valores para una partida limpia
            puntaje = 0;
            segundosTranscurridos = 0;
            txtPuntaje.Text = "0";
            txtResultado.Text = "";

            // Deshabilitamos el botón mientras los rodillos giran
            btnIniciar.IsEnabled = false;

            // Arrancamos el timer del juego
            juegoTimer.Start();
        }

        // Evento principal: Se ejecuta cada vez que pasa un segundo en el juego
        private void JuegoTimer_Tick(object sender, EventArgs e)
        {
            segundosTranscurridos++;

            // 1. Generamos 3 números aleatorios del 0 al 5 (para los índices del arreglo)
            int indice1 = rnd.Next(0, 6);
            int indice2 = rnd.Next(0, 6);
            int indice3 = rnd.Next(0, 6);

            // 2. Asignamos las imágenes a los controles del XAML
            imgSlot1.Source = arrayImagenes[indice1];
            imgSlot2.Source = arrayImagenes[indice2];
            imgSlot3.Source = arrayImagenes[indice3];

            // 3. Evaluamos la lógica de puntaje
            if (indice1 == indice2 && indice2 == indice3)
            {
                puntaje += 20; // 3 iguales
            }
            else if (indice1 == indice2 || indice1 == indice3 || indice2 == indice3)
            {
                puntaje += 10; // 2 iguales
            }

            // Actualizamos el número amarillo en pantalla
            txtPuntaje.Text = puntaje.ToString();

            // 4. Verificamos si el juego debe terminar
            if (puntaje >= 60)
            {
                FinalizarJuego("GANASTE");
            }
            else if (segundosTranscurridos >= 8)
            {
                FinalizarJuego($"PERDISTE, puntaje obtenido: {puntaje}");
            }
        }

        // Método para manejar el fin del juego y preguntar si se desea continuar
        private void FinalizarJuego(string mensajeFinal)
        {
            juegoTimer.Stop();
            btnIniciar.IsEnabled = true;

            txtResultado.Text = mensajeFinal;

            MessageBoxResult respuesta = MessageBox.Show(
                mensajeFinal + "\n\n¿Deseas seguir jugando?",
                "Fin del Juego",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta == MessageBoxResult.Yes)
            {
                IniciarNuevoJuego();
            }
            else
            {
                txtResultado.Text += " - Juego Terminado";
            }
        }
    }
}
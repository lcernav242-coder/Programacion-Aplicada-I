using Connect4;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Connect4
{
    public partial class MainWindow : Window
    {
        private JuegoConnect4 juego;

        private Button[,] botonesTablero;

        public MainWindow()
        {
            InitializeComponent();
            IniciarPartida();
        }

        private void IniciarPartida()
        {
            try
            {
                juego = new JuegoConnect4();
                botonesTablero = new Button[juego.Filas, juego.Columnas];
                gridTablero.Children.Clear();

                for (int f = 0; f < juego.Filas; f++)
                {
                    for (int c = 0; c < juego.Columnas; c++)
                    {
                        Button btnSlot = new Button
                        {
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            Cursor = System.Windows.Input.Cursors.Hand,
                            Tag = c 
                        };

                        Ellipse circulo = new Ellipse
                        {
                            Fill = Brushes.White,
                            Margin = new Thickness(8), 
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };

                        btnSlot.Content = circulo;
                        btnSlot.Click += Columna_Click;

                        botonesTablero[f, c] = btnSlot;
                        gridTablero.Children.Add(btnSlot);
                    }
                }

                ActualizarUIEstado();
                gridTablero.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico al inicializar el tablero: {ex.Message}", "Error de Sistema", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Columna_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btnPresionado = sender as Button;
                if (btnPresionado == null) return;

                int columnaSeleccionada = (int)btnPresionado.Tag;

                int filaAterrizaje = juego.InsertarFicha(columnaSeleccionada);

                if (filaAterrizaje == -1)
                {
                    MessageBox.Show("Esta columna ya está llena. Elige otra.", "Movimiento Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Ellipse circuloFicha = (Ellipse)botonesTablero[filaAterrizaje, columnaSeleccionada].Content;
                circuloFicha.Fill = juego.TurnoActual == 1 ? Brushes.Red : Brushes.Yellow;

                if (juego.VerificarVictoria(filaAterrizaje, columnaSeleccionada))
                {
                    string ganador = juego.TurnoActual == 1 ? "ROJO" : "AMARILLO";
                    lblEstado.Text = $"¡VICTORIA DEL JUGADOR {ganador}!";
                    lblEstado.Foreground = Brushes.LimeGreen;

                    gridTablero.IsEnabled = false; // Bloquea el tablero
                    MessageBox.Show($"¡Felicidades, el Jugador {ganador} ha conectado 4!", "Fin del Juego", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    juego.CambiarTurno();
                    ActualizarUIEstado();
                }
            }
            catch (InvalidOperationException exOp)
            {
                MessageBox.Show(exOp.Message, "Atención", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado durante el turno: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarUIEstado()
        {
            if (juego.TurnoActual == 1)
            {
                lblEstado.Text = "Turno: Jugador 1 (ROJO)";
                lblEstado.Foreground = Brushes.Red;
            }
            else
            {
                lblEstado.Text = "Turno: Jugador 2 (AMARILLO)";
                lblEstado.Foreground = Brushes.Gold;
            }
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            IniciarPartida();
        }

        private void MenuSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
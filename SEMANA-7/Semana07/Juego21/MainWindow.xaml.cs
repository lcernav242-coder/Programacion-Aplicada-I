using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Juego21
{
    public partial class MainWindow : Window
    {
        // Estructura interna para manejar las cartas
        private struct Carta
        {
            public string NombreArchivo;
            public int Valor;
        }

        private List<Carta> mazo;
        private Random rnd = new Random();

        private int puntosJ1 = 0;
        private int puntosJ2 = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- INICIO Y GESTIÓN DEL MAZO ---

        private void btnNuevoJuego_Click(object sender, RoutedEventArgs e)
        {
            // Reiniciar puntajes y textos
            puntosJ1 = 0;
            puntosJ2 = 0;
            lblPuntosJ1.Text = "0";
            lblPuntosJ2.Text = "0";

            // Limpiar las imágenes de la mesa
            pnlCartasJ1.Children.Clear();
            pnlCartasJ2.Children.Clear();

            // Ocultar mensaje de resultado
            brdResultado.Visibility = Visibility.Collapsed;

            // Llenar el mazo con las 52 cartas
            GenerarMazo();

            // Habilitar turno del Jugador 1
            btnPedirJ1.IsEnabled = true;
            btnPlantarJ1.IsEnabled = true;

            btnPedirJ2.IsEnabled = false;
            btnPlantarJ2.IsEnabled = false;
        }

        private void GenerarMazo()
        {
            mazo = new List<Carta>();
            string[] palos = { "C", "D", "E", "T" }; // Corazones, Diamantes, Espadas, Tréboles

            foreach (string palo in palos)
            {
                for (int i = 1; i <= 13; i++)
                {
                    // Si es 11(J), 12(Q) o 13(K), vale 10. Si no, vale su número.
                    int puntosReales = (i > 10) ? 10 : i;

                    mazo.Add(new Carta
                    {
                        NombreArchivo = $"{i}{palo}.png",
                        Valor = puntosReales
                    });
                }
            }
        }

        private Carta ExtraerCartaAleatoria()
        {
            // Elegir un índice al azar, guardar la carta y sacarla de la lista para que no se repita
            int indice = rnd.Next(mazo.Count);
            Carta cartaSeleccionada = mazo[indice];
            mazo.RemoveAt(indice);
            return cartaSeleccionada;
        }

        private void MostrarCartaEnPanel(WrapPanel panel, string nombreArchivo)
        {
            // Crear el control Image dinámicamente
            Image nuevaImagen = new Image();
            nuevaImagen.Width = 70;
            nuevaImagen.Height = 100;
            nuevaImagen.Margin = new Thickness(2);

            // Cargar la imagen desde la carpeta "Images"
            nuevaImagen.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{nombreArchivo}"));

            panel.Children.Add(nuevaImagen);
        }

        // --- TURNO DEL JUGADOR 1 ---

        private void btnPedirJ1_Click(object sender, RoutedEventArgs e)
        {
            Carta carta = ExtraerCartaAleatoria();
            MostrarCartaEnPanel(pnlCartasJ1, carta.NombreArchivo);

            puntosJ1 += carta.Valor;
            lblPuntosJ1.Text = puntosJ1.ToString();

            // Regla: Si se pasa de 21, pierde automáticamente
            if (puntosJ1 > 21)
            {
                FinalizarJuego("¡GANADOR JUGADOR 2! El Jugador 1 se pasó de 21 puntos.");
            }
        }

        private void btnPlantarJ1_Click(object sender, RoutedEventArgs e)
        {
            // Termina turno J1, empieza J2
            btnPedirJ1.IsEnabled = false;
            btnPlantarJ1.IsEnabled = false;

            btnPedirJ2.IsEnabled = true;
            btnPlantarJ2.IsEnabled = true;
        }

        // --- TURNO DEL JUGADOR 2 ---

        private void btnPedirJ2_Click(object sender, RoutedEventArgs e)
        {
            Carta carta = ExtraerCartaAleatoria();
            MostrarCartaEnPanel(pnlCartasJ2, carta.NombreArchivo);

            puntosJ2 += carta.Valor;
            lblPuntosJ2.Text = puntosJ2.ToString();

            // Regla: Si se pasa de 21, pierde automáticamente
            if (puntosJ2 > 21)
            {
                FinalizarJuego("¡GANADOR JUGADOR 1! El Jugador 2 se pasó de 21 puntos.");
            }
        }

        private void btnPlantarJ2_Click(object sender, RoutedEventArgs e)
        {
            // Termina el juego y se evalúa quién ganó
            EvaluarGanador();
        }

        // --- EVALUACIÓN FINAL ---

        private void EvaluarGanador()
        {
            string mensaje = "";

            if (puntosJ1 > puntosJ2)
            {
                mensaje = $"¡GANADOR JUGADOR 1! Obtuvo {puntosJ1} puntos frente a {puntosJ2} del J2.";
            }
            else if (puntosJ2 > puntosJ1)
            {
                mensaje = $"¡GANADOR JUGADOR 2! Obtuvo {puntosJ2} puntos frente a {puntosJ1} del J1.";
            }
            else
            {
                mensaje = $"¡EMPATE! Ambos jugadores obtuvieron {puntosJ1} puntos.";
            }

            FinalizarJuego(mensaje);
        }

        private void FinalizarJuego(string mensajeFInal)
        {
            // Bloquear todos los botones de juego
            btnPedirJ1.IsEnabled = false;
            btnPlantarJ1.IsEnabled = false;
            btnPedirJ2.IsEnabled = false;
            btnPlantarJ2.IsEnabled = false;

            // Mostrar el mensaje en la barra inferior
            txtResultado.Text = mensajeFInal;
            brdResultado.Visibility = Visibility.Visible;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JuegoTinka
{
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSortear_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show("Por favor, ingresa tu Nombre y Dirección antes de empezar la jugada.",
                                "Registro Requerido",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            int[] jugada1 = LeerJugada(txtJ1_1, txtJ1_2, txtJ1_3, txtJ1_4, txtJ1_5, txtJ1_6);
            int[] jugada2 = LeerJugada(txtJ2_1, txtJ2_2, txtJ2_3, txtJ2_4, txtJ2_5, txtJ2_6);
            int[] jugada3 = LeerJugada(txtJ3_1, txtJ3_2, txtJ3_3, txtJ3_4, txtJ3_5, txtJ3_6);

            if (jugada1 == null || jugada2 == null || jugada3 == null)
            {
                return;
            }

            List<int> ganadora = new List<int>();
            while (ganadora.Count < 6)
            {
                int numAleatorio = rnd.Next(1, 46);
                if (!ganadora.Contains(numAleatorio))
                {
                    ganadora.Add(numAleatorio);
                }
            }

            ganadora.Sort();

            lblT1.Text = ganadora[0].ToString();
            lblT2.Text = ganadora[1].ToString();
            lblT3.Text = ganadora[2].ToString();
            lblT4.Text = ganadora[3].ToString();
            lblT5.Text = ganadora[4].ToString();
            lblT6.Text = ganadora[5].ToString();

            List<int[]> todasLasJugadas = new List<int[]> { jugada1, jugada2, jugada3 };
            int maximosAciertosObtenidos = 0;

            foreach (var jugada in todasLasJugadas)
            {
                int aciertos = jugada.Intersect(ganadora).Count();
                if (aciertos > maximosAciertosObtenidos)
                {
                    maximosAciertosObtenidos = aciertos;
                }
            }

            if (maximosAciertosObtenidos >= 3)
            {
                lblTicketNombre.Text = txtNombre.Text;
                lblTicketDireccion.Text = txtDireccion.Text;
                lblTicketFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

                if (maximosAciertosObtenidos == 6)
                    lblMensajePremio.Text = "¡PREMIO MAYOR!";
                else if (maximosAciertosObtenidos == 5)
                    lblMensajePremio.Text = "¡GANASTE S/. 5,000!";
                else if (maximosAciertosObtenidos == 4)
                    lblMensajePremio.Text = "¡GANASTE S/. 100!";
                else if (maximosAciertosObtenidos == 3)
                    lblMensajePremio.Text = "¡GANASTE S/. 5!";
            }
            else
            {
                lblTicketNombre.Text = "---";
                lblTicketDireccion.Text = "---";
                lblTicketFecha.Text = "---";
                lblMensajePremio.Text = "Sin premio. ¡Sigue intentando!";
            }
        }

        
        private int[] LeerJugada(TextBox t1, TextBox t2, TextBox t3, TextBox t4, TextBox t5, TextBox t6)
        {
            int[] jugada = new int[6];
            TextBox[] cajas = { t1, t2, t3, t4, t5, t6 };

            for (int i = 0; i < 6; i++)
            {
                if (!int.TryParse(cajas[i].Text, out int numero) || numero < 1 || numero > 45)
                {
                    MessageBox.Show("Asegúrate de llenar todas las casillas con números válidos entre 1 y 45.",
                                    "Error en las jugadas",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return null;
                }
                jugada[i] = numero;
            }

            if (jugada.Distinct().Count() != 6)
            {
                MessageBox.Show("No puedes repetir el mismo número dentro de una misma jugada.",
                                "Error de duplicados",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return null;
            }

            return jugada;
        }
    }
}
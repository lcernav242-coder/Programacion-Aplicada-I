using System;

namespace Connect4
{
    public class JuegoConnect4
    {
        public int[,] Tablero { get; private set; }
        public int TurnoActual { get; private set; } 
        public int Filas { get; } = 6;
        public int Columnas { get; } = 7;
        public bool JuegoTerminado { get; private set; }

        public JuegoConnect4()
        {
            ReiniciarJuego();
        }

        public void ReiniciarJuego()
        {
            Tablero = new int[Filas, Columnas];
            TurnoActual = 1;
            JuegoTerminado = false;
        }

        public int InsertarFicha(int columna)
        {
            if (columna < 0 || columna >= Columnas)
                throw new ArgumentOutOfRangeException("La columna especificada está fuera de los límites del tablero.");

            if (JuegoTerminado)
                throw new InvalidOperationException("El juego ya ha terminado. Reinicie para volver a jugar.");

            for (int fila = Filas - 1; fila >= 0; fila--)
            {
                if (Tablero[fila, columna] == 0)
                {
                    Tablero[fila, columna] = TurnoActual;
                    return fila; 
                }
            }

            return -1;
        }

        public void CambiarTurno()
        {
            TurnoActual = (TurnoActual == 1) ? 2 : 1;
        }

        public bool VerificarVictoria(int fila, int columna)
        {
            bool victoria = VerificarDireccion(fila, columna, 1, 0) || 
                            VerificarDireccion(fila, columna, 0, 1) || 
                            VerificarDireccion(fila, columna, 1, 1) || 
                            VerificarDireccion(fila, columna, 1, -1);  

            if (victoria) JuegoTerminado = true;

            return victoria;
        }

        private bool VerificarDireccion(int f, int c, int difFila, int difCol)
        {
            int contador = 1;
            int jugador = Tablero[f, c];

            int i = f + difFila, j = c + difCol;
            while (i >= 0 && i < Filas && j >= 0 && j < Columnas && Tablero[i, j] == jugador)
            {
                contador++;
                i += difFila; j += difCol;
            }

            i = f - difFila; j = c - difCol;
            while (i >= 0 && i < Filas && j >= 0 && j < Columnas && Tablero[i, j] == jugador)
            {
                contador++;
                i -= difFila; j -= difCol;
            }

            return contador >= 4;
        }
    }
}
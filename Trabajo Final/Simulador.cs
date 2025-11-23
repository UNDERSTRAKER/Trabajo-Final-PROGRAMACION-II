using System;
using System.Collections.Generic;

// LUIS FELIPE PADILLA - 2316668

namespace WpfInventarioCorte3
{
    public class Simulador
    {
        private long A, X0, B, N;
        private long _semillaActual;

        public void ConfigurarLCG(long a, long x0, long b, long n)
        {
            A = a; X0 = x0; B = b; N = n;
            _semillaActual = x0;
        }

        private double GenerarRi()
        {
            if (N == 0) return 0;
            long next = (A * _semillaActual + B) % N;
            _semillaActual = next;
            return (double)next / N;
        }

        private int CalcularDemanda(double ri, bool esModelo2)
        {
            if (!esModelo2) // Modelo 1 
            {
                if (ri < 0.12) return 30;
                if (ri < 0.33) return 32;
                if (ri < 0.58) return 34;
                if (ri < 0.85) return 36;
                return 38;
            }
            else // Modelo 2 
            {
                if (ri < 0.21) return 50;
                if (ri < 0.37) return 55;
                if (ri < 0.54) return 60;
                if (ri < 0.78) return 65;
                return 70;
            }
        }

        public List<FilaSimulacion> Ejecutar(int dias, int Q, int T, int invInicial, double cPed, double cInv, double cFalt, bool esModelo2)
        {
            var lista = new List<FilaSimulacion>();
            _semillaActual = X0;

            GenerarRi();

            for (int dia = 1; dia <= dias; dia++)
            {
                FilaSimulacion fila = new FilaSimulacion { Dia = dia };

                if (dia == 1)
                {
                    fila.InvInicial = invInicial;
                }
                else
                {
                    int invFinalAyer = lista[dia - 2].InvFinal;

                    int invFisicoAyer = (invFinalAyer < 0) ? 0 : invFinalAyer;

                    if ((dia - 1) % T == 0)
                    {
                        fila.InvInicial = invFisicoAyer + Q;
                        fila.Pedido = Q;
                    }
                    else
                    {
                        fila.InvInicial = invFisicoAyer;
                        fila.Pedido = 0;
                    }
                }
                fila.Ri = GenerarRi();
                fila.Demanda = CalcularDemanda(fila.Ri, esModelo2);

                fila.InvFinal = fila.InvInicial - fila.Demanda;

                if (fila.InvFinal < 0)
                {
                    fila.Faltante = Math.Abs(fila.InvFinal);
                }
                else
                {
                    fila.Faltante = 0;
                }

                fila.CostoInv = (fila.InvFinal > 0) ? fila.InvFinal * cInv : 0;
                fila.CostoPed = (fila.Pedido > 0) ? cPed : 0;
                fila.CostoFalt = fila.Faltante * cFalt;

                fila.CostoTotalDia = fila.CostoInv + fila.CostoPed + fila.CostoFalt;

                lista.Add(fila);
            }
            return lista;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

// LUIS FELIPE PADILLA - 2316668

namespace WpfInventarioCorte3
{
    public partial class MainWindow : Window
    {
        private Simulador _simulador;

        public MainWindow()
        {
            InitializeComponent();
            _simulador = new Simulador();
        }

        private void BtnSimular_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Q = int.Parse(txtQ.Text);
                int T = int.Parse(txtT.Text);
                int invIni = int.Parse(txtInvInicial.Text);
                double cFalt = double.Parse(txtCostoFalt.Text);
                double cPed = double.Parse(txtCostoPed.Text);
                double cInv = double.Parse(txtCostoInv.Text);

                long A = long.Parse(txtA.Text);
                long X0 = long.Parse(txtX0.Text);
                long B = long.Parse(txtB.Text);
                long N = long.Parse(txtN.Text);

                bool esModelo2 = (Q == 452);

                _simulador.ConfigurarLCG(A, X0, B, N);
                var resultados = _simulador.Ejecutar(1000, Q, T, invIni, cPed, cInv, cFalt, esModelo2);

                gridResultados.ItemsSource = resultados;

                double total = resultados.Where(r => r.Dia >= 100).Sum(r => r.CostoTotalDia);
                lblTotal.Text = total.ToString("C0");

                DibujarGrafico(resultados);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en los datos: " + ex.Message);
            }
        }

        private void BtnCargarModelo1_Click(object sender, RoutedEventArgs e)
        {
            txtQ.Text = "307"; txtT.Text = "13";
            txtInvInicial.Text = "965"; txtCostoFalt.Text = "1789";
            txtCostoPed.Text = "7452"; txtCostoInv.Text = "24";

            LimpiarVista();
        }

        private void BtnCargarModelo2_Click(object sender, RoutedEventArgs e)
        {
            txtQ.Text = "452"; txtT.Text = "22";
            txtInvInicial.Text = "1245"; txtCostoFalt.Text = "3542";
            txtCostoPed.Text = "8911"; txtCostoInv.Text = "56";

            LimpiarVista();
        }

        private void LimpiarVista()
        {
            lblTotal.Text = "$ 0";
            gridResultados.ItemsSource = null;
            canvasGrafico.Children.Clear();
            lblMaxY.Text = "Max";
        }

        private void DibujarGrafico(List<FilaSimulacion> datos)
        {
            canvasGrafico.Children.Clear();
            if (datos.Count == 0) return;

            double w = canvasGrafico.ActualWidth;
            double h = canvasGrafico.ActualHeight;

            double maxInv = datos.Max(x => x.InvFinal);
            double minInv = datos.Min(x => x.InvFinal); 

            double rangoTotal = maxInv - Math.Min(0, minInv);
            if (rangoTotal == 0) rangoTotal = 100;

            lblMaxY.Text = maxInv.ToString();

            double scaleX = w / datos.Count;
            double scaleY = h / rangoTotal;
            double baseCeroY = h - (Math.Abs(Math.Min(0, minInv)) * scaleY); 

            DibujarEjes(w, h, baseCeroY);

            Polyline linea = new Polyline { Stroke = Brushes.Blue, StrokeThickness = 1 };
            PointCollection puntos = new PointCollection();

            foreach (var fila in datos)
            {
                double x = (fila.Dia - 1) * scaleX;
                double y = baseCeroY - (fila.InvFinal * scaleY);
                puntos.Add(new Point(x, y));
            }
            linea.Points = puntos;
            canvasGrafico.Children.Add(linea);
        }

        private void DibujarEjes(double w, double h, double yCero)
        {
            canvasGrafico.Children.Add(new Line { X1 = 0, Y1 = yCero, X2 = w, Y2 = yCero, Stroke = Brushes.Black, StrokeThickness = 1 });

            canvasGrafico.Children.Add(new Line { X1 = 0, Y1 = 0, X2 = 0, Y2 = h, Stroke = Brushes.Black, StrokeThickness = 1 });
        }
    }
}
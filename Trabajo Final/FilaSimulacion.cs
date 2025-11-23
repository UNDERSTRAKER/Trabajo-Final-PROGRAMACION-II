using System;

// LUIS FELIPE PADILLA - 2316668

namespace WpfInventarioCorte3
{
    public class FilaSimulacion
    {
        public int Dia { get; set; }
        public double Ri { get; set; }      
        public int Demanda { get; set; }     
        public int InvInicial { get; set; }    
        public int InvFinal { get; set; }     
        public int Faltante { get; set; }      
        public int Pedido { get; set; }        


        public double CostoInv { get; set; }
        public double CostoPed { get; set; }
        public double CostoFalt { get; set; }
        public double CostoTotalDia { get; set; }
    }
}
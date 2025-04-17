using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Cliente
{
    public class Cliente
    {
        public int ID { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public string Estado { get; set; } = "Conectado";

        // Info del sistema
        public string Equipo { get; set; }
        public string Usuario { get; set; }
        public string Sistema { get; set; }
        public string DotNet { get; set; }
        public string Procesadores { get; set; }
        public string RAM { get; set; }
        public string CPU { get; set; }
        public string GPU { get; set; }
    }
}

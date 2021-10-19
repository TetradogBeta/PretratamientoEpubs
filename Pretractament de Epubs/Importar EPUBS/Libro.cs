using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportarEPUBS
{
    public class Libro
    {
        public Libro(bool genId = true)
        {
            if (genId)
                Id = DateTime.UtcNow.Ticks;
        }
        public long Id { get; set; }
        public string? Titulo { get; set; }
        public string? Editorial { get; set; }
        public string? Autor { get; set; }
        public int Año { get; set; }
    }
}

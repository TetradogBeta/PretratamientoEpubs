using CommonEbookPretractament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportarEPUBS
{
    public class Version
    {//si es null se sale del epub y si no está en él es desconocido
        public Libro? Libro { get; set; }
        public Ebook? Epub { get; set; }
        public string? Id { get; set; }
        public string? Autor { get; set; }
        public string? Editorial { get; set; }
        public int Año { get; set; }
        public Uri? ImgPortada { get; set; }
        public string? IdiomaCulture { get; set; }
        public Version? Base { get; set; }
        public bool EsBase => ReferenceEquals(Base, default);
    }
}

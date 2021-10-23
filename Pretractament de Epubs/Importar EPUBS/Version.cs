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
        public Version(Libro libro,Ebook ebook)
        {
            Libro = libro;
            Ebook = ebook;
            Capitulos = new Capitulo[Ebook.TotalChapters];
            for (int i = 0; i < Capitulos.Length; i++)
                Capitulos[i] = new Capitulo(this, i);
        }
        public Libro? Libro { get; set; }
        public Ebook? Ebook { get; set; }
        public string? Id { get; set; }
        public string? Autor { get; set; }
        public string? Editorial { get; set; }
        public int Año { get; set; }
        public Uri? ImgPortada { get; set; }
        public string? IdiomaCulture { get; set; }
        public Version? Base { get; set; }
        public bool EsBase => ReferenceEquals(Base, default);
        public Capitulo[] Capitulos { get; set; }
    }
    public class Capitulo
    {
        public Capitulo() { }
        public Capitulo(Version version,int posicion)
        {
            Version = version;
            Posicion = posicion;
        }
        public Version? Version { get; set; }
        public bool Omitido { get; set; }
        public int Posicion { get; set; }
        public bool Acabado { get; set; }
        public Parrafo[]? Parrafos { get; set; }

        public IEnumerable<string>? GetParrafosOri() => Version?.Ebook?.GetContentElements(Posicion);

        public void Load()
        {
            IEnumerable<string>? parrafosOri = GetParrafosOri();
            if (!Equals(parrafosOri, default))
            {
                Parrafos = new Parrafo[parrafosOri.Count()];
                for (int i = 0; i < Parrafos.Length; i++)
                    Parrafos[i] = new Parrafo(this, i);
            }
            else throw new NullReferenceException(nameof(Version.Ebook));
        }
    }
    public class Parrafo
    {
        public Parrafo(Capitulo? capitulo = default, int posicion = -1)
        {
            Frases = new List<Frase>();
            Capitulo = capitulo;
            Posicion = posicion;
        }
        public bool Omitido { get; set; }
        public Capitulo? Capitulo { get; set; }
        public int Posicion { get; set; }
        public List<Frase> Frases { get; private set; }//pueden venir de otros parrafos

        public void Load()
        {
            string? parrafo;

            if (Posicion > 0)
            {
                parrafo = Capitulo?.GetParrafosOri()?.Skip(Posicion - 1).First();
            }
            else
            {
                parrafo = Capitulo?.GetParrafosOri()?.First();
            }
            if (Equals(parrafo, default))
                throw new NullReferenceException("No se ha podido cargar el parrafo");
            //ahora divido en frases el parrafo
        }
    }
    public class Frase
    {
        public Frase()
        {
        }

        public Frase(Parrafo parrafo, int posicion,string texto)
        {
            Parrafo = parrafo;
            Posicion = posicion;
            Texto = texto;
        }

        public Parrafo? Parrafo { get; set; }
        public int Posicion { get; set; }
        public string? Texto { get; set; }
    }
}

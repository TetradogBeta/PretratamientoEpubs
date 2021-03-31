using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace CommonEbookPretractament
{
    public class EbookStandaritzed :ISaveAndLoad, IElementoBinarioComplejo
    {
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<EbookStandaritzed>();

        [IgnoreSerialitzer]
        public static string Directory { get; set; }

        static EbookStandaritzed()
        {
            Directory = new DirectoryInfo("EbookStandaritzed").FullName;
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
        }

        /// <summary>
        /// Es el libro en el que se basa la edición de la versión, haciendo que no tenga que ser el original, y si se traza un puente se puede obtener como lo tenga puesto cualquier versión
        /// </summary>
        [IgnoreSerialitzer]
        public EbookSplited Reference { get; set; }
        [IgnoreSerialitzer]
        public EbookSplited Version { get; set; }

        public string ReferencePath { get; set; }
        public string VersionPath { get; set; }
        public Capitulo[] CapitulosEditados { get; set; }
        public string SavePath => System.IO.Path.Combine(EbookStandaritzed.Directory, $"{Version.OriginalTitle} [{Version.Idioma}].ebookStandaritzed");

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
        void ISaveAndLoad.Save()
        {
          
        }

        void ISaveAndLoad.Load()
        {
            Reference = EbookSplited.GetEbookSplited(System.IO.File.ReadAllBytes(System.IO.Path.Combine(EbookSplited.Directory, ReferencePath)));
            Version = EbookSplited.GetEbookSplited(System.IO.File.ReadAllBytes(System.IO.Path.Combine(EbookSplited.Directory, VersionPath)));
            
        }
        public byte[] GetBytes() => Serializador.GetBytes(this);

        public void Save()
        {
            GetBytes().Save(SavePath);
        }
        public bool Finished()
        {
            bool finished = Reference.TotalChapters == Version.TotalChapters;

            for(int i=0;i<Reference.TotalChapters && !finished; i++)
            {
                finished = !Equals(CapitulosEditados[i],default) && CapitulosEditados[i].Finished(Reference, Version, i);
            }

            return finished;
        }
        public static EbookStandaritzed[] GetEbookStandaritzeds()
        {
            FileInfo[] files = new DirectoryInfo(Directory).GetFiles();
            return files.Convert((f) => (EbookStandaritzed)Serializador.GetObject(f.GetBytes()));

        }
    }
    public class Capitulo : IElementoBinarioComplejo
    {
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<Capitulo>();
        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
        public List<Parrafo> ParrafosEditados { get; set; } = new List<Parrafo>();

        public bool Finished(EbookSplited original,EbookSplited version, int chapter)
        {
            string[] parrafosOriginal = original.GetContentElementsArray(chapter);
            string[] parrafosVersion = version.GetContentElementsArray(chapter);
            bool finished =parrafosOriginal.Length == parrafosVersion.Length;

            if (!finished)
            {
                //miro los parrafos editados y si ya cuadra todo pues finished=true
            }

            return finished;
        }
    }
    public class Parrafo : IElementoBinarioComplejo
    {
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<Parrafo>();
        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
        public int Chapter { get; set; }
        /// <summary>
        /// Si dos o más Parrafos tiene el mismo Index, se usará la Posición para determinar el orden
        /// </summary>
        public int Index { get; set; }
        public int IndexInicio { get; set; } = -1;
        public int IndexFin { get; set; } = -1;
        public bool Saltar { get; set; } = false;
        public int Posicion { get; set; } = 0;

        public int Inicio { get; set; } = -1;
        public int Fin { get; set; } = -1;

    }
    //quizás hará falta hacer otra clase para frase que mire el parrafo que empieza y en cual acaba...depende del tamaño del <p>
}

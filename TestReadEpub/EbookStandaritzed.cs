﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public EbookStandaritzed() { }
        public EbookStandaritzed(EbookSplited ebookSplitedVersion,EbookSplited ebookSplitedReference = default)
        {
            if (Equals(ebookSplitedVersion, default))
            {
                throw new NullReferenceException("se requiere un ebook splited!");
            }
            if (Equals(ebookSplitedReference, default))
            {
                ebookSplitedReference = ebookSplitedVersion;
            }
            Version = ebookSplitedVersion;
            Reference = new EbookStandaritzed() { Version = ebookSplitedReference };
            CapitulosEditados = new Capitulo[Version.TotalChapters];
        }

        /// <summary>
        /// Es el libro en el que se basa la edición de la versión, haciendo que no tenga que ser el original, y si se traza un puente se puede obtener como lo tenga puesto cualquier versión
        /// </summary>
        [IgnoreSerialitzer]
        public EbookStandaritzed Reference { get; set; }
        [IgnoreSerialitzer]
        public EbookSplited Version { get; set; }

        public string ReferencePath { get; set; }
        public string VersionPath { get; set; }
        public Capitulo[] CapitulosEditados { get; set; }
        public int TotalChapters => Version.TotalChapters;
        public string SavePath => System.IO.Path.Combine(EbookStandaritzed.Directory, $"{Version.OriginalTitle} [{Version.Idioma}].ebookStandaritzed");

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
        void ISaveAndLoad.Save()
        {
            if (!Equals(Reference, default))
            {
                if(!Equals(Reference.Version.EbookPath, Version.EbookPath))
                    ReferencePath = System.IO.Path.GetRelativePath(EbookSplited.Directory, Reference.SavePath);
            }
            else
            {
                ReferencePath = default;
            }
            VersionPath   =System.IO.Path.GetRelativePath(EbookSplited.Directory,  Version.SavePath);
            for (int i = 0; i < CapitulosEditados.Length; i++)
                if (!CapitulosEditados[i].IsRelevant)
                    CapitulosEditados[i] = default;
        }

        void ISaveAndLoad.Load()
        {

            Version   = EbookSplited.GetEbookSplited(System.IO.File.ReadAllBytes(System.IO.Path.Combine(EbookSplited.Directory, VersionPath)));
            if (!Equals(ReferencePath, default))
            {
                Reference = EbookStandaritzed.GetEbookStandaritzed(System.IO.File.ReadAllBytes(System.IO.Path.Combine(EbookStandaritzed.Directory, ReferencePath)));
            }
            else
            {
                Reference = new EbookStandaritzed() { Version = this.Version };
            }
  
        }
        public byte[] GetBytes() => Serializador.GetBytes(this);

        public void Save()
        {
            GetBytes().Save(SavePath);
        }
        public Capitulo GetCapitulo(int pos)
        {
            if (pos < 0 || pos >= CapitulosEditados.Length)
                throw new ArgumentOutOfRangeException();

            if (Equals(CapitulosEditados[pos], default))
                CapitulosEditados[pos] = new Capitulo();
            return CapitulosEditados[pos];
        }
        public bool Finished()
        {
            bool finished = Reference.TotalChapters == Version.TotalChapters;

            for(int i=0;i<Reference.TotalChapters && finished; i++)
            {
                finished = Equals(CapitulosEditados[i],default) || CapitulosEditados[i].Finished(Reference, Version, i);
            }

            return finished;
        }
        public override string ToString()
        {
            return $"Reference {Reference.Version}";
        }
        public static EbookStandaritzed GetEbookStandaritzed(byte[] data) => (EbookStandaritzed)Serializador.GetObject(data);
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

        public bool IsRelevant => ParrafosEditados.Any(p => p.IsRelevant);

        public bool Finished(EbookStandaritzed original,EbookSplited version, int chapter)
        {
            string[] parrafosOriginal = original.Version.GetContentElementsArray(chapter);
            string[] parrafosVersion = version.GetContentElementsArray(chapter);
            bool finished =parrafosOriginal.Length == parrafosVersion.Length;

            if (!finished)
            {
                //miro los parrafos editados y si ya cuadra todo pues finished=true
                //tener en cuenta que un parrafo puede estar por partes en la versión y al revés
            }

            return finished;
        }
        
    }
    public class Parrafo : IElementoBinarioComplejo
    {
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<Parrafo>();
        static readonly byte[] Empty = Serializador.GetBytes(new Parrafo());
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

        public bool IsRelevant =>! GetBytes().AreEquals(Empty);
        public byte[] GetBytes() => Serializador.GetBytes(this);



    }
    //quizás hará falta hacer otra clase para frase que mire el parrafo que empieza y en cual acaba...depende del tamaño del <p>
}

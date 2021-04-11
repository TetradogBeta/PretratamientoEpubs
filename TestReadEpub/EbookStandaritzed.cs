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
                if (!Equals(CapitulosEditados[i],default) && !CapitulosEditados[i].IsRelevant)
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
        public List<Spliter> ParrafosEditados { get; set; } = new List<Spliter>();

        public bool IsRelevant => ParrafosEditados.Any(p => p.IsRelevant);

        public bool Finished(EbookStandaritzed original,EbookSplited version, int chapter)
        {

            return GetParrafos(version, chapter).ToArray().Length == original.Version.GetContentElementsArray(chapter).Length;
           
        }
        public IEnumerable<string> GetParrafos(EbookSplited version, int chapter)
        {
            IEnumerable<string> parrafosVer = version.GetContentElementsArray(chapter);

            if (ParrafosEditados.Count > 0)
            {
                ParrafosEditados.Sort();

                return Spliter.GetParts(ParrafosEditados,parrafosVer);
            }
            else
            {
                return parrafosVer;
            }
        }
        
        
    }
    public class Spliter : IElementoBinarioComplejo,ISaveAndLoad,IComparable,IComparable<Spliter>
    {
        const int DEFAULT = -1;
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<Spliter>();
        static readonly byte[] Empty = Serializador.GetBytes(new Spliter());
        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        /// <summary>
        /// Si dos o más Parrafos tiene el mismo Index, se usará la Posición para determinar el orden
        /// </summary>
        public int IndexReference { get; set; } = DEFAULT;
        public int IndexInicio { get; set; } = DEFAULT;
        /// <summary>
        /// Si es -1 se entiende que es el mismo que IndexInicio
        /// </summary>
        public int IndexFin { get; set; } = DEFAULT;
        public bool Saltar { get; set; } = false;
        public int Posicion { get; set; } = 0;

        public int CharInicio { get; set; } = DEFAULT;
        /// <summary>
        /// Si es -1 se entiende que es hasta el final
        /// </summary>
        public int CharFin { get; set; } = DEFAULT;

        public bool AcabaEnElMismoIndex => IndexFin == DEFAULT || IndexInicio == IndexFin;

        public bool IsRelevant =>! GetBytes().AreEquals(Empty);
        public byte[] GetBytes() => Serializador.GetBytes(this);

        void ISaveAndLoad.Save()
        {
            if (IndexInicio == IndexFin)
                IndexFin = DEFAULT;
            if (CharInicio == 0)
                CharInicio = DEFAULT;

        }

        void ISaveAndLoad.Load()
        {
            if (IndexFin == DEFAULT)
                IndexFin = IndexInicio;
            if (CharInicio == DEFAULT)
                CharInicio = 0;
        }

        int IComparable.CompareTo(object obj)
        {
            return IComparteTo(obj as Spliter);
        }

        int IComparable<Spliter>.CompareTo(Spliter other)
        {
            return IComparteTo(other);
        }
        int IComparteTo(Spliter other)
        {//se tiene que ajustar
            int compareTo=Saltar?other.Saltar?IndexInicio.CompareTo(other.IndexInicio):-1:1;

            if (compareTo == 0)
            {
                compareTo = IndexReference.CompareTo(other.IndexReference) * -1;
                if (compareTo == 0)
                {
                    if (CharInicio == -1)
                        compareTo = 1;
                    else if (other.CharInicio == -1)
                        compareTo = -1;
                    else
                        compareTo = CharInicio.CompareTo(other.CharInicio) *-1;//asi los ordeno de mas pequeño a mayor
                    if (compareTo == 0)
                    {
                        if (CharFin == -1)
                            compareTo = -1;
                        else if (other.CharFin == -1)
                            compareTo = 1;
                        else
                            compareTo = CharFin.CompareTo(other.CharFin) * -1;//asi los ordeno de mas pequeño a mayor
                        if (compareTo == 0)
                        {
                            compareTo = Posicion.CompareTo(other.Posicion) * -1;//asi los ordeno de mas pequeño a mayor

                        }

                    }
                }
            }
            return compareTo;
        }

 
        public static IEnumerable<string> GetParts(List<Spliter> parts, IEnumerable<string> textosATratar,string strJoin="")
        {//que transformaciones se hacen con los Parrafos editados para convertir la version en el original
         //asi se puede usar para dividir frases

            int strInicio;
            int strFin;
            StringBuilder strActual = new StringBuilder();
            string[] strsVer = textosATratar.ToArray();
            int posActual = 0;

           // parts.Sort();//me da problemas al momento de ordenarlo...
            //los parrafos que son identicos antes de encontrar uno editado
            for (int i = 0; i < parts[0].IndexInicio; i++)
                yield return strsVer[posActual++];
            //mix parrafos saltados,splited,joined,enteros
            for (int i = 0; i < parts.Count; i++)
            {
                if (posActual == parts[i].IndexInicio)
                {
                    if (parts[i].Saltar)
                    {//salto
                        posActual++;
                    }
                    else if (parts[i].AcabaEnElMismoIndex)
                    {//split
                        if (parts[i].CharInicio == -1)
                            parts[i].CharInicio = 0;
                        strInicio = parts[i].CharInicio;
                        if (parts[i].CharFin == -1)
                        {
                            strFin = strsVer[posActual].Length;
                        }
                        else
                        {
                            strFin = parts[i].CharFin;
                        }

                        yield return strsVer[posActual].Substring(strInicio, strFin - strInicio);


                        if (parts[i].CharFin == -1)
                        {
                            posActual++;
                        }
                    }
                    else
                    {//join
                        strActual.Clear();
                        //inicio
                        if (parts[i].CharInicio == -1)
                            parts[i].CharInicio = 0;
                        strInicio = parts[i].CharInicio;
                        strActual.Append(strsVer[posActual++].Substring(strInicio));
                        //medio
                        for (int j = 0,f= parts[i].IndexFin - parts[i].IndexInicio -1; j < f; j++)
                        {
                            strActual.Append(strJoin);
                            strActual.Append(strsVer[posActual++]);
                        }

                        //fin
                        if (parts[i].CharFin == -1)
                        {
                            strFin = strsVer[posActual].Length;
                        }
                        else
                        {
                            strFin = parts[i].CharFin;
                        }
                        if (strActual.ToString().Length > 0)
                            strActual.Append(strJoin);
                        strActual.Append(strsVer[posActual].Substring(0, strFin));
                        if (parts[i].CharFin == -1)
                        {
                            posActual++;
                        }
                        yield return strActual.ToString();

                    }
                }
                else
                {//entero
                    i--;//se que no es muy correcto pero es una solución...
                    yield return strsVer[posActual++];
            
                }
            }

            //los parrafos que son identicos después de encontrar el último editado
            for (int i = posActual; i < strsVer.Length; i++)
                yield return strsVer[i];

        }
    }
    //quizás hará falta hacer otra clase para frase que mire el parrafo que empieza y en cual acaba...depende del tamaño del <p>
}

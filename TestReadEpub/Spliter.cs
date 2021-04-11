using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace CommonEbookPretractament
{
    public class Spliter : IElementoBinarioComplejo,ISaveAndLoad,IComparable,IComparable<Spliter>
    {
        const int DEFAULT = -1;
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<Spliter>();
        static readonly byte[] Empty = Serializador.GetBytes(new Spliter());
        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        public int IndexInicio { get; set; } = DEFAULT;
        /// <summary>
        /// Si es -1 se entiende que es el mismo que IndexInicio
        /// </summary>
        public int IndexFin { get; set; } = DEFAULT;
        public bool Saltar { get; set; } = false;

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
            int compareTo = !Equals(other, default) ? 0 : 1;
            if (compareTo == 0)
            {
                compareTo= Saltar ? other.Saltar ? IndexInicio.CompareTo(other.IndexInicio) : 1 : -1;

                if (compareTo == 0)
                {
                    if (CharInicio == -1)
                        compareTo = -1;
                    else if (other.CharInicio == -1)
                        compareTo = 1;
                    else
                        compareTo = CharInicio.CompareTo(other.CharInicio);
                    if (compareTo == 0)
                    {
                        if (CharFin == -1)
                            compareTo = 1;
                        else if (other.CharFin == -1)
                            compareTo = -1;
                        else
                            compareTo = CharFin.CompareTo(other.CharFin);


                    }
                }
            }
            
            return compareTo*-1;//asi los ordeno de mas pequeño a mayor
        }

        public static IEnumerable<string> GetParts(List<Spliter> parts, IEnumerable<string> textosATratar, string strJoin = "")
        {
            return GetParts(parts, textosATratar.ToList(), strJoin);
        }
        public static IEnumerable<string> GetParts(List<Spliter> parts, IList<string> strsVer, string strJoin="")
        {//que transformaciones se hacen con los Parrafos editados para convertir la version en el original
         //asi se puede usar para dividir frases

            int strInicio;
            int strFin;
            StringBuilder strActual = new StringBuilder();
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
                        if(parts[i].CharFin == -1 || parts[i].CharFin == strsVer[posActual].Length)
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
            for (int i = posActual; i < strsVer.Count; i++)
                yield return strsVer[i];

        }
    }
    //quizás hará falta hacer otra clase para frase que mire el parrafo que empieza y en cual acaba...depende del tamaño del <p>
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace CommonEbookPretractament
{
    public class Spliter : IElementoBinarioComplejo, IComparable, IComparable<Spliter>
    {
        const int DEFAULT = -1;
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<Spliter>();
        static readonly byte[] Empty = Serializador.GetBytes(new Spliter());
        static readonly byte[] Invalid = Serializador.GetBytes(new Spliter() { Saltar = true });
        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
        public int IndexInicio => EditIndexInicio - 1;
        public int EditIndexInicio { get; set; } = 1;
        public int IndexFin => EditIndexFin - 1;
        /// <summary>
        /// Si es -1 se entiende que es el mismo que IndexInicio
        /// </summary>
        public int EditIndexFin { get; set; } = DEFAULT;
        public bool Saltar { get; set; } = false;

        public int CharInicio { get; set; } = 0;
        /// <summary>
        /// Si es -1 se entiende que es hasta el final
        /// </summary>
        public int CharFin { get; set; } = DEFAULT;

        public bool AcabaEnElMismoIndex => EditIndexFin == DEFAULT || IndexInicio == IndexFin;

        public bool IsRelevant => IsValid && !GetBytes().AreEquals(Empty);
        public bool IsValid => !GetBytes().AreEquals(Invalid);
        public byte[] GetBytes() => Serializador.GetBytes(this);



        int IComparable.CompareTo(object obj)
        {
            return ICompareTo(obj as Spliter);
        }

        int IComparable<Spliter>.CompareTo(Spliter other)
        {
            return ICompareTo(other);
        }
        int ICompareTo(Spliter other)
        {//se tiene que ajustar
            int compareTo = !Equals(other, default) ? 0 : 1;
            if (compareTo == 0)
            {
                compareTo = Saltar.CompareTo(other.Saltar) * -1;
                if (compareTo == 0)
                {
                    compareTo = IndexInicio.CompareTo(other.IndexInicio);
                    if (compareTo == 0)
                    {
                        compareTo = IndexFin.CompareTo(other.IndexFin);
                        if (compareTo == 0)
                        {
                            compareTo = CharInicio.CompareTo(other.CharInicio);
                            if (compareTo == 0)
                            {
                                compareTo = CharFin.CompareTo(other.CharFin);
                            }
                        }
                    }
                }

            }
            return compareTo;//asi los ordeno de mas pequeño a mayor
        }
        public override bool Equals(object obj)
        {
            return ICompareTo(obj as Spliter) == 0;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"{(Saltar ? "#" : string.Empty)} PI:{EditIndexInicio},PF:{EditIndexFin},CI:{CharInicio},CF:{CharFin}";
        }

        public static IEnumerable<string> GetParts(List<Spliter> parts, IEnumerable<string> textosATratar, string strJoin = "")
        {
            return GetParts(parts, textosATratar.ToList(), strJoin);
        }
        public static IEnumerable<string> GetParts(List<Spliter> parts, IList<string> strsVer, string strJoin = "")
        {//que transformaciones se hacen con los Parrafos editados para convertir la version en el original
         //asi se puede usar para dividir frases

            int strInicio;
            int strFin;
            StringBuilder strActual = new StringBuilder();
            int posActual = 0;

            parts.Sort();//mirar si ordena bien
            //los parrafos que son identicos antes de encontrar uno editado
            for (int i = 0; i < parts[0].IndexInicio; i++)
                yield return strsVer[posActual++];
            //mix parrafos saltados,splited,joined,enteros
            for (int i = 0, fCount = strsVer.Count - 1; i < parts.Count && posActual < strsVer.Count; i++)
            {

                if (parts[i].IndexFin >= fCount)
                    parts[i].EditIndexFin = strsVer.Count;

                if (parts[i].IndexInicio < 0)
                    parts[i].EditIndexInicio = 1;
                else if (parts[i].IndexInicio >= fCount)
                    parts[i].EditIndexInicio = strsVer.Count;

                if (parts[i].IsRelevant)
                {
                    if (posActual == parts[i].IndexInicio)
                    {

                        if (parts[i].Saltar)
                        {//salto
                            if (parts[i].CharFin == -1 || parts[i].CharFin == strsVer[posActual].Length)
                                posActual++;
                        }
                        else if (parts[i].AcabaEnElMismoIndex)
                        {//split
                            if (parts[i].CharInicio <= -1)
                                parts[i].CharInicio = 0;
                            if (parts[i].CharInicio >= strsVer[posActual].Length - 1)
                                parts[i].CharInicio = strsVer[posActual].Length - 1;

                            strInicio = parts[i].CharInicio;
                            if (parts[i].CharFin <= parts[i].CharInicio || parts[i].CharFin <= -1 || parts[i].CharFin >= strsVer[posActual].Length)
                            {
                                strFin = strsVer[posActual].Length;
                                if (parts[i].CharFin > parts[i].CharInicio)
                                    parts[i].CharFin = -1;
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
                            if (parts[i].CharInicio <= -1)
                                parts[i].CharInicio = 0;
                            strInicio = parts[i].CharInicio;
                            strActual.Append(strsVer[posActual++].Substring(strInicio));

                            //medio
                            for (int j = 0, f = parts[i].IndexFin - parts[i].IndexInicio - 1; j < f && posActual < fCount; j++)
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
            }

            //los parrafos que son identicos después de encontrar el último editado
            for (int i = posActual; i < strsVer.Count; i++)
                yield return strsVer[i];

        }
    }
    //quizás hará falta hacer otra clase para frase que mire el parrafo que empieza y en cual acaba...depende del tamaño del <p>
}

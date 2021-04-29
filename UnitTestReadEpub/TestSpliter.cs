using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CommonEbookPretractament;
using System.Linq;
using Gabriel.Cat.S.Extension;

namespace UnitTestReadEpub
{
    [TestClass]
    public class TestSpliter

    {
        [TestMethod]
        public void TestJoin()
        {
            string[] textos =
            {
                "Texto Frase 1",
                "Texto Frase 2",
                "Texto Frase 3"
            };
            string[] resultadoEsperado =
            {
               string.Join("", textos[0],textos[1]),
               textos[2]
            };
            List<Spliter> spliters =
            new List<Spliter>(){

                new Spliter()
                {
                    EditIndexInicio=0+1,
                    EditIndexFin=1+1
                }

            };
            string[] resultado = Spliter.GetParts(spliters, textos).ToArray();
            Assert.IsTrue(resultadoEsperado.AreEquals(resultado));
        }
        [TestMethod]
        public void TestSplit()
        {
            string[] resultadoEsperado =
            {
                "Texto1",
                "Texto2",
                "Texto3"
            };
            string[] textos =
            {
               string.Join("", resultadoEsperado[0],resultadoEsperado[1]),
               resultadoEsperado[2]
            };
            List<Spliter> spliters =
            new List<Spliter>(){

                new Spliter()
                {
                    EditIndexInicio=0+1,
                    CharFin=resultadoEsperado[0].Length,

                },
                  new Spliter()
                {
                    EditIndexInicio=0+1,
                    CharInicio=resultadoEsperado[0].Length,

                }

            };
            string[] resultado = Spliter.GetParts(spliters, textos).ToArray();

            Assert.IsTrue(resultadoEsperado.AreEquals(resultado));

        }
        [TestMethod]
        public void TestSplitAndJoin()
        {
            string[] resultadoEsperado =
            {
                "Texto1",
                "Texto22",
                "Texto3"
            };
            string[] textos =
            {
               string.Join("", resultadoEsperado[0],resultadoEsperado[1].Substring(0,resultadoEsperado[1].Length-1)),
              string.Join("",resultadoEsperado[1][resultadoEsperado[1].Length-1]+"", resultadoEsperado[2])
            };
            List<Spliter> spliters =
            new List<Spliter>(){

                new Spliter()
                {
                    EditIndexInicio=0+1,
                    CharFin=resultadoEsperado[0].Length,

                },
                  new Spliter()
                {
                    EditIndexInicio=0+1,
                    EditIndexFin=1+1,
                    CharInicio=resultadoEsperado[0].Length,
                    CharFin=1

                },
                  
                  new Spliter()
                {
                    EditIndexInicio=1+1,
                    CharInicio=1

                }

            };
            string[] resultado = Spliter.GetParts(spliters, textos).ToArray();

            Assert.IsTrue(resultadoEsperado.AreEquals(resultado));

        }
        [TestMethod]
        public void TestSplitAndJoinWithCustomJoiner()
        {
            const string JOINER = ".";
            string[] resultadoEsperado =
            {
                "Texto1",
                "Texto2"+JOINER+"2",
                "Texto3",
                "Texto4",
                "Texto5"
            };
            string[] textos =
            {
                resultadoEsperado[0]+resultadoEsperado[1].Substring(0,resultadoEsperado[1].Length-(JOINER.Length+1)),
                resultadoEsperado[1][resultadoEsperado[1].Length-1]+ resultadoEsperado[2]+resultadoEsperado[3],
                resultadoEsperado[4]
            
            };
            List<Spliter> spliters =
            new List<Spliter>(){

                new Spliter()
                {
                    EditIndexInicio=0+1,
                    CharFin=resultadoEsperado[0].Length,

                },
                  new Spliter()
                {
                    EditIndexInicio=0+1,
                    EditIndexFin=1+1,
                    CharInicio=resultadoEsperado[0].Length,
                    CharFin=1

                },

                  new Spliter()
                {
                    EditIndexInicio=1+1,
                    CharInicio=1,
                    CharFin=1+resultadoEsperado[2].Length

                },

                  new Spliter()
                {
                    EditIndexInicio=1+1,
                    CharInicio=1+resultadoEsperado[2].Length

                }
            };
            string[] resultado = Spliter.GetParts(spliters, textos, JOINER).ToArray();

            Assert.IsTrue(resultadoEsperado.AreEquals(resultado));

        }
       
        [TestMethod]
        public void TestMix()
        {
            //falta saltando
            //strings del medio(entre strings editadas hay otras) que se tienen que poner enteras
            //split,entera,salto,join,resto
            string[] textos =
            {
                "Texto0",
                "Texto1Texto2",
                "Texto3",
                "Texto4Saltado",
                "Texto5P1",
                "Texto5P2Texto6TextoOmitido",
                "Texto7",
                "Texto8"
            };
            string[] resultadoEsperado ={
                textos[0],
                textos[1].Substring(0,6),
                textos[1].Substring(6),
                textos[2],
                //salto textos[3],
                textos[4]+textos[5].Substring(0,8),
                textos[5].Substring(8,6),
                textos[6],
                textos[7]
            };
            List<Spliter> spliters = new List<Spliter>
            {
                new Spliter()
                {
                    EditIndexInicio=1+1,
                    CharFin=6
                },
                new Spliter()
                {
                    EditIndexInicio=1+1,
                    CharInicio=6
                },
                new Spliter()
                {
                    EditIndexInicio=3+1,
                    Saltar=true
                },
                new Spliter()
                {
                    EditIndexInicio=4+1,
                    EditIndexFin=5+1,
                    CharFin=8
                },
                new Spliter()
                {
                    EditIndexInicio=5+1,
                    CharInicio=8,
                    CharFin=8+6
                },
                new Spliter()
                {
                    EditIndexInicio=5+1,
                    Saltar=true
                }
            };
            string[] resultado = Spliter.GetParts(spliters, textos).ToArray();
            Assert.IsTrue(resultadoEsperado.AreEquals(resultado));
        }
    }
}

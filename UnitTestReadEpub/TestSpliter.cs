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
                    IndexInicio=0,
                    IndexFin=1
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
                    IndexInicio=0,
                    CharFin=resultadoEsperado[0].Length,

                },
                  new Spliter()
                {
                    IndexInicio=0,
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
                    IndexInicio=0,
                    CharFin=resultadoEsperado[0].Length,

                },
                  new Spliter()
                {
                    IndexInicio=0,
                    IndexFin=1,
                    CharInicio=resultadoEsperado[0].Length,
                    CharFin=1

                },
                  
                  new Spliter()
                {
                    IndexInicio=1,
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
                    IndexInicio=0,
                    CharFin=resultadoEsperado[0].Length,

                },
                  new Spliter()
                {
                    IndexInicio=0,
                    IndexFin=1,
                    CharInicio=resultadoEsperado[0].Length,
                    CharFin=1

                },

                  new Spliter()
                {
                    IndexInicio=1,
                    CharInicio=1,
                    CharFin=1+resultadoEsperado[2].Length

                },

                  new Spliter()
                {
                    IndexInicio=1,
                    CharInicio=1+resultadoEsperado[2].Length

                }
            };
            string[] resultado = Spliter.GetParts(spliters, textos, JOINER).ToArray();

            Assert.IsTrue(resultadoEsperado.AreEquals(resultado));

        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace CommonEbookPretractament
{
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
                return Spliter.GetParts(ParrafosEditados,parrafosVer);
            }
            else
            {
                return parrafosVer;
            }
        }
        
        
    }
    //quizás hará falta hacer otra clase para frase que mire el parrafo que empieza y en cual acaba...depende del tamaño del <p>
}

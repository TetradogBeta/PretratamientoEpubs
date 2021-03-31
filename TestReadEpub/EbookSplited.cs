using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace CommonEbookPretractament
{
    public class EbookSplited:ISaveAndLoad,IElementoBinarioComplejo
    {
        
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<EbookSplited>();

        [IgnoreSerialitzer]
        public static string Directory { get; set; } 
        static EbookSplited()
        {
            Directory = new DirectoryInfo("EbookSpliteds").FullName;
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
        }

        int totalChapters;
        public EbookSplited() { }
        public EbookSplited(string relativePath)
        {
            RelativeEbookPath = relativePath;
            Ebook = new Ebook(EbookPath);
            OriginalTitle = Ebook.Epub.Title;
            CapitulosAOmitir = new bool[Ebook.TotalChapters];
            Idioma = "NO DEFINIDO";
            UpdateTotalChapters();
        }
        public EbookSplited(FileInfo epubFile) : this(System.IO.Path.GetRelativePath(Ebook.Directory, epubFile.FullName)) { }

        public string RelativeEbookPath { get; set; }
        public string EbookPath => System.IO.Path.Combine(Ebook.Directory, RelativeEbookPath);
        public string OriginalTitle { get; set; }
        public string Idioma { get; set; }

        public bool[] CapitulosAOmitir { get; set; }

        public string SavePath => System.IO.Path.Combine(Directory, $"{OriginalTitle}  [{ Idioma}].ebookSplited");


        [IgnoreSerialitzer]
        public Ebook Ebook { get; set; }

        public int TotalChapters => totalChapters;

        #region Serializar

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        void ISaveAndLoad.Load()
        {
            Ebook = new Ebook(EbookPath);
            UpdateTotalChapters();
        }

        void ISaveAndLoad.Save()
        {
          
        }
        public byte[] GetBytes() => Serializador.GetBytes(this);
        public void Save()
        {
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
            GetBytes().Save(SavePath);
        }
        #endregion
        public void SetCapitulosAOmitir(params int[] capitulosAOmitir)
        {
            //pongo los capitulos a omitir
            for (int i = 0; i < capitulosAOmitir.Length; i++)
                CapitulosAOmitir[capitulosAOmitir[i]] = true;

            UpdateTotalChapters();
        }

        public void UpdateTotalChapters()
        {
            totalChapters = CapitulosAOmitir.Where((chapter) => !chapter).ToArray().Length;

        }
        public IEnumerable<string> GetContentElements(int chapterSplited, string element = "p")
        {
            return Ebook.GetContentElements(GetRealChapter(chapterSplited), element);
                
        }

        private int GetRealChapter(int chapterSplited)
        {
            int realChapter =0;

            for(int i = 0; i <= chapterSplited; i++)
            {
                while (CapitulosAOmitir[realChapter++]) ;
         
            }

            return realChapter-1;
        }

        public string[] GetContentElementsArray(int chapterSplited, string element = "p") => GetContentElements(chapterSplited, element).ToArray();

        public override string ToString()
        {
            return $"[{Idioma}] {RelativeEbookPath}";
        }

        public static EbookSplited GetEbookSplited(byte[] bytesFile) => (EbookSplited)Serializador.GetObject(bytesFile);
        public static EbookSplited[] GetEbookSplitedNewer(string folder)
        {
            FileInfo[] files = new DirectoryInfo(System.IO.Path.Combine(Ebook.Directory,folder)).GetFiles();
            return files.Convert((e) => new EbookSplited(e));
        }
        public static EbookSplited[] GetEbookSpliteds()
        {
            FileInfo[] files = new DirectoryInfo(EbookSplited.Directory).GetFiles();
            return files.Convert((e) => GetEbookSplited(e.GetBytes()));
        }
    }
}

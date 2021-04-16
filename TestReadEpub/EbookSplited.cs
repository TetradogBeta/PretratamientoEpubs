using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;
using Notifications.Wpf.Core;

namespace CommonEbookPretractament
{
    public class EbookSplited:ISaveAndLoad,IElementoBinarioComplejo,IComparable<EbookSplited>,IComparable
    {
        
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<EbookSplited>();
        const string INDEFINIDO = "NO DEFINIDO";

        static List<Task> ToDo = new List<Task>();
        [IgnoreSerialitzer]
        public static string Directory { get; set; }
        [IgnoreSerialitzer]
        public static string DirectoryInCompatible { get; set; }
        static EbookSplited()
        {
            Directory = new DirectoryInfo("EbookSpliteds").FullName;
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);

            DirectoryInCompatible = new DirectoryInfo("Ebooks Incompatibles").FullName;
            if (!System.IO.Directory.Exists(DirectoryInCompatible))
                System.IO.Directory.CreateDirectory(DirectoryInCompatible);
        }

        int totalChapters;
        public EbookSplited() { }
        public EbookSplited(string relativePath)
        {
            RelativeEbookPath = relativePath;
            Ebook = new Ebook(EbookPath);
            OriginalTitle = Ebook.Epub.Title;
            CapitulosAOmitir = new bool[Ebook.TotalChapters];
            Idioma = INDEFINIDO;
            UpdateTotalChapters();
        }
        public EbookSplited(FileInfo epubFile) : this(System.IO.Path.GetRelativePath(Ebook.Directory, epubFile.FullName)) { }

        public string RelativeEbookPath { get; set; }
        public string EbookPath => System.IO.Path.Combine(Ebook.Directory, RelativeEbookPath);
        public string OriginalTitle { get; set; }
        public string Idioma { get; set; }

        public bool[] CapitulosAOmitir { get; set; }

        public string SaveName => $"{OriginalTitle}  [{ Idioma}]";

        public string SavePath => System.IO.Path.Combine(Directory, $"{SaveName}.ebookSplited");


        [IgnoreSerialitzer]
        public Ebook Ebook { get; set; }

        public int TotalChapters => totalChapters;

        #region Serializar

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        public bool IsRelevant => CapitulosAOmitir.Any((s) => s);

        public bool IsOkey => !Equals(OriginalTitle, default) && !Equals(Idioma, INDEFINIDO);

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
            return $"[{Idioma}] {OriginalTitle}";
        }
        int IComparable<EbookSplited>.CompareTo(EbookSplited other)
        {
            return ICompareTo(other);
        }

        int IComparable.CompareTo(object obj)
        {
            return ICompareTo(obj as EbookSplited);
        }
        int ICompareTo(EbookSplited other)
        {
            int compareTo = !Equals(other, default) ? 0 : -1;
            if (compareTo == 0)
            {
                compareTo = SaveName.CompareTo(other.SaveName);
            }
            return compareTo;
        }
        public override bool Equals(object obj)
        {
            return ICompareTo(obj as EbookSplited) == 0;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static EbookSplited GetEbookSplited(byte[] bytesFile) => (EbookSplited)Serializador.GetObject(bytesFile);
        public static async Task<EbookSplited[]> GetEbookSplitedNewer(string folder)
        {
            string pathDirEbook;
            NotificationManager notificationManager;
            FileInfo[] files = new DirectoryInfo(System.IO.Path.Combine(Ebook.Directory,folder)).GetFiles();
            List<EbookSplited> ebooks = new List<EbookSplited>();
            for(int i=0;i<files.Length;i++)
                try
                {
                    ebooks.Add(new EbookSplited(files[i]));
                }
                catch {
                    pathDirEbook = System.IO.Path.Combine(DirectoryInCompatible, files[i].Directory.Name);
                    if (!System.IO.Directory.Exists(pathDirEbook))
                        System.IO.Directory.CreateDirectory(pathDirEbook);
                    files[i].MoveTo(System.IO.Path.Combine(pathDirEbook, files[i].Name));
                    //notifico el cambio
                    notificationManager = new NotificationManager();

                    ToDo.Add(notificationManager.ShowAsync(new NotificationContent
                    {
                        Title = "Libro Incompatible Encontrado!",
                        Message = files[i].FullName,
                        Type = NotificationType.Information
                    }));

                }
            return ebooks.ToArray();
        }

        public static EbookSplited[] GetEbookSpliteds()
        {
            FileInfo[] files = new DirectoryInfo(Directory).GetFiles();
            List<EbookSplited> ebooks = new List<EbookSplited>();
 
            for(int i=0;i<files.Length;i++)
            {
                try
                {
                    ebooks.Add(GetEbookSplited(files[i].GetBytes()));
                }
                catch {

                }
            }
            return ebooks.ToArray();
        }


    }
}

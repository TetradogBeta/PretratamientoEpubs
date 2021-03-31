using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VersOne.Epub;

namespace CommonEbookPretractament
{
    public class Ebook
    {
        static AngleSharp.Html.Parser.HtmlParser Parser = new AngleSharp.Html.Parser.HtmlParser();
        public static string Directory { get; set; }
        static Ebook()
        {
            Directory = new DirectoryInfo("Ebooks").FullName;
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
        }

        public Ebook() { }
        public Ebook(string filePath)
        {

            Epub = EpubReader.ReadBook(filePath);

            
        } 

        public EpubBook Epub { get; private set; }
        public EpubTextContentFile this[int chapter] => Epub.ReadingOrder[chapter];

        public int TotalChapters => Epub.ReadingOrder.Count;
  
        public IEnumerable<string> GetContentElements(int chapter, string element = "p")
        {
            foreach(IHtmlElement elementPage in Parser.ParseDocument(this[chapter].Content).GetElementsByTagName(element))
            {
               yield return elementPage.TextContent;
            }
    
        }
        public string[] GetContentElementsArray(int chapter, string element = "p") => GetContentElements(chapter, element).ToArray();
    }
}

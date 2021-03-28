using System;
using System.IO;
using VersOne.Epub;
using System.Collections.Generic;


namespace TestReadEpub
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] data;
            EbookSplited desSerializado;
            EbookSplited ebook;

            FileInfo file;
            DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);

            Ebook.Directory = new DirectoryInfo(System.IO.Path.Combine(dir.Parent.Parent.Parent.Parent.FullName,@"BooksSplitedGUI\bin\Debug\netcoreapp3.1\Ebooks")).FullName;
            file= new DirectoryInfo(Ebook.Directory).GetDirectories()[0].GetFiles()[5];


     
            
            ebook= new EbookSplited(System.IO.Path.GetRelativePath(EbookSplited.Directory, file.FullName));
        

            //omito estos capitulos
            ebook.SetCapitulosAOmitir(0,1, 2, 3, 4, 5, 6, 24, 25, 26, 27);

            //pruebo serialización
            data =ebook.GetBytes();
            desSerializado =EbookSplited.GetEbookSplited(data);

            for(int i = 0; i < desSerializado.TotalChapters; i++)
            {
                Console.WriteLine("{2} - Page {0}/{1}", i, desSerializado.TotalChapters, file.Name);
                Console.WriteLine(string.Join('\n', desSerializado.GetContentElements(i)));
                Console.ReadLine();
                Console.Clear();

            }
       


        }
    }
}

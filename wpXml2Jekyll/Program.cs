using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using wpXml2Jekyll.Properties;

namespace wpXml2Jekyll
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            if (args.Length == 0)
            {
                args = GetDebugArguments();
            }
#endif
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                FreeConsole();
                Application.Run(new UIForm());
            }
            else
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: wpXml2Jekyll [wordpress export file] [output folder]");
                    Environment.Exit(1);
                }
                var wordpressXmlFile = args[0];
                var outputFolder = args[1];

                var posts = new PostImporter().ReadWpPosts(wordpressXmlFile);
                int count = new PostWriter().WritePostToMarkdown(posts, outputFolder);
                Console.WriteLine("Saved " + count + " posts");
            }
        }

        private static string[] GetDebugArguments()
        {
            var repositoryRoot = Path.GetFullPath(@"..\..\..\..\..\");
            var xmlFile = Path.Combine(repositoryRoot, @"site\theofficialwebsiteofnicolemurphy.wordpress.xml");
            var outputDirectory = new DirectoryInfo(Path.Combine(repositoryRoot, @"artifacts\jekyll\posts"));

            if (!File.Exists(xmlFile))
            {
                throw new FileNotFoundException($"Cannot find xmlFile '{xmlFile}'.");
            }

            if (outputDirectory.Exists)
            {
                Console.WriteLine($"Deleting output directory '{outputDirectory.FullName}'.");
                outputDirectory.Delete(true);
            }
            Console.WriteLine($"Creating output directory '{outputDirectory.FullName}'.");
            outputDirectory.Create();

            return new[] {xmlFile, outputDirectory.FullName};
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();
    }
}

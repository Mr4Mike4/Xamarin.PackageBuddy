using System;
using System.Linq;
using System.Xml;
using System.IO;

namespace PackageBuddy.Runner
{
    class Program
    {
        private const string projectPathToken = "-projectPath=";
        private const string packageNameToken = "-packageName=";

        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                }
                else
                {
                    string projectPath = args.FirstOrDefault(a => a.StartsWith(projectPathToken, StringComparison.OrdinalIgnoreCase)).Substring(projectPathToken.Length);
                    string packageName = args.FirstOrDefault(a => a.StartsWith(packageNameToken, StringComparison.OrdinalIgnoreCase)).Substring(packageNameToken.Length);

                    Program.LoadManifestAndUpdatePackage(projectPath, packageName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
                   
        }

        private static void LoadManifestAndUpdatePackage(string projectPath, string newPackageName)
        {
            var fullProjectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, projectPath));

            Uri projectFile = new Uri(fullProjectPath);

            // get path without file name
            string projectFilename = projectFile.Segments.Last();
            string projectDirectory = fullProjectPath.Replace(projectFilename, string.Empty);

            string manifestPath = string.Format("{0}Properties/AndroidManifest.xml", projectDirectory);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Loading {0}", manifestPath);
            var xmldoc = new XmlDocument();
            xmldoc.Load(manifestPath);
            Console.WriteLine("Loaded {0}", manifestPath);

            Console.WriteLine("Loading manifest node...");
            var manifestNode = xmldoc.SelectSingleNode("/manifest");
            Console.WriteLine("Getting manifest attributes...");
            var attributes = manifestNode.Attributes;

            var package = attributes.GetNamedItem("package");

            string currentPackageName = package.Value;

            if (string.IsNullOrWhiteSpace(currentPackageName) == false && currentPackageName != newPackageName)
            {
                Console.WriteLine("Current package name: " + currentPackageName);

                string xmlString = xmldoc.OuterXml;
                Console.WriteLine("Replacing all occurrences of package name \"" + currentPackageName + "\"");
                xmlString = xmlString.Replace(currentPackageName, newPackageName);

                Console.WriteLine("Saving changes to manifest file");
                xmldoc.LoadXml(xmlString);
                xmldoc.Save(manifestPath);

                Console.WriteLine("Manifest file updated successfully");
            }
            else
            {
                Console.WriteLine("Don't need to change the package name. It is already " + newPackageName);
            }
        }
    }
}
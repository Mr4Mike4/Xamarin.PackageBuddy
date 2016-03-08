﻿using System;
using System.Linq;
using System.Xml;
using System.IO;

namespace PackageBuddy.Runner
{
    class Program
    {
        private const string platformToken = "-platform=";
        private const string projectPathToken = "-projectPath=";
        private const string packageNameToken = "-packageName=";

        private const string androidPlatform = "android";
        private const string iosPlatform = "ios";

        private const string bundleIdentifierXPath = "//key[text() ='CFBundleIdentifier']/following-sibling::*[1]";

        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                }
                else
                {
                    string platform = args.FirstOrDefault(a => a.StartsWith(platformToken, StringComparison.OrdinalIgnoreCase)).Substring(platformToken.Length);

                    if (string.IsNullOrWhiteSpace(platform))
                    {
                        throw new ArgumentNullException("platform must be specified in argument list. Allowed options are: \"android\", \"ios\"");
                    }
                    else if (platform != androidPlatform && platform != iosPlatform)
                    {
                        throw new ArgumentException("Allowed platform options are: \"android\", \"ios\"");
                    }

                    string projectPath = args.FirstOrDefault(a => a.StartsWith(projectPathToken, StringComparison.OrdinalIgnoreCase)).Substring(projectPathToken.Length);
                    string packageName = args.FirstOrDefault(a => a.StartsWith(packageNameToken, StringComparison.OrdinalIgnoreCase)).Substring(packageNameToken.Length);

                    if (platform == androidPlatform)
                    {
                        Program.LoadManifestAndUpdatePackage(projectPath, packageName);
                    }
                    else if (platform == iosPlatform)
                    {
                        Program.LoadPlistAndUpdateBundle(projectPath, packageName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
                   
        }

        private static void LoadPlistAndUpdateBundle(string projectPath, string newBundleId)
        {
            var fullProjectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, projectPath));

            Uri projectFile = new Uri(fullProjectPath);

            // get path without file name
            string projectFilename = projectFile.Segments.Last();
            string projectDirectory = fullProjectPath.Replace(projectFilename, string.Empty);

            string plistPath = string.Format("{0}/Info.plist", projectDirectory);   

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Loading {0}", plistPath);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(plistPath);
            Console.WriteLine("Loaded {0}", plistPath);

            Console.WriteLine("Updating CFBundleIdentifier in Info.plist to {0}", newBundleId);
            xmlDoc = Program.EditXmlNodes(xmlDoc, bundleIdentifierXPath, newBundleId);
            xmlDoc.Save(plistPath);
            Console.WriteLine("Info.plist updated successfully");
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

            Console.WriteLine("Loading manifest node");
            var manifestNode = xmldoc.SelectSingleNode("/manifest");
            Console.WriteLine("Getting manifest attributes");
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

        private static XmlDocument EditXmlNodes(XmlDocument doc, string xpath, string value, bool condition = true)
        {
            if (condition == true)
            {
                var nodes = doc.SelectNodes(xpath);

                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    if (node != null)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            node.InnerXml = value;
                        }
                        else
                        {
                            node.Value = value;
                        }
                    }
                }
            }

            return doc;
        }
    }
}
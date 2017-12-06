using System;
using System.Linq;
using System.Xml;
using System.IO;
using WhatsMissing;

namespace PackageBuddy.Runner
{
    class Program
    {
        private const string platformToken = "-platform=";
        private const string projectPathToken = "-projectPath=";
        private const string packageNameToken = "-packageName=";
        private const string buildNumberToken = "-build=";
        private const string versionToken = "-versionName=";
        private const string appNameToken = "-appName=";


        private const string androidPlatform = "android";
        private const string iosPlatform = "ios";

        private const string bundleIdentifierXPath_CFBundleIdentifier = "//key[text() ='CFBundleIdentifier']/following-sibling::*[1]";
        private const string bundleVersionBuildStringXPath_CFBundleVersion = "//key[text() ='CFBundleVersion']/following-sibling::*[1]";
        private const string bundleVersionNameStringXPath_CFBundleShortVersion = "//key[text() ='CFBundleShortVersionString']/following-sibling::*[1]";
        private const string bunleAppNameStringXPath_CFBundleName = "//key[text() ='CFBundleName']/following-sibling::*[1]";

        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("Usage:  PackageBuddy-Runner.exe -platform=android -projectPath=<path/to/AndroidProject.csproj> -packageName=my.new.packagename -applicationName='My App'");
                    Console.WriteLine("Usage:  PackageBuddy-Runner.exe -platform=ios -projectPath=<path/to/iOSProject.csproj> -packageName=my.new.packagename -applicationName='My App'");
                }
                else
                {
                    string platform = TrimArgument(args.FirstOrDefault(a => a.StartsWith(platformToken, StringComparison.OrdinalIgnoreCase)).Substring(platformToken.Length));

                    if (string.IsNullOrWhiteSpace(platform))
                    {
                        throw new ArgumentNullException("platform must be specified in argument list. Allowed options are: \"android\", \"ios\"");
                    }
                    else if (platform != androidPlatform && platform != iosPlatform)
                    {
                        throw new ArgumentException("Allowed platform options are: \"android\", \"ios\"");
                    }

                    string projectPath = TrimArgument(args.FirstOrDefault(a => a.StartsWith(projectPathToken, StringComparison.OrdinalIgnoreCase)).IfNotNull(x => x.Substring(projectPathToken.Length)));
                    string packageName = TrimArgument(args.FirstOrDefault(a => a.StartsWith(packageNameToken, StringComparison.OrdinalIgnoreCase)).IfNotNull(x => x.Substring(packageNameToken.Length)));
                    string versionNumber = TrimArgument(args.FirstOrDefault(a => a.StartsWith(buildNumberToken, StringComparison.OrdinalIgnoreCase)).IfNotNull(x => x.Substring(buildNumberToken.Length)));
                    string versionName = TrimArgument(args.FirstOrDefault(a => a.StartsWith(versionToken, StringComparison.OrdinalIgnoreCase)).IfNotNull(x => x.Substring(versionToken.Length)));
                    string appName = TrimArgument(args.FirstOrDefault(a => a.StartsWith(appNameToken, StringComparison.OrdinalIgnoreCase)).IfNotNull(x => x.Substring(appNameToken.Length)));

                    if (platform == androidPlatform)
                    {
                        Program.LoadManifestAndUpdatePackage(projectPath, packageName, versionName, versionNumber, appName);
                    }
                    else if (platform == iosPlatform)
                    {
                        Program.LoadPlistAndUpdateBundle(projectPath, packageName, versionName, versionNumber, appName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void LoadPlistAndUpdateBundle(string projectPath, string newBundleId = null, string newVersionName = null, string newBuildNumber = null, string newAppName = null)
        {
            var fullProjectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, projectPath));

            XmlDocument xmlDoc = Program.LoadXmlDoc("ios", fullProjectPath);

            if (string.IsNullOrEmpty(newBundleId) == false)
            {
                string currentBundleId = Program.GetXmlNodeValue(xmlDoc, bundleIdentifierXPath_CFBundleIdentifier);

                Console.WriteLine("Current BundleId: " + currentBundleId);
                if (string.IsNullOrWhiteSpace(currentBundleId) == false && currentBundleId != newBundleId)
                {
                    Console.WriteLine("Updating CFBundleIdentifier to {0}", newBundleId);
                    xmlDoc = Program.EditXmlNodes(xmlDoc, bundleIdentifierXPath_CFBundleIdentifier, newBundleId);
                }
                else
                {
                    Console.WriteLine("Don't need to change the Bundle Id. It is already " + newBundleId);
                }
            }

            if (string.IsNullOrWhiteSpace(newBuildNumber) == false)
            {
                string currentBuildNumber = Program.GetXmlNodeValue(xmlDoc, bundleVersionBuildStringXPath_CFBundleVersion);

                Console.WriteLine("Current Bundle Verion Build #: " + currentBuildNumber);
                if (string.IsNullOrWhiteSpace(currentBuildNumber) == false && currentBuildNumber != newBuildNumber)
                {
                    Console.WriteLine("Updating CFBundleVersion to {0}", newBuildNumber);
                    xmlDoc = Program.EditXmlNodes(xmlDoc, bundleVersionBuildStringXPath_CFBundleVersion, newBuildNumber);
                }
                else
                {
                    Console.WriteLine("Don't need to change the Bundle Version. It is already " + newBuildNumber);
                }
            }

            if (string.IsNullOrWhiteSpace(newVersionName) == false)
            {
                string currentVersionName = Program.GetXmlNodeValue(xmlDoc, bundleVersionNameStringXPath_CFBundleShortVersion);

                Console.WriteLine("Current Bundle Verion Name: " + currentVersionName);
                if (string.IsNullOrWhiteSpace(currentVersionName) == false && currentVersionName != newVersionName)
                {
                    Console.WriteLine("Updating CFBundleShortVersionString to {0}", newVersionName);
                    xmlDoc = Program.EditXmlNodes(xmlDoc, bundleVersionNameStringXPath_CFBundleShortVersion, newVersionName);
                }
                else
                {
                    Console.WriteLine("Don't need to change the Bundle Version. It is already " + newVersionName);
                }
            }

            if (string.IsNullOrWhiteSpace(newAppName) == false)
            {
                string currentAppName = Program.GetXmlNodeValue(xmlDoc, bunleAppNameStringXPath_CFBundleName);

                Console.WriteLine("Current Application Name: " + currentAppName);
                if (string.IsNullOrWhiteSpace(currentAppName) == false && currentAppName != newAppName)
                {
                    Console.WriteLine("Updating CFBundleName to {0}", newVersionName);
                    xmlDoc = Program.EditXmlNodes(xmlDoc, bunleAppNameStringXPath_CFBundleName, newAppName);
                }
                else
                {
                    Console.WriteLine("Don't need to change the Bundle Version. It is already " + newVersionName);
                }
            }

            Program.SaveXmlDoc(xmlDoc, "ios", fullProjectPath);
        }

        private static void LoadManifestAndUpdatePackage(string projectPath, string newPackageName = null, string versionName = null, string newBuildNumber = null, string newAppName = null)
        {
            var fullProjectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, projectPath));

            XmlDocument xmlDoc = Program.LoadXmlDoc("android", fullProjectPath);

            Console.WriteLine("Loading manifest node");
            var manifestNode = xmlDoc.SelectSingleNode("/manifest");
            Console.WriteLine("Getting manifest attributes");
            var manifestAttributes = manifestNode.Attributes;
            Console.WriteLine("Loading application node");
            var applicationNode = xmlDoc.SelectSingleNode("/manifest/application");
            Console.WriteLine("Getting application attributes");
            var applicationAttributes = applicationNode.Attributes;

            if (string.IsNullOrWhiteSpace(newPackageName) == false)
            {
                var package = manifestAttributes.GetNamedItem("package");
                if (package != null)
                {
                    if (string.IsNullOrWhiteSpace(package.Value) == false && package.Value != newPackageName)
                    {
                        Console.WriteLine("Current package name: " + package.Value);

                        package.Value = newPackageName;
                    }
                    else
                    {
                        Console.WriteLine("Don't need to change the package name. It is already " + newPackageName);
                    }
                }
                else
                {
                    Console.WriteLine("Could not find package. Skipping.");
                }
            }

            if (string.IsNullOrWhiteSpace(versionName) == false)
            {
                var currentVersionName = manifestAttributes.GetNamedItem("android:versionName");
                if (currentVersionName != null)
                {
                    if (string.IsNullOrWhiteSpace(currentVersionName.Value) == false && currentVersionName.Value != versionName)
                    {
                        Console.WriteLine("Current version number: " + versionName);

                        string xmlString = xmlDoc.OuterXml;

                        currentVersionName.Value = versionName;
                    }
                    else
                    {
                        Console.WriteLine("Don't need to change the version name. It is already " + versionName);
                    }
                }
                else
                {
                    Console.WriteLine("Could not find android:versionCode. Skipping.");
                }
            }

            if (string.IsNullOrWhiteSpace(newBuildNumber) == false)
            {
                var currentVersionNumber = manifestAttributes.GetNamedItem("android:versionCode");
                if (currentVersionNumber != null)
                {
                    if (string.IsNullOrWhiteSpace(currentVersionNumber.Value) == false && currentVersionNumber.Value != newBuildNumber)
                    {
                        Console.WriteLine("Current version number: " + newBuildNumber);

                        string xmlString = xmlDoc.OuterXml;

                        currentVersionNumber.Value = newBuildNumber;
                    }
                    else
                    {
                        Console.WriteLine("Don't need to change the version number. It is already " + newBuildNumber);
                    }
                }
                else
                {
                    Console.WriteLine("Could not find android:versionNumber. Skipping.");
                }
            }

            if (string.IsNullOrWhiteSpace(newAppName) == false)
            {
                var label = applicationAttributes.GetNamedItem("android:label");
                if (label != null)
                {
                    if (string.IsNullOrWhiteSpace(label.Value) == false && label.Value != newAppName)
                    {
                        Console.WriteLine("Current Application label: " + label.Value);

                        label.Value = newAppName;
                    }
                    else
                    {
                        Console.WriteLine("Don't need to change the application name. It is already " + newAppName);
                    }
                }
                else
                {
                    Console.WriteLine("Could not find label. Skipping.");
                }
            }


            Program.SaveXmlDoc(xmlDoc, "android", fullProjectPath);
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

        private static string GetXmlNodeValue(XmlDocument doc, string xpath)
        {
            var nodes = doc.SelectNodes(xpath);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                if (node != null)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        return node.InnerXml;
                    }
                    else
                    {
                        return node.Value;
                    }
                }
            }

            return null;
        }

        private static XmlDocument LoadXmlDoc(string platform, string fullProjectPath)
        {
            Uri projectFile = new Uri(fullProjectPath);

            // get path without file name
            string projectFilename = projectFile.Segments.Last();
            string projectDirectory = fullProjectPath.Replace(projectFilename, string.Empty);

            string plistPath = null;
            if (platform == "ios")
            {
                plistPath = string.Format("{0}Info.plist", projectDirectory);
            }
            else if (platform == "android")
            {
				
				plistPath = $"{projectDirectory}Properties{Path.DirectorySeparatorChar}AndroidManifest.xml";
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Loading {0}", plistPath);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(plistPath);
            Console.WriteLine("Loaded {0}", plistPath);

            return xmlDoc;
        }

        private static void SaveXmlDoc(XmlDocument doc, string platform, string fullProjectPath)
        {
            Uri projectFile = new Uri(fullProjectPath);

            // get path without file name
            string projectFilename = projectFile.Segments.Last();
            string projectDirectory = fullProjectPath.Replace(projectFilename, string.Empty);

            string plistPath = null;
            if (platform == "ios")
            {
				plistPath = $"{projectDirectory}{Path.DirectorySeparatorChar}Info.plist";
            }
            else if (platform == "android")
            {
                plistPath = $"{projectDirectory}Properties{Path.DirectorySeparatorChar}AndroidManifest.xml";
            }

            Console.WriteLine("Saving changed to {0}", fullProjectPath);
            doc.Save(plistPath);
            Console.WriteLine("{0} updated successfully", plistPath);
        }

        private static string TrimArgument(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg)) return arg;

            arg = arg.Trim();
            arg = arg.Trim('"');
            arg = arg.Trim('\'');

            return arg;
        }
    }
}
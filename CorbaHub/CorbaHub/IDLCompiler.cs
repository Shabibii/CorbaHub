using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ch.Elca.Iiop.IdlCompiler;

namespace CorbaHub
{
    public class IDLCompiler
    {
        // Interface Definition Language (.idl) file
        public static string _idlFile;

        // Represents the idlj.exe fle
        public static string _idlj;

        // Represents the mode of the program
        public static int _mode;

        // Location of output file 
        private static string outputPath;        

        // Define IDLToCLS (For translation of IDL to C#)
        private static IDLToCLS IDLToCLS;

        private static string fullInterfaceName = "";
        private static string interfaceName;

        // Converting bytes to data type string
        public static string ConvertByteToString(byte[] source)
        {
            return source != null ? System.Text.Encoding.UTF8.GetString(source) : null;
        }
        
        /// <summary>
        /// Generate stubs and skeletons based on IDL file
        /// </summary>
        public static void Compile()
        {
            try
            {
                outputPath = Directory.GetParent(_idlFile).FullName;

                // Read all text in IDL file
                string idlFileText = File.ReadAllText(_idlFile);

                // Use regular expression to check for the word after module and interface in the file 
                // .. group these two and separate with a dot
                foreach (Match item in Regex.Matches(idlFileText, @"module\s+(\w+)"))
                {
                    fullInterfaceName += item.Groups[1].Value + "."; 
                }
                interfaceName = Regex.Match(idlFileText, @"interface\s+(\w+)").Groups[1].Value;

                fullInterfaceName = fullInterfaceName.Trim('.');

                //  mode - 0, 1, 2
                //  0 -> C# and Java (default mode)
                //  1 -> only C#
                //  2 -> only Java
       
                if (_mode == 0)
                {
                    // Generate code templates for Java and C#
                    Console.WriteLine("Running default mode: generating files for Java and C#...");
                    GenerateCSharpFiles();
                    GenerateJavaFiles();
                }              
                else if (_mode == 1)
                {
                    // Generate code templates for C#
                    Console.WriteLine("Running mode 1: generating files for C#...");
                    GenerateCSharpFiles();
                }
                else if (_mode == 2)
                {
                    // Generate code templates for Java
                    Console.WriteLine("Running mode 2: generating files for Java...");
                    GenerateJavaFiles();
                }               
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Method to generate Java code templates 
        /// </summary>
        private static void GenerateJavaFiles()
        {
            // Put files in directory Java
            string outputPath = IDLCompiler.outputPath + "\\Java";

            // Define file name
            string fileName = new FileInfo(_idlFile).Name.Split('.')[0];

            // Create directory "Java" to store generated files
            if (!Directory.Exists("Java"))
                Directory.CreateDirectory("Java");

            // Create batch file for running idlj.exe on the IDL file            
            File.WriteAllText($"runIdlj.bat", $"@echo off\nidlj -fall {fileName}.idl\nmove {fileName} Java\\{fileName}");
            System.Diagnostics.Process.Start("runIdlj.bat");

            // Replace the IDL namespaces accordingly
            // .. generate for Server.java
            File.WriteAllText(outputPath + $"\\{fileName}Server.java", ConvertByteToString(CorbaHub.Properties.Resources.javaServer)
                .Replace("OBJECT", interfaceName)
                .Replace("*", fullInterfaceName));

            // .. generate for Client.java
            File.WriteAllText(outputPath + $"\\{fileName}Client.java", ConvertByteToString(CorbaHub.Properties.Resources.javaClient)
                .Replace("OBJECT", interfaceName)
                .Replace("*", fullInterfaceName));

            // .. generate for Object.java
            File.WriteAllText(outputPath + $"\\{fileName}Object.java", ConvertByteToString(CorbaHub.Properties.Resources.javaObject)
                .Replace("OBJECT", interfaceName)
                .Replace("*", fullInterfaceName));
        }

        private static void GenerateCSharpFiles()
        {
            // Put files in directory CSharp
            string outputPath = IDLCompiler.outputPath + "\\CSharp";

            // Define file name
            string fileName = new FileInfo(_idlFile).Name.Split('.')[0];

            // Create directory "CSharp" to store generated files
            if (!Directory.Exists("CSharp"))
                Directory.CreateDirectory("CSharp");

            // Put files in directory CSharp
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);          

            // Delete existing files from previous program executions
            if (File.Exists($"{fileName}.dll"))
                File.Delete($"{fileName}.dll");

            // Run IDL to CLS (create library for C#)
            string[] args = new string[2] { fileName, _idlFile };
            IDLToCLS = new IDLToCLS(args);
            IDLToCLS.MapIdl();

            // Create batch file to move generated files into C# folder
            File.WriteAllText("moveCSharpFiles.bat", $"move {fileName}.dll CSharp\\{fileName}.dll");
            System.Diagnostics.Process.Start("moveCSharpFiles.bat");

            // Write required files (from Program Resources) for CORBA implementation in C# in the CSharp directory
            // .. these files are available in the download of IIOP.NET (available at: https://sourceforge.net/projects/iiop-net/)
            File.WriteAllBytes(outputPath + "\\IDLPreProcCSharp.dll", CorbaHub.Properties.Resources.IDLPreProcCSharp);
            File.WriteAllBytes(outputPath + "\\IIOPChannel.dll", CorbaHub.Properties.Resources.IIOPChannel);
            File.WriteAllBytes(outputPath + "\\nunit.framework.dll", CorbaHub.Properties.Resources.nunit_framework);

            // Replace the IDL namespaces accordingly
            // .. generate for Server.cs
            File.WriteAllText(outputPath + $"\\{fileName}Server.cs", ConvertByteToString(CorbaHub.Properties.Resources.cSharpServer)
                .Replace("OBJECT", interfaceName)
                .Replace("*", fullInterfaceName));

            // .. generate for Client.cs
            File.WriteAllText(outputPath + $"\\{fileName}Client.cs", ConvertByteToString(CorbaHub.Properties.Resources.cSharpClient)
                .Replace("OBJECT", interfaceName)
                .Replace("*", fullInterfaceName));

            // .. generate for Object.cs
            File.WriteAllText(outputPath + $"\\{fileName}Object.cs", ConvertByteToString(CorbaHub.Properties.Resources.cSharpObject)
                .Replace("OBJECT", interfaceName)
                .Replace("*", fullInterfaceName));
        }

    }
}

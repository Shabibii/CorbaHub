using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorbaHub
{
    class CorbaHubMain
    {
        static void Main(string[] args)
        {
            // Handle cmd parameters
            HandleParameters(args);

            // Ask for jdk if not found
            AskForJdk();

            // Ask for idl file if not found
            AskForIdlFile();

            // Compile idl file according to the compilation mode
            CompileIdl();
        }

        private static void CompileIdl() =>
            // Compile idl file
            IDLCompiler.Compile();

        /// <summary>
        /// CorbaHub accepts three parameter:
        /// .. CorbaHub.exe idlFile mode jdkPath 
        /// .. CorbaHub.exe idlFile mode 
        /// .. CorbaHub.exe idlFile

        /// CorbaHub knows which language mapping is preferred through 'mode'.
        /// .. mode can be 0, 1 or 2:         0 -> Both
        /// ..                                1 -> C#
        /// ..                                2 -> Java
        /// </summary>
        /// <param name="args"></param>
        private static void HandleParameters(string[] args)
        {    
            int i = 0;           
            foreach (var parameter in args)
            {
                switch (i)
                {
                    case 0:
                        // Checking first parameter
                        IDLCompiler._idlFile = parameter;
                        break;

                    case 1:                        
                        // Checking second parameter
                        int.TryParse(args[1], out IDLCompiler._mode);
                        break;

                    case 2 when File.Exists(args[2] + "\\bin\\idlj.exe"):
                        // Checking third parameter
                        IDLCompiler._idlj = args[2];
                        break;
                }
                // Next parameterHandle 
                i++;
            }
            try
            {
                // Call method to search for the idlj.exe file manually
                if (IDLCompiler._idlj == null)
                    FindJDKPath();
                Console.Write("JDK path found!\n");
            }
            catch (Exception)
            {
                Console.WriteLine("Can not find idlj.exe file..");
            }
        }

        /// <summary>
        /// Request user for IDL file (.idl) in debug folder
        /// </summary>
        private static void AskForIdlFile()
        {
            //Make sure CorbaHub knows the idl file's path
            while (IDLCompiler._idlFile == null)
            {
                // Request for IDL file
                Console.WriteLine("No IDL file recieved");
                Console.Write("Please input IDL file path: ");
                string input = Console.ReadLine();

                // Check IDL file
                if (File.Exists(input))
                    IDLCompiler._idlFile = input;
            }
        }

        /// <summary>
        /// If search for idlj.exe in FindJDKPath has failed
        /// .. then, request user for path to the JDK folder (JDK 1 - 10, compatible with CORBA)
        /// </summary>
        private static void AskForJdk()
        {
            // Ensure CorbaHub knows the idl path
            while (IDLCompiler._idlj == null)
            {
                // If can not find jdk path
                if ((IDLCompiler._mode == 0 || IDLCompiler._mode == 2) && IDLCompiler._idlj == null)
                {   
                    // Request for JDK path                    
                    Console.Write("Please input JDK path: ");
                    string input = Console.ReadLine();

                    // Get the idlj.exe file from the bin folder in the JDK directory
                    if (File.Exists(input + "\\bin\\idlj.exe"))
                    {
                        IDLCompiler._idlj = input + "\\bin\\idlj.exe"; // full path
                    }
                }
            }
        }

        /// <summary>
        /// Search for idlj.exe file manually
        /// </summary>
        private static void FindJDKPath()
        {
            Console.Write("Performing search to JDK path manually...\n");

            // Start new process
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    // Use where.exe to check for idlj file on user's devices manually                     
                    FileName = "where.exe",
                    Arguments = "idlj",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            p.Start();
            p.WaitForExit(2000);

            IDLCompiler._idlj =  Directory.GetParent(p.StandardOutput.ReadToEnd()).Parent.FullName + "\\idlj.exe";
        }
    }
}

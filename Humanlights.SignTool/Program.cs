using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Humanlights.SignTool
{
    class Program
    {
        [DllImport ( "Imagehlp.dll " )]
        private static extern bool ImageRemoveCertificate ( IntPtr handle, int index );

        private const string signArg = "/sign";
        private const string unsignArg = "/unsign";
        private const string folderPathArg = "-folder";
        private const string certificatePathArg = "-certificate";

        static void Main ( string [] args )
        {
            var folderPath = Helper.GetLineArgumentResult ( args, folderPathArg, null );
            var certificatePath = Helper.GetLineArgumentResult ( args, certificatePathArg, null );
            var signMode = Helper.LineArgumentExists ( args, signArg );
            var unsignMode = Helper.LineArgumentExists ( args, unsignArg );

            if ( signMode )
                RunSigning ( folderPath, certificatePath );
            else if ( unsignMode )
                RunUnsigning ( folderPath );
            else
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  No mode set! Please set a mode through the batch file using:\n  {signArg} for signing\n  {unsignArg} for unsigning" );
                Console.Read ();
                return;
            }
        }

        private static void RunSigning ( string folder, string certificate )
        {
            var signtoolPath = System.IO.Path.GetDirectoryName ( System.Reflection.Assembly.GetExecutingAssembly ().Location ) + "/signtool.exe";
            var certificatePath = System.IO.Path.GetDirectoryName ( System.Reflection.Assembly.GetExecutingAssembly ().Location ) + $"/{certificate}";

            if ( string.IsNullOrEmpty ( folder ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Folder path not set." );
                Console.Read ();
                return;
            }
            if ( string.IsNullOrEmpty ( signtoolPath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Sign Tool path not set." );
                Console.Read ();
                return;
            }
            if ( string.IsNullOrEmpty ( certificatePath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Certificate path not set." );
                Console.Read ();
                return;
            }

            if ( !System.IO.Directory.Exists ( folder ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Folder not found." );
                Console.Read ();
                return;
            }

            if ( !System.IO.File.Exists ( signtoolPath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  \"signtool.exe\" not found." );
                Console.Read ();
                return;
            }
            if ( !System.IO.File.Exists ( certificatePath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Certificate not found." );
                Console.Read ();
                return;
            }

            foreach(string file in Helper.FilesInDirectories(folder))
            {
                var signingCommandLine = $"sign /f \"{certificatePath}\" /t http://timestamp.verisign.com/scripts/timstamp.dll /v \"{file}\"";

                var process = new Process ();
                process.StartInfo.FileName = signtoolPath;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.ErrorDialog = false;
                process.StartInfo.Arguments = signingCommandLine;
                process.Start ();

                Console.WriteLine ( $"Signing: {System.IO.Path.GetFileName(file).ToLower()}" );
            }

            Console.WriteLine ();
            Console.WriteLine ( $"  Finished Digital Singing successfully!" );
            Console.WriteLine ( $"  (c) Copyright 2017 Humanlights Studios LTD" );

            System.Threading.Thread.Sleep ( 5000 );
        }
        private static void RunUnsigning ( string folder )
        {
            if ( string.IsNullOrEmpty ( folder ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Folder path not set." );
                Console.Read ();
                return;
            }
            if ( !System.IO.Directory.Exists ( folder ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Folder not found." );
                Console.Read ();
                return;
            }

            foreach ( string file in Helper.FilesInDirectories ( folder ) )
            {
                using ( FileStream fileStream = new FileStream ( file, FileMode.Open, FileAccess.ReadWrite ) )
                {
                    Program.ImageRemoveCertificate ( fileStream.SafeFileHandle.DangerousGetHandle (), 0 );
                    fileStream.Close ();
                }

                Console.WriteLine ( $"Unsigning: {System.IO.Path.GetFileName ( file ).ToLower ()}" );
            }

            Console.WriteLine ();
            Console.WriteLine ( $"  Finished Digital Unsinging successfully!" );
            Console.WriteLine ( $"  (c) Copyright 2017 Humanlights Studios LTD" );

            System.Threading.Thread.Sleep ( 5000 );
        }
    }
}

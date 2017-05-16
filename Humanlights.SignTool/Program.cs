using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
        private const string fullArg = "/full";

        private const string folderPathArg = "-folder";
        private const string filePathArg = "-file";
        private const string certificatePathArg = "-certificate";
        private const string timestampPathArg = "-timestamp";

        static void Main ( string [] args )
        {
            var folderPath = Helper.GetLineArgumentResult ( args, folderPathArg, null );
            var filePath = Helper.GetLineArgumentResult ( args, filePathArg, null );
            var certificatePath = Helper.GetLineArgumentResult ( args, certificatePathArg, null );
            var timestampURL = Helper.GetLineArgumentResult ( args, timestampPathArg, "http://timestamp.verisign.com/scripts/timstamp.dll" );

            var signMode = Helper.LineArgumentExists ( args, signArg );
            var unsignMode = Helper.LineArgumentExists ( args, unsignArg );
            var fullPrint = Helper.LineArgumentExists ( args, fullArg );

            PrintHeader ( args );

            if ( signMode )
                RunSigning ( folderPath, filePath, certificatePath, timestampURL, fullPrint );
            else if ( unsignMode )
                RunUnsigning ( folderPath, filePath, fullPrint );
            else
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  No mode set! Please set a mode through the batch file using:\n  {signArg} for signing\n  {unsignArg} for unsigning" );
                Console.Read ();
                return;
            }
        }

        private static void RunSigning ( string folderPath, string filePath, string certificate, string timestamp, bool fullPrint )
        {
            var signtoolPath = Path.GetDirectoryName ( System.Reflection.Assembly.GetExecutingAssembly ().Location ) + "/signtool.exe";
            var certificatePath = File.Exists ( certificate ) ? certificate : Path.GetDirectoryName ( System.Reflection.Assembly.GetExecutingAssembly ().Location ) + $"/{certificate}";

            if ( string.IsNullOrEmpty ( signtoolPath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  \"signtool.exe\" path not set." );
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

            if ( !File.Exists ( signtoolPath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  \"signtool.exe\" not found: {signtoolPath}" );
                Console.Read ();
                return;
            }
            if ( !File.Exists ( certificatePath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Certificate not found: {certificatePath}" );
                Console.Read ();
                return;
            }

            if ( !string.IsNullOrEmpty ( folderPath ) )
            {
                Console.WriteLine ();

                if ( !Directory.Exists ( folderPath ) )
                {
                    Console.WriteLine ( $"  Folder not found: {folderPath}" );
                    Console.Read ();
                }
                else
                {
                    foreach ( string file in Helper.FilesInDirectories ( folderPath ) )
                    {
                        var signingCommandLine = $"sign /f \"{certificatePath}\" /t {timestamp} /v \"{file}\"";

                        var process = new Process ();
                        process.StartInfo.FileName = signtoolPath;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.ErrorDialog = false;
                        process.StartInfo.Arguments = signingCommandLine;
                        process.Start ();

                        Console.WriteLine ( $"Signing: {( fullPrint == false ? Path.GetFileName ( file ).ToLower () : file )}" );
                    }

                }
            }
            if ( !string.IsNullOrEmpty ( filePath ) )
            {
                Console.WriteLine ();

                if ( !File.Exists ( filePath ) )
                {
                    Console.WriteLine ( $"  File not found: {filePath}" );
                    Console.Read ();
                }
                else
                {
                    var signingCommandLine = $"sign /f \"{certificatePath}\" /t {timestamp} /v \"{filePath}\"";

                    var process = new Process ();
                    process.StartInfo.FileName = signtoolPath;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.ErrorDialog = false;
                    process.StartInfo.Arguments = signingCommandLine;
                    process.Start ();

                    Console.WriteLine ( $"Signing: {( fullPrint == false ? Path.GetFileName ( filePath ).ToLower () : filePath )}" );
                }
            }

            Console.WriteLine ();
            Console.WriteLine ( $"  Finished digital singing successfully!" );
            Console.WriteLine ( $"  (c) Copyright 2017 Humanlights Studios LTD" );

            System.Threading.Thread.Sleep ( 1000 );
        }
        private static void RunUnsigning ( string folderPath, string filePath, bool fullPrint )
        {
            if ( !string.IsNullOrEmpty ( folderPath ) )
            {
                Console.WriteLine ();

                if ( !Directory.Exists ( folderPath ) )
                {
                    Console.WriteLine ( $"  Folder not found: {folderPath}" );
                    Console.Read ();
                    return;
                }
                else
                {
                    foreach ( string file in Helper.FilesInDirectories ( folderPath ) )
                    {
                        using ( FileStream fileStream = new FileStream ( file, FileMode.Open, FileAccess.ReadWrite ) )
                        {
                            ImageRemoveCertificate ( fileStream.SafeFileHandle.DangerousGetHandle (), 0 );
                            fileStream.Close ();
                        }

                        Console.WriteLine ( $"Unsigning: {( fullPrint == false ? Path.GetFileName ( file ).ToLower () : file )}" );
                    }

                }
            }
            if ( !string.IsNullOrEmpty ( filePath ) )
            {
                Console.WriteLine ();

                if ( !File.Exists ( filePath ) )
                {
                    Console.WriteLine ( $"  File not found: {filePath}" );
                    Console.Read ();
                    return;
                }
                else
                {
                    using ( FileStream fileStream = new FileStream ( filePath, FileMode.Open, FileAccess.ReadWrite ) )
                    {
                        ImageRemoveCertificate ( fileStream.SafeFileHandle.DangerousGetHandle (), 0 );
                        fileStream.Close ();
                    }

                    Console.WriteLine ( $"Unsigning: {( fullPrint == false ? Path.GetFileName ( filePath ).ToLower () : filePath )}" );
                }
            }

            Console.WriteLine ();
            Console.WriteLine ( $"  Finished digital unsinging successfully!" );
            Console.WriteLine ( $"  (c) Copyright 2017 Humanlights Studios LTD" );

            System.Threading.Thread.Sleep ( 1000 );
        }

        private static void PrintHeader (string[] args)
        {
            var client = new WebClient();
            var githubLicense = client.DownloadString("https://raw.githubusercontent.com/Humanlights/Humanlights.SignTool/master/LICENSE");
            
            Console.WriteLine ();
            Console.WriteLine ( githubLicense.Trim () );

            System.Threading.Thread.Sleep ( 500 );
        }
    }
}
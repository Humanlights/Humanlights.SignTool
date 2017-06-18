using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Humanlights.SignTool
{
    class Program
    {
        [DllImport ( "Imagehlp.dll " )]
        private static extern bool ImageRemoveCertificate ( IntPtr handle, int index );

        private const string signArg = "sign";
        private const string unsignArg = "unsign";
        private const string overwriteArg = "overwrite";
        private const string fullPrintArg = "/f";

        private const string folderPathArg = "-folder";
        private const string filePathArg = "-file";

        private const string digestAlgorithmArg = "/da";
        private const string passwordArg = "+password";

        private const string certificatePathArg = "-certificate";
        private const string alternativeCertificatePathArg = "-altcertificate";
        private const string timestampPathArg = "-timestamp";

        //
        // Timestamps:
        // http://timestamp.verisign.com/scripts/timstamp.dll [*.cer OK]
        // http://timestamp.comodoca.com/rfc3161 [*pfx + password OK]
        //

        static void Main ( string [] args )
        {
            var signMode = Extensions.CommandLineEx.GetArgumentExists ( args, signArg );
            var unsignMode = Extensions.CommandLineEx.GetArgumentExists ( args, unsignArg );
            var overwrite = Extensions.CommandLineEx.GetArgumentExists ( args, overwriteArg );
            var fullPrint = Extensions.CommandLineEx.GetArgumentExists ( args, fullPrintArg );

            var folderPath = Extensions.CommandLineEx.GetArgumentResult ( args, folderPathArg, null );
            var filePath = Extensions.CommandLineEx.GetArgumentResult ( args, filePathArg, null );

            var digestAlgorithm = Extensions.CommandLineEx.GetArgumentResult ( args, digestAlgorithmArg, "SHA256" );
            var password = Extensions.CommandLineEx.GetArgumentResult ( args, passwordArg, null );

            var certificatePath = Extensions.CommandLineEx.GetArgumentResult ( args, certificatePathArg, null );
            var alternativeCertificatePath = Extensions.CommandLineEx.GetArgumentResult ( args, alternativeCertificatePathArg, null );
            var timestampURL = Extensions.CommandLineEx.GetArgumentResult ( args, timestampPathArg, null );

            if ( signMode )
                RunSigning ( overwrite, folderPath, filePath, certificatePath, alternativeCertificatePath, timestampURL, digestAlgorithm, password, fullPrint );
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

        private static void RunSigning (
            bool overwrite,
            string folderPath,
            string filePath,
            string certificate,
            string additionalCertificate,
            string timestamp,
            string digestAlgorithm,
            string password,
            bool fullPrint )
        {
            var signtoolPath = Path.GetDirectoryName ( System.Reflection.Assembly.GetExecutingAssembly ().Location ) + "/signtool.exe";

            if ( string.IsNullOrEmpty ( signtoolPath ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  \"signtool.exe\" path not set." );
                Console.Read ();
                return;
            }
            if ( string.IsNullOrEmpty ( certificate ) )
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
            if ( !File.Exists ( certificate ) )
            {
                Console.WriteLine ();
                Console.WriteLine ( $"  Certificate not found: {certificate}" );
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
                        if ( Helper.HasExtension ( certificate, "pfx" ) )
                        {
                            var commandLine = $"sign /v {( overwrite ? "" : "/as" )} /f \"{certificate}\" {( string.IsNullOrEmpty ( additionalCertificate ) ? "" : $"/ac \"{additionalCertificate}\"" )} /fd {digestAlgorithm.Trim ().ToLower ()} /p \"{password}\" {( string.IsNullOrEmpty ( timestamp ) ? "" : "/t \"{timestamp}\"" )} \"{file}\"";

                            var process = new Process ();
                            process.StartInfo.FileName = signtoolPath;
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            process.StartInfo.ErrorDialog = false;
                            process.StartInfo.Arguments = commandLine;
                            process.Start ();

                            Console.WriteLine ( $"Signing: {( fullPrint == false ? Path.GetFileName ( file ).ToLower () : file )}" );
                        }
                        else if ( Helper.HasExtension ( certificate, "cer" ) )
                        {
                            var commandLine = $"sign /v {( overwrite ? "" : "/as" )} /f \"{certificate}\" /t \"{timestamp}\" /fd {digestAlgorithm.Trim ().ToLower ()} \"{file}\"";

                            var process = new Process ();
                            process.StartInfo.FileName = signtoolPath;
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            process.StartInfo.ErrorDialog = false;
                            process.StartInfo.Arguments = commandLine;
                            process.Start ();

                            Console.WriteLine ( $"Signing: {( fullPrint == false ? Path.GetFileName ( file ).ToLower () : file )}" );
                        }
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
                    if ( Helper.EndsWith ( filePath ) )
                    {
                        if ( Helper.HasExtension ( certificate, "pfx" ) )
                        {
                            var commandLine = $"sign /v {( overwrite ? "" : "/as" )} /f \"{certificate}\" {( string.IsNullOrEmpty ( additionalCertificate ) ? "" : $"/ac \"{additionalCertificate}\"" )} /fd {digestAlgorithm.Trim ().ToLower ()} /p \"{password}\" {( string.IsNullOrEmpty ( timestamp ) ? "" : "/t \"{timestamp}\"" )} \"{filePath}\"";

                            var process = new Process ();
                            process.StartInfo.FileName = signtoolPath;
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            process.StartInfo.ErrorDialog = false;
                            process.StartInfo.Arguments = commandLine;
                            process.Start ();

                            Console.WriteLine ( $"Signing: {( fullPrint == false ? Path.GetFileName ( filePath ).ToLower () : filePath )}" );
                        }
                        else if ( Helper.HasExtension ( certificate, "cer" ) )
                        {
                            var commandLine = $"sign /v {( overwrite ? "" : "/as" )} /f \"{certificate}\" /t \"{timestamp}\" /fd {digestAlgorithm.Trim ().ToLower ()} \"{filePath}\"";

                            var process = new Process ();
                            process.StartInfo.FileName = signtoolPath;
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            process.StartInfo.ErrorDialog = false;
                            process.StartInfo.Arguments = commandLine;
                            process.Start ();

                            Console.WriteLine ( $"Signing: {( fullPrint == false ? Path.GetFileName ( filePath ).ToLower () : filePath )}" );
                        }
                    }
                    else
                    {
                        Console.WriteLine ( $"File {( fullPrint == false ? Path.GetFileName ( filePath ).ToLower () : filePath )} is not legible for signing. Be sure it ends with *.dll, *.exe, *.ocx or *.cab." );
                        System.Threading.Thread.Sleep ( 5000 );
                    }
                }
            }

            Console.WriteLine ();
            Console.WriteLine ( $"  Finished digital singing successfully!" );
            Console.WriteLine ( $"  (c) Copyright 2017 Humanlights Studios LTD" );

            System.Threading.Thread.Sleep ( 1000 );
        }
        private static void RunUnsigning (
            string folderPath,
            string filePath,
            bool fullPrint )
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
                    if ( Helper.EndsWith ( filePath ) )
                    {
                        using ( FileStream fileStream = new FileStream ( filePath, FileMode.Open, FileAccess.ReadWrite ) )
                        {
                            ImageRemoveCertificate ( fileStream.SafeFileHandle.DangerousGetHandle (), 0 );
                            fileStream.Close ();
                        }

                        Console.WriteLine ( $"Unsigning: {( fullPrint == false ? Path.GetFileName ( filePath ).ToLower () : filePath )}" );
                    }
                    else
                    {
                        Console.WriteLine ( $"File {( fullPrint == false ? Path.GetFileName ( filePath ).ToLower () : filePath )} is not legible for unsigning. Be sure it ends with *.dll, *.exe, *.ocx or *.cab." );
                        System.Threading.Thread.Sleep ( 5000 );
                    }
                }
            }

            Console.WriteLine ();
            Console.WriteLine ( $"  Finished digital unsinging successfully!" );
            Console.WriteLine ( $"  (c) Copyright 2017 Humanlights Studios LTD" );

            System.Threading.Thread.Sleep ( 1000 );
        }
    }
}
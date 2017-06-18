using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Humanlights.SignTool
{
    public class Helper
    {
        public static List<string> FilesInDirectories ( string mainFolder )
        {
            var all = new List<string> ();

            foreach ( string file in Directory.GetFiles ( mainFolder, "*.exe", SearchOption.AllDirectories ).ToList () )
                all.Add ( file );

            foreach ( string file in Directory.GetFiles ( mainFolder, "*.dll", SearchOption.AllDirectories ).ToList () )
                all.Add ( file );

            foreach ( string file in Directory.GetFiles ( mainFolder, "*.ocx", SearchOption.AllDirectories ).ToList () )
                all.Add ( file );

            foreach ( string file in Directory.GetFiles ( mainFolder, "*.cab", SearchOption.AllDirectories ).ToList () )
                all.Add ( file );

            return all;
        }
        public static bool EndsWith ( string file )
        {
            var f = file.ToLower ();

            if ( f.EndsWith ( ".exe" ) || f.EndsWith ( ".dll" ) || f.EndsWith ( ".ocx" ) || f.EndsWith ( ".cab" ) )
                return true;

            return false;
        }
        public static bool HasExtension ( string file, string extension )
        {
            if ( file.ToLower ().EndsWith ( $".{extension}" ) )
                return true;

            return false;
        }
    }
}

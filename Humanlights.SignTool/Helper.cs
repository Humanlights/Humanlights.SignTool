using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanlights.SignTool
{
    public class Helper
    {
        public static string GetLineArgumentResult ( string [] args, string argument, string Default )
        {
            for ( int i = 0; i < args.Length; i++ )
                if ( args [ i ] == argument ) return string.IsNullOrEmpty ( args [ i + 1 ] ) ? Default : args [ i + 1 ];

            return Default;
        }
        public static bool LineArgumentExists ( string [] args, string argument )
        {
            for ( int i = 0; i < args.Length; i++ )
                if ( args [ i ] == argument ) return true;

            return false;
        }

        public static List<string> FilesInDirectories (string mainFolder)
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
    }
}

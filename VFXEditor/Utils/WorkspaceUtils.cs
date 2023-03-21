using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Utils {
    public struct WorkspaceMetaAvfx {
        public SelectResult Source;
        public SelectResult Replace;
        public Dictionary<string, string> Renaming;
        public string RelativeLocation; // can be empty if no avfx file
    }

    public struct WorkspaceMetaTex {
        public int Height;
        public int Width;
        public int Depth;
        public int MipLevels;
        public TextureFormat.TextureFormat Format;
        public string RelativeLocation;
        public string ReplacePath;
    }

    public struct WorkspaceMetaBasic {
        public SelectResult Source;
        public SelectResult Replace;
        public string RelativeLocation;
    }

    public static class WorkspaceUtils {
        public static string ResolveWorkspacePath( string relativeLocation, string loadLocation ) =>
            ( relativeLocation == "" ) ? "" : Path.Combine( loadLocation, relativeLocation );

        public static T[] ReadFromMeta<T>( JObject meta, string key ) {
            if( !meta.ContainsKey( key ) ) return null;
            return JsonConvert.DeserializeObject<T[]>( meta[key].ToString() );
        }

        public static void WriteToMeta<T>( Dictionary<string, string> meta, T[] items, string key ) {
            meta[key] = JsonConvert.SerializeObject( items );
        }
    }
}

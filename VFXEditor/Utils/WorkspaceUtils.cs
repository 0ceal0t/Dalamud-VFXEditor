using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Select;

namespace VfxEditor.Utils {
    public struct WorkspaceMetaRenamed {
        public string Name;
        public SelectResult Source;
        public SelectResult Replace;
        public bool Disabled;
        public Dictionary<string, string> Renaming;
        public string RelativeLocation;
    }

    public struct WorkspaceMetaTex {
        public string Name;
        public string RelativeLocation;
        public string ReplacePath;
        public bool IsFolder;
        public WorkspaceMetaTex[] Children;
    }

    public struct WorkspaceMetaBasic {
        public string Name;
        public SelectResult Source;
        public SelectResult Replace;
        public bool Disabled;
        public string RelativeLocation;
    }

    public static class WorkspaceUtils {
        public static string ResolveWorkspacePath( string relativeLocation, string loadLocation ) => ( relativeLocation == "" ) ? "" : Path.Combine( loadLocation, relativeLocation );

        public static T[] ReadFromMeta<T>( JObject meta, string key ) {
            if( !meta.ContainsKey( key ) ) return null;
            return JsonConvert.DeserializeObject<T[]>( meta[key].ToString() );
        }

        public static void WriteToMeta<T>( Dictionary<string, string> meta, T[] items, string key ) {
            meta[key] = JsonConvert.SerializeObject( items );
        }
    }
}

using System;
using System.Collections.Generic;
using VfxEditor.TmbFormat.Entries;

namespace VfxEditor.TmbFormat.Utils {
    public struct ItemTypeStruct {
        public string DisplayName;
        public Type Type;

        public ItemTypeStruct( string displayName, Type type ) {
            DisplayName = displayName;
            Type = type;
        }
    }

    public class TmbUtils {
        public static readonly SortedDictionary<string, ItemTypeStruct> ItemTypes = new() {
            { C063.MAGIC, new ItemTypeStruct( C063.DISPLAY_NAME, typeof(C063) ) },
            { C006.MAGIC, new ItemTypeStruct( C006.DISPLAY_NAME, typeof(C006) ) },
            { C010.MAGIC, new ItemTypeStruct( C010.DISPLAY_NAME, typeof(C010) ) },
            { C131.MAGIC, new ItemTypeStruct( C131.DISPLAY_NAME, typeof(C131) ) },
            { C002.MAGIC, new ItemTypeStruct( C002.DISPLAY_NAME, typeof(C002) ) },
            { C011.MAGIC, new ItemTypeStruct( C011.DISPLAY_NAME, typeof(C011) ) },
            { C012.MAGIC, new ItemTypeStruct( C012.DISPLAY_NAME, typeof(C012) ) },
            { C067.MAGIC, new ItemTypeStruct( C067.DISPLAY_NAME, typeof(C067) ) },
            { C053.MAGIC, new ItemTypeStruct( C053.DISPLAY_NAME, typeof(C053) ) },
            { C075.MAGIC, new ItemTypeStruct( C075.DISPLAY_NAME, typeof(C075) ) },
            { C093.MAGIC, new ItemTypeStruct( C093.DISPLAY_NAME, typeof(C093) ) },
            { C009.MAGIC, new ItemTypeStruct( C009.DISPLAY_NAME, typeof(C009) ) },
            { C042.MAGIC, new ItemTypeStruct( C042.DISPLAY_NAME, typeof(C042) ) },
            { C014.MAGIC, new ItemTypeStruct( C014.DISPLAY_NAME, typeof(C014) ) },
            { C015.MAGIC, new ItemTypeStruct( C015.DISPLAY_NAME, typeof(C015) ) },
            { C118.MAGIC, new ItemTypeStruct( C118.DISPLAY_NAME, typeof(C118) ) },
            { C175.MAGIC, new ItemTypeStruct( C175.DISPLAY_NAME, typeof(C175) ) },
            { C174.MAGIC, new ItemTypeStruct( C174.DISPLAY_NAME, typeof(C174) ) },
            { C043.MAGIC, new ItemTypeStruct( C043.DISPLAY_NAME, typeof(C043) ) },
            { C031.MAGIC, new ItemTypeStruct( C031.DISPLAY_NAME, typeof(C031) ) },
            { C094.MAGIC, new ItemTypeStruct( C094.DISPLAY_NAME, typeof(C094) ) },
            { C203.MAGIC, new ItemTypeStruct( C203.DISPLAY_NAME, typeof(C203) ) },
            { C204.MAGIC, new ItemTypeStruct( C204.DISPLAY_NAME, typeof(C204) ) },
            { C198.MAGIC, new ItemTypeStruct( C198.DISPLAY_NAME, typeof(C198) ) },
            { C107.MAGIC, new ItemTypeStruct( C107.DISPLAY_NAME, typeof(C107) ) },
            { C120.MAGIC, new ItemTypeStruct( C120.DISPLAY_NAME, typeof(C120) ) },
            { C125.MAGIC, new ItemTypeStruct( C125.DISPLAY_NAME, typeof(C125) ) },
            { C173.MAGIC, new ItemTypeStruct( C173.DISPLAY_NAME, typeof(C173) ) },
            { C211.MAGIC, new ItemTypeStruct( C211.DISPLAY_NAME, typeof(C211) ) },
            { C187.MAGIC, new ItemTypeStruct( C187.DISPLAY_NAME, typeof(C187) ) },
            { C124.MAGIC, new ItemTypeStruct( C124.DISPLAY_NAME, typeof(C124) ) },
            { C034.MAGIC, new ItemTypeStruct( C034.DISPLAY_NAME, typeof(C034) ) },
            { C088.MAGIC, new ItemTypeStruct( C088.DISPLAY_NAME, typeof(C088) ) },
            { C212.MAGIC, new ItemTypeStruct( C212.DISPLAY_NAME, typeof(C212) ) },
            { C013.MAGIC, new ItemTypeStruct( C013.DISPLAY_NAME, typeof(C013) ) },
            { C117.MAGIC, new ItemTypeStruct( C117.DISPLAY_NAME, typeof(C117) ) },
            { C168.MAGIC, new ItemTypeStruct( C168.DISPLAY_NAME, typeof(C168) ) },
            { C188.MAGIC, new ItemTypeStruct( C188.DISPLAY_NAME, typeof(C188) ) },
            { C142.MAGIC, new ItemTypeStruct( C142.DISPLAY_NAME, typeof(C142) ) },
            { C177.MAGIC, new ItemTypeStruct( C177.DISPLAY_NAME, typeof(C177) ) },
            { C021.MAGIC, new ItemTypeStruct( C021.DISPLAY_NAME, typeof(C021) ) },
            { C139.MAGIC, new ItemTypeStruct( C139.DISPLAY_NAME, typeof(C139) ) },
            { C089.MAGIC, new ItemTypeStruct( C089.DISPLAY_NAME, typeof(C089) ) },
            { C033.MAGIC, new ItemTypeStruct( C033.DISPLAY_NAME, typeof(C033) ) },
            { C178.MAGIC, new ItemTypeStruct( C178.DISPLAY_NAME, typeof(C178) ) },
            { C068.MAGIC, new ItemTypeStruct( C068.DISPLAY_NAME, typeof(C068) ) },
            { C192.MAGIC, new ItemTypeStruct( C192.DISPLAY_NAME, typeof(C192) ) },
            { C197.MAGIC, new ItemTypeStruct( C197.DISPLAY_NAME, typeof(C197) ) },
            { C202.MAGIC, new ItemTypeStruct( C202.DISPLAY_NAME, typeof(C202) ) },
            { C136.MAGIC, new ItemTypeStruct( C136.DISPLAY_NAME, typeof(C136) ) },
            { C083.MAGIC, new ItemTypeStruct( C083.DISPLAY_NAME, typeof(C083) ) },
            { C084.MAGIC, new ItemTypeStruct( C084.DISPLAY_NAME, typeof(C084) ) },
            { C095.MAGIC, new ItemTypeStruct( C095.DISPLAY_NAME, typeof(C095) ) },
            { C100.MAGIC, new ItemTypeStruct( C100.DISPLAY_NAME, typeof(C100) ) },
            { C143.MAGIC, new ItemTypeStruct( C143.DISPLAY_NAME, typeof(C143) ) },
            { C144.MAGIC, new ItemTypeStruct( C144.DISPLAY_NAME, typeof(C144) ) },
            { C161.MAGIC, new ItemTypeStruct( C161.DISPLAY_NAME, typeof(C161) ) },
            { C176.MAGIC, new ItemTypeStruct( C176.DISPLAY_NAME, typeof(C176) ) },
            { C194.MAGIC, new ItemTypeStruct( C194.DISPLAY_NAME, typeof(C194) ) },
            { C215.MAGIC, new ItemTypeStruct( C215.DISPLAY_NAME, typeof(C215) ) },
        };
    }
}

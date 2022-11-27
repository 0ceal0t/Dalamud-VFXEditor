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
        public static readonly Dictionary<string, ItemTypeStruct> ItemTypes = new() {
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

            // TODO: C117 and TMFC in chara/action/mon_sp/m0109/mon_sp014.tmb (Exdeath #14)
            // TODO: C021 (Starcall end)
        };
    }
}

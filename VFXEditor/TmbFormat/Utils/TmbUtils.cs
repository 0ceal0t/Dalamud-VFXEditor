using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.TmbFormat.Entries;

namespace VFXEditor.TmbFormat.Utils {
    public class TmbUtils {
        public static readonly Dictionary<string, Type> ItemTypes = new() {
            { C063.MAGIC, typeof(C063) },
            { C006.MAGIC, typeof(C006) },
            { C010.MAGIC, typeof(C010) },
            { C131.MAGIC, typeof(C131) },
            { C002.MAGIC, typeof(C002) },
            { C011.MAGIC, typeof(C011) },
            { C012.MAGIC, typeof(C012) },
            { C067.MAGIC, typeof(C067) },
            { C053.MAGIC, typeof(C053) },
            { C075.MAGIC, typeof(C075) },
            { C093.MAGIC, typeof(C093) },
            { C009.MAGIC, typeof(C009) },
            { C042.MAGIC, typeof(C042) },
            { C014.MAGIC, typeof(C014) },
            { C015.MAGIC, typeof(C015) },
            { C118.MAGIC, typeof(C118) },
            { C175.MAGIC, typeof(C175) },
            { C174.MAGIC, typeof(C174) },
            { C043.MAGIC, typeof(C043) },
            { C031.MAGIC, typeof(C031) },
            { C094.MAGIC, typeof(C094) },
            { C203.MAGIC, typeof(C203) },
            { C204.MAGIC, typeof(C204) },
            { C198.MAGIC, typeof(C198) },
            { C107.MAGIC, typeof(C107) },
            { C120.MAGIC, typeof(C120) },
            { C125.MAGIC, typeof(C125) },
            { C173.MAGIC, typeof(C173) },
            { C211.MAGIC, typeof(C211) },
            { C187.MAGIC, typeof(C187) },
            { C124.MAGIC, typeof(C124) },
        };
    }
}

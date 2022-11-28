using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public abstract class ScdTrackData : ScdData {
        public abstract void Draw( string parentId );
    }
}

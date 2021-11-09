using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Tmb {
    public abstract class TmbItem {
        public abstract int GetSize();
        public abstract int GetStringSize();
        public abstract int GetExtraSize();
        public abstract string GetName();

        public abstract void Draw( string id );

        public abstract void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos, ref short id );
    }
}

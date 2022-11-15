using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Parsing {
    public interface IParsedBase : IParsedUiBase {
        public void Read( BinaryReader reader, int size );
        public void Write( BinaryWriter writer );
    }
}

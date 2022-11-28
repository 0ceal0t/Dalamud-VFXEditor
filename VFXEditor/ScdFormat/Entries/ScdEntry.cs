using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.ScdFormat {
    public abstract class ScdEntry : ScdData {
        public void Read( BinaryReader reader, int offset ) {
            var oldPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            Read( reader );
            reader.BaseStream.Position = oldPosition;
        }
    }
}

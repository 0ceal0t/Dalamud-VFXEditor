using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.FileManager {
    public abstract class FileManagerFile {
        protected bool Verified = true;

        public bool IsVerified() => Verified;

        public abstract void Draw( string id );

        public abstract void Write( BinaryWriter writer );

        public byte[] ToBytes() {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            Write( writer );
            return ms.ToArray();
        }
    }
}

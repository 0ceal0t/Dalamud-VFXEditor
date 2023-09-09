using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerFile {
        public readonly CommandManager Command;
        public VerifiedStatus Verified = VerifiedStatus.WORKSPACE;

        public FileManagerFile( CommandManager command ) {
            Command = command;
        }

        public abstract void Draw();

        public virtual void Update() { }

        public abstract void Write( BinaryWriter writer );

        public byte[] ToBytes() {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            Write( writer );
            return ms.ToArray();
        }

        public virtual void Dispose() { }
    }
}

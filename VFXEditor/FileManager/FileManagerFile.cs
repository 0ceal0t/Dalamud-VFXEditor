using System.IO;

namespace VfxEditor.FileManager {
    public abstract class FileManagerFile {
        public readonly CommandManager Command;
        protected bool Verified = true;

        public FileManagerFile( CommandManager command ) {
            Command = command;
        }

        public bool IsVerified() => Verified;

        public abstract void Draw( string id );

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

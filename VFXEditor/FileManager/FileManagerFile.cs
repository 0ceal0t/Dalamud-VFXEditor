using System.Collections.Generic;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerFile {
        public readonly CommandManager Command;
        public VerifiedStatus Verified = VerifiedStatus.WORKSPACE;
        public bool Unsaved { get; protected set; } = false;

        public FileManagerFile( CommandManager command = null ) {
            Command = command ?? new( this );
        }

        public abstract void Draw();

        public virtual void OnChange() {
            Unsaved = true;
        }

        public virtual void Update() {
            Unsaved = false;
        }

        public virtual List<string>? GetPapIds() => null;

        public virtual List<short>? GetPapTypes() => null;

        public abstract void Write( BinaryWriter writer );

        public byte[] ToBytes() {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            Write( writer );
            return ms.ToArray();
        }

        public virtual void Dispose() => Command.Dispose();
    }
}

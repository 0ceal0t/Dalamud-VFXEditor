using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidFile : FileManagerFile {
       // public readonly CommandManager Command = new( Data.CopyManager.Eid );

        public EidFile( BinaryReader reader, bool checkOriginal = true ) : base( new CommandManager( Plugin.EidManager.GetCopyManager() ) ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            // ...

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Draw( string id ) {
            // ...
        }

        public override void Write( BinaryWriter writer ) {
            // ...
        }
    }
}

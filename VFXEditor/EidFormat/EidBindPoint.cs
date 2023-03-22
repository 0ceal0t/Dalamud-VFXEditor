using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.EidFormat {
    public class EidBindPoint {
        public int BindPointId => Id.Value;

        private readonly ParsedString Name = new( "Bone Name", "n_root", 31 );
        private readonly ParsedInt Id = new( "Id" );
        private readonly ParsedFloat3 Position = new( "Position" );
        private readonly ParsedFloat3 Rotation = new( "Rotation" );

        public EidBindPoint() { }

        public EidBindPoint( BinaryReader reader ) {
            Name.Read( reader );
            reader.ReadBytes( 32 - Name.Value.Length - 1 ); // name padded to 32 bytes. also account for trailing null
            Id.Read( reader );
            Position.Read( reader );
            Rotation.Read( reader );
            reader.ReadInt32(); // padding
        }

        public void Write(  BinaryWriter writer ) {
            Name.Write( writer );
            for( var i = 0; i < ( 32 - Name.Value.Length - 1 ); i++ ) {
                writer.Write( ( byte )0 );
            }
            Id.Write( writer );
            Position.Write( writer );
            Rotation.Write( writer );
            writer.Write( 0 );
        }

        public void Draw( string parentId ) {
            Name.Draw( parentId, CommandManager.Eid );
            Id.Draw( parentId, CommandManager.Eid );
            Position.Draw( parentId, CommandManager.Eid );
            Rotation.Draw( parentId, CommandManager.Eid );
        }
    }
}

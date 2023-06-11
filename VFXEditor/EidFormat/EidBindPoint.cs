using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.EidFormat {
    public class EidBindPoint {
        public int BindPointId => Id.Value;

        public readonly ParsedString Name = new( "Bone Name", "n_root", 31 );
        public readonly ParsedInt Id = new( "Id" );
        public readonly ParsedFloat3 Position = new( "Position" );
        public readonly ParsedFloat3 Rotation = new( "Rotation" );

        public EidBindPoint() { }

        public EidBindPoint( BinaryReader reader ) {
            Name.Read( reader );
            Name.Pad( reader, 32 );
            Id.Read( reader );
            Position.Read( reader );
            Rotation.Read( reader );
            reader.ReadInt32(); // padding
        }

        public void Write( BinaryWriter writer ) {
            Name.Write( writer );
            Name.Pad( writer, 32 );
            Id.Write( writer );
            Position.Write( writer );
            Rotation.Write( writer );
            writer.Write( 0 );
        }

        public void Draw() {
            Name.Draw( CommandManager.Eid );
            Id.Draw( CommandManager.Eid );
            Position.Draw( CommandManager.Eid );
            Rotation.Draw( CommandManager.Eid );
        }
    }
}

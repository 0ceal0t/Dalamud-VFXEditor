using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.EidFormat {
    public class EidBindPoint : IUiItem {
        public int BindPointId => Id.Value;

        public readonly ParsedPaddedString Name = new( "Bone Name", "n_root", 32, 0x00 );
        public readonly ParsedInt Id = new( "Id" );
        public readonly ParsedFloat3 Position = new( "Position" );
        public readonly ParsedAngle3 Rotation = new( "Rotation" );

        public EidBindPoint() { }

        public EidBindPoint( BinaryReader reader ) {
            Name.Read( reader );
            Id.Read( reader );
            Position.Read( reader );
            Rotation.Read( reader );
            reader.ReadInt32(); // padding
        }

        public void Write( BinaryWriter writer ) {
            Name.Write( writer );
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

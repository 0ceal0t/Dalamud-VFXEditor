using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Angles;
using VfxEditor.Parsing.Float;
using VfxEditor.Parsing.String;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.AtchFormat.Entry {
    public class AtchEntryState : IUiItem {
        public readonly ParsedString Bone = new( "Bone" );
        public readonly ParsedFloat Scale = new( "Scale" );
        public readonly ParsedFloat3 Offset = new( "Offset" );
        public readonly ParsedRadians3 Rotation = new( "Rotation" );

        public AtchEntryState( BinaryReader reader ) {
            var stringPos = reader.ReadUInt32();
            var savePos = reader.BaseStream.Position;

            // Read string
            reader.BaseStream.Position = stringPos;
            Bone.Read( reader );

            // Reset
            reader.BaseStream.Position = savePos;
            Scale.Read( reader );
            Offset.Read( reader );
            Rotation.Read( reader );
        }

        public void Write( BinaryWriter writer, int stringStartPos, BinaryWriter stringWriter, Dictionary<string, int> stringPos ) {
            if( !stringPos.TryGetValue( Bone.Value, out var namePos ) ) {
                // Name not written yet
                namePos = stringStartPos + ( int )stringWriter.BaseStream.Position;
                stringPos[Bone.Value] = namePos;
                Bone.Write( stringWriter );
            }

            writer.Write( namePos );
            Scale.Write( writer );
            Offset.Write( writer );
            Rotation.Write( writer );
        }

        public void Draw() {
            Bone.Draw();
            Scale.Draw();
            Offset.Draw();
            Rotation.Draw();
        }
    }
}
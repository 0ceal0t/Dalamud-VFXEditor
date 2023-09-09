using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
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
            reader.BaseStream.Seek( stringPos, SeekOrigin.Begin );
            Bone.Read( reader );

            // Reset
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            Scale.Read( reader );
            Offset.Read( reader );
            Rotation.Read( reader );
        }

        public void Write( BinaryWriter writer, int stringStartPos, BinaryWriter stringWriter, Dictionary<string, int> stringPos ) {
            if( !stringPos.ContainsKey( Bone.Value ) ) {
                // Name not written yet
                stringPos[Bone.Value] = stringStartPos + ( int )stringWriter.BaseStream.Position;
                Bone.Write( stringWriter );
            }

            writer.Write( stringPos[Bone.Value] );
            Scale.Write( writer );
            Offset.Write( writer );
            Rotation.Write( writer );
        }

        public void Draw() {
            Bone.Draw( CommandManager.Atch );
            Scale.Draw( CommandManager.Atch );
            Offset.Draw( CommandManager.Atch );
            Rotation.Draw( CommandManager.Atch );
        }
    }
}

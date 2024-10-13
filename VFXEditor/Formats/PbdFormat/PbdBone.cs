using System.IO;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdBone : IUiItem {
        // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Data/TransformMatrix.cs#L211

        public readonly ParsedString Name = new( "Name" );
        public readonly ParsedFloat3 Translate = new( "Translate" );
        public readonly ParsedFloat3 Scale = new( "Scale" );
        public readonly ParsedQuat Rotation = new( "Rotation" );

        public TransformMatrix Matrix { get; private set; }

        public PbdBone() { }

        public PbdBone( BinaryReader reader ) {
            var xRow = new Vector4( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            var yRow = new Vector4( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            var zRow = new Vector4( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            Matrix = new TransformMatrix( xRow, yRow, zRow );

            Matrix.TryDecompose( out var scale, out var rot, out var translate );
            Translate.Value = translate;
            Scale.Value = scale;
            Rotation.Quaternion = rot;
        }

        public void Draw() {
            Name.Draw();

            using var edited = new Edited();
            Translate.Draw();
            Scale.Draw();
            Rotation.Draw();

            if( edited.IsEdited ) Matrix = TransformMatrix.Compose( Scale.Value, Rotation.Quaternion, Translate.Value );
        }

        public void Write( BinaryWriter writer ) {
            // xRow
            writer.Write( Matrix.XRow.X );
            writer.Write( Matrix.XRow.Y );
            writer.Write( Matrix.XRow.Z );
            writer.Write( Matrix.XRow.W );
            // yRow
            writer.Write( Matrix.YRow.X );
            writer.Write( Matrix.YRow.Y );
            writer.Write( Matrix.YRow.Z );
            writer.Write( Matrix.YRow.W );
            // zRow
            writer.Write( Matrix.ZRow.X );
            writer.Write( Matrix.ZRow.Y );
            writer.Write( Matrix.ZRow.Z );
            writer.Write( Matrix.ZRow.W );
        }
    }
}

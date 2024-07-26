using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdBone : IUiItem {
        // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Data/TransformMatrix.cs#L211

        public readonly ParsedString Name = new( "Name" );
        public readonly ParsedFloat3 Translate = new( "Translate" );
        public readonly ParsedFloat3 Scale = new( "Scale" );
        public readonly ParsedQuat Rotation = new( "Rotation" );

        public PbdBone() { }

        public PbdBone( BinaryReader reader ) {
            var xRow = new Vector4( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            var yRow = new Vector4( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            var zRow = new Vector4( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );

            var matrix = new Matrix4x4(
                xRow.X, yRow.X, zRow.X, 0.0f,
                xRow.Y, yRow.Y, zRow.Y, 0.0f,
                xRow.Z, yRow.Z, zRow.Z, 0.0f,
                xRow.W, yRow.W, zRow.W, 1.0f );

            Matrix4x4.Decompose( matrix, out var scale, out var rot, out var translate );
            Translate.Value = translate;
            Scale.Value = scale;
            Rotation.SetQuaternion( rot.X, rot.Y, rot.Z, rot.W );
        }

        public void Draw() {
            Name.Draw();
            Translate.Draw();
            Scale.Draw();
            Rotation.Draw();
        }
    }
}

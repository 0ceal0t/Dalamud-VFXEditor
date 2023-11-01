using System.IO;
using System.Numerics;
using VfxEditor.Parsing.HalfFloat;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTableRow : IUiItem {
        public const int Size = 32; // 16 ushorts

        public readonly ParsedHalf3 Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf SpecularStrength = new( "Specular Strength", 1f );
        public readonly ParsedHalf3 Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf GlossStrength = new( "Gloss Strength", 20f );
        public readonly ParsedHalf3 Emissive = new( "Emissive" );
        public readonly ParsedHalf TileSet = new( "Tile Set" );
        public readonly ParsedHalf MaterialRepeatX = new( "Material Repeat X", 16f );
        public readonly ParsedHalf2 MaterialSkew = new( "Material Skew" );
        public readonly ParsedHalf MaterialRepeatY = new( "Material Repeat Y", 16f );

        /*
            public ushort TileSet
            {
                readonly get => (ushort)(ToFloat(11) * 64f);
                set => _data[11] = FromFloat((value + 0.5f) / 64f);
            }
         */

        public MtrlColorTableRow() { }

        public MtrlColorTableRow( BinaryReader reader ) {
            Diffuse.Read( reader );
            SpecularStrength.Read( reader );
            Specular.Read( reader );
            GlossStrength.Read( reader );
            Emissive.Read( reader );
            TileSet.Read( reader );
            MaterialRepeatX.Read( reader );
            MaterialSkew.Read( reader );
            MaterialRepeatY.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Diffuse.Write( writer );
            SpecularStrength.Write( writer );
            Specular.Write( writer );
            GlossStrength.Write( writer );
            Emissive.Write( writer );
            TileSet.Write( writer );
            MaterialRepeatX.Write( writer );
            MaterialSkew.Write( writer );
            MaterialRepeatY.Write( writer );
        }

        public void Draw() {
            Diffuse.Draw();
            SpecularStrength.Draw();
            Specular.Draw();
            GlossStrength.Draw();
            Emissive.Draw();
            TileSet.Draw();
            MaterialRepeatX.Draw();
            MaterialRepeatY.Draw();
            MaterialSkew.Draw();
        }
    }
}

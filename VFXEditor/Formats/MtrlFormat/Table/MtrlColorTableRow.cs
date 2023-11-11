using System;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Parsing.HalfFloat;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTableRow : IUiItem {
        public const int Size = 32; // 16 ushorts

        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf SpecularStrength = new( "Specular Strength", 1f );
        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf GlossStrength = new( "Gloss Strength", 20f );
        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedInt TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf MaterialRepeatX = new( "Material Repeat X", 16f );
        public readonly ParsedHalf2 MaterialSkew = new( "Material Skew" );
        public readonly ParsedHalf MaterialRepeatY = new( "Material Repeat Y", 16f );

        public MtrlColorTableRow() { }

        public MtrlColorTableRow( BinaryReader reader ) {
            Diffuse.Read( reader );
            SpecularStrength.Read( reader );
            Specular.Read( reader );
            GlossStrength.Read( reader );
            Emissive.Read( reader );
            TileMaterial.Value = ( int )( ( float )reader.ReadHalf() * 64f );
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
            writer.Write( ( Half )( ( TileMaterial.Value + 0.5f ) / 64f ) );
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
            TileMaterial.Draw();
            MaterialRepeatX.Draw();
            MaterialRepeatY.Draw();
            MaterialSkew.Draw();
        }
    }
}

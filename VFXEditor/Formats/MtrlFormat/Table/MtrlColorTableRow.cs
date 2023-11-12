using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Parsing.HalfFloat;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTableRow : IUiItem {
        private readonly MtrlFile File;

        public const int Size = 32; // 16 ushorts

        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf SpecularStrength = new( "Specular Strength", 1f );
        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf GlossStrength = new( "Gloss Strength", 20f );
        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedTileMaterial TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf MaterialRepeatX = new( "Material Repeat X", 16f );
        public readonly ParsedHalf2 MaterialSkew = new( "Material Skew" );
        public readonly ParsedHalf MaterialRepeatY = new( "Material Repeat Y", 16f );

        public MtrlColorTableRow( MtrlFile file ) {
            File = file;
        }

        public MtrlColorTableRow( MtrlFile file, BinaryReader reader ) : this( file ) {
            Diffuse.Read( reader );
            SpecularStrength.Read( reader );
            Specular.Read( reader );
            GlossStrength.Read( reader );
            Emissive.Read( reader );
            TileMaterial.Read( reader );
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
            TileMaterial.Write( writer );
            MaterialRepeatX.Write( writer );
            MaterialSkew.Write( writer );
            MaterialRepeatY.Write( writer );
        }

        public void Draw() {
            using var editing = new Edited();

            Diffuse.Draw();
            SpecularStrength.Draw();
            Specular.Draw();
            GlossStrength.Draw();
            Emissive.Draw();
            TileMaterial.Draw();
            MaterialRepeatX.Draw();
            MaterialRepeatY.Draw();
            MaterialSkew.Draw();

            if( Plugin.DirectXManager.MaterialPreview.CurrentColorRow != this || editing.IsEdited ) {
                Plugin.DirectXManager.MaterialPreview.LoadColorRow( File, this );
            }
            Plugin.DirectXManager.MaterialPreview.DrawInline();
        }

        public void Draw( MtrlDyeTableRow dye ) {
            if( dye == null ) {
                Draw();
                return;
            }

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Color" ) ) {
                if( tab ) Draw();
            }

            using( var tab = ImRaii.TabItem( "Dye" ) ) {
                if( tab ) dye.Draw();
            }
        }
    }
}

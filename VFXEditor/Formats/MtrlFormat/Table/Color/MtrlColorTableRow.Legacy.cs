using Dalamud.Interface.Utility.Raii;
using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Table.Color {
    public partial class MtrlColorTableRow {
        private bool ReadLegacy( BinaryReader reader ) {
            if( !Tables.Legacy ) return false;

            Diffuse.Read( reader );
            SpecularStrength.Read( reader );
            Specular.Read( reader );
            GlossStrength.Read( reader );
            Emissive.Read( reader );
            TileMaterial.Read( reader );
            TileRepeatX.Read( reader );
            TileSkew.Read( reader );
            TileRepeatY.Read( reader );

            return true;
        }

        private bool WriteLegacy( BinaryWriter writer ) {
            if( !Tables.Legacy ) return false;

            Diffuse.Write( writer );
            SpecularStrength.Write( writer );
            Specular.Write( writer );
            GlossStrength.Write( writer );
            Emissive.Write( writer );
            TileMaterial.Write( writer );
            TileRepeatX.Write( writer );
            TileSkew.Write( writer );
            TileRepeatY.Write( writer );

            return true;
        }

        private bool DrawTabsLegacy() {
            if( !Tables.Legacy ) return false;

            using( var tab = ImRaii.TabItem( "Color" ) ) {
                if( tab ) {
                    Diffuse.Draw();
                    SpecularStrength.Draw();
                    Specular.Draw();
                    GlossStrength.Draw();
                    Emissive.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Tiling" ) ) {
                if( tab ) {
                    TileMaterial.Draw();
                    TileRepeatX.Draw();
                    TileRepeatY.Draw();
                    TileSkew.Draw();
                }
            }

            return true;
        }
    }
}

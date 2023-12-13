using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils {
    public class OverriddenMember : IUiItem {
        private readonly ParsedInt AssetType = new( "Asset Type" );
        private readonly ParsedIntByte4 Ids = new( "Member Ids" );

        public OverriddenMember() { }

        public void Read( BinaryReader reader ) {
            AssetType.Read( reader );
            Ids.Read( reader );
        }

        public void Draw() {
            AssetType.Draw();
            Ids.Draw();
        }
    }
}

using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils {
    public class WeaponModel : IUiItem {
        private readonly ParsedShort SkeletonId = new( "Skeleton Id" );
        private readonly ParsedShort PatternId = new( "Pattern Id" );
        private readonly ParsedShort ImageChangeId = new( "Image Change Id" );
        private readonly ParsedShort StainingId = new( "Staining Id" );

        public WeaponModel() { }

        public void Read( BinaryReader reader ) {
            SkeletonId.Read( reader );
            PatternId.Read( reader );
            ImageChangeId.Read( reader );
            StainingId.Read( reader );
        }

        public void Draw() {
            SkeletonId.Draw();
            PatternId.Draw();
            ImageChangeId.Draw();
            StainingId.Draw();
        }
    }
}

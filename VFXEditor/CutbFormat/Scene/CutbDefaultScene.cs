using System.IO;

namespace VfxEditor.CutbFormat.Scene {
    public class CutbDefaultScene : CutbHeader {
        public const string MAGIC = "CTDS";
        public override string Magic => MAGIC;

        public CutbDefaultScene( BinaryReader reader ) {

        }

        public override void Draw() {

        }
    }
}

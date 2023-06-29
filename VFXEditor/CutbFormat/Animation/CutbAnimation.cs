using System.IO;

namespace VfxEditor.CutbFormat.Animation {
    public class CutbAnimation : CutbHeader {
        public const string MAGIC = "CTPA";
        public override string Magic => MAGIC;

        public CutbAnimation( BinaryReader reader ) {

        }

        public override void Draw() {

        }
    }
}

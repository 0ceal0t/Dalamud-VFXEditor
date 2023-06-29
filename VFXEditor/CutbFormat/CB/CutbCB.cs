using System.IO;

namespace VfxEditor.CutbFormat.CB {
    public class CutbCB : CutbHeader {
        public const string MAGIC = "CTCB";
        public override string Magic => MAGIC;

        public CutbCB( BinaryReader reader ) {

        }

        public override void Draw() {

        }
    }
}

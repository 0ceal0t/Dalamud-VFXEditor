using System.IO;

namespace VfxEditor.CutbFormat.EX {
    public class CutbEX : CutbHeader {
        public const string MAGIC = "CTEX";
        public override string Magic => MAGIC;

        public CutbEX() {

        }

        public CutbEX( BinaryReader reader ) {

        }

        public override void Draw() {

        }
    }
}

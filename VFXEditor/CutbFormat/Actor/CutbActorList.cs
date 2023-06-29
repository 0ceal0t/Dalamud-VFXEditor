using System.IO;

namespace VfxEditor.CutbFormat.Actor {
    public class CutbActorList : CutbHeader {
        public const string MAGIC = "CTAL";
        public override string Magic => MAGIC;

        public CutbActorList( BinaryReader reader ) {

        }

        public override void Draw() {

        }
    }
}

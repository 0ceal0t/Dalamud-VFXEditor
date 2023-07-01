using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

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

        protected override List<ParsedBase> GetParsed() => new();
    }
}

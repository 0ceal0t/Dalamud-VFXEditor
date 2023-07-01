using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.CutbFormat.CB {
    public class CutbCB : CutbHeader {
        public const string MAGIC = "CTCB";
        public override string Magic => MAGIC;

        public CutbCB( BinaryReader reader ) {

        }

        public override void Draw() {

        }

        protected override List<ParsedBase> GetParsed() => new();
    }
}

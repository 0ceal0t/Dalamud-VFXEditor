using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.CutbFormat.Animation {
    public class CutbAnimation : CutbHeader {
        public const string MAGIC = "CTPA";
        public override string Magic => MAGIC;

        public CutbAnimation( BinaryReader reader ) {

        }

        public override void Draw() {

        }

        protected override List<ParsedBase> GetParsed() => new();
    }
}

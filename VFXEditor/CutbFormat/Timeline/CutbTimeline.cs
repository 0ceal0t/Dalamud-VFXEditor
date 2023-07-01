using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.CutbFormat.Timeline {
    public class CutbTimeline : CutbHeader {
        public const string MAGIC = "CTTL";
        public override string Magic => MAGIC;

        public CutbTimeline() {

        }

        public CutbTimeline( BinaryReader reader ) {

        }

        public override void Draw() {

        }

        protected override List<ParsedBase> GetParsed() => new();
    }
}

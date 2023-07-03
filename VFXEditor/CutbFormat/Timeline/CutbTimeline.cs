using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat;

namespace VfxEditor.CutbFormat.Timeline {
    public class CutbTimeline : CutbHeader {
        public const string MAGIC = "CTTL";
        public override string Magic => MAGIC;

        private readonly CutbFile File;
        private readonly TmbFile Tmb;

        public CutbTimeline( CutbFile file ) {
            File = file;
        }

        public CutbTimeline( BinaryReader reader, CutbFile file ) {
            File = file;
            Tmb = new TmbFile( reader, File.Command, false );
        }

        public override void Draw() {
            Tmb?.Draw();
        }

        protected override List<ParsedBase> GetParsed() => new();
    }
}

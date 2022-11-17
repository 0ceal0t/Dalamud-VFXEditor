using System.Collections.Generic;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class Tmal : TmbItem {
        public override string Magic => "TMAL";
        public override int Size => 0x10;
        public override int ExtraSize => 0;

        public readonly List<Tmac> Actors = new();
        private readonly List<int> TempIds;

        public Tmal( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            TempIds = reader.ReadOffsetTimeline();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.WriteOffsetTimeline( Actors );
        }

        public void PickActors( TmbReader reader ) {
            Actors.AddRange(reader.Pick<Tmac>( TempIds ));
        }
    }
}

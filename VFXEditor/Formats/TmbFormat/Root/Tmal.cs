using System.Collections.Generic;
using VfxEditor.Formats.TmbFormat.Actor;
using VfxEditor.Formats.TmbFormat.Utils;

namespace VfxEditor.Formats.TmbFormat.Root {
    public class Tmal : TmbItem {
        public override string Magic => "TMAL";
        public override int Size => 0x10;
        public override int ExtraSize => 0;

        public readonly List<Tmac> Actors = [];
        private readonly List<int> TempIds;

        public Tmal( TmbFile file, TmbReader reader ) : base( file, reader ) {
            TempIds = reader.ReadOffsetTimeline();
        }

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            writer.WriteOffsetTimeline( Actors );
        }

        public void PickActors( TmbReader reader ) {
            Actors.AddRange( reader.Pick<Tmac>( TempIds ) );
        }
    }
}

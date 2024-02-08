using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing.Data;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxData : AvfxItem, IData {
        protected List<AvfxBase> Parsed;

        public readonly List<AvfxItem> DisplayTabs = [];
        public readonly AvfxDisplaySplitView<AvfxItem> SplitView;

        public AvfxData() : base( "Data" ) {
            SplitView = new AvfxDisplaySplitView<AvfxItem>( "Data", DisplayTabs );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override string GetDefaultText() => "Data";

        public override void Draw() => SplitView.Draw();

        public virtual void Enable() { }

        public virtual void Disable() { }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxData : AvfxItem {
        protected List<AvfxBase> Parsed;

        public readonly List<AvfxItem> DisplayTabs = new();
        public readonly AvfxDisplaySplitView<AvfxItem> SplitView;

        public AvfxData() : base( "Data" ) {
            SplitView = new AvfxDisplaySplitView<AvfxItem>( DisplayTabs );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override string GetDefaultText() => "Data";

        public override void Draw( string parentId ) => SplitView.Draw( $"{parentId}/Data" );

        public virtual void Enable() { }

        public virtual void Disable() { }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxData : AvfxItem {
        protected List<AvfxBase> Children;

        public readonly List<AvfxItem> Tabs = new();
        public readonly AvfxDisplaySplitView<AvfxItem> SplitView;

        public AvfxData() : base( "Data" ) {
            SplitView = new AvfxDisplaySplitView<AvfxItem>( Tabs );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override string GetDefaultText() => "Data";

        public override void Draw( string parentId ) => SplitView.Draw( $"{parentId}/Data" );

        public virtual void Enable() { }

        public virtual void Disable() { }
    }
}

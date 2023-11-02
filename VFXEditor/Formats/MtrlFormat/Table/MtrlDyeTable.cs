using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlDyeTable {
        public const int Size = 16 * MtrlDyeTableRow.Size;

        public readonly List<MtrlDyeTableRow> Rows = new();
        private readonly UiSplitView<MtrlDyeTableRow> RowView;

        public MtrlDyeTable() {
            for( var i = 0; i < 16; i++ ) Rows.Add( new() );
            RowView = new( "Row", Rows, false, false );
        }

        public MtrlDyeTable( BinaryReader reader ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( reader ) );
            RowView = new( "Row", Rows, false, false );
        }

        public void Draw() => RowView.Draw();

        public void Write( BinaryWriter writer ) => Rows.ForEach( x => x.Write( writer ) );
    }
}

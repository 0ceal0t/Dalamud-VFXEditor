using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Timeline {
    public class UldTimelineDropdown : Dropdown<UldTimeline> {
        public UldTimelineDropdown( List<UldTimeline> items ) : base( "Timeline", items, true ) { }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldTimeline>( Items, new UldTimeline() ) );
        }

        protected override void OnDelete( UldTimeline item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldTimeline>( Items, item ) );
        }

        protected override string GetText( UldTimeline item, int idx ) => item.GetText();

        protected override void DrawSelected() => Selected.Draw();
    }
}

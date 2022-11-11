using System.Collections.Generic;
using VfxEditor.AvfxFormat.Views;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineItemSequencer : ImGuiSequencer<UiTimelineItem> {
        public UiTimeline Timeline;

        public UiTimelineItemSequencer( List<UiTimelineItem> items, UiTimeline timeline ) : base( items ) {
            Timeline = timeline;
        }

        public override int GetEnd( UiTimelineItem item ) => item.EndTime.Literal.GetValue();

        public override int GetStart( UiTimelineItem item ) => item.StartTime.Literal.GetValue();

        public override void OnNew() => CommandManager.Avfx.Add( new UiTimelineItemAddCommand( this ) );

        public override void OnDelete( UiTimelineItem item ) => CommandManager.Avfx.Add( new UiTimelineItemRemoveCommand( this, item ) );

        public override bool IsEnabled( UiTimelineItem item ) => item.Enabled.Literal.GetValue() == true;

        public override void Toggle( UiTimelineItem item ) => CommandManager.Avfx.Add( new UiCheckboxCommand( item.Enabled.Literal, !IsEnabled( item ) ) );

        public override void SetPos( UiTimelineItem item, int start, int end ) {
            item.StartTime.Literal.SetValue( start );
            item.EndTime.Literal.SetValue( end );
        }

        public override void OnDragEnd( UiTimelineItem item, int startBegin, int startFinish, int endBegin, int endFinish ) {
            if( startBegin == startFinish && endBegin == endFinish ) return;
            CommandManager.Avfx.Add( new UiTimelineItemDragCommand( item, startBegin, startFinish, endBegin, endFinish ) );
        }
    }
}

using System;
using System.Collections.Generic;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class UiTimelineItemSequencer : ImGuiSequencer<AvfxTimelineSubItem> {
        public AvfxTimeline Timeline;

        public UiTimelineItemSequencer( List<AvfxTimelineSubItem> items, AvfxTimeline timeline ) : base( items ) {
            Timeline = timeline;
        }

        public override int GetEnd( AvfxTimelineSubItem item ) => item.EndTime.GetValue();

        public override int GetStart( AvfxTimelineSubItem item ) => item.StartTime.GetValue();

        public override void OnNew() => CommandManager.Avfx.Add( new UiTimelineItemAddCommand( this ) );

        public override void OnDelete( AvfxTimelineSubItem item ) => CommandManager.Avfx.Add( new UiTimelineItemRemoveCommand( this, item ) );

        public override bool IsEnabled( AvfxTimelineSubItem item ) => item.Enabled.GetValue() == true;

        public override void Toggle( AvfxTimelineSubItem item ) => CommandManager.Avfx.Add( new ParsedBoolCommand( item.Enabled.Parsed, !IsEnabled( item ) ) );

        public override void SetPos( AvfxTimelineSubItem item, int start, int end ) {
            item.StartTime.SetValue( start );
            item.EndTime.SetValue( end );
        }

        public override void OnDragEnd( AvfxTimelineSubItem item, int startBegin, int startFinish, int endBegin, int endFinish ) {
            if( startBegin == startFinish && endBegin == endFinish ) return;
            CommandManager.Avfx.Add( new UiTimelineItemDragCommand( item, startBegin, startFinish, endBegin, endFinish ) );
        }
    }
}

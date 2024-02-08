using System.Collections.Generic;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class UiTimelineItemSequencer : ImGuiSequencer<AvfxTimelineItem> {
        public AvfxTimeline Timeline;

        public UiTimelineItemSequencer( List<AvfxTimelineItem> items, AvfxTimeline timeline ) : base( items ) {
            Timeline = timeline;
        }

        public override int GetEnd( AvfxTimelineItem item ) => item.EndTime.Value;

        public override int GetStart( AvfxTimelineItem item ) => item.StartTime.Value;

        public override void OnNew() => CommandManager.Add( new UiTimelineItemAddCommand( this ) );

        public override void OnDelete( AvfxTimelineItem item ) => CommandManager.Add( new UiTimelineItemRemoveCommand( this, item ) );

        public override bool IsEnabled( AvfxTimelineItem item ) => item.Enabled.Value == true;

        public override void Toggle( AvfxTimelineItem item ) => CommandManager.Add( new ParsedSimpleCommand<bool?>( item.Enabled.Parsed, !IsEnabled( item ) ) );

        public override void SetPos( AvfxTimelineItem item, int start, int end ) {
            item.StartTime.Value = start;
            item.EndTime.Value = end;
        }

        public override void OnDragEnd( AvfxTimelineItem item, int prevStart, int start, int prevEnd, int end ) {
            if( prevStart == start && prevEnd == end ) return;
            CommandManager.Add( new UiTimelineItemDragCommand( item, prevStart, start, prevEnd, end ) );
        }

        public override void OnDoubleClick( AvfxTimelineItem item ) {
            var file = Plugin.AvfxManager.File;
            file.SelectItem( file.EmitterView, item.Emitter );
        }
    }
}

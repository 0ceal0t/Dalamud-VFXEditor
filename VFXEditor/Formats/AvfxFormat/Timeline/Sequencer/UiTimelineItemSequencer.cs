using System.Collections.Generic;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class UiTimelineItemSequencer : ImGuiSequencer<AvfxTimelineItem> {
        public AvfxTimeline Timeline;

        public UiTimelineItemSequencer( List<AvfxTimelineItem> items, AvfxTimeline timeline ) : base( items ) {
            Timeline = timeline;
        }

        public override int GetEnd( AvfxTimelineItem item ) => item.EndTime.GetValue();

        public override int GetStart( AvfxTimelineItem item ) => item.StartTime.GetValue();

        public override void OnNew() => CommandManager.Avfx.Add( new UiTimelineItemAddCommand( this ) );

        public override void OnDelete( AvfxTimelineItem item ) => CommandManager.Avfx.Add( new UiTimelineItemRemoveCommand( this, item ) );

        public override bool IsEnabled( AvfxTimelineItem item ) => item.Enabled.GetValue() == true;

        public override void Toggle( AvfxTimelineItem item ) => CommandManager.Avfx.Add( new ParsedSimpleCommand<bool?>( item.Enabled.Parsed, !IsEnabled( item ) ) );

        public override void SetPos( AvfxTimelineItem item, int start, int end ) {
            item.StartTime.SetValue( start );
            item.EndTime.SetValue( end );
        }

        public override void OnDragEnd( AvfxTimelineItem item, int startBegin, int startFinish, int endBegin, int endFinish ) {
            if( startBegin == startFinish && endBegin == endFinish ) return;
            CommandManager.Avfx.Add( new UiTimelineItemDragCommand( item, startBegin, startFinish, endBegin, endFinish ) );
        }

        public override void OnDoubleClick( AvfxTimelineItem item ) {
            var file = Plugin.AvfxManager.CurrentFile;
            file.SelectItem( file.EmitterView, item.Emitter );
        }
    }
}

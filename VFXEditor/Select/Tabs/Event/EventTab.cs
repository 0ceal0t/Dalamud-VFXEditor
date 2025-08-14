using System.IO;
using System.Linq;

namespace VfxEditor.Select.Tabs.Event {
    public class EventTab : SelectTab<EventRow> {
        public EventTab( SelectDialog dialog, string name ) : base( dialog, name, "Event" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var idx = 0;
            foreach( var line in File.ReadLines( SelectDataUtils.CommonPapPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                if ( line.Contains( "bt_common/event/" ) ) {
                    Items.Add( new EventRow( idx++, line, line.Replace( "chara/human/", "" ).Replace( "/animation/", ", " ).Replace( "/bt_common/event/event_", " -  " ).Replace( "/bt_common/event_base/mount_talk_hiroshi", " - mount_talk_hiroshi" ).Replace( ".pap", "" )) ); //accounting for this one weird case
                }
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Selected.Path, Selected.Name, SelectResultType.GameMisc );
        }
    }
}
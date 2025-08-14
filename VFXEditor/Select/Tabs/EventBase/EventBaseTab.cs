using System.IO;
using System.Linq;

namespace VfxEditor.Select.Tabs.EventBase {
    public class EventBaseTab : SelectTab<EventBaseRow> {
        public EventBaseTab( SelectDialog dialog, string name ) : base( dialog, name, "EventBase" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var idx = 0;
            foreach( var line in File.ReadLines( SelectDataUtils.CommonPapPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                if( line.Contains( "bt_common/event_base/" ) ) {
                    Items.Add( new EventBaseRow( idx++, line, line.Replace( "chara/human/", "" ).Replace( "/animation/", ", " ).Replace( "/bt_common/event_base/event_base_", " -  " ).Replace( "/bt_common/event_base/even_base_", " - " ).Replace( "/bt_common/event_base/mount_hiroshi", " - mount_hiroshi" ).Replace( ".pap", "" ) ) ); //accounting for outliers
                }
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Selected.Path, Selected.Name, SelectResultType.GameMisc );
        }
    }
}
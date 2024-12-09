using System.Linq;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public static class AvfxWarning {

        public static string GetWarningText( AvfxFile File ) {
            string text = "";
            var list = GetWarningList( File );
            foreach( var item in list )
            {
                if(item != "") text += item + "\n";
            }
            return text;
        }

        static List<string> GetWarningList( AvfxFile File )
        {
            var list = new List<string>();
            list.Add( GetWarningTextScheduler( File ) );
            list.Add( GetWarningTextTimeline( File ) );
            return list;
        }

        static string GetWarningTextScheduler( AvfxFile File )
        {
            var invalidScheduler = File.ScheduleView.Group.Items.Where( scheduler => scheduler.Items.Any( item => !item.HasValue ) ).FirstOrDefault();
            if( invalidScheduler != null ) return $"Scheduler [{invalidScheduler.GetText()}] is Missing a Value";
            return "";
        }

        static string GetWarningTextTimeline( AvfxFile File )
        {
            string text = "";
            foreach(var timeline in File.TimelineView.Group.Items )
            {
                if( timeline.Items.Any( item => !item.HasValue ) )
                {
                    if(text != "") { text += "\n"; }
                    text += $"Timeline [{timeline.GetText()}] is Missing a Value";
                }
            }
            return text;
        }
    }
}

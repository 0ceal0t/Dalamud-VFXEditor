using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Pap.Emote {
    public enum EmoteRowType {
        Normal,
        Facial,
        PerJob
    }

    public class EmoteRowItem {
        public readonly EmoteRowType Type;
        public readonly string Key;

        public EmoteRowItem( EmoteRowType type, string key ) {
            Type = type;
            Key = key;
        }
    }

    public class EmoteRow {
        public readonly int RowId;
        public readonly string Name;
        public readonly ushort Icon;
        public readonly string Command;

        public readonly List<EmoteRowItem> PapFiles;

        public EmoteRow( Lumina.Excel.GeneratedSheets.Emote emote ) {
            RowId = ( int )emote.RowId;
            Name = emote.Name;
            Icon = emote.Icon;
            Command = emote.TextCommand.Value?.Command.ToString() ?? "";

            var emoteItems = emote.ActionTimeline.Select( x => ToPap( x.Value ) ).Where( x => x != null );
            PapFiles = emoteItems.GroupBy( x => x.Key ).Select( x => x.First() ).ToList(); // dedup by key
        }

        private static EmoteRowItem ToPap( Lumina.Excel.GeneratedSheets.ActionTimeline timeline ) {
            if( timeline == null ) return null;
            var key = timeline?.Key.ToString();
            if( string.IsNullOrEmpty( key ) ) return null;

            var loadType = timeline.LoadType;
            if( loadType == 2 ) return new EmoteRowItem( EmoteRowType.PerJob, key );
            else if( loadType == 1 ) return new EmoteRowItem( EmoteRowType.Normal, key );
            else if( loadType == 0 ) {
                if( key.StartsWith( "facial/pose/" ) ) return new EmoteRowItem( EmoteRowType.Facial, key.Replace( "facial/pose/", "" ) );
                return new EmoteRowItem( EmoteRowType.Normal, key );
            }

            return null;
        }
    }
}

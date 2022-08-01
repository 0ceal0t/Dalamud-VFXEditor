using System.Collections.Generic;
using System.Linq;

namespace VFXEditor.Select.Rows {
    public class XivEmotePap {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;

        public readonly List<XivEmotePapItem> PapFiles;

        public XivEmotePap( Lumina.Excel.GeneratedSheets.Emote emote ) {
            RowId = ( int )emote.RowId;
            Icon = emote.Icon;
            Name = emote.Name.ToString();

            var emoteItems = emote.ActionTimeline.Select( x => ToPap( x.Value ) ).Where( x => x != null );
            PapFiles = emoteItems.GroupBy( x => x.Key ).Select( x => x.First() ).ToList(); // dedup by key
        }

        private static XivEmotePapItem ToPap( Lumina.Excel.GeneratedSheets.ActionTimeline timeline ) {
            if( timeline == null ) return null;
            var key = timeline?.Key.ToString();
            if( string.IsNullOrEmpty( key ) ) return null;

            var loadType = timeline.LoadType;
            if( loadType == 2 ) {
                return new XivEmotePapItem( XivEmotePapType.PerJob, key );
            }
            else if( loadType == 1 ) {
                return new XivEmotePapItem( XivEmotePapType.Normal, key );
            }
            else if( loadType == 0 ) {
                if( key.StartsWith( "facial/pose/" ) ) {
                    return new XivEmotePapItem( XivEmotePapType.Facial, key.Replace( "facial/pose/", "" ) );
                }
                return new XivEmotePapItem( XivEmotePapType.Normal, key );
            }

            return null;
        }

        // ========================

        public enum XivEmotePapType {
            Normal,
            Facial,
            PerJob
        }

        public class XivEmotePapItem {
            public readonly XivEmotePapType Type;
            public readonly string Key;

            public XivEmotePapItem( XivEmotePapType type, string key ) {
                Type = type;
                Key = key;
            }
        }
    }
}

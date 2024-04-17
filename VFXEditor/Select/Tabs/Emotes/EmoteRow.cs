using Lumina.Excel.GeneratedSheets2;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Base;
using VfxEditor.Select.Tabs.Actions;

namespace VfxEditor.Select.Tabs.Emotes {
    public enum EmoteRowType {
        Normal,
        Facial,
        PerJob
    }

    public class EmoteRow : ISelectItemWithIcon {
        public readonly int RowId;
        public readonly ushort Icon;
        public readonly string Name;

        public readonly List<(string, byte)> Keys;
        public readonly string Command;

        public List<string> TmbFiles => Keys.Select( x => ActionRow.ToTmbPath( x.Item1 ) ).ToList();

        public List<string> VfxPapFiles => Keys.Where( x => x.Item1.Contains( "emote_sp" ) ).Select( x => $"chara/human/c0101/animation/a0001/bt_common/{x.Item1}.pap" ).ToList();

        public List<(string, EmoteRowType)> Items => Keys.Select( ToPap ).Where( x => !string.IsNullOrEmpty( x.Item1 ) ).GroupBy( x => x.Item1 ).Select( x => x.First() ).ToList();

        public EmoteRow( Emote emote ) {
            RowId = ( int )emote.RowId;
            Icon = emote.Icon;
            Name = emote.Name.ToString();

            Keys = emote.ActionTimeline.Where( x => !string.IsNullOrEmpty( x?.Value?.Key.ToString() ) ).Select( x => (x.Value.Key.ToString(), x.Value.LoadType) ).ToList();
            Command = emote.TextCommand.Value?.Command.ToString() ?? "";
        }

        private static (string, EmoteRowType) ToPap( (string, byte) item ) {
            var key = item.Item1;
            if( string.IsNullOrEmpty( key ) ) return (null, 0);

            var loadType = item.Item2;
            if( loadType == 2 ) return (key, EmoteRowType.PerJob);
            else if( loadType == 1 ) return (key, EmoteRowType.Normal);
            else if( loadType == 0 ) {
                if( key.StartsWith( "facial/pose/" ) ) return (key.Replace( "facial/pose/", "" ), EmoteRowType.Facial);
                return (key, EmoteRowType.Normal);
            }

            return (null, 0);
        }

        public string GetName() => Name;

        public uint GetIconId() => Icon;
    }
}
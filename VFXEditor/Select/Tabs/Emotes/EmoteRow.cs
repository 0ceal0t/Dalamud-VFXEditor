using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Base;
using VfxEditor.Select.Tabs.Actions;

namespace VfxEditor.Select.Tabs.Emotes {
    public enum EmoteRowType {
        PerJob = 1,
        Facial = 0,
        Normal = 2
    }

    public class EmoteRow : ISelectItemWithIcon {
        public readonly int RowId;
        public readonly ushort Icon;
        public readonly string Name;

        public readonly List<(string, byte)> Keys;
        public readonly string Command;

        public List<string> TmbFiles => Keys.Select( x => ActionRow.ToTmbPath( x.Item1 ) ).Distinct().ToList();

        public List<string> VfxPapFiles => Keys.Where( x => x.Item1.Contains( "emote_sp" ) ).Select( x => $"chara/human/c0101/animation/a0001/bt_common/{x.Item1}.pap" ).ToList();

        public List<(string, EmoteRowType)> Items => Keys.Select( ToPap ).Where( x => !string.IsNullOrEmpty( x.Item1 ) ).GroupBy( x => x.Item1 ).Select( x => x.First() ).ToList();

        public EmoteRow( Emote emote ) {
            RowId = ( int )emote.RowId;
            Icon = ( ushort )( emote.Icon == 64350 ? 405 : emote.Icon );
            Name = emote.Name.ToString();

            Keys = emote.ActionTimeline.Where( x => !string.IsNullOrEmpty( x.ValueNullable?.Key.ToString() ) ).Select( x => (x.Value.Key.ToString(), x.Value.LoadType) ).ToList();
            Command = emote.TextCommand.ValueNullable?.Command.ToString() ?? "";
        }

        private static (string, EmoteRowType) ToPap( (string, byte) item ) {
            var key = item.Item1;
            if( string.IsNullOrEmpty( key ) ) return (null, 0);

            var type = ( EmoteRowType )item.Item2;
            return type switch {
                EmoteRowType.Normal or EmoteRowType.PerJob => (key, type),
                EmoteRowType.Facial => key.StartsWith( "facial/pose/" ) ?
                    (key.Replace( "facial/pose/", "" ), EmoteRowType.Facial) :
                    (key, EmoteRowType.Normal),
                _ => (null, 0)
            };
        }

        public string GetName() => Name;

        public uint GetIconId() => Icon;
    }
}
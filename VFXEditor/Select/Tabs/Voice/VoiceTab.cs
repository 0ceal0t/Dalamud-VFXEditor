using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Voice {
    public class VoiceTab : SelectTab<VoiceRow, List<string>> {
        private static readonly Dictionary<string, string> Races = new() {
            { "Au Ra", "aur" },
            { "Elezen", "ele" },
            { "Highlander", "hil" },
            { "Lalafell", "lal" },
            { "Midlander", "mid" },
            { "Miqo'te", "miq" },
            { "Roegadyn", "rog" },
            { "Hrothgar", "ros" },
            { "Viera", "vie" }
        };

        private static readonly List<string> Languages = new( [
            "ja",
            "en",
            "de",
            "fr"
        ] );

        private static readonly List<string> Codes = new( [
            "a",
            "b",
            "c"
        ] );

        public VoiceTab( SelectDialog dialog, string name ) : base( dialog, name, "Voice" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var lineIdx = 0;
            foreach( var race in Races ) Items.Add( new( lineIdx++, race.Value, race.Key ) );
        }

        public override void LoadSelection( VoiceRow item, out List<string> loaded ) {
            var possiblePaths = new List<string>();
            foreach( var lang in Languages ) {
                foreach( var code in Codes ) {
                    // Add M + F
                    possiblePaths.Add( $"sound/voice/vo_battle/vo_battle_pc_{item.Id}_f{code}_{lang}.scd" );
                    possiblePaths.Add( $"sound/voice/vo_battle/vo_battle_pc_{item.Id}_m{code}_{lang}.scd" );
                }
            }

            loaded = possiblePaths.Where( Dalamud.DataManager.FileExists ).ToList();
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Loaded, Selected.Name, SelectResultType.GameCharacter );
        }
    }
}

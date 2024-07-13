using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Emotes {
    public abstract class EmoteTab<T> : SelectTab<EmoteRow, T> where T : class {
        public EmoteTab( SelectDialog dialog, string name ) : base( dialog, name, "Emote" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in Dalamud.DataManager.GetExcelSheet<Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) ) ) {
                var emote = new EmoteRow( item );
                if( emote.Keys.Count > 0 ) Items.Add( emote );
            }
        }

        // ===== DRAWING ======

        protected override bool CheckMatch( EmoteRow item, string searchInput ) => base.CheckMatch( item, SearchInput ) || SelectUiUtils.Matches( item.Command, searchInput );
    }
}
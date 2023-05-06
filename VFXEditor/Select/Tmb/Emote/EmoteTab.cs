using System.Linq;

namespace VfxEditor.Select.Tmb.Emote {
    public class EmoteTab : SelectTab<EmoteRow> {
        public EmoteTab( SelectDialog dialog, string name ) : base( dialog, name, "Tmb-Emote" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) Items.Add( new EmoteRow( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPaths( "Path", Selected.TmbFiles, parentId, SelectResultType.GameEmote, Selected.Name, true );
        }

        protected override string GetName( EmoteRow item ) => item.Name;
    }
}

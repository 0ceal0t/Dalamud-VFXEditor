using System.Linq;
using VfxEditor.Select2.Shared;

namespace VfxEditor.Select2.Vfx.Emote {
    public class EmoteTab : SelectTab<EmoteRow, ParseAvfxFromFile> {
        public EmoteTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        // ===== LOADING =====

        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) {
                var emoteItem = new EmoteRow( item );
                if( emoteItem.PapFiles.Count > 0 ) Items.Add( emoteItem );
            }
        }

        public override void SelectItem( EmoteRow item, out ParseAvfxFromFile loaded ) => ParseAvfxFromFile.ReadFile( item.PapFiles, out loaded );

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameEmote, Selected.Name, true );
        }

        protected override string GetName( EmoteRow item ) => item.Name;
    }
}

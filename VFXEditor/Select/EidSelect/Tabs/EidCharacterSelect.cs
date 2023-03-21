using VfxEditor.Select.Rows;
using VfxEditor.Select.Sheets;

namespace VfxEditor.Select.EidSelect {
    public class EidCharacterSelect : SelectTab<XivCharacterEid> {
        public EidCharacterSelect( string tabId, EidSelectDialog dialog ) : base( tabId, SheetManager.CharacterEid, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            DrawPath( "Path", Selected.Path, parentId, SelectResultType.GameNpc, Selected.Name );
        }

        protected override string GetName( XivCharacterEid item ) => item.Name;
    }
}

using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxGimmickSelect : SelectTab<XivGimmick, XivGimmickSelected> {
        public VfxGimmickSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Gimmicks, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            if( Loaded.VfxExists ) {
                ImGui.Text( "TMB:" );
                ImGui.SameLine();
                DisplayPath( Loaded.TmbPath );
                Copy( Loaded.TmbPath, id: $"{parentId}/CopyTmb" );

                DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameGimmick, Loaded.Gimmick.Name, true );
            }
        }

        protected override string GetName( XivGimmick item ) => item.Name;
    }
}

using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxActionSelect : SelectTab<XivActionBase, XivActionSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxActionSelect( string tabId, VfxSelectDialog dialog, bool nonPlayer = false ) : base( tabId, !nonPlayer ? SheetManager.Actions : SheetManager.NonPlayerActions, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            if( Loaded.SelfVfxExists ) {
                Copy( Loaded.SelfTmbPath, $"{parentId}/CopyTmb" );
                ImGui.SameLine();
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                DisplayPath( Loaded.SelfTmbPath );
            }

            DrawPath( "Cast VFX Path", Loaded.CastVfxPath, $"{parentId}/Cast", SelectResultType.GameAction, Loaded.Action.Name + " Cast", true );

            if( Loaded.SelfVfxExists ) {
                DrawPath( "VFX", Loaded.SelfVfxPaths, parentId, SelectResultType.GameAction, Loaded.Action.Name, true );
            }
        }

        protected override string GetName( XivActionBase item ) => item.Name;
    }
}
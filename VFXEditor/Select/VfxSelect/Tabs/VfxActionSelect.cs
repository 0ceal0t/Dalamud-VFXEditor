using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxActionSelect : SelectTab<XivAction, XivActionSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxActionSelect( string tabId, VfxSelectDialog dialog, bool nonPlayer = false ) : base( tabId, !nonPlayer ? SheetManager.Actions : SheetManager.NonPlayerActions, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            if( !string.IsNullOrEmpty( Loaded.SelfTmbPath ) ) {
                Copy( Loaded.SelfTmbPath, $"{parentId}/CopyTmb" );
                ImGui.SameLine();
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                DisplayPath( Loaded.SelfTmbPath );
            }

            DrawPath( "Cast VFX Path", Loaded.CastVfxPath, $"{parentId}/Cast", SelectResultType.GameAction, Loaded.Action.Name + " Cast", true );

            if( !string.IsNullOrEmpty( Loaded.SelfTmbPath ) ) {
                DrawPath( "VFX", Loaded.SelfVfxPaths, parentId, SelectResultType.GameAction, Loaded.Action.Name, true );
            }
        }

        protected override string GetName( XivAction item ) => item.Name;
    }
}

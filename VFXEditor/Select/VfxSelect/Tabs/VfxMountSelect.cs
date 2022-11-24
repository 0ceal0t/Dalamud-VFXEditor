using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxMountSelect : SelectTab<XivMount, XivMountSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxMountSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Mounts, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            ImGui.Text( "Variant: " + Loaded.Mount.Variant );
            ImGui.Text( "IMC Path: " );
            ImGui.SameLine();
            DisplayPath( Loaded.ImcPath );

            DrawPath( "VFX Path", Loaded.GetVFXPath(), parentId, SelectResultType.GameNpc, Loaded.Mount.Name, true );
        }

        protected override string GetName( XivMount item ) => item.Name;
    }
}
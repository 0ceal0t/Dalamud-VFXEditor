using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxItemSelect : SelectTab<XivItem, XivItemSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxItemSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Items, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            ImGui.Text( "Variant: " + Loaded.Item.Variant );
            ImGui.Text( "IMC Path: " );
            ImGui.SameLine();
            DisplayPath( Loaded.ImcPath );

            DrawPath( "VFX Path", Loaded.GetVfxPaths(), parentId, SelectResultType.GameItem, Loaded.Item.Name, true );

            if( !Loaded.VfxExists ) DisplayNoVfx();
        }

        protected override string GetName( XivItem item ) => item.Name;
    }
}
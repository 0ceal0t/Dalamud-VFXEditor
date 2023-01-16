using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxHousingSelect : SelectTab<XivHousing, XivHousingSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxHousingSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Housing, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            ImGui.Text( "SGB:" );
            ImGui.SameLine();
            DisplayPath( Loaded.Housing.SgbPath );

            DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameItem, Loaded.Housing.Name, true );
        }

        protected override string GetName( XivHousing item ) => item.Name;
    }
}
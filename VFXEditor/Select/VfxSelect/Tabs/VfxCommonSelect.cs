using ImGuiNET;
using System;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxCommonSelect : SelectTab<XivCommon, XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public VfxCommonSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Common, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawPath( "VFX Path", Loaded.Path, parentId, SelectResultType.GameAction, Loaded.Name, true );
        }

        protected override string GetName( XivCommon item ) => item.Name;
    }
}
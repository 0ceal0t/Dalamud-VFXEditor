using ImGuiNET;
using System;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxCommonSelect : SelectTab<XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public VfxCommonSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Common, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawPath( "Path", Selected.Path, parentId, SelectResultType.GameAction, Selected.Name, true );
        }

        protected override string GetName( XivCommon item ) => item.Name;
    }
}
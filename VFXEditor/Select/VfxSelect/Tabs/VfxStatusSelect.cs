using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxStatusSelect : SelectTab<XivStatus, XivStatus> {
        private ImGuiScene.TextureWrap Icon;

        public VfxStatusSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Statuses, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            DrawPath( "Hit VFX", Loaded.HitVFXPath, $"{parentId}/Hit", SelectResultType.GameStatus, $"{Loaded.Name} Hit", true );
            DrawPath( "Loop VFX 1", Loaded.LoopVFXPath1, $"{parentId}/Loop1", SelectResultType.GameStatus, $"{Loaded.Name} Loop 1", true );
            DrawPath( "Loop VFX 2", Loaded.LoopVFXPath2, $"{parentId}/Loop2", SelectResultType.GameStatus, $"{Loaded.Name} Loop 2", true );
            DrawPath( "Loop VFX 3", Loaded.LoopVFXPath3, $"{parentId}/Loop3", SelectResultType.GameStatus, $"{Loaded.Name} Loop 3", true );
        }

        protected override string GetName( XivStatus item ) => item.Name;
    }
}
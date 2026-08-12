using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public partial class SklbDocument : FileManagerHavokDocument<SklbFile> {
        public override string Id => "Sklb";
        public override string Extension => "sklb";

        public SklbDocument( SklbManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public SklbDocument( SklbManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override SklbFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, HkxTemp, Plugin.State != WorkspaceState.Loading, verify );

        protected override void DrawExtraColumn() {
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 126 - 28 - ImGui.GetStyle().FramePadding.X );
            Plugin.TrackerManager.Sklb.DrawEye( new Vector2( 28, ImGui.GetFrameHeight() * 2 + ImGui.GetStyle().ItemSpacing.Y ) );
        }
    }
}

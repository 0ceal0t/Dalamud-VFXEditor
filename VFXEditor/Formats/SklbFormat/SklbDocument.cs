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
            var space = ImGui.GetContentRegionAvail();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( space.X - 30 ) );
            Plugin.TrackerManager.Sklb.DrawEye( new Vector2( 30, ImGui.GetFrameHeight() * 2 + ImGui.GetStyle().ItemSpacing.Y ) );
        }
    }
}
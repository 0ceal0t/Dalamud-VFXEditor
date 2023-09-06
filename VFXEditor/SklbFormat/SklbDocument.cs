using ImGuiNET;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public partial class SklbDocument : FileManagerDocument<SklbFile, WorkspaceMetaBasic> {
        private string HkxTemp => WriteLocation.Replace( ".sklb", "_temp.hkx" );

        public SklbDocument( SklbManager manager, string writeLocation ) : base( manager, writeLocation, "Sklb", "sklb" ) { }

        public SklbDocument( SklbManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace, bool disabled ) :
                base( manager, writeLocation, localPath, name, source, replace, disabled, "Sklb", "sklb" ) { }

        protected override SklbFile FileFromReader( BinaryReader reader ) => new( reader, HkxTemp );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };

        public override void Dispose() {
            base.Dispose();
            File.Delete( HkxTemp );
        }

        protected override void DrawExtraColumn() {
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 126 - 28 - ImGui.GetStyle().FramePadding.X );
            Plugin.Tracker.Sklb.DrawEye( new Vector2( 28, ImGui.GetFrameHeight() * 2 + ImGui.GetStyle().ItemSpacing.Y ) );
        }
    }
}

using ImGuiNET;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public partial class SklbDocument : FileManagerDocument<SklbFile, WorkspaceMetaBasic> {
        public override string Id => "Sklb";
        public override string Extension => "sklb";

        private string HkxTemp => WriteLocation.Replace( ".sklb", "_temp.hkx" );

        public SklbDocument( SklbManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public SklbDocument( SklbManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override SklbFile FileFromReader( BinaryReader reader ) => new( reader, HkxTemp, true );

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
            Plugin.TrackerManager.Sklb.DrawEye( new Vector2( 28, ImGui.GetFrameHeight() * 2 + ImGui.GetStyle().ItemSpacing.Y ) );
        }
    }
}

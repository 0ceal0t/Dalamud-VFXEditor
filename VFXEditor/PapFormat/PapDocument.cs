using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public partial class PapDocument : FileManagerDocument<PapFile, WorkspaceMetaBasic> {
        private string HkxTemp => WriteLocation.Replace( ".pap", "_temp.hkx" );

        public PapDocument( PapManager manager, string writeLocation ) : base( manager, writeLocation, "Pap", "pap" ) { }

        public PapDocument( PapManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace ) :
            base( manager, writeLocation, localPath, name, source, replace, "Pap", "pap" ) { }

        protected override List<string> GetPapIds() => CurrentFile.GetPapIds();

        protected override PapFile FileFromReader( BinaryReader reader ) => new( reader, Source.Path, HkxTemp );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        public override void Dispose() {
            base.Dispose();
            File.Delete( HkxTemp );
        }

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DisplayAnimationWarning();
            base.DrawBody();
        }

        protected override void DrawExtraColumn() {
            var iconSize = UiUtils.GetIconSize( FontAwesomeIcon.InfoCircle );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 126 - iconSize.X - ImGui.GetStyle().FramePadding.X );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + ImGui.GetFrameHeight() / 2 + ImGui.GetStyle().ItemSpacing.Y );
            UiUtils.HelpMarker( "Loaded .pap resources can be found in File > Tools > Loaded Files" );
        }
    }
}

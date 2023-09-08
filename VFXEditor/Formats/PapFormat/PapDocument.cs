using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public partial class PapDocument : FileManagerDocument<PapFile, WorkspaceMetaBasic> {
        public override string Id => "Pap";
        public override string Extension => "pap";

        private string HkxTemp => WriteLocation.Replace( ".pap", "_temp.hkx" );

        public PapDocument( PapManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public PapDocument( PapManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override List<string> GetPapIds() => CurrentFile.GetPapIds();

        protected override PapFile FileFromReader( BinaryReader reader ) => new( reader, Source.Path, HkxTemp );

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

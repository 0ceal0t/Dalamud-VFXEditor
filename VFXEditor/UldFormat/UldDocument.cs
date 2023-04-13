using ImGuiNET;
using System;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public class UldDocument : FileManagerDocument<UldFile, WorkspaceMetaRenamed> {
        public UldDocument( UldManager manager, string writeLocation ) : base( manager, writeLocation, "Uld", "uld" ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, SelectResult source, SelectResult replace ) :
            base( manager, writeLocation, localPath, source, replace, "Uld", "uld" ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) : this( manager, writeLocation, localPath, data.Source, data.Replace) {
            CurrentFile.ReadRenamingMap( data.Renaming );
        }

        protected override UldFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaRenamed GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Renaming = CurrentFile.GetRenamingMap()
        };

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw( "##Uld" );
            }
        }

        protected override bool ExtraInputColumn() => false;
    }
}

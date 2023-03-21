using ImGuiNET;
using System;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidDocument : FileManagerDocument<EidFile, WorkspaceMetaBasic> {
        public EidDocument( EidManager manager, string writeLocation ) : base( manager, writeLocation, "Eid", "eid" ) { }
        public EidDocument( EidManager manager, string writeLocation, string localPath, SelectResult source, SelectResult replace ) : 
            base( manager, writeLocation, localPath, source, replace, "Eid", "eid" ) { }

        protected override EidFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw( "##Eid" );
            }
        }

        protected override bool ExtraInputColumn() => false;
    }
}

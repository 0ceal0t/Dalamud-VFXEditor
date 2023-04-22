using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public partial class PapDocument : FileManagerDocument<PapFile, WorkspaceMetaBasic> {
        private string HkxTemp => WriteLocation.Replace( ".pap", "_temp.hkx" );

        public PapDocument( PapManager manager, string writeLocation ) : base( manager, writeLocation, "Pap", "pap" ) { }

        public PapDocument( PapManager manager, string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( manager, writeLocation, localPath, source, replace, "Pap", "pap" ) { }

        protected override List<string> GetPapIds() => CurrentFile.GetPapIds();

        protected override PapFile FileFromReader( BinaryReader reader ) => new( reader, HkxTemp );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DisplayAnimationWarning();
            base.DrawBody();
        }

        public override void Dispose() {
            base.Dispose();
            File.Delete( HkxTemp );
        }
    }
}

using ImGuiNET;
using System;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdDocument : FileManagerDocument<ScdFile, WorkspaceMetaBasic> {
        public ScdDocument( ScdManager manager, string writeLocation ) : base( manager, writeLocation, "Scd", "scd" ) { }

        public ScdDocument( ScdManager manager, string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( manager, writeLocation, localPath, source, replace, "Scd", "scd" ) { }

        protected override ScdFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };
    }
}

using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public partial class PapDocument : FileManagerHavokDocument<PapFile> {
        public override string Id => "Pap";
        public override string Extension => "pap";

        public PapDocument( PapManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public PapDocument( PapManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override PapFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, Source.Path, HkxTemp, Plugin.State != WorkspaceState.Loading, verify );

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            //DrawAnimationWarning();
            base.DrawBody();
        }
    }
}

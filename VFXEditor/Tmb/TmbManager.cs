using Dalamud.Logging;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VFXEditor.FileManager;
using VFXEditor.Textools;
using VFXEditor.Avfx;
using VFXSelect;
using VFXSelect.VFX;

namespace VFXEditor.Tmb {
    public partial class TmbManager : FileManager<TmbDocument, WorkspaceMetaTmb, TmbFile> {
        public static TmbSelectDialog SourceSelect { get; private set; }
        public static TmbSelectDialog ReplaceSelect { get; private set; }

        public static void Setup() {
            SourceSelect = new TmbSelectDialog(
                "Tmb Select [SOURCE]",
                null,
                true,
                ( SelectResult result ) => SetSourceGlobal( result )
            );

            ReplaceSelect = new TmbSelectDialog(
                "Tmb Select [TARGET]",
                null,
                false,
                ( SelectResult result ) => SetReplaceGlobal( result )
            );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetSource( result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetReplace( result );
        }

        // =====================

        public TmbManager() : base( title: "Tmb Editor", id: "Tmb", tempFilePrefix: "TmbTemp", extension: "tmb", penumbaPath: "Tmb" ) { }

        protected override TmbDocument GetNewDocument() => new( LocalPath );

        protected override TmbDocument GetImportedDocument( string localPath, WorkspaceMetaTmb data ) => new( LocalPath, localPath, data.Source, data.Replace );

        public override void Dispose() {
            base.Dispose();
            SourceSelect.Hide();
            ReplaceSelect.Hide();
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            base.DrawBody();
        }
    }
}

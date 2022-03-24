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

namespace VFXEditor.Pap {
    public partial class PapManager : FileManager<PapDocument, WorkspaceMetaPap, PapFile> {
        public static PapSelectDialog SourceSelect { get; private set; }
        public static PapSelectDialog ReplaceSelect { get; private set; }

        public static void Setup() {
            SourceSelect = new PapSelectDialog(
                "Pap Select [SOURCE]",
                null,
                true,
                SetSourceGlobal
            );

            ReplaceSelect = new PapSelectDialog(
                "Pap Select [TARGET]",
                null,
                false,
                SetReplaceGlobal
            );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.PapManager?.SetSource( result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.PapManager?.SetReplace( result );
        }

        public static readonly string PenumbraPath = "Pap";

        // =====================

        public PapManager() : base( title: "Pap Editor", id: "Pap", tempFilePrefix: "PapTemp", extension: "pap", penumbaPath: PenumbraPath ) { }

        protected override PapDocument GetNewDocument() => new( LocalPath );

        protected override PapDocument GetImportedDocument( string localPath, WorkspaceMetaPap data ) => new( LocalPath, localPath, data.Source, data.Replace );

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

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Data;
using VfxEditor.FileManager;

namespace VfxEditor.EidFormat {
    /*public class EidManager : FileManagerWindow<EidDocument, WorkspaceMetaBasic, EidFile> {
        public static EidSelectDialog SourceSelect { get; private set; }
        public static EidSelectDialog ReplaceSelect { get; private set; }
        public static CopyManager Copy { get; private set; } = new();

        public static void SetSourceGlobal( SelectResult result ) {
            //Plugin.EidManager?.SetSource( result );
            //Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsEid, result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            //Plugin.EidManager?.SetReplace( result );
            //Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsEid, result );
        }

        public static readonly string PenumbraPath = "Eid";

        public EidManager() : base( title: "Eid Editor", id: "Eid", tempFilePrefix: "EidTemp", extension: "eid", penumbaPath: PenumbraPath ) { }

        protected override EidDocument GetNewDocument() => new( LocalPath );

        protected override EidDocument GetImportedDocument( string localPath, WorkspaceMetaBasic data ) => new( LocalPath, localPath, data.Source, data.Replace );

        protected override void DrawMenu() {
            if( CurrentFile == null ) return;
            if( ImGui.BeginMenu( "Edit##Menu" ) ) {
                //CopyManager.Eid.Draw();
                //CommandManager.Eid.Draw();
                ImGui.EndMenu();
            }
        }

        public override void Dispose() {
            base.Dispose();
            //SourceSelect.Hide();
            //ReplaceSelect.Hide();
            CurrentFile?.Dispose();
        }

        public override void DrawBody() {
            //SourceSelect.Draw();
            //ReplaceSelect.Draw();
            base.DrawBody();
        }
    }*/
}

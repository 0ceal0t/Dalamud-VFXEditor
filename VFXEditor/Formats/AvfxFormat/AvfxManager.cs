using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Select.Vfx;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxManager : FileManager<AvfxDocument, AvfxFile, WorkspaceMetaRenamed> {
        public AvfxManager() : base( "VFXEditor", "Vfx", "avfx", "Docs", "VFX" ) {
            SourceSelect = new VfxSelectDialog( "File Select [LOADED]", this, true );
            ReplaceSelect = new VfxSelectDialog( "File Select [REPLACED]", this, false );
        }

        protected override AvfxDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override AvfxDocument GetWorkspaceDocument( WorkspaceMetaRenamed data, string localPath ) => new( this, NewWriteLocation, localPath, data );

        protected override void DrawEditMenuExtra() {
            if( ImGui.BeginMenu( "Templates" ) ) {
                if( ImGui.MenuItem( "Blank" ) ) ActiveDocument?.OpenTemplate( "default_vfx.avfx" );
                if( ImGui.MenuItem( "Weapon" ) ) ActiveDocument?.OpenTemplate( "default_weapon.avfx" );
                ImGui.EndMenu();
            }

            if( ImGui.BeginMenu( "Convert Textures" ) ) {
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

                ImGui.SetNextItemWidth( 150 );
                if( ImGui.InputText( "##Prefix", ref Plugin.Configuration.CustomPathPrefix, 255 ) ) Plugin.Configuration.Save();

                ImGui.SameLine();
                if( ImGui.Button( "Apply" ) ) {
                    foreach( var document in Documents.Where( x => x.CurrentFile != null ) ) {
                        var file = document.CurrentFile;
                        file.TextureView.Group.Items.ForEach( x => x.ConvertToCustom() );
                    }
                }
                ImGui.EndMenu();
            }

            if( CurrentFile != null && ImGui.MenuItem( "Clean Up" ) ) CurrentFile.Cleanup();
        }

        public void Import( string path ) => ActiveDocument.Import( path );

        public void ShowExportDialog( AvfxNode node ) => ActiveDocument.ShowExportDialog( node );
    }
}

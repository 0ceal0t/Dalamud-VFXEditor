using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxFile {
        public readonly AvfxMain Main;
        public readonly CommandManager Command = new( Data.CopyManager.Avfx );

        public readonly UiEffectorView EffectorView;
        public readonly UiEmitterView EmitterView;
        public readonly UiModelView ModelView;
        public readonly UiParticleView ParticleView;
        public readonly UiTextureView TextureView;
        public readonly UiTimelineView TimelineView;
        public readonly UiScheduleView ScheduleView;
        public readonly UiBinderView BinderView;

        public readonly UiNodeGroupSet NodeGroupSet;

        public readonly ExportDialog ExportUi;

        private readonly bool Verified = true;
        public bool IsVerified => Verified;

        public AvfxFile( BinaryReader reader, bool checkOriginal = true ) {
            byte[] original = null;
            if( checkOriginal ) {
                var startPos = reader.BaseStream.Position;
                original = reader.ReadBytes( ( int )reader.BaseStream.Length );
                reader.BaseStream.Seek( startPos, SeekOrigin.Begin );
            }

            Main = AvfxMain.FromStream( reader ); // Parsing

            if( checkOriginal ) {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter( ms );
                Main.Write( writer );
                Verified = FileUtils.CompareFiles( original, ms.ToArray(), out var _ );
            }

            // ================

            NodeGroupSet = Main.NodeGroupSet;

            ParticleView = new UiParticleView( this, NodeGroupSet.Particles );
            BinderView = new UiBinderView( this, NodeGroupSet.Binders );
            EmitterView = new UiEmitterView( this, NodeGroupSet.Emitters );
            EffectorView = new UiEffectorView( this, NodeGroupSet.Effectors );
            TimelineView = new UiTimelineView( this, NodeGroupSet.Timelines );
            TextureView = new UiTextureView( this, NodeGroupSet.Textures );
            ModelView = new UiModelView( this, NodeGroupSet.Models );
            ScheduleView = new UiScheduleView( this, NodeGroupSet.Schedulers );

            NodeGroupSet.Initialize();

            ExportUi = new ExportDialog( this );
        }

        public void Draw() {
            if( ImGui.BeginTabBar( "##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters##Main" ) ) {
                    Main.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Scheduler##Main" ) ) {
                    ScheduleView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Timelines##Main" ) ) {
                    TimelineView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Emitters##Main" ) ) {
                    EmitterView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Particles##Main" ) ) {
                    ParticleView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Effectors##Main" ) ) {
                    EffectorView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Binders##Main" ) ) {
                    BinderView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Textures##Main" ) ) {
                    TextureView.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Models##Main" ) ) {
                    ModelView.Draw();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }

            ExportUi.Draw();
        }

        public void Write( BinaryWriter writer ) => Main?.Write( writer );

        public void Dispose() => NodeGroupSet?.Dispose();

        // ========== WORKSPACE ==============

        public Dictionary<string, string> GetRenamingMap() => NodeGroupSet.GetRenamingMap();

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) => NodeGroupSet.ReadRenamingMap( renamingMap );

        // =====================

        public void ShowExportDialog( AvfxNode node ) => ExportUi.ShowDialog( node );

        public void ShowImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", "Partial VFX{.vfxedit2,.vfxedit},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    Import( res );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not import data" );
                }
            } );
        }
    }
}

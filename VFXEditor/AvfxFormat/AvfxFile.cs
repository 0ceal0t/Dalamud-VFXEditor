using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxFile : FileManagerFile {
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

        private readonly HashSet<IAvfxUiBase> ForceOpenTabs = new();

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

        public override void Draw( string id ) {
            if( ImGui.BeginTabBar( "##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                DrawView( Main, "Parameters" );
                DrawView( ScheduleView, "Scheduler" );
                DrawView( TimelineView, "Timelines" );
                DrawView( EmitterView, "Emitters" );
                DrawView( ParticleView, "Particles" );
                DrawView( EffectorView, "Effectors" );
                DrawView( BinderView, "Binders" );
                DrawView( TextureView, "Textures" );
                DrawView( ModelView, "Models" );
                ImGui.EndTabBar();
            }
            ExportUi.Draw();
        }

        private unsafe void DrawView( IAvfxUiBase view, string label ) {
            var labelBytes = Encoding.UTF8.GetBytes( label + "##Main" );
            var labelRef = stackalloc byte[labelBytes.Length + 1];
            Marshal.Copy( labelBytes, 0, new IntPtr( labelRef ), labelBytes.Length );

            var forceOpen = ForceOpenTabs.Contains( view );
            if( forceOpen ) ForceOpenTabs.Remove( view );
            var flags = forceOpen ? ImGuiTabItemFlags.None | ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None;
            if( ImGuiNative.igBeginTabItem( labelRef, null, flags ) == 1 ) {
                view.Draw( "" );
                ImGuiNative.igEndTabItem();
            }
        }

        public void SelectItem( AvfxNode item ) {
            if( item is AvfxScheduler sched ) SelectItem( ScheduleView, sched );
            else if( item is AvfxTimeline timeline ) SelectItem( TimelineView, timeline );
            else if( item is AvfxEmitter emitter ) SelectItem( EmitterView, emitter );
            else if( item is AvfxParticle particle ) SelectItem( ParticleView, particle );
            else if( item is AvfxEffector effector ) SelectItem( EffectorView, effector );
            else if( item is AvfxBinder binder ) SelectItem( BinderView, binder );
            else if( item is AvfxTexture texture ) SelectItem( TextureView, texture );
            else if( item is AvfxModel model ) SelectItem( ModelView, model );
        }

        public void SelectItem<T>( IUiNodeView<T> view, T item ) where T : AvfxNode {
            if( item == null ) return;
            view.SetSelected( item );
            ForceOpenTabs.Add( view );
        }

        public override void Write( BinaryWriter writer ) => Main?.Write( writer );

        public void Dispose() {
            NodeGroupSet?.Dispose();
            ForceOpenTabs.Clear();
        }

        // ========== WORKSPACE ======

        public Dictionary<string, string> GetRenamingMap() => NodeGroupSet.GetRenamingMap();

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) => NodeGroupSet.ReadRenamingMap( renamingMap );

        // =======================

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

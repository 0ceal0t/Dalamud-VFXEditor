using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VfxEditor.AvfxFormat.Dialogs;
using VfxEditor.AvfxFormat.Nodes;
using VfxEditor.FileManager;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxFile : FileManagerFile {
        public readonly AvfxMain Main;

        public readonly UiEffectorView EffectorView;
        public readonly UiEmitterView EmitterView;
        public readonly UiModelView ModelView;
        public readonly UiParticleView ParticleView;
        public readonly UiTextureView TextureView;
        public readonly UiTimelineView TimelineView;
        public readonly UiScheduleView ScheduleView;
        public readonly UiBinderView BinderView;

        public readonly AvfxNodeGroupSet NodeGroupSet;

        public readonly ExportDialog ExportUi;

        private readonly HashSet<IUiItem> ForceOpenTabs = new();

        public AvfxFile( BinaryReader reader, bool checkOriginal = true ) : base( new( Plugin.AvfxManager ) ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            Main = AvfxMain.FromStream( reader );

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), 8, out var _ );

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

        public override void Draw() {
            using var _ = ImRaii.PushId( "Main" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawView( Main, "Parameters" );
            DrawView( ScheduleView, "Scheduler" );
            DrawView( TimelineView, "Timelines" );
            DrawView( EmitterView, "Emitters" );
            DrawView( ParticleView, "Particles" );
            DrawView( EffectorView, "Effectors" );
            DrawView( BinderView, "Binders" );
            DrawView( TextureView, "Textures" );
            DrawView( ModelView, "Models" );

            ExportUi.Draw();
        }

        private unsafe void DrawView( IUiItem view, string label ) {
            var labelBytes = Encoding.UTF8.GetBytes( label + "##Main" );
            var labelRef = stackalloc byte[labelBytes.Length + 1];
            Marshal.Copy( labelBytes, 0, new IntPtr( labelRef ), labelBytes.Length );

            var forceOpen = ForceOpenTabs.Contains( view );
            if( forceOpen ) ForceOpenTabs.Remove( view );
            var flags = forceOpen ? ImGuiTabItemFlags.None | ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None;
            if( ImGuiNative.igBeginTabItem( labelRef, null, flags ) == 1 ) {
                view.Draw();
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

        // ====== CLEANUP UNUSED =======

        public void Cleanup() {
            var removedNodes = new List<AvfxNode>();
            var command = new CompoundCommand( false, true );
            CleanupInternalView( TimelineView, command, removedNodes );
            CleanupInternalView( EmitterView, command, removedNodes );
            CleanupInternalView( ParticleView, command, removedNodes );
            CleanupInternalView( EffectorView, command, removedNodes );
            CleanupInternalView( BinderView, command, removedNodes );
            CleanupInternalView( TextureView, command, removedNodes );
            CleanupInternalView( ModelView, command, removedNodes );
            CommandManager.Avfx.Add( command );
        }

        private void CleanupInternalView<T>( IUiNodeView<T> view, CompoundCommand command, List<AvfxNode> removedNodes ) where T : AvfxNode {
            foreach( var node in view.GetGroup().Items ) {
                CleanupInternal( node, command, removedNodes );
            }
        }

        private void CleanupInternal( AvfxNode node, CompoundCommand command, List<AvfxNode> removedNodes ) {
            if( removedNodes.Contains( node ) ) return;

            if( !node.Parents.Select( x => x.Node ).Where( x => !removedNodes.Contains( x ) ).Any() ) {
                removedNodes.Add( node );
                command.Add( GetRemoveCommand( node ) );
                foreach( var child in node.ChildNodes ) {
                    CleanupInternal( child, command, removedNodes );
                }
            }
        }

        private ICommand GetRemoveCommand( AvfxNode node ) {
            if( node is AvfxTimeline timeline ) return new UiNodeViewRemoveCommand<AvfxTimeline>( TimelineView, TimelineView.GetGroup(), timeline );
            if( node is AvfxEmitter emitter ) return new UiNodeViewRemoveCommand<AvfxEmitter>( EmitterView, EmitterView.GetGroup(), emitter );
            if( node is AvfxParticle particle ) return new UiNodeViewRemoveCommand<AvfxParticle>( ParticleView, ParticleView.GetGroup(), particle );
            if( node is AvfxEffector effector ) return new UiNodeViewRemoveCommand<AvfxEffector>( EffectorView, EffectorView.GetGroup(), effector );
            if( node is AvfxBinder binder ) return new UiNodeViewRemoveCommand<AvfxBinder>( BinderView, BinderView.GetGroup(), binder );
            if( node is AvfxTexture texture ) return new UiNodeViewRemoveCommand<AvfxTexture>( TextureView, TextureView.GetGroup(), texture );
            if( node is AvfxModel model ) return new UiNodeViewRemoveCommand<AvfxModel>( ModelView, ModelView.GetGroup(), model );
            return null;
        }

        // ====================

        public override void Write( BinaryWriter writer ) => Main?.Write( writer );

        public override void Dispose() {
            NodeGroupSet?.Dispose();
            ForceOpenTabs.Clear();
        }

        // ========== WORKSPACE ==========

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

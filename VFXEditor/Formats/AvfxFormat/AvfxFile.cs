using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.AvfxFormat.Dialogs;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager;
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

        public readonly AvfxExport ExportUi;

        public AvfxFile( BinaryReader reader, bool verify ) : base() {
            Main = AvfxMain.FromStream( reader );

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );

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

            ExportUi = new AvfxExport( this );
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Main" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Parameters" ) ) {
                Main.Draw();
                ImGui.EndTabItem();
            }

            DrawView( ScheduleView, "Schedulers" );
            DrawView( TimelineView, "Timelines" );
            DrawView( EmitterView, "Emitters" );
            DrawView( ParticleView, "Particles" );
            DrawView( EffectorView, "Effectors" );
            DrawView( BinderView, "Binders" );
            DrawView( TextureView, "Textures" );
            DrawView( ModelView, "Models" );
        }

        private static unsafe void DrawView<T>( IUiNodeView<T> view, string label ) where T : AvfxNode {
            if( UiUtils.BeginTabItem<T>( label ) ) {
                view.Draw();
                ImGui.EndTabItem();
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

        public static void SelectItem<T>( IUiNodeView<T> view, T item ) where T : AvfxNode {
            if( item == null ) return;
            view.SetSelected( item );
            UiUtils.ForceOpenTabs.Add( typeof( T ) );
        }

        // ====== CLEANUP UNUSED =======

        public void Cleanup() {
            var removedNodes = new List<AvfxNode>();
            var commands = new List<ICommand>();

            var lastCount = -1;
            while( removedNodes.Count != lastCount ) {
                lastCount = removedNodes.Count;
                CleanupInternalView( TimelineView, commands, removedNodes );
                CleanupInternalView( EmitterView, commands, removedNodes );
                CleanupInternalView( ParticleView, commands, removedNodes );
                CleanupInternalView( EffectorView, commands, removedNodes );
                CleanupInternalView( BinderView, commands, removedNodes );
                CleanupInternalView( TextureView, commands, removedNodes );
                CleanupInternalView( ModelView, commands, removedNodes );
            }
            Dalamud.OkNotification( $"Removed {removedNodes.Count} nodes" );
            Command.AddAndExecute( new CompoundCommand( commands ) );
        }

        private void CleanupInternalView<T>( IUiNodeView<T> view, List<ICommand> commands, List<AvfxNode> removedNodes ) where T : AvfxNode {
            foreach( var node in new List<T>( view.GetGroup().Items ) ) {
                CleanupInternal( node, commands, removedNodes );
            }
        }

        private void CleanupInternal( AvfxNode node, List<ICommand> commands, List<AvfxNode> removedNodes ) {
            if( removedNodes.Contains( node ) ) return;

            if( !node.Parents.Select( x => x.Node ).Where( x => !removedNodes.Contains( x ) ).Any() ) {
                removedNodes.Add( node );
                commands.Add( GetRemoveCommand( node ) );
                foreach( var child in node.ChildNodes ) {
                    CleanupInternal( child, commands, removedNodes );
                }
            }
        }

        private ICommand GetRemoveCommand( AvfxNode node ) {
            if( node is AvfxTimeline timeline ) return new AvfxNodeViewRemoveCommand<AvfxTimeline>( TimelineView, TimelineView.GetGroup(), timeline );
            if( node is AvfxEmitter emitter ) return new AvfxNodeViewRemoveCommand<AvfxEmitter>( EmitterView, EmitterView.GetGroup(), emitter );
            if( node is AvfxParticle particle ) return new AvfxNodeViewRemoveCommand<AvfxParticle>( ParticleView, ParticleView.GetGroup(), particle );
            if( node is AvfxEffector effector ) return new AvfxNodeViewRemoveCommand<AvfxEffector>( EffectorView, EffectorView.GetGroup(), effector );
            if( node is AvfxBinder binder ) return new AvfxNodeViewRemoveCommand<AvfxBinder>( BinderView, BinderView.GetGroup(), binder );
            if( node is AvfxTexture texture ) return new AvfxNodeViewRemoveCommand<AvfxTexture>( TextureView, TextureView.GetGroup(), texture );
            if( node is AvfxModel model ) return new AvfxNodeViewRemoveCommand<AvfxModel>( ModelView, ModelView.GetGroup(), model );
            return null;
        }

        // ====================

        public override void Write( BinaryWriter writer ) => Main?.Write( writer );

        public override void Dispose() {
            NodeGroupSet?.Dispose();
        }

        // ========== WORKSPACE ==========

        public Dictionary<string, string> GetRenamingMap() => NodeGroupSet.GetRenamingMap();

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) => NodeGroupSet.ReadRenamingMap( renamingMap );

        // =======================

        public void ShowExportDialog( AvfxNode node ) => ExportUi.Show( node );

        public void ShowImportDialog() {
            FileBrowserManager.OpenFileDialog( "Select a File", "Partial VFX{.vfxedit2,.vfxedit},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    Import( res );
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Could not import data" );
                }
            } );
        }
    }
}

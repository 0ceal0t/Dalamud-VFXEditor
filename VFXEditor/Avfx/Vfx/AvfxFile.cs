using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class AvfxFile {
        public AVFXMain AVFX;

        public UIParameterView ParameterView;
        public UIEffectorView EffectorView;
        public UIEmitterView EmitterView;
        public UIModelView ModelView;
        public UIParticleView ParticleView;
        public UITextureView TextureView;
        public UITimelineView TimelineView;
        public UIScheduleView ScheduleView;
        public UIBinderView BinderView;

        public List<UINodeGroup> AllGroups;
        public UINodeGroup<UIBinder> Binders;
        public UINodeGroup<UIEmitter> Emitters;
        public UINodeGroup<UIModel> Models;
        public UINodeGroup<UIParticle> Particles;
        public UINodeGroup<UIScheduler> Schedulers;
        public UINodeGroup<UITexture> Textures;
        public UINodeGroup<UITimeline> Timelines;
        public UINodeGroup<UIEffector> Effectors;

        public ExportDialog ExportUI;

        public bool Verified = true;

        public bool GetVerified() => Verified;

        public AvfxFile( BinaryReader reader, bool checkOriginal = true ) {
            var startPos = reader.BaseStream.Position;

            byte[] original = null;
            if( checkOriginal ) {
                original = reader.ReadBytes( ( int )reader.BaseStream.Length );
                reader.BaseStream.Seek( startPos, SeekOrigin.Begin );
            }

            AVFX = AVFXMain.FromStream( reader );

            if (checkOriginal) {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter( ms );
                AVFX.Write( writer );

                var newData = ms.ToArray();

                for( var i = 0; i < Math.Min( newData.Length, original.Length ); i++ ) {
                    if( newData[i] != original[i] ) {
                        PluginLog.Log( $"Warning: files do not match at {i} {newData[i]} {original[i]}" );
                        Verified = false;
                        break;
                    }
                }
            }
            
            // ======================

            AllGroups = new() {
                ( Binders = new UINodeGroup<UIBinder>() ),
                ( Emitters = new UINodeGroup<UIEmitter>() ),
                ( Models = new UINodeGroup<UIModel>() ),
                ( Particles = new UINodeGroup<UIParticle>() ),
                ( Schedulers = new UINodeGroup<UIScheduler>() ),
                ( Textures = new UINodeGroup<UITexture>() ),
                ( Timelines = new UINodeGroup<UITimeline>() ),
                ( Effectors = new UINodeGroup<UIEffector>() )
            };

            ParticleView = new UIParticleView( this, AVFX );
            ParameterView = new UIParameterView( AVFX );
            BinderView = new UIBinderView( this, AVFX );
            EmitterView = new UIEmitterView( this, AVFX );
            EffectorView = new UIEffectorView( this, AVFX );
            TimelineView = new UITimelineView( this, AVFX );
            TextureView = new UITextureView( this, AVFX );
            ModelView = new UIModelView( this, AVFX );
            ScheduleView = new UIScheduleView( this, AVFX );

            AllGroups.ForEach( group => group.Init() );

            ExportUI = new ExportDialog( this );
        }

        public void Draw() {
            if( ImGui.BeginTabBar( "##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters##Main" ) ) {
                    ParameterView.Draw();
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
            ExportUI.Draw();
        }

        public void Write(BinaryWriter writer) => AVFX?.Write(writer);

        public void Dispose() {
            AllGroups.ForEach( group => group.Dispose() );
        }

        // ========== WORKSPACE ===========

        public Dictionary<string, string> GetRenamingMap() {
            Dictionary<string, string> ret = new();
            Schedulers.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            Timelines.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            Emitters.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            Particles.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            Effectors.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            Binders.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            Textures.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            Models.Items.ForEach( item => item.PopulateWorkspaceMeta( ret ) );
            return ret;
        }

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) {
            Dictionary<string, string> ret = new();
            Schedulers.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
            Timelines.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
            Emitters.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
            Particles.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
            Effectors.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
            Binders.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
            Textures.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
            Models.Items.ForEach( item => item.ReadWorkspaceMeta( renamingMap ) );
        }

        // ========= EXPORT ==============

        public void ExportDeps( UINode startNode, BinaryWriter bw ) {
            ExportDeps( new List<UINode>( new[] { startNode } ), bw );
        }

        public void ExportDeps( List<UINode> startNodes, BinaryWriter bw ) {
            var visited = new HashSet<UINode>();
            var nodes = new List<UINode>();
            foreach( var startNode in startNodes ) {
                RecurseChild( startNode, nodes, visited );
            }

            var IdxSave = new Dictionary<UINode, int>(); // save these to restore afterwards, since we don't want to modify the current document
            foreach( var n in nodes ) {
                IdxSave[n] = n.Idx;
            }

            OrderByType<UITimeline>( nodes );
            OrderByType<UIEmitter>( nodes );
            OrderByType<UIEffector>( nodes );
            OrderByType<UIBinder>( nodes );
            OrderByType<UIParticle>( nodes );
            OrderByType<UITexture>( nodes );
            OrderByType<UIModel>( nodes );

            UpdateAllNodes( nodes );
            foreach( var n in nodes ) {
                n.Write( bw );
            }
            foreach( var n in nodes ) {
                n.Idx = IdxSave[n];
            }
            UpdateAllNodes( nodes );
        }

        public void RecurseChild( UINode node, List<UINode> output, HashSet<UINode> visited ) {
            if( visited.Contains( node ) ) return; // prevents infinite loop
            visited.Add( node );

            foreach( var n in node.Children ) {
                RecurseChild( n, output, visited );
            }
            if( output.Contains( node ) ) return; // make sure elements get added AFTER their children. This doesn't work otherwise, since we want each node to be AFTER its dependencies
            output.Add( node );
        }

        public static void OrderByType<T>( List<UINode> items ) where T : UINode {
            var i = 0;
            foreach( var node in items ) {
                if( node is T ) {
                    node.Idx = i;
                    i++;
                }
            }
        }

        public static void UpdateAllNodes( List<UINode> nodes ) {
            foreach( var n in nodes ) {
                foreach( var s in n.Selectors ) {
                    s.UpdateNode();
                }
            }
        }

        public byte[] ToBytes() {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            AVFX.Write( writer );
            return ms.ToArray();
        }

        // ========= IMPORT ==============

        public void ImportData( string path ) {
            using var reader = new BinaryReader( File.Open( path, FileMode.Open ) );
            ImportData( reader );
        }

        public void ImportData( byte[] data ) {
            ImportData( new BinaryReader( new MemoryStream( data ) ) );
        }

        public void ImportData( BinaryReader reader ) {
            var totalSize = reader.BaseStream.Length;
            if( totalSize < 8 ) return;
            reader.ReadInt32(); // first name
            var firstSize = reader.ReadInt32();

            var has_dependencies = totalSize > ( firstSize + 8 + 4 );
            PluginLog.Log( $"Has dependencies: {has_dependencies}" );
            if( has_dependencies ) {
                PreImportGroups();
            }

            reader.BaseStream.Seek( 0, SeekOrigin.Begin ); // reset position

            List<NodePosition> models = new();
            List<NodePosition> textures = new();
            List<NodePosition> binders = new();
            List<NodePosition> effectors = new();
            List<NodePosition> particles = new();
            List<NodePosition> emitters = new();
            List<NodePosition> timelines = new();

            AVFXBase.ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                switch ( _name) {
                    case "Modl":
                        models.Add( new NodePosition( _reader.BaseStream.Position, _size ) );
                        break;
                    case "Tex":
                        textures.Add( new NodePosition( _reader.BaseStream.Position, _size ) );
                        break;
                    case "Bind":
                        binders.Add( new NodePosition( _reader.BaseStream.Position, _size ) );
                        break;
                    case "Efct":
                        effectors.Add( new NodePosition( _reader.BaseStream.Position, _size ) );
                        break;
                    case "Ptcl":
                        particles.Add( new NodePosition( _reader.BaseStream.Position, _size ) );
                        break;
                    case "Emit":
                        emitters.Add( new NodePosition( _reader.BaseStream.Position, _size ) );
                        break;
                    case "TmLn":
                        timelines.Add( new NodePosition( _reader.BaseStream.Position, _size ) );
                        break;
                }
                _reader.ReadBytes( _size ); // skip it for now, we'll come back later
            }, (int)totalSize );

            // Import items in a specific order
            foreach(var pos in models ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                ModelView.Group.Add( ModelView.OnImport( reader, pos.Size, has_dependencies ) );
            }
            foreach( var pos in textures ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                TextureView.Group.Add( TextureView.OnImport( reader, pos.Size, has_dependencies ) );
            }
            foreach( var pos in binders ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                BinderView.Group.Add( BinderView.OnImport( reader, pos.Size, has_dependencies ) );
            }
            foreach( var pos in effectors ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                EffectorView.Group.Add( EffectorView.OnImport( reader, pos.Size, has_dependencies ) );
            }
            foreach( var pos in particles ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                ParticleView.Group.Add( ParticleView.OnImport( reader, pos.Size, has_dependencies ) );
            }
            foreach( var pos in emitters ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                EmitterView.Group.Add( EmitterView.OnImport( reader, pos.Size, has_dependencies ) );
            }
            foreach( var pos in timelines ) {
                reader.BaseStream.Seek( pos.Position, SeekOrigin.Begin );
                TimelineView.Group.Add( TimelineView.OnImport( reader, pos.Size, has_dependencies ) );
            }
        }

        public void PreImportGroups() {
            AllGroups.ForEach( group => group.PreImport() );
        }

        public void ExportMultiple( UINode node ) {
            ExportUI.Export( node );
        }

        public static void ExportDialog( UINode node ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".vfxedit", "ExportedVfx", "vfxedit", ( bool ok, string res ) => {
                if( !ok ) return;

                using var fs = File.OpenWrite( res );
                using var writer = new BinaryWriter( fs );
                node.Write( writer );
            } );
        }

        public void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", ".vfxedit,.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    ImportData( res );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Could not import data", e );
                }
            } );
        }

        private struct NodePosition {
            public long Position;
            public int Size;

            public NodePosition(long position, int size) {
                Position = position;
                Size = size;
            }
        }
    }
}

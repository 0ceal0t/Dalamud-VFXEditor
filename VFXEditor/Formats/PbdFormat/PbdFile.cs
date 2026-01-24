using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FlatSharp;
using HelixToolkit.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.FileManager;
using VfxEditor.Flatbuffer;
using VfxEditor.Formats.PbdFormat.Extended;
using VfxEditor.Utils;
using VfxEditor.Utils.PackStruct;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdFile : FileManagerFile {
        // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Files/PbdFile.cs
        // https://github.com/Ottermandias/Penumbra.GameData/blob/f5a74c70ad3861c5c66e1df6ae9a29fc7a0d736a/Data/RacialDeformer.cs#L7

        public readonly List<PbdDeformer> Deformers = [];
        public readonly List<PbdConnection> Connections = [];

        private PbdConnection Selected;
        private PbdConnection Dragging;

        public const uint ExtendedType = 'E' | ( ( uint )'P' << 8 ) | ( ( uint )'B' << 16 ) | ( ( uint )'D' << 24 );
        public PbdExtended Extended;

        public PbdFile( BinaryReader reader, bool verify ) : base() {
            var count = reader.ReadInt32();
            // deformers and connections don't have matching orders for some reason. very cool, SE
            // so we have to track them separately for proper verification
            for( var i = 0; i < count; i++ ) {
                Deformers.Add( new( reader ) );
            }
            for( var i = 0; i < count; i++ ) {
                Connections.Add( new( Deformers, reader ) );
            }
            foreach( var connection in Connections ) connection.Populate( Connections );

            List<(int, int)> ignoreRange = null;
            var diff = 0;

            var packReader = new PackReader( reader );
            if( packReader.TryGetPrior( ExtendedType, out var extendedData ) ) {
                Extended = new( extendedData );
                ignoreRange = [(( int )packReader.StartPos, ( int )reader.BaseStream.Length)];
                diff = extendedData.Data.Length - Extended.GetEpbdData().Length;
                Dalamud.Log( $"Flatbuffer diff is: {diff}" );
            }

            if( verify ) {
                Verified = FileUtils.Verify( reader, ToBytes(), ignoreRange, diff );
                if( Verified == VerifiedStatus.VERIFIED && Extended != null ) Verified = VerifiedStatus.PARTIAL;
            }
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Deformers.Count );

            var offsetPositions = new Dictionary<PbdDeformer, long>();
            foreach( var deformer in Deformers ) deformer.Write( writer, Connections, offsetPositions );
            foreach( var connection in Connections ) connection.Write( writer, Connections, Deformers );

            var dataPositions = new Dictionary<PbdDeformer, long>();
            foreach( var deformer in Deformers ) {
                dataPositions[deformer] = writer.BaseStream.Position;
                deformer.WriteData( writer );
            }

            foreach( var (deformer, placeholder) in offsetPositions ) {
                writer.BaseStream.Position = placeholder;
                writer.Write( ( int )dataPositions[deformer] );
            }

            if( Extended != null ) {
                writer.BaseStream.Position = writer.BaseStream.Length;
                FileUtils.PadTo( writer, 16 );
                Extended.Write( writer );
            }
        }

        public override void Draw() {
            ImGui.Separator();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Entries" ) ) {
                if( tab ) DrawEntries();
            }


            if( Extended != null ) {
                using var tab = ImRaii.TabItem( "Extended" );
                if( tab ) Extended.Draw();
            }
        }

        private void DrawEntries() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var table = ImRaii.Table( "Table", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.NoHostExtendY, new( -1, ImGui.GetContentRegionAvail().Y ) );
            if( !table ) return;
            style.Dispose();

            ImGui.TableSetupColumn( "##Left", ImGuiTableColumnFlags.WidthFixed, 200 );
            ImGui.TableSetupColumn( "##Right", ImGuiTableColumnFlags.WidthStretch );

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) AddNew( null );
            }
            UiUtils.Tooltip( "Create new deformer at root" );

            using( var tree = ImRaii.Child( "Left" ) ) {
                using var indent = ImRaii.PushStyle( ImGuiStyleVar.IndentSpacing, 9 );
                foreach( var connection in Connections.Where( x => x.Parent == null ) ) DrawTree( connection );

                // Drag-drop to root
                using var rootStyle = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) );
                rootStyle.Push( ImGuiStyleVar.FramePadding, new Vector2( 0 ) );
                ImGui.BeginChild( "EndChild", new Vector2( ImGui.GetContentRegionAvail().X, 1 ), false );
                ImGui.EndChild();
                using var dragDrop = ImRaii.DragDropTarget();
                if( dragDrop ) StopDragging( null );
            }

            ImGui.TableNextColumn();

            using var right = ImRaii.Child( "Right" );

            if( Selected != null ) {
                using var color = ImRaii.PushColor( ImGuiCol.Button, UiUtils.RED_COLOR );
                if( UiUtils.IconButton( FontAwesomeIcon.Trash, "Delete" ) ) Delete( Selected );
            }

            Selected?.Item.Draw();
        }

        private void DrawTree( PbdConnection connection ) {
            var isLeaf = connection.Child == null;

            var flags =
                ImGuiTreeNodeFlags.DefaultOpen |
                ImGuiTreeNodeFlags.OpenOnArrow |
                ImGuiTreeNodeFlags.OpenOnDoubleClick |
                ImGuiTreeNodeFlags.SpanFullWidth;

            if( isLeaf ) flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            if( Selected == connection ) flags |= ImGuiTreeNodeFlags.Selected;

            var nodeOpen = ImGui.TreeNodeEx( $"{connection.Item.SkeletonId.Value}", flags );

            DragDrop( connection );

            if( ImGui.IsItemClicked( ImGuiMouseButton.Left ) && !ImGui.IsItemToggledOpen() ) Selected = connection;
            if( !isLeaf && nodeOpen ) {
                var node = connection.Child;
                while( node != null ) {
                    DrawTree( node );
                    node = node.Sibling;
                }
                ImGui.TreePop();
            }
        }

        private PbdConnection GetLastChild( PbdConnection parent ) => Connections.FirstOrDefault( x => x.Parent == parent && x.Sibling == null );

        private void AddAsChild( PbdConnection parent, PbdConnection item, List<ICommand> commands ) { // add to the end of list
            var lastExistingChild = GetLastChild( parent );
            commands.Add( PbdConnectionCommand.SetParent( item, parent ) );
            if( lastExistingChild != null ) commands.Add( PbdConnectionCommand.SetSibling( lastExistingChild, item ) ); // add as sibling of last child
            else if( parent != null ) commands.Add( PbdConnectionCommand.SetChild( parent, item ) ); // is now the child
        }

        private void AddNew( PbdConnection parent ) {
            var deformer = new PbdDeformer();
            var connection = new PbdConnection( deformer );
            var commands = new List<ICommand> {
                        new ListAddCommand<PbdDeformer>( Deformers, deformer ),
                        new ListAddCommand<PbdConnection>( Connections, connection )
                    };
            AddAsChild( parent, connection, commands );
            CommandManager.Add( new CompoundCommand( commands ) );
        }

        private void RemoveFromParent( PbdConnection item, List<ICommand> commands ) {
            if( item.Parent != null && item.Parent.Child == item ) commands.Add( PbdConnectionCommand.SetChild( item.Parent, item.Sibling ) );
            commands.Add( PbdConnectionCommand.SetParent( item, null ) );

            var prevSibling = Connections.FirstOrDefault( x => x.Sibling == item );
            if( prevSibling != null ) commands.Add( PbdConnectionCommand.SetSibling( prevSibling, item.Sibling ) );
            commands.Add( PbdConnectionCommand.SetSibling( item, null ) );
        }

        private void Delete( PbdConnection connection ) {
            var toDelete = new List<PbdConnection> {
                connection
            };
            PopulateChilden( connection, toDelete );

            if( toDelete.Contains( Selected ) ) Selected = null;

            var commands = new List<ICommand>();
            foreach( var item in toDelete ) {
                commands.Add( new ListRemoveCommand<PbdConnection>( Connections, item ) );
                commands.Add( new ListRemoveCommand<PbdDeformer>( Deformers, item.Item ) );
            }
            RemoveFromParent( connection, commands );
            CommandManager.Add( new CompoundCommand( commands ) );
        }

        private void PopulateChilden( PbdConnection parent, List<PbdConnection> children ) {
            foreach( var connection in Connections ) {
                if( connection.Parent == parent ) {
                    if( children.Contains( connection ) ) continue;
                    children.Add( connection );
                    PopulateChilden( connection, children );
                }
            }
        }

        // ======= DRAG + DROP ============

        private void DragDrop( PbdConnection connection ) {
            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                StartDragging( connection );
                ImGui.Text( $"{connection.Item.SkeletonId.Value}" );
                ImGui.EndDragDropSource();
            }

            if( ImGui.BeginDragDropTarget() ) {
                StopDragging( connection );
                ImGui.EndDragDropTarget();
            }
        }

        private void StartDragging( PbdConnection connection ) {
            ImGui.SetDragDropPayload( "PBD_CONNECTION", null, 0 );
            Dragging = connection;
        }

        public unsafe bool StopDragging( PbdConnection destination ) {
            if( Dragging == null ) return false;
            var payload = ImGui.AcceptDragDropPayload( "PBD_CONNECTION" );
            if( payload.Handle == null ) return false;

            if( Dragging != destination ) {
                if( destination != null && destination.IsChildOf( Dragging ) ) {
                    Dalamud.Log( "Tried to put deformer into itself" );
                }
                else {
                    var commands = new List<ICommand>();
                    RemoveFromParent( Dragging, commands );
                    AddAsChild( destination, Dragging, commands );
                    CommandManager.Add( new CompoundCommand( commands ) );
                }
            }

            Dragging = null;
            return true;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxFile {
        public void AddToNodeLibrary( AvfxNode node ) {
            var newId = UiUtils.RandomString( 12 );
            var newPath = Plugin.LibraryManager.GetNodePath( newId );
            Export( node, newPath, true );
            UiUtils.OkNotification( "Saved item to library" );
            Plugin.LibraryManager.AddNode( node.GetText(), newId, Plugin.AvfxManager.Selected.SourceDisplay, newPath );
        }

        public void Export( AvfxNode node, string path, bool exportDependencies ) => Export( new List<AvfxNode> { node }, path, exportDependencies );

        public void Export( List<AvfxNode> nodes, string path, bool exportDependencies ) {
            using var writer = new BinaryWriter( File.Open( path, FileMode.Create ) );

            writer.Write( 2 ); // Magic :)
            writer.Write( exportDependencies );

            var placeholderPos = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder, number of items
            writer.Write( 0 ); // placeholder, data size
            writer.Write( 0 ); // placeholder, rename offset

            var finalNodes = nodes;
            var dataPos = writer.BaseStream.Position;

            if( exportDependencies ) {
                finalNodes = ExportWithDepedencies( nodes, writer );
            }
            else {
                // leave nodes as-is
                finalNodes.ForEach( n => n.Write( writer ) );
            }

            var size = writer.BaseStream.Position - dataPos;

            var renames = finalNodes.Select( n => n.Renamed );
            var renamedOffset = writer.BaseStream.Position;
            var numberOfItems = finalNodes.Count;

            foreach( var renamed in renames ) {
                FileUtils.WriteString( writer, string.IsNullOrEmpty( renamed ) ? "" : renamed, true );
            }
            var finalPos = writer.BaseStream.Position;

            // go back and fill out placeholders
            writer.BaseStream.Seek( placeholderPos, SeekOrigin.Begin );
            writer.Write( numberOfItems );
            writer.Write( ( int )size );
            writer.Write( ( int )renamedOffset );
            writer.BaseStream.Seek( finalPos, SeekOrigin.Begin );
        }

        private List<AvfxNode> ExportWithDepedencies( List<AvfxNode> startNodes, BinaryWriter bw ) {
            var visited = new HashSet<AvfxNode>();
            var nodes = new List<AvfxNode>();
            foreach( var startNode in startNodes ) {
                RecurseVisit( startNode, nodes, visited );
            }

            var IdxToRestore = new Dictionary<AvfxNode, int>(); // save these to restore afterwards, since we don't want to modify the current document
            foreach( var n in nodes ) {
                IdxToRestore[n] = n.GetIdx();
            }

            OrderTypeOfNode<AvfxTimeline>( nodes );
            OrderTypeOfNode<AvfxEmitter>( nodes );
            OrderTypeOfNode<AvfxEffector>( nodes );
            OrderTypeOfNode<AvfxBinder>( nodes );
            OrderTypeOfNode<AvfxParticle>( nodes );
            OrderTypeOfNode<AvfxTexture>( nodes );
            OrderTypeOfNode<AvfxModel>( nodes );

            UpdateAllSelectors( nodes );
            foreach( var n in nodes ) n.Write( bw );
            foreach( var n in nodes ) { // reset index
                n.SetIdx( IdxToRestore[n] );
            }
            UpdateAllSelectors( nodes );

            return nodes;
        }

        private void RecurseVisit( AvfxNode node, List<AvfxNode> output, HashSet<AvfxNode> visited ) {
            if( node == null ) return;
            if( visited.Contains( node ) ) return; // prevents infinite loop
            visited.Add( node );

            foreach( var n in node.ChildNodes ) {
                RecurseVisit( n, output, visited );
            }
            if( output.Contains( node ) ) return; // make sure elements get added AFTER their children. This doesn't work otherwise, since we want each node to be AFTER its dependencies
            output.Add( node );
        }

        private static void OrderTypeOfNode<T>( List<AvfxNode> items ) where T : AvfxNode {
            var i = 0;
            foreach( var node in items ) {
                if( node is T ) {
                    node.SetIdx( i );
                    i++;
                }
            }
        }

        private static void UpdateAllSelectors( List<AvfxNode> nodes ) {
            foreach( var n in nodes ) {
                foreach( var s in n.Selectors ) {
                    s.UpdateLiteral();
                }
            }
        }
    }
}

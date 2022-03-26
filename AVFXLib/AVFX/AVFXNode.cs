using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AVFXLib.AVFX {
    public class AVFXNode {
        public string Name;
        public List<AVFXNode> Children = new();

        public AVFXNode( string name ) {
            Name = name;
            Children = new List<AVFXNode>();
        }

        public virtual byte[] ToBytes() {
            var totalSize = 0;
            var byteArrays = new byte[Children.Count][];
            for( var i = 0; i < Children.Count; i++ ) {
                byteArrays[i] = Children[i].ToBytes();
                totalSize += byteArrays[i].Length;
            }
            var bytes = new byte[8 + Util.RoundUp( totalSize )];
            var name = Util.NameTo4Bytes( Name );
            var size = Util.IntTo4Bytes( totalSize );
            Buffer.BlockCopy( name, 0, bytes, 0, 4 );
            Buffer.BlockCopy( size, 0, bytes, 4, 4 );
            var bytesSoFar = 8;
            for( var i = 0; i < byteArrays.Length; i++ ) {
                Buffer.BlockCopy( byteArrays[i], 0, bytes, bytesSoFar, byteArrays[i].Length );
                bytesSoFar += byteArrays[i].Length;
            }
            return bytes;
        }

        public bool CheckEquals( AVFXNode node, out List<string> messages ) {
            messages = new List<string>();
            return EqualsNode( node, messages );
        }

        public virtual bool EqualsNode( AVFXNode node, List<string> messages ) {
            if( ( node is AVFXLeaf ) || ( node is AVFXBlank ) ) {
                messages.Add( string.Format( "Wrong Type {0} / {1}", Name, node.Name ) );
                return false;
            }
            if( Name != node.Name ) {
                messages.Add( string.Format( "Wrong Name {0} / {1}", Name, node.Name ) );
                return false;
            }

            var notBlank = Children.Where( x => x is not AVFXBlank  ).ToList();
            var notBlank2 = node.Children.Where( x => x is not AVFXBlank  ).ToList();

            if( notBlank.Count != notBlank2.Count ) {
                messages.Add( string.Format( "Wrong Node Size {0} : {1} / {2} : {3}", Name, notBlank.Count, node.Name, notBlank2.Count ) );

                return false;
            }
            for( var idx = 0; idx < notBlank.Count; idx++ ) {
                var e = notBlank[idx].EqualsNode( notBlank2[idx], messages );
                if( !e ) {
                    messages.Add( string.Format( "Not Equal {0} index: {1}", Name, idx ) );
                    return false;
                }
            }
            return true;
        }

        public virtual string ExportString( int level ) {
            var ret = string.Format( "{0}+---  {1} ----\n", new string( '\t', level ), Name );
            foreach( var c in Children ) {
                ret += c.ExportString( level + 1 );
            }
            return ret;
        }
    }
}

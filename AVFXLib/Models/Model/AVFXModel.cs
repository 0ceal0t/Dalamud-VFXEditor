using AVFXLib.AVFX;
using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXModel : Base {
        public const string NAME = "Modl";

        public List<Vertex> Vertices = new();
        public List<Index> Indexes = new();
        public List<VNum> VNums = new();
        public List<EmitVertex> EmitVertices = new();

        public AVFXModel() : base( NAME ) {
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            foreach( var n in node.Children ) {
                AVFXLeaf leaf;
                switch( n.Name ) {
                    case "VNum":
                        leaf = ( AVFXLeaf )n;
                        foreach( var bytes in Util.SplitBytes( leaf.Contents, VNum.SIZE ) ) {
                            VNums.Add( new VNum( bytes ) );
                        }
                        break;

                    case "VDrw":
                        var vleaf = ( AVFXLeaf )n;
                        foreach( var bytes in Util.SplitBytes( vleaf.Contents, Vertex.SIZE ) ) {
                            Vertices.Add( new Vertex( bytes ) );
                        }
                        break;

                    case "VEmt":
                        leaf = ( AVFXLeaf )n;
                        foreach( var bytes in Util.SplitBytes( leaf.Contents, EmitVertex.SIZE ) ) {
                            EmitVertices.Add( new EmitVertex( bytes ) );
                        }
                        break;

                    case "VIdx":
                        leaf = ( AVFXLeaf )n;
                        foreach( var bytes in Util.SplitBytes( leaf.Contents, Index.SIZE ) ) {
                            Indexes.Add( new Index( bytes ) );
                        }
                        break;
                }
            }
        }

        public VNum AddVNum() {
            var vnum = new VNum( new byte[VNum.SIZE] );
            VNums.Add( vnum );
            return vnum;
        }
        public void AddVNum( VNum item ) {
            VNums.Add( item );
        }
        public void RemoveVNum( int idx ) {
            VNums.RemoveAt( idx );
        }
        public void RemoveVNum( VNum item ) {
            VNums.Remove( item );
        }
        //
        public EmitVertex AddEmitVertex() {
            var eVert = new EmitVertex( new byte[EmitVertex.SIZE] );
            EmitVertices.Add( eVert );
            return eVert;
        }
        public void AddEmitVertex( EmitVertex item ) {
            EmitVertices.Add( item );
        }
        public void RemoveEmitVertex( int idx ) {
            EmitVertices.RemoveAt( idx );
        }
        public void RemoveEmitVertex( EmitVertex item ) {
            EmitVertices.Remove( item );
        }

        public override AVFXNode ToAVFX() {
            var modelNode = new AVFXNode( "Modl" );

            // VNUM ==================
            if( VNums.Count > 0 ) {
                var vnumBytes = new byte[VNums.Count][];
                var nIdx = 0;
                foreach( var n in VNums ) {
                    vnumBytes[nIdx] = n.ToBytes();
                    nIdx++;
                }
                var nBytes = Util.JoinBytes( vnumBytes, VNum.SIZE );
                var vnum = new AVFXLeaf( "VNum", nBytes.Length, nBytes );
                modelNode.Children.Add( vnum );
            }

            // VEMT ==================
            if( EmitVertices.Count > 0 ) {
                var emtBytes = new byte[EmitVertices.Count][];
                var eIdx = 0;
                foreach( var e in EmitVertices ) {
                    emtBytes[eIdx] = e.ToBytes();
                    eIdx++;
                }
                var eBytes = Util.JoinBytes( emtBytes, EmitVertex.SIZE );
                var vemt = new AVFXLeaf( "VEmt", eBytes.Length, eBytes );
                modelNode.Children.Add( vemt );
            }

            // VDRW ==================
            if( Vertices.Count > 0 ) {
                var vertBytes = new byte[Vertices.Count][];
                var vIdx = 0;
                foreach( var v in Vertices ) {
                    vertBytes[vIdx] = v.ToBytes();
                    vIdx++;
                }
                var vBytes = Util.JoinBytes( vertBytes, Vertex.SIZE );
                var vdrw = new AVFXLeaf( "VDrw", vBytes.Length, vBytes );
                modelNode.Children.Add( vdrw );
            }

            // VIDX ==================
            if( Indexes.Count > 0 ) {
                var indexBytes = new byte[Indexes.Count][];
                var iIdx = 0;
                foreach( var i in Indexes ) {
                    indexBytes[iIdx] = i.ToBytes();
                    iIdx++;
                }
                var iBytes = Util.JoinBytes( indexBytes, Index.SIZE );
                var vidx = new AVFXLeaf( "VIdx", iBytes.Length, iBytes );
                modelNode.Children.Add( vidx );
            }


            return modelNode;
        }
    }
}

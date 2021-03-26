using AVFXLib.AVFX;
using AVFXLib.Main;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXModel : Base
    {
        public const string NAME = "Modl";

        public List<Vertex> Vertices = new List<Vertex>();
        public List<Index> Indexes = new List<Index>();
        public List<VNum> VNums = new List<VNum>();
        public List<EmitVertex> EmitVertices = new List<EmitVertex>();

        public AVFXModel() : base(NAME)
        {
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            foreach(AVFXNode n in node.Children)
            {
                AVFXLeaf leaf;
                switch (n.Name)
                {
                    case "VNum":
                        leaf = (AVFXLeaf)n;
                        foreach (byte[] bytes in Util.SplitBytes(leaf.Contents, VNum.SIZE))
                        {
                            VNums.Add(new VNum(bytes));
                        }
                        break;

                    case "VDrw":
                        AVFXLeaf vleaf = (AVFXLeaf)n;
                        foreach (byte[] bytes in Util.SplitBytes(vleaf.Contents, Vertex.SIZE))
                        {
                            Vertices.Add(new Vertex(bytes));
                        }
                        break;

                    case "VEmt":
                        leaf = (AVFXLeaf)n;
                        foreach (byte[] bytes in Util.SplitBytes(leaf.Contents, EmitVertex.SIZE))
                        {
                            EmitVertices.Add(new EmitVertex(bytes));
                        }
                        break;

                    case "VIdx":
                        leaf = (AVFXLeaf)n;
                        foreach(byte[] bytes in Util.SplitBytes(leaf.Contents, Index.SIZE))
                        {
                            Indexes.Add(new Index(bytes));
                        }
                        break;
                }
            }
        }

        public VNum addVNum()
        {
            VNum vnum = new VNum(new byte[VNum.SIZE]);
            VNums.Add( vnum );
            return vnum;
        }
        public void addVNum( VNum item) {
            VNums.Add( item );
        }
        public void removeVNum(int idx )
        {
            VNums.RemoveAt( idx );
        }
        public void removeVNum(VNum item ) {
            VNums.Remove( item );
        }
        //
        public EmitVertex addEmitVertex()
        {
            EmitVertex eVert = new EmitVertex( new byte[EmitVertex.SIZE] );
            EmitVertices.Add( eVert );
            return eVert;
        }
        public void addEmitVertex( EmitVertex item ) {
            EmitVertices.Add( item );
        }
        public void removeEmitVertex(int idx )
        {
            EmitVertices.RemoveAt( idx );
        }
        public void removeEmitVertex( EmitVertex item ) {
            EmitVertices.Remove( item );
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode modelNode =  new AVFXNode("Modl");

            // VNUM ==================
            if (VNums.Count > 0)
            {
                byte[][] vnumBytes = new byte[VNums.Count][];
                int nIdx = 0;
                foreach (VNum n in VNums)
                {
                    vnumBytes[nIdx] = n.toBytes();
                    nIdx++;
                }
                byte[] nBytes = Util.JoinBytes(vnumBytes, VNum.SIZE);
                AVFXLeaf vnum = new AVFXLeaf("VNum", nBytes.Length, nBytes);
                modelNode.Children.Add(vnum);
            }

            // VEMT ==================
            if (EmitVertices.Count > 0)
            {
                byte[][] emtBytes = new byte[EmitVertices.Count][];
                int eIdx = 0;
                foreach (EmitVertex e in EmitVertices)
                {
                    emtBytes[eIdx] = e.toBytes();
                    eIdx++;
                }
                byte[] eBytes = Util.JoinBytes(emtBytes, EmitVertex.SIZE);
                AVFXLeaf vemt = new AVFXLeaf("VEmt", eBytes.Length, eBytes);
                modelNode.Children.Add(vemt);
            }

            // VDRW ==================
            if (Vertices.Count > 0)
            {
                byte[][] vertBytes = new byte[Vertices.Count][];
                int vIdx = 0;
                foreach (Vertex v in Vertices)
                {
                    vertBytes[vIdx] = v.toBytes();
                    vIdx++;
                }
                byte[] vBytes = Util.JoinBytes(vertBytes, Vertex.SIZE);
                AVFXLeaf vdrw = new AVFXLeaf("VDrw", vBytes.Length, vBytes);
                modelNode.Children.Add(vdrw);
            }

            // VIDX ==================
            if (Indexes.Count > 0)
            {
                byte[][] indexBytes = new byte[Indexes.Count][];
                int iIdx = 0;
                foreach (Index i in Indexes)
                {
                    indexBytes[iIdx] = i.toBytes();
                    iIdx++;
                }
                byte[] iBytes = Util.JoinBytes(indexBytes, Index.SIZE);
                AVFXLeaf vidx = new AVFXLeaf("VIdx", iBytes.Length, iBytes);
                modelNode.Children.Add(vidx);
            }


            return modelNode;
        }
    }
}

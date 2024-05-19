using ImGuiNET;
using QuickGraph;
using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Content;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public enum Direction {
        N = 0,
        NE = 1,
        E = 2,
        SE = 3,
        S = 4,
        SW = 5,
        W = 6,
        NW = 7,
        None = 8
    }

    public enum EdgeEndpointOption {
        None = 0,
        Source = 1,
        Target = 2,
        Either = 3
    }

    public class NodeCanvas {
        public const float MinScale = 0.1f;
        public const float MaxScale = 2f;
        public const float StepScale = 0.1f;

        public int mId;
        public string mName;
        private int NodeCounter { get; set; } = -1;
        private NodeMap mMap = new();
        private Dictionary<string, Node> mNodes = [];
        private HashSet<string> _nodeIds = [];
        private OccupiedRegion mOccuppiedRegion;       // this shouldn't be serialized, and only initiated using node list and maps.
        public AdjacencyGraph<int, SEdge<int>> mGraph;        // whatever this is
        private List<Edge> mEdges = [];

        private NodeCanvasConfig mConfig { get; set; } = new();

        private bool _isNodeBeingDragged = false;
        private readonly HashSet<string> _selectedNodes = [];
        private LinkedList<string> _nodeRenderZOrder = new();
        private string _nodeQueueingHndCtxMnu = null;
        private Node _snappingNode = null;
        private Dictionary<int, string> _nodeIdAndNodeGraphId = [];
        private Vector2? _lastSnapDelta = null;
        private FirstClickType _firstClickInDrag = FirstClickType.None;
        private bool _isFirstFrameAfterLmbDown = true;      // specifically for Draw()
        private Vector2? _selectAreaOSP = null;
        private bool _isNodeSelectionLocked = false;
        private EdgeConn _nodeConnTemp = null;
        private readonly Dictionary<string, string> _cachePathToPTarget = [];
        private readonly Dictionary<string, string> _cachePathToPSource = [];
        private readonly Dictionary<string, string> _nodeLookUpData = [];
        private Tuple<string, List<string>> _cacheLookUpValue = new( "", [] );

        public NodeCanvas( int pId, string pName = null ) {
            mId = pId;
            mName = pName ?? $"Canvas {mId}";
            mOccuppiedRegion = new();
            mGraph = new();
        }

        /// <summary> Specifically for JsonConverter. </summary>
        public void Init(
            int _nodeCounter,
            NodeMap mMap,
            Dictionary<string, Node> mNodes,
            HashSet<string> _nodeIds,
            OccupiedRegion mOccuppiedRegion,
            AdjacencyGraph<int, SEdge<int>> mGraph,
            List<Edge> mEdges,
            NodeCanvasConfig mConfig,
            LinkedList<string> _nodeRenderZOrder,
            Dictionary<int, string> _nodeIdAndNodeGraphId ) {
            NodeCounter = _nodeCounter;
            this.mMap = mMap;
            this.mNodes = mNodes;
            this._nodeIds = _nodeIds;
            this.mOccuppiedRegion = mOccuppiedRegion;
            this.mGraph = mGraph;
            this.mEdges = mEdges;
            this.mConfig = mConfig;
            this._nodeRenderZOrder = _nodeRenderZOrder;
            this._nodeIdAndNodeGraphId = _nodeIdAndNodeGraphId;

            // Refresh node lookup data
            _nodeLookUpData.Clear();
            foreach( var node in this.mNodes.Values ) {
                AddNodeLookUpData( node.Id, node.Content.GetHeader() );
            }
        }

        public float GetScaling() => mConfig.scaling;
        public void SetScaling( float pScale ) => mConfig.scaling = pScale;
        public Vector2 GetBaseOffset() => mMap.GetBaseOffset();

        /// <summary>
        /// Add node at pos relative to the canvas's origin.
        /// </summary>
        protected string AddNode<T>(
                NodeContent pNodeContent,
                Vector2 pDrawRelaPos,
                string pTag = null
                          ) where T : Node, new() {
            // create node  (the node id is just a dummy. AddNode() should create from incremental static id val)
            T tNode = new();
            // tNode.GetType() == typeof( AuxNode ) ? AuxNode.minHandleSize : null
            tNode.Init( "-1", -1, pNodeContent, _style: new( Vector2.Zero, Vector2.Zero ), pTag: pTag );
            // add node
            if( AddNode( tNode, pDrawRelaPos ) == null ) return null;

            return tNode.Id;
        }
        private string AddNode( Node pNode, Vector2 pDrawRelaPos ) {
            var tNewId = NodeCounter + 1;
            // assimilate
            pNode.SetId( tNewId.ToString() );
            pNode.GraphId = tNewId;
            // add node
            try {
                if( mNodes.ContainsKey( pNode.Id ) || _nodeIds.Contains( pNode.Id ) ) return null;       // check nevertheless
                if( !mNodes.TryAdd( pNode.Id, pNode ) ) return null;
                if( !_nodeIds.Add( pNode.Id ) ) return null;
                if( !mOccuppiedRegion.IsUpdatedOnce() ) mOccuppiedRegion.Update( mNodes, mMap );
                _nodeRenderZOrder.AddLast( pNode.Id );
                mMap.AddNode( pNode.Id, pDrawRelaPos );
            }
            catch( Exception e ) { Dalamud.Error( e.Message ); }

            mOccuppiedRegion.Update( mNodes, mMap );
            // add node vertex to graph
            mGraph.AddVertex( pNode.GraphId );
            _nodeIdAndNodeGraphId.TryAdd( pNode.GraphId, pNode.Id );
            // add look up value
            AddNodeLookUpData( pNode.Id, pNode.Content.GetHeader() );

            NodeCounter++;
            return pNode.Id;
        }
        /// <summary>
        /// <para>Add node to one of the 4 corners of the occupied area, with preferred direction.</para>
        /// <para>Returns added node's ID if succeed, otherwise null.</para>
        /// </summary>
        public string AddNodeToAvailableCorner<T>(
                NodeContent pNodeContent,
                Direction pCorner = Direction.NE,
                Direction pDirection = Direction.E,
                Vector2? pPadding = null,
                string pTag = null
                                  ) where T : Node, new() {
            return AddNode<T>(
                    pNodeContent,
                    mOccuppiedRegion.GetAvailableRelaPos( pCorner, pDirection, pPadding ?? mConfig.nodeGap ),
                    pTag: pTag
                );
        }
        /// <summary>
        /// <para>Add node within the view area (master layer)</para>
        /// <para>Returns added node's ID if succeed, otherwise null.</para>
        /// </summary>
        public string AddNodeWithinView<T>(
                NodeContent pNodeContent,
                Vector2 pViewerSize
                                  ) where T : Node, new() {
            var tOffset = mMap.GetBaseOffset();
            Area pRelaAreaToScanForAvailableRegion = new(
                    -tOffset - pViewerSize * 0.5f,
                    pViewerSize * 0.95f              // only get up until 0.8 of the screen to avoid the new node going out of viewer
                );
            return AddNode<T>(
                    pNodeContent,
                    mOccuppiedRegion.GetAvailableRelaPos( pRelaAreaToScanForAvailableRegion )
                );
        }
        public string AddNodeWithinViewOffset<T>(
                NodeContent pNodeContent,
                Vector2 pNodeOffset,
                Vector2? pOffsetExtra = null
                                ) where T : Node, new() {
            var tViewerRelaPos = mMap.GetBaseOffset();
            return AddNode<T>(
                    pNodeContent,
                    -tViewerRelaPos + pNodeOffset + ( pOffsetExtra ?? Vector2.Zero ) * ( 1 / GetScaling() )
                );
        }
        public string AddNodeWithinViewOffset(
                Node pNode,
                Vector2 pNodeOffset,
                Vector2? pOffsetExtra = null ) {
            var tViewerRelaPos = mMap.GetBaseOffset();
            return AddNode(
                    pNode,
                    -tViewerRelaPos + pNodeOffset + ( pOffsetExtra ?? Vector2.Zero ) * ( 1 / GetScaling() )
                );
        }
        public string AddNodeAdjacent<T>(
                NodeContent pNodeContent,
                string pNodeIdToAdjoin,
                Vector2? pOffset = null,
                string pTag = null
                ) where T : Node, new() {
            if( !mNodes.TryGetValue( pNodeIdToAdjoin, out var pPrevNode ) || pPrevNode == null ) return null;
            Vector2 tRelaPosRes;

            mGraph.TryGetOutEdges( pPrevNode.GraphId, out var edges );
            if( edges == null ) return null;
            // Getting child nodes that is positioned at greatest Y
            float? tChosenY = null;
            Node tChosenNode = null;
            foreach( var e in edges ) {
                var iChildId = GetNodeIdWithNodeGraphId( e.Target );
                if( iChildId == null ) continue;
                var iChildRelaPos = mMap.GetNodeRelaPos( iChildId );
                if( !iChildRelaPos.HasValue ) continue;
                if( !tChosenY.HasValue || iChildRelaPos.Value.Y > tChosenY ) {
                    tChosenY = iChildRelaPos.Value.Y;
                    mNodes.TryGetValue( iChildId, out var val );
                    tChosenNode = val ?? tChosenNode;
                }
            }
            // Calc final draw pos
            if( tChosenNode == null ) tRelaPosRes = ( mMap.GetNodeRelaPos( pNodeIdToAdjoin ) ?? Vector2.One ) + new Vector2( pPrevNode.Style.GetSize().X, 0 ) + ( pOffset ?? Vector2.One );
            else {
                tRelaPosRes = new(
                        ( ( mMap.GetNodeRelaPos( pNodeIdToAdjoin ) ?? Vector2.One ) + new Vector2( pPrevNode.Style.GetSize().X, 0 ) + ( pOffset ?? Vector2.One ) ).X,
                        ( mMap.GetNodeRelaPos( tChosenNode.Id ) ?? mMap.GetNodeRelaPos( pNodeIdToAdjoin ) ?? Vector2.One ).Y + tChosenNode.Style.GetSize().Y + ( pOffset ?? Vector2.One ).Y
                    );
            }

            return AddNode<T>(
                    pNodeContent,
                    tRelaPosRes,
                    pTag: pTag
                );
        }
        public string AddNodeAdjacent(
                Seed pSeed,
                string pNodeIdToAdjoin,
                string pTag = null ) {
            var tRes = pSeed.nodeType switch {
                BasicNode.nodeType => AddNodeAdjacent<BasicNode>( pSeed.nodeContent, pNodeIdToAdjoin, pSeed.ofsToPrevNode, pTag: pTag ),
                _ => AddNodeAdjacent<BasicNode>( pSeed.nodeContent, pNodeIdToAdjoin, pSeed.ofsToPrevNode, pTag: pTag )
            };
            // Connect node
            if( pSeed.isEdgeConnected && tRes != null ) AddEdge( pNodeIdToAdjoin, tRes );
            return tRes;
        }
        /// <summary>
        /// Return false if the process partially/fully fails.
        /// </summary>
        public bool RemoveNode( string pNodeId, bool _isUpdatingOccupiedRegion = true ) {
            var tRes = true;
            int? tNodeGraphId = null;
            if( !mMap.RemoveNode( pNodeId ) ) tRes = false;
            if( mNodes.TryGetValue( pNodeId, out var tNode ) && tNode != null ) {
                tNodeGraphId = tNode.GraphId;
                mNodes[pNodeId].OnDelete();
                mNodes[pNodeId].Dispose();
            }
            if( !mNodes.Remove( pNodeId ) ) tRes = false;
            if( !_nodeIds.Remove( pNodeId ) ) tRes = false;
            _selectedNodes.Remove( pNodeId );
            _nodeRenderZOrder.Remove( pNodeId );

            // Also removing pack nodes (place this above OccupiedRegion.Update())
            if( tNode != null && tNode.Packing == Node.PackingStatus.PackingDone ) {
                foreach( var packMemId in tNode.Pack ) {
                    if( !mNodes.ContainsKey( packMemId ) ) continue;
                    RemoveNode( packMemId, _isUpdatingOccupiedRegion: false );
                }
            }

            if( _isUpdatingOccupiedRegion ) mOccuppiedRegion.Update( mNodes, mMap );
            // Graph stuff
            RemoveEdgesContainingNodeId( pNodeId );
            if( tNodeGraphId.HasValue ) {
                _nodeIdAndNodeGraphId.Remove( tNodeGraphId.Value );
                mGraph.RemoveVertex( tNodeGraphId.Value );
            }

            // Remove look up data
            RemoveNodeLookUpData( pNodeId );

            return tRes;
        }
        public bool FocusOnNode( string pNodeId, Vector2? pExtraOfs = null ) {
            return mMap.FocusOnNode( pNodeId, ( pExtraOfs ?? new Vector2( -90, -90 ) ) * GetScaling() );
        }
        private string GetNodeIdWithNodeGraphId( int pNodeGraphId ) {
            _nodeIdAndNodeGraphId.TryGetValue( pNodeGraphId, out var iChildId );
            return iChildId;
        }
        /// <summary>
        /// <para>Create a directional edge connecting two nodes.</para>
        /// <para>Return false if any node DNE, or there's already an edge with the same direction, or the new edge would introduce cycle.</para>
        /// </summary>
        public bool AddEdge( string pSourceNodeId, string pTargetNodeId ) {
            if( !( HasNode( pSourceNodeId ) && HasNode( pTargetNodeId ) )
                || GetEdge( pSourceNodeId, pTargetNodeId ) != null )
                return false;
            Edge tEdge = new( pSourceNodeId, pTargetNodeId, new SEdge<int>( mNodes[pSourceNodeId].GraphId, mNodes[pTargetNodeId].GraphId ) );
            mGraph.AddEdge( tEdge.QgEdge );
            // check cycle
            if( !mGraph.IsDirectedAcyclicGraph() ) {
                mGraph.RemoveEdge( tEdge.QgEdge );
                return false;
            }
            mEdges.Add( tEdge );

            return true;
        }
        /// <summary><para>Options: SOURCE to use edge's source for searching. TARGET for target, and EITHER for either source or target.</para></summary>
        public List<Edge> GetEdgesWithNodeId( string pNodeId, EdgeEndpointOption pOption = EdgeEndpointOption.Either ) {
            List<Edge> tRes = [];
            foreach( var e in mEdges ) {
                if( pOption switch {
                    EdgeEndpointOption.Source => e.StartsWith( pNodeId ),
                    EdgeEndpointOption.Target => e.EndsWith( pNodeId ),
                    EdgeEndpointOption.Either => e.EitherWith( pNodeId ),
                    _ => e.EitherWith( pNodeId )
                } ) {
                    tRes.Add( e );
                }
            }
            return tRes;
        }
        /// <summary>Return an edge with the same direction, otherwise null.</summary>
        public Edge GetEdge( string pSourceNodeId, string pTargetNodeId ) {
            foreach( var e in mEdges ) {
                if( e.BothWith( pSourceNodeId, pTargetNodeId ) ) return e;
            }
            return null;
        }

        /// <summary>Return false if no edge has the endpoint with given type equal to nodeId, otherwise true.</summary>
        public bool RemoveEdgesContainingNodeId( string pNodeId, EdgeEndpointOption pOption = EdgeEndpointOption.Either ) {
            var tEdges = GetEdgesWithNodeId( pNodeId, pOption );
            if( tEdges.Count == 0 ) return false;
            foreach( var e in tEdges ) {
                mGraph.RemoveEdge( e.QgEdge );
                mEdges.Remove( e );
            }
            return true;
        }
        /// <summary>Return true if an edge is removed, otherwise false.</summary>
        public bool RemoveEdge( string pSourceNodeId, string pTargetNodeId ) {
            var tEdge = GetEdge( pSourceNodeId, pTargetNodeId );
            if( tEdge == null ) return false;
            mGraph.RemoveEdge( tEdge.QgEdge );
            mEdges.Remove( tEdge );
            return true;
        }
        /// <summary>
        /// <para>Check if node exists in this canvas' collection and map.</para>
        /// </summary>
        public bool HasNode( string pNodeId ) => _nodeIds.Contains( pNodeId ) && mMap.CheckNodeExist( pNodeId );
        /// <summary> Return a node with given id, null if none is found.</summary>
        public Node GetNode( string pNodeId ) {
            if( !mNodes.TryGetValue( pNodeId, out var node ) ) return null;
            return node;
        }

        public void MoveCanvas( Vector2 pDelta ) {
            mMap.AddBaseOffset( pDelta );
        }
        public void MinimizeSelectedNodes() {
            foreach( var nid in _selectedNodes ) {
                if( mNodes.TryGetValue( nid, out var n ) && n != null ) {
                    n.Minimize();
                }
            }
        }
        public void UnminimizeSelectedNodes() {
            foreach( var nid in _selectedNodes ) {
                if( mNodes.TryGetValue( nid, out var n ) && n != null ) {
                    n.Unminimize();
                }
            }
        }
        public void RemoveSelectedNodes() {
            foreach( var nid in _selectedNodes ) {
                RemoveNode( nid );
            }
        }
        /// <summary>Might be useful for caller to detect changes in selected nodes of on this canvas.</summary>
        public int GetSelectedCount() => _selectedNodes.Count;
        /// <summary>Will not pack if node does not have PackingUnderway status</summary>
        private void PackNode( Node pPacker ) {
            if( pPacker.Packing != Node.PackingStatus.PackingUnderway ) return;
            if( !mGraph.TryGetOutEdges( pPacker.GraphId, out var tEdges ) || tEdges == null ) return;
            _packNodeWalker( pPacker.GraphId, ref pPacker.Pack, pPacker.Id );
            if( pPacker.Pack.Count == 0 ) {
                pPacker.Packing = Node.PackingStatus.None;
                return;
            }
            pPacker.RelaPosLastPackingCall = mMap.GetNodeRelaPos( pPacker.Id );
            pPacker.Packing = Node.PackingStatus.PackingDone;
        }
        private void _packNodeWalker( int pId, ref HashSet<string> pNodeIds, string pPackerNodeId ) {
            if( !mGraph.TryGetOutEdges( pId, out var tEdges ) || tEdges == null ) return;
            foreach( var outEdge in tEdges ) {
                var tTargetNodeId = GetNodeIdWithNodeGraphId( outEdge.Target );
                if( tTargetNodeId == null ) continue;
                if( !mNodes.TryGetValue( tTargetNodeId, out var n ) || n == null ) continue;

                if( n.PackerNodeId != null ) continue;      // ignore nodes that are already packed

                n.IsPacked = true;
                n.PackerNodeId = pPackerNodeId;
                pNodeIds.Add( tTargetNodeId );
                // free cache path + packer
                _cachePathToPTarget.Clear();
                _cachePathToPSource.Clear();
                _packNodeWalker( outEdge.Target, ref pNodeIds, pPackerNodeId );
            }
        }
        /// <summary>
        /// Will unpack if node's status is None and its pack still not empty.
        /// <para>A node can only be unpacked by its packer.</para>
        /// </summary>
        private void UnpackNode( Node pPacker ) {
            if( pPacker.Packing != Node.PackingStatus.UnpackingUnderway || pPacker.Pack.Count == 0 ) return;
            Vector2? tRelaPosDelta = null;
            var tCurrRelaPos = mMap.GetNodeRelaPos( pPacker.Id );
            if( pPacker.RelaPosLastPackingCall.HasValue && tCurrRelaPos.HasValue ) {
                tRelaPosDelta = tCurrRelaPos - pPacker.RelaPosLastPackingCall.Value;
            }
            foreach( var packedNodeId in pPacker.Pack ) {
                if( !mNodes.TryGetValue( packedNodeId, out var n ) || n == null ) continue;

                if( n.PackerNodeId == pPacker.Id )     // only packer can unpack the node
                {
                    n.PackerNodeId = null;
                    n.IsPacked = false;

                    // Set pack's nodes to new pos
                    var npos = mMap.GetNodeRelaPos( n.Id );
                    if( tRelaPosDelta.HasValue && npos.HasValue ) {
                        mMap.SetNodeRelaPos( n.Id, npos.Value + tRelaPosDelta.Value );
                    }
                }
            }
            pPacker.Pack.Clear();
            pPacker.RelaPosLastPackingCall = null;
            pPacker.Packing = Node.PackingStatus.None;
            // free cache path + packer
            _cachePathToPTarget.Clear();
            _cachePathToPSource.Clear();
        }
        private void SelectAllChild( Node pParent ) {
            if( !mGraph.TryGetOutEdges( pParent.GraphId, out var tEdges ) || tEdges == null ) return;
            _selectedNodes.Add( pParent.Id );
            _selectAllChildWalker( pParent.GraphId );
        }
        private void _selectAllChildWalker( int pId ) {
            if( !mGraph.TryGetOutEdges( pId, out var tEdges ) || tEdges == null ) return;
            foreach( var outEdge in tEdges ) {
                var tTargetNodeId = GetNodeIdWithNodeGraphId( outEdge.Target );
                if( tTargetNodeId == null ) continue;
                _selectedNodes.Add( tTargetNodeId );

                _selectAllChildWalker( outEdge.Target );
            }
        }
        public void MarkUnneedInitOfs() => mMap.MarkUnneedInitOfs();
        private bool AddNodeLookUpData( string pNodeId, string pData ) => _nodeLookUpData.TryAdd( pNodeId, pData );
        private bool RemoveNodeLookUpData( string pNodeId ) => _nodeLookUpData.Remove( pNodeId );
        /// <summary> Returns a list if node ids with matching value (partial match is allowed). </summary>
        public List<string> LookUpNode( string pValue ) {
            List<string> tRes = [];
            if( _cacheLookUpValue.Item1 != pValue ) {
                foreach( var d in _nodeLookUpData ) {
                    if( d.Value.Contains( pValue, StringComparison.CurrentCultureIgnoreCase ) ) {
                        tRes.Add( d.Key );
                    }
                }
                _cacheLookUpValue = new( pValue, tRes );
            }
            return _cacheLookUpValue.Item2;
        }
        /// <summary> Returns a list if node ids with matching tag. </summary>
        public List<string> LookUpNodeWithTag( string pTag ) {
            List<string> tRes = [];
            foreach( var n in mNodes.Values ) {
                if( n.Tag == pTag ) tRes.Add( n.Id );
            }
            return tRes;
        }


        public NodeInputProcessResult ProcessInputOnNode( Node pNode, Vector2 pNodeOSP, InputPayload pInputPayload, bool pReadClicks ) {
            var tIsNodeHandleClicked = false;
            var tIsNodeClicked = false;
            var tIsCursorWithin = pNode.Style.CheckPosWithin( pNodeOSP, mConfig.scaling, pInputPayload.mMousePos );
            var tIsCursorWithinHandle = pNode.Style.CheckPosWithinHandle( pNodeOSP, mConfig.scaling, pInputPayload.mMousePos );
            var tIsMarkedForDelete = false;
            var tIsMarkedForSelect = false;
            var tIsMarkedForDeselect = false;
            var tIsReqqingClearSelect = false;
            var tIsEscapingMultiselect = false;
            var tFirstClick = FirstClickType.None;
            var tCDFRes = CanvasDrawFlags.None;

            // Process node delete (we don't delete node in this method, but pass it to Draw())
            if( pReadClicks && !tIsNodeHandleClicked && pInputPayload.mIsMouseMid ) {
                if( tIsCursorWithinHandle ) tIsMarkedForDelete = true;
            }
            else if( pNode.IsMarkedDeleted ) {
                tIsMarkedForDelete = true;
            }
            // Process node select (on lmb release)
            if( pReadClicks && !tIsNodeHandleClicked && pInputPayload.mIsMouseLmb ) {
                if( tIsCursorWithinHandle ) {
                    tIsNodeClicked = true;
                    tIsNodeHandleClicked = true;
                    // single-selecting a node and deselect other node (while in multiselecting)
                    if( !pInputPayload.mIsKeyCtrl && !pInputPayload.mIsALmbDragRelease && _selectedNodes.Count > 1 ) {
                        tIsEscapingMultiselect = true;
                        //pReadClicks = false;
                        tIsReqqingClearSelect = true;
                        tIsMarkedForSelect = true;
                    }
                }
                else if( tIsCursorWithin ) {
                    tIsNodeClicked = true;
                }
            }
            // Process node holding and dragging, except for when multiselecting
            if( pInputPayload.mIsMouseLmbDown )          // if mouse is hold, and the holding's first pos is within a selected node
            {                                           // then mark state as being dragged
                                                        // as long as the mouse is hold, even if mouse then moving out of node zone
                                                        // First click in drag
                if( !_isNodeBeingDragged && _isFirstFrameAfterLmbDown ) {
                    if( tIsCursorWithin ) {
                        if( tIsCursorWithinHandle )
                            tFirstClick = FirstClickType.Handle;
                        else
                            tFirstClick = FirstClickType.Body;
                    }
                }

                if( !_isNodeBeingDragged
                    && tFirstClick != FirstClickType.None
                    && !tIsNodeHandleClicked ) {
                    if( tFirstClick == FirstClickType.Handle ) {
                        tIsNodeHandleClicked = true;
                        // multi-selecting
                        if( pInputPayload.mIsKeyCtrl ) {
                            // select (should be true, regardless of node's select status)
                            tIsMarkedForSelect = true;
                            // remove (process selecting first, then deselecting the node)
                            if( _selectedNodes.Contains( pNode.Id ) )
                                tIsMarkedForDeselect = true;
                        }
                        // single-selecting new node
                        else if( !pInputPayload.mIsKeyCtrl )     // don't check if node is alrady selected here
                        {
                            _snappingNode = pNode;
                            if( !_selectedNodes.Contains( pNode.Id ) ) tIsReqqingClearSelect = true;
                            tIsMarkedForSelect = true;
                        }
                    }
                    else if( tFirstClick == FirstClickType.Body ) {
                        tIsNodeClicked = true;
                    }
                }

                // determine node drag
                if( !_isNodeBeingDragged
                    && _firstClickInDrag == FirstClickType.Handle
                    && !pInputPayload.mIsKeyCtrl
                    && !pInputPayload.mIsKeyShift ) {
                    if( pInputPayload.mLmbDragDelta != null ) {
                        _isNodeBeingDragged = true;
                    }
                }
            }
            else {
                _isNodeBeingDragged = false;
                _snappingNode = null;
            }

            if( tIsCursorWithin ) tCDFRes |= CanvasDrawFlags.NoCanvasZooming;

            NodeInputProcessResult tRes = new() {
                isNodeHandleClicked = tIsNodeHandleClicked,
                readClicks = pReadClicks,
                isNodeClicked = tIsNodeClicked,
                firstClick = tFirstClick,
                CDFRes = tCDFRes,
                isMarkedForDelete = tIsMarkedForDelete,
                isWithin = tIsCursorWithin,
                isWithinHandle = tIsCursorWithinHandle,
                isMarkedForSelect = tIsMarkedForSelect,
                isMarkedForDeselect = tIsMarkedForDeselect,
                isReqqingClearSelect = tIsReqqingClearSelect,
                isEscapingMultiselect = tIsEscapingMultiselect
            };

            return tRes;
        }
        public CanvasDrawFlags ProcessInputOnCanvas( InputPayload pInputPayload, CanvasDrawFlags pCanvasDrawFlagIn ) {
            var pCanvasDrawFlags = CanvasDrawFlags.None;
            // Mouse drag
            if( pInputPayload.mLmbDragDelta.HasValue ) {
                mMap.AddBaseOffset( pInputPayload.mLmbDragDelta.Value / mConfig.scaling );
                pCanvasDrawFlags |= CanvasDrawFlags.StateCanvasDrag;
            }
            // Mouse wheel zooming
            if( !pCanvasDrawFlagIn.HasFlag( CanvasDrawFlags.NoCanvasZooming ) ) {
                switch( pInputPayload.mMouseWheelValue ) {
                    case 1:
                        mConfig.scaling += NodeCanvas.StepScale;
                        pCanvasDrawFlags |= CanvasDrawFlags.StateCanvasDrag;
                        break;
                    case -1:
                        mConfig.scaling -= NodeCanvas.StepScale;
                        pCanvasDrawFlags |= CanvasDrawFlags.StateCanvasDrag;
                        break;
                };
            }
            return pCanvasDrawFlags;
        }
        /// <summary>
        /// Interactable:   Window active. Either cursor within viewer, 
        ///                 or cursor can be outside of viewer while holding the viewer.
        /// <para>Base isn't necessarily Viewer. It is recommended that Base is a point in the center of Viewer.</para>
        /// </summary>
        public CanvasDrawFlags Draw(
            Vector2 pBaseOSP,               // Base isn't necessarily Viewer. In this case, Base is a point in the center of Viewer.
            Vector2 pViewerOSP,         // Viewer OSP.
            Vector2 pViewerSize,
            Vector2 pInitBaseOffset,
            float pGridSnapProximity,
            InputPayload pInputPayload,
            ImDrawListPtr pDrawList,
            GridSnapData pSnapData = null,
            CanvasDrawFlags pCanvasDrawFlag = CanvasDrawFlags.None ) {
            var tIsAnyNodeHandleClicked = false;
            var tIsReadingClicksOnNode = true;
            var tIsAnyNodeClicked = false;
            var tIsAnySelectedNodeInteracted = false;
            Area tSelectScreenArea = null;
            Vector2? tSnapDelta = null;

            // Get this canvas' origin' screenPos   (only scaling for zooming)
            if( mMap.CheckNeedInitOfs() ) {
                mMap.AddBaseOffset( pInitBaseOffset );
                mMap.MarkUnneedInitOfs();
            }
            var tCanvasOSP = pBaseOSP + mMap.GetBaseOffset() * mConfig.scaling;

            if( pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) )     // clean up stuff in case viewer is involuntarily lose focus, to avoid potential accidents.
            {
                _lastSnapDelta = null;
                _snappingNode = null;
                _isNodeBeingDragged = false;
            }

            // Capture selectArea
            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) ) {
                // Capture selectAreaOSP
                if( !_isNodeBeingDragged && pInputPayload.mIsKeyShift && pInputPayload.mIsMouseLmbDown ) {
                    if( !_selectAreaOSP.HasValue ) _selectAreaOSP = pInputPayload.mMousePos;
                }
                else _selectAreaOSP = null;

                // Capture selectArea
                if( _selectAreaOSP != null ) {
                    tSelectScreenArea = new( _selectAreaOSP.Value, pInputPayload.mMousePos, true );
                }
            }

            // Populate snap data
            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) ) {
                foreach( var node in mNodes.Values ) {
                    var tNodeOSP = mMap.GetNodeScreenPos( node.Id, tCanvasOSP, mConfig.scaling );
                    if( tNodeOSP == null ) continue;
                    if( _snappingNode != null && node.Id != _snappingNode.Id && !_selectedNodes.Contains( node.Id ) )  // avoid snapping itself & selected nodess
                    {
                        pSnapData?.AddUsingPos( tNodeOSP.Value );
                    }
                }
                // Get snap delta
                if( _snappingNode != null ) {
                    var tNodeOSP = mMap.GetNodeScreenPos( _snappingNode.Id, tCanvasOSP, mConfig.scaling );
                    Vector2? tSnapOSP = null;
                    if( tNodeOSP.HasValue )
                        tSnapOSP = pSnapData?.GetClosestSnapPos( tNodeOSP.Value, pGridSnapProximity );
                    if( tSnapOSP.HasValue )
                        tSnapDelta = tSnapOSP.Value - tNodeOSP;
                    _lastSnapDelta = tSnapDelta;
                }
            }

            // =====================
            // Draw
            // =====================
            var tFirstClickScanRes = FirstClickType.None;
            var tIsAnyNodeBusy = false;
            var tIsLockingSelection = false;
            var tIsRemovingConn = false;        // for outside node drawing loop

            // Draw edges
            List<Edge> tEdgeToRemove = [];
            foreach( var e in mEdges ) {
                if( !mNodes.TryGetValue( e.SourceNodeId, out var tSourceNode )
                    || !mNodes.TryGetValue( e.TargetNodeId, out var tTargetNode ) ) continue;

                // Check pack
                //if (tSourceNode.mIsPacked) continue;        // ignore if source is packed
                var _currTPackerId = tTargetNode.PackerNodeId;
                var _currSPackerId = tSourceNode.PackerNodeId;
                Node tFinalTarget = null;          // null if there's no pack or there's something wrong with pack data
                Node tFinalSource = null;
                if( tTargetNode.IsPacked && _currTPackerId == null ) continue;
                if( tSourceNode.IsPacked && _currSPackerId == null ) continue;
                // Case: Target is also a PSource
                if( tTargetNode.PackerNodeId != null && tTargetNode.PackerNodeId == tSourceNode.Id ) {
                    continue;         // avoid drawing edge from packer to packed node.
                }
                // Getting final target
                var path = tSourceNode.Id + tTargetNode.Id;
                if( _cachePathToPTarget.TryGetValue( path, out var PTargetId ) && PTargetId != null )     // try retrieving finalTarget from cache
                {
                    if( mNodes.TryGetValue( PTargetId, out var PTarget ) && PTarget != null ) {
                        tFinalTarget = PTarget;
                        _currTPackerId = null;
                    }
                }
                else {
                    while( _currTPackerId != null ) {
                        if( mNodes.TryGetValue( _currTPackerId, out var iFinalTarget ) && iFinalTarget != null ) {
                            tFinalTarget = iFinalTarget;
                            _currTPackerId = tFinalTarget.PackerNodeId;
                        }
                    }
                    if( tFinalTarget != null ) {
                        _cachePathToPTarget.Add( path, tFinalTarget.Id );     // cache the path
                    }
                }
                // Getting final source
                if( _cachePathToPSource.TryGetValue( path, out var PSourceId ) && PSourceId != null ) {
                    if( mNodes.TryGetValue( PSourceId, out var PSource ) && PSource != null ) {
                        tFinalSource = PSource;
                        _currSPackerId = null;
                    }
                }
                else {
                    while( _currSPackerId != null ) {
                        if( mNodes.TryGetValue( _currSPackerId, out var iFinalSource ) && iFinalSource != null ) {
                            tFinalSource = iFinalSource;
                            _currSPackerId = tFinalSource.PackerNodeId;
                        }
                    }
                    if( tFinalSource != null ) {
                        _cachePathToPSource.Add( path, tFinalSource.Id );
                    }
                }

                if( tFinalTarget != null && tFinalTarget.Id == tSourceNode.Id ) continue;         // avoid drawing edge from packer to packed node.
                if( tFinalSource != null && tFinalSource.Id == tSourceNode.Id ) continue;
                if( tFinalSource != null && tFinalTarget != null && tFinalSource.Id == tFinalTarget.Id ) continue;

                var tFinalSourceOSP = mMap.GetNodeScreenPos( tFinalSource != null ? tFinalSource.Id : tSourceNode.Id, tCanvasOSP, mConfig.scaling );
                if( !tFinalSourceOSP.HasValue ) continue;
                var tFinalTargetOSP = mMap.GetNodeScreenPos( tFinalTarget != null ? tFinalTarget.Id : tTargetNode.Id, tCanvasOSP, mConfig.scaling );
                if( !tFinalTargetOSP.HasValue ) continue;

                // Skip rendering if both ends of an edge is out of view
                if( !NodeUtils.IsLineIntersectRect( tFinalSourceOSP.Value, tFinalTargetOSP.Value, new( pViewerOSP, pViewerSize ) ) ) {
                    continue;
                }

                var tEdgeRes = e.Draw(
                    pDrawList,
                    tFinalSourceOSP.Value,
                    tFinalTargetOSP.Value,
                    highlighted: _selectedNodes.Contains( e.SourceNodeId ),
                    highlightedNegative: _selectedNodes.Contains( e.TargetNodeId ),
                    targetPacked: tFinalTarget != null || tFinalSource != null );
                if( tEdgeRes.HasFlag( NodeInteractionFlags.Edge ) ) pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasDrag;
                if( tEdgeRes.HasFlag( NodeInteractionFlags.RequestEdgeRemoval ) ) tEdgeToRemove.Add( e );
            }
            foreach( var e in tEdgeToRemove ) RemoveEdge( e.SourceNodeId, e.TargetNodeId );
            // Draw nodes
            List<Tuple<Seed, string>> tSeedToAdd = [];
            Stack<LinkedListNode<string>> tNodeToFocus = new();
            Stack<string> tNodeToSelect = new();
            Stack<string> tNodeReqqingHandleCtxMenu = new();
            List<string> tNodeToDeselect = [];
            string tNodeToDelete = null;
            HashSet<string> tNodesReqqingClearSelect = [];
            var tIsEscapingMultiselect = false;
            for( var znode = _nodeRenderZOrder.First; znode != null; znode = znode?.Next ) {
                if( znode == null ) break;
                var id = znode.Value;
                // Get NodeOSP
                var tNodeOSP = mMap.GetNodeScreenPos( id, tCanvasOSP, mConfig.scaling );
                if( tNodeOSP == null ) continue;
                if( !mNodes.TryGetValue( id, out var tNode ) || tNode == null ) continue;

                // Packing / Check pack
                if( tNode.IsPacked ) continue;
                switch( tNode.Packing ) {
                    case Node.PackingStatus.PackingUnderway: PackNode( tNode ); break;
                    case Node.PackingStatus.UnpackingUnderway: UnpackNode( tNode ); break;
                }
                // Skip rendering if node is out of view
                if( ( ( tNodeOSP.Value.X + tNode.Style.GetSizeScaled( GetScaling() ).X ) < pViewerOSP.X || tNodeOSP.Value.X > pViewerOSP.X + pViewerSize.X )
                    || ( ( tNodeOSP.Value.Y + tNode.Style.GetSizeScaled( GetScaling() ).Y ) < pViewerOSP.Y || tNodeOSP.Value.Y > pViewerOSP.Y + pViewerSize.Y ) ) {
                    continue;
                }

                // Process input on node
                // We record the inputs of each individual node.
                // Then, we evaluate those recorded inputs in context of z-order, determining which one we need and which we don't.
                if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) && !_isNodeSelectionLocked ) {
                    // Process input on node    (tIsNodeHandleClicked, pReadClicks, tIsNodeClicked, tFirstClick)
                    var t = ProcessInputOnNode( tNode, tNodeOSP.Value, pInputPayload, tIsReadingClicksOnNode );
                    {
                        if( t.firstClick != FirstClickType.None ) {
                            tFirstClickScanRes = ( t.firstClick == FirstClickType.Body && _selectedNodes.Contains( id ) )
                                                 ? FirstClickType.BodySelected
                                                 : t.firstClick;
                        }
                        if( t.isEscapingMultiselect ) tIsEscapingMultiselect = true;
                        if( t.isNodeHandleClicked ) {
                            tIsAnyNodeHandleClicked = t.isNodeHandleClicked;
                        }
                        if( t.isNodeHandleClicked && pInputPayload.mIsMouseLmbDown ) {
                            // Queue the focus nodes
                            if( znode != null ) {
                                tNodeToFocus.Push( znode );
                            }
                        }
                        else if( pInputPayload.mIsMouseLmbDown && t.isWithin && !t.isWithinHandle )     // if an upper node's body covers the previously chosen nodes, discard the focus/selection queue.
                        {
                            tNodeToFocus.Clear();
                            tNodeToSelect.Clear();
                            tFirstClickScanRes = tFirstClickScanRes == FirstClickType.BodySelected
                                                 ? FirstClickType.BodySelected
                                                 : FirstClickType.Body;
                        }
                        tIsReadingClicksOnNode = t.readClicks;
                        if( t.isNodeClicked ) {
                            tIsAnyNodeClicked = true;
                            if( _selectedNodes.Contains( id ) ) tIsAnySelectedNodeInteracted = true;
                        }
                        pCanvasDrawFlag |= t.CDFRes;
                        if( t.isMarkedForDelete )       // proc node delete, only one node deletion allowed per button interaction.
                                                        // If the interaction picks up multiple nodes, choose the highest one in the z-order (last one rendered)
                        {
                            tNodeToDelete = tNode.Id;      // the upper node will override the lower chosen node
                        }
                        else if( t.isNodeClicked && !t.isNodeHandleClicked )     // if an upper node's body covers a lower node that was chosen, nullify the deletion call.
                        {
                            tNodeToDelete = null;
                        }
                        if( t.isMarkedForSelect )                                       // Process node adding
                                                                                        // prevent marking multiple handles with a single lmbDown. Get the uppest node.
                        {
                            if( pInputPayload.mIsMouseLmbDown )      // this one is for lmbDown. General use.
                            {
                                if( pInputPayload.mIsKeyCtrl || tNodeToSelect.Count == 0 ) tNodeToSelect.Push( tNode.Id );
                                else if( tNodeToSelect.Count != 0 ) {
                                    tNodeToSelect.Pop();
                                    tNodeToSelect.Push( tNode.Id );
                                }
                            }
                            else if( pInputPayload.mIsMouseLmb && t.isEscapingMultiselect )     // this one is for lmbClick. Used for when the node is marked at lmb is lift up.
                            {
                                tNodeToSelect.TryPop( out var _ );
                                tNodeToSelect.Push( tNode.Id );
                            }
                        }
                        if( t.isMarkedForDeselect ) {
                            tNodeToDeselect.Add( tNode.Id );
                        }
                        if( t.isReqqingClearSelect ) tNodesReqqingClearSelect.Add( tNode.Id );
                        // Handle ctx menu
                        if( t.isWithinHandle && pInputPayload.mIsMouseRmb ) tNodeReqqingHandleCtxMenu.Push( tNode.Id );
                    }

                    if( tNode.Style.CheckPosWithin( tNodeOSP.Value, GetScaling(), pInputPayload.mMousePos )
                        && pInputPayload.mMouseWheelValue != 0
                        && _selectedNodes.Contains( id ) ) {
                        pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasZooming;
                    }
                    // Select using selectArea
                    if( tSelectScreenArea != null && !_isNodeBeingDragged && _firstClickInDrag != FirstClickType.Handle ) {
                        if( tNode.Style.CheckAreaIntersect( tNodeOSP.Value, mConfig.scaling, tSelectScreenArea ) ) {
                            if( _selectedNodes.Add( id ) && znode != null )
                                tNodeToFocus.Push( znode );
                        }
                    }
                }

                // Draw using NodeOSP
                var tNodeRes = tNode.Draw(
                    tSnapDelta != null && _selectedNodes.Contains( id )
                        ? tNodeOSP.Value + tSnapDelta.Value
                        : tNodeOSP.Value,
                    mConfig.scaling,
                    _selectedNodes.Contains( id ),
                    pInputPayload,
                    pIsEstablishingConn: _nodeConnTemp != null && _nodeConnTemp.IsSource( tNode.Id ),
                    pIsDrawingHndCtxMnu: _nodeQueueingHndCtxMnu != null && tNode.Id == _nodeQueueingHndCtxMnu );

                if( _nodeQueueingHndCtxMnu != null && tNode.Id == _nodeQueueingHndCtxMnu )
                    _nodeQueueingHndCtxMnu = null;
                var tNodeRelaPos = mMap.GetNodeRelaPos( id );
                if( _isNodeBeingDragged && _selectedNodes.Contains( tNode.Id ) && tNodeRelaPos.HasValue )
                    ImGui.GetWindowDrawList().AddText(
                        tNodeOSP.Value + new Vector2( 0, -30 ) * GetScaling()
                        , ImGui.ColorConvertFloat4ToU32( NodeUtils.Colors.NodeText ),
                        $"({( tNodeRelaPos.Value.X / 10 ):F1}, {( tNodeRelaPos.Value.Y / 2 ):F1})" );

                if( tNode.IsBusy ) tIsAnyNodeBusy = true;
                // Process node's content interaction
                if( tNodeRes.HasFlag( NodeInteractionFlags.Internal ) ) pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasInteraction | CanvasDrawFlags.NoNodeDrag | CanvasDrawFlags.NoNodeSnap;
                if( tNodeRes.HasFlag( NodeInteractionFlags.LockSelection ) ) {
                    tIsLockingSelection = true;
                }
                if( _nodeConnTemp != null
                    && pInputPayload.mIsMouseRmb
                    && _nodeConnTemp.IsSource( tNode.Id )
                    && !( tNodeRes.HasFlag( NodeInteractionFlags.RequestingEdgeConn ) ) ) {
                    tIsRemovingConn = true;             // abort connection establishing if RMB outside of connecting plug
                }
                if( tNodeRes.HasFlag( NodeInteractionFlags.RequestSelectAllChild ) ) {
                    pCanvasDrawFlag |= CanvasDrawFlags.NoInteract;      // prevent node unselect
                    SelectAllChild( tNode );
                }
                // Get seed and grow if possible
                var tSeed = tNode.GetSeed();
                if( tSeed != null ) tSeedToAdd.Add( new( tSeed, tNode.Id ) );
                // Node connection
                var tConnRes = _nodeConnTemp?.GetConn();
                if( tConnRes != null )                                                     // implement conn
                {
                    if( !AddEdge( tConnRes.Item1, tConnRes.Item2 ) ) {
                        //pNotiListener.Add( new( $"NodeCanvasEdgeConnection{this.mId}", "Invalid node connection.\n(Connection is either duplicated, or causing cycles)", ViewerNotificationType.Error ) );
                    }
                    _nodeConnTemp = null;
                }
                else if( tNodeRes.HasFlag( NodeInteractionFlags.UnrequestingEdgeConn ) ) {
                    _nodeConnTemp = null;
                }
                else if( tNodeRes.HasFlag( NodeInteractionFlags.RequestingEdgeConn ) )       // setup conn
                {
                    // establishing new conn
                    if( _nodeConnTemp == null ) {
                        _nodeConnTemp = new( tNode.Id );
                    }
                    // connect to existing conn
                    else {
                        _nodeConnTemp.Connect( tNode.Id );
                    }
                }
                // Draw conn tether to cursor
                if( _nodeConnTemp != null && _nodeConnTemp.IsSource( tNode.Id ) ) {
                    pDrawList.AddLine( tNodeOSP.Value, pInputPayload.mMousePos, ImGui.ColorConvertFloat4ToU32( NodeUtils.Colors.NodeFg ) );
                    pDrawList.AddText( pInputPayload.mMousePos, ImGui.ColorConvertFloat4ToU32( NodeUtils.Colors.NodeText ), "[Right-click] another plug to connect, elsewhere to cancel.\n\nConnection will fail if it causes cycling or repeated graph." );
                }
            }
            // Node interaction z-order process (order of op: clearing > selecting > deselecting)
            if( tNodeToSelect.Count != 0 ) pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasDrag;
            if( tNodeToFocus.TryPeek( out var topF ) && tNodesReqqingClearSelect.Contains( topF.Value )
                || tIsEscapingMultiselect )      // only accept a clear-select-req from a node that is on top of the focus queue
            {
                _selectedNodes.Clear();
            }
            foreach( var tId in tNodeToSelect ) _selectedNodes.Add( tId );
            foreach( var tId in tNodeToDeselect ) _selectedNodes.Remove( tId );
            if( tNodeToDelete != null ) RemoveNode( tNodeToDelete );
            if( tNodeReqqingHandleCtxMenu.TryPeek( out var tNodeReqCtxMnu ) && tNodeReqCtxMnu != null )        // handle ctx menu
            {
                _nodeQueueingHndCtxMnu = tNodeReqCtxMnu;
            }
            // Bring to focus (only get the top node)
            if( tNodeToFocus.Count != 0 ) {
                var zFocusNode = tNodeToFocus.Pop();
                if( zFocusNode != null ) {
                    _nodeRenderZOrder.Remove( zFocusNode );
                    _nodeRenderZOrder.AddLast( zFocusNode );
                }
            }
            foreach( var pair in tSeedToAdd ) {
                AddNodeAdjacent( pair.Item1, pair.Item2 );
            }
            if( tIsRemovingConn && _nodeConnTemp?.GetConn() == null ) _nodeConnTemp = null;
            if( tIsLockingSelection ) _isNodeSelectionLocked = true;
            else _isNodeSelectionLocked = false;
            // Capture drag's first click. State Body or Handle can only be accessed from state None.
            if( pInputPayload.mIsMouseLmb ) _firstClickInDrag = FirstClickType.None;
            else if( pInputPayload.mIsMouseLmbDown && _firstClickInDrag == FirstClickType.None && tFirstClickScanRes != FirstClickType.None )
                _firstClickInDrag = tFirstClickScanRes;

            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract )
                && pInputPayload.mIsMouseLmb
                && !tIsAnyNodeBusy
                && ( !tIsAnyNodeHandleClicked && ( pInputPayload.mLmbDragDelta == null ) )
                && !pInputPayload.mIsALmbDragRelease
                && !tIsAnySelectedNodeInteracted ) {
                _selectedNodes.Clear();
            }


            // Draw selectArea
            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) && tSelectScreenArea != null ) {
                ImGui.GetForegroundDrawList().AddRectFilled( tSelectScreenArea.start, tSelectScreenArea.end, ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NodeFg, 0.5f ) ) );
            }

            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) && !tIsAnyNodeBusy ) {
                // Drag selected node
                if( _isNodeBeingDragged
                    && pInputPayload.mLmbDragDelta.HasValue
                    && !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoNodeDrag ) ) {
                    foreach( var id in _selectedNodes ) {
                        mMap.MoveNodeRelaPos(
                            id,
                            pInputPayload.mLmbDragDelta.Value,
                            mConfig.scaling );
                    }
                    pCanvasDrawFlag |= CanvasDrawFlags.StateNodeDrag;
                }
                // Snap if available
                else if( !_isNodeBeingDragged
                         && _lastSnapDelta != null
                         && ( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoNodeDrag ) || !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoNodeSnap ) ) ) {
                    foreach( var id in _selectedNodes ) {
                        mMap.MoveNodeRelaPos(
                            id,
                            _lastSnapDelta.Value,
                            mConfig.scaling );
                    }
                    _lastSnapDelta = null;
                    pCanvasDrawFlag |= CanvasDrawFlags.StateNodeDrag;
                }
                // Process input on canvas
                if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoCanvasInteraction )
                    && !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoCanvasDrag )
                    && ( !_isNodeBeingDragged || ( _isNodeBeingDragged && _firstClickInDrag != FirstClickType.Handle ) )
                    && !tIsAnyNodeClicked
                    && ( _firstClickInDrag == FirstClickType.None || _firstClickInDrag == FirstClickType.Body )
                    && _selectAreaOSP == null ) {
                    pCanvasDrawFlag |= ProcessInputOnCanvas( pInputPayload, pCanvasDrawFlag );
                }
            }

            // Mass delete nodes
            if( pInputPayload.mIsKeyDel ) {
                foreach( var id in _selectedNodes ) {
                    RemoveNode( id );
                }
            }

            // First frame after lmb down. Leave this at the bottom (end of frame drawing).
            if( pInputPayload.mIsMouseLmb ) _isFirstFrameAfterLmbDown = true;
            else if( pInputPayload.mIsMouseLmbDown ) _isFirstFrameAfterLmbDown = false;

            return pCanvasDrawFlag;
        }


        public void Dispose() {
            var tNodeIds = mNodes.Keys.ToList();
            foreach( var id in tNodeIds ) {
                RemoveNode( id );
            }
        }

        public enum FirstClickType {
            None = 0,
            Handle = 1,
            Body = 2,
            BodySelected = 3
        }
        private class EdgeConn {
            private readonly string source;
            private string target;

            private EdgeConn() { }
            public EdgeConn( string sourceNodeId ) => source = sourceNodeId;
            /// <summary>Set the target node. If target is the same as source, return false, otherwise true.</summary>
            public bool Connect( string targetNodeId ) {
                if( source == targetNodeId ) return false;
                target = targetNodeId;
                return true;
            }
            private bool IsEstablished() => target != null;
            public bool IsSource( string nodeId ) => source == nodeId;
            /// <summary>Get a connection between two nodes. If connection is not established, returns null, otherwise a tuple of (source, target)</summary>
            public Tuple<string, string> GetConn() {
                if( !IsEstablished() ) return null;
                return new( source, target! );
            }
        }
        public class PartialCanvasData {
            public List<Node> nodes = [];
            public List<Edge> relatedEdges = [];

            // position-related info
            public string anchorNodeId = null;
            public Dictionary<string, Vector2> offsetFromAnchor = [];
        }

        public struct NodeInputProcessResult {
            public bool isNodeHandleClicked = false;
            public bool isNodeClicked = false;
            public bool isWithin = false;
            public bool isWithinHandle = false;
            public bool isMarkedForDelete = false;
            public bool isMarkedForSelect = false;
            public bool isMarkedForDeselect = false;
            public bool isReqqingClearSelect = false;
            public bool isEscapingMultiselect = false;
            public FirstClickType firstClick = FirstClickType.None;
            public CanvasDrawFlags CDFRes = CanvasDrawFlags.None;
            public bool readClicks = false;
            public NodeInputProcessResult() { }
        }
    }
}

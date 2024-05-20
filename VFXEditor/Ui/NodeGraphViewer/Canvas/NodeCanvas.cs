using ImGuiNET;
using QuickGraph;
using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

        public int Id;
        public string Name;
        private readonly NodeMap Map = new();
        private readonly OccupiedRegion Region;

        public AdjacencyGraph<int, SEdge<int>> Graph;
        public readonly List<Node> Nodes = [];
        public readonly List<Edge> Edges = [];

        private NodeCanvasConfig Config { get; set; } = new();

        private bool NodeBeingDragged = false;
        private readonly HashSet<Node> SelectedNodes = [];
        private readonly LinkedList<Node> NodeRenderZOrder = new();

        private Node SnappingNode = null;
        private Vector2? LastSnapDelta = null;
        private FirstClickType FirstClickInDrag = FirstClickType.None;
        private bool IsFirstFrameAfterLmbDown = true;      // specifically for Draw()
        private Vector2? SelectAreaPosition = null;
        private bool IsNodeSelectionLocked = false;
        private EdgeConn NodeConnTemp = null;

        public NodeCanvas( int pId, string pName = null ) {
            Id = pId;
            Name = pName ?? $"Canvas {Id}";
            Region = new();
            Graph = new();
        }

        public float GetScaling() => Config.Scaling;

        public void SetScaling( float pScale ) => Config.Scaling = pScale;

        public Vector2 GetBaseOffset() => Map.GetBaseOffset();

        private void AddNode( Node node, Vector2 pDrawRelaPos ) {
            // add node
            try {
                if( !Region.IsUpdatedOnce() ) Region.Update( Nodes, Map );
                NodeRenderZOrder.AddLast( node );
                Map.AddNode( node, pDrawRelaPos );
                Nodes.Add( node );
            }
            catch( Exception e ) { Dalamud.Error( e.Message ); }

            Region.Update( Nodes, Map );
            Graph.AddVertex( node.Id );
        }

        public void AddNodeWithinView( Node node, Vector2 pViewerSize ) {
            var tOffset = Map.GetBaseOffset();
            Area pRelaAreaToScanForAvailableRegion = new(
                    -tOffset - pViewerSize * 0.5f,
                    pViewerSize * 0.95f              // only get up until 0.8 of the screen to avoid the new node going out of viewer
                );
            AddNode( node, Region.GetAvailableRelaPos( pRelaAreaToScanForAvailableRegion ) );
        }

        public void AddNodeAdjacent( Node node, Node parent, Vector2? pOffset = null ) {
            Graph.TryGetOutEdges( parent.Id, out var edges );
            if( edges == null ) return;

            Vector2 relativePosition;
            float? chosenY = null;
            Node tChosenNode = null;

            foreach( var e in edges ) {
                var childNode = GetNode( e.Target );
                if( childNode == null ) continue;
                var childPos = Map.GetNodeRelaPos( childNode );
                if( !childPos.HasValue ) continue;
                if( !chosenY.HasValue || childPos.Value.Y > chosenY ) {
                    chosenY = childPos.Value.Y;
                    tChosenNode = childNode ?? tChosenNode;
                }
            }
            // Calc final draw pos
            if( tChosenNode == null ) relativePosition = ( Map.GetNodeRelaPos( parent ) ?? Vector2.One ) + new Vector2( parent.Style.GetSize().X, 0 ) + ( pOffset ?? Vector2.One );
            else {
                relativePosition = new(
                        ( ( Map.GetNodeRelaPos( parent ) ?? Vector2.One ) + new Vector2( parent.Style.GetSize().X, 0 ) + ( pOffset ?? Vector2.One ) ).X,
                        ( Map.GetNodeRelaPos( tChosenNode ) ?? Map.GetNodeRelaPos( parent ) ?? Vector2.One ).Y + tChosenNode.Style.GetSize().Y + ( pOffset ?? Vector2.One ).Y
                    );
            }

            AddNode( node, relativePosition );
        }

        public Node GetNode( int id ) => Nodes.FirstOrDefault( x => x.Id == id );

        public void RemoveNode( Node node, bool _isUpdatingOccupiedRegion = true ) {
            Map.RemoveNode( node );
            node.OnDelete();
            node.Dispose();
            Nodes.Remove( node );
            SelectedNodes.Remove( node );
            NodeRenderZOrder.Remove( node );

            if( _isUpdatingOccupiedRegion ) Region.Update( Nodes, Map );
            // Graph stuff
            RemoveEdgesContainingNode( node );
            Graph.RemoveVertex( node.Id );
        }

        public bool FocusOnNode( Node node, Vector2? pExtraOfs = null ) => Map.FocusOnNode( node, ( pExtraOfs ?? new Vector2( -90, -90 ) ) * GetScaling() );

        public bool AddEdge( Node source, Node target ) {
            if( GetEdge( source, target ) != null ) return false;
            Edge edge = new( source.Id, target.Id, new SEdge<int>( source.Id, target.Id ) );
            Graph.AddEdge( edge.QgEdge );
            // check cycle
            if( !Graph.IsDirectedAcyclicGraph() ) {
                Graph.RemoveEdge( edge.QgEdge );
                return false;
            }
            Edges.Add( edge );

            return true;
        }

        public List<Edge> GetEdgesWithNode( Node node, EdgeEndpointOption pOption = EdgeEndpointOption.Either ) {
            List<Edge> tRes = [];
            foreach( var e in Edges ) {
                if( pOption switch {
                    EdgeEndpointOption.Source => e.StartsWith( node.Id ),
                    EdgeEndpointOption.Target => e.EndsWith( node.Id ),
                    EdgeEndpointOption.Either => e.EitherWith( node.Id ),
                    _ => e.EitherWith( node.Id )
                } ) tRes.Add( e );
            }
            return tRes;
        }

        public Edge GetEdge( Node source, Node target ) {
            foreach( var e in Edges ) if( e.BothWith( source.Id, target.Id ) ) return e;
            return null;
        }

        public bool RemoveEdgesContainingNode( Node node, EdgeEndpointOption pOption = EdgeEndpointOption.Either ) {
            var tEdges = GetEdgesWithNode( node, pOption );
            if( tEdges.Count == 0 ) return false;
            foreach( var e in tEdges ) {
                Graph.RemoveEdge( e.QgEdge );
                Edges.Remove( e );
            }
            return true;
        }

        public bool RemoveEdge( Node source, Node target ) {
            var tEdge = GetEdge( source, target );
            if( tEdge == null ) return false;
            Graph.RemoveEdge( tEdge.QgEdge );
            Edges.Remove( tEdge );
            return true;
        }


        public void MoveCanvas( Vector2 pDelta ) => Map.AddBaseOffset( pDelta );

        public void MinimizeSelectedNodes() {
            foreach( var node in SelectedNodes ) node.Minimize();
        }

        public void UnminimizeSelectedNodes() {
            foreach( var node in SelectedNodes ) node.Unminimize();
        }

        public void RemoveSelectedNodes() {
            foreach( var node in SelectedNodes ) RemoveNode( node );
        }

        public int GetSelectedCount() => SelectedNodes.Count;

        private void SelectAllChild( Node node ) {
            if( !Graph.TryGetOutEdges( node.Id, out var edges ) || edges == null ) return;
            SelectedNodes.Add( node );
            SelectAllChildWalker( node );
        }

        private void SelectAllChildWalker( Node node ) {
            if( !Graph.TryGetOutEdges( node.Id, out var edges ) || edges == null ) return;
            foreach( var outEdge in edges ) {
                var targetNode = GetNode( outEdge.Target );
                if( targetNode == null ) continue;
                SelectedNodes.Add( targetNode );

                SelectAllChildWalker( targetNode );
            }
        }

        public void MarkUnneedInitOfs() => Map.MarkUnneedInitOfs();

        public NodeInputProcessResult ProcessInputOnNode( Node node, Vector2 nodePosition, InputPayload pInputPayload, bool pReadClicks ) {
            var tIsNodeHandleClicked = false;
            var tIsNodeClicked = false;
            var tIsCursorWithin = node.Style.CheckPosWithin( nodePosition, Config.Scaling, pInputPayload.mMousePos );
            var tIsCursorWithinHandle = node.Style.CheckWithinHandle( nodePosition, Config.Scaling, pInputPayload.mMousePos );
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
            else if( node.IsMarkedDeleted ) {
                tIsMarkedForDelete = true;
            }
            // Process node select (on lmb release)
            if( pReadClicks && !tIsNodeHandleClicked && pInputPayload.mIsMouseLmb ) {
                if( tIsCursorWithinHandle ) {
                    tIsNodeClicked = true;
                    tIsNodeHandleClicked = true;
                    // single-selecting a node and deselect other node (while in multiselecting)
                    if( !pInputPayload.mIsKeyCtrl && !pInputPayload.mIsALmbDragRelease && SelectedNodes.Count > 1 ) {
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
                if( !NodeBeingDragged && IsFirstFrameAfterLmbDown ) {
                    if( tIsCursorWithin ) {
                        if( tIsCursorWithinHandle )
                            tFirstClick = FirstClickType.Handle;
                        else
                            tFirstClick = FirstClickType.Body;
                    }
                }

                if( !NodeBeingDragged
                    && tFirstClick != FirstClickType.None
                    && !tIsNodeHandleClicked ) {
                    if( tFirstClick == FirstClickType.Handle ) {
                        tIsNodeHandleClicked = true;
                        // multi-selecting
                        if( pInputPayload.mIsKeyCtrl ) {
                            // select (should be true, regardless of node's select status)
                            tIsMarkedForSelect = true;
                            // remove (process selecting first, then deselecting the node)
                            if( SelectedNodes.Contains( node ) )
                                tIsMarkedForDeselect = true;
                        }
                        // single-selecting new node
                        else if( !pInputPayload.mIsKeyCtrl )     // don't check if node is alrady selected here
                        {
                            SnappingNode = node;
                            if( !SelectedNodes.Contains( node ) ) tIsReqqingClearSelect = true;
                            tIsMarkedForSelect = true;
                        }
                    }
                    else if( tFirstClick == FirstClickType.Body ) {
                        tIsNodeClicked = true;
                    }
                }

                // determine node drag
                if( !NodeBeingDragged
                    && FirstClickInDrag == FirstClickType.Handle
                    && !pInputPayload.mIsKeyCtrl
                    && !pInputPayload.mIsKeyShift ) {
                    if( pInputPayload.mLmbDragDelta != null ) {
                        NodeBeingDragged = true;
                    }
                }
            }
            else {
                NodeBeingDragged = false;
                SnappingNode = null;
            }

            if( tIsCursorWithin ) tCDFRes |= CanvasDrawFlags.NoCanvasZooming;

            NodeInputProcessResult tRes = new() {
                IsNodeHandleClicked = tIsNodeHandleClicked,
                ReadClicks = pReadClicks,
                IsNodeClicked = tIsNodeClicked,
                FirstClick = tFirstClick,
                CDFRes = tCDFRes,
                IsMarkedForDelete = tIsMarkedForDelete,
                IsWithin = tIsCursorWithin,
                IsWithinHandle = tIsCursorWithinHandle,
                IsMarkedForSelect = tIsMarkedForSelect,
                IsMarkedForDeselect = tIsMarkedForDeselect,
                IsReqqingClearSelect = tIsReqqingClearSelect,
                IsEscapingMultiselect = tIsEscapingMultiselect
            };

            return tRes;
        }
        public CanvasDrawFlags ProcessInputOnCanvas( InputPayload pInputPayload, CanvasDrawFlags pCanvasDrawFlagIn ) {
            var pCanvasDrawFlags = CanvasDrawFlags.None;
            // Mouse drag
            if( pInputPayload.mLmbDragDelta.HasValue ) {
                Map.AddBaseOffset( pInputPayload.mLmbDragDelta.Value / Config.Scaling );
                pCanvasDrawFlags |= CanvasDrawFlags.StateCanvasDrag;
            }
            // Mouse wheel zooming
            if( !pCanvasDrawFlagIn.HasFlag( CanvasDrawFlags.NoCanvasZooming ) ) {
                switch( pInputPayload.mMouseWheelValue ) {
                    case 1:
                        Config.Scaling += NodeCanvas.StepScale;
                        pCanvasDrawFlags |= CanvasDrawFlags.StateCanvasDrag;
                        break;
                    case -1:
                        Config.Scaling -= NodeCanvas.StepScale;
                        pCanvasDrawFlags |= CanvasDrawFlags.StateCanvasDrag;
                        break;
                };
            }
            return pCanvasDrawFlags;
        }

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
            if( Map.CheckNeedInitOfs() ) {
                Map.AddBaseOffset( pInitBaseOffset );
                Map.MarkUnneedInitOfs();
            }
            var tCanvasOSP = pBaseOSP + Map.GetBaseOffset() * Config.Scaling;

            if( pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) )     // clean up stuff in case viewer is involuntarily lose focus, to avoid potential accidents.
            {
                LastSnapDelta = null;
                SnappingNode = null;
                NodeBeingDragged = false;
            }

            // Capture selectArea
            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) ) {
                // Capture selectAreaOSP
                if( !NodeBeingDragged && pInputPayload.mIsKeyShift && pInputPayload.mIsMouseLmbDown ) {
                    if( !SelectAreaPosition.HasValue ) SelectAreaPosition = pInputPayload.mMousePos;
                }
                else SelectAreaPosition = null;

                // Capture selectArea
                if( SelectAreaPosition != null ) {
                    tSelectScreenArea = new( SelectAreaPosition.Value, pInputPayload.mMousePos, true );
                }
            }

            // Populate snap data
            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) ) {
                foreach( var node in Nodes ) {
                    var tNodeOSP = Map.GetNodeScreenPos( node, tCanvasOSP, Config.Scaling );
                    if( tNodeOSP == null ) continue;
                    if( SnappingNode != null && node != SnappingNode && !SelectedNodes.Contains( node ) )  // avoid snapping itself & selected nodess
                    {
                        pSnapData?.AddUsingPos( tNodeOSP.Value );
                    }
                }
                // Get snap delta
                if( SnappingNode != null ) {
                    var tNodeOSP = Map.GetNodeScreenPos( SnappingNode, tCanvasOSP, Config.Scaling );
                    Vector2? tSnapOSP = null;
                    if( tNodeOSP.HasValue )
                        tSnapOSP = pSnapData?.GetClosestSnapPos( tNodeOSP.Value, pGridSnapProximity );
                    if( tSnapOSP.HasValue )
                        tSnapDelta = tSnapOSP.Value - tNodeOSP;
                    LastSnapDelta = tSnapDelta;
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
            List<Edge> edgesToRemove = [];
            foreach( var e in Edges ) {
                var sourceNode = GetNode( e.SourceNodeId );
                var targetNode = GetNode( e.TargetNodeId );
                if( sourceNode == null || targetNode == null ) continue;

                var tFinalSourceOSP = Map.GetNodeScreenPos( sourceNode, tCanvasOSP, Config.Scaling );
                if( !tFinalSourceOSP.HasValue ) continue;
                var tFinalTargetOSP = Map.GetNodeScreenPos( targetNode, tCanvasOSP, Config.Scaling );
                if( !tFinalTargetOSP.HasValue ) continue;

                // Skip rendering if both ends of an edge is out of view
                if( !NodeUtils.IsLineIntersectRect( tFinalSourceOSP.Value, tFinalTargetOSP.Value, new( pViewerOSP, pViewerSize ) ) ) {
                    continue;
                }

                var tEdgeRes = e.Draw(
                    pDrawList,
                    sourceNode.GetOutputPosition( tFinalSourceOSP.Value ),
                    tFinalTargetOSP.Value,
                    highlighted: SelectedNodes.Contains( sourceNode ),
                    highlightedNegative: SelectedNodes.Contains( targetNode ) );
                if( tEdgeRes.HasFlag( NodeInteractionFlags.Edge ) ) pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasDrag;
                if( tEdgeRes.HasFlag( NodeInteractionFlags.RequestEdgeRemoval ) ) edgesToRemove.Add( e );
            }
            foreach( var e in edgesToRemove ) RemoveEdge( GetNode( e.SourceNodeId ), GetNode( e.TargetNodeId ) );

            // Draw nodes
            Stack<LinkedListNode<Node>> tNodeToFocus = new();
            Stack<Node> tNodeToSelect = new();
            List<Node> tNodeToDeselect = [];
            Node tNodeToDelete = null;
            HashSet<Node> tNodesReqqingClearSelect = [];
            var tIsEscapingMultiselect = false;
            for( var znode = NodeRenderZOrder.First; znode != null; znode = znode?.Next ) {
                if( znode == null ) break;
                var node = znode.Value;
                // Get NodeOSP
                var nodePosition = Map.GetNodeScreenPos( node, tCanvasOSP, Config.Scaling );
                if( nodePosition == null ) continue;

                // Skip rendering if node is out of view
                if( ( ( nodePosition.Value.X + node.Style.GetSizeScaled( GetScaling() ).X ) < pViewerOSP.X || nodePosition.Value.X > pViewerOSP.X + pViewerSize.X )
                    || ( ( nodePosition.Value.Y + node.Style.GetSizeScaled( GetScaling() ).Y ) < pViewerOSP.Y || nodePosition.Value.Y > pViewerOSP.Y + pViewerSize.Y ) ) {
                    continue;
                }

                // Process input on node
                // We record the inputs of each individual node.
                // Then, we evaluate those recorded inputs in context of z-order, determining which one we need and which we don't.
                if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) && !IsNodeSelectionLocked ) {
                    // Process input on node    (tIsNodeHandleClicked, pReadClicks, tIsNodeClicked, tFirstClick)
                    var t = ProcessInputOnNode( node, nodePosition.Value, pInputPayload, tIsReadingClicksOnNode );
                    {
                        if( t.FirstClick != FirstClickType.None ) {
                            tFirstClickScanRes = ( t.FirstClick == FirstClickType.Body && SelectedNodes.Contains( node ) )
                                                 ? FirstClickType.BodySelected
                                                 : t.FirstClick;
                        }
                        if( t.IsEscapingMultiselect ) tIsEscapingMultiselect = true;
                        if( t.IsNodeHandleClicked ) {
                            tIsAnyNodeHandleClicked = t.IsNodeHandleClicked;
                        }
                        if( t.IsNodeHandleClicked && pInputPayload.mIsMouseLmbDown ) {
                            // Queue the focus nodes
                            if( znode != null ) {
                                tNodeToFocus.Push( znode );
                            }
                        }
                        else if( pInputPayload.mIsMouseLmbDown && t.IsWithin && !t.IsWithinHandle )     // if an upper node's body covers the previously chosen nodes, discard the focus/selection queue.
                        {
                            tNodeToFocus.Clear();
                            tNodeToSelect.Clear();
                            tFirstClickScanRes = tFirstClickScanRes == FirstClickType.BodySelected
                                                 ? FirstClickType.BodySelected
                                                 : FirstClickType.Body;
                        }
                        tIsReadingClicksOnNode = t.ReadClicks;
                        if( t.IsNodeClicked ) {
                            tIsAnyNodeClicked = true;
                            if( SelectedNodes.Contains( node ) ) tIsAnySelectedNodeInteracted = true;
                        }
                        pCanvasDrawFlag |= t.CDFRes;
                        if( t.IsMarkedForDelete )       // proc node delete, only one node deletion allowed per button interaction.
                                                        // If the interaction picks up multiple nodes, choose the highest one in the z-order (last one rendered)
                        {
                            tNodeToDelete = node;      // the upper node will override the lower chosen node
                        }
                        else if( t.IsNodeClicked && !t.IsNodeHandleClicked )     // if an upper node's body covers a lower node that was chosen, nullify the deletion call.
                        {
                            tNodeToDelete = null;
                        }
                        if( t.IsMarkedForSelect )                                       // Process node adding
                                                                                        // prevent marking multiple handles with a single lmbDown. Get the uppest node.
                        {
                            if( pInputPayload.mIsMouseLmbDown )      // this one is for lmbDown. General use.
                            {
                                if( pInputPayload.mIsKeyCtrl || tNodeToSelect.Count == 0 ) tNodeToSelect.Push( node );
                                else if( tNodeToSelect.Count != 0 ) {
                                    tNodeToSelect.Pop();
                                    tNodeToSelect.Push( node );
                                }
                            }
                            else if( pInputPayload.mIsMouseLmb && t.IsEscapingMultiselect )     // this one is for lmbClick. Used for when the node is marked at lmb is lift up.
                            {
                                tNodeToSelect.TryPop( out var _ );
                                tNodeToSelect.Push( node );
                            }
                        }
                        if( t.IsMarkedForDeselect ) {
                            tNodeToDeselect.Add( node );
                        }
                        if( t.IsReqqingClearSelect ) tNodesReqqingClearSelect.Add( node );
                    }

                    if( node.Style.CheckPosWithin( nodePosition.Value, GetScaling(), pInputPayload.mMousePos )
                        && pInputPayload.mMouseWheelValue != 0
                        && SelectedNodes.Contains( node ) ) {
                        pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasZooming;
                    }
                    // Select using selectArea
                    if( tSelectScreenArea != null && !NodeBeingDragged && FirstClickInDrag != FirstClickType.Handle ) {
                        if( node.Style.CheckAreaIntersect( nodePosition.Value, Config.Scaling, tSelectScreenArea ) ) {
                            if( SelectedNodes.Add( node ) && znode != null )
                                tNodeToFocus.Push( znode );
                        }
                    }
                }

                // Draw using NodeOSP
                var tNodeRes = node.Draw(
                    tSnapDelta != null && SelectedNodes.Contains( node )
                        ? nodePosition.Value + tSnapDelta.Value
                        : nodePosition.Value,
                    Config.Scaling,
                    SelectedNodes.Contains( node ),
                    pInputPayload,
                    thisNodeConnecting: NodeConnTemp != null && NodeConnTemp.IsSource( node ),
                    someNodeConnecting: NodeConnTemp != null );

                var tNodeRelaPos = Map.GetNodeRelaPos( node );
                if( NodeBeingDragged && SelectedNodes.Contains( node ) && tNodeRelaPos.HasValue )
                    ImGui.GetWindowDrawList().AddText(
                        nodePosition.Value + new Vector2( 0, -30 ) * GetScaling()
                        , ImGui.ColorConvertFloat4ToU32( NodeUtils.Colors.NodeText ),
                        $"({tNodeRelaPos.Value.X / 10:F1}, {tNodeRelaPos.Value.Y / 2:F1})" );

                if( node.IsBusy ) tIsAnyNodeBusy = true;
                // Process node's content interaction
                if( tNodeRes.HasFlag( NodeInteractionFlags.Internal ) ) pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasInteraction | CanvasDrawFlags.NoNodeDrag | CanvasDrawFlags.NoNodeSnap;
                if( tNodeRes.HasFlag( NodeInteractionFlags.LockSelection ) ) {
                    tIsLockingSelection = true;
                }
                if( NodeConnTemp != null
                    && pInputPayload.mIsMouseRmb
                    && NodeConnTemp.IsSource( node )
                    && !tNodeRes.HasFlag( NodeInteractionFlags.RequestingEdgeConn ) ) {

                    tIsRemovingConn = true;             // abort connection establishing if RMB outside of connecting plug
                }
                if( tNodeRes.HasFlag( NodeInteractionFlags.RequestSelectAllChild ) ) {
                    pCanvasDrawFlag |= CanvasDrawFlags.NoInteract;      // prevent node unselect
                    SelectAllChild( node );
                }
                // Node connection
                var tConnRes = NodeConnTemp?.GetConn();
                if( tConnRes != null )                                                     // implement conn
                {
                    if( !AddEdge( tConnRes.Item1, tConnRes.Item2 ) ) {
                        // TODO
                    }
                    NodeConnTemp = null;
                }
                else if( tNodeRes.HasFlag( NodeInteractionFlags.UnrequestingEdgeConn ) ) {
                    NodeConnTemp = null;
                }
                else if( tNodeRes.HasFlag( NodeInteractionFlags.RequestingEdgeConn ) )       // setup conn
                {
                    // establishing new conn
                    if( NodeConnTemp == null ) {
                        NodeConnTemp = new( node );
                    }
                    // connect to existing conn
                    else {
                        NodeConnTemp.Connect( node );
                    }
                }
                // Draw conn tether to cursor
                if( NodeConnTemp != null && NodeConnTemp.IsSource( node ) ) {
                    var startPos = node.GetOutputPosition( nodePosition.Value );
                    var endPos = pInputPayload.mMousePos;
                    var midPos = startPos + ( endPos - startPos ) / 2f;

                    pDrawList.AddBezierCubic( startPos, new( midPos.X, startPos.Y ), new( midPos.X, endPos.Y ), endPos, ImGui.ColorConvertFloat4ToU32( NodeUtils.Colors.NodeFg ), 1f );
                }
            }
            // Node interaction z-order process (order of op: clearing > selecting > deselecting)
            if( tNodeToSelect.Count != 0 ) pCanvasDrawFlag |= CanvasDrawFlags.NoCanvasDrag;
            if( tNodeToFocus.TryPeek( out var topF ) && tNodesReqqingClearSelect.Contains( topF.Value )
                || tIsEscapingMultiselect )      // only accept a clear-select-req from a node that is on top of the focus queue
            {
                SelectedNodes.Clear();
            }
            foreach( var tId in tNodeToSelect ) SelectedNodes.Add( tId );
            foreach( var tId in tNodeToDeselect ) SelectedNodes.Remove( tId );
            if( tNodeToDelete != null ) RemoveNode( tNodeToDelete );
            // Bring to focus (only get the top node)
            if( tNodeToFocus.Count != 0 ) {
                var zFocusNode = tNodeToFocus.Pop();
                if( zFocusNode != null ) {
                    NodeRenderZOrder.Remove( zFocusNode );
                    NodeRenderZOrder.AddLast( zFocusNode );
                }
            }
            if( tIsRemovingConn && NodeConnTemp?.GetConn() == null ) NodeConnTemp = null;
            if( tIsLockingSelection ) IsNodeSelectionLocked = true;
            else IsNodeSelectionLocked = false;
            // Capture drag's first click. State Body or Handle can only be accessed from state None.
            if( pInputPayload.mIsMouseLmb ) FirstClickInDrag = FirstClickType.None;
            else if( pInputPayload.mIsMouseLmbDown && FirstClickInDrag == FirstClickType.None && tFirstClickScanRes != FirstClickType.None )
                FirstClickInDrag = tFirstClickScanRes;

            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract )
                && pInputPayload.mIsMouseLmb
                && !tIsAnyNodeBusy
                && ( !tIsAnyNodeHandleClicked && ( pInputPayload.mLmbDragDelta == null ) )
                && !pInputPayload.mIsALmbDragRelease
                && !tIsAnySelectedNodeInteracted ) {
                SelectedNodes.Clear();
            }


            // Draw selectArea
            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) && tSelectScreenArea != null ) {
                ImGui.GetForegroundDrawList().AddRectFilled( tSelectScreenArea.Start, tSelectScreenArea.End, ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NodeFg, 0.5f ) ) );
            }

            if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoInteract ) && !tIsAnyNodeBusy ) {
                // Drag selected node
                if( NodeBeingDragged
                    && pInputPayload.mLmbDragDelta.HasValue
                    && !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoNodeDrag ) ) {
                    foreach( var id in SelectedNodes ) {
                        Map.MoveNodeRelaPos(
                            id,
                            pInputPayload.mLmbDragDelta.Value,
                            Config.Scaling );
                    }
                    pCanvasDrawFlag |= CanvasDrawFlags.StateNodeDrag;
                }
                // Snap if available
                else if( !NodeBeingDragged
                         && LastSnapDelta != null
                         && ( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoNodeDrag ) || !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoNodeSnap ) ) ) {
                    foreach( var id in SelectedNodes ) {
                        Map.MoveNodeRelaPos(
                            id,
                            LastSnapDelta.Value,
                            Config.Scaling );
                    }
                    LastSnapDelta = null;
                    pCanvasDrawFlag |= CanvasDrawFlags.StateNodeDrag;
                }
                // Process input on canvas
                if( !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoCanvasInteraction )
                    && !pCanvasDrawFlag.HasFlag( CanvasDrawFlags.NoCanvasDrag )
                    && ( !NodeBeingDragged || ( NodeBeingDragged && FirstClickInDrag != FirstClickType.Handle ) )
                    && !tIsAnyNodeClicked
                    && ( FirstClickInDrag == FirstClickType.None || FirstClickInDrag == FirstClickType.Body )
                    && SelectAreaPosition == null ) {
                    pCanvasDrawFlag |= ProcessInputOnCanvas( pInputPayload, pCanvasDrawFlag );
                }
            }

            // Mass delete nodes
            if( pInputPayload.mIsKeyDel ) {
                foreach( var id in SelectedNodes ) {
                    RemoveNode( id );
                }
            }

            // First frame after lmb down. Leave this at the bottom (end of frame drawing).
            if( pInputPayload.mIsMouseLmb ) IsFirstFrameAfterLmbDown = true;
            else if( pInputPayload.mIsMouseLmbDown ) IsFirstFrameAfterLmbDown = false;

            return pCanvasDrawFlag;
        }

        public void Dispose() {
            foreach( var node in new List<Node>( Nodes ) ) RemoveNode( node );
        }

        public enum FirstClickType {
            None = 0,
            Handle = 1,
            Body = 2,
            BodySelected = 3
        }

        private class EdgeConn {
            private readonly Node source;
            private Node target;

            private EdgeConn() { }
            public EdgeConn( Node sourceNodeId ) => source = sourceNodeId;

            public bool Connect( Node targetNodeId ) {
                if( source == targetNodeId ) return false;
                target = targetNodeId;
                return true;
            }

            private bool IsEstablished() => target != null;

            public bool IsSource( Node nodeId ) => source == nodeId;

            public Tuple<Node, Node> GetConn() {
                if( !IsEstablished() ) return null;
                return new( source, target! );
            }
        }

        public struct NodeInputProcessResult {
            public bool IsNodeHandleClicked = false;
            public bool IsNodeClicked = false;
            public bool IsWithin = false;
            public bool IsWithinHandle = false;
            public bool IsMarkedForDelete = false;
            public bool IsMarkedForSelect = false;
            public bool IsMarkedForDeselect = false;
            public bool IsReqqingClearSelect = false;
            public bool IsEscapingMultiselect = false;
            public FirstClickType FirstClick = FirstClickType.None;
            public CanvasDrawFlags CDFRes = CanvasDrawFlags.None;
            public bool ReadClicks = false;
            public NodeInputProcessResult() { }
        }
    }
}

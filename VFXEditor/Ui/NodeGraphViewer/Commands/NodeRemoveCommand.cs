using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.NodeGraphViewer.Canvas;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Ui.NodeGraphViewer.Commands {
    public class NodeRemoveCommand<T> : ListRemoveCommand<T> where T : Node {
        private readonly NodeCanvas<T> Canvas;
        private readonly Vector2 Position;
        private readonly List<(Slot, Slot)> Slots;

        public NodeRemoveCommand( NodeCanvas<T> canvas, T node ) : base( canvas.Nodes, node ) {
            Canvas = canvas;
            Position = Canvas.Map.GetNodeRelaPos( Item ).Value;
            Slots = Canvas.Nodes.SelectMany( node => node.Inputs.Where( slot => slot.Connected.Node == Item ).Select( slot => (slot, slot.Connected) ) ).ToList();
            // ==================
            Canvas.Map.RemoveNode( Item );
            Canvas.SelectedNodes.Remove( Item );
            Canvas.NodeOrder.Remove( Item );
            Canvas.Region.Update( Canvas.Nodes, Canvas.Map );
            foreach( var (slot, _) in Slots ) slot.ConnectTo( null );
        }

        public override void Undo() {
            base.Undo();
            Canvas.Map.AddNode( Item, Position );
            Canvas.NodeOrder.AddLast( Item );
            Canvas.Region.Update( Canvas.Nodes, Canvas.Map );
            foreach( var (slot, connected) in Slots ) slot.ConnectTo( connected );
        }

        public override void Redo() {
            base.Redo();
            Canvas.Map.RemoveNode( Item );
            Canvas.SelectedNodes.Remove( Item );
            Canvas.NodeOrder.Remove( Item );
            Canvas.Region.Update( Canvas.Nodes, Canvas.Map );
            foreach( var (slot, _) in Slots ) slot.ConnectTo( null );
        }
    }
}

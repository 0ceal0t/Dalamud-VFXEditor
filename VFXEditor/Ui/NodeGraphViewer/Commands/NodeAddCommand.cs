using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.NodeGraphViewer.Canvas;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Ui.NodeGraphViewer.Commands {
    public class NodeAddCommand<T, S> : ListAddCommand<T> where T : Node<S> where S : Slot {
        private readonly NodeCanvas<T, S> Canvas;
        private readonly Vector2 Position;

        public NodeAddCommand( NodeCanvas<T, S> canvas, T node, Vector2 position ) : base( canvas.Nodes, node ) {
            Canvas = canvas;
            Position = position;
            // ==================
            Canvas.NodeOrder.AddLast( Item );
            Canvas.Map.AddNode( Item, Position );
            Canvas.Region.Update( Canvas.Nodes, Canvas.Map );
        }

        public override void Undo() {
            base.Undo();
            Canvas.NodeOrder.Remove( Item );
            Canvas.Map.RemoveNode( Item );
            Canvas.SelectedNodes.Clear();
            Canvas.Region.Update( Canvas.Nodes, Canvas.Map );
        }

        public override void Redo() {
            base.Redo();
            Canvas.NodeOrder.AddLast( Item );
            Canvas.Map.AddNode( Item, Position );
            Canvas.Region.Update( Canvas.Nodes, Canvas.Map );
        }
    }
}

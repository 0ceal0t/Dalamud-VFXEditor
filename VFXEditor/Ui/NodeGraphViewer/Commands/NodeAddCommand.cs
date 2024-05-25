using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.NodeGraphViewer.Canvas;

namespace VfxEditor.Ui.NodeGraphViewer.Commands {
    public class NodeAddCommand<T> : ListAddCommand<T> where T : Node {
        private readonly NodeCanvas<T> Canvas;
        private readonly Vector2 Position;

        public NodeAddCommand( NodeCanvas<T> canvas, T node, Vector2 position ) : base( canvas.Nodes, node ) {
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

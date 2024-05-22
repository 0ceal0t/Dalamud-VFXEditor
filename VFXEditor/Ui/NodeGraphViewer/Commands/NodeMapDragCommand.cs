using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Commands {
    public class NodeMapDragCommand : ICommand {
        public readonly Dictionary<Node, Vector2> PrevState;
        public readonly Dictionary<Node, Vector2> State;
        public readonly NodeMap Map;

        public NodeMapDragCommand( NodeMap map, Dictionary<Node, Vector2> prevState ) {
            Map = map;
            PrevState = prevState;
            State = map.GetState();
        }

        public void Redo() => Map.LoadState( State );

        public void Undo() => Map.LoadState( PrevState );
    }
}

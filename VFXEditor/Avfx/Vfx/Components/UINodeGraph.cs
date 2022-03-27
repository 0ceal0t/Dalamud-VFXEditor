using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UINodeGraphItem {
        public int Level;
        public int Level2;
        public List<UINode> Next;
    }

    public class UINodeGraph {
        public Dictionary<UINode, UINodeGraphItem> Graph = new();
        public bool Outdated = false;
        public bool Cycle = false;

        public UINodeGraph( UINode node) {
            ParseGraph( 0, node, new HashSet<UINode>() );
            var L2Dict = new Dictionary<int, int>();
            foreach(var val in Graph.Values ) {
                if( L2Dict.ContainsKey( val.Level ) ) {
                    L2Dict[val.Level] += 1;
                    val.Level2 = L2Dict[val.Level];
                }
                else {
                    L2Dict[val.Level] = 0;
                    val.Level2 = 0;
                }
            }
        }

        public void ParseGraph(int level, UINode node, HashSet<UINode> visited ) {
            if(visited.Contains(node) || Cycle ) {
                Cycle = true;
                return;
            }
            if( Graph.ContainsKey( node ) ) { // already defined
                if(level > Graph[node].Level ) {
                    PushBack( node, level - Graph[node].Level );
                }
                Graph[node].Level = Math.Max( level, Graph[node].Level );
            }
            else {
                visited.Add( node );
                var item = new UINodeGraphItem {
                    Level = level,
                    Next = new List<UINode>()
                };
                foreach(var n in node.Parents ) {
                    item.Next.Add( n.Node );
                    ParseGraph( level + 1, n.Node, new HashSet<UINode>( visited ) );
                }
                Graph[node] = item;
            }
        }

        public void PushBack(UINode node, int amount ) {
            Graph[node].Level += amount;
            foreach(var item in Graph[node].Next ) {
                PushBack( item, amount );
            }
        }

        public void NowOutdated() {
            Outdated = true;
        }
    }
}

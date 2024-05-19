using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public class NodeCanvasConfig {
        private float _scaling;
        public float scaling {
            get { return _scaling; }
            set {
                if( value > NodeCanvas.MaxScale )
                    _scaling = NodeCanvas.MaxScale;
                else if( value < NodeCanvas.MinScale )
                    _scaling = NodeCanvas.MinScale;
                else
                    _scaling = value;
            }
        }
        public Vector2 nodeGap;

        public NodeCanvasConfig() {
            scaling = 1f;
            _scaling = 1f;
            nodeGap = Vector2.One;
        }
    }
}

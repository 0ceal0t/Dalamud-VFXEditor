using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public class NodeCanvasConfig {
        private float ScalingInternal;

        public float Scaling {
            get { return ScalingInternal; }
            set {
                if( value > NodeCanvas.MaxScale )
                    ScalingInternal = NodeCanvas.MaxScale;
                else if( value < NodeCanvas.MinScale )
                    ScalingInternal = NodeCanvas.MinScale;
                else
                    ScalingInternal = value;
            }
        }

        public Vector2 NodeGap;

        public NodeCanvasConfig() {
            Scaling = 1f;
            ScalingInternal = 1f;
            NodeGap = Vector2.One;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public class GridSnapData {
        public List<float> X = new();
        public List<float> Y = new();
        public float? lastClosestSnapX = null;
        public float? lastClosestSnapY = null;

        public void AddUsingPos( Vector2 pos ) {
            this.X.Add( pos.X );
            this.Y.Add( pos.Y );
        }
        public Vector2 GetClosestSnapPos( Vector2 currPos, float proximity ) {
            var tXClosest = NodeUtils.GetClosestItem( currPos.X, this.X );
            var tYClosest = NodeUtils.GetClosestItem( currPos.Y, this.Y );
            var x = tXClosest ?? currPos.X;
            if( Math.Abs( x - currPos.X ) > proximity ) {
                x = currPos.X;
                this.lastClosestSnapX = null;
            }
            else if( tXClosest.HasValue ) this.lastClosestSnapX = x;
            var y = tYClosest ?? currPos.Y;
            if( Math.Abs( y - currPos.Y ) > proximity ) {
                y = currPos.Y;
                this.lastClosestSnapY = null;
            }
            else if( tYClosest.HasValue ) this.lastClosestSnapY = y;
            return new( x, y );
        }
    }
}

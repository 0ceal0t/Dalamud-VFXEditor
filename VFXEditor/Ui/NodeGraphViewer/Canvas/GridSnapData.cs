using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public class GridSnapData {
        public List<float> X = [];
        public List<float> Y = [];
        public float? LastClosestSnapX = null;
        public float? LastClosestSnapY = null;

        public void AddUsingPos( Vector2 pos ) {
            X.Add( pos.X );
            Y.Add( pos.Y );
        }
        public Vector2 GetClosestSnapPos( Vector2 currPos, float proximity ) {
            var tXClosest = NodeUtils.GetClosestItem( currPos.X, X );
            var tYClosest = NodeUtils.GetClosestItem( currPos.Y, Y );
            var x = tXClosest ?? currPos.X;
            if( Math.Abs( x - currPos.X ) > proximity ) {
                x = currPos.X;
                LastClosestSnapX = null;
            }
            else if( tXClosest.HasValue ) LastClosestSnapX = x;
            var y = tYClosest ?? currPos.Y;
            if( Math.Abs( y - currPos.Y ) > proximity ) {
                y = currPos.Y;
                LastClosestSnapY = null;
            }
            else if( tYClosest.HasValue ) LastClosestSnapY = y;
            return new( x, y );
        }
    }
}

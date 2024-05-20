using System.Drawing;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Utils {
    public class Area {
        public Vector2 Start;
        public Vector2 End;
        public Vector2 Size;
        public Vector2 Center;
        public RectangleF Rect;

        public Area( Vector2 pos, Vector2 size ) {
            Size = size;
            Start = pos;
            End = pos + size;
            Rect = new( new PointF( Start ), new SizeF( size ) );
            Center = Start + Size / 2;
        }

        public Area( Vector2 start, Vector2 end, bool _ ) {
            Start = new(
                    start.X < end.X ? start.X : end.X,
                    start.Y < end.Y ? start.Y : end.Y
                );
            End = new(
                    start.X < end.X ? end.X : start.X,
                    start.Y < end.Y ? end.Y : start.Y
                );
            Size = End - Start;
            Rect = new( new PointF( Start ), new SizeF( Size ) );
        }

        public bool CheckPosIsWithin( Vector2 pos ) {
            var xRev = End.X < Start.X;
            var yRev = End.Y < Start.Y;
            return ( xRev ? pos.X < Start.X : pos.X > Start.X )
                && ( xRev ? pos.X > End.X : pos.X < End.X )
                && ( yRev ? pos.Y < Start.Y : pos.Y > Start.Y )
                && ( yRev ? pos.Y > End.Y : pos.Y < End.Y );
        }

        public bool CheckAreaIntersect( Area area ) => Rect.IntersectsWith( area.Rect );
    }
}

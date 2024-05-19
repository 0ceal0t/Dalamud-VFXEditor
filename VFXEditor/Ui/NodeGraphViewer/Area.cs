using System.Drawing;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer {
    public class Area {
        public Vector2 start;
        public Vector2 end;
        public Vector2 size;
        public Vector2 center;
        public RectangleF _rect;

        public Area( Vector2 pos, Vector2 size ) {
            this.size = size;
            this.start = pos;
            this.end = pos + size;
            this._rect = new( new PointF( start ), new SizeF( size ) );
            this.center = this.start + this.size / 2;
        }
        public Area( Vector2 start, Vector2 end, bool isUsingStartEnd ) {
            this.start = new(
                    start.X < end.X ? start.X : end.X,
                    start.Y < end.Y ? start.Y : end.Y
                );
            this.end = new(
                    start.X < end.X ? end.X : start.X,
                    start.Y < end.Y ? end.Y : start.Y
                );
            this.size = this.end - this.start;
            _rect = new( new PointF( this.start ), new SizeF( this.size ) );
        }

        public bool CheckPosIsWithin( Vector2 pos ) {
            bool xRev = end.X < start.X;
            bool yRev = end.Y < start.Y;
            return ( xRev ? ( pos.X < start.X ) : ( pos.X > start.X ) )
                && ( xRev ? ( pos.X > end.X ) : ( pos.X < end.X ) )
                && ( yRev ? ( pos.Y < start.Y ) : ( pos.Y > start.Y ) )
                && ( yRev ? ( pos.Y > end.Y ) : ( pos.Y < end.Y ) );
        }
        public bool CheckAreaIntersect( Area area ) {
            //PluginLog.LogDebug($"> s=({_rect.X}, {_rect.Y}):({_rect.X + _rect.Size.Width}, {_rect.Y + _rect.Size.Height}) p=({area._rect.X}, {area._rect.Y}):({area._rect.X + area._rect.Size.Width}, {area._rect.Y + area._rect.Size.Height})");
            return this._rect.IntersectsWith( area._rect );
        }
    }
}

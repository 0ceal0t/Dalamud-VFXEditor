using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public class OccupiedRegion {
        private readonly Set X = new();
        private readonly Set Y = new();
        private bool _isUpdatedOnce = false;

        public OccupiedRegion() { }

        public bool IsUpdatedOnce() => this._isUpdatedOnce;
        public void Update( Dictionary<string, Node> pNodes, NodeMap pMap ) {
            this.ResetRegion();
            foreach( var n in pNodes.Values ) {
                var tStart = pMap.GetNodeRelaPos( n.Id );
                if( tStart == null ) continue;
                var tEnd = tStart.Value + n.Style.GetSize();
                this.AddRegion( tStart.Value, tEnd );
            }
            this._isUpdatedOnce = true;
        }
        private void AddRegion( Vector2 pStart, Vector2 pEnd ) {
            switch( pStart.X ) {
                case var x when x < pEnd.X:
                    this.X.AddRange( x, pEnd.X ); break;
                case var x when x > pEnd.X:
                    this.X.AddRange( pEnd.X, x ); break;
            }
            switch( pStart.Y ) {
                case var y when y < pEnd.Y:
                    this.Y.AddRange( y, pEnd.Y ); break;
                case var y when y > pEnd.Y:
                    this.Y.AddRange( pEnd.Y, y ); break;
            }
        }
        private void RemoveRegion( Vector2 pStart, Vector2 pEnd ) {
            switch( pStart.X ) {
                case var x when x < pEnd.X:
                    this.X.RemoveRange( x, pEnd.X ); break;
                case var x when x > pEnd.X:
                    this.X.RemoveRange( pEnd.X, x ); break;
            }
            switch( pStart.Y ) {
                case var y when y < pEnd.Y:
                    this.Y.RemoveRange( y, pEnd.Y ); break;
                case var y when y > pEnd.Y:
                    this.Y.RemoveRange( pEnd.Y, y ); break;
            }
        }
        private void ResetRegion() {
            this.X.ResetRange();
            this.Y.ResetRange();
        }
        // |---------|
        // |         |
        // |         |
        // |---------|     <---- anchors are these corners. From that anchor, search for avail in direciton Dir.
        /// <summary>
        /// CornerAnchor: general area to search (NW, NE, SE, SW)
        /// Dir: cardinal direction to search for avail space from the anchor
        /// </summary>
        public Vector2 GetAvailableRelaPos( Direction pCornerAnchor, Direction pDir = Direction.None, Vector2? pPadding = null ) {
            var tAnchor = OccupiedRegion.ToIntercardinal( pCornerAnchor );
            var tDir = OccupiedRegion.ToCardinal( pDir );
            var tPadding = pPadding ?? Vector2.Zero;

            switch( tAnchor ) {
                case Direction.NW:
                    return tDir switch {
                        Direction.N => new Vector2( this.X.MinBound, this.Y.MinBound - tPadding.Y ),
                        Direction.S => new Vector2( this.X.MinBound, this.Y.MinBoundLarger + tPadding.Y ),
                        Direction.W => new Vector2( this.X.MinBound - tPadding.X, this.Y.MinBoundLarger ),
                        Direction.E => new Vector2( this.X.MinBoundLarger + tPadding.X, this.Y.MinBoundLarger ),
                        _ => new Vector2( this.X.MinBoundLarger + tPadding.X, this.Y.MinBoundLarger )
                    };
                case Direction.NE:
                    return tDir switch {
                        Direction.N => new Vector2( this.X.MaxBound, this.Y.MinBound - tPadding.Y ),
                        Direction.S => new Vector2( this.X.MaxBound, this.Y.MinBoundLarger + tPadding.Y ),
                        Direction.W => new Vector2( this.X.MaxBoundSmaller - tPadding.X, this.Y.MinBoundLarger ),
                        Direction.E => new Vector2( this.X.MaxBound + tPadding.X, this.Y.MinBoundLarger ),
                        _ => new Vector2( this.X.MaxBound + tPadding.X, this.Y.MinBoundLarger )
                    };
                case Direction.SE:
                    return tDir switch {
                        Direction.N => new Vector2( this.X.MaxBound, this.Y.MaxBoundSmaller - tPadding.Y ),
                        Direction.S => new Vector2( this.X.MaxBound, this.Y.MaxBound + tPadding.Y ),
                        Direction.W => new Vector2( this.X.MaxBoundSmaller - tPadding.X, this.Y.MaxBoundSmaller ),
                        Direction.E => new Vector2( this.X.MaxBound + tPadding.X, this.Y.MaxBoundSmaller ),
                        _ => new Vector2( this.X.MaxBound + tPadding.X, this.Y.MaxBoundSmaller )
                    };
                case Direction.SW:
                    return tDir switch {
                        Direction.N => new Vector2( this.X.MinBound, this.Y.MaxBoundSmaller - tPadding.Y ),
                        Direction.S => new Vector2( this.X.MinBound, this.Y.MaxBound + tPadding.Y ),
                        Direction.W => new Vector2( this.X.MinBound - tPadding.X, this.Y.MaxBound ),
                        Direction.E => new Vector2( this.X.MinBoundLarger + tPadding.X, this.Y.MaxBound ),
                        _ => new Vector2( this.X.MinBoundLarger + tPadding.X, this.Y.MaxBound )
                    };
            }

            return new Vector2( this.X.MaxBound + tPadding.X, this.Y.MinBound );
        }
        public Vector2 GetAvailableRelaPos( Direction pCornerAnchor, Direction pDir = Direction.None, float pPadding = 0 ) {
            return this.GetAvailableRelaPos( pCornerAnchor, pDir, new Vector2( pPadding ) );
        }
        /// <summary>
        /// <para>Try to get the free-est relaPos within given relaArea.</para>
        /// <para>relaArea is an Area on the canvas that this OccupiedRegion belongs to.</para>
        /// <para>Result are padded so that the occupied region aren't clumped up.</para>
        /// </summary>
        public Vector2 GetAvailableRelaPos( Area pRelaArea, float pPadding = 8 ) {
            float? tBestX = this.X.FindTwoFurthestEndpoints( pRelaArea.start.X, pRelaArea.end.X )?.Item1;
            float? tBestY = this.Y.FindTwoFurthestEndpoints( pRelaArea.start.Y, pRelaArea.end.Y )?.Item1;
            if( !tBestX.HasValue ) tBestX = pRelaArea.start.X;
            if( !tBestY.HasValue ) tBestY = pRelaArea.start.Y;
            return new( tBestX.Value + pPadding, tBestY.Value + pPadding );
        }

        public static Vector2? GetRecommendedOriginRelaPos( Area pArea, Direction pDir = Direction.E, Vector2? pPadding = null ) {
            var tDir = ToCardinal( pDir );
            var tPadding = pPadding ?? Vector2.Zero;
            return tDir switch {
                Direction.N => new Vector2( pArea.start.X, pArea.start.Y - tPadding.Y ),
                Direction.S => new Vector2( pArea.start.X, pArea.end.Y + tPadding.Y ),
                Direction.W => new Vector2( pArea.start.X - tPadding.X, pArea.start.Y ),
                Direction.E => new Vector2( pArea.end.X + tPadding.X, pArea.start.Y ),
                _ => new Vector2( pArea.end.X + tPadding.X, pArea.start.Y )
            };
        }
        public static Vector2? GetRecommendedOriginRelaPos( Area pArea, Direction pDir = Direction.E, float pPadding = 0 ) {
            return OccupiedRegion.GetRecommendedOriginRelaPos( pArea, pDir, new Vector2( pPadding ) );
        }

        public static Direction ToCardinal( Direction pDir, Direction pDefaultForNone = Direction.S ) {
            return pDir switch {
                Direction.NE => Direction.E,
                Direction.SE => Direction.E,
                Direction.NW => Direction.W,
                Direction.SW => Direction.W,
                Direction.None => pDefaultForNone,
                _ => pDir
            };
        }
        public static Direction ToIntercardinal( Direction pDir, Direction pDefaultForNone = Direction.SW ) {
            return pDir switch {
                Direction.N => Direction.NE,
                Direction.E => Direction.NE,
                Direction.W => Direction.NW,
                Direction.S => Direction.SW,
                Direction.None => pDefaultForNone,
                _ => pDir
            };
        }
    }
}

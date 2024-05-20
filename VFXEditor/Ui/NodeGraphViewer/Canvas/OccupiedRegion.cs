using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public class OccupiedRegion {
        private readonly Set X = new();
        private readonly Set Y = new();
        private bool UpdatedOnce = false;

        public OccupiedRegion() { }

        public bool IsUpdatedOnce() => UpdatedOnce;

        public void Update( List<Node> nodes, NodeMap pMap ) {
            ResetRegion();
            foreach( var n in nodes ) {
                var tStart = pMap.GetNodeRelaPos( n );
                if( tStart == null ) continue;
                var tEnd = tStart.Value + n.Style.GetSize();
                AddRegion( tStart.Value, tEnd );
            }
            UpdatedOnce = true;
        }

        private void AddRegion( Vector2 pStart, Vector2 pEnd ) {
            switch( pStart.X ) {
                case var x when x < pEnd.X:
                    X.AddRange( x, pEnd.X ); break;
                case var x when x > pEnd.X:
                    X.AddRange( pEnd.X, x ); break;
            }
            switch( pStart.Y ) {
                case var y when y < pEnd.Y:
                    Y.AddRange( y, pEnd.Y ); break;
                case var y when y > pEnd.Y:
                    Y.AddRange( pEnd.Y, y ); break;
            }
        }

        private void ResetRegion() {
            X.ResetRange();
            Y.ResetRange();
        }

        public Vector2 GetAvailableRelaPos( Direction pCornerAnchor, Direction pDir = Direction.None, Vector2? pPadding = null ) {
            var tAnchor = ToIntercardinal( pCornerAnchor );
            var tDir = ToCardinal( pDir );
            var tPadding = pPadding ?? Vector2.Zero;

            return tAnchor switch {
                Direction.NW => tDir switch {
                    Direction.N => new Vector2( X.MinBound, Y.MinBound - tPadding.Y ),
                    Direction.S => new Vector2( X.MinBound, Y.MinBoundLarger + tPadding.Y ),
                    Direction.W => new Vector2( X.MinBound - tPadding.X, Y.MinBoundLarger ),
                    Direction.E => new Vector2( X.MinBoundLarger + tPadding.X, Y.MinBoundLarger ),
                    _ => new Vector2( X.MinBoundLarger + tPadding.X, Y.MinBoundLarger )
                },
                Direction.NE => tDir switch {
                    Direction.N => new Vector2( X.MaxBound, Y.MinBound - tPadding.Y ),
                    Direction.S => new Vector2( X.MaxBound, Y.MinBoundLarger + tPadding.Y ),
                    Direction.W => new Vector2( X.MaxBoundSmaller - tPadding.X, Y.MinBoundLarger ),
                    Direction.E => new Vector2( X.MaxBound + tPadding.X, Y.MinBoundLarger ),
                    _ => new Vector2( X.MaxBound + tPadding.X, Y.MinBoundLarger )
                },
                Direction.SE => tDir switch {
                    Direction.N => new Vector2( X.MaxBound, Y.MaxBoundSmaller - tPadding.Y ),
                    Direction.S => new Vector2( X.MaxBound, Y.MaxBound + tPadding.Y ),
                    Direction.W => new Vector2( X.MaxBoundSmaller - tPadding.X, Y.MaxBoundSmaller ),
                    Direction.E => new Vector2( X.MaxBound + tPadding.X, Y.MaxBoundSmaller ),
                    _ => new Vector2( X.MaxBound + tPadding.X, Y.MaxBoundSmaller )
                },
                Direction.SW => tDir switch {
                    Direction.N => new Vector2( X.MinBound, Y.MaxBoundSmaller - tPadding.Y ),
                    Direction.S => new Vector2( X.MinBound, Y.MaxBound + tPadding.Y ),
                    Direction.W => new Vector2( X.MinBound - tPadding.X, Y.MaxBound ),
                    Direction.E => new Vector2( X.MinBoundLarger + tPadding.X, Y.MaxBound ),
                    _ => new Vector2( X.MinBoundLarger + tPadding.X, Y.MaxBound )
                },
                _ => new Vector2( X.MaxBound + tPadding.X, Y.MinBound ),
            };
        }

        public Vector2 GetAvailableRelaPos( Direction pCornerAnchor, Direction pDir = Direction.None, float pPadding = 0 ) =>
            GetAvailableRelaPos( pCornerAnchor, pDir, new Vector2( pPadding ) );

        public Vector2 GetAvailableRelaPos( Area pRelaArea, float pPadding = 8 ) {
            var tBestX = X.FindTwoFurthestEndpoints( pRelaArea.Start.X, pRelaArea.End.X )?.Item1;
            var tBestY = Y.FindTwoFurthestEndpoints( pRelaArea.Start.Y, pRelaArea.End.Y )?.Item1;
            if( !tBestX.HasValue ) tBestX = pRelaArea.Start.X;
            if( !tBestY.HasValue ) tBestY = pRelaArea.Start.Y;
            return new( tBestX.Value + pPadding, tBestY.Value + pPadding );
        }

        public static Vector2? GetRecommendedOriginRelaPos( Area pArea, Direction pDir = Direction.E, Vector2? pPadding = null ) {
            var tDir = ToCardinal( pDir );
            var tPadding = pPadding ?? Vector2.Zero;
            return tDir switch {
                Direction.N => new Vector2( pArea.Start.X, pArea.Start.Y - tPadding.Y ),
                Direction.S => new Vector2( pArea.Start.X, pArea.End.Y + tPadding.Y ),
                Direction.W => new Vector2( pArea.Start.X - tPadding.X, pArea.Start.Y ),
                Direction.E => new Vector2( pArea.End.X + tPadding.X, pArea.Start.Y ),
                _ => new Vector2( pArea.End.X + tPadding.X, pArea.Start.Y )
            };
        }

        public static Vector2? GetRecommendedOriginRelaPos( Area pArea, Direction pDir = Direction.E, float pPadding = 0 ) =>
            GetRecommendedOriginRelaPos( pArea, pDir, new Vector2( pPadding ) );

        public static Direction ToCardinal( Direction pDir, Direction pDefaultForNone = Direction.S ) =>
            pDir switch {
                Direction.NE => Direction.E,
                Direction.SE => Direction.E,
                Direction.NW => Direction.W,
                Direction.SW => Direction.W,
                Direction.None => pDefaultForNone,
                _ => pDir
            };

        public static Direction ToIntercardinal( Direction pDir, Direction pDefaultForNone = Direction.SW ) =>
            pDir switch {
                Direction.N => Direction.NE,
                Direction.E => Direction.NE,
                Direction.W => Direction.NW,
                Direction.S => Direction.SW,
                Direction.None => pDefaultForNone,
                _ => pDir
            };
    }
}

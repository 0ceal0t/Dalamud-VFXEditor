using System;
using System.Collections.Generic;

namespace VfxEditor.Ui.NodeGraphViewer {
    public class Set {
        private readonly HashSet<SubSet> SubSets = [];
        public float MinBound { get; private set; } = 0;
        public float MaxBound { get; private set; } = 0;
        public float MinBoundLarger { get; private set; } = 0;
        public float MaxBoundSmaller { get; private set; } = 0;

        public bool Contains( float val ) {
            foreach( var s in SubSets ) {
                if( !s.Contains( val ) ) return false;
            }
            return true;
        }

        public bool AddRange( float negativeSide, float positiveSide ) {
            if( !UnionIfOverlap( negativeSide, positiveSide ) ) {
                SubSet subset = new( negativeSide, positiveSide );
                SubSets.Add( subset );
                UpdateBounds( subset );
                return true;
            }
            return false;
        }

        public bool RemoveRange( float negativeSide, float positiveSide ) => DifferenceIfOverlap( negativeSide, positiveSide );

        public bool IsOverlap( float negativeSide, float positiveSide ) {
            foreach( var subset in SubSets ) {
                if( subset.IsOverlap( negativeSide, positiveSide ) != OverlapType.None ) return true;
            }
            return false;
        }

        public void ResetRange() {
            SubSets.Clear();
            MinBound = 0;
            MaxBound = 0;
            MinBoundLarger = 0;
            MaxBoundSmaller = 0;
        }

        private bool UnionIfOverlap( float negativeSide, float positiveSide ) {
            var subset = new SubSet( negativeSide, positiveSide );
            SubSet chosen = null;
            List<SubSet> garbo = [];
            foreach( var s in SubSets ) {
                if( s.UnionIfOverlap( subset ) ) {
                    if( chosen == null )
                        chosen = s;
                    else
                        garbo.Add( chosen );
                }
            }
            if( chosen == null ) return false;
            else {
                foreach( var g in garbo ) {
                    chosen.UnionIfOverlap( g );
                    SubSets.Remove( g );
                }
                UpdateBounds();
                return true;
            }
        }

        private bool DifferenceIfOverlap( float negativeSide, float positiveSide ) {
            var subset = new SubSet( negativeSide, positiveSide );
            var res = false;
            List<SubSet> garbo = [];
            List<SubSet> recruits = [];
            foreach( var s in SubSets ) {
                if( s.DifferentIfOverlap( subset, out var splitSets, out var isDeleted ) ) {
                    if( isDeleted ) {
                        garbo.Add( s );
                    }
                    else if( splitSets != null ) {
                        garbo.Add( s );
                        recruits.Add( splitSets.Item1 );
                        recruits.Add( splitSets.Item2 );
                    }
                    res = true;
                }
            }
            foreach( var g in garbo ) SubSets.Remove( g );
            foreach( var r in recruits ) SubSets.Add( r );
            if( res ) UpdateBounds();
            return res;
        }

        private void UpdateBounds() {
            foreach( var s in SubSets ) UpdateBounds( s );
        }

        private void UpdateBounds( SubSet subset ) {
            MinBound = subset.NegativeSide < MinBound ? subset.NegativeSide : MinBound;
            MaxBound = subset.PositiveSide > MaxBound ? subset.PositiveSide : MaxBound;
            MinBoundLarger = subset.NegativeSide < MinBound ? subset.PositiveSide : MinBoundLarger;
            MaxBoundSmaller = subset.PositiveSide > MaxBound ? subset.NegativeSide : MaxBoundSmaller;
        }

        public Tuple<float, float> FindTwoFurthestEndpoints( float lowerBound, float upperBound, float minThresholdToTriggerRandom = 10 ) {
            List<float> endpoints = [];
            foreach( var s in SubSets ) {
                // check if the search range is within a subset
                if( s.NegativeSide < lowerBound && s.PositiveSide > upperBound ) return null;
                // update upperBound/lowerBound if it's within a subset
                // OR the subset is out of search range
                if( s.NegativeSide < lowerBound ) {
                    if( s.PositiveSide > lowerBound ) lowerBound = s.PositiveSide;
                    continue;
                }
                else if( s.PositiveSide > upperBound ) {
                    if( s.NegativeSide < upperBound ) upperBound = s.NegativeSide;
                    continue;
                }
                endpoints.Add( s.NegativeSide );
                endpoints.Add( s.PositiveSide );
            }
            endpoints.Add( upperBound );
            endpoints.Add( lowerBound );
            endpoints.Sort();
            float? e1 = null;
            float? e2 = null;
            for( var i = 0; i + 1 < endpoints.Count; i += 2 ) {
                e1 ??= endpoints[i];
                e2 ??= endpoints[i + 1];

                if( endpoints[i + 1] - endpoints[i] > ( e2.Value - e1.Value ) ) {
                    e1 = endpoints[i];
                    e2 = endpoints[i + 1];
                }
            }
            if( !( e1.HasValue && e2.HasValue ) ) return null;
            if( e2.Value - e1.Value < minThresholdToTriggerRandom ) {
                var randomEnpointIdx = new Random().Next( 0, endpoints.Count / 2 );
                e1 = endpoints[randomEnpointIdx];
                e2 = endpoints[randomEnpointIdx + 1];
            }
            return new( e1.Value, e2.Value );
        }


        private class SubSet {
            public float PositiveSide;
            public float NegativeSide;

            private SubSet() { }
            public SubSet( float nergativeSide, float positiveSide ) {
                NegativeSide = nergativeSide;
                PositiveSide = positiveSide;
            }

            public bool Contains( float val ) => val > PositiveSide && val < NegativeSide;

            public OverlapType IsOverlap( SubSet subset ) {
                if( subset.NegativeSide < NegativeSide ) {
                    switch( subset.PositiveSide ) {
                        case var a when a < NegativeSide: return OverlapType.None;
                        case var a when a > PositiveSide: return OverlapType.Outside;
                        case var a when a > NegativeSide: return OverlapType.Negative;
                    }
                }
                else if( subset.NegativeSide > PositiveSide ) return OverlapType.None;
                else {
                    switch( subset.PositiveSide ) {
                        case var a when a < PositiveSide: return OverlapType.Inside;
                        case var a when a > PositiveSide: return OverlapType.Positive;
                    }
                }
                return OverlapType.None;
            }

            public OverlapType IsOverlap( float negativeSide, float positiveSide ) => IsOverlap( new SubSet( negativeSide, positiveSide ) );

            public bool UnionIfOverlap( SubSet subset ) {
                switch( IsOverlap( subset ) ) {
                    case var o when o == OverlapType.None: return false;
                    case var o when o == OverlapType.Negative:
                        NegativeSide = subset.NegativeSide; return true;
                    case var o when o == OverlapType.Positive:
                        PositiveSide = subset.PositiveSide; return true;
                    case var o when o == OverlapType.Outside:
                        NegativeSide = subset.NegativeSide;
                        PositiveSide = subset.PositiveSide;
                        return true;
                    case var o when o == OverlapType.Inside: return true;
                }
                return false;
            }

            public bool DifferentIfOverlap( SubSet subset, out Tuple<SubSet, SubSet> splitSets, out bool isDeleted ) {
                splitSets = null;
                isDeleted = false;
                switch( IsOverlap( subset ) ) {
                    case var o when o == OverlapType.None: return false;
                    case var o when o == OverlapType.Negative:
                        NegativeSide = subset.PositiveSide; return true;
                    case var o when o == OverlapType.Positive:
                        PositiveSide = subset.NegativeSide; return true;
                    case var o when o == OverlapType.Outside:
                        isDeleted = true;
                        return true;
                    case var o when o == OverlapType.Inside:
                        splitSets = new( new( NegativeSide, subset.NegativeSide ), new( subset.PositiveSide, PositiveSide ) );
                        return true;
                }
                return false;
            }
        }

        public enum OverlapType {
            None = 0,           // No overlap
            Negative = 1,       // A overlap B on the left (negative side)
            Positive = 2,       // A overlap B on the right (positive side)
            Inside = 3,         // A fully inside B 
            Outside = 4         // A fully wrap around B (B is fully inside A)
        }
    }
}

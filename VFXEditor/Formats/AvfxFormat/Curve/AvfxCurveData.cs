using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor;
using VfxEditor.AvfxFormat;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VFXEditor.Formats.AvfxFormat.Curve {
    public enum CurveType {
        Color,
        Angle,
        Base
    }

    public class AvfxCurveData : AvfxCurveBase {
        public readonly CurveType Type;
        public readonly AvfxEnum<CurveBehavior> PreBehavior = new( "Pre Behavior", "BvPr" );
        public readonly AvfxEnum<CurveBehavior> PostBehavior = new( "Post Behavior", "BvPo" );
        public readonly AvfxEnum<RandomType> Random = new( "Random Type", "RanT" );
        public readonly AvfxCurveKeys KeyList;

        public List<AvfxCurveKey> Keys => KeyList.Keys;
        public bool IsColor => Type == CurveType.Color;

        private readonly List<AvfxBase> Parsed;

        public AvfxCurveData( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( name, avfxName, locked ) {
            Type = type;

            KeyList = new( this );

            Parsed = [
                PreBehavior,
                PostBehavior,
                Random,
                KeyList
            ];
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        public override void WriteContents( BinaryWriter writer ) {
            WriteLeaf( writer, "KeyC", 4, KeyList.Keys.Count );
            if( Type == CurveType.Color ) Random.SetAssigned( false );
            WriteNested( writer, Parsed );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() { } // should not be drawing this

        public void Sort( List<ICommand> commands ) {
            var sorted = new List<AvfxCurveKey>( Keys );
            sorted.Sort( ( x, y ) => x.Time.Value.CompareTo( y.Time.Value ) );
            commands.Add( new ListSetCommand<AvfxCurveKey>( Keys, sorted, Cleanup ) );
        }

        public double ToRadians( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( Math.PI / 180 ) * value;
        }

        public double ToDegrees( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( 180 / Math.PI ) * value;
        }

        public virtual void Cleanup() {
            KeyList.SetAssigned( true );
            if( !IsColor || Keys.Count < 2 ) return;
            if( !Keys.FindFirst( x => x.Time.Value != Keys[0].Time.Value, out var _ ) ) Keys[^1].Time.Value++;
        }

        // ======== EVALUATION ===========

        private List<AvfxCurveEvaluator.Key> GetEvaluatorKeys( bool display ) => [.. Keys.Select( key => new AvfxCurveEvaluator.Key(
            key.Time.Value,
            key.Type.Value,
            display ? key.DisplayY : key.Data.Value.Z,
            key.Data.Value.X,
            key.Data.Value.Y
        ) )];

        public double Evaluate( double frame ) => AvfxCurveEvaluator.Evaluate( GetEvaluatorKeys( false ), PreBehavior.Value, PostBehavior.Value, frame );

        private const int MaxBehaviorLoops = 200;

        public void GetDrawLine( double minX, double maxX, out List<double> xs, out List<double> ys, out List<double> preXs, out List<double> preYs, out List<double> postXs, out List<double> postYs ) {
            xs = [];
            ys = [];
            preXs = [];
            preYs = [];
            postXs = [];
            postYs = [];
            if( Keys.Count == 0 ) return;

            var keys = GetEvaluatorKeys( true );

            xs.Add( keys[0].Time );
            ys.Add( keys[0].Value );

            for( var idx = 0; idx < keys.Count - 1; idx++ ) {
                var k0 = keys[idx];
                var k1 = keys[idx + 1];

                if( IsColor || k1.Type == KeyType.Linear || k1.Time <= k0.Time ) {
                    xs.Add( k1.Time );
                    ys.Add( k1.Value );
                }
                else if( k1.Type == KeyType.Step ) {
                    xs.Add( k1.Time );
                    ys.Add( k0.Value );
                    xs.Add( k1.Time );
                    ys.Add( k1.Value );
                }
                else {
                    var samples = Math.Clamp( ( int )( ( k1.Time - k0.Time ) * 2 ), 8, 64 );
                    for( var i = 1; i <= samples; i++ ) {
                        var frame = k0.Time + ( ( k1.Time - k0.Time ) * i / samples );
                        xs.Add( frame );
                        ys.Add( AvfxCurveEvaluator.EvaluateSegment( keys, idx, frame ) );
                    }
                }
            }

            if( IsColor || keys.Count < 2 ) return;

            var first = keys[0];
            var last = keys[^1];
            var length = ( int )( last.Time - first.Time );
            var offset = last.Value - first.Value;

            // Before the first key
            if( minX < first.Time ) {
                var pre = PreBehavior.Value;
                if( length <= 0 || ( pre != CurveBehavior.Repeat && pre != CurveBehavior.Add ) ) {
                    var value = pre == CurveBehavior.None && length > 0 ? last.Value : first.Value;
                    preXs.AddRange( [minX, first.Time] );
                    preYs.AddRange( [value, value] );
                }
                else {
                    var loops = Math.Min( ( int )Math.Ceiling( ( first.Time - minX ) / length ), MaxBehaviorLoops );
                    for( var loop = loops; loop >= 1; loop-- ) {
                        var yOffset = pre == CurveBehavior.Add ? -offset * loop : 0;
                        for( var i = 0; i < xs.Count; i++ ) {
                            preXs.Add( xs[i] - ( loop * length ) );
                            preYs.Add( ys[i] + yOffset );
                        }
                    }
                    // Connect to the start of the main line
                    preXs.Add( xs[0] );
                    preYs.Add( ys[0] );
                }
            }

            // After the last key
            if( maxX > last.Time ) {
                var post = PostBehavior.Value;
                if( length <= 0 || ( post != CurveBehavior.Repeat && post != CurveBehavior.Add ) ) {
                    postXs.AddRange( [last.Time, maxX] );
                    postYs.AddRange( [last.Value, last.Value] );
                }
                else {
                    // Connect to the end of the main line
                    postXs.Add( xs[^1] );
                    postYs.Add( ys[^1] );
                    var loops = Math.Min( ( int )Math.Ceiling( ( maxX - last.Time ) / length ), MaxBehaviorLoops );
                    for( var loop = 1; loop <= loops; loop++ ) {
                        var yOffset = post == CurveBehavior.Add ? offset * loop : 0;
                        for( var i = 0; i < xs.Count; i++ ) {
                            postXs.Add( xs[i] + ( loop * length ) );
                            postYs.Add( ys[i] + yOffset );
                        }
                    }
                }
            }
        }
    }
}
using System;
using System.IO;
using System.Runtime.InteropServices;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    // A value made of consecutive components of type C (e.g. a Vector3 is 3 floats), read and written one component at a time
    public abstract class ParsedVectorBase<T, C> : ParsedSimpleBase<T> where T : struct where C : unmanaged {
        public bool HighPrecision = true;

        protected string FloatFormat => HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f";

        public ParsedVectorBase( string name, T value ) : base( name, value ) { }

        public ParsedVectorBase( string name ) : base( name ) { }

        private Span<C> Components => MemoryMarshal.Cast<T, C>( MemoryMarshal.CreateSpan( ref Value, 1 ) );

        protected abstract C ReadComponent( BinaryReader reader );

        protected abstract void WriteComponent( BinaryWriter writer, C value );

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            var components = Components;
            for( var i = 0; i < components.Length; i++ ) components[i] = ReadComponent( reader );
        }

        public override void Write( BinaryWriter writer ) {
            foreach( var component in Components ) WriteComponent( writer, component );
        }
    }

    // Vector2/3/4 stored as floats, or as halfs when Half is set
    public abstract class ParsedFloatVector<T> : ParsedVectorBase<T, float> where T : struct {
        protected virtual bool Half => false;

        public ParsedFloatVector( string name, T value ) : base( name, value ) { }

        public ParsedFloatVector( string name ) : base( name ) { }

        protected override float ReadComponent( BinaryReader reader ) => Half ? ( float )reader.ReadHalf() : reader.ReadSingle();

        protected override void WriteComponent( BinaryWriter writer, float value ) {
            if( Half ) writer.Write( ( Half )value );
            else writer.Write( value );
        }
    }

    public abstract class ParsedDoubleVector<T> : ParsedVectorBase<T, double> where T : struct {
        public ParsedDoubleVector( string name, T value ) : base( name, value ) { }

        public ParsedDoubleVector( string name ) : base( name ) { }

        protected override double ReadComponent( BinaryReader reader ) => reader.ReadDouble();

        protected override void WriteComponent( BinaryWriter writer, double value ) => writer.Write( value );
    }
}
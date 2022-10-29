using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Curve {
    public class AVFXCurveKeys : AVFXBase {
        public readonly List<AVFXCurveKey> Keys = new();

        public AVFXCurveKeys() : base( "Keys" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 16;
            for( var i = 0; i < count; i++ ) {
                Keys.Add( new AVFXCurveKey( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var key in Keys ) key.Write( writer );
        }

        public AVFXCurveKey Add() {
            SetAssigned( true );
            var key = new AVFXCurveKey( KeyType.Linear, 0, 1, 1, 1 );
            Keys.Add( key );
            return key;
        }

        public void Add( AVFXCurveKey key ) {
            SetAssigned( true );
            Keys.Add( key );
        }

        public void Remove( int idx ) {
            SetAssigned( true );
            Keys.RemoveAt( idx );
        }

        public void Remove( AVFXCurveKey key ) {
            SetAssigned( true );
            Keys.Remove( key );
        }
    }

    public class AVFXCurveKey {
        public KeyType Type;
        public int Time;

        public float X;
        public float Y;
        public float Z;

        public AVFXCurveKey( KeyType type, int time, float x, float y, float z ) {
            Type = type;
            Time = time;
            X = x;
            Y = y;
            Z = z;
        }

        public AVFXCurveKey( BinaryReader reader ) {
            Time = reader.ReadInt16();
            Type = ( KeyType )reader.ReadInt16();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( short )Time );
            writer.Write( ( short )Type );
            writer.Write( X );
            writer.Write( Y );
            writer.Write( Z );
        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace VFXEditor.AVFXLib.Model {
    public class AVFXVNums : AVFXBase {
        public readonly List<AVFXVNum> Nums = new();

        public AVFXVNums() : base( "VNum" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 2;
            for( var i = 0; i < count; i++ ) {
                Nums.Add( new AVFXVNum( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var num in Nums ) num.Write( writer );
        }

        public AVFXVNum Add() {
            SetAssigned( true );
            var num = new AVFXVNum( 0 );
            Nums.Add( num );
            return num;
        }

        public void Add( AVFXVNum num ) {
            SetAssigned( true );
            Nums.Add( num );
        }

        public void Remove( int idx ) {
            SetAssigned( true );
            Nums.RemoveAt( idx );
        }

        public void Remove( AVFXVNum num ) {
            SetAssigned( true );
            Nums.Remove( num );
        }
    }

    public class AVFXVNum {
        public int Num;

        public AVFXVNum( int num ) {
            Num = num;
        }

        public AVFXVNum( BinaryReader reader ) {
            Num = reader.ReadInt16();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( short )Num );
        }
    }
}

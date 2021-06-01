using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class LiteralIntList : LiteralBase
    {
        public List<int> Value { get; set; }

        public LiteralIntList(string avfxName, int size = 1 ) : base(avfxName, size )
        {
            Value = new List<int>();
            Value.Add( 0 );
        }

        public override void Read( AVFXNode node )
        {
        }

        public override void read( AVFXLeaf leaf )
        {
            Size = leaf.Size;
            Value.Clear();
            for(int i = 0; i < Size; i++ )
            {
                Value.Add( ( int )leaf.Contents[i] );
            }
            Assigned = true;
        }

        public void GiveValue(int value )
        {
            GiveValue(new int[]{ value});
        }
        public void GiveValue(int[] value )
        {
            GiveValue( new List<int>( value ) );
        }
        public void GiveValue(int value, int idx ) {
            Value[idx] = value;
        }
        public void GiveValue( List<int> value )
        {
            Value.Clear();
            Value.AddRange( value );
            Size = Value.Count;
            Assigned = true;
        }

        public void AddItem(int item )
        {
            Size++;
            Value.Add( item );
        }
        public void RemoveItem(int idx )
        {
            Size--;
            Value.RemoveAt( idx );
        }

        public override void ToDefault()
        {
            GiveValue( 0 );
        }

        public override AVFXNode ToAVFX()
        {
            byte[] bytes = new byte[Value.Count];
            for(int i = 0; i < Value.Count; i++ )
            {
                bytes[i] = Convert.ToByte( Value[i] );
            }
            return new AVFXLeaf( AVFXName, Size, bytes );
        }

        public override string stringValue()
        {
            return Value.ToString();
        }
    }
}

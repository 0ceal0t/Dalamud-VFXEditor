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

        public LiteralIntList( string jsonPath, string avfxName, int size = 1 ) : base( jsonPath, avfxName, size )
        {
            Value = new List<int>();
        }

        public override void read( AVFXNode node )
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

        public override void toDefault()
        {
            var v = new List<int>();
            v.Add( 0 );
            GiveValue( v ) ;
        }

        public override JToken toJSON()
        {
            JArray val = new JArray();
            foreach(int i in Value )
            {
                val.Add( i );
            }
            return val;
        }

        public override AVFXNode toAVFX()
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

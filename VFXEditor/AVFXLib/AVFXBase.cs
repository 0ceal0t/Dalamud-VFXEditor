using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VfxEditor.AVFXLib {
    public abstract class AVFXBase {
        private readonly string Name;
        private bool Assigned = true;

        public AVFXBase( string name ) {
            Name = name;
        }

        public bool IsAssigned() => Assigned;

        public void SetAssigned( bool assigned ) {
            Assigned = assigned;
        }

        public string GetName() => Name;

        public void Read( BinaryReader reader, int size ) {
            Assigned = true;
            RecurseChildrenAssigned( false );
            ReadContents( reader, size );
        }

        protected abstract void RecurseChildrenAssigned( bool assigned );

        public abstract void ReadContents( BinaryReader reader, int size ); // size is the contents size (does not include the name and size of this element)

        public void Write( BinaryWriter writer ) {
            if( !Assigned ) return;

            WriteName( writer, Name );

            var sizePos = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            WriteContents( writer );

            var endPos = writer.BaseStream.Position;
            var size = endPos - sizePos - 4;

            WritePad( writer, CalculatePadding( ( int )size ) );

            endPos = writer.BaseStream.Position;

            writer.BaseStream.Seek( sizePos, SeekOrigin.Begin );
            writer.Write( ( int )size );

            writer.BaseStream.Seek( endPos, SeekOrigin.Begin );
        }

        protected abstract void WriteContents( BinaryWriter writer );

        // =====================

        public static int CalculatePadding( int size ) => ( size % 4 == 0 ) ? 0 : ( 4 - ( size % 4 ) );

        public static void Peek( BinaryReader reader, AVFXBase item, int size ) => Peek( reader, new List<AVFXBase> { item }, size );

        public static void Peek( BinaryReader reader, List<AVFXBase> items, int size ) {
            var startPosition = reader.BaseStream.Position;
            ReadNested( reader, items, size );
            reader.BaseStream.Seek( startPosition, SeekOrigin.Begin ); // reset position
        }

        public static void WriteNested( BinaryWriter writer, List<AVFXBase> children ) {
            foreach( var child in children ) {
                child.Write( writer );
            }
        }

        public static void ReadNested( BinaryReader reader, List<AVFXBase> children, int size ) => ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
            foreach( var child in children ) {
                if( child.GetName() == _name ) {
                    child.Read( _reader, _size );
                    break;
                }
            }
        }, size );

        public static void RecurseAssigned<T>( T child, bool assigned ) where T : AVFXBase {
            if( child != null ) {
                child.SetAssigned( assigned );
                child.RecurseChildrenAssigned( assigned );
            }
        }

        public static void RecurseAssigned<T>( List<T> children, bool assigned ) where T : AVFXBase {
            foreach( var child in children ) {
                if( child == null ) continue;
                child.SetAssigned( assigned );
                child.RecurseChildrenAssigned( assigned );
            }
        }

        public static void ReadNested( BinaryReader reader, Action<BinaryReader, string, int> onRead, int size ) {
            var bytesRead = 0;
            while( bytesRead < size ) {
                var name = ReadName( reader );
                bytesRead += 4;

                var contentSize = reader.ReadInt32();
                bytesRead += 4;

                var finalPosition = reader.BaseStream.Position + contentSize;

                onRead( reader, name, contentSize );

                reader.BaseStream.Seek( finalPosition, SeekOrigin.Begin );
                bytesRead += contentSize;

                var padding = CalculatePadding( contentSize );
                reader.ReadBytes( padding );
                bytesRead += padding;
            }
        }

        public static void WriteLeaf( BinaryWriter writer, string name, int size, int value ) {
            WriteName( writer, name );
            writer.Write( size );
            writer.Write( value );
        }

        public static void WriteName( BinaryWriter writer, string name ) {
            var nameBytes = Encoding.ASCII.GetBytes( name ).Reverse().ToArray();
            writer.Write( nameBytes );
            WritePad( writer, 4 - name.Length );
        }

        public static string ReadName( BinaryReader reader ) {
            return ReadName( BitConverter.GetBytes( reader.ReadInt32() ) );
        }

        public static string ReadName( byte[] bytes ) {
            var nonZeroBytes = bytes.Reverse().Where( x => x != 0 ).ToArray();
            return Encoding.ASCII.GetString( nonZeroBytes );
        }

        public static void WritePad( BinaryWriter writer, int count ) {
            for( var i = 0; i < count; i++ ) {
                writer.Write( ( byte )0 );
            }
        }

        public static byte[] FloatTo2Bytes( float floatVal ) {
            var ushortVal = Pack( floatVal );
            return BitConverter.GetBytes( ushortVal );
        }

        public static float Bytes2ToFloat( byte[] bytes ) {
            return Unpack( bytes, 0 );
        }

        public static unsafe ushort Pack( float value ) {
            var num5 = *( ( uint* )&value );
            var num3 = ( uint )( ( num5 & -2147483648 ) >> 0x10 );
            var num = num5 & 0x7fffffff;
            if( num > 0x47ffefff ) {
                return ( ushort )( num3 | 0x7fff );
            }
            if( num >= 0x38800000 ) return ( ushort )( num3 | ( ( ( ( num + -939524096 ) + 0xfff ) + ( ( num >> 13 ) & 1 ) ) >> 13 ) );

            var num6 = ( num & 0x7fffff ) | 0x800000;
            var num4 = 0x71 - ( ( int )( num >> 0x17 ) );
            num = ( num4 > 0x1f ) ? 0 : ( num6 >> num4 );
            return ( ushort )( num3 | ( ( ( num + 0xfff ) + ( ( num >> 13 ) & 1 ) ) >> 13 ) );
        }

        public static float Unpack( byte[] buffer, int offset ) {
            return Unpack( BitConverter.ToUInt16( buffer, offset ) );
        }

        public static unsafe float Unpack( ushort value ) {
            uint num3;
            if( ( value & -33792 ) == 0 ) {
                if( ( value & 0x3ff ) != 0 ) {
                    var num2 = 0xfffffff2;
                    var num = ( uint )( value & 0x3ff );
                    while( ( num & 0x400 ) == 0 ) {
                        num2--;
                        num <<= 1;
                    }
                    num &= 0xfffffbff;
                    num3 = ( ( uint )( ( ( value & 0x8000 ) << 0x10 ) | ( ( num2 + 0x7f ) << 0x17 ) ) ) | ( num << 13 );
                }
                else {
                    num3 = ( uint )( ( value & 0x8000 ) << 0x10 );
                }
            }
            else {
                num3 =
                    ( uint )
                    ( ( ( ( value & 0x8000 ) << 0x10 ) | ( ( ( ( ( value >> 10 ) & 0x1f ) - 15 ) + 0x7f ) << 0x17 ) )
                     | ( ( value & 0x3ff ) << 13 ) );
            }
            return *( ( ( float* )&num3 ) );
        }
    }
}

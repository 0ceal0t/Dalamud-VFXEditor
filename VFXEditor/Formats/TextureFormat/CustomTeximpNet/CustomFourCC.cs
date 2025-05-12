using System;
using System.Runtime.InteropServices;
using TeximpNet;

namespace VfxEditor.Formats.TextureFormat.CustomTeximpNet {
    [StructLayout( LayoutKind.Sequential, Size = 4 )]
    internal readonly struct CustomFourCC : IEquatable<CustomFourCC> {
        private static readonly CustomFourCC s_empty = new( 0 );
        private static readonly int s_sizeInBytes = MemoryHelper.SizeOf<CustomFourCC>();

        private readonly uint m_packedValue;

        public static CustomFourCC Empty {
            get {
                return s_empty;
            }
        }

        public static int SizeInBytes {
            get {
                return s_sizeInBytes;
            }
        }

        public char First {
            get {
                return ( char )( m_packedValue & 255 );
            }
        }

        public char Second {
            get {
                return ( char )( m_packedValue >> 8 & 255 );
            }
        }

        public char Third {
            get {
                return ( char )( m_packedValue >> 16 & 255 );
            }
        }

        public char Fourth {
            get {
                return ( char )( m_packedValue >> 24 & 255 );
            }
        }

        public CustomFourCC( string fourCharacterCode ) {
            if( fourCharacterCode != null ) {
                if( fourCharacterCode.Length != 4 )
                    throw new ArgumentOutOfRangeException( nameof( fourCharacterCode ), "CustomFourCC must have four characters only." );

                m_packedValue = ( uint )( fourCharacterCode[3] << 24 | fourCharacterCode[2] << 16 | fourCharacterCode[1] << 8 | fourCharacterCode[0] );
            }
            else {
                m_packedValue = 0;
            }
        }

        public CustomFourCC( char first, char second, char third, char fourth ) {
            m_packedValue = ( uint )( fourth << 24 | third << 16 | second << 8 | first );
        }

        public CustomFourCC( uint packedValue ) {
            m_packedValue = packedValue;
        }

        public CustomFourCC( int packedValue ) {
            m_packedValue = ( uint )packedValue;
        }

        public static implicit operator uint( CustomFourCC fourCharacterCode ) {
            return fourCharacterCode.m_packedValue;
        }

        public static implicit operator int( CustomFourCC fourCharacterCode ) {
            return ( int )fourCharacterCode.m_packedValue;
        }

        public static implicit operator string( CustomFourCC fourCharacterCode ) {
            return new string( [fourCharacterCode.First, fourCharacterCode.Second, fourCharacterCode.Third, fourCharacterCode.Fourth] );
        }

        public static implicit operator CustomFourCC( uint packedValue ) {
            return new CustomFourCC( packedValue );
        }

        public static implicit operator CustomFourCC( int packedValue ) {
            return new CustomFourCC( packedValue );
        }

        public static implicit operator CustomFourCC( string fourCharacterCode ) {
            return new CustomFourCC( fourCharacterCode );
        }

        public static bool operator ==( CustomFourCC a, CustomFourCC b ) {
            return a.m_packedValue == b.m_packedValue;
        }

        public static bool operator !=( CustomFourCC a, CustomFourCC b ) {
            return a.m_packedValue != b.m_packedValue;
        }

        public override bool Equals( object? obj ) {
            if( obj is CustomFourCC cC ) return Equals( cC );
            return false;
        }

        public bool Equals( CustomFourCC other ) {
            return m_packedValue == other.m_packedValue;
        }

        public override int GetHashCode() {
            unchecked {
                return ( int )m_packedValue;
            }
        }

        public override string ToString() {
            if( m_packedValue == 0 ) return "0";
            return new string( [First, Second, Third, Fourth] );
        }
    }
}

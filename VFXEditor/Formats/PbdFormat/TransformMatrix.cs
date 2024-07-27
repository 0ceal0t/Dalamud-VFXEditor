using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VfxEditor.Formats.PbdFormat {
    [StructLayout( LayoutKind.Sequential )]
    public readonly struct TransformMatrix {
        public readonly Vector4 XRow;
        public readonly Vector4 YRow;
        public readonly Vector4 ZRow;

        public readonly Vector3 XColumn => new( XRow.X, YRow.X, ZRow.X );
        public readonly Vector3 YColumn => new( XRow.Y, YRow.Y, ZRow.Y );
        public readonly Vector3 ZColumn => new( XRow.Z, YRow.Z, ZRow.Z );

        public readonly Vector3 Translation => new( XRow.W, YRow.W, ZRow.W );

        public TransformMatrix( Vector4 xRow, Vector4 yRow, Vector4 zRow ) {
            XRow = xRow;
            YRow = yRow;
            ZRow = zRow;
        }

        public TransformMatrix( Vector3 xColumn, Vector3 yColumn, Vector3 zColumn, Vector3 translation ) {
            XRow = new Vector4( xColumn.X, yColumn.X, zColumn.X, translation.X );
            YRow = new Vector4( xColumn.Y, yColumn.Y, zColumn.Y, translation.Y );
            ZRow = new Vector4( xColumn.Z, yColumn.Z, zColumn.Z, translation.Z );
        }

        public static TransformMatrix CreateTranslation( Vector3 translation )
            => new(
                new Vector4( 1.0f, 0.0f, 0.0f, translation.X ),
                new Vector4( 0.0f, 1.0f, 0.0f, translation.Y ),
                new Vector4( 0.0f, 0.0f, 1.0f, translation.Z ) );

        public static TransformMatrix CreateScale( Vector3 scale )
            => new(
                new Vector4( scale.X, 0.0f, 0.0f, 0.0f ),
                new Vector4( 0.0f, scale.Y, 0.0f, 0.0f ),
                new Vector4( 0.0f, 0.0f, scale.Z, 0.0f ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static TransformMatrix CreateRotation( Quaternion rotation )
            => ( TransformMatrix )Matrix4x4.CreateFromQuaternion( rotation );

        public static TransformMatrix Compose( Vector3 scale, Quaternion rotation, Vector3 translation )
            => CreateTranslation( translation ) * CreateRotation( rotation ) * CreateScale( scale );

        public bool TryDecompose( out Vector3 scale, out Quaternion rotation, out Vector3 translation )
            => Matrix4x4.Decompose( this, out scale, out rotation, out translation );

        public override string ToString()
            => $"TransformMatrix {{ X = {XRow}, Y = {YRow}, Z = {ZRow} }}";


        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public static Vector3 operator *( in TransformMatrix mat, Vector3 vec )
            => mat * new Vector4( vec, 1.0f );

        [MethodImpl( MethodImplOptions.AggressiveOptimization )]
        public static Vector3 operator *( in TransformMatrix mat, Vector4 vec )
            => new( Vector4.Dot( mat.XRow, vec ), Vector4.Dot( mat.YRow, vec ), Vector4.Dot( mat.ZRow, vec ) );

        [MethodImpl( MethodImplOptions.AggressiveOptimization )]
        public static TransformMatrix operator *( in TransformMatrix lhs, in TransformMatrix rhs )
            => new(
                lhs * new Vector4( rhs.XColumn, 0.0f ),
                lhs * new Vector4( rhs.YColumn, 0.0f ),
                lhs * new Vector4( rhs.ZColumn, 0.0f ),
                lhs * new Vector4( rhs.Translation, 1.0f ) );

        [MethodImpl( MethodImplOptions.AggressiveOptimization )]
        public static implicit operator Matrix4x4( in TransformMatrix mat )
            => new(
                mat.XRow.X, mat.YRow.X, mat.ZRow.X, 0.0f,
                mat.XRow.Y, mat.YRow.Y, mat.ZRow.Y, 0.0f,
                mat.XRow.Z, mat.YRow.Z, mat.ZRow.Z, 0.0f,
                mat.XRow.W, mat.YRow.W, mat.ZRow.W, 1.0f );


        public static explicit operator TransformMatrix( in Matrix4x4 mat ) {
            if( mat.M14 != 0.0f || mat.M24 != 0.0f || mat.M34 != 0.0f || mat.M44 != 1.0f )
                throw new InvalidCastException(
                    $"Invalid transform matrix, expected the fourth column to be <0 0 0 1>, found <{mat.M14} {mat.M24} {mat.M34} {mat.M44}>" );

            return new TransformMatrix(
                new Vector4( mat.M11, mat.M21, mat.M31, mat.M41 ),
                new Vector4( mat.M12, mat.M22, mat.M32, mat.M42 ),
                new Vector4( mat.M13, mat.M23, mat.M33, mat.M43 ) );
        }
    }
}

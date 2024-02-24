using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Interop.Havok.SkeletonBuilder {
    public abstract class SkeletonMeshBuilder {
        protected readonly IList<Bone> Bones;

        protected readonly Vector3Collection Positions;
        protected readonly IntCollection Tris;
        protected readonly Color4Collection Colors;

        protected readonly List<float> BoneScales = new();

        protected readonly MeshGeometry3D SingleBone;
        protected readonly MeshBuilder SphereBuilder;

        protected int Offset = 0;

        public SkeletonMeshBuilder( IList<Bone> bones ) {
            Bones = bones;

            var singleBoneBuilder = new MeshBuilder( true, false );
            singleBoneBuilder.AddPyramid( new Vector3( 0, 0, 0 ), Vector3.UnitZ, Vector3.UnitX, 1, 0, true );
            SingleBone = singleBoneBuilder.ToMesh();

            Positions = new Vector3Collection( Bones.Count * SingleBone.Positions.Count );
            Tris = new IntCollection( Bones.Count * SingleBone.Indices.Count );
            Colors = new Color4Collection( Positions.Capacity );

            SphereBuilder = new MeshBuilder( true, false );
        }

        public BoneSkinnedMeshGeometry3D Build() {
            // ==== SCALES =======

            for( var i = 0; i < Bones.Count; i++ ) { BoneScales.Add( -1 ); }
            for( var i = 0; i < Bones.Count; i++ ) { PopulateBoneScales( i ); }

            // ===== BONES ==========

            for( var i = 0; i < Bones.Count; i++ ) {
                var count = Positions.Count;
                PopulateBone( i );
                PushColor( GetColor( i ), Positions.Count - count );
            }

            // ===== SPHERES ========

            for( var i = 0; i < Bones.Count; i++ ) { PopulateSpheres( i ); }

            Positions.AddRange( SphereBuilder.Positions );
            Tris.AddRange( SphereBuilder.TriangleIndices.Select( x => x + Offset ) );

            // =================

            var mesh = new BoneSkinnedMeshGeometry3D() {
                Positions = Positions,
                Indices = Tris,
                Colors = Colors
            };
            mesh.Normals = mesh.CalculateNormals();
            return mesh;
        }

        protected abstract void PopulateBoneScales( int idx );

        protected abstract void PopulateSpheres( int idx );

        protected abstract void PopulateBone( int idx );

        protected abstract Color4 GetColor( int idx );

        protected void AddPyramid( float scale, Matrix startMatrix, Matrix endMatrix ) {
            Tris.AddRange( SingleBone.Indices.Select( x => x + Offset ) );

            var j = 0;
            for( ; j < SingleBone.Positions.Count - 6; j += 3 ) { // iterate over everything but last 2 faces
                Positions.Add( Vector3.TransformCoordinate( SingleBone.Positions[j] * scale, startMatrix ) );
                Positions.Add( Vector3.TransformCoordinate( SingleBone.Positions[j + 1] * scale, startMatrix ) );
                Positions.Add( endMatrix.TranslationVector );
            }
            for( ; j < SingleBone.Positions.Count; ++j ) {
                Positions.Add( Vector3.TransformCoordinate( SingleBone.Positions[j] * scale, startMatrix ) );
            }
            Offset += SingleBone.Positions.Count;
        }

        protected void AddSphere( int idx, Matrix matrix, float width ) {
            var color = GetColor( idx );
            var count = SphereBuilder.Positions.Count;
            SphereBuilder.AddSphere( Vector3.Zero, width / 2, 12, 12 );

            for( var j = count; j < SphereBuilder.Positions.Count; ++j ) {
                SphereBuilder.Positions[j] = Vector3.TransformCoordinate( SphereBuilder.Positions[j], matrix );
            }

            PushColor( color, SphereBuilder.Positions.Count - count );
        }

        protected void PushColor( Color4 color, int n ) {
            for( var i = 0; i < n; i++ ) Colors.Add( color );
        }
    }
}

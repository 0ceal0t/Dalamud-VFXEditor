using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Interop.Havok {
    public static class HavokUtils {
        public static Matrix CleanMatrix( Matrix matrix ) {
            var newMatrix = new Matrix();
            for( var i = 0; i < 16; i++ ) {
                var m = matrix[i];
                if( m > 0.999 && m < 1 ) {
                    newMatrix[i] = 1;
                }
                else if( m > -0.001 && m < 0 ) {
                    newMatrix[i] = 0;
                }
                else if( m < 0.001 && m > 0 ) {
                    newMatrix[i] = 0;
                }
                else {
                    newMatrix[i] = m;
                }
            }
            return newMatrix;
        }

        public static BoneSkinnedMeshGeometry3D CreateSkeletonMesh( IList<Bone> bones, int selectedIdx ) {
            var pyramid = new MeshBuilder( true, false );
            pyramid.AddPyramid( new Vector3( 0, 0, 0 ), Vector3.UnitZ, Vector3.UnitX, 1, 0, true );
            var singleBone = pyramid.ToMesh();

            //var boneIds = new List<BoneIds>();
            var positions = new Vector3Collection( bones.Count * singleBone.Positions.Count );
            var tris = new IntCollection( bones.Count * singleBone.Indices.Count );
            var colors = new Color4Collection( positions.Capacity );

            // ====== BONE LENGTH =========

            List<float> boneScale = new();

            for( var i = 0; i < bones.Count; i++ ) { boneScale.Add( -1 ); }
            for( var i = 0; i < bones.Count; i++ ) {
                var parent = bones[i].ParentIndex;

                if( parent >= 0 ) {
                    var startPos = Vector3.TransformCoordinate( new Vector3( 0 ), bones[i].BindPose );
                    var endPos = Vector3.TransformCoordinate( new Vector3( 0 ), bones[parent].BindPose );

                    var length = ( endPos - startPos ).Length();
                    boneScale[i] = length;
                    if( length > boneScale[parent] ) boneScale[parent] = length;
                }
            }
            for( var i = 0; i < bones.Count; i++ ) {
                var scale = boneScale[i];
                boneScale[i] = scale == -1 ? 0.02f : ( float )( Math.Sqrt( scale ) / 15f );
            }

            // =================

            var offset = 0;

            for( var i = 0; i < bones.Count; ++i ) {
                var scale = boneScale[i] / 2;
                var count = positions.Count;

                if( bones[i].ParentIndex >= 0 ) {
                    tris.AddRange( singleBone.Indices.Select( x => x + offset ) );

                    var j = 0;
                    for( ; j < singleBone.Positions.Count - 6; j += 3 ) { // iterate over everything but last 2 faces
                        positions.Add( Vector3.TransformCoordinate( singleBone.Positions[j] * scale, bones[bones[i].ParentIndex].BindPose ) );
                        positions.Add( Vector3.TransformCoordinate( singleBone.Positions[j + 1] * scale, bones[bones[i].ParentIndex].BindPose ) );
                        positions.Add( bones[i].BindPose.TranslationVector );

                        //boneIds.Add( new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4( 1, 0, 0, 0 ) } );
                        //boneIds.Add( new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4( 1, 0, 0, 0 ) } );
                        //boneIds.Add( new BoneIds() { Bone1 = i, Weights = new Vector4( 1, 0, 0, 0 ) } );
                    }
                    for( ; j < singleBone.Positions.Count; ++j ) {
                        positions.Add( Vector3.TransformCoordinate( singleBone.Positions[j] * scale, bones[bones[i].ParentIndex].BindPose ) );
                        //boneIds.Add( new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4( 1, 0, 0, 0 ) } );
                    }
                    offset += singleBone.Positions.Count;
                }

                PushColor( colors, GetColor( i, selectedIdx ), positions.Count - count );
            }

            var sphere = new MeshBuilder( true, false );

            for( var i = 0; i < bones.Count; ++i ) {
                AddSphere( sphere, i, bones[i].BindPose, boneScale[i], GetColor( i, selectedIdx ), colors );
            }

            positions.AddRange( sphere.Positions );
            tris.AddRange( sphere.TriangleIndices.Select( x => x + offset ) );

            var mesh = new BoneSkinnedMeshGeometry3D() {
                Positions = positions,
                Indices = tris,
                //VertexBoneIds = boneIds,
                Colors = colors
            };
            mesh.Normals = mesh.CalculateNormals();
            return mesh;
        }

        private static void AddSphere( MeshBuilder builder, int idx, Matrix matrix, float diff, Color4 color, Color4Collection colors ) {
            var count = builder.Positions.Count;
            builder.AddSphere( Vector3.Zero, diff / 2, 12, 12 );

            for( var j = count; j < builder.Positions.Count; ++j ) {
                builder.Positions[j] = Vector3.TransformCoordinate( builder.Positions[j], matrix );
                //boneIds.Add( new BoneIds() { Bone1 = idx, Weights = new Vector4( 1, 0, 0, 0 ) } );
            }

            PushColor( colors, color, builder.Positions.Count - count );
        }

        private static Color4 GetColor( int idx, int selected ) => idx == selected ? new Color4( 0.980f, 0.621f, 0, 1 ) : new Color4( 1, 1, 1, 1 );

        private static void PushColor( Color4Collection colors, Color4 color, int n ) {
            for( var i = 0; i < n; i++ ) colors.Add( color );
        }
    }
}

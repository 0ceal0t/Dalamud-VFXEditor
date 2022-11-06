using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dalamud.Logging;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;

namespace VFXEditor.Animation {
    public class AnimationData {
        private readonly List<int> BoneParents = new();
        private readonly List<Matrix> BoneRefPoses = new();
        private readonly List<List<Matrix>> TrackData = new();
        private readonly List<int> BoneTrack = new();

        public int NumFrames { get; private set; } = 0;

        public AnimationData( string path ) {
            

            using var reader = new BinaryReader( File.Open( path, FileMode.Open ) );

            var numBones = reader.ReadInt32();
            for( var boneIdx = 0; boneIdx < numBones; boneIdx++ ) {
                BoneParents.Add( reader.ReadInt16() );
                BoneTrack.Add( -1 ); // default
            }

            for( var boneIdx = 0; boneIdx < numBones; boneIdx++ ) {
                BoneRefPoses.Add( ReadMatrix( reader ) );
            }

            var numTracks = reader.ReadInt32();
            for( var trackIdx = 0; trackIdx < numTracks; trackIdx++ ) {
                var boneIdx = reader.ReadInt16();
                if (boneIdx > 0) {
                    BoneTrack[boneIdx] = trackIdx;
                }
            }

            NumFrames = reader.ReadInt32();
            for( var frameIdx = 0; frameIdx < NumFrames; frameIdx++ ) {
                List<Matrix> frameData = new();

                for( var trackIdx = 0; trackIdx < numTracks; trackIdx++ ) {
                    frameData.Add( ReadMatrix( reader ) );
                }

                TrackData.Add( frameData );
            }
        }

        private static Matrix ReadMatrix( BinaryReader reader ) {
            var posX = reader.ReadSingle();
            var posY = reader.ReadSingle();
            var posZ = reader.ReadSingle();
            reader.ReadSingle(); // W

            var rotX = reader.ReadSingle();
            var rotY = reader.ReadSingle();
            var rotZ = reader.ReadSingle();
            var rotW = reader.ReadSingle();

            var scaleX = reader.ReadSingle();
            reader.ReadSingle(); // Y
            reader.ReadSingle(); // Z
            reader.ReadSingle(); // W

            var pos = new Vector3( posX, posY, posZ );
            var rot = new Quaternion( rotX, rotY, rotZ, rotW );

            return Matrix.AffineTransformation( scaleX, rot, pos );
        }

        public BoneSkinnedMeshGeometry3D GetBoneMesh(int frame) {
            List<Matrix> bindPoses = new();

            for( var idx = 0; idx < BoneParents.Count; idx++ ) {
                bindPoses.Add( Matrix.Identity );
            }

            for( var target = 0; target < BoneParents.Count; target++ ) {
                var current = target;
                while ( current >= 0 ) {
                    var targetPose = bindPoses[target];

                    var trackIdx = BoneTrack[current];
                    var framePose = trackIdx == -1 ? Matrix.Identity : TrackData[frame][trackIdx];
                    var currentPose = framePose;

                    bindPoses[target] = Matrix.Multiply( targetPose, currentPose );
                    current = BoneParents[current];
                }
            }

            List<Bone> bones = new();
            for( var idx = 0; idx < BoneParents.Count; idx++ ) {
                bones.Add( new Bone {
                    BindPose = CleanMatrix( bindPoses[idx]),
                    ParentIndex = BoneParents[idx]
                } );
            }

            return CreateSkeletonMesh( bones );
        }

        private static Matrix CleanMatrix( Matrix matrix ) {
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

        public static BoneSkinnedMeshGeometry3D CreateSkeletonMesh( IList<Bone> bones ) {
            var builder = new MeshBuilder( true, false );
            builder.AddPyramid( new Vector3( 0, 0, 0 ), Vector3.UnitZ, Vector3.UnitX, 1, 0, true );
            var singleBone = builder.ToMesh();
            var boneIds = new List<BoneIds>();
            var positions = new Vector3Collection( bones.Count * singleBone.Positions.Count );
            var tris = new IntCollection( bones.Count * singleBone.Indices.Count );

            var offset = 0;

            // calculate length of connections
            List<float> boneScale = new();
            for( var i = 0; i < bones.Count; i++ ) {
                boneScale.Add( -1 );
            }
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
                boneScale[i] = scale == -1 ? 0.02f : (float) (Math.Sqrt( scale ) / 15f);
            }

            for( var i = 0; i < bones.Count; ++i ) {
                var scale = boneScale[i] / 2;

                if( bones[i].ParentIndex >= 0 ) {
                    var currPos = positions.Count;
                    tris.AddRange( singleBone.Indices.Select( x => x + offset ) );

                    var j = 0;
                    for( ; j < singleBone.Positions.Count - 6; j += 3 ) { // iterate over everything but last 2 faces
                        positions.Add( Vector3.TransformCoordinate( singleBone.Positions[j] * scale, bones[bones[i].ParentIndex].BindPose ) );
                        positions.Add( Vector3.TransformCoordinate( singleBone.Positions[j + 1] * scale, bones[bones[i].ParentIndex].BindPose ) );
                        positions.Add( bones[i].BindPose.TranslationVector );

                        boneIds.Add( new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4( 1, 0, 0, 0 ) } );
                        boneIds.Add( new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4( 1, 0, 0, 0 ) } );
                        boneIds.Add( new BoneIds() { Bone1 = i, Weights = new Vector4( 1, 0, 0, 0 ) } );
                    }
                    for( ; j < singleBone.Positions.Count; ++j ) {
                        positions.Add( Vector3.TransformCoordinate( singleBone.Positions[j] * scale, bones[bones[i].ParentIndex].BindPose ) );
                        boneIds.Add( new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4( 1, 0, 0, 0 ) } );
                    }
                    offset += singleBone.Positions.Count;
                }
            }

            builder = new MeshBuilder( true, false );
            for( var i = 0; i < bones.Count; ++i ) {
                var currPos = builder.Positions.Count;

                // on a human, generally less than 0.5
                // 0 = small
                // 0.5 = good size (size = 0.05)
                // very large = don't make them toooooo big

                var diff = boneScale[i];

                builder.AddSphere( Vector3.Zero, diff / 2, 12, 12 );

                for( var j = currPos; j < builder.Positions.Count; ++j ) {
                    builder.Positions[j] = Vector3.TransformCoordinate( builder.Positions[j], bones[i].BindPose );
                    boneIds.Add( new BoneIds() { Bone1 = i, Weights = new Vector4( 1, 0, 0, 0 ) } );
                }
            }

            positions.AddRange( builder.Positions );
            tris.AddRange( builder.TriangleIndices.Select( x => x + offset ) );
            var mesh = new BoneSkinnedMeshGeometry3D() { Positions = positions, Indices = tris, VertexBoneIds = boneIds };
            mesh.Normals = mesh.CalculateNormals();
            return mesh;
        }
    }
}

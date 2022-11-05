using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using SharpDX.DXGI;

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
                    //var currentPose = Matrix.Multiply( framePose, BoneRefPoses[current] );
                    var currentPose = framePose;

                    bindPoses[target] = Matrix.Multiply( targetPose, currentPose );
                    current = BoneParents[current];
                }
            }

            List<Bone> bones = new();
            for( var idx = 0; idx < BoneParents.Count; idx++ ) {
                bones.Add( new Bone {
                    BindPose = bindPoses[idx],
                    ParentIndex = BoneParents[idx]
                } );
            }

            return BoneSkinnedMeshGeometry3D.CreateSkeletonMesh( bones, 0.05f );
        }
    }
}

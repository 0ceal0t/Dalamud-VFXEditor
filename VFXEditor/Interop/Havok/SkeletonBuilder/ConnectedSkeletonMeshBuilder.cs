using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System;
using System.Collections.Generic;

namespace VfxEditor.Interop.Havok.SkeletonBuilder {
    public class ConnectedSkeletonMeshBuilder : SkeletonMeshBuilder {
        private readonly int SelectedIdx;
        private readonly HashSet<int> DimmedIdx;

        public ConnectedSkeletonMeshBuilder( IList<Bone> bones, int selectedIdx = -1, HashSet<int> dimmedIdx = null ) : base( bones ) {
            SelectedIdx = selectedIdx;
            DimmedIdx = dimmedIdx;
        }

        protected override void PopulateBoneScales( int idx ) {
            var parent = Bones[idx].ParentIndex;
            if( parent == -1 ) {
                BoneScales[idx] = 0.02f;
                return;
            }

            var startPos = Vector3.TransformCoordinate( new Vector3( 0 ), Bones[idx].BindPose );
            var endPos = Vector3.TransformCoordinate( new Vector3( 0 ), Bones[parent].BindPose );

            var length = ( endPos - startPos ).Length();
            var scale = ( float )( Math.Sqrt( length ) / 15f );

            BoneScales[idx] = scale;
            if( length > BoneScales[parent] ) BoneScales[parent] = scale;
        }

        protected override void PopulateBone( int idx ) {
            var parent = Bones[idx].ParentIndex;
            if( parent == -1 ) return;

            AddPyramid( BoneScales[idx] / 2f, Bones[parent].BindPose, Bones[idx].BindPose );
        }

        protected override void PopulateSpheres( int idx ) {
            AddSphere( idx, Bones[idx].BindPose, BoneScales[idx] );
        }

        protected override Color4 GetColor( int idx ) {
            if( idx == SelectedIdx ) return new Color4( 0.980f, 0.621f, 0f, 1f );
            if( DimmedIdx != null && DimmedIdx.Contains( idx ) ) return new Color4( 0.85f, 0.5f, 0.5f, 1f );

            return new Color4( 1f, 1f, 1f, 1f );
        }
    }
}

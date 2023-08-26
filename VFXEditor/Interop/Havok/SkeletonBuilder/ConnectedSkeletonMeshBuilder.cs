using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System;
using System.Collections.Generic;

namespace VfxEditor.Interop.Havok.SkeletonBuilder {
    public class ConnectedSkeletonMeshBuilder : SkeletonMeshBuilder {
        public ConnectedSkeletonMeshBuilder( IList<Bone> bones, int selectedIdx ) : base( bones, selectedIdx ) { }

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
    }
}

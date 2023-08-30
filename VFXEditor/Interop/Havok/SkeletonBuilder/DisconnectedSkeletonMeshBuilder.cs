using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System;
using System.Collections.Generic;

namespace VfxEditor.Interop.Havok.SkeletonBuilder {
    public class DisconnectedSkeletonMeshBuilder : SkeletonMeshBuilder {
        private readonly bool Perpendicular;

        public DisconnectedSkeletonMeshBuilder( IList<Bone> bones, int selectedIdx, bool perpendicular ) : base( bones, selectedIdx ) {
            Perpendicular = perpendicular;
        }

        protected override void PopulateBoneScales( int idx ) {
            var parent = Bones[idx].ParentIndex;
            if( parent == -1 ) {
                BoneScales[idx] = 0.2f;
                return;
            }

            var startPos = Vector3.TransformCoordinate( new Vector3( 0 ), Bones[idx].BindPose );
            var endPos = Vector3.TransformCoordinate( new Vector3( 0 ), Bones[parent].BindPose );

            var length = ( endPos - startPos ).Length();
            if( Bones[parent].ParentIndex == -1 ) length /= 5;
            BoneScales[idx] = length;
        }

        protected override void PopulateBone( int idx ) {
            // ===== BONE ======

            var matrix = Bones[idx].BindPose;
            var rotMatrix = Matrix.RotationZ( ( float )( Math.PI / 2 ) );
            var startMartix = Perpendicular ? Matrix.Multiply( rotMatrix, matrix ) : matrix;

            var posMatrix = Matrix.Translation( new( 0.70f * BoneScales[idx], 0, 0 ) );
            var endMatrix = Matrix.Multiply( posMatrix, startMartix );

            AddPyramid( 0.15f * BoneScales[idx], startMartix, endMatrix );

            // ====== CAP ========

            var capRotMatrix = Matrix.RotationZ( -( float )( Math.PI ) );
            var capStartMartix = Matrix.Multiply( capRotMatrix, startMartix );

            var capPosMatrix = Matrix.Translation( new( 0.15f * BoneScales[idx], 0, 0 ) );
            var capEndMatrix = Matrix.Multiply( capPosMatrix, capStartMartix );

            AddPyramid( 0.15f * BoneScales[idx], capStartMartix, capEndMatrix );
        }

        protected override void PopulateSpheres( int idx ) { }
    }
}

using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.EidFormat.BindPoint {
    public abstract class EidBindPoint : IUiItem {
        public abstract string GetName();

        public abstract void Draw();

        public abstract void Write( BinaryWriter writer );

        protected abstract Vector3 GetOffset();

        protected abstract Quaternion GetRotation();

        protected abstract string GetBoneName();

        public void AddBindPoint( MeshBuilder mesh, Dictionary<string, Bone> boneMatrixes ) {
            // Test if x,y,z line up the way that's expected
            // ^ same but for rotation
            // which gets applied first?
            // is it relative to the bone's rotation?

            if( !boneMatrixes.TryGetValue( GetBoneName(), out var bone ) ) return;

            var offset = GetOffset();
            var rotation = GetRotation();

            var transform = Matrix.AffineTransformation( 1, rotation, offset );
            var matrix = Matrix.Multiply( transform, bone.BindPose );

            var startPos = Vector3.TransformCoordinate( new Vector3( 0 ), matrix );
            var endPos = Vector3.TransformCoordinate( new Vector3( 0, 0, 0.1f ), matrix );

            mesh.AddCone( startPos, endPos, 0.06f, true, 10 );
        }
    }
}

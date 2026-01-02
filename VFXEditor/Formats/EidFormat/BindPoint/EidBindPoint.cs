using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX.Animations;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.EidFormat.BindPoint {
    public abstract class EidBindPoint : IUiItem {
        public readonly EidFile File;

        public EidBindPoint( EidFile file ) {
            File = file;
        }

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

            var transform = MatrixHelper.AffineTransformation( 1, rotation, offset );
            var matrix = Matrix4x4.Multiply( transform, bone.BindPose );

            var startPos = Vector3Helper.TransformCoordinate( new Vector3( 0 ), matrix );
            var endPos = Vector3Helper.TransformCoordinate( new Vector3( 0, 0, 0.1f ), matrix );

            mesh.AddCone( startPos, endPos, 0.06f, true, 10 );
        }
    }
}

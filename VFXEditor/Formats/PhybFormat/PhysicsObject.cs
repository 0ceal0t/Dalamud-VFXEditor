using HelixToolkit.SharpDX.Animations;
using System.Collections.Generic;

namespace VfxEditor.Formats.PhybFormat {
    public interface IPhysicsObject {
        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes );
    }
}

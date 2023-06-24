using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;

namespace VfxEditor.PhybFormat {
    public interface IPhysicsObject {
        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes );
    }
}

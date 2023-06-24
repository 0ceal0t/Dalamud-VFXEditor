using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;

namespace VfxEditor.PhybFormat {
    public interface IPhysicsObject {
        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes );
    }
}

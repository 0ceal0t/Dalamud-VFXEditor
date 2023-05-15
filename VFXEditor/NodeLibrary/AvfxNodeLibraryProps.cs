using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.NodeLibrary {
    [Serializable]
    public class AvfxNodeLibraryProps {
        public enum Type {
            Node,
            Folder
        }

        public string Name;
        public string Id;
        public Type PropType = Type.Node;

        // Node only
        public string Path;
        public string Description;
        public Vector4 Color;

        // Folder only
        public List<AvfxNodeLibraryProps> Children = new();
    }
}

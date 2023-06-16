using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Library {
    public enum ItemType {
        Node,
        Folder,
        Texture
    }

    [Serializable]
    public class LibraryProps {

        public string Name;
        public string Id;
        public ItemType PropType = ItemType.Node;

        // Node + Texture only
        public string Path; // Local path for node, file path for texture
        public string Description;
        public Vector4 Color;

        // Folder only
        public List<LibraryProps> Children = new();
    }
}

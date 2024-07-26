using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.PbdFormat {
    public class PdbConnection {
        public readonly PbdDeformer Parent;
        public readonly PbdDeformer Child;
        public readonly PbdDeformer Sibling;
        public readonly PbdDeformer Deformer;

        public PdbConnection( List<PbdDeformer> deformers, BinaryReader reader ) {
            var parent = reader.ReadInt16();
            var child = reader.ReadInt16();
            var sibling = reader.ReadInt16();
            var deformer = reader.ReadInt16();

            Parent = parent == -1 ? null : deformers[parent];
            Child = child == -1 ? null : deformers[child];
            Sibling = sibling == -1 ? null : deformers[sibling];
            Deformer = deformer == -1 ? null : deformers[deformer];
        }
    }
}

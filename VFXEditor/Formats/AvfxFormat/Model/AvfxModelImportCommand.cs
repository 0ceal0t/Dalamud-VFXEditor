using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Model {
    public class AvfxModelImportCommand : ICommand {
        public readonly AvfxModel Model;
        public readonly List<AvfxIndex> NewIndex;
        public readonly List<AvfxVertex> NewVertex;
        public readonly List<AvfxIndex> OldIndex = new();
        public readonly List<AvfxVertex> OldVertex = new();

        public AvfxModelImportCommand( AvfxModel model, List<AvfxIndex> newIndex, List<AvfxVertex> newVertex ) {
            Model = model;
            NewIndex = newIndex;
            NewVertex = newVertex;
        }

        public void Execute() {
            OldIndex.AddRange( Model.Indexes.Indexes );
            OldVertex.AddRange( Model.Vertexes.Vertexes );
            Set( NewIndex, NewVertex );
        }

        public void Redo() => Set( NewIndex, NewVertex );

        public void Undo() => Set( OldIndex, OldVertex );

        private void Set( List<AvfxIndex> indexes, List<AvfxVertex> vertexes ) {
            Model.Indexes.Indexes.Clear();
            Model.Vertexes.Vertexes.Clear();
            Model.Indexes.Indexes.AddRange( indexes );
            Model.Vertexes.Vertexes.AddRange( vertexes );
            Model.RefreshModelPreview();
        }
    }
}

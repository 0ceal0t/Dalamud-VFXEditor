using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Model {
    public class AvfxModelImportCommand : ICommand {
        public readonly AvfxModel Model;
        public List<AvfxIndex> OldIndex = new();
        public List<AvfxIndex> NewIndex;
        public List<AvfxVertex> OldVertex = new();
        public List<AvfxVertex> NewVertex;

        public AvfxModelImportCommand( AvfxModel model, List<AvfxIndex> newIndex, List<AvfxVertex> newVertex ) {
            Model = model;
            NewIndex = newIndex;
            NewVertex = newVertex;
            OldIndex.AddRange( model.Indexes.Indexes );
            OldVertex.AddRange( model.Vertexes.Vertexes );
        }

        public void Execute() => Set( NewIndex, NewVertex );

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

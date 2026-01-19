using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Model {
    public class AvfxModelImportCommand : ICommand {
        public readonly AvfxModel Model;
        public readonly (List<AvfxIndex>, List<AvfxVertex>) State;
        public readonly (List<AvfxIndex>, List<AvfxVertex>) PrevState;

        public AvfxModelImportCommand( AvfxModel model, List<AvfxIndex> newIndex, List<AvfxVertex> newVertex ) {
            Model = model;
            State = (newIndex, newVertex);
            PrevState = ([.. Model.Indexes.Indexes], [.. Model.Vertexes.Vertexes]);
            SetState( State );
        }

        public void Redo() => SetState( State );

        public void Undo() => SetState( PrevState );

        private void SetState( (List<AvfxIndex>, List<AvfxVertex>) state ) {
            Model.Indexes.Indexes.Clear();
            Model.Vertexes.Vertexes.Clear();
            Model.Indexes.Indexes.AddRange( state.Item1 );
            Model.Vertexes.Vertexes.AddRange( state.Item2 );
            Model.Updated();
        }
    }
}

using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Model {
    public class AvfxEmitImportCommand : ICommand {
        public readonly AvfxModel Model;
        public readonly (List<UiEmitVertex>, List<UiVertexNumber>) State;
        public readonly (List<UiEmitVertex>, List<UiVertexNumber>) PrevState;

        public AvfxEmitImportCommand( AvfxModel model, List<UiEmitVertex> newEmit, List<UiVertexNumber> newNumbers ) {
            Model = model;
            State = (newEmit, newNumbers);
            PrevState = ([.. Model.AllEmitVertexes], [.. Model.AllVertexNumbers]);
            SetState( State );
        }

        public void Redo() => SetState( State );

        public void Undo() => SetState( PrevState );

        private void SetState( (List<UiEmitVertex>, List<UiVertexNumber>) state ) {
            Model.AllEmitVertexes.Clear();
            Model.AllVertexNumbers.Clear();
            Model.AllEmitVertexes.AddRange( state.Item1 );
            Model.AllVertexNumbers.AddRange( state.Item2 );
            Model.Updated();
        }
    }
}

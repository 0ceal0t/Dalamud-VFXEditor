using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiAssignableCommandMultiple : ICommand {
        private readonly List<AVFXBase> Item;
        private readonly bool State;

        public UiAssignableCommandMultiple( List<AVFXBase> item, bool state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            Item.ForEach( x => x.SetAssigned( State ) );
        }

        public void Redo() {
            Item.ForEach( x => x.SetAssigned( State ) );
        }

        public void Undo() {
            Item.ForEach( x => x.SetAssigned( !State ) );
        }
    }
}

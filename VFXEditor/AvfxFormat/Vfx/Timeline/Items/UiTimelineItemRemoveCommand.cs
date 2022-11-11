using System;
using System.Collections.Generic;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineItemRemoveCommand : ICommand {
        private readonly UiTimelineItemSequencer View;
        private readonly UiTimelineItem Item;
        private readonly int Idx;

        public UiTimelineItemRemoveCommand( UiTimelineItemSequencer view, UiTimelineItem item ) {
            View = view;
            Item = item;
            Idx = view.Items.IndexOf( item );
        }

        public void Execute() => Redo();

        public void Redo() {
            View.Items.Remove( Item );
            View.Timeline.Timeline.Items.Remove( Item.Item );
            View.UpdateIdx();
            View.ClearSelected();

            Item.BinderSelect.Disable();
            Item.EmitterSelect.Disable();
            Item.EffectorSelect.Disable();
        }

        public void Undo() {
            View.Items.Insert( Idx, Item );
            View.Timeline.Timeline.Items.Insert( Idx, Item.Item );
            View.UpdateIdx();

            Item.BinderSelect.Enable();
            Item.EmitterSelect.Enable();
            Item.EffectorSelect.Enable();
        }
    }
}

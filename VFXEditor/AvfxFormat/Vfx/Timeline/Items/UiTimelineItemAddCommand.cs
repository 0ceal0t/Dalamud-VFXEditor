using System;
using System.Collections.Generic;
using VfxEditor.AVFXLib.Timeline;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineItemAddCommand: ICommand {
        private readonly UiTimelineItemSequencer View;
        private readonly int Idx;
        private UiTimelineItem Item;

        public UiTimelineItemAddCommand( UiTimelineItemSequencer view ) {
            View = view;
            Idx = View.Items.Count;
        }

        public void Execute() {
            var newItem = new AVFXTimelineSubItem();
            newItem.BinderIdx.SetValue( -1 );
            newItem.EffectorIdx.SetValue( -1 );
            newItem.EmitterIdx.SetValue( -1 );
            newItem.EndTime.SetValue( 1 );
            newItem.Platform.SetValue( 0 );
            Item = new UiTimelineItem( newItem, View.Timeline );

            Add();

            View.Selected = Item;
        }

        public void Redo() {
            Add();

            Item.BinderSelect.Enable();
            Item.EmitterSelect.Enable();
            Item.EffectorSelect.Enable();
        }

        public void Undo() {
            View.Items.Remove( Item );
            View.Timeline.Timeline.Items.Remove( Item.Item );
            View.UpdateIdx();
            View.ClearSelected();

            Item.BinderSelect.Disable();
            Item.EmitterSelect.Disable();
            Item.EffectorSelect.Disable();
        }

        private void Add() {
            View.Items.Insert( Idx, Item );
            View.Timeline.Timeline.Items.Insert( Idx, Item.Item );
            View.UpdateIdx();
        }
    }
}

using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects {
    public class SgbObjectSplitView : CommandSplitView<SgbObject> {
        public SgbObjectSplitView( List<SgbObject> items ) : base( "Object", items, false, ( SgbObject item, int idx ) => $"{item.Name} ({item.Type})", null ) { }

        protected override void DrawControls() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) ImGui.OpenPopup( "NewObjectPopup" );

            if( ImGui.BeginPopup( "NewObjectPopup" ) ) {
                DrawNewPopup();
                ImGui.EndPopup();
            }

            if( Selected != null ) {
                ImGui.SameLine();
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    OnDelete( Selected );
                    Selected = null;
                }
            }
        }

        private void DrawNewPopup() {
            using var _ = ImRaii.PushId( "NewObject" );

            foreach( var option in SgbObjectUtils.ObjectTypes ) {
                if( ImGui.Selectable( $"{option.Key}" ) ) {
                    var type = option.Value;
                    var constructor = type.GetConstructor( Array.Empty<Type>() );
                    var newEntry = ( SgbObject )constructor.Invoke( Array.Empty<object>() );
                    CommandManager.Add( new ListAddCommand<SgbObject>( Items, newEntry ) );
                }
            }
        }
    }
}

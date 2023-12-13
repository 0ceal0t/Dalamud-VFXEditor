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
        public static readonly List<Type> DangerousObjectTypes = new() {
            // TODO
        };

        public SgbObjectSplitView( List<SgbObject> items ) : base( "Object", items, false, ( SgbObject item, int idx ) => $"{item.Name} ({item.Type})", null ) { }

        protected override void DrawControls() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) ImGui.OpenPopup( "NewObjectPopup" );

            if( ImGui.BeginPopup( "NewObjectPopup" ) ) {
                DrawNewPopup();
                ImGui.EndPopup();
            }

            if( Selected != null && !DangerousObjectTypes.Contains( Selected.GetType() ) ) {
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
                if( DangerousObjectTypes.Contains( option.Value ) ) continue;
                if( ImGui.Selectable( $"{option.Key}" ) ) {
                    var constructor = option.Value.GetConstructor( new Type[] { typeof( LayerEntryType ) } );
                    var newEntry = ( SgbObject )constructor.Invoke( new object[] { option.Key } );
                    CommandManager.Add( new ListAddCommand<SgbObject>( Items, newEntry ) );
                }
            }
        }

        protected override bool DrawLeftItem( SgbObject item, int idx ) {
            using var style = ImRaii.PushColor( ImGuiCol.Text, UiUtils.RED_COLOR, DangerousObjectTypes.Contains( item.GetType() ) );
            return base.DrawLeftItem( item, idx );
        }

        protected override void DrawSelected() {
            using var disabled = ImRaii.Disabled( DangerousObjectTypes.Contains( Selected.GetType() ) );
            base.DrawSelected();
        }
    }
}

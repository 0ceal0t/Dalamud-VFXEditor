using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiNodeSelectList<T> : UiNodeSelect where T : UiNode {
        public List<T> Selected = new();
        public AVFXIntList Literal;
        public UiNodeGroup<T> Group;
        public string Name;

        public UiNodeSelectList( UiNode node, string name, UiNodeGroup<T> group, AVFXIntList literal ) {
            Node = node;
            Name = name;
            Group = group;
            Literal = literal;
            LinkEvent();
            if( Group.IsInitialized ) SetupNode();
            else Group.OnInit += SetupNode;
            node.Selectors.Add( this );
        }

        public override void DrawInline( string parentId ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[Name] = Literal.IsAssigned();
                CopyManager.Ints[Name] = Literal.GetValue()[0];
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue( Name, out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal, a ) );
                if( CopyManager.Ints.TryGetValue( Name, out var l ) ) CopyManager.PasteCommand.Add( new UiIntListCommand( Literal, l ) );


                foreach( var selected in Selected ) UnlinkFrom( selected );
                Selected.Clear();
                foreach( var item in Literal.GetValue() ) {
                    if( item >= 0 && item < Group.Items.Count ) {
                        Selected.Add( Group.Items[item] );
                        LinkTo( Group.Items[item] );
                    }
                    else Selected.Add( null );
                }
            }

            var id = parentId + "/Node";

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            for( var idx = 0; idx < Selected.Count; idx++ ) {
                var itemId = id + idx;
                var text = ( idx == 0 ) ? Name : "";
                if( ImGui.BeginCombo( text + itemId, Selected[idx] == null ? "[NONE]" : Selected[idx].GetText() ) ) {
                    if( ImGui.Selectable( "[NONE]", Selected[idx] == null ) ) CommandManager.Avfx.Add( new UiNodeSelectListCommand<T>( this, null, idx ) );
                    foreach( var item in Group.Items ) {
                        if( ImGui.Selectable( item.GetText(), Selected[idx] == item ) ) CommandManager.Avfx.Add( new UiNodeSelectListCommand<T>( this, item, idx ) );
                        if( ImGui.IsItemHovered() ) item.ShowTooltip();
                    }
                    ImGui.EndCombo();
                }

                IUiBase.DrawRemoveContextMenu( Literal, Name, id );

                if( idx > 0 ) {
                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( "- Remove" + itemId, small: true ) ) {
                        CommandManager.Avfx.Add( new UiNodeSelectListRemoveCommand<T>( this, idx ) );
                        return;
                    }
                }
            }

            if( Selected.Count == 0 ) {
                ImGui.Text( Name );
                ImGui.TextColored( UiUtils.RED_COLOR, "WARNING: Add an item!" );
            }
            if( Group.Items.Count == 0 )ImGui.TextColored( UiUtils.RED_COLOR, "WARNING: Add a selectable item first!" );
            if( Selected.Count < 4 ) {
                if( ImGui.SmallButton( "+ " + Name + id ) ) CommandManager.Avfx.Add( new UiNodeSelectListAddCommand<T>( this ) );

                IUiBase.DrawRemoveContextMenu( Literal, Name, id );
            }
        }

        // Internal selection
        public void Select( T item, int idx ) {
            if( item == null ) SelectNone( idx );
            else SelectItem( item, idx );
        }

        private void SelectNone( int idx ) {
            if( Selected == null ) return;
            UnlinkFrom( Selected[idx] );
            Selected[idx] = null;
            UpdateNode();
        }

        private void SelectItem( T item, int idx ) {
            if( Selected[idx] == item ) return;
            UnlinkFrom( Selected[idx] );
            LinkTo( item );
            Selected[idx] = item;
            UpdateNode();
        }

        public override void Enable() {
            LinkEvent();
            foreach( var item in Selected ) LinkTo( item );
        }

        public override void Disable() {
            UnlinkEvent();
            foreach( var item in Selected ) UnlinkFrom( item );
        }

        public override void LinkEvent() {
            Group.OnChange += UpdateNode;
        }

        public override void UnlinkEvent() {
            Group.OnChange -= UpdateNode;
        }

        public override void UpdateNode() {
            var idxs = new List<int>();
            foreach( var item in Selected ) {
                if( item == null ) idxs.Add( 255 );
                else idxs.Add( item.Idx );
            }
            Literal.SetValue( idxs );
        }

        public override void SetupNode() {
            for( var i = 0; i < Literal.GetValue().Count; i++ ) {
                var val = Literal.GetValue()[i];
                if( Node.HasDependencies && val != 255 && val >= 0 ) {
                    val += Group.PreImportSize;
                    Literal.SetValue( val, i );
                }
                if( val != 255 && val >= 0 && val < Group.Items.Count ) {
                    var item = Group.Items[val];
                    Selected.Add( item );
                    LinkTo( item );
                }
                else {
                    Selected.Add( null );
                }
            }
        }

        // External selection
        public override List<int> GetNodeIdx( UiNode node ) {
            List<int> idx = new();
            for( var i = 0; i < Selected.Count; i++ ) {
                if( Selected[i] == node ) idx.Add( i );
            }
            return idx;
        }

        public override void NodeEnabled( UiNode node, int idx ) {
            Selected.Insert( idx, ( T )node );
            UpdateNode();
        }

        public override void NodeDisabled( UiNode node ) {
            Selected.RemoveAll( x => x == node );
            UpdateNode();
        }
    }
}

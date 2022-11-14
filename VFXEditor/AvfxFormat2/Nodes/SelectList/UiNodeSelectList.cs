using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat2 {
    public class UiNodeSelectList<T> : UiNodeSelect where T : AvfxNode {
        public List<T> Selected = new();
        public readonly AvfxIntList Literal;
        public readonly UiNodeGroup<T> Group; // the group being selected from
        public readonly string Name;

        private bool Enabled = true;

        public UiNodeSelectList( AvfxNode node, string name, UiNodeGroup<T> group, AvfxIntList literal ) : base( node ) {
            Name = name;
            Group = group;
            Literal = literal;
            LinkOnIndexChange();
            if( Group.IsInitialized ) Initialize(); // already good to go
            else Group.OnInit += Initialize;
            node.Selectors.Add( this );
        }

        // an item was removed from the group, for example
        // 255 = -1 = nothing selected
        public override void UpdateLiteral() => Literal.SetValue( Selected.Select( x => x == null ? 255 : x.GetIdx() ).ToList() );

        public override void Initialize() {
            for( var i = 0; i < Literal.GetValue().Count; i++ ) {
                var value = Literal.GetValue()[i];
                if( Node.DepedencyImportInProgress && value != 255 && value >= 0 ) {
                    value += Group.PreImportSize;
                    Literal.SetValue( value, i );
                }
                if( value != 255 && value >= 0 && value < Group.Items.Count ) {
                    var item = Group.Items[value];
                    Selected.Add( item );
                    LinkParentChild( item );
                }
                else Selected.Add( null );
            }
        }

        public void Select( T item, int idx ) {
            if( item == null ) SelectNone( idx );
            else SelectItem( item, idx );
        }

        private void SelectNone( int idx ) {
            if( Selected[idx] == null ) return;
            UnlinkParentChild( Selected[idx] );
            Selected[idx] = null;
            UpdateLiteral();
        }

        private void SelectItem( T item, int idx ) {
            if( Selected[idx] == item ) return;
            UnlinkParentChild( Selected[idx] );
            LinkParentChild( item );
            Selected[idx] = item;
            UpdateLiteral();
        }

        public override void LinkOnIndexChange() {
            if( OnChangeLinked ) return;
            OnChangeLinked = true;
            Group.OnChange += UpdateLiteral;
        }

        public override void UnlinkOnIndexChange() {
            if( !OnChangeLinked ) return;
            OnChangeLinked = false;
            Group.OnChange -= UpdateLiteral;
        }

        // For when something happens to the selector

        public override void Enable() {
            if( Enabled ) return;
            Enabled = true;
            LinkOnIndexChange();
            foreach( var item in Selected ) {
                if( item != null ) LinkParentChild( item );
            }
        }

        public override void Disable() {
            if( !Enabled ) return;
            Enabled = false;
            UnlinkOnIndexChange();
            foreach( var item in Selected ) {
                if( item != null ) UnlinkParentChild( item );
            }
        }

        // For when something happens to the selected node

        public override List<int> GetSelectedIdx( AvfxNode node ) {
            List<int> idx = new();
            for( var i = 0; i < Selected.Count; i++ ) {
                if( Selected[i] == node ) idx.Add( i );
            }
            return idx;
        }

        public override void EnableNode( AvfxNode node, int idx ) {
            Selected.Insert( idx, ( T )node );
            UpdateLiteral();
        }

        public override void DisableNode( AvfxNode node ) {
            Selected.RemoveAll( x => x == node );
            UpdateLiteral();
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Node";

            // Unassigned
            if( AvfxBase.DrawAddButton( Literal, Name, id ) ) return;

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

                AvfxBase.DrawRemoveContextMenu( Literal, Name, id );

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
            if( Group.Items.Count == 0 ) ImGui.TextColored( UiUtils.RED_COLOR, "WARNING: Add a selectable item first!" );
            if( Selected.Count < 4 ) {
                if( ImGui.SmallButton( "+ " + Name + id ) ) CommandManager.Avfx.Add( new UiNodeSelectListAddCommand<T>( this ) );

                AvfxBase.DrawRemoveContextMenu( Literal, Name, id );
            }
        }
    }
}

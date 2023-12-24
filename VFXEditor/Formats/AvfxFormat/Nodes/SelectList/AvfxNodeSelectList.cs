using Dalamud.Interface;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Data.Copy;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxNodeSelectList<T> : AvfxNodeSelect where T : AvfxNode {
        public List<T> Selected = new();
        public readonly AvfxIntList Literal;
        public readonly NodeGroup<T> Group; // the group being selected from
        public readonly string Name;

        private bool Enabled = true;

        public AvfxNodeSelectList( AvfxNode node, string name, NodeGroup<T> group, AvfxIntList literal ) : base( node ) {
            Name = name;
            Group = group;
            Literal = literal;
            if( Group.ImportInProgress ) group.OnImportFinish += ImportFinished;
            else {
                LinkOnIndexChange();
                if( Group.IsInitialized ) Initialize(); // already good to go
                else Group.OnInit += Initialize;
            }
            node.Selectors.Add( this );
        }

        // an item was removed from the group, for example
        // 255 = -1 = nothing selected
        public override void UpdateLiteral() => Literal.SetItems( Selected.Select( x => x == null ? 255 : x.GetIdx() ).ToList() );

        public override void Initialize() {
            for( var idx = 0; idx < Literal.GetItems().Count; idx++ ) {
                var value = Literal.GetItems()[idx];
                if( value != 255 && value >= 0 && value < Group.Items.Count ) {
                    var item = Group.Items[value];
                    Selected.Add( item );
                    LinkParentChild( item );
                }
                else Selected.Add( null );
            }
        }

        public void ImportFinished() {
            for( var idx = 0; idx < Literal.GetItems().Count; idx++ ) {
                var value = Literal.GetItems()[idx];
                if( value != 255 && value >= 0 ) Literal.SetItem( value + Group.PreImportSize, idx );
            }
            LinkOnIndexChange();
            Initialize();
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

        public override void Draw() => Draw( UiUtils.GetOffsetInputSize( FontAwesomeIcon.Share ) );

        public void Draw( float inputWidth ) {
            using var _ = ImRaii.PushId( $"Node{Name}" );

            // Unassigned
            Literal.AssignedCopyPaste( Name );
            if( Literal.DrawAssignButton( Name ) ) return;

            // Copy/Paste
            if( CopyManager.IsCopying ) {
                for( var idx = 0; idx < Selected.Count; idx++ ) CopyManager.TrySetValue( this, $"{Name}_{idx}", Literal.GetItems()[idx] );
            }
            if( CopyManager.IsPasting ) {
                for( var idx = 0; idx < Selected.Count; idx++ ) {
                    if( CopyManager.TryGetValue<int>( this, $"{Name}_{idx}", out var val ) ) {
                        var newSelected = ( val == -1 || val >= Group.Items.Count ) ? null : Group.Items[val];
                        CommandManager.Paste( new AvfxNodeSelectListCommand<T>( this, newSelected, idx ) );
                    }
                }
            }

            // Draw
            for( var idx = 0; idx < Selected.Count; idx++ ) {
                using var __ = ImRaii.PushId( idx );

                ImGui.SetNextItemWidth( inputWidth );
                DrawCombo( idx );

                Literal.DrawUnassignPopup( Name );

                // Draw go button
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    using var dimmed = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.5f, Selected[idx] == null );
                    if( ImGui.Button( FontAwesomeIcon.Share.ToIconString() ) ) Plugin.AvfxManager.File.SelectItem( Selected[idx] );
                }

                UiUtils.Tooltip( "Navigate to selected node" );

                if( idx == 0 ) {
                    ImGui.SameLine();
                    ImGui.Text( Name );
                }
                else {
                    // Remove button
                    ImGui.SameLine();
                    using var font = ImRaii.PushFont( UiBuilder.IconFont );
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                        CommandManager.Add( new AvfxNodeSelectListRemoveCommand<T>( this, idx ) );
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
                if( ImGui.SmallButton( $"+ {Name}" ) ) CommandManager.Add( new AvfxNodeSelectListAddCommand<T>( this ) );
                Literal.DrawUnassignPopup( Name );
            }
        }

        private void DrawCombo( int idx ) {
            using var combo = ImRaii.Combo( $"##Combo", Selected[idx] == null ? "[NONE]" : Selected[idx].GetText() );
            if( !combo ) return;

            if( ImGui.Selectable( "[NONE]", Selected[idx] == null ) ) CommandManager.Add( new AvfxNodeSelectListCommand<T>( this, null, idx ) ); // "None" selector
            foreach( var item in Group.Items ) {
                var cycle = Node.IsChildOf( item );
                using var disabled = ImRaii.Disabled( cycle );

                if( ImGui.Selectable( item.GetText(), Selected[idx] == item ) && !cycle ) CommandManager.Add( new AvfxNodeSelectListCommand<T>( this, item, idx ) );
                if( ImGui.IsItemHovered() ) item.ShowTooltip();
            }
        }
    }
}

using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
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
        public override void UpdateLiteral() => Literal.SetValue( Selected.Select( x => x == null ? 255 : x.GetIdx() ).ToList() );

        public override void Initialize() {
            for( var idx = 0; idx < Literal.GetValue().Count; idx++ ) {
                var value = Literal.GetValue()[idx];
                if( value != 255 && value >= 0 && value < Group.Items.Count ) {
                    var item = Group.Items[value];
                    Selected.Add( item );
                    LinkParentChild( item );
                }
                else Selected.Add( null );
            }
        }

        public void ImportFinished() {
            for( var idx = 0; idx < Literal.GetValue().Count; idx++ ) {
                var value = Literal.GetValue()[idx];
                if( value != 255 && value >= 0 ) Literal.SetValue( value + Group.PreImportSize, idx );
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

        public override void Draw( string parentId ) {
            var id = parentId + "/Node";

            // Unassigned
            AvfxBase.AssignedCopyPaste( Literal, Name );
            if( AvfxBase.DrawAddButton( Literal, Name, id ) ) return;

            // Copy/Paste
            var copy = CopyManager.Avfx;
            if( copy.IsCopying ) {
                for( var idx = 0; idx < Selected.Count; idx++ ) copy.Ints[$"{Name}_{idx}"] = Literal.GetValue()[idx];
            }
            if( copy.IsPasting ) {
                for(var idx = 0; idx < Selected.Count; idx++ ) {
                    if (copy.Ints.TryGetValue("", out var val ) ) {
                        var newSelected = ( val == -1 || val >= Group.Items.Count ) ? null : Group.Items[val];
                        copy.PasteCommand.Add( new UiNodeSelectListCommand<T>( this, newSelected, idx ) );
                    }
                }
            }

            // Draw
            var style = ImGui.GetStyle();
            ImGui.PushFont( UiBuilder.IconFont );
            var inputSize = UiUtils.GetOffsetInputSize( FontAwesomeIcon.Share );
            ImGui.PopFont();

            for( var idx = 0; idx < Selected.Count; idx++ ) {
                ImGui.SetNextItemWidth( inputSize );
                if( ImGui.BeginCombo( $"{id}{idx}-MainInput", Selected[idx] == null ? "[NONE]" : Selected[idx].GetText() ) ) {
                    if( ImGui.Selectable( "[NONE]", Selected[idx] == null ) ) CommandManager.Avfx.Add( new UiNodeSelectListCommand<T>( this, null, idx ) ); // "None" selector
                    foreach( var item in Group.Items ) {
                        if( ImGui.Selectable( item.GetText(), Selected[idx] == item ) ) CommandManager.Avfx.Add( new UiNodeSelectListCommand<T>( this, item, idx ) );
                        if( ImGui.IsItemHovered() ) item.ShowTooltip();
                    }
                    ImGui.EndCombo();
                }

                AvfxBase.DrawRemoveContextMenu( Literal, Name, id );

                // Draw go button
                ImGui.SameLine( inputSize + 2 );
                ImGui.PushFont( UiBuilder.IconFont );
                if( Selected[idx] == null ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Share}" + id + idx ) ) Plugin.AvfxManager.CurrentFile.SelectItem( Selected[idx] );
                if( Selected[idx] == null ) ImGui.PopStyleVar();
                ImGui.PopFont();

                if( idx == 0 ) {
                    ImGui.SameLine();
                    ImGui.Text( Name );
                }
                else {
                    // Remove button
                    ImGui.SameLine();
                    ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 4 );
                    ImGui.PushFont( UiBuilder.IconFont );
                    if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + id + idx ) ) {
                        CommandManager.Avfx.Add( new UiNodeSelectListRemoveCommand<T>( this, idx ) );
                        ImGui.PopFont();
                        return;
                    }
                    ImGui.PopFont();
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

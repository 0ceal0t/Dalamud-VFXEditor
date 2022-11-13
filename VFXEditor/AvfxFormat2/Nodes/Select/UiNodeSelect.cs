using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public abstract class UiNodeSelect : IUiBase {
        public readonly AvfxNode Node;
        protected bool ParentChildLinked = false;
        protected bool OnChangeLinked = false;

        public UiNodeSelect( AvfxNode node ) {
            Node = node;
        }

        public abstract void UpdateLiteral();
        public abstract void Initialize();

        public abstract void LinkOnIndexChange();
        public abstract void UnlinkOnIndexChange();

        public abstract void Draw( string id );

        // For when something happens to the selector

        public abstract void Enable();
        public abstract void Disable();

        // For when something happens to the selected node

        public abstract List<int> GetSelectedIdx( AvfxNode node );
        public abstract void EnableNode( AvfxNode node, int idx );
        public abstract void DisableNode( AvfxNode node );

        public void UnlinkParentChild( AvfxNode node ) {
            if( node == null ) return;
            if( !ParentChildLinked ) return;

            ParentChildLinked = false;
            Node.ChildNodes.Remove( node );
            node.Parents.Remove( this );
            node.Graph?.NowOutdated();
        }

        public void LinkParentChild( AvfxNode node ) {
            if( node == null ) return;
            if( ParentChildLinked ) return;

            ParentChildLinked = true;
            Node.ChildNodes.Add( node );
            node.Parents.Add( this );
            node.Graph?.NowOutdated();
        }
    }

    public class UiNodeSelect<T> : UiNodeSelect where T : AvfxNode {
        public T Selected = null;
        public readonly AvfxInt Literal;
        public readonly UiNodeGroup<T> Group; // the group being selected from
        public readonly string Name;

        private bool Enabled = true;

        public UiNodeSelect( AvfxNode node, string name, UiNodeGroup<T> group, AvfxInt literal ) : base( node ) {
            Name = name;
            Group = group;
            Literal = literal;
            LinkOnIndexChange();
            if( Group.IsInitialized ) Initialize(); // already good to go
            else Group.OnInit += Initialize;
            node.Selectors.Add( this );
        }

        public override void UpdateLiteral() => Literal.SetValue( Selected != null ? Selected.GetIdx() : -1 ); // an item was removed from the group, for example

        public override void Initialize() {
            var value = Literal.GetValue();
            if( Node.HasDependencies && value >= 0 ) {
                value += Group.PreImportSize;
                Literal.SetValue( value );
            }
            if( value >= 0 && value < Group.Items.Count ) {
                Selected = Group.Items[value];
                LinkParentChild( Selected );
            }
        }

        public void Select( T item ) {
            if( item == null ) SelectNone();
            else SelectItem( item );
        }

        private void SelectNone() {
            if( Selected == null ) return;
            UnlinkParentChild( Selected );
            Selected = null;
            UpdateLiteral();
        }

        private void SelectItem( T item ) {
            if( Selected == item ) return;
            UnlinkParentChild( Selected );
            LinkParentChild( item );
            Selected = item;
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

            if( Selected != null ) LinkParentChild( Selected );
        }

        public override void Disable() {
            if( !Enabled ) return;
            Enabled = false;
            UnlinkOnIndexChange();
            if( Selected != null ) UnlinkParentChild( Selected );
        }

        // For when something happens to the selected node

        public override List<int> GetSelectedIdx( AvfxNode node ) => null; // not used for this one, since there is only every 1 selector

        public override void EnableNode( AvfxNode node, int _ ) {
            Selected = ( T )node;
            UpdateLiteral();
        }

        public override void DisableNode( AvfxNode node ) {
            Selected = null;
            UpdateLiteral();
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Node";

            // Unassigned
            if( AvfxBase.DrawAddButton( Literal, Name, id ) ) return;

            if( ImGui.BeginCombo( Name + id, Selected == null ? "[NONE]" : Selected.GetText() ) ) {
                // "None" selector
                if( ImGui.Selectable( "[NONE]", Selected == null ) ) CommandManager.Avfx.Add( new UiNodeSelectCommand<T>( this, null ) );

                foreach( var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText(), Selected == item ) ) CommandManager.Avfx.Add( new UiNodeSelectCommand<T>( this, item ) );
                    if( ImGui.IsItemHovered() ) item.ShowTooltip();
                }
                ImGui.EndCombo();
            }

            AvfxBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}

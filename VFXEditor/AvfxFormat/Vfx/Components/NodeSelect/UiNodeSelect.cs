using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiNodeSelect : IUiBase {
        public UiNode Node;

        public void UnlinkFrom( UiNode node ) {
            if( node == null ) return;
            Node.Children.Remove( node );
            node.Parents.Remove( this );

            node.Graph?.NowOutdated();
        }

        public void LinkTo( UiNode node ) {
            if( node == null ) return;
            Node.Children.Add( node );
            node.Parents.Add( this );

            node.Graph?.NowOutdated();
        }

        // For when a select is modified, but not the underlying node
        // Example: emitter item, or model data
        public abstract void Enable();
        public abstract void Disable();

        public abstract void LinkEvent();
        public abstract void UnlinkEvent();

        public abstract List<int> GetNodeIdx( UiNode node );
        public abstract void NodeEnabled( UiNode node, int idx );
        public abstract void NodeDisabled( UiNode node );

        public abstract void UpdateNode();
        public abstract void SetupNode();

        public abstract void DrawInline(string id);
    }

    public class UiNodeSelect<T> : UiNodeSelect where T : UiNode {
        public T Selected = null;
        public AVFXInt Literal;
        public UiNodeGroup<T> Group;
        public string Name;

        public UiNodeSelect( UiNode node, string name, UiNodeGroup<T> group, AVFXInt literal ) : base() {
            Node = node;
            Name = name;
            Group = group;
            Literal = literal;
            Group.OnChange += UpdateNode;
            if( Group.IsInitialized ) SetupNode();
            else Group.OnInit += SetupNode;
            node.Selectors.Add( this );
        }

        public override void DrawInline( string parentId ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is AVFXInt literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                UnlinkFrom( Selected );
                if( Literal.GetValue() >= 0 && Literal.GetValue() < Group.Items.Count ) LinkTo( Selected = Group.Items[Literal.GetValue()] );
                else Selected = null;
            }

            var id = parentId + "/Node";

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            if( ImGui.BeginCombo( Name + id, Selected == null ? "[NONE]" : Selected.GetText() ) ) {
                if( ImGui.Selectable( "[NONE]", Selected == null ) ) CommandManager.Avfx.Add( new UiNodeSelectCommand<T>( this, null ) );
                foreach( var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText(), Selected == item ) ) CommandManager.Avfx.Add( new UiNodeSelectCommand<T>( this, item ) );
                    if( ImGui.IsItemHovered() ) item.ShowTooltip();
                }
                ImGui.EndCombo();
            }

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }

        // Internal selection
        public void Select( T item ) {
            if( item == null ) SelectNone();
            else SelectItem( item );
        }

        private void SelectNone() {
            if( Selected == null ) return;
            UnlinkFrom( Selected );
            Selected = null;
            UpdateNode();
        }

        private void SelectItem( T item ) {
            if( Selected == item ) return;
            UnlinkFrom( Selected );
            LinkTo( item );
            Selected = item;
            UpdateNode();
        }

        public override void Enable() {
            LinkEvent();
            if( Selected != null ) LinkTo( Selected );
        }

        public override void Disable() {
            UnlinkEvent();
            if( Selected != null ) UnlinkFrom( Selected );
        }

        public override void LinkEvent() {
            Group.OnChange += UpdateNode;
        }

        public override void UnlinkEvent() {
            Group.OnChange -= UpdateNode;
        }

        public override void UpdateNode() => Literal.SetValue( Selected != null ? Selected.Idx : -1 );

        public override void SetupNode() {
            var val = Literal.GetValue();
            if( Node.HasDependencies && val >= 0 ) {
                val += Group.PreImportSize;
                Literal.SetValue( val );
            }
            if( val >= 0 && val < Group.Items.Count ) {
                Selected = Group.Items[val];
                LinkTo( Selected );
            }
        }

        // External selection
        public override List<int> GetNodeIdx( UiNode node ) => null;

        public override void NodeEnabled( UiNode node, int _ ) {
            Selected = ( T )node;
            UpdateNode();
        }

        public override void NodeDisabled( UiNode node ) {
            Selected = null;
            UpdateNode();
        }
    }
}

using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using VfxEditor.TmbFormat;
using VfxEditor.TmbFormat.Actor;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.TmbFormat.Track {
    public class TmbTrackSplitView : ItemSplitView<Tmtr> {
        public readonly Tmac Actor;

        public TmbTrackSplitView( Tmac actor ) : base( "Tracks", actor.Tracks ) {
            Actor = actor;
        }

        protected override void DrawControls() => DrawNewDeleteControls( Actor.AddTrack, Actor.DeleteTrack );

        protected override bool DrawLeftItem( Tmtr item, int idx ) {
            using var _ = ImRaii.PushId( idx );

            var isColored = TmbEntry.DoColor( item.MaxDanger, out var col );
            using var color = ImRaii.PushColor( ImGuiCol.Text, col, isColored );

            if( ImGui.Selectable( $"Track {idx}", item == Selected ) ) {
                Selected = item;
            }

            if( ImGui.BeginDragDropTarget() ) {
                Actor.File.StopDragging( item );
                ImGui.EndDragDropTarget();
            }
            return false;
        }

        protected override void DrawSelected() {
            using var _ = ImRaii.PushId( Items.IndexOf( Selected ) );
            Selected.Draw();
        }
    }
}

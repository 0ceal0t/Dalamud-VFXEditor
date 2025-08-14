using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using VfxEditor.FileBrowser;
using VfxEditor.TmbFormat;
using VfxEditor.TmbFormat.Actor;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TmbFormat.Track {
    public class TmbTrackSplitView : ItemSplitView<Tmtr> {
        public readonly Tmac Actor;

        public TmbTrackSplitView( Tmac actor ) : base( "Tracks", actor.Tracks ) {
            Actor = actor;
        }

        protected override void DrawControls() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) Actor.AddTrack();

            ImGui.SameLine();
            if( ImGui.Button( FontAwesomeIcon.Upload.ToIconString() ) ) {
                FileBrowserManager.OpenFileDialog( "Select a File", "TMB entry{.tmbtrack},.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    try {
                        Actor.ImportTrack( System.IO.File.ReadAllBytes( res ) );
                    }
                    catch( Exception e ) {
                        Dalamud.Error( e, "Could not import data" );
                    }
                } );
            }

            using var disabled = ImRaii.Disabled( Selected == null );

            ImGui.SameLine();
            if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) ) {
                FileBrowserManager.SaveFileDialog( "Select a Save Location", ".tmbtrack,.*", "ExportedTmbTrack", "tmbtrack", ( bool ok, string res ) => {
                    if( ok ) System.IO.File.WriteAllBytes( res, Selected.ToBytes() );
                } );
            }

            ImGui.SameLine();
            if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                Actor.DeleteTrack( Selected );
                Selected = null;
            }
        }

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

using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using VfxEditor.FileBrowser;
using VfxEditor.Utils;

namespace VfxEditor.Interop.Havok.Ui {
    public class SkeletonSelector {
        public string Path => SklbPreviewPath;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";
        private SklbSource Source = SklbSource.Game;
        private readonly Action<SimpleSklb> OnUpdate;

        private bool Initialized = false;

        public SkeletonSelector( string sklbPath, Action<SimpleSklb> onUpdate ) {
            OnUpdate = onUpdate;
            if( !string.IsNullOrEmpty( sklbPath ) && Dalamud.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;
        }

        public void Init() {
            if( Initialized ) return;
            Plugin.SklbManager.GetSimpleSklb( SklbPreviewPath, out var simple, out var source );
            Source = source;
            OnUpdate.Invoke( simple );
            Initialized = true;
        }

        public void Draw() {
            Init();

            using var _ = ImRaii.PushId( "Selector" );

            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Sync );
            var inputSize = ImGui.GetContentRegionAvail().X - 400;
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##SklbPath", ref SklbPreviewPath, 255 );

            var imguiStyle = ImGui.GetStyle();
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) )
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) {
                    if( Plugin.SklbManager.GetSimpleSklb( SklbPreviewPath, out var simple, out var source ) ) {
                        Source = source;
                        OnUpdate.Invoke( simple );
                    }
                }

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.FileUpload.ToIconString() ) ) {
                    FileBrowserManager.OpenFileDialog( "Select a File", ".sklb,.*", ( ok, res ) => {
                        if( !ok ) return;
                        Source = SklbSource.Local;
                        OnUpdate.Invoke( SimpleSklb.LoadFromLocal( res ) );
                    } );
                }
            }

            ImGui.SameLine();
            switch( Source ) {
                case SklbSource.Penumbra:
                    ImGui.TextColored( UiUtils.GREEN_COLOR, "[Penumbra]" );
                    break;
                case SklbSource.Document:
                    ImGui.TextColored( UiUtils.GREEN_COLOR, "[Replaced]" );
                    break;
                case SklbSource.Local:
                    ImGui.TextDisabled( "[Local]" );
                    break;
                default:
                    ImGui.TextDisabled( "[Game]" );
                    break;
            }
        }
    }
}

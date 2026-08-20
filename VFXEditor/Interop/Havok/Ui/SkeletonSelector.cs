using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using VfxEditor.FileBrowser;
using VfxEditor.Utils;

namespace VfxEditor.Interop.Havok.Ui {
    public class SkeletonSelector {
        public string Path => SklbPreviewPath;

        private const string FallbackSklbPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";

        // The default game path for this file, picked once at construction: either the caller's
        // best guess (if it exists) or the hardcoded fallback. Restored by the reset button.
        private readonly string DefaultSklbPath;
        private readonly Action<SimpleSklb> OnUpdate;

        private string SklbPreviewPath;
        private SklbSource Source = SklbSource.Game;
        private bool Initialized = false;

        public SkeletonSelector( string sklbPath, Action<SimpleSklb> onUpdate ) {
            OnUpdate = onUpdate;
            DefaultSklbPath = !string.IsNullOrEmpty( sklbPath ) && Dalamud.DataManager.FileExists( sklbPath ) ? sklbPath : FallbackSklbPath;
            SklbPreviewPath = DefaultSklbPath;
        }

        public void Init() {
            if( Initialized ) return;
            Resolve();
            Initialized = true;
        }

        private void Resolve() {
            if( Plugin.SklbManager.GetSimpleSklb( SklbPreviewPath, out var simple, out var source ) ) {
                Source = source;
                OnUpdate.Invoke( simple );
            }
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
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) Resolve();
                UiUtils.Tooltip( "Refresh" );

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.FileUpload.ToIconString() ) ) {
                    FileBrowserManager.OpenFileDialog( "Select a File", ".sklb,.*", ( ok, res ) => {
                        if( !ok ) return;
                        SklbPreviewPath = res;
                        Resolve();
                    } );
                }
                UiUtils.Tooltip( "Load a local file" );

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Undo.ToIconString() ) ) {
                    SklbPreviewPath = DefaultSklbPath;
                    Resolve();
                }
                UiUtils.Tooltip( "Reset to the default game skeleton" );
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

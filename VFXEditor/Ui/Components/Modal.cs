using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public abstract class Modal {
        public readonly string Title;
        private readonly bool ShowCancel;
        protected readonly CommandManager Command;

        public Modal( string title, bool showCancel ) {
            Title = title;
            ShowCancel = showCancel;
            Command = CommandManager.Current;
        }

        protected abstract void DrawBody();
        protected abstract void OnOk();
        protected abstract void OnCancel();

        public void Draw() {
            using var command = new CommandRaii( Command );

            var open = true;
            if( ImGui.BeginPopupModal( Title, ref open, ImGuiWindowFlags.AlwaysAutoResize ) ) {
                using var _ = ImRaii.PushId( "Modal" );
                DrawBody();
                ImGui.Separator();

                if( ImGui.Button( "OK", new Vector2( 120, 0 ) ) ) {
                    ImGui.CloseCurrentPopup();
                    Remove();
                    OnOk();
                }

                if( ShowCancel ) {
                    ImGui.SameLine();
                    using var style = ImRaii.PushColor( ImGuiCol.Button, UiUtils.RED_COLOR );
                    if( ImGui.Button( "Cancel", new Vector2( 120, 0 ) ) ) {
                        ImGui.CloseCurrentPopup();
                        Remove();
                        OnCancel();
                    }
                }

                ImGui.EndPopup();
            }

            if( !open ) Remove();
        }

        protected void Remove() => Plugin.Modals.Remove( Title );

        public void Show() => Plugin.AddModal( this );
    }

    public class TextModal : Modal {
        private readonly Action Ok;
        private readonly string Text;

        public TextModal( string title, string text, Action ok ) : base( title, true ) {
            Text = text;
            Ok = ok;
        }

        protected override void DrawBody() {
            ImGui.PushTextWrapPos( 240 );
            ImGui.TextWrapped( Text );
            ImGui.PopTextWrapPos();
        }

        protected override void OnCancel() { }

        protected override void OnOk() => Ok?.Invoke();
    }
}

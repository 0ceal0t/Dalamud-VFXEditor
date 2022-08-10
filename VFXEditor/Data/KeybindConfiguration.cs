using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor {
    [Serializable]
    public class KeybindConfiguration {
        public enum KeybindModifierKeys {
            Alt,
            Ctrl,
            Shift,
            Ctrl_Shift,
            Ctrl_Alt
        }

        public KeybindModifierKeys Modifier = KeybindModifierKeys.Ctrl;
        public VirtualKey Key = VirtualKey.NONCONVERT;

        [NonSerialized]
        private static readonly KeybindModifierKeys[] ValidKeybindModifiers = ( KeybindModifierKeys[] )Enum.GetValues( typeof( KeybindModifierKeys ) );
        [NonSerialized]
        private static readonly Dictionary<VirtualKey, bool> LastState = new();

        public static void UpdateState() {
            LastState.Clear();
            foreach(var key in Plugin.KeyState.GetValidVirtualKeys()) {
                LastState[key] = Plugin.KeyState[key];
            }
        }

        public static bool CheckState(VirtualKey key) {
            if( !Plugin.KeyState.IsVirtualKeyValid( key ) ) return false;
            return ImGui.IsKeyPressed( ImGuiHelpers.VirtualKeyToImGuiKey( key ) ) || CheckVirtualKeyState( key );
        }

        // Check if was pressed, but now isn't
        private static bool CheckVirtualKeyState(VirtualKey key) {
            if( !LastState.TryGetValue( key, out var lastKeyState ) ) return false;
            if( !lastKeyState ) return false;
            if( Plugin.KeyState[key] ) return false;
            return true;
        }

        public static bool CtrlState() => ImGui.GetIO().KeyCtrl || CheckVirtualKeyState( VirtualKey.CONTROL );

        public static bool AltState() => ImGui.GetIO().KeyAlt || CheckVirtualKeyState( VirtualKey.MENU );

        public static bool ShiftState() => ImGui.GetIO().KeyShift || CheckVirtualKeyState( VirtualKey.SHIFT );

        public bool Draw( string label, string id ) {
            var ret = false;

            ImGui.SetNextItemWidth( 100f );
            if( UIHelper.EnumComboBox( $"{id}-Modifier", ValidKeybindModifiers, Modifier, out var newModifier ) ) {
                Modifier = newModifier;
                ret = true;
            }

            var inputPreview = Key == VirtualKey.NONCONVERT ? "[NONE]" : $"{Key.GetFancyName()}";
            ImGui.SetNextItemWidth( 100f );
            ImGui.SameLine();
            ImGui.InputText( $"{id}-Input", ref inputPreview, 255, ImGuiInputTextFlags.ReadOnly );
            if (ImGui.IsItemActive() ) {
                foreach(var key in Plugin.KeyState.GetValidVirtualKeys() ) {
                    if(CheckState(key)) {
                        Key = key;
                        ret = true;
                    }
                }
            }

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( UIHelper.RemoveButton( $"{( char )FontAwesomeIcon.Times}" + id ) ) {
                Modifier = KeybindModifierKeys.Ctrl;
                Key = VirtualKey.NONCONVERT;
                ret = true;
            }
            ImGui.PopFont();


            ImGui.SameLine();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() - 1 );

            if (Key == VirtualKey.NONCONVERT) ImGui.TextDisabled( label );
            else ImGui.Text( label );

            return ret;
        }

        public bool KeyPressed() {
            if( Key == VirtualKey.NONCONVERT ) return false;
            var modifierKeys = Modifier switch {
                KeybindModifierKeys.Alt => AltState(),
                KeybindModifierKeys.Ctrl => CtrlState(),
                KeybindModifierKeys.Shift => ShiftState(),
                KeybindModifierKeys.Ctrl_Shift => CtrlState() && ShiftState(),
                KeybindModifierKeys.Ctrl_Alt =>CtrlState() && AltState(),
                _ => false
            };
            if( !modifierKeys ) return false;
            return CheckState( Key );
        }
    }
}

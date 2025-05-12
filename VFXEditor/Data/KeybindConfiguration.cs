using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor {
    [Serializable]
    public class KeybindConfiguration {
        public enum KeybindModifierKeys {
            Alt,
            Ctrl,
            Shift,
            Ctrl_Shift,
            Ctrl_Alt
        }

        [NonSerialized]
        private static readonly KeybindModifierKeys[] ValidKeybindModifiers = Enum.GetValues<KeybindModifierKeys>();
        [NonSerialized]
        private static readonly Dictionary<VirtualKey, bool> LastState = [];

        public KeybindModifierKeys Modifier = KeybindModifierKeys.Ctrl;
        public VirtualKey Key = VirtualKey.NONCONVERT;

        public bool Draw( string name ) {
            using var _ = ImRaii.PushId( name );

            var ret = false;
            var unassigned = Key == VirtualKey.NONCONVERT;

            ImGui.SetNextItemWidth( 100f );
            if( UiUtils.EnumComboBox( "##Modifier", ValidKeybindModifiers, Modifier, out var newModifier ) ) {
                Modifier = newModifier;
                ret = true;
            }

            var inputPreview = unassigned ? "[NONE]" : $"{Key.GetFancyName()}";
            ImGui.SetNextItemWidth( 100f );
            ImGui.SameLine();
            ImGui.InputText( "##Input", ref inputPreview, 255, ImGuiInputTextFlags.ReadOnly );
            if( ImGui.IsItemActive() ) {
                foreach( var key in Dalamud.KeyState.GetValidVirtualKeys() ) {
                    if( CheckState( key ) ) {
                        Key = key;
                        ret = true;
                    }
                }
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );

            using( var dimmed = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.5f, unassigned ) )
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.RemoveButton( FontAwesomeIcon.Times.ToIconString() ) ) {
                    Modifier = KeybindModifierKeys.Ctrl;
                    Key = VirtualKey.NONCONVERT;
                    ret = true;
                }
            }


            ImGui.SameLine();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() - 1 );

            if( unassigned ) ImGui.TextDisabled( name );
            else ImGui.Text( name );

            return ret;
        }

        public bool KeyPressed() {
            if( Key == VirtualKey.NONCONVERT ) return false;
            var modifierKeys = Modifier switch {
                KeybindModifierKeys.Alt => AltState(),
                KeybindModifierKeys.Ctrl => CtrlState(),
                KeybindModifierKeys.Shift => ShiftState(),
                KeybindModifierKeys.Ctrl_Shift => CtrlState() && ShiftState(),
                KeybindModifierKeys.Ctrl_Alt => CtrlState() && AltState(),
                _ => false
            };
            if( !modifierKeys ) return false;
            return CheckState( Key );
        }

        // ====================

        public static void UpdateState() {
            LastState.Clear();
            foreach( var key in Dalamud.KeyState.GetValidVirtualKeys() ) {
                LastState[key] = Dalamud.KeyState[key];
            }
        }

        public static bool CheckState( VirtualKey key ) {
            if( !Dalamud.KeyState.IsVirtualKeyValid( key ) ) return false;
            return ImGui.IsKeyPressed( ImGuiHelpers.VirtualKeyToImGuiKey( key ) ) || CheckVirtualKeyState( key );
        }

        // Check if was pressed, but now isn't
        private static bool CheckVirtualKeyState( VirtualKey key ) {
            if( !LastState.TryGetValue( key, out var lastKeyState ) ) return false;
            if( !lastKeyState ) return false;
            if( Dalamud.KeyState[key] ) return false;
            return true;
        }

        public static bool CtrlState() => ImGui.GetIO().KeyCtrl || CheckVirtualKeyState( VirtualKey.CONTROL );

        public static bool AltState() => ImGui.GetIO().KeyAlt || CheckVirtualKeyState( VirtualKey.MENU );

        public static bool ShiftState() => ImGui.GetIO().KeyShift || CheckVirtualKeyState( VirtualKey.SHIFT );

        public static bool UpState() => CheckState( VirtualKey.UP );

        public static bool DownState() => CheckState( VirtualKey.DOWN );

        public static bool NavigateUpDown<T>( List<T> items, T selected, out T newSelected ) where T : class {
            newSelected = selected;
            if( !ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) ) return false;
            if( items == null ) return false;
            if( items.Count == 0 ) return false;
            var up = UpState();
            var down = DownState();

            if( selected == null ) { // if nothing selected, up/down select the first item
                if( up || down ) {
                    newSelected = items[0];
                    return true;
                }
                return false;
            }

            var idx = items.IndexOf( selected );
            if( idx == -1 ) return false;

            if( up ) {
                if( idx == 0 ) return false; // alread the first item
                newSelected = items[idx - 1];
                return true;
            }
            else if( down ) {
                if( idx == ( items.Count - 1 ) ) return false; // already at the end
                newSelected = items[idx + 1];
                return true;
            }

            return false;
        }

    }
}

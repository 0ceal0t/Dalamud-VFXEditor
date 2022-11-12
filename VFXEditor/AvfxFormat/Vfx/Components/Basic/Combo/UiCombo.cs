using ImGuiNET;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCombo<T> : IUiBase where T : Enum {
        public readonly string Name;
        public readonly AVFXEnum<T> Literal;
        public readonly Func<ICommand> ExtraCommand;

        public UiCombo( string name, AVFXEnum<T> literal, Func<ICommand> extraCommand = null ) {
            Name = name;
            Literal = literal;
            ExtraCommand = extraCommand;
        }

        public void DrawInline( string id ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[Name] = Literal.IsAssigned();
                CopyManager.Strings[Name] = Literal.GetValue().ToString();
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue( Name, out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal, a ) );
                if( CopyManager.Strings.TryGetValue( Name, out var l ) ) CopyManager.PasteCommand.Add( new UiComboCommand<T>( Literal, (T) Enum.Parse(typeof(T), l), ExtraCommand?.Invoke() ) );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var text = Literal.Options.Contains( Literal.GetValue() ) ? Literal.GetValue().ToString() : "[NONE]";
            if( UiUtils.EnumComboBox( $"{Name}{id}", text, Literal.Options, Literal.GetValue(), out var newValue ) ) {
                CommandManager.Avfx.Add( new UiComboCommand<T>( Literal, newValue, ExtraCommand?.Invoke() ) );
            }

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}

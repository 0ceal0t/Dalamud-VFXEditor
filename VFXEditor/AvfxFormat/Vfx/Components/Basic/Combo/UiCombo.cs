using ImGuiNET;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCombo<T> : IUiBase {
        public readonly string Name;
        public readonly AVFXEnum<T> Literal;
        public readonly Func<ICommand> ExtraCommand;

        public UiCombo( string name, AVFXEnum<T> literal, Func<ICommand> extraCommand = null ) {
            Name = name;
            Literal = literal;
            ExtraCommand = extraCommand;
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXEnum<T> literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                ExtraCommand?.Invoke()?.Execute();
            }

            // Unassigned
            if( IUiBase.DrawCommandButton( Literal, Name, id ) ) return;

            var text = Literal.Options.Contains( Literal.GetValue() ) ? Literal.GetValue().ToString() : "[NONE]";
            if( UiUtils.EnumComboBox( $"{Name}{id}", text, Literal.Options, Literal.GetValue(), out var newValue ) ) {
                CommandManager.Avfx.Add( new UiComboCommand<T>( Literal, newValue, ExtraCommand?.Invoke() ) );
            }

            IUiBase.DrawCommandContextMenu( Literal, Name, id );
        }
    }
}

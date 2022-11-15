using System;
using System.IO;
using System.Linq;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxEnum<T> : AvfxDrawable where T : Enum {
        private T Value = ( T )( object )0;

        public readonly string Name;
        public readonly T[] Options = ( T[] )Enum.GetValues( typeof( T ) );
        public Func<ICommand> ExtraCommandGenerator; // can be changed later

        public AvfxEnum( string name, string avfxName, Func<ICommand> extraCommandGenerator = null ) : base( avfxName ) {
            Name = name;
            ExtraCommandGenerator = extraCommandGenerator;
        }

        public T GetValue() => Value;

        public void SetValue( T value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int _ ) {
            var intValue = reader.ReadInt32();
            Value = ( T )( object )intValue;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            writer.Write( Value == null ? -1 : ( int )( object )Value );
        }

        public override void Draw( string id ) {
            // Unassigned
            AssignedCopyPaste( this, Name );
            if( DrawAddButton( this, Name, id ) ) return;

            // Copy/Paste
            var manager = CopyManager.Avfx;
            if( manager.IsCopying ) manager.Ints[Name] = ( int )( object )Value;
            if( manager.IsPasting && manager.Ints.TryGetValue( Name, out var val ) ) {
                manager.PasteCommand.Add( new AvfxEnumCommand<T>( this, ( T )( object )val, ExtraCommandGenerator?.Invoke() ) );
            }

            var text = Options.Contains( Value ) ? Value.ToString() : "[NONE]";
            if( UiUtils.EnumComboBox( $"{Name}{id}", text, Options, Value, out var newValue ) ) {
                CommandManager.Avfx.Add( new AvfxEnumCommand<T>( this, newValue, ExtraCommandGenerator?.Invoke() ) );
            }

            DrawRemoveContextMenu( this, Name, id );
        }
    }
}

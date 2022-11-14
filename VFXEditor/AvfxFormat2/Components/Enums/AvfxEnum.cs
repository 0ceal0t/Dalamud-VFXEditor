using System;
using System.IO;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEnum<T> : AvfxDrawable {
        private T Value = ( T )( object )0;

        public readonly string Name;
        public readonly T[] Options = ( T[] )Enum.GetValues( typeof( T ) );
        public Func<ICommand> ExtraCommand; // can be changed later

        public AvfxEnum( string name, string avfxName, Func<ICommand> extraCommand = null ) : base( avfxName ) {
            Name = name;
            ExtraCommand = extraCommand;
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

            var text = Options.Contains( Value ) ? Value.ToString() : "[NONE]";
            if( UiUtils.EnumComboBox( $"{Name}{id}", text, Options, Value, out var newValue ) ) {
                CommandManager.Avfx.Add( new AvfxEnumCommand<T>( this, newValue, ExtraCommand?.Invoke() ) );
            }

            DrawRemoveContextMenu( this, Name, id );
        }
    }
}

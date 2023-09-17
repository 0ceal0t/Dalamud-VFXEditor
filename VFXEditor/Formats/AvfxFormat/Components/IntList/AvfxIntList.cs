using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat {
    public class AvfxIntList : AvfxDrawable {
        public readonly string Name;
        private int Size;
        private readonly List<int> Items = new() { 0 };

        public AvfxIntList( string name, string avfxName, int value, int size = 1 ) : this( name, avfxName, size ) {
            SetItems( value );
        }

        public AvfxIntList( string name, string avfxName, int size = 1 ) : base( avfxName ) {
            Name = name;
            Size = size;
        }

        public List<int> GetItems() => Items;

        public void SetItems( List<int> value ) {
            SetAssigned( true );
            Items.Clear();
            Items.AddRange( value );
            Size = Items.Count;
        }

        public void SetItems( int value ) => SetItems( new List<int> { value } );

        public void SetItem( int value, int idx ) {
            SetAssigned( true );
            Items[idx] = value;
        }

        public void AddItem( int item ) {
            SetAssigned( true );
            Size++;
            Items.Add( item );
        }

        public void RemoveItem( int idx ) {
            SetAssigned( true );
            Size--;
            Items.Remove( idx );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Size = size;
            Items.Clear();
            for( var i = 0; i < Size; i++ ) Items.Add( reader.ReadByte() );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) writer.Write( ( byte )item );
        }

        public override void Draw() {
            // Unassigned
            AssignedCopyPaste( this, Name );
            if( DrawAddButton( this, Name ) ) return;

            // Copy/Paste
            var manager = CopyManager.Avfx;
            if( manager.IsCopying ) manager.Ints[Name] = Items[0];
            if( manager.IsPasting && manager.Ints.TryGetValue( Name, out var val ) ) manager.PasteCommand.Add( new AvfxIntListCommand( this, val ) );

            var value = Items[0];
            if( ImGui.InputInt( Name, ref value ) ) {
                CommandManager.Avfx.Add( new AvfxIntListCommand( this, value ) );
            }

            DrawRemoveContextMenu( this, Name );
        }
    }
}

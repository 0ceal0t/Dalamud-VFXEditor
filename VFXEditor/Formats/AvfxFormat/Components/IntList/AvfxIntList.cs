using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Copy;

namespace VfxEditor.AvfxFormat {
    public class AvfxIntList : AvfxDrawable {
        public readonly string Name;
        private int Size;
        private readonly List<int> Items = [0];

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

        public void SetItems( int value ) => SetItems( [value] );

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

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) writer.Write( ( byte )item );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }

        public override void Draw() {
            // Unassigned
            AssignedCopyPaste( Name );
            if( DrawAssignButton( Name ) ) return;

            // Copy/Paste
            CopyManager.TrySetValue( this, Name, Items[0] );
            if( CopyManager.TryGetValue<int>( this, Name, out var val ) ) {
                CommandManager.Paste( new AvfxIntListCommand( this, val ) );
            }

            var value = Items[0];
            if( ImGui.InputInt( Name, ref value ) ) {
                CommandManager.Add( new AvfxIntListCommand( this, value ) );
            }

            DrawUnassignPopup( Name );
        }
    }
}

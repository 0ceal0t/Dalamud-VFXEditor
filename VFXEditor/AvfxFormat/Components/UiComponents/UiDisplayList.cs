using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    // Dummy class

    public class UiDisplayList : AvfxItem {
        public readonly string Name;
        private readonly List<IAvfxUiBase> Display;

        public UiDisplayList( string name ) : base( "" ) {
            Name = name;
            Display = new List<IAvfxUiBase>();
            SetAssigned( true );
        }

        public void Add( IAvfxUiBase item ) => Display.Add( item );

        public void Remove( IAvfxUiBase item ) => Display.Remove( item );

        public void Prepend( IAvfxUiBase item ) => Display.Insert( 0, item );

        public override void Draw( string id ) => IAvfxUiBase.DrawList( Display, id );

        public override string GetDefaultText() => Name;

        protected override void RecurseChildrenAssigned( bool assigned ) {
            throw new NotImplementedException();
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            throw new NotImplementedException();
        }

        protected override void WriteContents( BinaryWriter writer ) {
            throw new NotImplementedException();
        }
    }
}

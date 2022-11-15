using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat2{
    // Dummy class

    public class UiDisplayList : AvfxItem {
        public readonly string Name;
        private readonly List<IUiBase> Display;

        public UiDisplayList( string name ) : base( "" ) {
            Name = name;
            Display = new List<IUiBase>();
            SetAssigned( true );
        }

        public void Add( IUiBase item ) => Display.Add( item );

        public void Remove( IUiBase item ) => Display.Remove( item );

        public void Prepend( IUiBase item ) => Display.Insert( 0, item );

        public override void Draw( string id ) => IUiBase.DrawList( Display, id );

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

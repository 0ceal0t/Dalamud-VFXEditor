using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiDisplayList : AvfxItem {
        public readonly string Name;
        private readonly List<IUiItem> Display;

        public UiDisplayList( string name ) : base( "" ) {
            Name = name;
            Display = new List<IUiItem>();
            SetAssigned( true );
        }

        public void Add( IUiItem item ) => Display.Add( item );

        public void Remove( IUiItem item ) => Display.Remove( item );

        public void Prepend( IUiItem item ) => Display.Insert( 0, item );

        public override void Draw( string id ) => AvfxBase.DrawItems( Display, id );

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

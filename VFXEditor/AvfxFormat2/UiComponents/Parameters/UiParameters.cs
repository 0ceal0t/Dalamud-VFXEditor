using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2{
    // Dummy class

    public class UiParameters : AvfxItem {
        public readonly string Name;
        private readonly List<IUiBase> Parameters;

        public UiParameters( string name ) : base( "" ) {
            Name = name;
            Parameters = new List<IUiBase>();
            SetAssigned( true );
        }

        public void Add( IUiBase item ) => Parameters.Add( item );

        public void Remove( IUiBase item ) => Parameters.Remove( item );

        public void Prepend( IUiBase item ) => Parameters.Insert( 0, item );

        public override void Draw( string id ) => IUiBase.DrawList( Parameters, id );

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

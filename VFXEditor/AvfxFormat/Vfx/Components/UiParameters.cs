using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParameters : UiItem {
        public string Name;
        private readonly List<IUiBase> Parameters;

        public UiParameters( string name ) {
            Name = name;
            Parameters = new List<IUiBase>();
        }

        public void Add( IUiBase item ) {
            Parameters.Add( item );
        }

        public void Remove( IUiBase item ) {
            Parameters.Remove( item );
        }

        public void Prepend( IUiBase item ) {
            Parameters.Insert( 0, item );
        }

        public override void DrawInline( string id ) {
            IUiBase.DrawList( Parameters, id );
        }

        public override string GetDefaultText() => Name;
    }
}

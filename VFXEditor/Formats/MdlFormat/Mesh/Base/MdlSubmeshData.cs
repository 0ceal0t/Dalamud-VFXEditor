using SharpDX.Direct3D11;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh.Base {
    public abstract class MdlSubmeshData<T> : MdlMeshDrawable, IUiItem where T : MdlMeshData {
        private T Parent;

        public MdlSubmeshData( MdlFile file, T parent ) : base( file ) {
            Parent = parent;
        }

        public void Populate( T parent, BinaryReader reader, uint indexBufferPos ) {
            Parent = parent;
            reader.BaseStream.Position = indexBufferPos + _IndexOffset;
            RawIndexData = reader.ReadBytes( ( int )( IndexCount * 2 ) );
        }

        public override void RefreshBuffer( Device device ) {
            if( Parent == null ) return;

            Data?.Dispose();
            var data = Parent.GetData( ( int )IndexCount, RawIndexData );
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        protected override void DrawPreview() {
            if( Parent == null ) return;
            base.DrawPreview();
        }

        public abstract void Draw();
    }
}

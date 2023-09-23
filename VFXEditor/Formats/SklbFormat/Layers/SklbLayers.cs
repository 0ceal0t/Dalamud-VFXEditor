using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.SklbFormat.Layers {
    public class SklbLayers {
        public bool IsSklb => File != null;
        public readonly SklbFile File;

        private readonly List<SklbLayer> Layers = new();
        private readonly CommandSplitView<SklbLayer> LayerView;

        public SklbLayers( SklbFile file ) {
            File = file;
            LayerView = new( "Layer", Layers, true, null, () => new SklbLayer( file ), () => IsSklb ? CommandManager.Sklb : CommandManager.Skp );
        }

        public SklbLayers( SklbFile file, BinaryReader reader ) : this( file ) {
            Read( reader );
        }

        public void Read( BinaryReader reader ) {
            reader.ReadInt32(); // Magic

            var numLayers = reader.ReadInt16();
            for( var i = 0; i < numLayers; i++ ) {
                reader.ReadInt16(); // offset
            }

            for( var i = 0; i < numLayers; i++ ) {
                Layers.Add( new( File, reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( 0x616C7068 );
            writer.Write( ( short )Layers.Count );

            var offset = 6 + 2 * Layers.Count; // magic + numLayers + offsets
            foreach( var layer in Layers ) {
                writer.Write( ( short )offset );
                offset += layer.Size;
            }

            Layers.ForEach( x => x.Write( writer ) );
        }

        public void Draw() => LayerView.Draw();
    }
}

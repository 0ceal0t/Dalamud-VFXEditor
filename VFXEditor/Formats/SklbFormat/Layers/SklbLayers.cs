using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.SklbFormat.Layers {
    public class SklbLayers {
        public readonly List<SklbLayer> Layers = new();
        public readonly CommandSplitView<SklbLayer> LayerView;
        public readonly SklbFile File;

        public SklbLayers( SklbFile file, BinaryReader reader ) {
            File = file;

            reader.ReadInt32(); // Magic

            var numLayers = reader.ReadInt16();
            for( var i = 0; i < numLayers; i++ ) {
                reader.ReadInt16(); // offset
            }

            for( var i = 0; i < numLayers; i++ ) {
                Layers.Add( new( file, reader ) );
            }

            LayerView = new( "Layer", Layers, true, null, () => new SklbLayer( file ), () => CommandManager.Sklb );
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

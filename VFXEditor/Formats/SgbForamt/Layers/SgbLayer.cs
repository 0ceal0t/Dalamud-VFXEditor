using System.IO;

namespace VfxEditor.Formats.SgbForamt.Layers {
    public class SgbLayer {
        public SgbLayer() { }

        public SgbLayer( BinaryReader reader ) : this() {
            // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Parsing/Layer/LayerCommon.cs#L1596
        }
    }
}

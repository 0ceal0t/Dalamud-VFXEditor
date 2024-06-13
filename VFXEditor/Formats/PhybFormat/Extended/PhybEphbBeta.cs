using System.Collections.Generic;
using System.Linq;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybEphbBeta : IUiItem {
        public readonly List<PhybEphbGamma> Gamma = [];
        private readonly CommandSplitView<PhybEphbGamma> GammaView;

        public PhybEphbBeta() {
            GammaView = new( "Gamma", Gamma, false, null, () => new() );
        }

        public PhybEphbBeta( EphbBetaT beta ) : this() {
            Gamma.AddRange( beta.Gamma.Select( x => new PhybEphbGamma( x ) ) );
        }

        public void Draw() => GammaView.Draw();

        public EphbBetaT Export() => new() {
            Gamma = Gamma.Select( x => x.Export() ).ToList(),
        };
    }
}

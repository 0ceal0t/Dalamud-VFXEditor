using System.Collections.Generic;
using System.Linq;
using VfxEditor.Flatbuffer;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbBeta : IUiItem {
        public readonly List<PhybEphbGamma> Gamma = [];
        private readonly CommandSplitView<PhybEphbGamma> GammaView;

        public PhybEphbBeta() {
            GammaView = new( "Gamma", Gamma, false, null, () => new() );
        }

        public PhybEphbBeta( EphbBeta beta ) : this() {
            Gamma.AddRange( beta.Gamma.Select( x => new PhybEphbGamma( x ) ) );
        }

        public void Draw() => GammaView.Draw();

        public EphbBeta Export() => new() {
            Gamma = [.. Gamma.Select( x => x.Export() )],
        };
    }
}

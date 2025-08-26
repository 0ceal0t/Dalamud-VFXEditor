using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Parsing;
using VfxEditor.Ui.Assignable;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbAlpha : IUiItem {
        public readonly ParsedBool Unknown1 = new( "Unknown 1" );
        public readonly AssignableData<PhybEphbZeta> Zeta = new( "Zeta" );

        public readonly List<PhybEphbBeta> Beta = [];
        private readonly CommandSplitView<PhybEphbBeta> BetaView;

        public readonly List<PhybEphbEta> Eta = [];
        private readonly CommandSplitView<PhybEphbEta> EtaView;

        public readonly List<PhybEphbEpsilon> Epsilon = [];
        private readonly CommandSplitView<PhybEphbEpsilon> EpsilonView;

        public PhybEphbAlpha() {
            BetaView = new( "Beta", Beta, false, null, () => new() );
            EtaView = new( "Eta", Eta, false, null, () => new() );
            EpsilonView = new( "Epsilon", Epsilon, false, null, () => new() );
        }

        public PhybEphbAlpha( EphbAlphaT alpha ) : this() {
            Unknown1.Value = alpha.Unknown1;
            Zeta.SetValue( alpha.Zeta == null ? null : new( alpha.Zeta ) );
            Beta.AddRange( alpha.Beta.Select( x => new PhybEphbBeta( x ) ) );
            Eta.AddRange( alpha.Eta.Select( x => new PhybEphbEta( x ) ) );
            Epsilon.AddRange( alpha.Epsilon.Select( x => new PhybEphbEpsilon( x ) ) );
        }

        public void Draw() {
            Unknown1.Draw();
            Zeta.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Beta" ) ) {
                if( tab ) BetaView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Eta" ) ) {
                if( tab ) EtaView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Epsilon" ) ) {
                if( tab ) EpsilonView.Draw();
            }
        }

        public EphbAlphaT Export() => new() {
            Unknown1 = Unknown1.Value,
            Zeta = Zeta.GetValue()?.Export(),
            Beta = Beta.Select( x => x.Export() ).ToList(),
            Eta = Eta.Select( x => x.Export() ).ToList(),
            Epsilon = Epsilon.Select( x => x.Export() ).ToList()
        };
    }
}

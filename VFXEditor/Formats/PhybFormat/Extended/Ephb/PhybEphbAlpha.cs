using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Parsing;
using VfxEditor.Ui.Assignable;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Flatbuffer;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbAlpha : IUiItem {
        public readonly List<PhybEphbBeta> Beta = [];
        private readonly CommandSplitView<PhybEphbBeta> BetaView;

        public readonly ParsedString Unknown1 = new( "Unknown 1" );
        public readonly ParsedString Unknown2 = new( "Unknown 2" );
        public readonly ParsedUInt Unknown3 = new( "Unknown 3" );
        public readonly EphbUnknown Unknown4 = default;
        public readonly AssignableData<PhybEphbZeta> Zeta = new( "Zeta" );

        public PhybEphbAlpha() {
            BetaView = new( "Beta", Beta, false, null, () => new() );
        }

        public PhybEphbAlpha( EphbAlpha alpha ) : this() {
            Beta.AddRange( alpha.Beta.Select( x => new PhybEphbBeta( x ) ) );
            Unknown1.Value = alpha.A;
            Unknown2.Value = alpha.B;
            Unknown3.Value = alpha.C;
            Unknown4 = alpha.D;
            Zeta.SetValue( alpha.E == null ? null : new( alpha.E ) );
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
            Zeta.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using var tab = ImRaii.TabItem( "Beta" );
            if( tab ) BetaView.Draw();
        }

        public EphbAlpha Export() => new() {
            Beta = [.. Beta.Select( x => x.Export() )],
            A = Unknown1.Value,
            B = Unknown2.Value,
            C = Unknown3.Value,
            D = Unknown4,
            E = Zeta.GetValue()?.Export()
        };
    }
}

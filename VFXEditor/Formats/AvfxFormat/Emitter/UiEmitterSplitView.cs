using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.AvfxFormat {
    public class UiEmitterSplitView : AvfxItemSplitView<AvfxEmitterItem> {
        public readonly AvfxEmitter Emitter;
        public readonly bool IsParticle;

        public UiEmitterSplitView( string id, List<AvfxEmitterItem> items, AvfxEmitter emitter, bool isParticle ) : base( id, items ) {
            Emitter = emitter;
            IsParticle = isParticle;
        }

        public override void Disable( AvfxEmitterItem item ) {
            if( IsParticle ) item.ParticleSelect.Disable();
            else item.EmitterSelect.Disable();
        }

        public override void Enable( AvfxEmitterItem item ) {
            if( IsParticle ) item.ParticleSelect.Enable();
            else item.EmitterSelect.Enable();
        }

        protected override bool DrawLeftItem( AvfxEmitterItem item, int idx ) {
            using var _ = ImRaii.PushId( idx );

            using( var style = ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(0f, 0f))) {
                var enabled = item.CreateProbability.Value > 0;
                if( ImGui.Checkbox( "##Enable", ref enabled ) ) {
                    item.CreateProbability.Value = enabled ? 100 : 0;
                }
                ImGui.SameLine();
            }

            if( ImGui.Selectable( item.GetText(), Selected == item ) ) {
                OnSelect( item );
                Selected = item;
            }
            return false;
        }

        public override AvfxEmitterItem CreateNewAvfx() => new( IsParticle, Emitter, true );
    }
}

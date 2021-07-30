using Dalamud.Game.ClientState.Actors.Types;

namespace VFXEditor.Structs.Vfx {
    public unsafe class ActorVfx : BaseVfx {
        public ActorVfx( Plugin plugin, Actor caster, Actor target, string path ) : base( plugin, path ) {
            Vfx = Plugin.ResourceLoader.ActorVfxCreate( path, caster.Address, target.Address, -1, ( char ) 0, 0, ( char ) 0 );
        }

        public override void Remove() {
            Plugin.ResourceLoader.ActorVfxRemove( Vfx, (char) 1 );
        }
    }
}
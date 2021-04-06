using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;
using ImGuizmoNET;

namespace VFXEditor.Structs.Vfx {
    public class ActorVfx : BaseVfx {
        public ActorVfx( Plugin plugin, Actor caster, Actor target, string path ) : base( plugin, path ) {
            Vfx = _Plugin.ResourceLoader.StatusAdd( path, caster.Address, target.Address, -1, ( char ) 0, 0, ( char ) 0 );
        }

        public override void Remove() {
             _Plugin.ResourceLoader.StatusRemove( Vfx, (char) 1 );
        }
    }
}
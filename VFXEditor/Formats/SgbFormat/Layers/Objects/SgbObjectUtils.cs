using System;
using System.Collections.Generic;
using VfxEditor.Formats.SgbFormat.Layers.Objects.Data;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects {
    public class SgbObjectUtils {
        public static readonly SortedDictionary<LayerEntryType, Type> ObjectTypes = new() {
            { LayerEntryType.Background, typeof(BgInstanceObject ) },
            { LayerEntryType.LayLight, typeof(LightInstanceObject ) },
            { LayerEntryType.Vfx, typeof(VfxInstanceObject ) },
            { LayerEntryType.PositionMarker, typeof(PositionMarkerInstanceObject ) },
            { LayerEntryType.SharedGroup, typeof(SharedGroupInstanceObject ) },
            { LayerEntryType.Sound, typeof(SoundInstanceObject ) },
            { LayerEntryType.EventNPC, typeof(ENPCInstanceObject ) },
            { LayerEntryType.BattleNPC, typeof(BNPCInstanceObject ) },
            { LayerEntryType.Aetheryte, typeof(AetheryteInstanceObject ) },
            { LayerEntryType.EnvSet, typeof(EnvSetInstanceObject ) },
            { LayerEntryType.Gathering, typeof(GatheringInstanceObject ) },
            { LayerEntryType.Treasure, typeof(TreasureInstanceObject ) },
            { LayerEntryType.PopRange, typeof(PopRangeInstanceObject ) },
            { LayerEntryType.ExitRange, typeof(ExitRangeInstanceObject ) },
            { LayerEntryType.MapRange, typeof(MapRangeInstanceObject ) },
            { LayerEntryType.EventObject, typeof(EventInstanceObject ) },
            { LayerEntryType.EnvLocation, typeof(EnvLocationInstanceObject ) },
            { LayerEntryType.EventRange, typeof(EventRangeInstanceObject ) },
            { LayerEntryType.QuestMarker, typeof(QuestMarkerInstanceObject ) },
            { LayerEntryType.CollisionBox, typeof(CollisionBoxInstanceObject ) },
            { LayerEntryType.LineVfx, typeof(LineVfxInstanceObject ) },
            { LayerEntryType.ClientPath, typeof(ClientPathInstanceObject ) },
            { LayerEntryType.ServerPath, typeof(ServerPathInstanceObject ) },
            { LayerEntryType.GimmickRange, typeof(GimmickRangeInstanceObject ) },
            { LayerEntryType.TargetMarker, typeof(TargetMarkerInstanceObject ) },
            { LayerEntryType.ChairMarker, typeof(ChairMarkerInstanceObject ) },
            { LayerEntryType.PrefetchRange, typeof(PrefetchRangeInstanceObject ) },
            { LayerEntryType.FateRange, typeof(FateRangeInstanceObject ) }
        };
    }
}

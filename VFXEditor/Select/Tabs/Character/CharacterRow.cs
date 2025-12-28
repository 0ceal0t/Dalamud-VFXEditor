using System.Collections.Generic;
using VfxEditor.Select.Base;
using VfxEditor.Select.Data;

namespace VfxEditor.Select.Tabs.Character {
    public enum CharacterPart {
        Body,
        Ear,
        Tail,
        Hair,
        Face
    }

    public struct CharacterPartParams {
        public string Id;
        public string MdlSuffix;
        public bool MtrlVariant;

        public CharacterPartParams( string id, string suffix, bool variant ) {
            Id = id;
            MdlSuffix = suffix;
            MtrlVariant = variant;
        }
    }

    public class CharacterRow : ISelectItem {
        public string Name => Data.Name;
        public readonly RacialData Data;

        public string SkeletonId => Data.Id;
        public string AtchPath => $"chara/xls/attachoffset/{SkeletonId}.atch";
        public string PapPrefix => $"chara/human/{SkeletonId}/animation/a0001/bt_common";

        private static readonly Dictionary<CharacterPart, CharacterPartParams> PartParams = new() {
            { CharacterPart.Body, new( "body", "top", true ) },
            { CharacterPart.Ear, new( "zear", "zer", false ) },
            { CharacterPart.Tail, new( "tail", "til", true ) },
            { CharacterPart.Hair, new( "hair", "hir", true ) },
            { CharacterPart.Face, new( "face", "fac", false ) },
        };

        public CharacterRow( RacialData data ) {
            Data = data;
        }

        public string GetName() => Name;

        public string GetLoopPap( int id, string prefix ) => $"{PapPrefix}/emote/{prefix}pose{id:D2}_loop.pap";

        public string GetStartPap( int id, string prefix ) => $"{PapPrefix}/emote/{prefix}pose{id:D2}_start.pap";

        public string GetOrnamentLoopPap( int id, string prefix ) => $"{PapPrefix}/ornament_sp/m6001/{prefix}pose{id:D2}_loop.pap";

        public string GetOrnamentStartPap( int id, string prefix ) => $"{PapPrefix}/ornament_sp/m6001/{prefix}pose{id:D2}_start.pap";

        public string GetPap( string path ) => $"{PapPrefix}/{path}.pap";

        public string GetMtrl( CharacterPart part, int id, string suffix ) {
            var p = PartParams[part];
            var pId = p.Id;
            return $"chara/human/{SkeletonId}/obj/{pId}/{pId[0]}{id:D4}/material/" +
                ( p.MtrlVariant ? "v0001/" : "" ) +
                $"mt_{SkeletonId}{pId[0]}{id:D4}_{suffix}.mtrl";
        }

        public string GetMdl( CharacterPart part, int id ) {
            var p = PartParams[part];
            var pId = p.Id;
            return $"chara/human/{SkeletonId}/obj/{pId}/{pId[0]}{id:D4}/model/{SkeletonId}{pId[0]}{id:D4}_{p.MdlSuffix}.mdl";
        }
    }
}
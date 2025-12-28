using Dalamud.Game.ClientState.Objects.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Excel;

namespace VfxEditor.Select.Data {
    public class RacialData {
        public readonly string Name;
        public readonly string Id;
        public readonly SortedSet<int> FaceOptions = [];
        public readonly SortedSet<int> HairOptions = []; // Id, icon
        public readonly SortedSet<int> TailOptions = [];
        public readonly SortedSet<int> EarOptions = [];
        public readonly SortedSet<int> BodyOptions = [];

        public readonly Dictionary<int, uint> HairToIcon = [];
        public readonly Dictionary<int, uint> FaceToIcon = [];
        public readonly Dictionary<int, uint> FeatureToIcon = [];

        public RacialData( string name, string id, uint row ) {
            Name = name;
            Id = id;

            var data = CharaMakeType.BuildMenus( row );
            var charaRow = Dalamud.DataManager.GetExcelSheet<CharaMakeType>( name: "CharaMakeType" ).GetRow( row );

            var faceTypes = data.GetMenuForCustomize( CustomizeIndex.FaceType );
            if(faceTypes != null ) {
                foreach( var (param, idx) in faceTypes.SubParams.WithIndex() ) FaceToIcon[idx + 1] = ( uint )param ;
            }

            var featureTypes = data.GetMenuForCustomize( CustomizeIndex.RaceFeatureType );
            if( featureTypes != null ) {
                foreach( var (param, idx) in featureTypes.SubParams.WithIndex() ) FeatureToIcon[idx + 1] = ( uint )param;
            }

            var hairStyles = HairMakeType.GetHairStyles( ( uint )charaRow.Gender , charaRow.Race.RowId, charaRow.Tribe.RowId );
            if( hairStyles != null ) {
                foreach( var hair in  hairStyles ) HairToIcon[hair.FeatureID] = hair.Icon;
            }

            foreach( var line in File.ReadAllLines( SelectDataUtils.CommonRacialPath ) ) {
                var split = line.Split( "/" );
                if( !Id.Equals( split[2] ) ) continue; // c0804

                ( split[4] switch {
                    "face" => FaceOptions,
                    "zear" => EarOptions,
                    "tail" => TailOptions,
                    "body" => BodyOptions,
                    "hair" => HairOptions,
                    _ => null
                } )?.Add( Convert.ToInt32( split[5][1..] ) ); // f0101 -> 0101
            }
        }
    }
}

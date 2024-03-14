using Dalamud.Game.ClientState.Objects.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Excel;

namespace VfxEditor.Select.Data {
    public class RacialData {
        public readonly string Name;
        public readonly string Id;
        public readonly SortedSet<int> FaceOptions = new();
        public readonly SortedSet<int> HairOptions = new(); // Id, icon
        public readonly SortedSet<int> TailOptions = new();
        public readonly SortedSet<int> EarOptions = new();
        public readonly SortedSet<int> BodyOptions = new();

        public readonly Dictionary<int, uint> HairToIcon = new();
        public readonly Dictionary<int, uint> FaceToIcon = new();
        public readonly Dictionary<int, uint> FeatureToIcon = new();

        public RacialData( string name, string id, uint row ) {
            Name = name;
            Id = id;

            var data = Dalamud.DataManager.GetExcelSheet<CharaMakeType>().GetRow( row );

            foreach( var hair in data.FeatureMake.Value.HairStyles ) HairToIcon[hair.Value.FeatureId] = hair.Value.Icon;

            if( data.Menus.FindFirst( x => x.Index == CustomizeIndex.FaceType, out var faceMenu ) ) {
                foreach( var (param, idx) in faceMenu.Params.WithIndex() ) FaceToIcon[idx + 1] = param;
            }

            if( data.Menus.FindFirst( x => x.Index == CustomizeIndex.RaceFeatureType, out var featureMenu ) ) {
                foreach( var (param, idx) in featureMenu.Params.WithIndex() ) FeatureToIcon[idx + 1] = param;
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

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

        public RacialData( string name, string id, uint hairOffset ) {
            Name = name;
            Id = id;

            for( var i = hairOffset; i < hairOffset + 100; i++ ) {
                var row = Dalamud.DataManager.GetExcelSheet<CharaMakeCustomize>().GetRow( i );
                HairToIcon[row.FeatureId] = row.Icon;
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

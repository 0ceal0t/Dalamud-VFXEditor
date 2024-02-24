using System.Collections.Generic;

namespace VfxEditor.TmbFormat.Root {
    public class LuaPool {
        public readonly int Id;
        public readonly int Size;
        public readonly Dictionary<int, string> Names;

        public LuaPool( int id, int maxSize, Dictionary<int, string> names ) {
            Id = id;
            Size = maxSize;
            Names = names;
        }

        public static readonly LuaPool Pool1 = new( 1, 64, new() {
            { 0x00, "[CUTSCENE] Game Language" },
            { 0x01, "[CUTSCENE] Caption Language" },
            { 0x02, "[CUTSCENE] Voice Language" },
            { 0x03, "[CUTSCENE] Skeleton Id" },
            { 0x04, "[CUTSCENE] Starting Town" },
            { 0x10, "[CUTSCENE] Legacy Player" },
            { 0x12, "[CUTSCENE] Class/Job" },
            { 0x13, "Is Player Character" },
            { 0x23, "[CUTSCENE] Is Gatherer" },
            { 0x24, "[CUTSCENE] Is Crafter" },
            { 0x25, "Is Mount" },
            { 0x26, "Mounted" },
            { 0x27, "Swimming" },
            { 0x28, "Underwater" },
            { 0x29, "Class/Job" },
            { 0x33, "Mount Id" },
            { 0x39, "GPose" },
        } );

        public static readonly LuaPool Pool2 = new( 2, 32, new() );

        public static readonly LuaPool Pool3 = new( 3, 32, new() );

        public static List<LuaPool> Pools => new() {
            Pool1,
            Pool2,
            Pool3,
        };
    }
}

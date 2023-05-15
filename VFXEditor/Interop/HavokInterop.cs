namespace VfxEditor.Interop {
    public class HavokInterop {
        public static void ReplaceHavokAnimation( string baseHkx, int indexToReplace, string newHkx, int indexToUse, string output ) {
            Run( $"1 {baseHkx} {indexToReplace} {newHkx} {indexToUse} {output}" );
        }

        public static void RemoveHavokAnimation( string baseHkx, int indexToRemove, string output ) {
            Run( $"2 {baseHkx} {indexToRemove} {baseHkx} {indexToRemove} {output}" );
        }

        public static void AddHavokAnimation( string baseHkx, string newHkx, int indexToUse, string output ) {
            Run( $"3 {baseHkx} {indexToUse} {newHkx} {indexToUse} {output}" );
        }

        public static void HavokToBin( string animHkx, int animIndex, string sklHkx, string output ) {
            Run( $"4 {animHkx} {animIndex} {sklHkx} 0 {output}" );
        }

        public static void Run( string arguments ) => InteropUtils.Run( "animassist_custom.exe", arguments );
    }
}

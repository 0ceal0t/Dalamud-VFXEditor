using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat {
    // https://github.com/lua/lua/blob/master/lopcodes.h#L201
    // maybe this instead? http://www.lua.org/source/5.1/lopcodes.h.html
    public enum LuaOperation {
        MOVE,
        LOADI,
        LOADF,
        LOADK,
        LOADKX,
        LOADFALSE,
        LFALSESKIP,
        LOADTRUE,
        LOADNIL,
        GETUPVAL,
        SETUPVAL,
        GETTABUP,
        GETTABLE,
        GETI,
        GETFIELD,
        SETTABUP,
        SETTABLE,
        SETI,
        SETFIELD,
        NEWTABLE,
        SELF,
        ADDI,
        ADDK,
        SUBK,
        MULK,
        MODK,
        POWK,
        DIVK,
        IDIVK,
        BANDK,
        BORK,
        BXORK,
        SHRI,
        SHLI,
        ADD,
        SUB,
        MUL,
        MOD,
        POW,
        DIV,
        IDIV,
        BAND,
        BOR,
        BXOR,
        SHL,
        SHR,
        MMBIN,
        MMBINI,
        MMBINK,
        UNM,
        BNOT,
        NOT,
        LEN,
        CONCAT,
        CLOSE,
        TBC,
        JMP,
        EQ,
        LT,
        LE,
        EQK,
        EQI,
        LTI,
        LEI,
        GTI,
        GEI,
        TEST,
        TESTSET,
        CALL,
        TAILCALL,
        RETURN,
        RETURN0,
        RETURN1,
        FORLOOP,
        FORPREP,
        TFORPREP,
        TFORCALL,
        TFORLOOP,
        SETLIST,
        CLOSURE,
        VARARG,
        VARARGPREP,
        EXTRAARG,

    }

    public class TmtrLuaEntry {
        public readonly TmbFile File;

        private readonly ParsedEnum<LuaOperation> Operation = new( "Operation" );
        private readonly ParsedShort Value1 = new( "Value 1" );
        private readonly ParsedShort Value2 = new( "Value 2" );
        private readonly ParsedFloat Unknown = new( "Unknown" );

        public TmtrLuaEntry( TmbFile file ) {
            File = file;
        }

        public TmtrLuaEntry( BinaryReader reader, TmbFile file ) : this( file ) {
            Operation.Read( reader );
            Value1.Read( reader );
            Value2.Read( reader );
            Unknown.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Operation.Write( writer );
            Value1.Write( writer );
            Value2.Write( writer );
            Unknown.Write( writer );
        }

        public void Draw() {
            Operation.Draw( File.Command );
            Value1.Draw( File.Command );
            Value2.Draw( File.Command );
            Unknown.Draw( File.Command );
        }
    }
}

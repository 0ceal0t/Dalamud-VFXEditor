using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AVFXLib.AVFX
{
    public class AVFXBlank : AVFXNode {
        public AVFXBlank() : base( "" ) { }

        public override byte[] ToBytes() {
            return new byte[] {};
        }

        public override bool EqualsNode(AVFXNode node, List<string> messages ) {
            if(!(node is AVFXBlank)) {
                messages.Add(string.Format("Not Blank"));
                return false;
            }
            return true;
        }

        public override string ExportString(int level) {
            return "";
        }
    }
}

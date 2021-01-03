using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXEditor
{
    public class XivSelectedAction
    {
        public XivActionBase Action;
        public bool CastVfxExists = false;
        public string CastVfxPath;

        public bool SelfVfxExists = false;
        public string SelfTmbPath;
        public List<string> SelfVfxPaths = new List<string>();

        public static Regex rx = new Regex( @"vfx([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled ); // scuffed file parsing, because I literally don't give a shit about anything else in this file...probably

        public XivSelectedAction( Lumina.Data.FileResource file, XivActionBase action )
        {
            Action = action;
            CastVfxExists = action.CastVFXExists;
            CastVfxPath = action.GetCastVFXPath();

            if(file != null )
            {
                var data = file.Data;
                SelfVfxExists = true;
                SelfTmbPath = file.FilePath.Path;

                string stringData = Encoding.UTF8.GetString( data );
                MatchCollection matches = rx.Matches( stringData );
                foreach(Match m in matches )
                {
                    SelfVfxPaths.Add( m.Value );
                }
            }
        }
    }
}

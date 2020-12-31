using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AVFXLib.AVFX
{
    public class AVFXNode
    {
        // jank logging shit
        public static List<string> LogMessages = new List<string>();
        public static void ResetLog() { LogMessages = new List<string>(); }

        public string Name { get; set; }
        // calculate size on the fly

        public List<AVFXNode> Children { get; set; }

        public AVFXNode(string n)
        {
            Name = n;
            Children = new List<AVFXNode>();
        }

        public virtual byte[] toBytes()
        {
            int totalSize = 0;
            byte[][] byteArrays = new byte[Children.Count][];
            for(int i = 0; i < Children.Count; i++)
            {
                byteArrays[i] = Children[i].toBytes();
                totalSize += byteArrays[i].Length;
            }
            byte[] bytes = new byte[8 + Util.RoundUp(totalSize)];
            byte[] name = Util.NameTo4Bytes(Name);
            byte[] size = Util.IntTo4Bytes(totalSize);
            Buffer.BlockCopy(name, 0, bytes, 0, 4);
            Buffer.BlockCopy(size, 0, bytes, 4, 4);
            int bytesSoFar = 8;
            for(int i = 0; i < byteArrays.Length; i++)
            {
                Buffer.BlockCopy(byteArrays[i], 0, bytes, bytesSoFar, byteArrays[i].Length);
                bytesSoFar += byteArrays[i].Length;
            }
            return bytes;
        }

        // =====================
        public bool CheckEquals(AVFXNode node, out List<string> messages)
        {
            ResetLog();
            bool result = EqualsNode(node);
            messages = new List<string>(AVFXNode.LogMessages);
            return result;
        }

        public virtual bool EqualsNode(AVFXNode node)
        {
            if((node is AVFXLeaf) || (node is AVFXBlank))
            {
                AVFXNode.LogMessages.Add(string.Format("Wrong Type {0} / {1}", Name, node.Name));
                return false;
            }
            if (Name != node.Name)
            {
                AVFXNode.LogMessages.Add(string.Format("Wrong Name {0} / {1}", Name, node.Name));
                return false;
            }

            List<AVFXNode> notBlank = new List<AVFXNode>();
            List<AVFXNode> notBlank2 = new List<AVFXNode>();
            foreach (AVFXNode n in Children)
            {

                if(!(n is AVFXBlank))
                    notBlank.Add(n);
            }
            foreach (AVFXNode n in node.Children)
            {
                if (!(n is AVFXBlank))
                    notBlank2.Add(n);
            }

            if(notBlank.Count != notBlank2.Count)
            {
                AVFXNode.LogMessages.Add(string.Format("Wrong Node Size {0} : {1} / {2} : {3}", Name, notBlank.Count, node.Name, notBlank2.Count));
                return false;
            }
            for(int idx = 0; idx < notBlank.Count; idx++)
            {
                bool e = notBlank[idx].EqualsNode(notBlank2[idx]);
                if (!e)
                {
                    AVFXNode.LogMessages.Add(string.Format("Not Equal {0} index: {1}", Name, idx));
                    return false;
                }
            }

            return true;
        }

        public virtual string exportString(int level)
        {
            string ret = string.Format("{0}+---  {1} ----\n", new string('\t', level), Name);
            foreach(var c in Children)
            {
                ret = ret + c.exportString(level + 1);
            }
            return ret;
        }
    }
}

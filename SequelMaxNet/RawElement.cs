using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SequelMaxNet
{
    public class RawElement : RawTreeNode
    {
        public RawElement()
        {
            SetXmlType(XMLTYPE.XML_ELEMENT);
        }

        public RawElement(string name)
        {
            SetXmlType(XMLTYPE.XML_ELEMENT);

            SetName(name);
        }

        public RawElement(string name, string sValue)
        {
            SetXmlType(XMLTYPE.XML_ELEMENT);

            SetName(name);

            SetValue(sValue);
        }

        public bool Exists()
        {
            return GetName() != "";
        }

        public bool Append(RawTreeNode child)
        {
            if (child == null)
                return false;

            child.SetParent(this);
            GetVec().Add(child);

            return true;
        }

        public RawElement Add(RawTreeNode node1)
        {
            Append(node1);

            return this;
        }

        public RawElement Find(string names)
        {
            string[] vec = null;
            bool bMultipleParent = false;

            string temp = names;
            int size = temp.IndexOf("|");
            if (size != -1)
            {
                bMultipleParent = true;
            }
            size = temp.IndexOf("\\");
            if (size != -1)
            {
                bMultipleParent = true;
            }
            size = temp.IndexOf("/");
            if (size != -1)
            {
                bMultipleParent = true;
            }
            if (bMultipleParent)
            {
                char[] seps = new char[3];
                seps[0] = '|';
                seps[1] = '/';
                seps[2] = '\\';
                vec = temp.Split(seps);
            }

            RawElement elem = this;

            if (vec == null)
                return elem;

            for (int i = 0; i < vec.Length; ++i)
            {
                elem = elem.FindFirstChild(vec[i]);

                if (elem.Exists() == false)
                    return new RawElement();
            }

            return elem;
        }

        private RawElement FindFirstChild(string name)
        {
            int cnt = GetVec().Count;
            List<RawTreeNode> vec = GetVec();
            for (int i = 0; i < cnt; ++i)
            {
                if (vec[i].GetXmlType() == XMLTYPE.XML_ELEMENT && vec[i].GetName() == name)
                {
                    return vec[i] as RawElement;
                }
            }

            return new RawElement();
        }

        public static bool IsValidName(string name)
        {
            for (int i = 0; i < name.Length; ++i)
            {
                char ch = name[i];

                if (i == 0)
                {
                    if (ch >= '0' && ch <= '9')
                        return false;
                }

                if (!(
                    (ch >= 'a' && ch <= 'z') ||
                    (ch >= 'A' && ch <= 'Z') ||
                    (ch >= '0' && ch <= '9') ||
                    ch == '-' || ch == '_' || ch == '.'
                    ))
                {
                    return false;
                }
            }

            return true;
        }

        private RawElement FindElementRoot()
        {
            RawElement tmp = this;
            RawElement found = null;

            while (true)
            {
                if (tmp.parent != null)
                    tmp = tmp.parent as RawElement;
                else
                {
                    found = tmp;
                    break;
                }
            }

            return found;
        }

    }
}

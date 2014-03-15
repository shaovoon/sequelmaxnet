using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SequelMaxNet
{
    public enum XMLTYPE
    {
	    XML_NONE,
	    XML_ELEMENT,
	    XML_ATTRIBUTE,
	    XML_COMMENT,
	    XML_CDATA
    };

    public class RawTreeNode
    {
        protected List<RawTreeNode> pVec;
        protected List<KeyValuePair<string, string>> pAttrs;
        protected string Name;
        protected string Value;
        protected XMLTYPE xmltype;

        public RawTreeNode parent;

        public RawTreeNode()
        {
            pVec = null;
            pAttrs = null;
            Name = "";
            Value = "";
            xmltype = XMLTYPE.XML_NONE;
            parent = null;
        }

        public void Destroy()
        {
            if (pVec != null)
            {
                for (int i = 0; i < pVec.Count; ++i)
                    pVec[i].Destroy();

                pVec.Clear();

                pVec = null;
            }

            pAttrs = null;
        }

        public void DetachAndDelete()
        {
            if (parent != null && parent.pVec != null)
            {
                // remove
                List<RawTreeNode> vec = parent.pVec;

                for (int i = 0; i < vec.Count; ++i)
                {
                    if (this == vec[i])
                    {
                        vec.Remove(vec[i]);
                        break;
                    }
                }

                Destroy();
            }
        }

        public void Detach()
        {
            if (parent != null && parent.pVec != null)
            {
                // remove
                List<RawTreeNode> vec = parent.pVec;

                for (int i = 0; i < vec.Count; ++i)
                {
                    if (this == vec[i])
                    {
                        vec.Remove(vec[i]);
                    }
                }
                SetParent(null);
            }
        }

        public void AddChild(RawTreeNode pChild)
        {
            if (pVec == null)
                pVec = new List<RawTreeNode>();

            pChild.SetParent(this);
            pVec.Add(pChild);
        }

        public string GetName()
        {
            return Name;
        }

        public string GetValue()
        {
            return Value;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetValue(string val)
        {
            Value = val;
        }

        public List<RawTreeNode> GetVec()
        {
            if (pVec == null)
                pVec = new List<RawTreeNode>();

            return pVec;
        }

        public List<KeyValuePair<string, string>> GetAttrs()
        {
            if (pAttrs == null)
                pAttrs = new List<KeyValuePair<string, string>>();

            return pAttrs;
        }

        public int GetAttrCount()
        {
            if (pAttrs == null)
                return 0;

            return pAttrs.Count();
        }

        public XMLTYPE GetXmlType()
        {
            return xmltype;
        }

        public void SetXmlType(XMLTYPE type)
        {
            xmltype = type;
        }

        public RawTreeNode FindRoot()
        {
            RawTreeNode tmp = this;
            RawTreeNode found = null;

            while (true)
            {
                if (tmp.parent != null)
                    tmp = tmp.parent;
                else
                {
                    found = tmp;
                    break;
                }
            }

            return found;
        }

        public string FindRootName()
        {
            RawTreeNode found = FindRoot();

            return found.GetName();
        }

        public void Discard()
        {
            this.Destroy();
        }

        public RawTreeNode GetParent()
        {
            return parent;
        }

        public void SetParent(RawTreeNode p)
        {
            parent = p;
        }

    }
}

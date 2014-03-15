using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SequelMaxNet
{
    public class Element
    {
        protected RawElement m_pRawElement;

        public Element(RawElement pRawElement)
        {
            m_pRawElement = pRawElement;
        }


        public string GetRootName()
        {
            if (m_pRawElement != null)
                return m_pRawElement.FindRootName();

            return "";
        }

        public string GetName()
        {
            return m_pRawElement.GetName();
        }
        private void SetNode(RawElement ptrElement)
        {
            m_pRawElement = ptrElement;
        }

        private static bool SplitString(string str, List<string> vec, ref bool bMultipleParent)
        {
            vec.Clear();
            bMultipleParent = false;

            string temp = str;
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

                string[] arr = temp.Split(seps);
                for (int i = 0; i < arr.Length; ++i)
                    vec.Add(arr[i]);

            }

            if (vec.Count == 0 && string.IsNullOrEmpty(str) == false)
                vec.Add(str);

            return true;
        }

        private static bool SplitNamespace(string src, string wstrName, string wstrNamespace)
        {
            wstrName = src;
            wstrNamespace = "";
            bool bNamespace = false;
            string temp = src;
            int size = temp.IndexOf(":");
            if (size != -1)
            {
                bNamespace = true;
            }
            else
            {
                wstrName = src;
                return false;
            }
            if (bNamespace)
            {
                char[] seps = new char[1];
                seps[0] = ':';

                string[] vec = temp.Split(seps);
                if (vec.Length > 1)
                {
                    wstrNamespace = vec[0];
                    wstrName = vec[1];
                }
            }

            return true;
        }

        public bool Exists()
        {
            if (m_pRawElement == null)
                return false;

            return true;
        }

        public List<KeyValuePair<string, string>> GetAttributeCollection()
        {
            if (m_pRawElement == null)
                throw new NullReferenceException("Null Element");

            return m_pRawElement.GetAttrs();
        }



        public Attribute Attr(string attrName)
        {
            if (m_pRawElement == null)
                return new Attribute();

            Attribute attr = new Attribute();

            attr.SetParam(m_pRawElement, attrName);
            return attr;
        }

        public List<string> GetAttrNames()
        {
            List<string> vec = new List<string>();

            if (m_pRawElement == null)
                return vec;

            if (m_pRawElement != null && m_pRawElement.GetAttrCount() > 0)
            {
                List<KeyValuePair<string, string>> attrList = m_pRawElement.GetAttrs();

                for (int i = 0; i < attrList.Count; ++i)
                {
                    string name = attrList[i].Key;
                    vec.Add(name);
                }
            }

            return vec;
        }

        public List<Attribute> GetAllAttr()
        {
            List<Attribute> vec = new List<Attribute>();

            if (m_pRawElement == null)
                return vec;

            if (m_pRawElement != null && m_pRawElement.GetAttrCount() > 0)
            {
                List<KeyValuePair<string, string>> attrList = m_pRawElement.GetAttrs();

                for (int i = 0; i < attrList.Count(); ++i)
                {
                    Attribute attr = new Attribute();
                    attr.SetParam(m_pRawElement, attrList[i].Key); // TODO: set name
                    vec.Add(attr);
                }
            }

            return vec;
        }

        void Destroy()
        {
            if (m_pRawElement != null)
            {
                m_pRawElement.Destroy();
                m_pRawElement = null;
            }
        }
    }
}

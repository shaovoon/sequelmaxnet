using System;
using System.Collections.Generic;
using System.Text;

namespace SequelMaxNet
{
    public class Attribute
    {
        private RawElement m_pRawElement;
        private string m_sName;

        public Attribute()
        {
            m_pRawElement = null;
            m_sName = "";
        }

        // Non-default constructor
        public Attribute(RawElement pRawElement)
        {
            m_pRawElement = pRawElement;
            m_sName = "";
        }

        bool GetAttributeAt(string name, out string val, ref bool bExists)
        {
            bExists = false;
            val = "";
            if (m_pRawElement != null)
            {
                List<KeyValuePair<string, string>> attrmap = m_pRawElement.GetAttrs();

                if (attrmap != null)
                {
                    for (int i = 0; i < attrmap.Count; ++i)
                    {
                        KeyValuePair<string, string> pair = attrmap[i];
                        if (pair.Key == name)
                        {
                            val = pair.Value;
                            bExists = true;

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool Exists()
        {
            if (m_pRawElement==null)
                return false;

            string val = "";
            bool bExists = false;
            GetAttributeAt(m_sName, out val, ref bExists);

            return bExists;
        }

        public string GetName()
        {
            return m_sName;
        }

        public void SetParam(RawElement pRawElement, string name)
        {
            m_pRawElement = pRawElement;
            m_sName = name;
        }

        private bool GetString(string defaultVal, out string val)
        {
            if (m_pRawElement == null)
                throw new NullReferenceException("Invalid element!");

            string strValue = "";
            bool bExists = false;
            GetAttributeAt(m_sName, out strValue, ref bExists);
            if (false == bExists || string.IsNullOrEmpty(strValue))
            {
                val = defaultVal;
                return false;
            }

            val = strValue;

            return true;
        }

        public bool GetBool(bool defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            bool val = defaultVal;
            try
            {
                val = Convert.ToBoolean(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }

            return val;
        }

        public char GetChar(char defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            char val = defaultVal;
            try
            {
                val = Convert.ToChar(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public short GetInt16(short defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            short val = defaultVal;
            try
            {
                val = Convert.ToInt16(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public int GetInt32(int defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            int val = defaultVal;
            try
            {
                val = Convert.ToInt32(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public Int64 GetInt64(Int64 defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            Int64 val = defaultVal;
            try
            {
                val = Convert.ToInt64(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public ushort GetUInt16(ushort defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            ushort val = defaultVal;
            try
            {
                val = Convert.ToUInt16(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public uint GetUInt32(uint defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            uint val = defaultVal;
            try
            {
                val = Convert.ToUInt32(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public UInt64 GetUInt64(UInt64 defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            UInt64 val = defaultVal;
            try
            {
                val = Convert.ToUInt64(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public float GetFloat(float defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            float val = defaultVal;
            try
            {
                val = (float)Convert.ToDouble(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public double GetDouble(double defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            double val = defaultVal;
            try
            {
                val = Convert.ToDouble(src);
            }
            catch (System.FormatException)
            {
            }
            catch (System.OverflowException)
            {
            }
            return val;
        }

        public string GetString(string defaultVal)
        {
            string src = string.Empty;
            if (false == GetString("", out src))
                return defaultVal;

            string val = defaultVal;
            if (string.IsNullOrEmpty(src) == false)
                val = src;

            return val;
        }


    }
}

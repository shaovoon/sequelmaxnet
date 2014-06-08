using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SequelMaxNet
{
    public class Document
    {
        enum AfterElementName
        {
            None,
            NodeEnded,
            ElementEnded,
            EncounterWhitespace
        };

        private const int StartElementEndLength = 2;
        private const int EndElementStartLength = 2;
        private const int ProcessingInstructionStartLength = 5;
        private const int ProcessingInstructionEndLength = 2;
        private const int DocTypeStartLength = 9;
        private const int DocTypeEndLength = 1;
        private const int CDataStartLength = 9;
        private const int CDataEndLength = 3;
        private const int CommentStartLength = 4;
        private const int CommentEndLength = 3;

        public delegate void ProcessingInstructionDelegate(string name, string value);
        public delegate void EndElementDelegate(string text);
        public delegate void StartElementDelegate(Element elem);

        private ProcessingInstructionDelegate processingInstructionDelegate;
        private Dictionary<string, StartElementDelegate> dict_StartElemDelegate = new Dictionary<string, StartElementDelegate>();
        private Dictionary<string, EndElementDelegate> dict_EndElemDelegate = new Dictionary<string, EndElementDelegate>();
        private Dictionary<string, EndElementDelegate> dict_CommentDelegate = new Dictionary<string, EndElementDelegate>();
        private Dictionary<string, EndElementDelegate> dict_CDataDelegate = new Dictionary<string, EndElementDelegate>();

        public bool Open(string file)
        {
            Dictionary<string, string> processingInstruction = new Dictionary<string, string>();
            return Open(processingInstruction, file);
        }

        public bool Open(Dictionary<string, string> processingInstruction, string file)
        {
            TextReader reader = new StreamReader(file, Encoding.UTF8); //read the file. Here in 1.txt you need to put your data in single line
            string all = reader.ReadToEnd();
            Encoding iso = Encoding.GetEncoding("unicode");
            byte[] isoBytes = iso.GetBytes(all);

            string contents = new System.Text.UnicodeEncoding().GetString(((byte[])(isoBytes)));

            return ParseXMLString(processingInstruction, contents);
        }

        public void RegisterProcessingInstructionDelegate(ProcessingInstructionDelegate f)
        {
            processingInstructionDelegate = f;
        }

        public void RegisterStartElementDelegate(string elems, StartElementDelegate f)
        {
            dict_StartElemDelegate[elems] = f;
        }

        public void RegisterEndElementDelegate(string elems, EndElementDelegate f)
        {
            dict_EndElemDelegate[elems] = f;
        }

        public void RegisterCommentDelegate(string elems, EndElementDelegate f)
        {
            dict_CommentDelegate[elems] = f;
        }

        public void RegisterCDataDelegate(string elems, EndElementDelegate f)
        {
            dict_CDataDelegate[elems] = f;
        }

        private static bool IsContent(string src, int index)
        {
            if (index >= src.Length)
                return false;

            char ch = src[index];

            return ch != '<' && ch != '>';
        }

        private static bool IsWhitespace(string src, ref int newline_cnt)
        {
            if (src == "")
                return true;

            bool whiteline = true;
            for (int i = 0; i < src.Length; ++i)
            {
                if (IsWhitespace(src[i], ref newline_cnt) == false)
                {
                    whiteline = false;
                }
            }
            return whiteline;
        }

        private static bool IsWhitespace(string src, int index, ref int newline_cnt)
        {
            if (index >= src.Length)
                return false;

            char ch = src[index];

            if (ch == '\n')
                ++newline_cnt;

            return ch == ' ' || ch == '\n' || ch == '\t' || ch == '\r';
        }

        private static bool IsWhitespace(char ch, ref int newline_cnt)
        {
            if (ch == '\n')
                ++newline_cnt;

            return ch == ' ' || ch == '\n' || ch == '\t' || ch == '\r';
        }

        private static bool IsStartElementEnd(string src, int index)
        {
            if (index + 1 < src.Length)
            {
                // comparing "/>"
                return src[index] == '/' && src[index + 1] == '>';
            }

            return false;
        }

        private static bool IsEndElementStart(string src, int index)
        {
            if (index + 1 < src.Length)
            {
                // comparing "</"
                return src[index] == '<' && src[index + 1] == '/';
            }

            return false;
        }

        private static bool IsProcessingInstructionStart(string src, int index)
        {
            if (index + 4 < src.Length)
            {
                // comparing "<?xml"
                return src[index] == '<' && src[index + 1] == '?' && src[index + 2] == 'x'
                    && src[index + 3] == 'm' && src[index + 4] == 'l';
            }

            return false;
        }

        private static bool IsProcessingInstructionEnd(string src, int index)
        {
            if (index + 1 < src.Length)
            {
                // comparing "?>"
                return src[index] == '?' && src[index + 1] == '>';
            }

            return false;
        }

        private static bool IsDocTypeStart(string src, int index)
        {
            if (index + 8 < src.Length)
            {
                // comparing "<!DOCTYPE"
                return src[index] == '<' && src[index + 1] == '!' && src[index + 2] == 'D'
                    && src[index + 3] == 'O' && src[index + 4] == 'C' && src[index + 5] == 'T'
                    && src[index + 6] == 'Y' && src[index + 7] == 'P' && src[index + 8] == 'E';
            }

            return false;
        }

        private static bool IsDocTypeEnd(string src, int index)
        {
            if (index < src.Length)
            {
                // comparing ">"
                return src[index] == '>';
            }

            return false;
        }

        private static bool IsCDataStart(string src, int index)
        {
            if (index + 8 < src.Length)
            {
                // comparing "<![CDATA["
                return src[index] == '<' && src[index + 1] == '!' && src[index + 2] == '[' &&
                    src[index + 3] == 'C' && src[index + 4] == 'D' && src[index + 5] == 'A' &&
                    src[index + 6] == 'T' && src[index + 7] == 'A' && src[index + 8] == '[';
            }

            return false;
        }

        private static bool IsCDataEnd(string src, int index)
        {
            if (index + 2 < src.Length)
            {
                // comparing "]]>"
                return src[index] == ']' && src[index + 1] == ']' && src[index + 2] == '>';
            }

            return false;
        }

        private static bool IsCommentStart(string src, int index)
        {
            if (index + 3 < src.Length)
            {
                // comparing "<!--"
                return src[index] == '<' && src[index + 1] == '!' && src[index + 2] == '-' && src[index + 3] == '-';
            }

            return false;
        }

        private static bool IsCommentEnd(string src, int index)
        {
            if (index + 2 < src.Length)
            {
                // comparing "-."
                return src[index] == '-' && src[index + 1] == '-' && src[index + 2] == '>';
            }

            return false;
        }

        private static bool ReadProcessingInstructionValue(string src, ref int index, out string dest, Dictionary<string, string> processingInstruction, ref int newline_cnt)
        {
            dest = "";
            while (IsProcessingInstructionEnd(src, index) == false)
                dest += src[index++];

            index += ProcessingInstructionEndLength;

            string name = "";
            string val = "";
            for (int i = 0; i < dest.Length; ++i)
            {
                while (IsWhitespace(dest, i, ref newline_cnt))
                {
                    ++i;
                }

                ReadAttributeName(dest, ref i, out name, ref newline_cnt);

                while (IsWhitespace(dest, i, ref newline_cnt) || dest[i] == '=')
                    ++i;

                ReadAttributeValue(dest, ref i, out val);

                if (name != "" && val != "")
                {
                    processingInstruction[name] = val;
                }
            }

            return true;
        }

        private static bool ReadDocType(string src, ref int index, out string dest)
        {
            dest = "";
            while (IsDocTypeEnd(src, index) == false)
                dest += src[index++];

            index += DocTypeEndLength;

            return true;
        }

        private static bool ReadElementName(string src, ref int index, out string dest, out AfterElementName res, ref int newline_cnt)
        {
            dest = "";
            res = AfterElementName.None;
            while (src[index] != '>')
            {
                if (IsWhitespace(src[index], ref newline_cnt))
                {
                    res = AfterElementName.EncounterWhitespace;
                    return true;
                }

                if (IsStartElementEnd(src, index))
                {
                    res = AfterElementName.ElementEnded;
                    index += 1;
                    return true;
                }

                dest += src[index];

                ++index;

                if (index >= src.Length)
                    return false;
            }

            if (src[index] == '>')
            {
                res = AfterElementName.NodeEnded;
            }

            if (dest.Length == 0)
                return false;

            return true;
        }

        // return false means this element has no text, what follows may be a start element or end element.
        private static bool ReadElementValue(string src, ref int index, out string dest, ref int newline_cnt)
        {
            dest = "";
            while (src[index] != '<')
            {
                if (IsWhitespace(src[index], ref newline_cnt) == false)
                    dest += src[index];

                ++index;

                if (index >= src.Length)
                    return false;
            }

            --index;

            if (dest.Length > 0)
            {
                dest = UnescapeXML(dest);
                return true;
            }

            return false;
        }

        private static bool ReadAttributeName(string src, ref int index, out string dest, ref int newline_cnt)
        {
            dest = "";
            while (src[index] != '=')
            {
                if (IsWhitespace(src[index], ref newline_cnt) == false)
                    dest += src[index];
                else
                    break;

                ++index;

                if (index >= src.Length)
                    return false;
            }

            return true;
        }

        private static bool ReadAttributeValue(string src, ref int index, out string dest)
        {
            dest = "";
            ++index;
            if ( (src[index] == '\"' && src[index + 1] == '\"') || (src[index] == '\'' && src[index + 1] == '\'') )
            {
                ++index;
                return true;
            }
            while (src[index] != '\"' && src[index] != '\'')
            {
                dest += src[index];
                ++index;

                if (index >= src.Length)
                    return false;
            }

            ++index;

            dest = UnescapeXML(dest);

            return true;
        }

        private static bool ReadCDataValue(string src, ref int index, out string dest)
        {
            dest = "";
            while (IsCDataEnd(src, index) == false)
                dest += src[index++];

            index += (CDataEndLength - 1);

            return true;
        }

        private static bool ReadCommentValue(string src, ref int index, out string dest)
        {
            dest = "";
            while (IsCommentEnd(src, index) == false)
                dest += src[index++];

            index += (CommentEndLength - 1);

            dest = UnescapeXML(dest);

            return true;
        }

        string GetStackString(Stack<string> StackElement)
        {
            string str = "";
            Stack<string>.Enumerator enumElement = StackElement.GetEnumerator();
            while(enumElement.MoveNext())
            {
                if(string.IsNullOrEmpty(str))
                    str = enumElement.Current;
                else
                {
                    str = enumElement.Current + "|" + str;
                }
            }
            return str;
        }

        // /> : start element end
        // </ : end element start

        // /> : start element end
        // </ : end element start
        bool ParseXMLString(Dictionary<string, string> processingInstruction, string src)
        {
            int i = 0;
            int newline_cnt = 0;
            while (IsProcessingInstructionStart(src, i))
            {
                i += ProcessingInstructionStartLength;
                string prep = "";
                if (ReadProcessingInstructionValue(src, ref i, out prep, processingInstruction, ref newline_cnt))
                {
                    if (processingInstructionDelegate != null)
                    {
                        Dictionary<string, string>.Enumerator enumInstr = processingInstruction.GetEnumerator();

                        while (enumInstr.MoveNext())
                        {
                            processingInstructionDelegate(enumInstr.Current.Key, enumInstr.Current.Value);
                        }
                    }
                }
            }
            while (IsDocTypeStart(src, i))
            {
                i += DocTypeStartLength;
                string dest = "";
                ReadDocType(src, ref i, out dest);
            }

            Stack<string> StackElement = new Stack<string>();
            Stack<int> StackLineNum = new Stack<int>();
            string name = "";
            string val = "";
            string text = "";
            while (i < src.Length)
            {
                text = "";
                while (IsContent(src, i))
                {
                    text += src[i];
                    ++i;
                }

                if (i >= src.Length)
                    break;

                if (IsCDataStart(src, i))
                {
                    i += CDataStartLength;
                    ReadCDataValue(src, ref i, out val);
                    if (StackElement.Count > 0)
                    {
                        string key = GetStackString(StackElement);
                        if (dict_CDataDelegate.ContainsKey(key))
                        {
                            EndElementDelegate del = dict_CDataDelegate[key];
                            del(val);
                        }
                    }
                }
                else if (IsCommentStart(src, i))
                {
                    i += CommentStartLength;
                    ReadCommentValue(src, ref i, out val);
                    if (StackElement.Count > 0)
                    {
                        string key = GetStackString(StackElement);
                        if (dict_CommentDelegate.ContainsKey(key))
                        {
                            EndElementDelegate del = dict_CommentDelegate[key];
                            del(val);
                        }
                    }
                }
                else if (src[i] == '<' && IsEndElementStart(src, i) == false) // is start element
                {
                    // read element
                    AfterElementName res = AfterElementName.None;
                    ++i;
                    if (ReadElementName(src, ref i, out name, out res, ref newline_cnt))
                    {
                        if (name == "")
                        {
                            throw new InvalidOperationException("empty name");
                        }
                        StackElement.Push(name);
                        StackLineNum.Push(newline_cnt + 1);

                        RawElement pRawElement = new RawElement(name);
                        if (res == AfterElementName.EncounterWhitespace)
                        {
                            bool isStartElementEnd = false;
                            while (i < src.Length)
                            {
                                while (IsWhitespace(src, i, ref newline_cnt))
                                {
                                    ++i;
                                }

                                isStartElementEnd = IsStartElementEnd(src, i);
                                if (isStartElementEnd)
                                {
                                    i += 1;
                                    string key = GetStackString(StackElement);
                                    if (dict_StartElemDelegate.ContainsKey(key))
                                    {
                                        StartElementDelegate del = dict_StartElemDelegate[key];
                                        Element elem = new Element(pRawElement);
                                        del(elem);
                                    }
                                    else
                                    {
                                        pRawElement.Destroy();
                                        pRawElement = null;
                                    }
									if (dict_EndElemDelegate.ContainsKey(key))
									{
										EndElementDelegate del = dict_EndElemDelegate[key];
										del("");
									}

                                    StackElement.Pop();
                                    StackLineNum.Pop();
                                    goto breakout;
                                }

                                if (src[i] == '>')
                                    break;

                                ReadAttributeName(src, ref i, out name, ref newline_cnt);

                                while (IsWhitespace(src, i, ref newline_cnt) || src[i] == '=')
                                    ++i;

                                ReadAttributeValue(src, ref i, out val);

                                if (name != "")
                                {
                                    pRawElement.GetAttrs().Add(new KeyValuePair<string, string>(name, val));
                                }
                            }
                            string key1 = GetStackString(StackElement);
                            if (dict_StartElemDelegate.ContainsKey(key1))
                            {
                                StartElementDelegate del = dict_StartElemDelegate[key1];
                                Element elem = new Element(pRawElement);
                                del(elem);
                            }
                            else
                            {
                                if (!isStartElementEnd)
                                {
                                    pRawElement.Destroy();
                                    pRawElement = null;
                                }
                            }
                        }
                        else if (res == AfterElementName.ElementEnded)
                        {
                            string key = GetStackString(StackElement);
                            if (dict_StartElemDelegate.ContainsKey(key))
                            {
                                StartElementDelegate del = dict_StartElemDelegate[key];
                                Element elem = new Element(pRawElement);
                                del(elem);
                            }
                            else
                            {
                                pRawElement.Destroy();
                                pRawElement = null;
                            }
                            if (dict_EndElemDelegate.ContainsKey(key))
                            {
                                EndElementDelegate del = dict_EndElemDelegate[key];
                                del("");
                            }

                            StackElement.Pop();
                            StackLineNum.Pop();
                        }
                        else // NodeEnded
                        {
                            string key = GetStackString(StackElement);
                            if (dict_StartElemDelegate.ContainsKey(key))
                            {
                                StartElementDelegate del = dict_StartElemDelegate[key];
                                Element elem = new Element(pRawElement);
                                del(elem);
                            }
                            else
                            {
                                pRawElement.Destroy();
                                pRawElement = null;
                            }
                        }
                    }
                    else // parser error
                    {
                        PrintStack(i, newline_cnt, "Start tag error", StackElement, StackLineNum);
                    }
                }
                else // is end element
                {
                    AfterElementName result;
                    i += 2;
                    if (ReadElementName(src, ref i, out name, out result, ref newline_cnt))
                    {
                        if (name == "")
                        {
                            PrintStack(i, newline_cnt, "The ending tag error", StackElement, StackLineNum);
                        }

                        if (result == AfterElementName.NodeEnded)
                        {
                            if (StackElement.Peek() != name)
                            {
                                // print error of different start and end names
                                string error = "Different start and end tag:";
                                error += "Expect <" + StackElement.Peek() + "> but received </" + name + ">";
                                PrintStack(i, newline_cnt, error, StackElement, StackLineNum);
                                return false;
                            }
                            else
                            {
                                string key1 = GetStackString(StackElement);

                                if (dict_EndElemDelegate.ContainsKey(key1))
                                {
                                    EndElementDelegate del = dict_EndElemDelegate[key1];

                                    int newline_cnt1 = 0;
                                    if (IsWhitespace(text, ref newline_cnt1))
                                        del("");
                                    else
                                        del(UnescapeXML(text));

                                }
                                StackElement.Pop();
                                StackLineNum.Pop();
                            }
                        }
                    }
                    else
                    {
                        // print all stack names and element without a end element
                        PrintStack(i, newline_cnt, "The last element does not have a ending tag", StackElement, StackLineNum);
                    }

                }
breakout:
                ++i;
            }

            if (StackElement.Count > 0)
            {
                PrintStack(i, newline_cnt, "The element does not have a ending tag", StackElement, StackLineNum);
                return false;
            }

            return true;
        }

        private static void PrintStack(int pos, int line, string error, Stack<string> StackElement, Stack<int> StackLineNum)
        {
            ++line; // one based counting
            // print all the stack names
            List<string> vecTemp = new List<string>();
            List<int> vecLineNum = new List<int>();
            while (StackElement.Count > 0)
            {
                string name = StackElement.Pop();
                vecTemp.Add(name);
                int tmpline = StackLineNum.Pop();
                vecLineNum.Add(tmpline);
            }
            string err = error + ":Parsing error near pos=" + pos.ToString() + ", line no.=" + line.ToString() + ":";

            for (int i = vecTemp.Count - 1; i > 0; --i)
                err += "<" + vecTemp[i] + " LineNum:" + vecLineNum[i].ToString() + ">";

            throw new InvalidOperationException(err);
        }

        private static string Replace(string fmtstr, string anchor, string replace)
        {
            string fmtstrReturn = fmtstr;

            fmtstrReturn.Replace(anchor, replace);

            return fmtstrReturn;
        }

        private static string UnescapeXML(string fmtstr)
        {
            string fmtstr1 = Replace(fmtstr, "&amp;", "&");
            fmtstr1 = Replace(fmtstr1, "&quot;", "\"");
            fmtstr1 = Replace(fmtstr1, "&apos;", "\'");
            fmtstr1 = Replace(fmtstr1, "&lt;", "<");
            fmtstr1 = Replace(fmtstr1, "&gt;", ">");

            return fmtstr1;
        }

    }
}

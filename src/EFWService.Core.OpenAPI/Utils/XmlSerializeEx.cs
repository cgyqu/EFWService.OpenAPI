using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Runtime.Serialization;

namespace EFWService.Core.OpenAPI.Exs
{
    public static class XmlSerializeEx
    {
        public static string CustomXmlSerialize(this object response, bool removeAllAttributes = false,
            bool removeXsd = false,
            bool removeXsi = false,
            string[] removeNode = null, string head = "<?xml version=\"1.0\" encoding=\"utf-8\"?>")
        {
            string xml = string.Empty;
            XmlSerializer ser = new XmlSerializer(response.GetType());

            using (MemoryStream stream = new MemoryStream(100))
            {
                ser.Serialize(stream, response);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    xml = reader.ReadToEnd();
                }
            }

            var xe = XElement.Parse(xml);
            if (removeAllAttributes)
            {
                xe.RemoveAttributes();
            }

            RemoveXsXXX(removeXsd, removeXsi, xe);

            if (removeNode != null && removeNode.Length > 0)
            {
                List<XElement> removeList = new List<XElement>();

                foreach (var item in xe.Elements())
                {
                    if (removeNode.Contains(item.Name.LocalName))
                    {
                        removeList.Add(item);
                    }

                }
                foreach (var item in removeList)
                {
                    item.Remove();
                }
            }
            return head + System.Environment.NewLine + xe.ToString();
        }

        private static void RemoveXsXXX(bool removeXsd, bool removeXsi, XElement xe)
        {
            List<XAttribute> rList = new List<XAttribute>();
            foreach (var item in xe.Attributes())
            {
                if (removeXsd && item.Name.LocalName == "xsd")
                {
                    rList.Add(item);
                }
                if (removeXsi && item.Name.LocalName == "xsi")
                {
                    rList.Add(item);
                }
            }
            foreach (var item in rList)
            {
                item.Remove();
            }
        }

        public static string CustomXmlSerialize(this object response, bool removeAttributes,
            params string[] removeNode)
        {
            string xml = string.Empty;
            XmlSerializer ser = new XmlSerializer(response.GetType());

            using (MemoryStream stream = new MemoryStream(100))
            {
                ser.Serialize(stream, response);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    xml = reader.ReadToEnd();
                }
            }

            var xe = XElement.Parse(xml);
            if (removeAttributes)
            {
                xe.RemoveAttributes();
            }

            foreach (var item in xe.Attributes())
            {
                Console.WriteLine(item);
            }

            if (removeNode != null && removeNode.Length > 0)
            {
                List<XElement> removeList = new List<XElement>();

                foreach (var item in xe.Elements())
                {
                    if (removeNode.Contains(item.Name.LocalName))
                    {
                        removeList.Add(item);
                    }

                }
                foreach (var item in removeList)
                {
                    item.Remove();
                }
            }
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + System.Environment.NewLine + xe.ToString();
        }

        public static T XmlDeserialize<T>(this string s)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(s);
            XmlNodeReader reader = new XmlNodeReader(xdoc.DocumentElement);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            object obj = ser.Deserialize(reader);
            return (T)obj;
        }
    }

    /// <summary>
    /// xml的相关操作
    /// </summary>
    public static class XmlOperator
    {
        // <summary>
        /// 获取父节点中的节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNodeName"></param>
        /// <returns></returns>
        public static string GetNodeText(XmlNode parentNode, string childNodeName)
        {
            if (parentNode == null || string.IsNullOrEmpty(childNodeName))
                return "";
            XmlNode node = parentNode.SelectSingleNode(childNodeName);
            return node == null ? "" : node.InnerText.Trim();
        }
        /// <summary>
        /// 从XElement中获取值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetValue(this XElement x, string node)
        {
            if (x.Element(node) != null)
            {
                return x.Element(node).Value;
            }
            return "";
        }
        /// <summary>
        /// 从XmlNode中获取值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetValue(this XmlNode node, string name)
        {
            string result = "";
            XmlNode nodeName = node.SelectSingleNode(name);
            if (nodeName != null && string.IsNullOrEmpty(nodeName.InnerText))
            {
                result = nodeName.InnerText.Trim();
            }
            return result;
        }
    }
}

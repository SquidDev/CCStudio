using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CCStudio.Core.Serialization
{
    /// <summary>
    /// Serialize a dictionary
    /// </summary>
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue> : IXmlSerializable
    {
        public const string ItemNode = "Item";
        public const string KeyNode = "Key";
        public const string ValueNode = "Value";

        public IDictionary<TKey, TValue> Dict { get; protected set; }
        public SerializableDictionary() { }
        public SerializableDictionary(IDictionary<TKey, TValue> Dict)
        {
            this.Dict = Dict;
        }
        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader Reader)
        {
            Dict = new Dictionary<TKey, TValue>();

            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            bool WasEmpty = Reader.IsEmptyElement;
            Reader.Read();

            if (WasEmpty) return;

            while (Reader.NodeType != XmlNodeType.EndElement)
            {
                Reader.ReadStartElement(ItemNode);

                Reader.ReadStartElement(KeyNode);
                TKey Key = (TKey)KeySerializer.Deserialize(Reader);
                Reader.ReadEndElement();

                Reader.ReadStartElement(ValueNode);
                TValue Value = (TValue)ValueSerializer.Deserialize(Reader);
                Reader.ReadEndElement();

                Dict.Add(Key, Value);

                Reader.ReadEndElement();
                Reader.MoveToContent();
            }
            Reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter Writer)
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey Key in Dict.Keys)
            {
                Writer.WriteStartElement(ItemNode);

                Writer.WriteStartElement(KeyNode);
                KeySerializer.Serialize(Writer, Key);
                Writer.WriteEndElement();

                Writer.WriteStartElement(ValueNode);
                TValue Value = Dict[Key];
                ValueSerializer.Serialize(Writer, Value);
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }
        }
        #endregion

    }
}

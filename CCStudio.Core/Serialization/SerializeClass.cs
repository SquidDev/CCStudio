using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CCStudio.Core.Serialization
{
    /// <summary>
    /// Serialize any class
    /// </summary>
    [XmlRoot("Node")]
    public class SerializeClass<T> : IXmlSerializable
    {
        public const string TypeAttribute = "Type";

        public T Value { get; set; }

        public SerializeClass() { }
        public SerializeClass(T Obj)
        {
            Value = Obj; 
        }

        public void WriteXml(XmlWriter Writer)
        {
            if (Value == null)
            {
                Writer.WriteAttributeString(TypeAttribute, "null");
                return;
            }

            Type ClassType = this.Value.GetType();
            XmlSerializer Serializer = new XmlSerializer(ClassType);

            Writer.WriteAttributeString(TypeAttribute, ClassType.AssemblyQualifiedName);
            Serializer.Serialize(Writer, Value);
        }

        public void ReadXml(XmlReader Reader)
        {
            if (!Reader.HasAttributes) throw new FormatException(String.Format("Expected a {0} attribute!", TypeAttribute));

            string ClassType = Reader.GetAttribute(TypeAttribute);
            Reader.Read(); // consume the value
            if (ClassType == "null") return; // Leave T at default value

            XmlSerializer Serializer = new XmlSerializer(Type.GetType(ClassType));
            Value = (T)Serializer.Deserialize(Reader);
            Reader.ReadEndElement();
        }

        public XmlSchema GetSchema() 
        {
            return null;
        }
    }
}

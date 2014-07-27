using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CCStudio.Core.Computers
{
    public class Session
    {
        public static XmlSerializer Serializer = new XmlSerializer(typeof(Session));

        public List<ComputerConfig> Computers { get; set; }
        public int NextComputer { get; set; }
        public string ComputerDirectory { get; set; }

        [XmlIgnore]
        public string Path { get; protected set; }

        public Session() { }

        public Session(string Path)
        {
            this.Path = Path;
            ComputerDirectory = System.IO.Path.GetDirectoryName(Path);

            Computers = new List<ComputerConfig>();
            NextComputer = 0;
        }
        public static Session Load(string Path)
        {
            using (StreamReader Reader = new StreamReader(Path))
            {
                Session NewSession = (Session)Serializer.Deserialize(Reader);
                NewSession.Path = Path;
                return NewSession;
            }
        }

        public void Save()
        {
            using (StreamWriter Writer = new StreamWriter(Path))
            {
                Serializer.Serialize(Writer, this);
            }
        }
    }
}

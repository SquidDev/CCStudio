using CCStudio.Core.Computers.Time;
using CCStudio.Core.Filesystem;
using CCStudio.Core.Peripheral;
using CCStudio.Core.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CCStudio.Core.Computers
{
    /// <summary>
    /// Stores config of a computer for Serialization
    /// </summary>
    public class ComputerConfig
    {       
        public int Id { get; set; }
        public string Label { get; set; }
        public bool IsColour { get; set; }

        public string BootFile { get; set; }

        public ComputerState State { get; set; }
        public Stopwatch Uptime { get; set; }
        public DynamicComputerTime Time { get; set; }

        [XmlIgnore]
        protected ComboMount _Mount;

        [XmlIgnore]
        public ComboMount Mount
        {
            get
            {
                return _Mount;
            }
            set
            {
                _Mount = value;
                Mounts = new SerializableDictionary<FilePath, SerializeClass<IMount>>(
                    value.Mounts.ToDictionary(P => P.Key, P => new SerializeClass<IMount>(P.Value))
                );
            }
        }

        public SerializableDictionary<FilePath, SerializeClass<IMount>> Mounts { get; set; }

        [XmlIgnore]
        protected Dictionary<string, IPeripheral> _Peripherals;

        [XmlIgnore]
        public Dictionary<string, IPeripheral> Peripherals
        {
            get
            {
                return _Peripherals;
            }
            set
            {
                _Peripherals = value;
               SerializePeripherals = new SerializableDictionary<string,SerializeClass<IPeripheral>>(
                    value.ToDictionary(P => P.Key, P => new SerializeClass<IPeripheral>(P.Value))   
               );
            }
        }

        [XmlElement("Peripherals")]
        public SerializableDictionary<string, SerializeClass<IPeripheral>> SerializePeripherals { get; set; }

        public Redstone Redstone { get; set; }

        [XmlArrayItem("Global")]
        public List<string> BannedGlobals { get; set; }

        public ComputerConfig() { }

        public static ComputerConfig Create(int Id, string Directory)
        {
            ComputerConfig Config = new ComputerConfig();

            Config.Id = Id;
            Config.Label = "";
            Config.IsColour = true;

            Config.BootFile = "bios.lua";

            Config.Time = new DynamicComputerTime();
            Config.Time.StartTime();

            Config.Uptime = new Stopwatch();
            Config.State = ComputerState.Stopped;


            Config.Mount = new ComboMount(new Dictionary<FilePath, IMount> {
                {
                    new FilePath(""),
                    new DirectoryMount(Path.Combine(Directory, Id.ToString()), false, "hdd")
                },
                { 
                    new FilePath("rom"), 
                    new DirectoryMount("rom", true, "rom")
                }
            });
            Config.BannedGlobals = new List<string>()
            {
                "collectgarbage", "debug", "dofile", "io", "load", "loadfile", 
                "module", "newproxy", "os", "package", "print", "require"
            };

            Config.Peripherals = new Dictionary<string, IPeripheral>();
            Config.Redstone = Redstone.Create();

            return Config;
        }
    }
}

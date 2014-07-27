using System;
using System.Collections.Generic;
using System.IO;

namespace CCStudio.Core.Filesystem
{
    /// <summary>
    /// Mounts to a real life directory
    /// </summary>
    public class DirectoryMount : IMount
    {
        public string Base { get; set; }
        public string Label { get; set; }
        public bool ReadOnly  { get; set; }

        public DirectoryMount(string BaseDirectory, bool ReadOnly = false, string Label = "none")
        {
            Base = BaseDirectory;
            this.ReadOnly = ReadOnly;
            this.Label = Label;
        }
        public DirectoryMount() { }

        #region IMount Members
        public string GetLabel(FilePath FileP)
        {
            return Label;
        }
        public bool IsReadOnly(FilePath FileP)
        {
            return ReadOnly;
        }

        public bool Exists(FilePath FileP)
        {
            string WholePath = RealPath(FileP);
            return (File.Exists(WholePath) || Directory.Exists(WholePath));
        }

        public bool IsDirectory(FilePath File)
        {
            return Directory.Exists(RealPath(File));
        }

        public IEnumerable<FilePath> List(FilePath FileP)
        {
            string WholePath = RealPath(FileP);
            FilePath FullPath = new FilePath(WholePath);

            if (!Directory.Exists(WholePath)) throw new Exception("Not a directory");

            List<FilePath> Entries = new List<FilePath>();
            foreach (string Entry in Directory.EnumerateFileSystemEntries(WholePath))
            {
                Entries.Add(new FilePath(Entry).RemoveParent(FullPath));
            }

            return Entries;
        }

        public long GetSize(FilePath FileP)
        {
            string WholePath = RealPath(FileP);
            if (File.Exists(WholePath))
            {
                return new FileInfo(WholePath).Length;
            }
            else if (Directory.Exists(WholePath))
            {
                return 0;
            }
            else
            {
                throw new Exception("No such file");
            }
        }

        public StreamReader Read(FilePath FileP)
        {
            string WholePath = RealPath(FileP);

            if (File.Exists(WholePath))
            {
                return new StreamReader(WholePath);
            }
            throw new Exception("No such file");
        }

        public StreamWriter Write(FilePath FileP)
        {
            if (IsReadOnly(FileP)) throw new Exception("Access Denied");

            string Real = RealPath(FileP);
            string Base = Path.GetDirectoryName(Real);
            if (!Directory.Exists(Base)) MakeDirectory(new FilePath(Base));

            return new StreamWriter(Real);
        }

        public StreamWriter Append(FilePath FileP)
        {
            if (IsReadOnly(FileP)) throw new Exception("Access Denied");

            string Real = RealPath(FileP);
            string Base = Path.GetDirectoryName(Real);
            if (!Directory.Exists(Base)) MakeDirectory(new FilePath(Base));

            return new StreamWriter(Real, true);
        }

        public void MakeDirectory(FilePath FileP)
        {
            if (IsReadOnly(FileP)) throw new Exception("Access Denied");

            if (Exists(FileP)) throw new Exception("Directory exists");

            Directory.CreateDirectory(RealPath(FileP));
        }

        public void Delete(FilePath FileP)
        {
            if (IsReadOnly(FileP)) throw new Exception("Access Denied");

            string WholePath = RealPath(FileP);

            if (Directory.Exists(WholePath))
            {
                Directory.Delete(WholePath, true);
            }
            else if (File.Exists(WholePath))
            {
                File.Delete(WholePath);
            }
        }

        public long GetRemainingSpace()
        {
            //TODO: Add proper space management
            return 999999999;
        }

        public void Dispose()
        {

        }
        #endregion

        protected string RealPath(FilePath File)
        {
            return Path.Combine(Base, File.ToString().Replace(FilePath.DirectorySeparator, Path.DirectorySeparatorChar));
        }
        
    }
}

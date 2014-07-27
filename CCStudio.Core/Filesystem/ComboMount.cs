using CCStudio.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CCStudio.Core.Filesystem
{
    /// <summary>
    /// Maps several mounts to paths inside the virtual file system
    /// </summary>
    public class ComboMount : IMount
    {
        public IDictionary<FilePath, IMount> Mounts { get; protected set; }

        public ComboMount(IDictionary<FilePath, IMount> Mounts)
        {
            //Keep sorted in reverse so '/a/path/' is more relavent than '/a/' or '/'
            this.Mounts = new SortedList<FilePath, IMount>(new InverseComparer<FilePath>(Comparer<FilePath>.Default));
            foreach(KeyValuePair<FilePath, IMount> Mount in Mounts)
            {
                this.Mounts.Add(Mount);
            }
        }

        public ComboMount() : this(new Dictionary<FilePath, IMount>()) { }

        public void Dispose()
        {
            foreach (IMount Mount in Mounts.Select(P => P.Value))
            {
                Mount.Dispose();
            }
        }
        #region Getters
        /// <summary>
        /// Gets the appropriate mount for one a path
        /// </summary>
        protected KeyValuePair<FilePath, IMount> Get(FilePath Path)
        {
            return Mounts.First(Pair => Pair.Key == Path || Pair.Key.IsParentOf(Path));
        }

        /// <summary>
        /// Gets all mounts for one path
        /// </summary>
        protected IEnumerable<KeyValuePair<FilePath, IMount>> GetAll(FilePath Path)
        {
            return Mounts.Where(Pair => Pair.Key == Path || Pair.Key.IsParentOf(Path));
        }

        /// <summary>
        /// Gets the path and all children of that path
        /// </summary>
        protected IEnumerable<KeyValuePair<FilePath, IMount>> GetChildren(FilePath Path)
        {
            return Mounts.Where(Pair => Pair.Key == Path || Pair.Key.IsChildOf(Path));
        }
        #endregion

        #region IMount Members
        public string GetLabel(FilePath Path)
        {
            return Get(Path).Value.GetLabel(Path);
        }
        public bool IsReadOnly(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path);
            return Pair.Value.IsReadOnly(Path.RemoveParent(Pair.Key));
        }

        public bool Exists(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path);
            return Pair.Value.Exists(Path.RemoveParent(Pair.Key));
        }

        public bool IsDirectory(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path);;
            return Pair.Value.IsDirectory(Path.RemoveParent(Pair.Key));
        }

        public IEnumerable<FilePath> List(FilePath Path)
        {
            SortedSet<FilePath> Paths = new SortedSet<FilePath>();

            try
            {
                KeyValuePair<FilePath, IMount> ThisItem = Get(Path);
                foreach (FilePath Item in ThisItem.Value.List(Path.RemoveParent(ThisItem.Key)))
                {
                    Paths.Add(Item);
                }
            }
            catch (Exception) { }

            foreach (KeyValuePair<FilePath, IMount> Child in Mounts.Where(Pair => Pair.Key.IsChildOf(Path)))
            {
                FilePath ChildPath = Child.Key;
                while (true)
                {
                    FilePath Parent = ChildPath.ParentPath;

                    if (Parent == Path)
                    {
                        break;
                    }

                    ChildPath = Parent;
                }
                Paths.Add(ChildPath.RemoveParent(Path));
            }

            return Paths;
        }

        public long GetSize(FilePath Path)
        {
            return GetAll(Path).Sum(Pair => Pair.Value.GetSize(Path.RemoveParent(Pair.Key)));
        }

        public long GetRemainingSpace()
        {
            return Mounts.Sum(Pair => Pair.Value.GetRemainingSpace());
        }

        public StreamReader Read(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path); ;
            return Pair.Value.Read(Path.RemoveParent(Pair.Key));
        }

        public StreamWriter Write(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path); ;
            return Pair.Value.Write(Path.RemoveParent(Pair.Key));
        }

        public StreamWriter Append(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path); ;
            return Pair.Value.Append(Path.RemoveParent(Pair.Key));
        }

        public void MakeDirectory(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path); ;
            Pair.Value.MakeDirectory(Path.RemoveParent(Pair.Key));
        }

        public void Delete(FilePath Path)
        {
            KeyValuePair<FilePath, IMount> Pair = Get(Path); ;
            Pair.Value.Delete(Path.RemoveParent(Pair.Key));
        }
        #endregion
    }
}

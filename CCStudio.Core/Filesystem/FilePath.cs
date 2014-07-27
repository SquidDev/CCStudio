using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CCStudio.Core.Filesystem
{
    /// <summary>
    /// Used for basic path manipulation and standardisation
    /// </summary>
    public struct FilePath : IEquatable<FilePath>, IComparable<FilePath>, IXmlSerializable
    {
        public const char DirectorySeparator = '/';

        #region Properties
        private string _Path;
        
        public string Path
        {
            get
            {
                return _Path ?? "";
            }
            private set
            {
                _Path = value.Replace(System.IO.Path.DirectorySeparatorChar, DirectorySeparator)
                    // Don't know if I want to do this:
                    .Replace(String.Concat(DirectorySeparator, DirectorySeparator), DirectorySeparator.ToString())
                    //Remove trailing or leading slashes
                    .Trim(DirectorySeparator);
            }
        }

        public bool IsRoot
        {
            get { return Path.Length == 0; }
        }

        public string EntityName
        {
            get
            {
                string Name = Path;
                if (IsRoot) return null;
                int EndOfName = Name.Length;
                int StartOfName = Name.LastIndexOf(DirectorySeparator, EndOfName - 1, EndOfName) + 1;

                return Name.Substring(StartOfName, EndOfName - StartOfName);
            }
        }

        public FilePath ParentPath
        {
            get
            {
                string ParentPath = Path;
                if (IsRoot) throw new InvalidOperationException("There is no parent of root.");

                int LookaheadCount = ParentPath.Length;
                int Index = ParentPath.LastIndexOf(DirectorySeparator, LookaheadCount - 1, LookaheadCount);

                ParentPath = ParentPath.Remove(Index + 1);

                return new FilePath(ParentPath);
            }
        }
        #endregion

        public FilePath(params string[] Paths)
        {
            _Path = String.Join(DirectorySeparator.ToString(), Paths)
                .Replace(System.IO.Path.DirectorySeparatorChar, DirectorySeparator)
                // Don't know if I want to do this:
                .Replace(String.Concat(DirectorySeparator, DirectorySeparator), DirectorySeparator.ToString())
                //Remove trailing or leading slashes
                .Trim(DirectorySeparator);
        }

        #region Manipulation
        /// <summary>
        /// Removes "../"s and non printable characters
        /// </summary>
        /// <param name="AllowWildcards">Allow wildcards (*)</param>
        /// <returns>Prettified path</returns>
        public FilePath Prettify(bool AllowWildcards = false)
        {
            string FileP = Path;

            List<char> SpecialChars = new List<char>() { '"', ':', '<', '>', '?', '|' };

            StringBuilder CleanName = new StringBuilder();
            foreach (char C in FileP)
            {
                if ((C >= ' ') && (!SpecialChars.Contains(C)) && ((AllowWildcards) || (C != '*'))) CleanName.Append(C);
            }
            FileP = CleanName.ToString();

            // If it contains .. then we want to remove them
            if(FileP.Contains(".."))
            {
                string[] Parts = FileP.Split('/');
                Stack<string> OutputParts = new Stack<string>();
                foreach (string Part in Parts)
                {
                    if ((Part.Length != 0) && (!Part.Equals(".")))
                    {
                        if (Part.Equals(".."))
                        {
                            if (OutputParts.Count > 0)
                            {
                                string Top = OutputParts.Peek();
                                if (!Top.Equals(".."))
                                {
                                    OutputParts.Pop();
                                }
                                else
                                {
                                    OutputParts.Push("..");
                                }
                            }
                            else
                            {
                                OutputParts.Push("..");
                            }
                        }
                        else if (Part.Length >= 255)
                        {
                            OutputParts.Push(Part.Substring(0, 255));
                        }
                        else
                        {
                            OutputParts.Push(Part);
                        }
                    }
                }

                StringBuilder Result = new StringBuilder();
                IEnumerator<string> It = OutputParts.GetEnumerator();
                bool Next = It.MoveNext();
                while (Next)
                {
                    Result.Insert(0, It.Current);

                    Next = It.MoveNext();
                    if (Next)
                    {
                        Result.Insert(0, "/");
                    }
                }

                FileP = Result.ToString();
            }

            return new FilePath(FileP);
        }

        /// <summary>
        /// Simplifies path and removes "../"s 
        /// </summary>
        /// <param name="FilePath">Path to sanitise</param>
        /// <param name="AllowWildcards">Allow wildcards (*)</param>
        /// <returns>Sanitised path</returns>
        public FilePath Sanitise(bool AllowWildcards = false)
        {
            FilePath NewPath = Prettify();
            return new FilePath(NewPath.Path.Replace("..", ""));
        }

        
        public FilePath AppendPath(string RelativePath)
        {
            return new FilePath(Path, RelativePath);
        }

        public FilePath AppendPath(FilePath RelativePath)
        {
            return new FilePath(Path, RelativePath.Path);
        }
        #endregion
        #region Parent/Children
        public bool IsParentOf(FilePath path)
        {
            return Path.Length != path.Path.Length && path.Path.StartsWith(Path);
        }

        public bool IsChildOf(FilePath path)
        {
            return path.IsParentOf(this);
        }

        public FilePath RemoveParent(FilePath Parent)
        {
            if (Parent.Path == String.Empty) return Clone();
            if (!Path.StartsWith(Parent.Path))
                throw new ArgumentException("The specified path is not a parent of this path.");
            return new FilePath(Path.Remove(0, Parent.Path.Length));
        }

        public FilePath RemoveChild(FilePath Child)
        {
            if (!Path.EndsWith(Child.Path))
                throw new ArgumentException("The specified path is not a child of this path.");
            return new FilePath(Path.Substring(0, Path.Length - Child.Path.Length + 1));
        }
        #endregion
        public string GetExtension()
        {
            string Name = EntityName;
            int ExtensionIndex = Name.LastIndexOf('.');
            if (ExtensionIndex < 0) return "";
            return Name.Substring(ExtensionIndex);
        }

        public FilePath ChangeExtension(string Extension)
        {
            string Name = EntityName;
            int ExtensionIndex = Name.LastIndexOf('.');
            if (ExtensionIndex >= 0) return ParentPath.AppendPath(Name.Substring(0, ExtensionIndex) + Extension);
            return new FilePath(Path + Extension);
        }

        public string[] GetDirectorySegments()
        {
            FilePath Path = this;
            LinkedList<string> Segments = new LinkedList<string>();
            while(!Path.IsRoot)
            {
                Segments.AddFirst(Path.EntityName);
                Path = Path.ParentPath;
            }
            return Segments.ToArray();
        }

        #region Comparison
        public int CompareTo(FilePath Other)
        {
            return Path.CompareTo(Other.Path);
        }

        public override string ToString()
        {
            return Path;
        }

        public override bool Equals(object Obj)
        {
            if (Obj is FilePath) return Equals((FilePath) Obj);
            return false;
        }

        public bool Equals(FilePath Other)
        {
            return Other.Path.Equals(Path);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public static bool operator ==(FilePath PathA, FilePath PathB)
        {
            return PathA.Equals(PathB);
        }

        public static bool operator !=(FilePath PathA, FilePath PathB)
        {
            return !(PathA == PathB);
        }

        public static bool operator ==(FilePath PathA, object PathB)
        {
            return PathA.Equals(PathB);
        }

        public static bool operator !=(FilePath PathA, object PathB)
        {
            return !(PathA == PathB);
        }
        #endregion

        public FilePath Clone()
        {
            return new FilePath(Path);
        }
        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader Reader)
        {
            Path = Reader.ReadString();
        }

        public void WriteXml(XmlWriter Writer)
        {
            Writer.WriteString(Path);
        }
        #endregion
    }
}

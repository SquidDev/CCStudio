using System;
using System.IO;

namespace CCStudio.Core.Filesystem
{
    /// <summary>
    /// Helper functions for File systems
    /// </summary>
    public class FileUtilities
    {
        #region DirectoryUtils
        public static void CopyRecursive(IMount Mount, FilePath Source, FilePath Destination)
        {
            if (Source == Destination || Source.IsParentOf(Destination)) throw new Exception("Cannot copy a directory inside itself");
            if (!Mount.Exists(Source)) return;

            if (Mount.IsDirectory(Source))
            {
                if(!Mount.IsDirectory(Destination)) Mount.MakeDirectory(Destination);

                foreach (FilePath Child in Mount.List(Source))
                {
                    CopyRecursive(Mount, Source.AppendPath(Child), Destination.AppendPath(Child));
                }
            }
            else
            {
                StreamReader Reader = Mount.Read(Source);
                StreamWriter Writer = Mount.Write(Destination);

                while (Reader.Peek() >= 0)
                {
                    Writer.Write((char)Reader.Read());
                }

                Reader.Close();
                Writer.Close();
            }
        }
        #endregion
    }
}

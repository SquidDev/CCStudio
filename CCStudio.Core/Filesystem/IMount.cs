using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CCStudio.Core.Filesystem
{
    /// <summary>
    /// Base mount
    /// </summary>
    public interface IMount : IDisposable
    {
        string GetLabel(FilePath FileP);

        bool IsReadOnly(FilePath FileP);

        bool Exists(FilePath FileP);
        bool IsDirectory(FilePath FileP);

        IEnumerable<FilePath> List(FilePath FileP);

        long GetSize(FilePath FileP);
        long GetRemainingSpace();

        StreamReader Read(FilePath FileP);
        StreamWriter Write(FilePath FileP);
        StreamWriter Append(FilePath FileP);

        void MakeDirectory(FilePath FileP);
        void Delete(FilePath FileP);
    }
}

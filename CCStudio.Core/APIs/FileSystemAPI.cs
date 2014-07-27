using CCStudio.Core.Computers;
using CCStudio.Core.Filesystem;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CCStudio.Core.APIs
{
    /// <summary>
    /// Filesystem API (fs object).
    /// </summary>
    public class FileSystemAPI : ILuaAPI
    {
        public static readonly String[] _Names = new String[] { "fs" };
        public static readonly String[] _MethodNames = new String[] {
            "list", "combine", "getName", "getSize", 
            "exists", "isDir", "isReadOnly", 
            "makeDir", "move", "copy", "delete", "open", 
            "getDrive", "getFreeSpace", "find", "getDir"
        };

        protected IMount Mount;
        protected string BasePath;

        protected int MaxSize = 2 * 1024 * 1024;

        #region API methods
        public LuaTable list(string FileP)
        {
            return new LuaTable(Mount.List(new FilePath(FileP).Sanitise()).Select(P => P.ToString()));
        }

        public string combine(string Base, string Child)
        {
            return new FilePath(Base, Child).Prettify().ToString();
        }

        public string getName(string FileP)
        {
            return Path.GetFileName(FileP);
        }

        public long getSize(string FileP)
        {
            return Mount.GetSize(new FilePath(FileP).Sanitise());
        }

        public bool exists(string FileP)
        {
            return Mount.Exists(new FilePath(FileP).Sanitise());
        }

        public bool isDir(string FileP)
        {
            FilePath Path = new FilePath(FileP).Prettify();
            if (Path.Path.Contains("..")) return false;
            return Mount.IsDirectory(Path.Sanitise());
        }

        public bool isReadOnly(string FileP)
        {
            return Mount.IsReadOnly(new FilePath(FileP).Sanitise());
        }

        public void makeDir(string FileP)
        {
            Mount.MakeDirectory(new FilePath(FileP).Sanitise());
        }

        public void move(string Inital, string New)
        {
            FilePath Original = new FilePath(Inital).Sanitise();
            FileUtilities.CopyRecursive(Mount, Original, new FilePath(New).Sanitise());
            Mount.Delete(Original);
        }

        public void copy(string Inital, string New)
        {
            FileUtilities.CopyRecursive(Mount, new FilePath(Inital).Sanitise(), new FilePath(New).Sanitise());
        }

        public void delete(string FileP)
        {
            Mount.Delete(new FilePath(FileP).Sanitise());
        }

        public ILuaObject open(string FileP, string Mode)
        {
            FilePath Path = new FilePath(FileP).Sanitise();
            switch (Mode)
            {
                case "r":
                    return new ReadFileHandle(Mount.Read(Path));
                case "rb":
                    return new ReadFileBinaryHandle(Mount.Read(Path));
                case "w":
                    return new WriteFileHandle(Mount.Write(Path));
                case "wb":
                    return new WriteFileBinaryHandle(Mount.Write(Path));
                case "a":
                    return new WriteFileHandle(Mount.Append(Path));
                case "ab":
                    return new WriteFileBinaryHandle(Mount.Append(Path));
            }

            throw new Exception("Unsupported mode");
        }

        public string getDrive(string FileP)
        {
            return Mount.GetLabel(new FilePath(FileP).Sanitise());
        }

        public long getFreeSpace()
        {
            return Math.Max(0, Mount.GetRemainingSpace());
        }

        public string find()
        {
            throw new NotImplementedException("Woops!");
        }

        public string getDir(string FileP)
        {
            return Path.GetDirectoryName(FileP);
        }

        #endregion
        #region ILuaAPI Members
        public override string[] GetNames()
        {
            return _Names;
        }

        public override string[] GetMethodNames()
        {
            return _MethodNames;
        }

        public override void Load(Computer OwnerComputer)
        {
            base.Load(OwnerComputer);
            Mount = OwnerComputer.Config.Mount;
        }
        #endregion

    }
    #region Reading
    public class ReadFileHandle : ILuaObject
    {
        protected StreamReader FileStream;
        protected bool IsBinary;

        public static readonly String[] _MethodNames = new String[] { "readLine", "readAll", "close" };
        public static readonly String[] _BinaryMethodNames = new String[] { "readLine", "readAll", "close", "read" };

        public ReadFileHandle(StreamReader File, bool IsBinary = false)
        {
            FileStream = File;
            this.IsBinary = IsBinary;
        }

        public string readLine()
        {
            return FileStream.ReadLine();
        }
        public string readAll()
        {
            string Result = FileStream.ReadToEnd();
            FileStream.Close();
            return Result;
        }
        public void close()
        {
            FileStream.Close();
        }
        public override string[] GetMethodNames()
        {
            return IsBinary ? _BinaryMethodNames : _MethodNames;
        }
    }
    public class ReadFileBinaryHandle : ReadFileHandle
    {
        public ReadFileBinaryHandle(StreamReader File) : base(File, true) { }

        public int Read()
        {
            return FileStream.Read();
        }
    }
    #endregion
    #region Writing
    public class WriteFileHandle : ILuaObject
    {
        protected StreamWriter FileStream;

        public static readonly String[] _MethodNames = new String[] { "writeLine", "write", "close", "flush" };

        public WriteFileHandle(StreamWriter File)
        {
            FileStream = File;
        }

        public void writeLine(string Text)
        {
            FileStream.WriteLine(Text);
        }

        public void write(string Text)
        {
            FileStream.Write(Text);
        }
 
        public void close()
        {
            FileStream.Close();
        }

        public void flush()
        {
            FileStream.Flush();
        }
        public override string[] GetMethodNames()
        {
            return _MethodNames;
        }
    }
    public class WriteFileBinaryHandle : WriteFileHandle
    {
        public WriteFileBinaryHandle(StreamWriter File) : base(File) { }

        public void Write(int Character)
        {
            FileStream.Write((char)Character);
        }
    }
    #endregion
}

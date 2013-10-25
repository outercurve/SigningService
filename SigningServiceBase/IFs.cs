using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SigningServiceBase.Flags;

namespace SigningServiceBase
{
    public interface IFs : IDependency
    {
        IFileSystem FileSystem { get; }
        void MoveFileEx(string oldFilename, string newFilename, MoveFileFlags moveFlags);
        void TryHardToMakeFileWriteable(string filename);
        void TryHardToDelete(string location);
    }
}

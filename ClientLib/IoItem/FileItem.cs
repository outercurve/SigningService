using System.IO;
using System.IO.Abstractions;
using ClrPlus.Powershell.Provider.Utility;

namespace Outercurve.ToolsLib.IoItem
{
    class FileItem : IIoItem
    {
        private readonly FileInfoBase _file;
        public FileItem(FileInfoBase file)
        {
            _file = file;
        }


        public string FullName
        {
            get { return _file.FullName; }
        }

        public Stream OpenRead()
        {
            return _file.OpenRead();
        }

        public Stream OpenWrite()
        {
            return  _file.Open(FileMode.Create, FileAccess.Write);
        }




        public string Name
        {
            get { return _file.Name; }
        }
    }
}

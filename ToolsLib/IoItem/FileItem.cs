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

        public ProgressStream OpenRead()
        {
           return new ProgressStream(_file.OpenRead());
        }

        public ProgressStream OpenWrite()
        {
            return new ProgressStream(_file.OpenWrite());
        }




        public string Name
        {
            get { return _file.Name; }
        }
    }
}

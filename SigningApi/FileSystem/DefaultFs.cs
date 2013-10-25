using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Web;
using Outercurve.SigningApi.WinApi;
using SigningServiceBase;
using SigningServiceBase.Flags;

namespace Outercurve.SigningApi.FileSystem
{
    public class DefaultFs : IFs
    {

        public DefaultFs(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }
        public IFileSystem FileSystem
        {
            get;
            private set;
        }

        public void MoveFileEx(string oldFilename, string newFilename, MoveFileFlags moveFlags)
        {
            Kernel32.MoveFileEx(oldFilename, newFilename, moveFlags);
        }

        public void TryHardToMakeFileWriteable(string filename)
        {
            filename = FileSystem.Path.GetFullPath(filename);

            if (FileSystem.File.Exists(filename))
            {
                var tmpFilename = filename.GenerateTemporaryFilename();
                FileSystem.File.Move(filename, tmpFilename);
                FileSystem.File.Copy(tmpFilename, filename);
                TryHardToDelete(tmpFilename);
            }
        }


        public void TryHardToDelete(string location)
        {
            if (FileSystem.Directory.Exists(location))
            {
                try
                {
                    FileSystem.Directory.Delete(location, true);
                }
                catch
                {
                    // didn't take, eh?
                }
            }

            if (FileSystem.File.Exists(location))
            {
                try
                {
                    FileSystem.File.Delete(location);
                }
                catch
                {
                    // didn't take, eh?
                }
            }

            // if it is still there, move and mark it.
            if (FileSystem.File.Exists(location) || FileSystem.Directory.Exists(location))
            {
                try
                {
                    // move the file to the tmp file
                    // and tell the OS to remove it next reboot.
                    var tmpFilename = location.GenerateTemporaryFilename(); // generates a unique filename but not a file!
                    MoveFileEx(location, tmpFilename, MoveFileFlags.MOVEFILE_REPLACE_EXISTING);

                    if (FileSystem.File.Exists(location) || FileSystem.Directory.Exists(location))
                    {
                        // of course, if the tmpFile isn't on the same volume as the location, this doesn't work.
                        // then, last ditch effort, let's rename it in the current directory
                        // and then we can hide it and mark it for cleanup .
                        tmpFilename = FileSystem.Path.Combine(FileSystem.Path.GetDirectoryName(location), "tmp." + CounterHex + "." + FileSystem.Path.GetFileName(location));
                        MoveFileEx(location, tmpFilename, MoveFileFlags.MOVEFILE_REPLACE_EXISTING);
                        if (FileSystem.File.Exists(tmpFilename) || FileSystem.Directory.Exists(location))
                        {
                            // hide the file for convenience.
                            FileSystem.File.SetAttributes(tmpFilename, FileSystem.File.GetAttributes(tmpFilename) | FileAttributes.Hidden);
                        }
                    }

                    // Now we mark the locked file to be deleted upon next reboot (or until another coapp app gets there)
                    MoveFileEx(FileSystem.File.Exists(tmpFilename) ? tmpFilename : location, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
                }
                catch
                {
                    // really. Hmmm. 
                    // Logger.Error(e);
                }

                if (FileSystem.File.Exists(location))
                {
                    // Logger.Error("Unable to forcably remove file '{0}'. This can't be good.", location);
                }
            }
            return;
        }


    }
}
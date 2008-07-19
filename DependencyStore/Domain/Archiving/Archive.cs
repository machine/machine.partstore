using System;
using System.Collections.Generic;

using ICSharpCode.SharpZipLib.Zip;

namespace DependencyStore.Domain.Archiving
{
  public class Archive : IDisposable
  {
    private readonly List<ManifestEntry> _entries = new List<ManifestEntry>();
    private readonly ZipFile _zipFile;

    public long UncompressedBytes
    {
      get
      {
        long total = 0;
        foreach (ManifestEntry entry in _entries)
        {
          total += entry.UncompressedLength;
        }
        return total;
      }
    }

    public IEnumerable<ManifestEntry> Entries
    {
      get { return _entries; }
    }

    public IEnumerable<FileAsset> FileAssets
    {
      get
      {
        foreach (ManifestEntry entry in _entries)
        {
          yield return entry.FileAsset;
        }
      }
    }

    public Archive()
    {
    }

    public Archive(ZipFile zipFile)
    {
      _zipFile = zipFile;
    }

    public void Add(ManifestEntry manifestEntry)
    {
      _entries.Add(manifestEntry);
    }

    public void Add(Purl archivePath, FileSystemFile file)
    {
      Add(new ManifestEntry(archivePath, file));
    }

    public FileSet ToFileSet()
    {
      FileSet fileSet = new FileSet();
      foreach (FileAsset file in this.FileAssets)
      {
        fileSet.Add(file);
      }
      return fileSet;
    }

    #region IDisposable Members
    public void Dispose()
    {
      if (_zipFile != null)
      {
        _zipFile.Close();
      }
    }
    #endregion
  }
}

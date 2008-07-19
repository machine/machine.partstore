using System;
using System.Collections.Generic;

namespace DependencyStore.Domain
{
  public class SinkLocation : Location
  {
    public override bool IsSource
    {
      get { return false; }
    }

    public SinkLocation(Purl path, FileSystemEntry entry)
      : base(path, entry)
    {
    }

    public SinkLocation()
    {
    }

    public void CheckForNewerFiles(LatestFileSet latestFileSet)
    {
      foreach (FileSystemFile file in this.FileEntry.BreadthFirstFiles)
      {
        FileAsset possiblyNewer = latestFileSet.FindExistingByName(file);
        if (possiblyNewer != null && possiblyNewer.IsNewerThan(file))
        {
          DomainEvents.OnEncounteredOutdatedSinkFile(this, new OutdatedSinkFileEventArgs(this, file, possiblyNewer));
        }
      }
    }
  }
}
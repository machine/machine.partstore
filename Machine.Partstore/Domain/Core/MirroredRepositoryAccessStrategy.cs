using System;
using System.Collections.Generic;

using Machine.Core.Services;
using Machine.Partstore.Domain.Archiving;
using Machine.Partstore.Domain.FileSystem;

namespace Machine.Partstore.Domain.Core
{
  public class MirroredRepositoryAccessStrategy : IRepositoryAccessStrategy
  {
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(MirroredRepositoryAccessStrategy));

    private readonly IFileSystem _fileSystem;

    public MirroredRepositoryAccessStrategy(IFileSystem fileSystem)
    {
      _fileSystem = fileSystem;
    }

    #region IRepositoryAccessStrategy Members
    public void CommitVersionToRepository(Repository repository, NewProjectVersion newProjectVersion)
    {
      _log.Info("Committing: " + newProjectVersion);
      Purl destiny = repository.PathFor(newProjectVersion);
      CopyFiles(newProjectVersion.FileSet, destiny, false);
    }

    public void CheckoutVersionFromRepository(Repository repository, ArchivedProjectVersion version, Purl directory)
    {
      _log.Info("Checking out: " + version + " into " + directory);
      FileSystemEntry entry = Infrastructure.FileSystemEntryRepository.FindEntry(repository.PathFor(version));
      FileSet fileSet = new FileSet();
      fileSet.AddAll(entry.BreadthFirstFiles);
      CopyFiles(fileSet, directory, true);
    }

    public bool IsVersionPresentInRepository(Repository repository, ArchivedProjectVersion version)
    {
      return _fileSystem.IsDirectory(repository.PathFor(version).AsString);
    }
    #endregion

    private void CopyFiles(FileSet fileSet, Purl destiny, bool overwrite)
    {
      if (!_fileSystem.IsDirectory(destiny.AsString))
      {
        _fileSystem.CreateDirectory(destiny.AsString);
      }
      int filesSoFar = 0;
      foreach (FileSystemFile file in fileSet.Files)
      {
        Purl fileDestiny = destiny.Join(file.Path.ChangeRoot(fileSet.FindCommonDirectory()));
        fileDestiny.CreateParentDirectory();
        _fileSystem.CopyFile(file.Purl.AsString, fileDestiny.AsString, overwrite);
        filesSoFar++;
        DistributionDomainEvents.OnProgress(this, new FileCopyProgressEventArgs(filesSoFar / (double)fileSet.Count, file, destiny));
      }
    }
  }
}

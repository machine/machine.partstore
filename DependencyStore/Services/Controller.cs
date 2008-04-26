using System;
using System.Collections.Generic;

using DependencyStore.Domain;
using DependencyStore.Domain.Configuration;
using DependencyStore.Services.DataAccess;

using Machine.Core.Utility;
using Machine.Core.Services;

namespace DependencyStore.Services
{
  public class Controller : IController
  {
    private readonly IFileAndDirectoryRulesRepository _fileAndDirectoryRulesRepository;
    private readonly IConfigurationRepository _configurationRepository;
    private readonly IFileSystemEntryRepository _fileSystemEntryRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IFileSystem _fileSystem;

    public Controller(IFileSystemEntryRepository fileSystemEntryRepository, ILocationRepository locationRepository, IFileAndDirectoryRulesRepository fileAndDirectoryRulesRepository, IFileSystem fileSystem, IConfigurationRepository configurationRepository)
    {
      _fileSystemEntryRepository = fileSystemEntryRepository;
      _configurationRepository = configurationRepository;
      _fileSystem = fileSystem;
      _fileAndDirectoryRulesRepository = fileAndDirectoryRulesRepository;
      _locationRepository = locationRepository;
    }

    #region IController Members
    public void Show()
    {
      DomainEvents.EncounteredOutdatedSinkFile += ReportOutdatedFile;
      CheckForNewerFiles();
    }

    public void Update()
    {
      DomainEvents.EncounteredOutdatedSinkFile += UpdateOutdatedFile;
      CheckForNewerFiles();
    }
    #endregion

    private void CheckForNewerFiles()
    {
      DependencyStoreConfiguration configuration = _configurationRepository.FindConfiguration();
      FileAndDirectoryRules rules = _fileAndDirectoryRulesRepository.FindDefault();
      IList<SourceLocation> sources = _locationRepository.FindAllSources(rules);
      IList<SinkLocation> sinks = _locationRepository.FindAllSinks(rules);
      LatestFiles latest = new LatestFiles();
      latest.AddAll(sources);
      foreach (SinkLocation location in sinks)
      {
        location.CheckForNewerFiles(latest);
      }
    }

    private static void ReportOutdatedFile(object sender, OutdatedSinkFileEventArgs e)
    {
      TimeSpan age = e.SourceFile.ModifiedAt - e.SinkFile.ModifiedAt;
      FileSystemPath chrooted = e.SinkFile.Path.Chroot(e.SinkLocation.Path);
      Console.WriteLine("+ {0} {1}", chrooted.Full, TimeSpanHelper.ToPrettyString(age));
    }

    private void UpdateOutdatedFile(object sender, OutdatedSinkFileEventArgs e)
    {
      ReportOutdatedFile(sender, e);
      _fileSystem.CopyFile(e.SourceFile.Path.Full, e.SinkFile.Path.Full, true);
    }
  }
}

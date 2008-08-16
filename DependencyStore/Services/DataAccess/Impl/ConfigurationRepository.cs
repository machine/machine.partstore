using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using DependencyStore.Domain.Configuration;

using Machine.Core.Services;
using Machine.Core.Utility;

namespace DependencyStore.Services.DataAccess.Impl
{
  public class ConfigurationRepository : IConfigurationRepository
  {
    private readonly IFileSystem _fileSystem;
    private readonly IFileAndDirectoryRulesRepository _fileAndDirectoryRulesRepository;
    private readonly ConfigurationPaths _paths;

    public ConfigurationRepository(IFileSystem fileSystem, IFileAndDirectoryRulesRepository fileAndDirectoryRulesRepository, ConfigurationPaths paths)
    {
      _fileSystem = fileSystem;
      _fileAndDirectoryRulesRepository = fileAndDirectoryRulesRepository;
      _paths = paths;
    }

    #region IConfigurationRepository Members
    public DependencyStoreConfiguration FindConfiguration(string configurationFile)
    {
      try
      {
        using (StreamReader reader = _fileSystem.OpenText(configurationFile))
        {
          DependencyStoreConfiguration configuration = XmlSerializationHelper.DeserializeString<DependencyStoreConfiguration>(reader.ReadToEnd());
          configuration.EnsureValid();
          configuration.FileAndDirectoryRules = _fileAndDirectoryRulesRepository.FindDefault();
          return configuration;
        }
      }
      catch (InvalidOperationException e)
      {
        throw new InvalidConfigurationException("Error reading configuration", e);
      }
      catch (XmlException e)
      {
        throw new InvalidConfigurationException("Error reading configuration", e);
      }
    }

    public DependencyStoreConfiguration FindDefaultConfiguration()
    {
      return FindConfiguration(_paths.FindDefaultConfigurationPath());
    }

    public DependencyStoreConfiguration FindProjectConfiguration()
    {
      return FindConfiguration(_paths.FindConfigurationForCurrentProjectPath());
    }
    #endregion
  }
}

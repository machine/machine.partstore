﻿using System;
using System.Collections.Generic;

using DependencyStore.Domain.Configuration;
using DependencyStore.Services.DataAccess;

namespace DependencyStore.Domain.Services
{
  [Machine.Container.Model.Transient]
  public class UnpackageProjectManifest
  {
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectManifestRepository _projectManifestRepository;
    private readonly DependencyStoreConfiguration _configuration;

    public UnpackageProjectManifest(IProjectRepository projectRepository, IProjectManifestRepository projectManifestRepository, DependencyStoreConfiguration configuration)
    {
      _projectRepository = projectRepository;
      _projectManifestRepository = projectManifestRepository;
      _configuration = configuration;
    }

    public void Unpack(Repository repository)
    {
      foreach (Project project in _projectRepository.FindAllProjects(_configuration))
      {
        foreach (ProjectManifest manifest in _projectManifestRepository.FindProjectManifests(project))
        {
          ArchivedProjectVersion version = repository.FindProjectVersion(manifest);
          if (version == null)
          {
            continue;
          }
          Console.WriteLine(version);
          Console.WriteLine(manifest);
        }
      }
    }
  }
}

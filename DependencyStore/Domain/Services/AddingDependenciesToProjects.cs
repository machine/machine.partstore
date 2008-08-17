﻿using System;
using System.Collections.Generic;

using DependencyStore.Domain.Repositories;
using DependencyStore.Services.DataAccess;

namespace DependencyStore.Domain.Services
{
  [Machine.Container.Model.Transient]
  public class AddingDependenciesToProjects
  {
    private readonly IProjectReferenceRepository _projectReferenceRepository;

    public AddingDependenciesToProjects(IProjectReferenceRepository projectReferenceRepository)
    {
      _projectReferenceRepository = projectReferenceRepository;
    }

    public void SetAllProjects(Repository repository)
    {
      foreach (ProjectReference reference in _projectReferenceRepository.FindAllProjectReferences())
      {
        reference.MakeLatestVersion();
      }
    }
  }
}

﻿using System;
using System.Collections.Generic;

using Machine.Container;

using DependencyStore.Domain.Core.Repositories;

namespace DependencyStore.Domain.Core
{
  public class Infrastructure : DependencyStore.Domain.FileSystem.Infrastructure
  {
    public static IProjectManifestRepository ProjectManifestRepository
    {
      get { return IoC.Container.Resolve.Object<IProjectManifestRepository>(); }
    }

    public static IProjectReferenceRepository ProjectReferenceRepository
    {
      get { return IoC.Container.Resolve.Object<IProjectReferenceRepository>(); }
    }
  }
}

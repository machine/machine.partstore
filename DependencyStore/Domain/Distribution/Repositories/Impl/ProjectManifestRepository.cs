﻿using System;
using System.Collections.Generic;
using System.IO;

using Machine.Core.Services;
using Machine.Core.Utility;

using DependencyStore.Domain.Core;

namespace DependencyStore.Domain.Distribution.Repositories.Impl
{
  public class ProjectManifestRepository : IProjectManifestRepository
  {
    private static readonly XmlSerializer<ProjectManifest> _serializer = new XmlSerializer<ProjectManifest>();
    private readonly Dictionary<Purl, ProjectManifest> _cache = new Dictionary<Purl, ProjectManifest>();
    private readonly IFileSystem _fileSystem;

    public ProjectManifestRepository(IFileSystem fileSystem)
    {
      _fileSystem = fileSystem;
    }

    #region IProjectManifestRepository Members
    public ProjectManifestStore FindProjectManifestStore(Purl path)
    {
      return new ProjectManifestStore(path, ReadManifests(path));
    }

    public ProjectManifestStore FindProjectManifestStore(Project project)
    {
      if (project.HasLibraryDirectory)
      {
        return FindProjectManifestStore(project.LibraryDirectory);
      }
      return new ProjectManifestStore(project.LibraryDirectory, new List<ProjectManifest>());
    }

    public void SaveProjectManifestStore(ProjectManifestStore projectManifestStore)
    {
      foreach (ProjectManifest manifest in projectManifestStore)
      {
        Purl manifestPath = projectManifestStore.RootDirectory.Join(manifest.FileName);
        SaveProjectManifest(manifest, manifestPath);
      }
    }
    #endregion

    private IList<ProjectManifest> ReadManifests(Purl directory)
    {
      List<ProjectManifest> manifests = new List<ProjectManifest>();
      foreach (string fileName in _fileSystem.GetFiles(directory.AsString, "*." + ProjectManifest.Extension))
      {
        manifests.Add(ReadProjectManifest(new Purl(fileName)));
      }
      return manifests;
    }

    private ProjectManifest ReadProjectManifest(Purl path)
    {
      if (_cache.ContainsKey(path))
      {
        return _cache[path];
      }
      using (StreamReader stream = new StreamReader(_fileSystem.OpenFile(path.AsString)))
      {
        ProjectManifest manifest = _serializer.DeserializeString(stream.ReadToEnd());
        if (!manifest.IsAcceptableFileName(path))
        {
          throw new InvalidOperationException("Project reference manifest and project name should match: " + path);
        }
        _cache[path] = manifest;
        return manifest;
      }
    }

    private void SaveProjectManifest(ProjectManifest manifest, Purl path)
    {
      using (StreamWriter stream = new StreamWriter(_fileSystem.CreateFile(path.AsString)))
      {
        _cache[path] = manifest;
        stream.Write(_serializer.Serialize(manifest));
      }
    }
  }
}

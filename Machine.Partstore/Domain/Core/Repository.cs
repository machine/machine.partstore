using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Machine.Partstore.Domain.FileSystem;

namespace Machine.Partstore.Domain.Core
{
  public class Repository
  {
    public static readonly IRepositoryAccessStrategy AccessStrategy = new MirroredRepositoryAccessStrategy(Infrastructure.FileSystem);
    private readonly List<ArchivedProject> _projects = new List<ArchivedProject>();
    private Purl _rootPath;

    public List<ArchivedProject> Projects
    {
      get { return _projects; }
    }

    [XmlIgnore]
    public Purl RootPath
    {
      get { return _rootPath; }
      set { _rootPath = value; }
    }

    public string Name
    {
      get { return _rootPath.Name; }
    }

    public bool IsEmpty
    {
      get { return _projects.Count == 0; }
    }

    public ArchivedProject FindOrCreateProject(Project project)
    {
      ArchivedProject archived = FindProject(project.Name);
      if (archived == null)
      {
        archived = new ArchivedProject(project.Name);
        AddProject(archived);
      }
      return archived;
    }

    public ArchivedProject FindProject(string name)
    {
      foreach (ArchivedProject existing in _projects)
      {
        if (existing.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        {
          return existing;
        }
      }
      return null;
    }

    private void AddProject(ArchivedProject project)
    {
      if (FindProject(project.Name) != null)
      {
        throw new InvalidOperationException();
      }
      _projects.Add(project);
    }

    public ArchivedProject FindProject(ProjectManifest manifest)
    {
      return FindProject(manifest.ProjectName);
    }

    public IEnumerable<ReferenceCandidate> FindAllReferenceCandidates()
    {
      foreach (ArchivedProject project in _projects)
      {
        yield return new ReferenceCandidate(this.Name, project.Name, project.LatestVersion.Number, project.LatestVersion.Tags);
      }
    }

    public void Refresh()
    {
      Infrastructure.RepositoryRepository.RefreshRepository(this);
    }
  }
}

﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DependencyStore.Domain.Archiving;

namespace DependencyStore.Domain
{
  public class Repository
  {
    private readonly List<ArchivedProject> _projects = new List<ArchivedProject>();

    public List<ArchivedProject> Projects
    {
      get { return _projects; }
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

    public ArchivedProject FindProject(Project project)
    {
      return FindProject(project.Name);
    }

    public ArchivedProject FindProject(string name)
    {
      foreach (ArchivedProject existing in _projects)
      {
        if (existing.Name.Equals(name))
        {
          return existing;
        }
      }
      return null;
    }

    public void AddProject(ArchivedProject project)
    {
      if (FindProject(project.Name) != null)
      {
        throw new InvalidOperationException();
      }
      _projects.Add(project);
    }
  }
  public class ArchivedProject
  {
    private readonly List<ArchivedProjectVersion> _versions = new List<ArchivedProjectVersion>();
    private string _name;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public List<ArchivedProjectVersion> Versions
    {
      get { return _versions; }
    }

    public ArchivedProject()
    {
    }

    public ArchivedProject(string name)
    {
      _name = name;
    }

    protected ArchivedProjectVersion FindVersion(DateTime createdAt)
    {
      foreach (ArchivedProjectVersion existing in _versions)
      {
        if (existing.CreatedAt.Equals(createdAt))
        {
          return existing;
        }
      }
      return null;
    }

    public void AddVersion(ArchivedProjectVersion version)
    {
      if (FindVersion(version.CreatedAt) != null)
      {
        throw new InvalidOperationException("Duplicate project versions: " + this.Name + "-" + version.CreatedAt);
      }
      _versions.Add(version);
    }
  }
  public class ArchivedProjectVersion
  {
    private string _packager;
    private DateTime _createdAt;
    private string _archiveFileName;

    public string Packager
    {
      get { return _packager; }
      set { _packager = value; }
    }

    public DateTime CreatedAt
    {
      get { return _createdAt; }
      set { _createdAt = value; }
    }

    public string ArchiveFileName
    {
      get { return _archiveFileName; }
      set { _archiveFileName = value; }
    }

    public string CreatedAtVersion
    {
      get { return DateTimeToUniqueString(_createdAt); }
    }

    protected ArchivedProjectVersion()
    {
    }

    protected ArchivedProjectVersion(DateTime createdAt, string archiveFileName, string packager)
    {
      _createdAt = createdAt;
      _archiveFileName = archiveFileName;
      _packager = packager;
    }

    public static ArchivedProjectVersion Create(ArchivedProject project)
    {
      DateTime createdAt = DateTime.Now;
      string archiveFileName = project.Name + "-" + DateTimeToUniqueString(createdAt) + ZipArchiveWriter.ZipExtension;
      return new ArchivedProjectVersion(createdAt, archiveFileName, "Nobody");
    }

    private static string DateTimeToUniqueString(DateTime when)
    {
      return when.ToString("yyyyMMdd-HHmmssf");
    }
  }
}
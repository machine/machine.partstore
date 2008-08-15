using System;
using System.Collections.Generic;

using DependencyStore.Domain.Archiving;

namespace DependencyStore.Domain
{
  public class Project
  {
    private readonly string _name;
    private readonly SourceLocation _buildDirectory;
    private readonly Purl _libraryDirectory;

    public string Name
    {
      get { return _name; }
    }

    public SourceLocation BuildDirectory
    {
      get { return _buildDirectory; }
    }

    public bool HasLibrary
    {
      get { return _libraryDirectory != null; }
    }

    public Purl LibraryDirectory
    {
      get
      {
        if (!this.HasLibrary)
        {
          throw new InvalidOperationException("Not allowed to use this project's library!");
        }
        return _libraryDirectory;
      }
    }

    public Project(string name, SourceLocation buildDirectory, Purl libraryDirectory)
    {
      _name = name;
      _buildDirectory = buildDirectory;
      _libraryDirectory = libraryDirectory;
    }

    public Archive MakeArchive()
    {
      FileSet fileSet = this.BuildDirectory.ToFileSet();
      Purl fileRootDirectory = fileSet.FindCommonDirectory();
      Archive archive = new Archive();
      foreach (FileSystemFile file in fileSet.Files)
      {
        archive.Add(file.Path.ChangeRoot(fileRootDirectory), file);
      }
      return archive;
    }

    public override string ToString()
    {
      return "Project<" + this.Name + ">";
    }
  }
}

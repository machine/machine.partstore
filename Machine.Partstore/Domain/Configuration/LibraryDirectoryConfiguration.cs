using System;
using System.Xml.Serialization;

using Machine.Partstore.Domain.FileSystem;

namespace Machine.Partstore.Domain.Configuration
{
  [XmlType("Library")]
  public class LibraryDirectoryConfiguration : DirectoryConfiguration
  {
    public LibraryDirectoryConfiguration()
    {
    }

    public LibraryDirectoryConfiguration(string path)
      : base(path)
    {
    }

    public LibraryDirectoryConfiguration(Purl path)
      : base(path)
    {
    }
  }
}
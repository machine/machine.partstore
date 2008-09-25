using System;

using DependencyStore.Domain.Distribution;
using DependencyStore.Domain.Distribution.Repositories;

namespace DependencyStore.Commands
{
  public class AddNewVersionCommand : Command
  {
    private readonly ICurrentProjectRepository _currentProjectRepository;
    private readonly IRepositoryRepository _repositoryRepository;

    public AddNewVersionCommand(ICurrentProjectRepository currentProjectRepository, IRepositoryRepository repositoryRepository)
    {
      _currentProjectRepository = currentProjectRepository;
      _repositoryRepository = repositoryRepository;
    }

    public override CommandStatus Run()
    {
      new ArchiveProgressDisplayer(true);
      Repository repository = _repositoryRepository.FindDefaultRepository();
      CurrentProject project = _currentProjectRepository.FindCurrentProject();
      project.AddNewVersion(repository);
      _repositoryRepository.SaveRepository(repository);
      return CommandStatus.Success;
    }
  }
}
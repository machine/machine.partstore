using System;

using Machine.Container;

using DependencyStore.Services;
using DependencyStore.Domain.Repositories;
using DependencyStore.Domain.Configuration;
using DependencyStore.Services.DataAccess;

namespace DependencyStore.CommandLine
{
  public class Program
  {
    public class Command
    {
    }
    public class ShowCommand : Command
    {
    }
    public class UnpackageCommand : Command
    {
    }
    public class AddDependencyCommand : Command
    {
    }
    public class AddNewVersionCommand : Command
    {
    }
    public static void Main(string[] args)
    {
      using (MachineContainer container = new MachineContainer())
      {
        container.Initialize();
        container.PrepareForServices();
        ContainerRegistrationHelper helper = new ContainerRegistrationHelper(container);
        helper.AddServiceCollectionsFrom(typeof(DependencyStoreServices).Assembly);
        container.Start();
        IoC.Container = container;

        IConfigurationRepository configurationRepository = container.Resolve.Object<IConfigurationRepository>();
        bool dryRun = false;
        bool archiving = false;
        bool unpack = false;
        bool upgrade = false;
        foreach (string arg in args)
        {
          if (arg == "--archive")
          {
            archiving = true;
          }
          if (arg == "--unpack")
          {
            unpack = true;
          }
          if (arg == "--upgrade")
          {
            upgrade = true;
          }
          if (arg == "--dry")
          {
            dryRun = true;
          }
        }
        IController controller = container.Resolve.Object<IController>();
        DependencyStoreConfiguration configuration = configurationRepository.FindDefaultConfiguration();
        if (archiving)
        {
          IRepositoryRepository repositoryRepository = container.Resolve.Object<IRepositoryRepository>();
          Repository repository = repositoryRepository.FindDefaultRepository(configuration);
          controller.AddLatestToRepository(configuration, repository);
          repositoryRepository.SaveRepository(repository, configuration);
        }
        else if (unpack)
        {
          IRepositoryRepository repositoryRepository = container.Resolve.Object<IRepositoryRepository>();
          Repository repository = repositoryRepository.FindDefaultRepository(configuration);
          controller.Unpack(configuration, repository);
        }
        else if (upgrade)
        {
          IRepositoryRepository repositoryRepository = container.Resolve.Object<IRepositoryRepository>();
          Repository repository = repositoryRepository.FindDefaultRepository(configuration);
          controller.Upgrade(configuration, repository);
        }
        else
        {
          if (dryRun)
          {
            controller.Show(configuration);
          }
          else
          {
            controller.Update(configuration);
          }
        }
      }
    }
  }
}
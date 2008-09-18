using System;
using System.Collections.Generic;

using DependencyStore.Domain.Configuration;
using DependencyStore.Domain.Configuration.Repositories;
using DependencyStore.Domain.SimpleCopying;
using DependencyStore.Domain.SimpleCopying.Repositories;

namespace DependencyStore.Services.Impl
{
  public class DependencyState
  {
    private readonly ILocationRepository _locationRepository;
    private readonly IConfigurationRepository _configurationRepository;
    private IList<SourceLocation> _sources;
    private IList<SinkLocation> _sinks;
    private LatestFileSet _latestFiles;

    public LatestFileSet LatestFiles
    {
      get { return _latestFiles; }
    }

    public IList<SinkLocation> Sinks
    {
      get { return _sinks; }
    }

    public IList<SourceLocation> Sources
    {
      get { return _sources; }
    }

    public FileSetGroupedByLocation LatestFilesGroupByLocation
    {
      get { return FileSetGroupedByLocation.GroupFileSetIntoLocations(_sources, _latestFiles); }
    }

    public DependencyState(ILocationRepository locationRepository, IConfigurationRepository configurationRepository)
    {
      _locationRepository = locationRepository;
      _configurationRepository = configurationRepository;
    }

    public void Refresh()
    {
      _sinks = _locationRepository.FindAllSinks();
      _sources = _locationRepository.FindAllSources();
      _latestFiles = new LatestFileSet();
      foreach (SourceLocation location in _sources)
      {
        location.AddTo(_latestFiles);
      }
      _latestFiles.SortByModifiedAt();
    }

    public SynchronizationPlan CreatePlanForEverything()
    {
      SynchronizationPlan plan = new SynchronizationPlan();
      foreach (SinkLocation location in _sinks)
      {
        plan.Merge(location.CreateSynchronizationPlan(this.LatestFiles));
      }
      return plan;
    }

    public SynchronizationPlan CreatePlanFor(SinkLocation location)
    {
      return location.CreateSynchronizationPlan(this.LatestFiles);
    }
  }
}
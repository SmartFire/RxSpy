﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using RxSpy.Models;
using System.Reactive.Linq;
using System.Diagnostics;

namespace RxSpy.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public RxSpySessionModel Model { get; set; }

        readonly ObservableAsPropertyHelper<IReadOnlyReactiveList<RxSpyObservableModel>> _trackedObservables;
        public IReadOnlyReactiveList<RxSpyObservableModel> TrackedObservables
        {
            get { return _trackedObservables.Value; }
        }

        RxSpyObservablesGridViewModel _gridViewModel;
        public RxSpyObservablesGridViewModel GridViewModel
        {
            get { return _gridViewModel; }
            set { this.RaiseAndSetIfChanged(ref _gridViewModel, value); }
        }

        readonly ObservableAsPropertyHelper<RxSpyObservableModel> _selectedObservable;
        public RxSpyObservableModel SelectedObservable
        {
            get { return _selectedObservable.Value; }
        }

        readonly ObservableAsPropertyHelper<RxSpyObservableDetailsViewModel> _detailViewModel;
        public RxSpyObservableDetailsViewModel DetailsViewModel
        {
            get { return _detailViewModel.Value; }
        }

        readonly ObservableAsPropertyHelper<double> _signalsPerSecond;
        public double SignalsPerSecond
        {
            get { return _signalsPerSecond.Value; }
        }

        readonly ObservableAsPropertyHelper<long> _signalCount;
        public long SignalCount
        {
            get { return _signalCount.Value; }
        }

        readonly ObservableAsPropertyHelper<long> _errorCount;
        public long ErrorCount
        {
            get { return _errorCount.Value; }
        }


        public MainViewModel(RxSpySessionModel model)
        {
            Model = model;

            this.WhenAnyValue(x => x.Model.TrackedObservables)
                .ToProperty(this, x => x.TrackedObservables, out _trackedObservables);

            GridViewModel = new RxSpyObservablesGridViewModel(model.TrackedObservables);

            this.WhenAnyValue(x => x.GridViewModel.SelectedItem)
                .Select(x => x == null ? null : x.Model)
                .ToProperty(this, x => x.SelectedObservable, out _selectedObservable);

            this.WhenAnyValue(x => x.SelectedObservable)
                .Select(x => x == null ? null : new RxSpyObservableDetailsViewModel(x))
                .ToProperty(this, x => x.DetailsViewModel, out _detailViewModel);

            var throttledSignalCount = this.WhenAnyValue(x => x.Model.SignalCount)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Publish()
                .RefCount();

            throttledSignalCount
                .Scan(Tuple.Create(0L, 0L), (acc, cur) => Tuple.Create(acc.Item2, cur))
                .Select(x => (double)(x.Item2 - x.Item1))
                .ToProperty(this, x => x.SignalsPerSecond, out _signalsPerSecond);

            throttledSignalCount.ToProperty(this, x => x.SignalCount, out _signalCount);

            this.WhenAnyValue(x => x.Model.ErrorCount)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.ErrorCount, out _errorCount);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using RxSpy.ViewModels;

namespace RxSpy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += (s, e) => ViewModel = e.NewValue as MainViewModel;
            DataContext = RxApp.DependencyResolver.GetService<MainViewModel>();

            this.OneWayBind(ViewModel, vm => vm.SpySessionViewModel.GridViewModel, v => v.observablesGrid.DataContext);
        }

        public MainViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as MainViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(MainViewModel),
            typeof(MainWindow),
            new PropertyMetadata(null)
        );

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = value as MainViewModel; }
        }
    }
}

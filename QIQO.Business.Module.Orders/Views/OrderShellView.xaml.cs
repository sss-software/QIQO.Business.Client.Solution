﻿using QIQO.Business.Module.Orders.ViewModels;
using System.Windows.Controls;

namespace QIQO.Business.Module.Orders.Views
{
    /// <summary>
    /// Interaction logic for OrderShellView.xaml
    /// </summary>
    public partial class OrderShellView : UserControl
    {
        public OrderShellView(OrderShellViewModel view_model) //()
        {
            InitializeComponent();
            DataContext = view_model;
            //DataContext = new OrderShellViewModel();
        }
    }
}

﻿using QIQO.Business.Module.Account.ViewModels;
using System.Windows.Controls;

namespace QIQO.Business.Module.Account.Views
{
    /// <summary>
    /// Interaction logic for AccountNavigationView.xaml
    /// </summary>
    public partial class AccountNavigationViewX : UserControl
    {
        public AccountNavigationViewX()
        {
            InitializeComponent();
            DataContext = new AccountNavigationViewModelX();
        }
    }
}

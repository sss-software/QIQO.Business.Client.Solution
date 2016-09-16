﻿using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using QIQO.Business.Client.Core;
using QIQO.Business.Client.Core.UI;
using QIQO.Business.Client.Entities;
using QIQO.Business.Client.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace QIQO.Business.Module.Company.ViewModels
{
    public class ChartOfAccountsViewModel : ViewModelBase, IInteractionRequestAware
    {
        IEventAggregator event_aggregator;
        IServiceFactory service_factory;
        ChartOfAccountWrapper _chart_of_account;
        private string _viewTitle = "Chart of Account Add/Edit";

        private ItemEditNotification notification;
        private Client.Entities.Company _currentCoObject;
        private List<string> _coa_acct_types;
        private List<string> _coa_bal_types;

        public ChartOfAccountsViewModel()
        {
            event_aggregator = Unity.Container.Resolve<IEventAggregator>();
            service_factory = Unity.Container.Resolve<IServiceFactory>();

            SaveCommand = new DelegateCommand(DoSave, CanDoSave);

            _currentCoObject = CurrentCompany as Client.Entities.Company;
            if (_currentCoObject.GLAccounts != null)
            {
               var  _coa = _currentCoObject.GLAccounts;
                _coa_acct_types = new List<string>(_coa.Select(x => x.AccountType).Distinct().OrderBy(x => x).ToList());
                _coa_bal_types = new List<string>(_coa.Select(x => x.BalanceType).Distinct().OrderBy(x => x).ToList());
            }
        }
        public override string ViewTitle { get { return _viewTitle; } }

        public DelegateCommand SaveCommand { get; set; }

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get
            {
                return notification;
            }
            set
            {
                if (value is ItemEditNotification)
                {
                    notification = value as ItemEditNotification;
                    var chart_of_account = notification.EditibleObject as ChartOfAccount;
                    if (chart_of_account != null)
                    {
                        ChartOfAccount = new ChartOfAccountWrapper(chart_of_account);
                        ChartOfAccount.AcceptChanges();
                        ChartOfAccount.PropertyChanged += Context_PropertyChanged;
                        notification.Confirmed = false;
                    }
                    OnPropertyChanged(() => Notification);
                }
            }
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        public ChartOfAccountWrapper ChartOfAccount
        {
            get { return _chart_of_account; }
            private set { SetProperty(ref _chart_of_account, value); }
        }

        public List<string> AccountTypes
        {
            get { return _coa_acct_types; }
        }
        public List<string> BalanceTypes
        {
            get { return _coa_bal_types; }
        }

        private bool CanDoSave()
        {
            if (ChartOfAccount == null) return false;

            return ChartOfAccount.IsChanged && ChartOfAccount.IsValid;
        }

        private void DoSave()
        {
            notification.EditibleObject = ChartOfAccount.Model;
            notification.Confirmed = true;
            FinishInteraction();
            ChartOfAccount.PropertyChanged -= Context_PropertyChanged;
        }
    }
}


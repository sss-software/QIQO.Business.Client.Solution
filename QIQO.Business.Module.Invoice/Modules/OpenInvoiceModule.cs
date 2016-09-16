﻿using Microsoft.Practices.Unity;
using Prism.Regions;
using QIQO.Business.Client.Core;
using QIQO.Business.Client.Core.Infrastructure;
using QIQO.Business.Module.Invoices.Views;

namespace QIQO.Business.Module.Invoices.Modules
{
    public class OpenInvoiceModule : ModuleBase
    {
        public OpenInvoiceModule(IUnityContainer container, IRegionManager region_manager) : base(container, region_manager)
        {
        }

        public override void Initialize()
        {
            //RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(OpenInvoiceView));
            RegionManager.RegisterViewWithRegion(RegionNames.DashboardRegion, typeof(OpenInvoiceView));
        }
    }
}

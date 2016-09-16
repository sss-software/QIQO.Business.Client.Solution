﻿using Prism.Events;
using QIQO.Business.Client.Core;
using QIQO.Business.Client.Core.UI;
using QIQO.Custom.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using QIQO.Business.Client.Contracts;
using QIQO.Business.Client.Entities;
using QIQO.Business.Client.Wrappers;
//using System.Threading.Tasks;
using QIQO.Common.Core.Logging;

namespace QIQO.Business.Module.General.ViewModels
{
    public class CalendarBarViewModel : ViewModelBase
    {
        IEventAggregator event_aggregator;
        private ObservableCollection<QIQODate> _selected_days = new ObservableCollection<QIQODate>();
        private static ObservableCollection<QIQODate> _working_days = new ObservableCollection<QIQODate>();
        private ICurrentCompanyService current_company;
        private IServiceFactory service_factory;

        public CalendarBarViewModel(IEventAggregator event_aggtr, ICurrentCompanyService curr_company, IServiceFactory service_fctry)
        {
            event_aggregator = event_aggtr;
            current_company = curr_company;
            service_factory = service_fctry;

            event_aggregator.GetEvent<CalendarContextChangedEvent>().Subscribe(OnEventDaysChanged, ThreadOption.BackgroundThread);

            CurrentMonthDefaultDate = DateTime.Today;
            NextMonthDefaultDate = CurrentMonthDefaultDate.AddMonths(1);
            ThirdMonthDefaultDate = CurrentMonthDefaultDate.AddMonths(2);

            LoadStandardDeliveryDates();
            GetCompanyOpenOrders();
            GetCompanyOpenInvoices();
            LoadHolidayEvents();

            SelectedDays = _working_days;
        }

        private void OnEventDaysChanged(IEnumerable<QIQODate> obj)
        {
            var sds = obj as ObservableCollection<QIQODate>;
            SelectedDays = _working_days;
        }

        public ObservableCollection<QIQODate> SelectedDays
        {
            get { return _selected_days; }
            set { SetProperty(ref _selected_days, value); Console.WriteLine($"Dates in SelectedDays: {SelectedDays.Count}"); }
        }


        public DateTime CurrentMonthDefaultDate { get; set; }
        public DateTime NextMonthDefaultDate { get; set; }
        public DateTime ThirdMonthDefaultDate { get; set; }

        private void LoadHolidayEvents()
        {
            string line;
            string file_path;

            file_path = Directory.GetCurrentDirectory();
            //Console.WriteLine(file_path);

            try
            {
                StreamReader file = new StreamReader(Path.Combine(file_path, "holidays.txt"));
                while ((line = file.ReadLine()) != null)
                {
                    //Console.WriteLine(line);
                    var fields = line.Split(',');
                    //Console.WriteLine("Holliday Load: " + DateTime.Parse(fields[1]).ToString());
                    _working_days.Add(new QIQODate()
                    {
                        Date = DateTime.Parse(fields[1]),
                        EntityType = "Company",
                        EntityName = current_company.CurrentCompany.CompanyName,
                        BackgroundBrush = Brushes.LightSalmon,
                        DateDescription = $" {fields[2]}",
                        DateType = QIQODateType.Holiday,
                        FontWeight = FontWeights.Bold
                    });
                }
                file.Close();
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex.Message, "holidays.txt is not found");
                MessageBox.Show("File not found exception in the calendar view. Make sure that the 'holidays.txt' file is in the applicaion root directory.",
                    "File Not Found Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "holidays.txt is not found");
                MessageBox.Show("Something odd happened in the calendar view while trying to load the holiday file. Make sure that the 'holidays.txt' file is in the applicaion root directory.",
                    "File Not Found Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GetCompanyOpenOrders()
        {
            IOrderService proxy = service_factory.CreateClient<IOrderService>();
            Company company = new Company() { CompanyKey = CurrentCompanyKey };
            ObservableCollection<OrderWrapper> open_order_col = new ObservableCollection<OrderWrapper>();

            using (proxy)
            {
                //Task<List<Order>> orders = proxy.GetOrdersByCompanyAsync(company);
                List<Order> orders = proxy.GetOrdersByCompany(company);
                //await orders;

                foreach (Order order in orders)
                {
                    QIQODate id = new QIQODate()
                    {
                        Date = (DateTime)order.DeliverByDate,
                        EntityType = "Account",
                        EntityName = order.Account.AccountName,
                        BackgroundBrush = Brushes.LightGreen,
                        DateDescription = $"Due Date\nOrder: {order.OrderNumber}",
                        DateType = QIQODateType.OrderDeliverByDate,
                        FontWeight = FontWeights.Bold
                    };

                    _working_days.Add(id);
                }
            }
        }
        private void GetCompanyOpenInvoices()
        {
            IInvoiceService proxy = service_factory.CreateClient<IInvoiceService>();
            Company company = new Company() { CompanyKey = CurrentCompanyKey };
            ObservableCollection<InvoiceWrapper> open_order_col = new ObservableCollection<InvoiceWrapper>();

            using (proxy)
            {
                //Task<List<Invoice>> invoices = proxy.GetInvoicesByCompanyAsync(company);
                List<Invoice> invoices = proxy.GetInvoicesByCompany(company);
                //await invoices;

                foreach (Invoice invoice in invoices) //invoices.Result
                {
                    QIQODate id = new QIQODate()
                    {
                        Date = DateTime.Parse(invoice.InvoiceEntryDate.AddDays(15).ToString("yyyy-MM-dd")),
                        EntityType = "Account",
                        EntityName = invoice.Account.AccountName,
                        BackgroundBrush = Brushes.LightPink,
                        DateDescription = $"Due Date\nInvoice: {invoice.InvoiceNumber}",
                        DateType = QIQODateType.InvoiceDueDate,
                        FontWeight = FontWeights.Bold
                    };

                    _working_days.Add(id);
                }
            }
        }

        private void LoadStandardDeliveryDates()
        {
            for (int i = 1; i <= 90; i++)
            {
                DateTime dt = DateTime.Today.AddDays(i);
                if (dt.DayOfWeek == DayOfWeek.Monday)
                    _working_days.Add(new QIQODate()
                    {
                        Date = dt,
                        EntityType = "Company",
                        EntityName = CurrentCompanyName,
                        BackgroundBrush = Brushes.LightBlue,
                        DateDescription = "Standard Delivery Date",
                        DateType = QIQODateType.OrderDeliverByDate,
                        FontWeight = FontWeights.Bold
                    });
            }
        }
    }
}

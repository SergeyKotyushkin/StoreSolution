﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Web.UI;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.Mail.Contracts;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : Page
    {
        private const string PageIndexName = "pageIndexNameBasket";
        private const string CurrencyCultureName = "currencyCultureName";
        private const string SmtpSectionPath = "system.net/mailSettings/smtp";

        private static readonly int[] PriceColumnsIndexes = {1, 3};

        private StoreMaster _master;
        private readonly IDbOrderHistoryRepository _dbOrderHistoryRepository;
        private readonly IMailSender _mailSender;
        private readonly IUserGroup _userGroup;
        private readonly ILangSetter _langSetter;
        private readonly ICurrencyCultureService<HttpCookieCollection> _currencyCultureService;
        private readonly IGridViewBasketManager<HttpSessionState> _gridViewBasketManager;

        protected Basket()
            : this(StructureMapFactory.Resolve<IDbOrderHistoryRepository>(), StructureMapFactory.Resolve<IMailSender>(),
                StructureMapFactory.Resolve<IUserGroup>(), StructureMapFactory.Resolve<ILangSetter>(),
                StructureMapFactory.Resolve<ICurrencyCultureService<HttpCookieCollection>>(),
                StructureMapFactory.Resolve<IGridViewBasketManager<HttpSessionState>>())
        {
        }

        protected Basket(IDbOrderHistoryRepository dbOrderHistoryRepository, IMailSender mailSender,
            IUserGroup userGroup, ILangSetter langSetter, ICurrencyCultureService<HttpCookieCollection> currencyCultureService,
            IGridViewBasketManager<HttpSessionState> gridViewBasketManager)
        {
            _dbOrderHistoryRepository = dbOrderHistoryRepository;
            _mailSender = mailSender;
            _userGroup = userGroup;
            _langSetter = langSetter;
            _currencyCultureService = currencyCultureService;
            _gridViewBasketManager = gridViewBasketManager;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Add(cookie);
            }
            Page.Culture = Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster) Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            var user = _userGroup.GetUser();

            SetTitles(user.UserName);

            if (!IsPostBack)
                FillGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser();

            var currencyCulture = _currencyCultureService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName);

            var orderItemsList = _gridViewBasketManager.GetOrderItemsList(Session, currencyCulture).ToArray();

            SaveOrderHistoryInDatabase(orderItemsList, user.UserName, user.Email, currencyCulture.Name);

            SendMailMessage(user.Email, orderItemsList);

            MakePurchase(user.UserName);

            Response.Redirect("~/User/ProductCatalog.aspx");
        }
        
        protected void GV_table_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTable.PageIndex = e.NewPageIndex;

            _gridViewBasketManager.SavePageIndex(Session, PageIndexName, e.NewPageIndex);

            FillGridView();
        }
        
        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            if (_gridViewBasketManager.CheckIsPageIndexNeedToRefresh(Session, PageIndexName, gvTable))
            {
                _gridViewBasketManager.SetGridViewPageIndex(Session, PageIndexName, gvTable);
                FillGridView();
                return;
            }

            _gridViewBasketManager.SetCultureForPriceColumns(gvTable,
                _currencyCultureService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName), false,
                PriceColumnsIndexes);
        }

        protected void gvTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Header) return;

            e.Row.Cells[0].Text = _langSetter.Set("Basket_HeaderName");
            e.Row.Cells[1].Text = _langSetter.Set("Basket_HeaderPrice");
            e.Row.Cells[2].Text = _langSetter.Set("Basket_HeaderCount");
            e.Row.Cells[3].Text = _langSetter.Set("Basket_HeaderTotalPrice");
        }


        private void FillGridView()
        {
            var cultureTo = _currencyCultureService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName);

            var data = _gridViewBasketManager.GetOrderItemsList(Session, cultureTo);

            _gridViewBasketManager.Fill(gvTable, data);

            SetUiProperties(data, cultureTo);
        }

        private void SaveOrderHistoryInDatabase(IEnumerable<OrderItem> orderItemsList, string userName, string userEmail,
            string currencyCultureName)
        {
            _dbOrderHistoryRepository.AddOrUpdate(orderItemsList, userName, userEmail, currencyCultureName);
        }

        private void SendMailMessage(string userEmail, IEnumerable<OrderItem> orderItemsList)
        {
            var @from = ((SmtpSection)ConfigurationManager.GetSection(SmtpSectionPath)).From;
            var mailMessageSubject = _langSetter.Set("Basket_MailMessageSubject");

            _mailSender.Create(@from, userEmail, mailMessageSubject, orderItemsList, true, CultureInfo.CurrentCulture);
            _mailSender.Send();
        }

        private void MakePurchase(string userName)
        {
            Logger.Log.Info(string.Format("Products has bought by user - {0}. {1}", userName, labTotal.Text));
            Session["Bought"] = 1;
            Session["CurrentOrder"] = null;
        }

        private void SetUiProperties(IQueryable<OrderItem> data, IFormatProvider cultureTo)
        {
            labTotal.Text = string.Format(_langSetter.Set("Basket_Total"),
                data.Sum(p => p.Total).ToString("C", cultureTo));

            btnBuy.Enabled = true;
            if (data.Count() != 0) return;

            btnBuy.Enabled = false;
            labTotal.Text = _langSetter.Set("Basket_EmptyOrder");
        }

        private void SetTitles(string userName)
        {
            _master.HlUserText = userName;
        }
    }
}
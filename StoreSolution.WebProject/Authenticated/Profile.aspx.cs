using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.ImageService.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;
using Image = System.Drawing.Image;

namespace StoreSolution.WebProject.Authenticated
{
    public partial class Profile : Page
    {
        private const string PageIndexName = "pageIndexNameProfile";

        private static readonly Color ErrorColor = Color.Red;
        private static readonly Color SuccessColor = Color.DarkGreen;

        private static readonly Regex RgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");

        private StoreMaster _master;
        private readonly IEfPersonRepository _efPersonRepository;
        private readonly IUserGroup _userGroup;
        private readonly ILangSetter _langSetter;
        private readonly IGridViewProfileManager<HttpSessionState> _gridViewProfileManager;
        private readonly IImageService _imageService;

        protected Profile()
            : this(
                StructureMapFactory.Resolve<IEfPersonRepository>(), StructureMapFactory.Resolve<IUserGroup>(),
                StructureMapFactory.Resolve<ILangSetter>(),
                StructureMapFactory.Resolve<IGridViewProfileManager<HttpSessionState>>(),
                StructureMapFactory.Resolve<IImageService>())
        {
        }

        protected Profile(IEfPersonRepository efPersonRepository,
            IUserGroup userGroup, ILangSetter langSetter,
            IGridViewProfileManager<HttpSessionState> gridViewProfileManager, IImageService imageService)
        {
            _efPersonRepository = efPersonRepository;
            _userGroup = userGroup;
            _langSetter = langSetter;
            _gridViewProfileManager = gridViewProfileManager;
            _imageService = imageService;
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
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            SetUiProperties();

            if(!Page.IsPostBack)
                RefereshUser();

            FillGridView();
        }
        
        protected void btnBack_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser();

            Logger.Log.Debug(string.Format("User {0} escaped Profile page.", user.UserName));
            Response.Redirect("~/Index.aspx");
        }

        protected void btnNewIcon_Click(object sender, EventArgs e)
        {
            _master.SetLabMessage(Color.Empty, string.Empty);

            var extension = Path.GetExtension(btnChooseIcon.FileName);
            if (extension == null)
            {
                _master.SetLabMessage(ErrorColor, "Profile_NotImage");
                return;
            }

            if (!btnChooseIcon.HasFile || (extension.ToLower() != ".jpg" && extension.ToLower() != ".jpeg"))
            {
                _master.SetLabMessage(ErrorColor, !btnChooseIcon.HasFile ? "Profile_NotImage" : "Profile_NotJpg");
                return;
            }

            if (btnChooseIcon.FileBytes.Length == 0)
            {
                _master.SetLabMessage(ErrorColor, "Profile_NotImage");
                return;
            }

            var user = _userGroup.GetUser();
            var person = _efPersonRepository.Persons.FirstOrDefault(p => p.Login == user.UserName) ?? new Person
            {
                Login = user.UserName,
                Name = "",
                SecondName = ""
            };

            var image = _imageService.ByteArrayToImage(btnChooseIcon.FileBytes);
            var newImage = (Image) (new Bitmap(image, _imageService.GetSize(image.Size, 500)));

            var updatedPerson = new Person
            {
                Login = person.Login,
                Name = person.Name,
                SecondName = person.SecondName,
                Icon = _imageService.ImageToByteArray(newImage)
            };

            _master.SetLabMessage(Color.Empty, string.Empty);
            if (_efPersonRepository.AddOrUpdate(updatedPerson))
            {
                _master.SetLabMessage(SuccessColor, "Profile_IconWasChanged");
                Logger.Log.Info(string.Format("Icon successfully changed for user - {0}.", user.UserName));
            }
            else _master.SetLabMessage(ErrorColor, "Profile_IconWasNotChanged");

            RefreshIcon(updatedPerson.Icon);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser();

            var person = _efPersonRepository.Persons.FirstOrDefault(p => p.Login == user.UserName);
            if(person == null) return;

            var updatedPerson = new Person
            {
                Login = person.Login,
                Name = tbName.Text,
                SecondName = tbSecondName.Text,
                Icon = person.Icon
            };

            if (!_efPersonRepository.AddOrUpdate(updatedPerson))
            {
                _master.SetLabMessage(ErrorColor, "Profile_IconWasNotChanged");
            }
            else
            {
                _master.SetLabMessage(SuccessColor, "Profile_PersonalDataChanged");
                Logger.Log.Info(string.Format("Personal data successfully commited for user - {0}.", user.UserName));
            }

            RefereshUser();
        }

        protected void btnNewPassword_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            {
                _master.SetLabMessage(ErrorColor, "Profile_BothPasswordsError");
                return;
            }

            if (!RgxPassword.IsMatch(tbNewPassword.Text) || !RgxPassword.IsMatch(tbOldPassword.Text))
            {
                _master.SetLabMessage(ErrorColor, "Profile_PasswordError");
                rfvNewPassword.IsValid = rfvOldPassword.IsValid = false;
                return;
            }

            var user = _userGroup.GetUser();
            if (user.ChangePassword(tbOldPassword.Text, tbNewPassword.Text))
            {
                _master.SetLabMessage(SuccessColor, "Profile_PasswordWasChanged");
                Logger.Log.Info(string.Format("Password successfully changed for user - {0}.", user.UserName));
            }
            else _master.SetLabMessage(ErrorColor, "Profile_PasswordWasChanged");
        }

        protected void gvOrderHistory_OnDataBound(object sender, EventArgs e)
        {
            if (!_gridViewProfileManager.CheckIsPageIndexNeedToRefresh(Session, PageIndexName, gvOrderHistory)) return;

            _gridViewProfileManager.SetGridViewPageIndex(Session, PageIndexName, gvOrderHistory);
            FillGridView();
        }

        protected void gvOrderHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    e.Row.Cells[3].Text =
                        e.Row.Cells[3].Text.Replace(Environment.NewLine, "<br/>")
                            .Replace(HttpContext.Current.Server.HtmlEncode("<b>"), "<b>")
                            .Replace(HttpContext.Current.Server.HtmlEncode("</b>"), "</b>");

                    e.Row.Cells[4].Text = e.Row.Cells[4].Text
                        .Replace(HttpContext.Current.Server.HtmlEncode("<b>"), "<b>")
                        .Replace(HttpContext.Current.Server.HtmlEncode("</b>"), "</b>");
                    break;
                case DataControlRowType.Header:
                    e.Row.Cells[0].Text = _langSetter.Set("Profile_HeaderNumber");
                    e.Row.Cells[1].Text = _langSetter.Set("Profile_HeaderDate");
                    e.Row.Cells[2].Text = _langSetter.Set("Profile_HeaderEmail");
                    e.Row.Cells[3].Text = _langSetter.Set("Profile_HeaderOrder");
                    e.Row.Cells[4].Text = _langSetter.Set("Profile_HeaderTotal");
                    break;
            }
        }

        protected void gvOrderHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrderHistory.PageIndex = e.NewPageIndex;

            _gridViewProfileManager.SavePageIndex(Session, PageIndexName, e.NewPageIndex);

            FillGridView();
        }


        private void RefreshIcon(byte[] iconArray)
        {
            var base64String = Convert.ToBase64String(iconArray, 0, iconArray.Length);
            icon.ImageUrl = "data:image/jpeg;base64," + base64String;
            var image = _imageService.ByteArrayToImage(iconArray);
            var size = _imageService.GetSize(image.Size, 200);
            icon.Width = size.Width;
            icon.Height = size.Height;
        }

        private void RefereshUser()
        {
            _master.SetLabMessage(Color.Empty, string.Empty);

            var user = _userGroup.GetUser();

            labTitle.Text = user.UserName;

            var person = _efPersonRepository.Persons.FirstOrDefault(p => p.Login == user.UserName);

            if (person == null) return;
            tbName.Text = person.Name;
            tbSecondName.Text = person.SecondName;

            if (person.Icon != null)  
                RefreshIcon(person.Icon);
        }

        private void FillGridView()
        {
            var user = _userGroup.GetUser();

            if (Roles.IsUserInRole(user.UserName, "Admin")) labOrderHistory.Visible = false;

            var data = _gridViewProfileManager.GetOrderToGridList(user.UserName);

            _gridViewProfileManager.Fill(gvOrderHistory, data);
        }

        private void SetUiProperties()
        {
            _master.HideMoney();
        }
    }
}
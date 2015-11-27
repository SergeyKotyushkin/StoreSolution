using System;
using System.IO;
using DrawingImage = System.Drawing;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.Authenticated
{
    public partial class Profile : System.Web.UI.Page
    {
        private static readonly Color ErrorColor = Color.Red;
        private static readonly Color SuccessColor = Color.DarkGreen;

        private StoreMaster _master;
        private readonly IEfPersonRepository _efPersonRepository;
        private readonly IEfOrderHistoryRepository _efOrderHistoryRepository;
        private readonly IUserGroup _userGroup;
        private readonly ILangSetter _langSetter;

        protected Profile()
            : this(
                StructureMapFactory.Resolve<IEfPersonRepository>(), StructureMapFactory.Resolve<IEfOrderHistoryRepository>(),
                StructureMapFactory.Resolve<IUserGroup>(), StructureMapFactory.Resolve<ILangSetter>())
        {
        }

        protected Profile(IEfPersonRepository efPersonRepository, IEfOrderHistoryRepository efOrderHistoryRepository,
            IUserGroup userGroup, ILangSetter langSetter)
        {
            _efPersonRepository = efPersonRepository;
            _efOrderHistoryRepository = efOrderHistoryRepository;
            _userGroup = userGroup;
            _langSetter = langSetter;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Add(cookie);
            }
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            _master.HideMoney();

            if(!Page.IsPostBack)
                RefereshUser();

            FillOrdersHistoryTable(true);
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

            var image = ByteArrayToImage(btnChooseIcon.FileBytes);
            var newImage = (DrawingImage.Image) (new Bitmap(image, GetSize(image.Size, 500)));

            var updatedPerson = new Person
            {
                Login = person.Login,
                Name = person.Name,
                SecondName = person.SecondName,
                Icon = ImageToByteArray(newImage)
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

            var rgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");
            if (!rgxPassword.IsMatch(tbNewPassword.Text) || !rgxPassword.IsMatch(tbOldPassword.Text))
            {
                _master.SetLabMessage(ErrorColor, "Profile_PasswordError");
                rfvNewPassword.IsValid = false;
                rfvOldPassword.IsValid = false;
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

        protected void gvOrderHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            { 
                e.Row.Cells[3].Text =
                    e.Row.Cells[3].Text.Replace(Environment.NewLine, "<br/>")
                        .Replace(HttpContext.Current.Server.HtmlEncode("<b>"), "<b>")
                        .Replace(HttpContext.Current.Server.HtmlEncode("</b>"), "</b>");

                e.Row.Cells[4].Text = e.Row.Cells[4].Text
                    .Replace(HttpContext.Current.Server.HtmlEncode("<b>"), "<b>")
                    .Replace(HttpContext.Current.Server.HtmlEncode("</b>"), "</b>");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = _langSetter.Set("Profile_HeaderNumber");
                e.Row.Cells[1].Text = _langSetter.Set("Profile_HeaderDate");
                e.Row.Cells[2].Text = _langSetter.Set("Profile_HeaderEmail");
                e.Row.Cells[3].Text = _langSetter.Set("Profile_HeaderOrder");
                e.Row.Cells[4].Text = _langSetter.Set("Profile_HeaderTotal");
            }
        }

        protected void gvOrderHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrderHistory.PageIndex = e.NewPageIndex;
            gvOrderHistory.DataBind();
        }


        private void RefreshIcon(byte[] iconArray)
        {
            var base64String = Convert.ToBase64String(iconArray, 0, iconArray.Length);
            icon.ImageUrl = "data:image/jpeg;base64," + base64String;
            var image = ByteArrayToImage(iconArray);
            var size = GetSize(image.Size, 200);
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

            if (person.Icon != null)  RefreshIcon(person.Icon);
        }

        private static DrawingImage.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return DrawingImage.Image.FromStream(ms);
            }
        }

        private static byte[] ImageToByteArray(DrawingImage.Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private static Size GetSize(Size size, int bound)
        {
            var newSize = new Size();

            if (size.Width >= size.Height)
            {
                newSize.Width = bound;
                newSize.Height = bound*size.Height/size.Width;
            }
            else
            {
                newSize.Height = bound;
                newSize.Width = bound*size.Width/size.Height;
            }

            return newSize;
        }

        private void FillOrdersHistoryTable(bool bind)
        {
            var user = _userGroup.GetUser();
            var history = _efOrderHistoryRepository.GetAll.Where(u => u.PersonEmail == user.Email).OrderBy(u => u.Date).ToList();

            if (history.Count == 0)
            {
                labOrderHistory.Visible = false;
                return;
            }

            var jss= new JavaScriptSerializer();

            var number = 1;
            var ordersFromHistory = (from h in history
                let productsOrder = jss.Deserialize<ProductsOrder[]>(h.Order)
                select new OrderFromHistory
                {
                    Number = number++, Email = h.PersonEmail, Date = h.Date, ProductsOrder = productsOrder, Total = h.Total, CultureName = h.Culture
                }).ToList();

            var orderToGrid =
                ordersFromHistory.Select(o => new OrderToGrid(o, _langSetter))
                    .Select(o => new { o.Number, o.Date, o.Email, Order = HttpContext.Current.Server.HtmlDecode(o.Order), o.Total })
                    .ToList();

            gvOrderHistory.DataSource = orderToGrid;
            if (bind)
                gvOrderHistory.DataBind();
        }
    }
}
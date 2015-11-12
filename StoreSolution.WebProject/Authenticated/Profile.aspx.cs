using System;
using System.IO;
using System.Web.Security;
using DrawingImage = System.Drawing;
using System.Linq;
using StoreSolution.DatabaseProject.Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.MyIoC;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.Authenticated
{
    public partial class Profile : System.Web.UI.Page
    {
        private StoreMaster _master;
        private readonly IPersonRepository _iPersonRepository;

        protected Profile()
            : this(SimpleContainer.Resolve<IPersonRepository>())
        {
        }

        protected Profile(IPersonRepository iPersonRepository)
        {
            _iPersonRepository = iPersonRepository;
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

            _master.HiddenMoney();

            if(!Page.IsPostBack)
                RefereshUser();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            var user = GetUserOrResponseEnd();
            Logger.Log.Debug(string.Format("User {0} escaped Profile page.", user.UserName));

            Response.Redirect("~/Index.aspx");
        }

        protected void btnNewIcon_Click(object sender, EventArgs e)
        {
            _master.LabMessageText = "";
            _master.LabMessageForeColor = Color.Red;

            var extension = Path.GetExtension(btnChooseIcon.FileName);
            if (extension == null)
            {
                _master.LabMessageText = LangSetter.Set("Profile_NotImage");
                return;
            }

            if (!btnChooseIcon.HasFile || (extension.ToLower() != ".jpg" && extension.ToLower() != ".jpeg"))
            {
                _master.LabMessageText = !btnChooseIcon.HasFile
                    ? LangSetter.Set("Profile_NotImage")
                    : LangSetter.Set("Profile_NotJpg");
                return;
            }

            if (btnChooseIcon.FileBytes.Length == 0)
            {
                _master.LabMessageText = LangSetter.Set("Profile_NotImage");
                return;
            }

            var user = GetUserOrResponseEnd();
            var person = _iPersonRepository.Persons.FirstOrDefault(p => p.Login == user.UserName) ?? new Person
            {
                Login = user.UserName,
                Name = "",
                SecondName = ""
            };

            var image = ByteArrayToImage(btnChooseIcon.FileBytes);
            var newImage = (Image) (new Bitmap(image, GetSize(image.Size, 500)));

            var updatedPerson = new Person
            {
                Login = person.Login,
                Name = person.Name,
                SecondName = person.SecondName,
                Icon = ImageToByteArray(newImage)
            };

            _master.LabMessageText = "";
            if (_iPersonRepository.AddOrUpdate(updatedPerson))
            {
                _master.LabMessageForeColor = Color.DarkGreen;
                _master.LabMessageText = LangSetter.Set("Profile_IconWasChanged");
                Logger.Log.Info(string.Format("Icon successfully changed for user - {0}.", user.UserName));
            }
            else _master.LabMessageText = LangSetter.Set("Profile_IconWasNotChanged");

            RefreshIcon(updatedPerson.Icon);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var user = GetUserOrResponseEnd();

            var person = _iPersonRepository.Persons.FirstOrDefault(p => p.Login == user.UserName);
            if(person == null) return;

            var updatedPerson = new Person
            {
                Login = person.Login,
                Name = tbName.Text,
                SecondName = tbSecondName.Text,
                Icon = person.Icon
            };

            var result = _iPersonRepository.AddOrUpdate(updatedPerson);

            string message;
            if (!result)
            {
                _master.LabMessageForeColor = Color.Red;
                message = LangSetter.Set("Profile_NothingChanged");
            }
            else
            {
                _master.LabMessageForeColor = Color.DarkGreen;
                message = LangSetter.Set("Profile_PersonalDataChanged");
                Logger.Log.Info(string.Format("Personal data successfully commited for user - {0}.", user.UserName));
            }

            RefereshUser();
            _master.LabMessageText = message;
        }

        protected void btnNewPassword_Click(object sender, EventArgs e)
        {
            _master.LabMessageForeColor = Color.Red;

            Page.Validate();
            if (!Page.IsValid)
            {
                _master.LabMessageText = LangSetter.Set("Profile_BothPasswordsError");
                return;
            }

            var rgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");
            if (!rgxPassword.IsMatch(tbNewPassword.Text) || !rgxPassword.IsMatch(tbOldPassword.Text))
            {
                _master.LabMessageText = LangSetter.Set("Profile_PasswordError");
                rfvNewPassword.IsValid = false;
                rfvOldPassword.IsValid = false;
                return;
            }

            var user = GetUserOrResponseEnd();
            if (user.ChangePassword(tbOldPassword.Text, tbNewPassword.Text))
            {
                _master.LabMessageForeColor = Color.DarkGreen;
                _master.LabMessageText = LangSetter.Set("Profile_PasswordWasChanged");
                Logger.Log.Info(string.Format("Password successfully changed for user - {0}.", user.UserName));
            }
            else _master.LabMessageText = LangSetter.Set("Profile_PasswordWasChanged");
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
            _master.LabMessageText = "";

            var user = GetUserOrResponseEnd();

            labTitle.Text = user.UserName;

            var person = _iPersonRepository.Persons.FirstOrDefault(p => p.Login == user.UserName);

            if (person == null) return;
            tbName.Text = person.Name;
            tbSecondName.Text = person.SecondName;

            if (person.Icon == null) return;

            RefreshIcon(person.Icon);
        }

        private MembershipUser GetUserOrResponseEnd()
        {
            var user = Membership.GetUser();
            if (user != null) return user;

            _master.SignOut(true);
            return null;
        }

        private static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        public byte[] ImageToByteArray(Image imageIn)
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

    }
}
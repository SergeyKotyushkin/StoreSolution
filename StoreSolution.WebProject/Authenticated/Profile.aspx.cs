using System;
using System.IO;
using System.Web.Security;
using DrawingImage = System.Drawing;
using System.Linq;
using StoreSolution.DatabaseProject.Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.MyIoC;
using StoreSolution.WebProject.Log4net;

namespace StoreSolution.WebProject.Authenticated
{
    public partial class Profile : System.Web.UI.Page
    {
        private readonly IPersonRepository _iPersonRepository;

        protected Profile()
            : this(SimpleContainer.Resolve<IPersonRepository>())
        {
            
        }

        protected Profile(IPersonRepository iPersonRepository)
        {
            _iPersonRepository = iPersonRepository;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
                RefereshUser();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            var user = GetUser();
            Logger.Log.Debug("User " + user.UserName + " escaped Profile page.");

            FormsAuthentication.RedirectToLoginPage();
        }

        protected void btnNewIcon_Click(object sender, EventArgs e)
        {
            labMessage.Text = "";
            labMessage.ForeColor = Color.Red;

            var extension = Path.GetExtension(btnChooseIcon.FileName);
            if (extension != null)
            {
                if (!btnChooseIcon.HasFile || (extension.ToLower() != ".jpg" &&
                                               extension.ToLower() != ".jpeg"))
                {
                    labMessage.Text = !btnChooseIcon.HasFile
                        ? "Image file was not choosen."
                        : "Only .Jpg image files allowed";
                    return;
                }
            }
            else
            {
                labMessage.Text = "No image.";
                return;
            }

            if (btnChooseIcon.FileBytes.Length == 0)
            {
                labMessage.Text = "Image file is empty.";
                return;
            }

            var user = GetUser();
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

            var result = _iPersonRepository.AddOrUpdate(updatedPerson);

            labMessage.Text = "";
            if (result)
            {
                labMessage.ForeColor = Color.DarkGreen;
                labMessage.Text = "Icon successfully changed.";
                Logger.Log.Info("Icon successfully changed for user - " + user.UserName + ".");
            }
            else
            {
                labMessage.ForeColor = Color.Red;
                labMessage.Text = "Icon has not changed.";
            }
            RefreshIcon(updatedPerson.Icon);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var user = GetUser();

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
                labMessage.ForeColor = Color.Red;
                message = "Nothing changed. Try again.";
            }
            else
            {
                labMessage.ForeColor = Color.DarkGreen;
                message = "Your new data successfully commited.";
                Logger.Log.Info("Personal data successfully commited for user - " + user.UserName + ".");
            }

            RefereshUser();
            labMessage.Text = message;
        }

        protected void btnNewPassword_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            {
                labMessage.ForeColor = Color.Red;
                labMessage.Text = "Enter both passwords.";
                return;
            }

            var rgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");
            if (!rgxPassword.IsMatch(tbNewPassword.Text) || !rgxPassword.IsMatch(tbOldPassword.Text))
            {
                labMessage.ForeColor = Color.Red;
                labMessage.Text =
                    "Password must have letters, numbers and symbols: _,!,@,#,$,%,^,&,*. " +
                    "First symbol must be letter. Length must be more than 5 symbols.";
                rfvNewPassword.IsValid = false;
                rfvOldPassword.IsValid = false;
                return;
            }

            var user = GetUser();
            if (user.ChangePassword(tbOldPassword.Text, tbNewPassword.Text))
            {
                labMessage.ForeColor = Color.DarkGreen;
                labMessage.Text = "Password successfully changed.";
                Logger.Log.Info("Password successfully changed for user - " + user.UserName + ".");
            }
            else
            {
                labMessage.ForeColor = Color.Red;
                labMessage.Text = "Password has not changed.";
            }
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
            labMessage.Text = "";

            var user = GetUser();

            labTitle.Text = user.UserName;

            var person = _iPersonRepository.Persons.FirstOrDefault(p => p.Login == user.UserName);

            if (person == null) return;
            tbName.Text = person.Name;
            tbSecondName.Text = person.SecondName;

            if (person.Icon == null) return;

            RefreshIcon(person.Icon);
        }

        private MembershipUser GetUser()
        {
            var user = Membership.GetUser();
            if (user != null) return user;

            SignOut();
            return null;
        }

        private void SignOut()
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                Logger.Log.Error("No user at Product Management page start.");
                Logger.Log.Error("Sign out.");
            }
            else Logger.Log.Error("User " + user.UserName + " sing out.");

            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            Response.End();
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
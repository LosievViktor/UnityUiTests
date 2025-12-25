using UnityEngine.UIElements;
using System.Collections;

namespace Tests.Pages
{
    public sealed class LoginPage : BasePage
    {
        public LoginPage(UIDocument document) : base(document) {}
        
        private TextField Username => Q<TextField>(Strings.txtUsername);
        private TextField Password => Q<TextField>(Strings.txtPassword);
        private Toggle Remember => Q<Toggle>(Strings.chkRemember);
        private RadioButtonGroup Mode => Q<RadioButtonGroup>(Strings.RadioButtonGroup);
        private Button LoginButton => Q<Button>(Strings.btnLogin);

        private Label ResultMessage => Q<Label>(Strings.lblResult);
        private Label RememberUserMessage => Q<Label>(Strings.lblRememberUser);
        private Label OnlineStatusMessage => Q<Label>(Strings.lblOnlineStatus);
        
        public string Result => ResultMessage.text;
        public string RememberText => RememberUserMessage.text;
        public string ModeText => OnlineStatusMessage.text;
        
        public void EnterUsername(string value) => Username.value = value;
        public void EnterPassword(string value) => Password.value = value;
        public void SetRemember(bool value) => Remember.value = value;
        
        public void SetMode(LoginMode mode)
        {
            Mode.value = (int)mode;
        }
        
        public IEnumerator ClickLogin()
        {
            yield return null;
            LoginButton.Focus();

            using (var e = NavigationSubmitEvent.GetPooled())
            {
                e.target = LoginButton;
                LoginButton.SendEvent(e);
            }
            yield return null;
        }
        
        public IEnumerator Login(string username, string password, 
            bool remember = false, LoginMode mode = LoginMode.Online)
        {
            EnterUsername(username);
            EnterPassword(password);
            SetRemember(remember);
            SetMode(mode);
            yield return ClickLogin();
        }
    }
}
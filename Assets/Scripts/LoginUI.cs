using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

    public class LoginUI : MonoBehaviour
    {
        public const string UsernameId = "username";
        public const string PasswordId = "password";
        public const string LoginButtonId = "loginButton";
        public const string ResultMessageId = "resultMessage";
        public const string RememberUserMessageId = "rememberUserMessage";
        public const string OnlineStatusMessageId = "onlineStatusMessage";
        public const string RememberUserCheckBoxId = "remember";
        public const string OnlineRadioButtonId = "mode";
        
        private List<string> labelsRadio = new  List<string>
        {
          "Online",
          "Offline"
        };
        
        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            var usernameField = root.Q<TextField>(UsernameId);
            var passwordField = root.Q<TextField>(PasswordId);
            
            var checkboxRememberUser = root.Q<Toggle>(RememberUserCheckBoxId);
            var radio = root.Q<RadioButtonGroup>(OnlineRadioButtonId);
            
            var loginButton = root.Q<Button>(LoginButtonId);
           
            var resultLabel = root.Q<Label>(ResultMessageId);
            var rememberUserMessage = root.Q<Label>(RememberUserMessageId);
            var onlineStatusMessage= root.Q<Label>(OnlineStatusMessageId);

            resultLabel.text = string.Empty;
            rememberUserMessage.text = string.Empty;
            onlineStatusMessage.text = string.Empty;

            loginButton.clicked += () =>
            {
                if (AuthService.IsValid(usernameField.value, passwordField.value))
                {
                    rememberUserMessage.text = "Remember user:"+checkboxRememberUser.value;
                    onlineStatusMessage.text = "Mode:"+ labelsRadio[radio.value];
                    resultLabel.text = "Access granted.";
                    resultLabel.RemoveFromClassList("error");
                    resultLabel.AddToClassList("success");
                    Debug.Log("Access granted.");
                }
                else
                {
                    rememberUserMessage.text =  "Remember user:"+checkboxRememberUser.value;
                    onlineStatusMessage.text = "Mode:"+ labelsRadio[radio.value];
                    resultLabel.text = "Access denied.";
                    resultLabel.RemoveFromClassList("success");
                    resultLabel.AddToClassList("error");
                    Debug.Log("Access denied.");
                }
            };
        }
        
    }
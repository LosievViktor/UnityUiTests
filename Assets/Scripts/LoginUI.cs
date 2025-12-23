using UnityEngine;
using UnityEngine.UIElements;

    public class LoginUI : MonoBehaviour
    {
        public const string UsernameId = "username";
        public const string PasswordId = "password";
        public const string LoginButtonId = "loginButton";
        public const string ResultMessageId = "resultMessage";
        
        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            var usernameField = root.Q<TextField>(UsernameId);
            var passwordField = root.Q<TextField>(PasswordId);
            var resultLabel = root.Q<Label>(ResultMessageId);
            var loginButton = root.Q<Button>(LoginButtonId);

            resultLabel.text = string.Empty;

            loginButton.clicked += () =>
            {
                if (AuthService.IsValid(usernameField.value, passwordField.value))
                {
                    resultLabel.text = "Access granted.";
                    resultLabel.RemoveFromClassList("error");
                    resultLabel.AddToClassList("success");
                    Debug.Log("Access granted.");
                }
                else
                {
                    resultLabel.text = "Access denied.";
                    resultLabel.RemoveFromClassList("success");
                    resultLabel.AddToClassList("error");
                    Debug.Log("Access denied.");
                }
            };
        }
    }
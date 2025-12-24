using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using System.Collections;

    public class LoginPlayModeTests
    {
        private const string SceneName = "LoginScene";

        private VisualElement root;
        private TextField username;
        private TextField password;
        private Toggle remember;
        private RadioButtonGroup mode;
        private Button loginButton;
        private Label result;

        private IEnumerator LoadScene()
        {
            SceneManager.LoadScene(SceneName);
            yield return null;
            yield return null; // UI Toolkit needs 2 frames

            var doc = Object.FindFirstObjectByType<UIDocument>();
            root = doc.rootVisualElement;

            username = root.Q<TextField>("username");
            password = root.Q<TextField>("password");
            remember = root.Q<Toggle>("remember");
            mode = root.Q<RadioButtonGroup>("mode");
            loginButton = root.Q<Button>("loginButton");
            result = root.Q<Label>("resultMessage");
        }

        // ---------- SUCCESS LOGIN ----------
        [UnityTest]
        public IEnumerator Successful_Login_Online_RememberOn()
        {
            yield return LoadScene();

            username.value = "Viktor";
            password.value = "password";
            remember.value = true;
            mode.value = 0; // Online
            
            loginButton.SendEvent(new ClickEvent());
            yield return null;
            yield return new WaitForSeconds(1f);
            Assert.AreEqual("Access granted.", result.text);
        }

        // ---------- FAILED LOGIN ----------
        [UnityTest]
        public IEnumerator Failed_Login_Offline_RememberOff()
        {
            yield return LoadScene();

            username.value = "Viktor";
            password.value = "wrong";
            remember.value = false;
            mode.value = 1; // Offline

            loginButton.SendEvent(new ClickEvent());
            yield return null;
            yield return new WaitForSeconds(1f);
            Assert.AreEqual("Access denied.", result.text);
        }
    }

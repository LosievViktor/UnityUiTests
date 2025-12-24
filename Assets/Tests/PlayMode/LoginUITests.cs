using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
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

    [UnitySetUp]
    public IEnumerator SetUp()
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
    
    [UnityTest]
    public IEnumerator Successful_Login_Online_RememberOn()
    {
        username.value = "Viktor";
        password.value = "password";
        remember.value = true;
        mode.value = 0; // Online
        
        yield return Click();

        yield return null;
        Assert.AreEqual("Access granted.", result.text);
    }
    
    [UnityTest]
    public IEnumerator Failed_Login_Offline_RememberOff()
    {
        username.value = "Viktor";
        password.value = "wrong";
        remember.value = false;
        mode.value = 1; // Offline
        
        yield return Click();

        yield return null;
        Assert.AreEqual("Access denied.", result.text);
    }

    private IEnumerator Click()
    {
        yield return new WaitForSeconds(1);
        Debug.Log($"Trying to click login button {loginButton.text}");
        loginButton.Focus();
        
        // TODO - add code for click Login button. 
        
    }
}
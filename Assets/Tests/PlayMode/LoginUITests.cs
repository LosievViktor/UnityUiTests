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
    private Label resultMessage;
    private Label rememberUserMessage;
    private Label onlineStatusMessage;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene(SceneName);
        yield return null;
        yield return null; 

        var doc = Object.FindFirstObjectByType<UIDocument>();
        root = doc.rootVisualElement;

        username = root.Q<TextField>("username");
        password = root.Q<TextField>("password");
        remember = root.Q<Toggle>("remember");
        mode = root.Q<RadioButtonGroup>("mode");
        loginButton = root.Q<Button>("loginButton");
        resultMessage = root.Q<Label>("resultMessage");
        rememberUserMessage = root.Q<Label>("rememberUserMessage");
        onlineStatusMessage  = root.Q<Label>("onlineStatusMessage"); 
    }
    
    [UnityTest]
    public IEnumerator Success_Login()
    {
        username.value = "Viktor";
        password.value = "password";
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access granted.", resultMessage.text);
        Assert.AreEqual("Remember user:False",rememberUserMessage.text);
        Assert.AreEqual("Mode:Online",onlineStatusMessage.text);
    }
    
    [UnityTest]
    public IEnumerator Fail_Login()
    {
        username.value = "Viktor";
        password.value = "wrong";
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access denied.", resultMessage.text);
        Assert.AreEqual("Remember user:False",rememberUserMessage.text);
        Assert.AreEqual("Mode:Online",onlineStatusMessage.text);
    }
    
    [UnityTest]
    public IEnumerator Success_Login_RememberUser()
    {
        username.value = "Viktor";
        password.value = "password";
        remember.value = true;
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access granted.", resultMessage.text);
        Assert.AreEqual("Remember user:True",rememberUserMessage.text);
        Assert.AreEqual("Mode:Online",onlineStatusMessage.text);
    }
    
    [UnityTest]
    public IEnumerator Fail_Login_RememberUser()
    {
        username.value = "Viktor";
        password.value = "wrong";
        remember.value = true;
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access denied.", resultMessage.text);
        Assert.AreEqual("Remember user:True",rememberUserMessage.text);
        Assert.AreEqual("Mode:Online",onlineStatusMessage.text);
    }
    
    [UnityTest]
    public IEnumerator Success_Login_RememberUser_OfflineMode()
    {
        username.value = "Viktor";
        password.value = "password";
        mode.value = 1;
        remember.value = true;
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access granted.", resultMessage.text);
        Assert.AreEqual("Remember user:True",rememberUserMessage.text);
        Assert.AreEqual("Mode:Offline",onlineStatusMessage.text);
    }
    
    [UnityTest]
    public IEnumerator Fail_Login_RememberUser_OfflineMode()
    {
        username.value = "Viktor";
        password.value = "wrong";
        mode.value = 1;
        remember.value = true;
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access denied.", resultMessage.text);
        Assert.AreEqual("Remember user:True",rememberUserMessage.text);
        Assert.AreEqual("Mode:Offline",onlineStatusMessage.text);
    }
    
    [UnityTest]
    public IEnumerator Success_Login_OfflineMode()
    {
        username.value = "Viktor";
        password.value = "password";
        mode.value = 1;
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access granted.", resultMessage.text);
        Assert.AreEqual("Remember user:False",rememberUserMessage.text);
        Assert.AreEqual("Mode:Offline",onlineStatusMessage.text);
    }
    
    [UnityTest]
    public IEnumerator Fail_Login_OfflineMode()
    {
        username.value = "Viktor";
        password.value = "wrong";
        mode.value = 1;
        yield return ButtonClick(loginButton);
        yield return null;
        Assert.AreEqual("Access denied.", resultMessage.text);
        Assert.AreEqual("Remember user:False",rememberUserMessage.text);
        Assert.AreEqual("Mode:Offline",onlineStatusMessage.text);
    }
    

    private IEnumerator ButtonClick(Button button)
    {
        yield return null;
        Debug.Log($"Trying to click login button {button.text}");
        button.Focus();
        
        using (var e = NavigationSubmitEvent.GetPooled())
        {
            e.target = button;
            button.SendEvent(e);
        }
        yield return null;
    }
}
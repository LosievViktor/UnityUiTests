using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Tests.Pages;
using System.Collections;
using UnityEngine.UIElements;

public class LoginPlayModeTests
{
    private const string SceneName = Strings.SceneName;
    private LoginPage loginPage;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene(SceneName);
        yield return null;
        yield return null;
        
        loginPage = new LoginPage(Object.FindFirstObjectByType<UIDocument>());
    }

    [UnityTest]
    public IEnumerator Success_Login()
    {
        yield return loginPage.Login(username: Strings.Username, password: Strings.Password);
        
        Assert.AreEqual("Access granted.", loginPage.Result);
        Assert.AreEqual("Remember user:False", loginPage.RememberText);
        Assert.AreEqual("Mode:Online", loginPage.ModeText);
    }

    [UnityTest]
    public IEnumerator Fail_Login()
    {
        yield return loginPage.Login(username: Strings.Username, password: Strings.WrongPassword);
        
        Assert.AreEqual("Access denied.", loginPage.Result);
        Assert.AreEqual("Remember user:False", loginPage.RememberText);
        Assert.AreEqual("Mode:Online", loginPage.ModeText);
    }

    [UnityTest]
    public IEnumerator Success_Login_RememberUser()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.Password, remember: true);

        Assert.AreEqual("Access granted.", loginPage.Result);
        Assert.AreEqual("Remember user:True", loginPage.RememberText);
        Assert.AreEqual("Mode:Online", loginPage.ModeText);
    }

    [UnityTest]
    public IEnumerator Fail_Login_RememberUser()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.WrongPassword, remember: true);

        Assert.AreEqual("Access denied.", loginPage.Result);
        Assert.AreEqual("Remember user:True", loginPage.RememberText);
        Assert.AreEqual("Mode:Online", loginPage.ModeText);
    }

    [UnityTest]
    public IEnumerator Success_Login_RememberUser_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.Password, remember: true, 
                mode: LoginMode.Offline);

        Assert.AreEqual("Access granted.", loginPage.Result);
        Assert.AreEqual("Remember user:True", loginPage.RememberText);
        Assert.AreEqual("Mode:Offline", loginPage.ModeText);
    }

    [UnityTest]
    public IEnumerator Fail_Login_RememberUser_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.WrongPassword, remember: true, 
                mode: LoginMode.Offline);

        Assert.AreEqual("Access denied.", loginPage.Result);
        Assert.AreEqual("Remember user:True", loginPage.RememberText);
        Assert.AreEqual("Mode:Offline", loginPage.ModeText);
    }

    [UnityTest]
    public IEnumerator Success_Login_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.Password, 
                mode:LoginMode.Offline);

        Assert.AreEqual("Access granted.", loginPage.Result);
        Assert.AreEqual("Remember user:False", loginPage.RememberText);
        Assert.AreEqual("Mode:Offline", loginPage.ModeText);
    }

    [UnityTest]
    public IEnumerator Fail_Login_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.WrongPassword, 
                mode: LoginMode.Offline);

        Assert.AreEqual("Access denied.", loginPage.Result);
        Assert.AreEqual("Remember user:False", loginPage.RememberText);
        Assert.AreEqual("Mode:Offline", loginPage.ModeText);
    }
}
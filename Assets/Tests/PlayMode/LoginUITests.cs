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

        Assert.AreEqual("Access granted.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.Password}' is correct," +
            $" so the label must show Access granted.");

        Assert.AreEqual("Remember user:False", loginPage.RememberText,
            "Remember checkbox was left unchecked, so the label must be Remember user:False.");

        Assert.AreEqual("Mode:Online", loginPage.ModeText,
            "Online radiobutton was selected, so the label must be Mode:Online.");
    }

    [UnityTest]
    public IEnumerator Fail_Login()
    {
        yield return loginPage.Login(username: Strings.Username, password: Strings.WrongPassword);

        Assert.AreEqual("Access denied.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.WrongPassword}' is wrong," +
            $" so the label must show Access denied.");

        Assert.AreEqual("Remember user:False", loginPage.RememberText,
            "Remember checkbox was left unchecked, so the label must be Remember user:False.");

        Assert.AreEqual("Mode:Online", loginPage.ModeText,
            "Online radiobutton was selected, so the label must be Mode:Online.");
    }

    [UnityTest]
    public IEnumerator Success_Login_RememberUser()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.Password, remember: true);

        Assert.AreEqual("Access granted.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.Password}' is correct, " +
            $"so the label must show Access granted.");

        Assert.AreEqual("Remember user:True", loginPage.RememberText,
            "Remember checkbox was checked, so the label must be Remember user:True.");

        Assert.AreEqual("Mode:Online", loginPage.ModeText,
            "Online radiobutton was selected, so the label must be Mode:Online.");
    }

    [UnityTest]
    public IEnumerator Fail_Login_RememberUser()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.WrongPassword, remember: true);

        Assert.AreEqual("Access denied.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.WrongPassword}' is wrong," +
            $" so the label must show Access denied.");

        Assert.AreEqual("Remember user:True", loginPage.RememberText,
            "Remember checkbox was checked, so the label must be Remember user:True.");

        Assert.AreEqual("Mode:Online", loginPage.ModeText,
            "Online radiobutton was selected, so the label must be Mode:Online.");
    }

    [UnityTest]
    public IEnumerator Success_Login_RememberUser_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.Password, remember: true,
                mode: LoginMode.Offline);

        Assert.AreEqual("Access granted.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.Password}' is correct, " +
            $"so the label must show Access granted.");

        Assert.AreEqual("Remember user:True", loginPage.RememberText,
            "Remember checkbox was checked, so the label must be Remember user:True.");

        Assert.AreEqual("Mode:Offline", loginPage.ModeText,
            "Offline radiobutton was selected, so the label must be Mode:Offline.");
    }

    [UnityTest]
    public IEnumerator Fail_Login_RememberUser_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.WrongPassword, remember: true,
                mode: LoginMode.Offline);

        Assert.AreEqual("Access denied.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.WrongPassword}' is wrong, " +
            $"so the label must show Access denied.");

        Assert.AreEqual("Remember user:True", loginPage.RememberText,
            "Remember checkbox was checked, so the label must be Remember user:True.");

        Assert.AreEqual("Mode:Offline", loginPage.ModeText,
            "Offline radiobutton was selected, so the label must be Mode:Offline.");
    }

    [UnityTest]
    public IEnumerator Success_Login_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.Password,
                mode: LoginMode.Offline);

        Assert.AreEqual("Access granted.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.Password}' is correct, " +
            $"so the label must show Access granted.");

        Assert.AreEqual("Remember user:False", loginPage.RememberText,
            "Remember checkbox was left unchecked, so the label must be Remember user:False.");

        Assert.AreEqual("Mode:Offline", loginPage.ModeText,
            "Offline radiobutton was selected, so the label must be Mode:Offline.");
    }

    [UnityTest]
    public IEnumerator Fail_Login_OfflineMode()
    {
        yield return loginPage
            .Login(username: Strings.Username, password: Strings.WrongPassword,
                mode: LoginMode.Offline);

        Assert.AreEqual("Access denied.", loginPage.Result,
            $"Login '{Strings.Username}' is correct and password '{Strings.WrongPassword}' is wrong, " +
            $"so the label must show Access denied.");

        Assert.AreEqual("Remember user:False", loginPage.RememberText,
            "Remember checkbox was left unchecked, so the label must be Remember user:False.");

        Assert.AreEqual("Mode:Offline", loginPage.ModeText,
            "Offline radiobutton was selected, so the label must be Mode:Offline.");
    }
}
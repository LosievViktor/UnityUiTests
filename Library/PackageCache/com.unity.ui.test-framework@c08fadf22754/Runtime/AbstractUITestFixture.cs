using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Creates reusable test components that define
    /// custom behavior at specific phases of the test lifecycle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implement the <see cref="Initialize(AbstractUITestFixture)"/>, <see cref="BeforeTest()"/>,
    /// <see cref="AfterTest()"/>, <see cref="Shutdown()"/> as needed.
    /// </para>
    /// <para>
    /// You can activate and deactivate test components:
    /// <list type="bullet">
    /// <item>
    /// <description>To activate the test component, use <see cref="CommonUITestFixture.AddTestComponent(UITestComponent)"/>.</description>
    /// </item>
    /// <item>
    /// <description>To deactivate the test component, use <see cref="CommonUITestFixture.RemoveTestComponent(UITestComponent)"/>.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public abstract class UITestComponent
    {
        /// <summary>
        /// The test fixture to which the `UITestComponent` is attached.
        /// </summary>
        protected internal AbstractUITestFixture fixture { get;
            [System.Obsolete("For Internal Use Only.")]
            internal set; }

        /// <summary>
        /// Invoked when the
        /// <see cref="AbstractUITestFixture.AddTestComponent(UITestComponent)"/> function is called.
        /// </summary>
        /// <param name="testFixture">The test fixture to which the test component is added.</param>
        protected virtual void Initialize(AbstractUITestFixture testFixture)
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            fixture = testFixture;
#pragma warning restore CS0618
        }

        /// <summary>
        /// Runs when the <see cref="AbstractUITestFixture.RemoveTestComponent(UITestComponent)"/> function is called.
        /// </summary>
        protected virtual void Shutdown() { }

        /// <summary>
        /// Runs when the
        /// <see cref="AbstractUITestFixture.AddTestComponent(UITestComponent)"/> function is called.
        /// </summary>
        protected virtual void BeforeTest() { }

        /// <summary>
        /// Runs when the
        /// <see cref="AbstractUITestFixture.RemoveTestComponent(UITestComponent)"/> function is called.
        /// </summary>
        protected virtual void AfterTest() { }

        [System.Obsolete("For Internal Use Only.")]
        internal void DoInitialize(AbstractUITestFixture testFixture) => Initialize(testFixture);
        [System.Obsolete("For Internal Use Only.")]
        internal void DoShutdown() => Shutdown();
        [System.Obsolete("For Internal Use Only.")]
        internal void DoBeforeTest() => BeforeTest();
        [System.Obsolete("For Internal Use Only.")]
        internal void DoAfterTest() => AfterTest();
    }

    /// <summary>
    /// For unit testing purposes only.
    /// This interface wrapper around NUnit is required
    /// in order to fake the test status when testing the test framework.
    /// </summary>
    [System.Obsolete("For Internal Use Only.")]
    internal interface ITestStatusProvider
    {
        bool hasTestFailed { get; }
        string testName { get; }
    }

    /// <summary>
    /// Common interface for UI test fixtures.
    /// Manages NUnit's lifetime and provides a `PanelSimulator` to simulate interactions with UI Toolkit content.
    /// Provides component-based extensibility for reusable utilities.
    /// Actual usage done via <see cref="UITestFixture"/>,
    /// <see cref="T:UnityEditor.UIElements.TestFramework.EditorWindowUITestFixture`1"/>,
    /// or <see cref="RuntimeUITestFixture"/>.
    /// </summary>
    public abstract class AbstractUITestFixture
    {
        // Inheritance by test classes is prohibited.
        [System.Obsolete("For Internal Use Only.")]
        internal AbstractUITestFixture() { }

        /// <summary>
        /// Internal enum used for tracking the state of the test.
        /// </summary>
        [System.Obsolete("For Internal Use Only.")]
        internal enum LifetimeState
        {
            Default,
            Initialized,
            DuringTest,
            Suspended,
            Disposed,
        }

        [System.Obsolete("For Internal Use Only.")]
        internal abstract LifetimeState lifetimeState { get; }

        /// <summary>
        /// Adds the <paramref name="component"/> to the test fixture.
        /// </summary>
        /// <param name="component">The <see cref="UITestComponent"/> to add to the test fixture.</param>
        /// <remarks>
        /// Adding the <paramref name="component"/> to the test fixture triggers relevant
        /// <see cref="UITestComponent"/> virtual methods based on the current test state.
        /// </remarks>
        public abstract void AddTestComponent(UITestComponent component);

        /// <summary>
        /// Removes the <paramref name="component"/> from the test fixture.
        /// </summary>
        /// <param name="component">The `UITestComponent` to remove from the test fixture.</param>
        /// <remarks>
        /// Removing the <paramref name="component"/> from the test fixture triggers relevant
        /// <see cref="UITestComponent"/> virtual methods based on the current test state.
        /// </remarks>
        public abstract void RemoveTestComponent(UITestComponent component);

        /// <summary>
        /// Creates and adds a `UITestComponent` to the test fixture.
        /// </summary>
        /// <typeparam name="T">The type of `UITestComponent` to attach to the test fixture.</typeparam>
        /// <returns>The added test component.</returns>
        public T AddTestComponent<T>() where T : UITestComponent, new()
        {
            var component = new T();
            AddTestComponent(component);
            return component;
        }
        /// <summary>
        /// Finds and removes the first component of type <typeparamref name="T"/> from the test fixture.
        /// </summary>
        /// <typeparam name="T">The type of `UITestComponent` to search for and remove.</typeparam>
        /// <remarks>
        /// Does nothing if a test component of the specified type <typeparamref name="T"/> is not found.
        /// </remarks>
        public void RemoveTestComponent<T>() where T : UITestComponent
        {
            var component = FindTestComponent<T>();
            if (component != null)
            {
                RemoveTestComponent(component);
            }
        }

        /// <summary>
        /// Returns the first component of type <typeparamref name="T"/> attached to the test fixture.
        /// </summary>
        /// <typeparam name="T">The type of `UITestComponent` to search for.</typeparam>
        /// <returns>The first component of type <typeparamref name="T"/>.</returns>
        public abstract T FindTestComponent<T>();

        /// <summary>
        /// Returns the `PanelSimulator` used by the test fixture.
        /// </summary>
        /// <remarks>
        /// Use this property to interact with the simulated panel.
        /// </remarks>
        public abstract PanelSimulator simulate { get; set; }

        /// <inheritdoc cref="PanelSimulator.panel"/>
        public IPanel panel => simulate?.panel;

        /// <inheritdoc cref="PanelSimulator.rootVisualElement"/>
        public VisualElement rootVisualElement => simulate?.rootVisualElement;

        /// <inheritdoc cref="PanelSimulator.panelName"/>
        public string panelName => simulate?.panelName;

        /// <summary>
        /// Set to `true` if the tested elements require `ImmediateModeElement` rendering or
        /// an `IMGUIContainer`'s `OnGUI()` logic that requires a repaint event.
        /// Defaults to `false`.
        /// </summary>
        public bool needsRendering
        {
            get => simulate.needsRendering;
            set => simulate.needsRendering = value;
        }

        /// <summary>
        /// The pixels per point scaling factor of the panel.
        /// Defaults to `1`.
        /// </summary>
        public float pixelsPerPoint
        {
            get => simulate.pixelsPerPoint;
            set => simulate.pixelsPerPoint = value;
        }

        /// <summary>
        /// The size of the `rootVisualElement` of the panel.
        /// </summary>
        public abstract Vector2 panelSize { get; set; }

        /// <summary>
        /// Clears the `rootVisualElement` after each test when set to `true`.
        /// </summary>
        public abstract bool clearContentAfterTest { get; set; }

        /// <summary>
        /// Theme style sheet used by this test fixture.
        /// </summary>
        /// <remarks>
        /// Defaults to `null`.
        /// When the value is `null`, the style applied is the default theme style sheet.
        /// </remarks>
        public abstract ThemeStyleSheet themeStyleSheet { get; set; }

        /// <summary>
        /// Property which determines whether the window should remain open at the end of a failed test for debugging purposes.
        /// Default is false (off).
        /// </summary>
        /// <remarks>
        /// Set this property to `true` if you want an editor window to remain open for manual debugging after a test failure. Tests in playmode will pause execution
        /// Should be set to `false` when running tests in batch mode.
        /// </remarks>
        public abstract bool debugMode { get; set; }

        #region NUnit lifetime methods

        /// <summary>
        /// Sets up the test fixture.
        /// </summary>
        [OneTimeSetUp]
        public abstract void FixtureOneTimeSetUp();

        /// <summary>
        /// Tears down the test fixture.
        /// </summary>
        [OneTimeTearDown]
        public abstract void FixtureOneTimeTearDown();

        /// <summary>
        /// Sets up the test using coroutines.
        /// </summary>
        /// <returns>An IEnumerator for yield instructions</returns>
        [UnitySetUp]
        public abstract IEnumerator FixtureUnitySetUp();

        /// <summary>
        /// Sets up the test.
        /// </summary>
        [SetUp]
        public abstract void FixtureSetUp();

        /// <summary>
        /// Tears down the test.
        /// </summary>
        [TearDown]
        public abstract void FixtureTearDown();

        /// <summary>
        /// Tears down the test using coroutines.
        /// </summary>
        /// <returns>An IEnumerator for yield instructions</returns>
        [UnityTearDown]
        public abstract IEnumerator FixtureUnityTearDown();

        #endregion

        /// <summary>
        /// Releases the currently simulated UI Toolkit panel.
        /// </summary>
        public abstract void ReleasePanel();

        /// <summary>
        /// Recreates the simulated UI Toolkit panel, providing a fresh instance.
        /// </summary>
        public abstract void RecreatePanel();
    }
}

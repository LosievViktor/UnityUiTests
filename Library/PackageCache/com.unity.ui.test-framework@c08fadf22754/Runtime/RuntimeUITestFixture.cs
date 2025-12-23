using System;
using System.Collections;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
	/// Test fixture base class that creates a runtime panel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this class to write Play mode tests for UI Toolkit content.
    /// </para>
    /// <para>
    /// By default, the runtime panel uses the default runtime theme.
    /// To customize the theme during tests, set <see cref="CommonUITestFixture.themeStyleSheet"/>.
    /// The test fixture creates an empty runtime panel with no `UIDocument` attached.
    /// To provide your own UI content for testing, call <see cref="SetUIContent(UIDocument)"/>.
    /// </para>
    /// </remarks>
    public abstract class RuntimeUITestFixture : CommonUITestFixture
    {
        [System.Obsolete("For Internal Use Only.")]
        internal RuntimePanelSimulator runtimeSimulate;

        private Vector2 m_PanelSize = PanelSimulator.GetDefaultPanelSize();

        private PanelSettings m_PanelSettings;

        private bool m_OwnsUIDocument = true;
        private UIDocument m_AttachedUI;
        private GameObject m_GameObject;

        /// <summary>
        /// The default theme style sheet to be used when default PanelSettings are created during tests.
        /// </summary>
        [System.Obsolete("For Internal Use Only.")]
        internal static ThemeStyleSheet defaultThemeStyleSheet { get; set; }

        /// <summary>
        /// Instantiates a new empty `RuntimeUITestFixture`.
        /// </summary>
        protected RuntimeUITestFixture()
#pragma warning disable CS0618 // Disable warning on Internal usage
            : base()
#pragma warning restore CS0618
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            runtimeSimulate = new RuntimePanelSimulator();
            simulate = runtimeSimulate;
#pragma warning restore CS0618
            SetUIContent(null);
        }

        /// <summary>
        /// Assigns the specified `UIDocument` to be used by the test fixture.
        /// </summary>
        /// <param name="doc">The `UIDocument` component to host the test content.</param>
        public void SetUIContent(UIDocument doc)
        {
            if (doc == null)
            {
                m_OwnsUIDocument = true;
                return;
            }

            DestroyOwnedObjects();
            m_OwnsUIDocument = false;
            clearContentAfterTest = false;
            m_AttachedUI = doc;
            m_PanelSettings = doc.panelSettings;

            if (m_PanelSettings == null)
            {
                throw new InvalidOperationException("The provided UIDocument does not have a PanelSettings assigned. Please assign a PanelSettings to the UIDocument before using it in tests.");
            }

            m_GameObject = doc.gameObject;
            AssignPanel();
        }

        private void AssignPanel()
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            runtimeSimulate.AssignPanel(m_PanelSettings);
            runtimeSimulate.SetRootVisualElement(m_AttachedUI.rootVisualElement);
#pragma warning restore CS0618
        }

        /// <inheritdoc cref="AbstractUITestFixture.panelSize"/>
        public sealed override Vector2 panelSize
        {
            get => m_PanelSize;
            set
            {
                if (m_PanelSize != value)
                {
                    m_PanelSize = value;
                    ApplyPanelSize();
                }
            }
        }

        /// <summary>
        /// Applies the <see cref="panelSize"/> to the panel.
        /// </summary>
        public void ApplyPanelSize()
        {
            if (panel != null)
            {
                rootVisualElement.SetSize(m_PanelSize);
            }
        }

        #region NUnit lifetime methods

        /// <inheritdoc cref="AbstractUITestFixture.FixtureOneTimeSetUp"/>
        public override void FixtureOneTimeSetUp()
        {
            RecreatePanel();
            base.FixtureOneTimeSetUp();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureOneTimeTearDown"/>
        public override void FixtureOneTimeTearDown()
        {
            base.FixtureOneTimeTearDown();
            ReleasePanel();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureSetUp"/>
        public override void FixtureSetUp()
        {
            if (panel != null)
            {
                ApplyPanelSize();
            }

            base.FixtureSetUp();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureTearDown"/>
        public override void FixtureTearDown()
        {
#if UNITY_EDITOR
#pragma warning disable CS0618 // Disable warning on Internal usage
            if (testStatus.hasTestFailed && debugMode)
#pragma warning restore CS0618
            {
                //Let's pause
                UnityEditor.EditorApplication.isPaused = true;

#pragma warning disable CS0618 // Disable warning on Internal usage
                DisplayNotification($"Test failed: {testStatus.testName}. Playmode execution paused.");
#pragma warning restore CS0618
            }
            else
#endif
            if (clearContentAfterTest)
            {
                rootVisualElement.Clear();
            }

            base.FixtureTearDown();
        }

        #if UNITY_EDITOR
        void DisplayNotification(string msg)
        {
            var mainWindow =  UnityEditor.EditorWindow.focusedWindow;
            if (mainWindow != null)
            {
                mainWindow.ShowNotification(new GUIContent(msg));
            }
            else
            {
                Debug.Log(msg);
            }
        }
        #endif

        /// <inheritdoc cref="AbstractUITestFixture.FixtureUnityTearDown()"/>
        public override IEnumerator FixtureUnityTearDown()
        {
#if UNITY_EDITOR
#pragma warning disable CS0618 // Disable warning on Internal usage
            if (testStatus.hasTestFailed && debugMode)
#pragma warning restore CS0618
            {
                while (UnityEditor.EditorApplication.isPaused && UnityEditor.EditorApplication.isPlaying)
                    yield return null;
#pragma warning disable CS0618 // Disable warning on Internal usage
                UnsuspendTestExecution();
#pragma warning restore CS0618
            }
#endif

            yield return base.FixtureUnityTearDown();
        }

        #endregion

        /// <summary>
        /// Recreates the simulated UI Toolkit panel, providing a fresh instance.
        /// If you set a custom `UIDocument` using <see cref="SetUIContent(UIDocument)"/>,
        /// it disables the associated GameObject and re-enables it instead of recreating.
        /// </summary>
        public override void RecreatePanel()
        {
            if (!m_OwnsUIDocument)
            {
                // When not in control, we simply disable and re-enable the UIDocument.
                m_GameObject.SetActive(false);
                m_GameObject.SetActive(true);
            }
            else
            {
                CreateDefaultUIScene();
            }
            AssignPanel();
        }

        /// <summary>
        /// Sets up a GameObject with an empty `UIDocument`.
        /// Use <see cref="CommonUITestFixture.themeStyleSheet"/> to set the style sheet.
        /// To use your own UI content
        /// during the test, use <see cref="SetUIContent(UIDocument)"/>.
        /// </summary>
        public void CreateDefaultUIScene()
        {
            DestroyOwnedObjects();

#pragma warning disable CS0618 // Disable warning on Internal usage
            var theme = m_ThemeStyleSheet;
#pragma warning restore CS0618

            if (theme == null)
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                var defaultTheme = defaultThemeStyleSheet;
#pragma warning restore CS0618

                if (defaultTheme == null)
                {
                    // Let's try to find one if already loaded
                    var allThemes = Resources.FindObjectsOfTypeAll(typeof(ThemeStyleSheet));

                    if (allThemes.Length == 0)
                    {
                        // Might be a bit much as it can be slow on large projects
                        allThemes = Resources.LoadAll("", typeof(ThemeStyleSheet));
                    }

                    if (allThemes.Length > 0)
                    {
#pragma warning disable CS0618 // Disable warning on Internal usage
                        defaultThemeStyleSheet = defaultTheme = allThemes[0] as ThemeStyleSheet;
#pragma warning restore CS0618
                    }
                }

                if (defaultTheme == null)
                {
                    throw new Exception($"No ThemeStyleSheet found in the project. Please load one and assign it to {nameof(AbstractUITestFixture)}.{nameof(themeStyleSheet)}");
                }

                theme = defaultTheme;
            }

            m_PanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            m_PanelSettings.themeStyleSheet = theme;

            m_GameObject = new GameObject("m_AttachedUI");
            m_AttachedUI = m_GameObject.AddComponent<UIDocument>();

            m_PanelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            m_PanelSettings.scale = 1;
            m_PanelSettings.sortingOrder = 0;
            // Assign and apply the panel settings.
            m_AttachedUI.panelSettings = m_PanelSettings;

            m_OwnsUIDocument = true;
        }

        /// <summary>
        /// Destroys created scene components and the `UIDocument`.
        /// </summary>
        public override void ReleasePanel()
        {
            DestroyOwnedObjects();
#pragma warning disable CS0618 // Disable warning on Internal usage
            runtimeSimulate.AssignPanel(null);
#pragma warning restore CS0618
        }

        void DestroyOwnedObjects()
        {
            if (m_OwnsUIDocument)
            {
                Object.DestroyImmediate(m_AttachedUI);
                Object.DestroyImmediate(m_GameObject);

                Object.DestroyImmediate(m_PanelSettings);

                m_AttachedUI = null;
                m_GameObject = null;
                m_PanelSettings = null;
            }
        }
    }
}

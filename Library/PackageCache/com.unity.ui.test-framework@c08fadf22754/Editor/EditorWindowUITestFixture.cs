using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;
using Object = UnityEngine.Object;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// Test fixture base class that creates an [EditorWindow](xref:UnityEditor.EditorWindow).
    /// </summary>
    /// <remarks>
    /// Inherit from this class when your UI is defined in the [CreateGUI](xref:EditorWindow.CreateGUI) method,
    /// or when your window contains IMGUI content that requires a GUIView context.
    /// If you don't require an actual `EditorWindow` and an Editor panel is sufficient,
    /// use <see cref="UITestFixture"/> instead.
    /// </remarks>
    /// <typeparam name="EditorWindowType">The type of `EditorWindow` to create with this test fixture.</typeparam>
    public abstract class EditorWindowUITestFixture<EditorWindowType> : CommonUITestFixture where EditorWindowType : EditorWindow
    {
        EditorWindowPanelSimulator editorWindowPanelSimulator;

        /// <summary>
        /// `EditorWindow` created by the test fixture. Null if no window is created.
        /// </summary>
        /// <remarks>
        /// The window is reused for the entire duration of the test fixture except if
        /// a test fails. In that case, the next test creates a new window.
        /// </remarks>
        protected EditorWindowType window { get; private set; }

        /// <summary>
        /// Creates a new window.
        /// </summary>
        /// <remarks>
        /// If not set, the `EditorWindow` instance is created using <see cref="ScriptableObject.CreateInstance(Type)"/>.
        /// Use it to designate an alternate window creation function.
        /// Called when the test fixture starts or the window is null at test start.
        /// </remarks>
        public Func<EditorWindowType> createWindowFunction { get; set; }

        /// <summary>
        /// Releases the window.
        /// </summary>
        /// <remarks>
        /// By default, if this function is not set, the `EditorWindow` instance is `Closed()` and destroyed.
        /// Use this function to designate an alternate window release function.
        /// This function is called at the end of the test fixture,
        /// or at the end of a test if it has failed.
        /// </remarks>
        public Action<EditorWindowType> releaseWindowFunction { get; set; }

        /// <summary>
        /// Whether to call <see cref="EditorWindowPanelSimulator.FrameUpdate"/>
        /// within an IMGUI context. Defaults to `false`.
        /// </summary>
        [System.Obsolete("IMGUI is not fully supported.")]
        internal bool needsImprovedIMGUISupport
        {
            #pragma warning disable CS0618 // IMGUI is not fully supported
            get => editorWindowPanelSimulator.needsImprovedIMGUISupport;
            set => editorWindowPanelSimulator.needsImprovedIMGUISupport = value;
            #pragma warning restore CS0618
        }

        private bool m_PanelSizeSet = false;
        private Vector2 m_PanelSize = EditorPanelSimulator.GetDefaultPanelSize();

        /// <summary>
        /// The size of the `rootVisualElement` of the panel.
        /// </summary>
        public sealed override Vector2 panelSize
        {
            get => m_PanelSize;
            set
            {
                m_PanelSizeSet = true;
                m_PanelSize = value;
                ApplyPanelSize();
            }
        }

        private Vector2 effectivePanelSize => new Vector2(m_PanelSize.x, m_PanelSize.y + (DockArea.kTabHeight + DockArea.kFloatingWindowTopBorderWidth));

        /// <summary>
        /// Instantiates an empty `EditorWindowUITestFixture`.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="CommonUITestFixture.simulate"/> property to access the UI Toolkit panel.
        /// Use the <see cref="window"/> property to access the window.
        /// </remarks>
        protected EditorWindowUITestFixture()
#pragma warning disable CS0618 // Disable warning on Internal usage
            : base()
#pragma warning restore CS0618
        {
            editorWindowPanelSimulator = new EditorWindowPanelSimulator(null);
            simulate = editorWindowPanelSimulator;
            clearContentAfterTest = false;
        }

        /// <summary>
        /// Executes the given command within a valid IMGUI context.
        /// </summary>
        /// <param name="command">The `Action` to be executed.</param>
        [System.Obsolete("IMGUI is not fully supported.")]
        internal void ExecuteWithinIMGUIContext(System.Action command)
        {
            #pragma warning disable CS0618 // IMGUI is not fully supported
            editorWindowPanelSimulator.ExecuteWithinIMGUIContext(command);
            #pragma warning restore CS0618
        }

        /// <summary>
        /// Sets up the test fixture.
        /// Creates and sets up the <see cref="window"/>.
        /// </summary>
        public override void FixtureOneTimeSetUp()
        {
            if (window == null)
                RecreatePanel();

            base.FixtureOneTimeSetUp();
        }

        /// <summary>
        /// Sets up the test.
        /// Creates and initializes the <see cref="window"/> if it is null.
        /// </summary>
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            RecreateWindowIfNecessary();
        }

        /// <summary>
        /// Tears down the test.
        /// When a test fails, closes and recreates the <see cref="window"/> from scratch.
        /// When <see cref="AbstractUITestFixture.debugMode"/> is set to `true`, windows of failed tests are left open.
        /// </summary>
        public override void FixtureTearDown()
        {
            base.FixtureTearDown();
#pragma warning disable CS0618 // Disable warning on Internal usage
            if (testStatus.hasTestFailed)
#pragma warning restore CS0618
            {
                if (debugMode)
                {
                    // If the test failed but we are in debug mode, we don't want to recreate the window
                    // so that the user can interact with it and diagnose the issues.
                    window.disableInputEvents = false;
                }
                else
                {
                    // We recreate the window to avoid any failure side-effects in the next test.
                    RecreatePanel();
                }
            }
            else if (clearContentAfterTest)
            {
                rootVisualElement.Clear();
                // Clear might not be enough, RegisterCallbacks, classnames, or other state could still be dangling.
            }
        }

        /// <summary>
        /// Tears down the test fixture.
        /// When a test fails and <see cref="AbstractUITestFixture.debugMode"/> is set to `true`, windows are left open.
        /// </summary>
        public override void FixtureOneTimeTearDown()
        {
            base.FixtureOneTimeTearDown();

            if (window != null)
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                if (debugMode && lifetimeState == LifetimeState.Suspended)
#pragma warning restore CS0618
                {
#pragma warning disable CS0618 // Disable warning on Internal usage
                    rootVisualElement.RegisterCallback<DetachFromPanelEvent>((evt) => ShutdownAfterSuspension());
#pragma warning restore CS0618
                    // We simply remove the PanelSimulator
                    DisconnectWindow();
                }
                else
                {
                    ReleasePanel();
                }
            }
        }

        void RecreateWindowIfNecessary()
        {
            if (window == null)
            {
                RecreatePanel();
            }
        }

        private void DisconnectWindow()
        {
            if (window != null)
            {
                editorWindowPanelSimulator.SetWindow(null);
                window = null;
            }
        }

        /// <summary>
        /// Releases the panel.
        /// Calls <see cref="releaseWindowFunction"/> if it's set;
        /// otherwise, closes and destroys the <see cref="window"/>.
        /// </summary>
        public override void ReleasePanel()
        {
            if (window != null)
            {
                var currentWin = window;

                DisconnectWindow();

                if (releaseWindowFunction != null)
                {
                    releaseWindowFunction(currentWin);
                }
                else
                {
                    currentWin.Close();
                    Object.DestroyImmediate(currentWin);
                }
            }
        }

        /// <summary>
        /// Recreates the panel.
        /// Calls <see cref="createWindowFunction"/> if it's set;
        /// otherwise, creates the <see cref="window"/> using <see cref="ScriptableObject.CreateInstance(Type)"/>.
        /// </summary>
        public override void RecreatePanel()
        {
            ReleasePanel();

            if (createWindowFunction != null)
            {
                window = createWindowFunction();
            }
            else
            {
                window = ScriptableObject.CreateInstance<EditorWindowType>();
            }

            window.Show();
            editorWindowPanelSimulator.SetWindow(window);

            if (themeStyleSheet != null)
            {
                var panelRoot = panel.visualTree;
                panelRoot.styleSheets.Clear();
                panelRoot.styleSheets.Add(themeStyleSheet);
            }
            ApplyPanelSize();
        }

        void ApplyPanelSize()
        {
            if (window != null && m_PanelSizeSet)
            {
                Rect winPos = window.position;
                winPos.size = effectivePanelSize;
                window.position = winPos;
            }
        }
    }
}

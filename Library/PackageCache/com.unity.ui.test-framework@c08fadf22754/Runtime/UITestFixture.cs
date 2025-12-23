using System;
using System.Collections;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Test fixture base class that creates a UI Toolkit panel without an <c>EditorWindow</c>.
    /// </summary>
    /// <remarks>
    /// Use this class when you can decouple the UI Toolkit content from an actual <c>EditorWindow</c>.
    /// To test with an <c>EditorWindow</c>, use <see cref="T:UnityEditor.UIElements.TestFramework.EditorWindowUITestFixture`1"/>.
    /// To temporarily create an <c>EditorWindow</c> while developing or debugging tests,
    /// enable debugging mode by using <see cref="UITestFixture(bool)"/>.
    /// In Play mode, this fixture runs as a <see cref="RuntimeUITestFixture"/> with a default runtime theme.
    /// Use <see cref="CommonUITestFixture.themeStyleSheet"/> to set the stylesheet.
    /// </remarks>
    public abstract class UITestFixture : AbstractUITestFixture
    {
        private readonly CommonUITestFixture m_CurrentFixture;

        [System.Obsolete("For Internal Use Only.")]
        internal CommonUITestFixture currentFixture => m_CurrentFixture;


        [System.Obsolete("For Internal Use Only.")]
        internal static Func<CommonUITestFixture> s_EditorDefaultFixtureCreation { get; set; }

        [System.Obsolete("For Internal Use Only.")]
        internal static Func<CommonUITestFixture> s_EditorDebugFixtureCreation { get; set; }

        [System.Obsolete("For Internal Use Only.")]
        internal sealed override LifetimeState lifetimeState => m_CurrentFixture.lifetimeState;

        /// <inheritdoc cref="AbstractUITestFixture.simulate"/>
        public sealed override PanelSimulator simulate { get => m_CurrentFixture.simulate; set => m_CurrentFixture.simulate = value; }

        /// <inheritdoc cref="AbstractUITestFixture.panelSize"/>
        public sealed override Vector2 panelSize { get => m_CurrentFixture.panelSize; set => m_CurrentFixture.panelSize = value; }

        /// <inheritdoc cref="AbstractUITestFixture.clearContentAfterTest"/>
        public sealed override bool clearContentAfterTest { get => m_CurrentFixture.clearContentAfterTest; set => m_CurrentFixture.clearContentAfterTest = value; }

        /// <inheritdoc cref="AbstractUITestFixture.themeStyleSheet"/>
        public sealed override ThemeStyleSheet themeStyleSheet { get => m_CurrentFixture.themeStyleSheet; set => m_CurrentFixture.themeStyleSheet = value; }

        /// <inheritdoc cref="AbstractUITestFixture.debugMode"/>
        public sealed override bool debugMode { get => m_CurrentFixture.debugMode; set => m_CurrentFixture.debugMode = value; }

        /// <summary>
        /// The type of fixture to create during instantiation.
        /// </summary>
        public enum FixtureType
        {
            /// <summary>
            /// Automatically detects the type of fixture to create.
            /// </summary>
            /// <remarks>
            /// It uses the Assembly and whether the test is currently 
            /// in Play mode to decide the fixture type.
            /// </remarks>
            AutoDetect,

            /// <summary>
            /// Editor fixture type for tests around an Editor panel.
            /// </summary>
            Editor,

            /// <summary>
            /// Runtime fixture type for tests around a runtime panel.
            /// </summary>
            Runtime
        }

        class PlayModeTestFixture : RuntimeUITestFixture { }

        /// <summary>
        /// Instantiates a `UITestFixture` using the <see cref="FixtureType.AutoDetect"/> functionality.
        /// </summary>
        protected UITestFixture() : this(FixtureType.AutoDetect) { }

        /// <summary>
        /// Instantiates a `UITestFixture` using the <see cref="FixtureType.AutoDetect"/> functionality.
        /// </summary>
        /// <param name="debugMode">Enables debugging for tests.</param>
        protected UITestFixture(bool debugMode) : this(FixtureType.AutoDetect, debugMode) { }

        /// <summary>
        /// Instantiates a `UITestFixture` for the supplied <paramref name="fixtureType"/>.
        /// </summary>
        /// <param name="fixtureType">The type of test fixture to create.</param>
        /// <param name="debugMode">Enables debugging for tests.</param>
        /// <exception cref="InvalidOperationException">Throws when trying to create an Editor `UITestFixture` in the wrong setting.</exception>
        protected UITestFixture(FixtureType fixtureType, bool debugMode = false)
#pragma warning disable CS0618 // Disable warning on Internal usage
            : base()
#pragma warning restore CS0618
        {
            if (ShouldCreateEditorFixture(fixtureType))
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                if (debugMode)
                {
                    if (s_EditorDebugFixtureCreation == null)
                    {
                        throw new InvalidOperationException("Unable to create Editor DebugUITestFixture.");
                    }
                    m_CurrentFixture = s_EditorDebugFixtureCreation();
                }
                else if (s_EditorDefaultFixtureCreation != null)
                {
                    m_CurrentFixture = s_EditorDefaultFixtureCreation();
                }
                else
                {
                    throw new InvalidOperationException("Unable to create Editor UITestFixture.");
                }
            }
            else
            {
                m_CurrentFixture = new PlayModeTestFixture();
            }
#pragma warning restore CS0618

            this.debugMode = debugMode;
        }

        bool ShouldCreateEditorFixture(FixtureType fixtureType)
        {
            switch (fixtureType)
            {
                case FixtureType.Editor:
                    return true;
                case FixtureType.Runtime:
                    return false;
                case FixtureType.AutoDetect:
                    return !IsRunningPlayModeTest();
                default:
                    throw new ArgumentOutOfRangeException(nameof(fixtureType), fixtureType, null);
            }
        }

        private static bool IsRunningPlayModeTest()
        {
#if UNITY_EDITOR
            return Application.isPlaying;
#else
            return true; // Always playmode outside editor
#endif
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureOneTimeSetUp"/>
        public override void FixtureOneTimeSetUp()
        {
            m_CurrentFixture.FixtureOneTimeSetUp();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureOneTimeTearDown"/>
        public override void FixtureOneTimeTearDown()
        {
            m_CurrentFixture.FixtureOneTimeTearDown();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureSetUp"/>
        public override void FixtureSetUp()
        {
            m_CurrentFixture.FixtureSetUp();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureTearDown"/>
        public override void FixtureTearDown()
        {
            m_CurrentFixture.FixtureTearDown();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureUnitySetUp"/>
        public override IEnumerator FixtureUnitySetUp()
        {
            return m_CurrentFixture.FixtureUnitySetUp();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureUnityTearDown"/>
        public override IEnumerator FixtureUnityTearDown()
        {
            return m_CurrentFixture.FixtureUnityTearDown();
        }

        /// <inheritdoc cref="AbstractUITestFixture.RecreatePanel"/>
        public sealed override void RecreatePanel()
        {
            m_CurrentFixture?.RecreatePanel();
        }

        /// <inheritdoc cref="AbstractUITestFixture.ReleasePanel"/>
        public sealed override void ReleasePanel()
        {
            m_CurrentFixture?.ReleasePanel();
        }

        /// <inheritdoc cref="AbstractUITestFixture.AddTestComponent(UITestComponent)"/>
        public sealed override void AddTestComponent(UITestComponent component)
        {
#pragma warning disable CS0618 // Disable warning on Internal usage

            m_CurrentFixture.AddTestComponent(component, this);
#pragma warning restore CS0618

        }

        /// <inheritdoc cref="AbstractUITestFixture.RemoveTestComponent(UITestComponent)"/>
        public sealed override void RemoveTestComponent(UITestComponent component)
        {
            m_CurrentFixture.RemoveTestComponent(component);
        }

        /// <inheritdoc cref="Overload:UnityEngine.UIElements.TestFramework.AbstractUITestFixture.FindTestComponent"/>
        public sealed override T FindTestComponent<T>()
        {
            return m_CurrentFixture.FindTestComponent<T>();
        }
    }
}

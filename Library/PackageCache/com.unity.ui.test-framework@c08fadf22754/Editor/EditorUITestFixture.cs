using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// Test fixture base class that creates a UI Toolkit panel without requiring an [EditorWindow](xref:UnityEditor.EditorWindow).
    /// </summary>
    /// <remarks>
    /// Use this class when you can decouple the tested UI Toolkit content from an actual `EditorWindow`.
    /// <note>
    /// <list>
    /// <item>
    /// If an `EditorWindow` is required for the test, use <see cref="EditorWindowUITestFixture{EditorWindowType}"/> instead.
    /// </item>
    /// <item>
    /// To temporarily create an `EditorWindow` while developing or debugging tests, enable debugging mode by
    /// using <see cref="UITestFixture(bool)/>.
    /// </item>
    /// </list>
    /// </remarks>
    [InitializeOnLoad]
    internal sealed class EditorUITestFixture : CommonUITestFixture
    {
        static EditorUITestFixture()
        {
            // This ensures that the EditorUITestFixture is used in edit mode tests.
            // It allows the UITestFixture to use this fixture without needing to have explicit dependency on this assembly
#pragma warning disable CS0618 // Disable warning on Internal usage
            UITestFixture.s_EditorDefaultFixtureCreation = () => new EditorUITestFixture();
#pragma warning restore CS0618
        }


        EditorPanelSimulator panelSimulator;
        /// <summary>
        /// Instantiates a blank UI Toolkit panel that can be populated within the test class.
        /// </summary>
        /// <remarks>Use the <see cref="CommonUITestFixture.simulate"/> property to access the UI Toolkit panel.</remarks>
        public EditorUITestFixture()
#pragma warning disable CS0618 // Disable warning on Internal usage
            : base()
#pragma warning restore CS0618
        {
            panelSimulator = new EditorPanelSimulator();
            simulate = panelSimulator;
        }

        /// <summary>
        /// The size of the `rootVisualElement` of the panel.
        /// </summary>
        public sealed override Vector2 panelSize
        {
            get => panelSimulator.panelSize;
            set => panelSimulator.panelSize = value;
        }

        /// <summary>
        /// Sets up the test.
        /// Sets up the UI Toolkit panel and applies the default stylesheet.
        /// </summary>
        public override void FixtureSetUp()
        {
            if (panel != null)
            {
                var panelRoot = panel.visualTree;

#pragma warning disable CS0618 // Disable warning on Internal usage
                var theme = m_ThemeStyleSheet;
#pragma warning restore CS0618

                if (theme != null)
                {
                    if (!panelRoot.styleSheets.Contains(theme))
                    {
                        panelRoot.styleSheets.Add(theme);
                    }
                }
                else
                {
                    // If no theme is set, use the default editor stylesheet
                    UIElementsEditorUtility.AddDefaultEditorStyleSheets(panelRoot);
                }
                StyleCache.ClearStyleCache();
                panelSimulator.ApplyPanelSize();
            }

            base.FixtureSetUp();
        }

        /// <summary>
        /// Tears down the test.
        /// When <see cref="ClearContentAfterTest"/> is `true`, clears the <see cref="CommonUITestFixture.rootVisualElement"/>.
        /// </summary>
        public override void FixtureTearDown()
        {
            if (rootVisualElement != null && clearContentAfterTest)
            {
                rootVisualElement.Clear();
                // Clear might not be enough, RegisterCallbacks, inline styles or other state could still be dangling
                rootVisualElement.ClearClassList();
                rootVisualElement.styleSheets.Clear();
                rootVisualElement.Unbind();
                simulate.FrameUpdate();
            }

            base.FixtureTearDown();
        }


        /// <summary>
        /// Tears down the test fixture.
        /// Disposes of the panel and releases its resources.
        /// </summary>
        public override void FixtureOneTimeTearDown()
        {
            base.FixtureOneTimeTearDown();
            panelSimulator.Dispose();
            simulate = null;
        }

        /// <summary>
        /// Releases the panel.
        /// </summary>
        /// <remarks>Use this to close the panel tracked by the test.</remarks>
        public override void ReleasePanel()
        {
            panelSimulator.ReleasePanel();
        }

        /// <summary>
        /// Recreates the panel.
        /// </summary>
        /// <remarks>Use this to recreate a clean version of the panel.</remarks>
        public override void RecreatePanel()
        {
            panelSimulator.RecreatePanel();
        }
    }
}

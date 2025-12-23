using UnityEngine;
using UnityEngine.UIElements.TestFramework;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// This class is instantiated instead of <see cref="UITestFixture"/>
    /// when debugging is enabled for an Editor test.
    /// It keeps an Editor window visible while developing or debugging tests.
    /// </summary>
    [InitializeOnLoad]
    internal abstract class DebugUITestFixture : EditorWindowUITestFixture<EditorWindow>
    {
        static DebugUITestFixture()
        {
            // This ensures that the DebugUITestFixture is used in edit mode tests.
            // It allows the UITestFixture to use this fixture without needing to have explicit dependency on this assembly
#pragma warning disable CS0618 // Disable warning on Internal usage
            UITestFixture.s_EditorDebugFixtureCreation = () => new InvocableDebugUITestFixture();
#pragma warning restore CS0618
        }

        class InvocableDebugUITestFixture : DebugUITestFixture { }

        /// <summary>
        /// Instantiates an <see cref="EditorWindowUITestFixture{EditorWindowType}"/> 
        /// and sets <see cref="EditorWindowUITestFixture{EditorWindowType}.debugMode"/> to `true`.
        /// </summary>
        /// <remarks>
        /// To access the UI Toolkit panel, use the <see cref="CommonUITestFixture.simulate"/> property.
        /// </remarks>
        protected DebugUITestFixture()
        {
            debugMode = true;
            clearContentAfterTest = true;
        }

        /// <summary>
        /// Sets up the test state.
        /// </summary>
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

#pragma warning disable CS0618 // Disable warning on Internal usage
            if (!testStatus.hasTestFailed && window != null)
            {
                window.titleContent = new GUIContent(testStatus.testName);
            }
#pragma warning restore CS0618
        }

        /// <summary>
        /// Tears down the test state.
        /// If a test fails, the Editor window is left open to facilitate debugging.
        /// </summary>
        public override void FixtureTearDown()
        {
            base.FixtureTearDown();

#pragma warning disable CS0618 // Disable warning on Internal usage
            if (testStatus.hasTestFailed)
            {
                // Leave the state of the window as-is in order to let user see what went wrong.
                if (window != null)
                {
                    window.titleContent = new GUIContent("Test Failed: " + testStatus.testName);
                    window.disableInputEvents = false;
                }
            }
#pragma warning restore CS0618
        }
    }
}

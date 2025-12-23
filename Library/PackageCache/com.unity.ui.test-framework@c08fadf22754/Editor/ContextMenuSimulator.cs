using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// Provides a testable wrapper for context menu interactions in UI Toolkit tests.
    /// Allows simulation and verification of context menu display, item selection, and menu content
    /// without invoking the native system menu.
    /// </summary>
    /// <remarks>
    /// For an example usage, refer to [Simulate context menu actions](xref:simulate-context-menu-actions).
    /// </remarks>
#pragma warning disable CS0618 // Disable warning on Internal usage
    public sealed class ContextMenuSimulator : MenuSimulator
#pragma warning restore CS0618
    {
        TestEditorContextualMenuManager m_MenuManager;
        ContextualMenuManager m_OriginalManager;

        /// <summary>
        /// Internal lifecycle method invoked automatically by the test fixture.
        /// </summary>
        protected override void BeforeTest()
        {
            base.BeforeTest();

            Panel p = fixture.panel as Panel;

            // Set up the test context menu manager
            m_OriginalManager = p.contextualMenuManager;

            if (m_MenuManager == null)
            {
                m_MenuManager = new TestEditorContextualMenuManager(this);
            }

            p.contextualMenuManager = m_MenuManager;
        }

        /// <summary>
        /// Internal lifecycle method invoked automatically by the test fixture.
        /// </summary>
        protected override void AfterTest()
        {
            Panel p = fixture.panel as Panel;
            p.contextualMenuManager = m_OriginalManager;
            base.AfterTest();
        }

        internal class TestEditorContextualMenuManager : EditorContextualMenuManager
        {
            ContextMenuSimulator m_Owner;

            public TestEditorContextualMenuManager(ContextMenuSimulator owner)
            {
                this.m_Owner = owner;
            }

            protected internal override void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent)
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                m_Owner.SetMenuContent(menu);
#pragma warning restore CS0618
            }
        }

        // For internal testing purposes
        internal static object GetContextualMenuManager(IPanel p)
        {
            if (p is Panel panel)
            {
                return panel.contextualMenuManager;
            }
            return null;
        }

    }
}

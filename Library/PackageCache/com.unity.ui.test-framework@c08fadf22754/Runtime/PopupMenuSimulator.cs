using System;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Provides a testable wrapper for both Editor and runtime popup menu interactions in UI Toolkit tests.
    /// Allows simulation and verification of popup menu display, item selection, and menu content
    /// without invoking the native system menu.
    /// </summary>
#pragma warning disable CS0618 // Disable warning on Internal usage
    public sealed class PopupMenuSimulator : MenuSimulator
#pragma warning restore CS0618
    {

        /// <summary>
        /// The position of the menu.
        /// </summary>
        public Rect position { get; private set; }

        /// <summary>
        /// The element used to determine the menu's root.
        /// </summary>
        public VisualElement targetElement { get; private set; }

        /// <summary>
        /// `True` if the menu is anchored, `false` otherwise.
        /// </summary>
        public bool anchored {  get; private set; }

        Func<AbstractGenericMenu> m_OriginalCreateMenuFunctor;

        #pragma warning disable CS0618 // Disable warning on Internal usage
        TestPopupMenu m_SimulatedMenu;
        #pragma warning restore CS0618

        /// <inheritdoc/>
        protected override void BeforeTest()
        {
            base.BeforeTest();

            Panel p = fixture.panel as Panel;

            m_OriginalCreateMenuFunctor = p.CreateMenuFunctor;
            #pragma warning disable CS0618 // Disable warning on Internal usage
            p.CreateMenuFunctor = () => new TestPopupMenu(this);
            #pragma warning restore CS0618
        }

        /// <inheritdoc/>
        protected override void AfterTest()
        {
            DiscardMenu();

            #pragma warning disable CS0618 // Disable warning on Internal usage
            Panel p = fixture.panel as Panel;
            #pragma warning restore CS0618

            p.CreateMenuFunctor = m_OriginalCreateMenuFunctor;
            m_OriginalCreateMenuFunctor = null;

            base.AfterTest();
        }

        /// <inheritdoc/>
        public override void DiscardMenu()
        {
            base.DiscardMenu();

            m_SimulatedMenu = null;
            position = Rect.zero;
            targetElement = null;
            anchored = false;
        }

        #pragma warning disable CS0618 // Disable warning on Internal usage
        private void DoDisplayMenu(TestPopupMenu popup, Rect position, VisualElement targetElement, bool anchored)
        {
            SetMenuContent(popup.menu);
            m_SimulatedMenu = popup;
            this.position = position;
            this.targetElement = targetElement;
            this.anchored = anchored;
        }
        #pragma warning restore CS0618

        [Obsolete("For Internal Use Only.")]
        internal class TestPopupMenu : AbstractGenericMenu
        {
            public DropdownMenu menu;
            PopupMenuSimulator m_PopupMenuSimulator;

            public TestPopupMenu(PopupMenuSimulator popupSimulator)
            {
                m_PopupMenuSimulator = popupSimulator;
                menu = new DropdownMenu();
            }

            private void AddItem(string itemName, bool isChecked, bool isEnabled, Action action, Action<object> actionUserData, object data)
            {
                DropdownMenuAction.Status s = DropdownMenuAction.Status.None;
                if (isChecked)
                {
                    s |= DropdownMenuAction.Status.Checked;
                }
                if (!isEnabled)
                {
                    s |= DropdownMenuAction.Status.Disabled;
                }
                if (s == DropdownMenuAction.Status.None)
                {
                    s = DropdownMenuAction.Status.Normal;
                }

                menu.AppendAction(itemName,
                    (a) => { action?.Invoke(); actionUserData?.Invoke(data); },
                    (a) => s,
                    data);
            }

            public override void AddDisabledItem(string itemName, bool isChecked)
            {
                AddItem(itemName, isChecked, false, null, null, null);
            }

            public override void AddItem(string itemName, bool isChecked, Action action)
            {
                AddItem(itemName, isChecked, true, action, null, null);
            }

            public override void AddItem(string itemName, bool isChecked, Action<object> action, object data)
            {
                AddItem(itemName, isChecked, true, null, action, data);
            }

            public override void AddSeparator(string path)
            {
                menu.AppendSeparator(path);
            }

            public override void DropDown(Rect position, VisualElement targetElement, DropdownMenuSizeMode dropdownMenuSizeMode)
            {
                m_PopupMenuSimulator.DoDisplayMenu(this, position, targetElement,
                    dropdownMenuSizeMode == DropdownMenuSizeMode.Auto || dropdownMenuSizeMode == DropdownMenuSizeMode.Fixed);
            }
        }

        // For internal testing purposes
        [Obsolete("For Internal Use Only.")]
        internal static object GetCreateMenuFunctor(IPanel p)
        {
            if (p is Panel panel)
            {
                return panel.CreateMenuFunctor;
            }
            return null;
        }
    }
}

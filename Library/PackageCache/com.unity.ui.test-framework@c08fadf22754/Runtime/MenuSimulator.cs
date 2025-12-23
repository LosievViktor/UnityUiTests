using System.Collections.Generic;
using NUnit.Framework;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Base class for components related to menu simulation.
    /// </summary>
    public abstract class MenuSimulator : UITestComponent
    {
        /// <summary>
        /// Forbid external direct inheritance of MenuSimulator class.
        /// MenuSimulator is exclusively intended as a base class.
        /// </summary>
        [System.Obsolete("For Internal Use Only.")]
        internal MenuSimulator() { }

        /// <summary>
        /// Whether the context menu was displayed.
        /// </summary>
        #pragma warning disable CS0618 // Disable warning on Internal usage
        public bool menuIsDisplayed => displayedMenu != null;
        #pragma warning restore CS0618

        /// <summary>
        /// The number of items currently in the displayed context menu.
        /// Returns `0` if no menu is displayed.
        /// </summary>
        #pragma warning disable CS0618 // Disable warning on Internal usage
        public int menuItemCount { get => displayedMenu != null ? displayedMenu.MenuItems().Count : 0; }
        #pragma warning restore CS0618

        [System.Obsolete("For Internal Use Only.")]
        internal DropdownMenu displayedMenu { get; private set; }

        /// <inheritdoc/>
        protected override void BeforeTest()
        {
            DiscardMenu();
        }

        /// <inheritdoc/>
        protected override void AfterTest()
        {
            DiscardMenu();
        }

        /// <summary>
        /// Resets relevant `MenuSimulator` properties.
        /// Resets the menu state, hides the menu, and clears any menu data.
        /// </summary>
        public virtual void DiscardMenu()
        {
            #pragma warning disable CS0618 // Disable warning on Internal usage
            displayedMenu = null;
            #pragma warning restore CS0618
        }

        [System.Obsolete("For Internal Use Only.")]
        internal void SetMenuContent(DropdownMenu menu)
        {
            #pragma warning disable CS0618 // Disable warning on Internal usage
            if (displayedMenu != null)
            #pragma warning restore CS0618
            {
                Assert.Fail("Menu content has already been set. Use Reset() to clear the menu before setting new content.");
            }
            else
            {
                #pragma warning disable CS0618 // Disable warning on Internal usage
                displayedMenu = menu;
                #pragma warning restore CS0618
            }
        }

        /// <summary>
        /// Simulates the selection of a menu item by index.
        /// Executes the associated action if the menu is displayed, the <paramref name="itemIndex"/> is valid and the action is not disabled.
        /// </summary>
        /// <param name="itemIndex">The zero-based index of the menu item to select.</param>
        /// <returns>`True` if the action was executed; `false` otherwise.</returns>
        public bool SimulateItemSelection(int itemIndex)
        {
            if (!menuIsDisplayed)
            {
                return false;
            }
            if (itemIndex < 0 || itemIndex >= menuItemCount)
            {
                return false;
            }

            #pragma warning disable CS0618 // Disable warning on Internal usage
            List<DropdownMenuItem> items = displayedMenu.MenuItems();
            #pragma warning restore CS0618
            var action = items[itemIndex] as DropdownMenuAction;

            if (action != null)
            {
                if (action.status == DropdownMenuAction.Status.Disabled)
                {
                    return false;
                }

                action.Execute();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Simulates the selection of a menu item by its action <paramref name="name"/>.
        /// Executes the associated action if found and the action is not disabled.
        /// </summary>
        /// <param name="name">The name of the menu action to select.</param>
        /// <returns>`True` if the action was executed; `false` otherwise.</returns>
        public bool SimulateMenuSelection(string name)
        {
            int index = FindActionIndex(name);

            return SimulateItemSelection(index);
        }

        /// <summary>
        /// Finds the index of a menu action by its <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the menu action to find.</param>
        /// <returns>
        /// The zero-based index of the action if found and the menu is displayed; otherwise, `-1`.
        /// </returns>
        public int FindActionIndex(string name)
        {
            if (!menuIsDisplayed)
            {
                return -1;
            }

            #pragma warning disable CS0618 // Disable warning on Internal usage
            List<DropdownMenuItem> items = displayedMenu.MenuItems();
            #pragma warning restore CS0618
            for (int i = 0; i < items.Count; i++)
            {
                var action = items[i] as DropdownMenuAction;
                if (action != null && action.name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Asserts that the context menu contains an action with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the action to check for.</param>
        /// <exception cref="AssertionException">
        /// Thrown if the action is not found in the menu.
        /// </exception>
        public void AssertContainsAction(string name)
        {
            Assert.AreNotEqual(FindActionIndex(name), -1, $"Menu does not contain action {name}");
        }

        /// <summary>
        /// Asserts that the context menu contains an action with the specified <paramref name="name"/> and <paramref name="expectedStatus"/>.
        /// </summary>
        /// <param name="name">The name of the action to check for.</param>
        /// <param name="expectedStatus">The expected status of the action.</param>
        /// <exception cref="AssertionException">
        /// Thrown if the action is not found or its status does not match the expected value.
        /// </exception>
        public void AssertContainsAction(string name, DropdownMenuAction.Status expectedStatus)
        {
            var index = FindActionIndex(name);

            Assert.AreNotEqual(index, -1, $"Menu does not contain action {name}");

            #pragma warning disable CS0618 // Disable warning on Internal usage
            var item = displayedMenu.MenuItems()[index];
            #pragma warning restore CS0618
            if (item is DropdownMenuAction action)
            {
                Assert.AreEqual(expectedStatus, action.status, $"Expected menu item {name} status to match");
                return;
            }

            Assert.Fail($"Menu does not contain action {name}");
        }
    }
}

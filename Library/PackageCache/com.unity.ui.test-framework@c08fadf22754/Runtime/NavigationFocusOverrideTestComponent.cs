using System;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Provides a wrapper to override the focus controller for navigation focus tests.
    /// </summary>
    [Obsolete("NavigationFocusOverrideTestComponent will be deprecated.")]
    internal class NavigationFocusOverrideTestComponent : UITestComponent
    {
        #pragma warning disable CS0618 // Disable warning on Internal usage
        private NavigationFocusControllerOverride m_NavigationFocusControllerOverride;
        #pragma warning restore CS0618

        /// <summary>
        /// Overrides the focus controller of the panel.
        /// </summary>
        protected override void BeforeTest()
        {
            base.BeforeTest();
            #pragma warning disable CS0618 // Disable warning on Internal usage
            m_NavigationFocusControllerOverride = new NavigationFocusControllerOverride(fixture.panel);
            #pragma warning restore CS0618
        }

        /// <summary>
        /// Resets the focus controller of the panel to the original one,
        /// and disposes of the focus controller override.
        /// </summary>
        protected override void AfterTest()
        {
            // Dispose of the runtime focus controller
            base.AfterTest();
            m_NavigationFocusControllerOverride?.Dispose();
            m_NavigationFocusControllerOverride = null;
        }

        /// <summary>
        /// Overrides the focus controller of the panel.
        /// </summary>
        /// <param name="panel">The panel whose focus controller you want to override.</param>
        public void ForceResetPanel(IPanel panel)
        {
            #pragma warning disable CS0618 // Disable warning on Internal usage
            m_NavigationFocusControllerOverride = new NavigationFocusControllerOverride(panel);
            #pragma warning restore CS0618
        }

        [System.Obsolete("For Internal Use Only.")]
        internal static object GetFocusController(IPanel panel)
        {
            if (panel is Panel p)
            {
                return p.focusController;
            }

            return null;
        }

        // Use a runtime focus controller to simulate navigation events
        [System.Obsolete("For Internal Use Only.")]
        internal class NavigationFocusControllerOverride : IDisposable
        {
            Panel panel;
            FocusController originalController;

            /// <summary>
            /// Creates a `NavigationFocusControllerOverride` and
            /// sets it as the panel's focus controller.
            /// </summary>
            /// <param name="p">The panel whose focus controller you want to override.</param>
            /// <remarks>Saves the original focus controller object so that it can be restored later via the <see cref="AfterTest()"/>.</remarks>
            public NavigationFocusControllerOverride(IPanel p)
            {
                panel = p as Panel;
                originalController = p.focusController;
                panel.focusController = new FocusController(new NavigateFocusRing(panel.visualTree));
            }

            /// <summary>
            /// Resets the panel's focus controller to its original value.
            /// </summary>
            public void Dispose()
            {
                panel.focusController = originalController;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// A <see cref="PanelSimulator"/> accessing an [EditorWindow](xref:UnityEditor.EditorWindow)'s panel.
    /// Allows for the simulation of time passing, sending events,
    /// and updating the panel in a synchronous manner.
    /// </summary>
    public sealed class EditorWindowPanelSimulator : PanelSimulator
    {
        /// <summary>
        /// Sets up the provided <paramref name="window"/> for simulation.
        /// </summary>
        /// <param name="window">The `EditorWindow` to set up for simulation.</param>
        public EditorWindowPanelSimulator(EditorWindow window)
#pragma warning disable CS0618 // Disable warning on Internal usage
            : base()
#pragma warning restore CS0618
        {
            SetWindow(window);
        }

        private EditorWindow m_Window;
        private GUIView nativeView;
        private bool m_InitialDisableInputEvents;

        /// <summary>
        /// `EditorWindow` tied to the panel.
        /// </summary>
        public EditorWindow window
        {
            get => m_Window;
        }

        /// <summary>
        /// Whether events are sent within an IMGUI context.
        /// Set this to true if the `EditorWindow` contains IMGUI content.
        /// Defaults to `false`.
        /// </summary>
        [System.Obsolete("IMGUI is not fully supported.")]
        internal bool needsImprovedIMGUISupport { get; set; } = false;

        /// <summary>
        /// Assigns the specified window to the `EditorWindowPanelSimulator`.
        /// </summary>
        /// <param name="window">
        /// The `EditorWindow` instance to associate with the `EditorWindowPanelSimulator`.
        /// </param>
        public void SetWindow(EditorWindow window)
        {
            if (m_Window != null)
            {
                m_Window.disableInputEvents = m_InitialDisableInputEvents;
            }

            m_Window = window;

            if (m_Window != null)
            {
                nativeView = m_Window.m_Parent;
                m_InitialDisableInputEvents = m_Window.disableInputEvents;

                m_Window.disableInputEvents = true; //we don't want user input to mess with our tests

                #pragma warning disable CS0618 // Disable warning on Internal usage
                SetPanel((Panel)window.rootVisualElement.panel);
                SetRootVisualElement(window.rootVisualElement);
                #pragma warning restore CS0618
            }
            else
            {
                nativeView = null;
                #pragma warning disable CS0618 // Disable warning on Internal usage
                SetPanel(null);
                SetRootVisualElement(null);
                #pragma warning restore CS0618
            }
        }

        /// <summary>
        /// Invokes an `Action` after setting up the `EditorWindow`'s IMGUI context.
        /// This ensures that the IMGUI state is properly setup when executing the provided <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The `Action` to be executed.</param>
        [System.Obsolete("IMGUI is not fully supported.")]
        internal void ExecuteWithinIMGUIContext(System.Action command)
        {
            Debug.Assert(nativeView != null, "ExecuteWithinIMGUIContext needs a valid EditorWindow");

            var evt = UIElementsUtility.CreateTestFrameUpdateEvent(command);
            nativeView.SendEvent(evt);
        }

        /// <summary>
        /// Performs a frame update of the panel.
        /// </summary>
        /// <param name="time">The amount of time in seconds to increment the simulated time.</param>
        /// <remarks>
        /// This method simulates yielding a frame by the following:
        /// <list type="bullet">
        ///     <item>
        ///         <description>Advances the panel's time by the specified <paramref name="time"/>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>Updates the panel's scheduler and visual tree updaters.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>If <see cref="PanelSimulator.needsRendering"/> is true, triggers rendering for the panel's
        ///            <c>ImmediateModeElement</c> and invokes <c>IMGUIContainer</c>'s <c>OnGUI</c> with a Repaint Event.
        ///         </description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <seealso cref="PanelSimulator.FrameUpdate()"/>
        /// <seealso cref="PanelSimulator.FrameUpdateMs(long)"/>
        public sealed override void FrameUpdate(double time)
        {
            #pragma warning disable CS0618 // Disable warning on Internal usage
            EnsureFrameUpdateCalledDuringTest();
            #pragma warning restore CS0618

            #pragma warning disable CS0618 // IMGUI is not fully supported
            if (needsImprovedIMGUISupport && m_Window != null)
            {
            #pragma warning restore CS0618

                #pragma warning disable CS0618 // IMGUI is not fully supported
                ExecuteWithinIMGUIContext(() =>
                {
                #pragma warning restore CS0618

                    #pragma warning disable CS0618 // Disable warning on Internal usage
                    DoFrameUpdate(time);
                    #pragma warning restore CS0618
                });
            }
            else
            {
                #pragma warning disable CS0618 // Disable warning on Internal usage
                DoFrameUpdate(time);
                #pragma warning restore CS0618
            }
        }
    }
}

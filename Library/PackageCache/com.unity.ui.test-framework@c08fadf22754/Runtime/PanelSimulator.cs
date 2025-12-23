using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Base class for managing Editor or runtime panels in tests.
    /// Enables simulation of time progression, event dispatching, and synchronous panel updates.
    /// Panel creation and ownership are handled by derived classes.
    /// </summary>
    public abstract class PanelSimulator
    {
        /// <summary>
        /// Forbid external direct inheritance of PanelSimulator class.
        /// PanelSimulator is exclusively intended as a base class.
        /// </summary>
        [System.Obsolete("For Internal Use Only.")]
        internal PanelSimulator() { }

        private Panel m_Panel;

        /// <summary>
        /// The panel associated to the instance of `PanelSimulator`.
        /// </summary>
        public IPanel panel => m_Panel;

        /// <summary>
        /// The effective `rootVisualElement` of the panel.
        /// </summary>
        /// <remarks>
        /// Use this property to add elements to or query the UI of the panel.
        /// </remarks>
        public VisualElement rootVisualElement { get; private set; }

        static double s_CurrentTime;

        /// <summary>
        /// The simulated time in milliseconds used by the panel.
        /// Simulates `Time.RealtimeSinceStartup`.
        /// Use <see cref="IncrementCurrentTimeMs(long)"/> or <see cref="IncrementCurrentTime(double)"/> to increment the time.
        /// </summary>
        public static long currentTimeMs => (long)(s_CurrentTime * 1000.0);

        /// <summary>
        /// The simulated time in seconds used by the panel.
        /// Simulates `Time.RealtimeSinceStartup`.
        /// Use <see cref="IncrementCurrentTimeMs(long)"/> or <see cref="IncrementCurrentTime(double)"/> to increment the time.
        /// </summary>
        public static double currentTime => s_CurrentTime;

        /// <summary>
        /// Gets or sets <see cref="timePerSimulatedFrame"/> in milliseconds.
        /// The simulated time is incremented by this amount through calls to
        /// <see cref="IncrementCurrentTimeMs(long)"/>, <see cref="IncrementCurrentTime(double)"/>,
        /// <see cref="FrameUpdate()"/> or calls to the Event simulation utility methods.
        /// Defaults to `200`milliseconds.
        /// </summary>
        /// <remarks>
        /// This value is not automatically reset within the test fixture.
        /// Call <see cref="ResetTimePerSimulatedFrameToDefault"/> when the value that was set is no longer required.
        /// </remarks>
        public long timePerSimulatedFrameMs
        {
            get => (long)(timePerSimulatedFrame * 1000.0);
            set => timePerSimulatedFrame = value / 1000.0;
        }

        private const double m_DefaultTimePerSimulatedFrame = 0.2;

        /// <summary>
        /// The amount of time in seconds to increment the simulated time.
        /// The simulated time is incremented by this amount through calls to
        /// <see cref="IncrementCurrentTimeMs(long)"/>, <see cref="IncrementCurrentTime(double)"/>,
        /// <see cref="FrameUpdate()"/> or calls to the Event simulation utility methods.
        /// Default is `0.2` seconds.
        /// </summary>
        /// <remarks>
        /// This value is not automatically reset within the test fixture.
        /// Call <see cref="ResetTimePerSimulatedFrameToDefault"/> when the value that was set is no longer required.
        /// </remarks>
        public double timePerSimulatedFrame { get; set; } = m_DefaultTimePerSimulatedFrame;

        /// <summary>
        /// Resets the <see cref="timePerSimulatedFrame"/> to its default value of `0.2` seconds.
        /// The <see cref="timePerSimulatedFrame"/> is not automatically reset within the test fixture.
        /// Call this function when the value that was set is no longer required.
        /// </summary>
        public void ResetTimePerSimulatedFrameToDefault()
        {
            timePerSimulatedFrame = m_DefaultTimePerSimulatedFrame;
        }

        private float m_SetPixelsPerPoint = 0;

        /// <summary>
        /// The pixels per point scaling of the panel.
        /// Defaults to `1`.
        /// </summary>
        public float pixelsPerPoint
        {
            get
            {
                if (panel is Panel p)
                {
                    return p.pixelsPerPoint;
                }
                return 1.0f;
            }
            set
            {
                m_SetPixelsPerPoint = value;

                if (panel is Panel p)
                {
                    p.pixelsPerPoint = value;
#if UNITY_EDITOR
                    p.UpdateScalingFromEditorWindow = false;
#endif
                }
            }
        }

        Event m_IMGUIRepaintEvent = new Event { type = EventType.Repaint };

        [System.Obsolete("For Internal Use Only.")]
        internal void SetPanel(IPanel p)
        {
            RemoveTimeSinceStartupOverride();

            m_Panel = p as Panel;

            if (m_Panel != null)
            {
                m_Panel.repaintData.repaintEvent = m_IMGUIRepaintEvent;
                OverrideTimeSinceStartup();
            }

            if (m_SetPixelsPerPoint != 0)
            {
                pixelsPerPoint = m_SetPixelsPerPoint;
            }
        }

        [System.Obsolete("For Internal Use Only.")]
        internal void SetRootVisualElement(VisualElement root)
        {
            if (root != null && root.panel != panel)
                throw new System.Exception("RootVisualElement of the panel cannot be set to an element belonging to a different panel.");

            rootVisualElement = root;
        }

        /// <summary>
        /// Returns the name of the panel.
        /// </summary>
        public string panelName
        {
            get
            {
                if (m_Panel != null)
                {
                    return m_Panel.name;
                }
                throw new System.Exception("Cannot get name for null Panel.");
            }
        }

        /// <summary>
        /// The default size of the panel, which is `(500.0f, 500.0f)`.
        /// </summary>
        /// <returns>Vector2 representing the default size of the panel.</returns>
        public static Vector2 GetDefaultPanelSize() => new Vector2(500.0f, 500.0f);

        [System.Obsolete("For Internal Use Only.")]
        internal long GetPanelTime() => m_Panel.TimeSinceStartupMs();

        bool m_TimeSinceStartupSet = false;
        TimeFunction m_PreviousTimeFunction = null;

        private bool OverrideTimeSinceStartup()
        {
            m_PreviousTimeFunction = m_Panel.TimeSinceStartupFunc;
            m_TimeSinceStartupSet = true;
            m_Panel.TimeSinceStartupFunc = () => s_CurrentTime;
            activeSimulators.Add(this);
            return true;
        }

        private bool RemoveTimeSinceStartupOverride()
        {
            if (m_TimeSinceStartupSet && m_Panel != null)
            {
                m_Panel.TimeSinceStartupFunc = m_PreviousTimeFunction;
                m_PreviousTimeFunction = null;
                m_TimeSinceStartupSet = false;

                activeSimulators.Remove(this);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Increments the current simulated time by the provided <paramref name="timeMs"/> (in milliseconds) amount.
        /// </summary>
        /// <param name="timeMs">The amount of time in milliseconds to increment the simulated time.</param>
        public static void IncrementCurrentTimeMs(long timeMs) => IncrementCurrentTime(timeMs / 1000.0);

        /// <summary>
        /// Increments the current simulated time by the provided <paramref name="time"/> (in seconds) amount.
        /// </summary>
        /// <param name="time">The amount of time in seconds to increment the simulated time.</param>
        public static void IncrementCurrentTime(double time)
        {
            s_CurrentTime += time;
        }

        private static List<PanelSimulator> activeSimulators = new List<PanelSimulator>();
        /// <summary>
        /// Resets the current simulated time to zero.
        /// </summary>
        public static void ResetCurrentTime()
        {
            var previousTime = s_CurrentTime;

            s_CurrentTime = 0;

            for(int i = 0; i < activeSimulators.Count; ++i)
                activeSimulators[i].m_Panel.ApplyTimeAdjustment(previousTime, s_CurrentTime);
        }

        /// <summary>
        /// Whether rendering during a
        /// <see cref="FrameUpdate()"/>, <see cref="FrameUpdateMs(long)"/>, or <see cref="FrameUpdate(double)"/>
        /// for the panel must be done via a repaint event.
        /// Defaults to `false`.
        /// </summary>
        /// <remarks>
        /// Set to `true` if the panel requires either `ImmediateModeElement` rendering
        /// or an `IMGUIContainer`'s `OnGUI` logic.
        /// </remarks>
        public bool needsRendering { get; set; } = false;

        /// <summary>
        /// Performs a frame update of the panel.
        /// </summary>
        /// <param name="time">The amount of time in seconds to increment the simulated time.</param>
        /// <remarks>
        /// This method simulates yielding a frame by the following:
        /// <list type="bullet">
        /// <item><description>Updates the scheduler and the panel's visual tree updaters.</description></item>
        /// <item><description>Advances the time by the amount specified in <paramref name="time"/>.</description></item>
        /// <item><description>When <see cref="PanelSimulator.needsRendering"/> is true, it triggers rendering for the panel's <c>ImmediateModeElement</c>
        /// and invokes <c>IMGUIContainer</c>'s <c>OnGUI</c> with a Repaint Event.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="FrameUpdate()"/>
        /// <seealso cref="FrameUpdateMs(long)"/>
        public abstract void FrameUpdate(double time);

        /// <summary>
        /// Increments the simulated time by the amount specified in
        /// <see cref="timePerSimulatedFrame"/> and then performs a <see cref="FrameUpdate(double)"/>.
        /// </summary>
        /// <seealso cref="FrameUpdate(double)"/>
        /// <seealso cref="FrameUpdateMs(long)"/>
        public void FrameUpdate() => FrameUpdate(timePerSimulatedFrame);

        /// <summary>
        /// Increments the simulated time by the amount specified in
        /// <paramref name="timeMs"/> (in milliseconds) and then performs a <see cref="FrameUpdate(double)"/>.
        /// </summary>
        /// <param name="timeMs">The amount of time in milliseconds to increment the simulated time.</param>
        /// <seealso cref="FrameUpdate()"/>
        /// <seealso cref="FrameUpdate(double)"/>
        public void FrameUpdateMs(long timeMs) => FrameUpdate(timeMs / 1000.0);

        [System.Obsolete("For Internal Use Only.")]
        internal void DoFrameUpdate(double time)
        {
            IncrementCurrentTime(time);
            PanelUpdate(m_Panel, needsRendering);
        }

        private static void PanelUpdate(Panel p, bool render = true)
        {
            if (p.disposed)
                throw new System.ObjectDisposedException("Panel");

            p.TickSchedulingUpdaters();

            if (p.disposed)
                return;

            if (render)
            {
                p.repaintData.repaintEvent = new Event { type = EventType.Repaint };
            }
            p.UpdateForRepaint();

            if (p.disposed)
                return;

            if (render)
            {
                p.Render();
            }
        }

        /// <summary>
        /// Sends key events to the panel to simulate pressing a given key.
        /// </summary>
        /// <param name="keyCode">The `KeyCode` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// <para>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </para>
        /// <para>
        /// Do the following to send specific key events:
        /// </para>
        /// <para>
        /// <list type="bullet">
        ///  <item><description>To send text, use <see cref="TypingText(string, bool)"/>.</description></item>
        ///  <item><description>To send Return, use <see cref="ReturnKeyPress(EventModifiers)"/>.</description></item>
        ///  <item><description>To send KeypadEnter, use <see cref="KeypadEnterKeyPress(EventModifiers)"/>.</description></item>
        ///  <item><description>To send Tab, use <see cref="TabKeyPress(EventModifiers)"/>.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Note:</b>
        /// This function does not generate Navigation events.
        /// Interactions with certain controls that depend on Navigation events
        /// are therefore not supported in Play mode tests.
        /// </para>
        /// </remarks>
        public void KeyPress(KeyCode keyCode, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateKey(keyCode, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends key events to the panel to simulate pressing the Return key.
        /// </summary>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// <para>
        /// <b>Note:</b>
        /// This function does not generate Navigation events.
        /// Interactions with certain controls that depend on Navigation events
        /// are therefore not supported in Play mode tests.
        /// </para>
        /// <para>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </para>
        /// </remarks>
        public void ReturnKeyPress(EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateReturnKey(modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends key events to the panel to simulate pressing the KeypadEnter key.
        /// </summary>
        /// <param name="modifiers">The EventModifiers for the event.</param>
        /// <remarks>
        /// <para>
        /// <b>Note:</b>
        /// This function does not generate Navigation events.
        /// Interactions with certain controls that depend on Navigation events
        /// are therefore not supported in Play mode tests.
        /// </para>
        /// <para>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </para>
        /// </remarks>
        public void KeypadEnterKeyPress(EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateKeypadEnterKey(modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends key events to the panel to simulate pressing the Tab key.
        /// </summary>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// <para>
        /// <b>Note:</b>
        /// This function does not generate Navigation events.
        /// Interactions with certain controls that depend on Navigation events
        /// are therefore not supported in Play mode tests.
        /// </para>
        /// <para>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </para>
        /// </remarks>
        public void TabKeyPress(EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateTabKey(modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends key events to the panel to simulate typing the given text.
        /// </summary>
        /// <param name="text">Text to type.</param>
        /// <param name="useKeypad">Whether keypad `KeyCodes` (for example, `Keypad0`, `KeypadMinus`)
        /// can be used instead of Alpha `KeyCodes` (for example, `Alpha0`, `Minus`).
        /// Defaults to `false`.</param>
        /// <remarks>
        /// <para>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </para>
        /// <para>
        /// <list type="bullet">
        ///     <item><description>Only officially supports US layout keyboard and English language.</description></item>
        ///     <item><description>On Windows, Linux, and macOS runtime platforms, sends UI Toolkit key events.</description></item>
        ///     <item><description>On most other runtime platforms, sets the text using the `TouchScreenKeyboard`.</description></item>
        ///     <item><description>On Switch, does nothing to prevent freezing the device, set it directly on the TextElement instead.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public void TypingText(string text, bool useKeypad = false)
        {
#if !(PLATFORM_SWITCH || PLATFORM_SWITCH2)
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateTypingText(text, useKeypad).ExecuteWithinFrame();
            }
#endif
        }

        /// <summary>
        /// Sends key events to the panel to simulate pressing down a given key.
        /// </summary>
        /// <param name="keyCode">The `KeyCode` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// <para>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </para>
        /// <para>
        /// Do the following to send specific key events:
        /// </para>
        /// <para>
        /// <list type="bullet">
        ///  <item><description>To send text, use <see cref="TypingText(string, bool)"/>.</description></item>
        ///  <item><description>To send Return, use <see cref="ReturnKeyPress(EventModifiers)"/>.</description></item>
        ///  <item><description>To send KeypadEnter, use <see cref="KeypadEnterKeyPress(EventModifiers)"/>.</description></item>
        ///  <item><description>To send Tab, use <see cref="TabKeyPress(EventModifiers)"/>.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Note:</b>
        /// This function does not generate Navigation events.
        /// Interactions with certain controls that depend on Navigation events
        /// are therefore not supported in Play mode tests.
        /// </para>
        /// </remarks>
        public void KeyDown(KeyCode keyCode, EventModifiers modifiers = EventModifiers.None)
        {
            // Some keys send the FunctionKey modifier even if it isn't specified, so the helper also sends it.
            if (EventHelpers.ShouldSendFunctionModifier(keyCode))
            {
                modifiers |= EventModifiers.FunctionKey;
            }

            using (var evt = EventHelpers.MakeKeyDown(keyCode, modifiers))
            {
                panel.SendEvent(evt);
            }

            IncrementCurrentTime(timePerSimulatedFrame);
        }

        /// <summary>
        /// Sends key events to the panel to simulate releasing a given key.
        /// </summary>
        /// <param name="keyCode">The `KeyCode` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// <para>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </para>
        /// <para>
        /// Do the following to send specific key events:
        /// </para>
        /// <para>
        /// <list type="bullet">
        ///  <item><description>To send text, use <see cref="TypingText(string, bool)"/>.</description></item>
        ///  <item><description>To send Return, use <see cref="ReturnKeyPress(EventModifiers)"/>.</description></item>
        ///  <item><description>To send KeypadEnter, use <see cref="KeypadEnterKeyPress(EventModifiers)"/>.</description></item>
        ///  <item><description>To send Tab, use <see cref="TabKeyPress(EventModifiers)"/>.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Note:</b>
        /// This function does not generate Navigation events.
        /// Interactions with certain controls that depend on Navigation events
        /// are therefore not supported in Play mode tests.
        /// </para>
        /// </remarks>
        public void KeyUp(KeyCode keyCode, EventModifiers modifiers = EventModifiers.None)
        {
            // Some keys send the FunctionKey modifier even if it isn't specified, so the helper also sends it.
            if (EventHelpers.ShouldSendFunctionModifier(keyCode))
            {
                modifiers |= EventModifiers.FunctionKey;
            }

            using (var evt = EventHelpers.MakeKeyUp(keyCode, modifiers))
            {
                panel.SendEvent(evt);
            }

            IncrementCurrentTime(timePerSimulatedFrame);
        }

        /// <summary>
        /// Sends a single click to the <paramref name="ve"/>'s panel.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel receives the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <remarks>
        /// The position of the click is the center of the <paramref name="ve"/>'s world bound.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void Click(VisualElement ve, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                ve.SimulateClick(button, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a single click to the panel.
        /// </summary>
        /// <param name="position">The absolute position for the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <remarks>
        /// The position of the click is the center of the <paramref name="ve"/>'s world bound.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void Click(Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateClick(position, button, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a double click to the panel.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel receives the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <remarks>
        /// The position of the click is the center of the <paramref name="ve"/>'s world bound.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void DoubleClick(VisualElement ve, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                ve.SimulateDoubleClick(button, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a double click to the panel.
        /// </summary>
        /// <param name="position">The absolute position for the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <remarks>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void DoubleClick(Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateDoubleClick(position, button, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a `PointerDownEvent`, incremental PointerMoveEvent`s, and a `PointerUpEvent`,
        /// in that order, to the panel.
        /// </summary>
        /// <param name="positionFrom">The absolute starting position of the Mouse.</param>
        /// <param name="positionTo">The absolute final position to move the Mouse to.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <remarks>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void DragAndDrop(Vector2 positionFrom, Vector2 positionTo, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateDragAndDrop(positionFrom, positionTo, button, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a `PointerDownEvent` to the panel.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel receives the events.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// The position of the event is the center of the <paramref name="ve"/>'s world bound.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void MouseDown(VisualElement ve, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            MouseDown(ve.worldBound.center, button, modifiers);
        }

        /// <summary>
        /// Sends a `PointerDownEvent` to the panel.
        /// </summary>
        /// <param name="position">The absolute position for the event in UI coordinates.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void MouseDown(Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateMouseDownAt(position, button, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a `PointerUpEvent` to the panel.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// The position of the event is the center of the <paramref name="ve"/>'s worldBound.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void MouseUp(VisualElement ve, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            MouseUp(ve.worldBound.center, button, modifiers);
        }

        /// <summary>
        /// Sends a `PointerUpEvent` to the panel.
        /// </summary>
        /// <param name="position">The absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void MouseUp(Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateMouseUpAt(position, button, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends incremental `PointerMoveEvent`s to the panel.
        /// </summary>
        /// <param name="positionFrom">The absolute starting position of the Mouse.</param>
        /// <param name="positionTo">The absolute final position to move the Mouse to.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <remarks>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void MouseMove(Vector2 positionFrom, Vector2 positionTo, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateMouseMove(positionFrom, positionTo, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a `PointerMoveEvent` event to the panel.
        /// </summary>
        /// <param name="positionTo">The absolute position to move the Mouse to.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <remarks>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void MouseMove(Vector2 positionTo, EventModifiers modifiers = EventModifiers.None)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateMouseMoveTo(positionTo, modifiers).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a `WheelEvent` to the panel.
        /// </summary>
        /// <param name="delta">The delta (scroll amount) for the event.</param>
        /// <param name="position">The absolute position for the event.</param>
        /// <remarks>
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void ScrollWheel(Vector2 delta, Vector2 position)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateScrollWheel(delta, position).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends an `ExecuteCommandEvent` for the specified <paramref name="commandName"/> to the panel.
        /// </summary>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <remarks>
        /// `ExecuteCommandEvent` are only officially supported in Editor (not Runtime).
        /// The event is dispatched to the focused element.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void ExecuteCommand(string commandName)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateExecuteCommand(commandName).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends an `ExecuteCommandEvent` for the specified <paramref name="command"/> to the panel.
        /// </summary>
        /// <param name="command">The `Command` to execute.</param>
        /// <remarks>
        /// `ExecuteCommandEvent` are only officially supported in Editor (not Runtime).
        /// The event will be dispatched to the focused element.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        ///
        /// Requires Internal access because EventHelpers.Command is internal.
        /// Will be made public if a new enum is created for public use.
        /// </remarks>
        /// <seealso cref="SimulateRuntimeHelpers.SimulateExecuteCommand(VisualElement, EventHelpers.Command)"/>
        internal void ExecuteCommand(EventHelpers.Command command) => ExecuteCommand(command.ToString());

        /// <summary>
        /// Sends a `ValidateCommandEvent` for the specified <paramref name="commandName"/> to the panel.
        /// </summary>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <remarks>
        /// `ValidateCommandEvent` are only supported in Editor (not Runtime).
        /// The event is dispatched to the focused element.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        /// </remarks>
        public void ValidateCommand(string commandName)
        {
            using (EventHelpers.SetFrameWaiter(() => IncrementCurrentTime(timePerSimulatedFrame)))
            {
                rootVisualElement.SimulateValidateCommand(commandName).ExecuteWithinFrame();
            }
        }

        /// <summary>
        /// Sends a `ValidateCommandEvent` for the specified <paramref name="command"/> to the panel.
        /// </summary>
        /// <param name="command">The `Command` to execute.</param>
        /// <remarks>
        /// `ValidateCommandEvent` are only officially supported in Editor (not Runtime).
        /// The event will be dispatched to the focused element.
        /// Increments the current simulated time by <see cref="timePerSimulatedFrame"/> 
        /// after dispatching each event.
        /// After all events have been sent, disposes of the events.
        ///
        /// Requires Internal access because EventHelpers.Command is internal.
        /// Will be made public if a new enum is created for public use.
        /// </remarks>
        /// <seealso cref="SimulateRuntimeHelpers.SimulateValidateCommand(VisualElement, EventHelpers.Command)"/>
        internal void ValidateCommand(EventHelpers.Command command) => ValidateCommand(command.ToString());

        private CommonUITestFixture m_UITestFixture;

        [System.Obsolete("For Internal Use Only.")]
        internal void SetUITestFixture(CommonUITestFixture baseUITestFixture)
        {
            m_UITestFixture = baseUITestFixture;
        }

        [System.Obsolete("For Internal Use Only.")]
        internal void EnsureFrameUpdateCalledDuringTest()
        {
            if (m_UITestFixture != null)
                m_UITestFixture.EnsureDuringTest("FrameUpdate");
        }
    }
}

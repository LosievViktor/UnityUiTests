using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;
using static UnityEngine.UIElements.TestFramework.RuntimeFrameHelpers;

namespace UnityEditor.UIElements.TestFramework
{
    #region Obsolete IEnumerator helpers
    // They are now internal until they will be replaced by a better solution that prevent API duplication of UITestFixtures.simulateXXX

    /// <summary>
    /// Contains functionality related to Editor frames.
    /// </summary>
    internal static class EditorFrameHelpers
    {
        /// <summary>
        /// Function to wait for the next UI frame.
        /// Performs checks on the `EditorWindow`'s `Panel`'s updaters and scheduler to ensure
        /// that at least one Update loop for each has elapsed.
        /// </summary>
        /// <param name="window">The EditorWindow whose panel will be used for frame verification.</param>
        /// <param name="enumerator">The enumerator which will be executed during each waited frame. Default results in a yield return null.</param>
        /// <exception cref="Exception">
        /// If a full update for each frame requested cannot be confirmed within the MaxTime, throws an exception.
        /// </exception>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator UIFrameWaiter(this EditorWindow window, Func<IEnumerator> enumerator = null)
        {
            return window.rootVisualElement.panel.UIFrameWaiter(enumerator);
        }
    }

    /// <summary>
    /// Contains functionality related to simulating UI interactions within the Editor.
    /// </summary>
    internal static class SimulateEditorHelpers
    {
        /// <summary>
        /// Sends a single click to the given window's `rootVisualElement`'s panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the events.</param>
        /// <param name="position">The Absolute position for the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateClick(this EditorWindow window, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateClick(position, button, modifiers);
        }

        /// <summary>
        /// Sends a double click to the given window's rootVisualElement's panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the events.</param>
        /// <param name="position">The Absolute position for the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateDoubleClick(this EditorWindow window, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateDoubleClick(position, button, modifiers);
        }

        /// <summary>
        /// Sends a `PointerDownEvent` to the given window's rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseDownAt(this EditorWindow window, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateMouseDownAt(position, button, modifiers);
        }

        /// <summary>
        /// Sends a `PointerUpEvent` to the given window's rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseUpAt(this EditorWindow window, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateMouseUpAt(position, button, modifiers);
        }

        /// <summary>
        /// Sends incremental `PointerMoveEvent`s to the given window's rootVisualElement's panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the events.</param>
        /// <param name="positionFrom">The Absolute starting position of the Mouse.</param>
        /// <param name="positionTo">The Absolute final position to move the Mouse to.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseMove(this EditorWindow window, Vector2 positionFrom, Vector2 positionTo, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateMouseMove(positionFrom, positionTo, modifiers);
        }

        /// <summary>
        /// Sends a `PointerMoveEvent` to the given window's rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseMoveTo(this EditorWindow window, Vector2 position, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateMouseMoveTo(position, modifiers);
        }

        /// <summary>
        /// Sends a `PointerDownEvent`, incremental `PointerMoveEvent`s, and a `PointerUpEvent` event,
        /// in that order, to the given window's rootVisualElement's panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the events.</param>
        /// <param name="positionFrom">The Absolute starting position of the Mouse.</param>
        /// <param name="positionTo">The Absolute final position to move the Mouse to.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateDragAndDrop(this EditorWindow window, Vector2 positionFrom, Vector2 positionTo, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateDragAndDrop(positionFrom, positionTo, button, modifiers);
        }

        /// <summary>
        /// Sends a ScrollWheel event to the given window's rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="delta">The delta (scroll amount) for the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateScrollWheel(this EditorWindow window, Vector2 delta, Vector2 position)
        {
            yield return window.rootVisualElement.SimulateScrollWheel(delta, position);
        }

        /// <summary>
        /// Sends key events to the window's rootVisualElement's panel to simulate typing the given text.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="text">Text to type.</param>
        /// <param name="useKeypad">Boolean indicating whether keypad KeyCodes (e.g. Keypad0, KeypadMinus)
        /// should be used instead of Alpha keycodes (e.g. Alpha0, Minus).
        /// Default is to use non-keypad KeyCodes.</param>
        /// <remarks>Only officially supports US layout keyboard and English language.</remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateTypingText(this EditorWindow window, string text, bool useKeypad = false)
        {
            yield return window.rootVisualElement.SimulateTypingText(text, useKeypad);
        }

        /// <summary>
        /// Sends key events to the window's rootVisualElement's panel to simulate pressing a given key.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="code">The KeyCode for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// <para>For sending text, use the SimulateTypingText function.</para>
        /// <para>For sending Return or KeypadEnter, use the SimulateReturnKey or SimulateKeypadEnterKey function.</para>
        /// <para>For sending Tab, use the SimulateTabKey function.</para>
        /// </remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateKey(this EditorWindow window, KeyCode code, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateKey(code, modifiers);
        }


        /// <summary>
        /// Sends key events to the window's rootVisualElement's panel to simulate pressing the Return key.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateReturnKey(this EditorWindow window, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateReturnKey(modifiers);
        }


        /// <summary>
        /// Sends key events to the window's rootVisualElement's panel to simulate pressing the KeypadEnter key.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateKeypadEnterKey(this EditorWindow window, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateKeypadEnterKey(modifiers);
        }

        /// <summary>
        /// Sends key events to the window's rootVisualElement's panel to simulate pressing the Tab key.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateTabKey(this EditorWindow window, EventModifiers modifiers = EventModifiers.None)
        {
            yield return window.rootVisualElement.SimulateTabKey(modifiers);
        }

        /// <summary>
        /// Sends an `ExecuteCommandEvent` for the specified <paramref name="commandName"/> to the <paramref name="window"/>'s rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        /// <remarks><paramref name="commandName"/> should correspond to one of the valid `UnityEngine.EventCommandNames`.</remarks>
        public static IEnumerator SimulateExecuteCommand(this EditorWindow window, string commandName)
        {
            yield return window.rootVisualElement.SimulateExecuteCommand(commandName);
        }

        /// <summary>
        /// Sends an `ExecuteCommandEvent` for the specified <paramref name="command"/> to the <paramref name="window"/>'s rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="command">The `Command` to execute.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateExecuteCommand(this EditorWindow window, EventHelpers.Command command)
        {
            yield return window.rootVisualElement.SimulateExecuteCommand(command);
        }

        /// <summary>
        /// Sends an `ValidateCommandEvent` for the specified <paramref name="commandName"/> to the <paramref name="window"/>'s rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        /// <remarks><paramref name="commandName"/> should correspond to one of the valid `UnityEngine.EventCommandNames`.</remarks>
        public static IEnumerator SimulateValidateCommand(this EditorWindow window, string commandName)
        {
            yield return window.rootVisualElement.SimulateValidateCommand(commandName);
        }

        /// <summary>
        /// Sends an `ValidateCommandEvent` for the specified <paramref name="command"/> to the <paramref name="window"/>'s rootVisualElement's panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the event.</param>
        /// <param name="command">The `Command` to execute.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateValidateCommand(this EditorWindow window, EventHelpers.Command command)
        {
            yield return window.rootVisualElement.SimulateValidateCommand(command);
        }
    }

    /// <summary>
    /// Dispatches and disposes of events in the Editor.
    /// </summary>
    internal static class DispatchEditorHelpers
    {
        /// <summary>
        /// Sends one or more events to the <paramref name="window"/>'s rootVisualElement's panel.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the events.</param>
        /// <param name="evts">`EventBase` events to send.</param>
        public static void Dispatch(this EditorWindow window, params EventBase[] evts)
        {
            window.rootVisualElement.Dispatch(evts);
        }

        /// <summary>
        /// Sends one or more events to the <paramref name="window"/>'s rootVisualElement's panel. Waits one frame after sending each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="window">`EditorWindow` whose rootVisualElement's panel should receive the events.</param>
        /// <param name="evts">`EventBase` events to send.</param>
        /// <remarks>The wait function can be set with <see cref="EventHelpers.SetFrameWaiter(Action)"/>
        /// or <see cref="EventHelpers.SetFrameWaiter(Func{IEnumerator})"/>.</remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator DispatchAndWaitForNextFrame(this EditorWindow window, params EventBase[] evts)
        {
            yield return window.rootVisualElement.DispatchAndWaitForNextFrame(evts);
        }
    }

    #endregion // IEnumerator helpers
}

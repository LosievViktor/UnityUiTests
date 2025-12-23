using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using static UnityEngine.UIElements.Panel;
using static UnityEngine.UIElements.TestFramework.DispatchRuntimeHelpers;

namespace UnityEngine.UIElements.TestFramework
{
    #region Obsolete IEnumerator helpers
    // They are now internal until they will be replaced by a better solution that prevent API duplication of UITestFixtures.simulateXXX

    /// <summary>
    /// Class containing general UI Test Framework base functionality related to set up and events.
    /// </summary>
    internal static class EventHelpers
    {
        private const string k_SetUpWasNotCalled = "EventHelpers.TestSetUp() was not called.";
        private const string k_SetUpWasAlreadyCalled = "EventHelpers.TestSetUp() was already called.";

        private const string k_TearDownWasNotCalled =
            "Previous test forget to call EventHelpers.TestTearDown() to cleanup the input state.";

        private const string k_TearDownNotNeeded =
            "EventHelpers.TestSetUp() was not called or EventHelpers.TestTearDown() was already called.";

        private static string _initializedTestId;
        private static string currentTestId => TestContext.CurrentTestExecutionContext.CurrentTest.Id;

        /// <summary>
        /// Frame waiter used during event dispatching or simulation calls.
        /// Defaults to `null`, performing a `yield return null`.
        /// </summary>
        private static Func<IEnumerator> frameWaiterFunction = null;

        /// <summary>
        /// The current frame waiter function.
        /// </summary>
        internal static Func<IEnumerator> FrameWaiterFunction
        {
            get
            {
                return frameWaiterFunction;
            }
            set
            {
                frameWaiterFunction = value;
            }
        }

        /// <summary>
        /// Sets the frame waiter used during event dispatching or simulation calls
        /// to the specified function.
        /// </summary>
        /// <param name="func">`Void` function to set as the frame waiter.</param>
        /// <returns>`FrameWaiter` wrapper of the provided `Action`.</returns>
        public static FrameWaiter SetFrameWaiter(Action func)
        {
            EnsureSetUp();
            return new FrameWaiter(func);
        }

        /// <summary>
        /// Sets the frame waiter used during event dispatching or simulation calls
        /// to the specified function.
        /// </summary>
        /// <param name="func">`IEnumerator` function to set as the frame waiter.</param>
        /// <returns>`FrameWaiter` wrapper of the provided `Func`.</returns>
        public static FrameWaiter SetFrameWaiter(Func<IEnumerator> func)
        {
            EnsureSetUp();
            return new FrameWaiter(func);
        }

        private static void EnsureSetUp()
        {
            if (_initializedTestId != currentTestId)
            {
                Debug.LogError(k_SetUpWasNotCalled);
            }
        }

        /// <summary>
        /// Sets up the initialized test ID. If a test was already initialized, logs an error.
        /// </summary>
        /// <remarks>
        /// If a test triggers a Domain Reload, TestSetUp should be called after the Domain Reload.
        /// </remarks>
        public static void TestSetUp()
        {
            if (_initializedTestId != null)
            {
                Debug.LogError(_initializedTestId == currentTestId ? k_SetUpWasAlreadyCalled : k_TearDownWasNotCalled);
            }
            _initializedTestId = currentTestId;
            PointerDeviceState.Reset();

            frameWaiterFunction = null;
        }

        /// <summary>
        /// Clears the intialized test ID. If no test was initialized, logs an error.
        /// </summary>
        /// <remarks>
        /// If a test triggers a Domain Reload, TestTearDown should be called before the Domain Reload.
        /// </remarks>
        public static void TestTearDown()
        {
            if (_initializedTestId == null)
            {
                Debug.LogError(k_TearDownNotNeeded);
            }
            else
            {
                _initializedTestId = null;
            }

            PointerDeviceState.Reset();

            frameWaiterFunction = null;
        }

        // In order for tests to run without an EditorWindow but still be able to send
        // events, we sometimes need to force the event type. IMGUI::GetEventType() (native) will
        // return the event type as Ignore if the proper views haven't yet been
        // initialized. This (falsely) breaks tests that rely on the event type. So for tests, we
        // just ensure the event type is what we originally set it to when we sent it.
        // This original type can be retrieved via Event.rawType.
        /// <summary>
        /// Creates a UIToolkit `EventBase` event.
        /// </summary>
        /// <param name="evt">IMGUI Event to use as a base.</param>
        /// <returns>UI Toolkit event.</returns>
        private static EventBase CreateEvent(Event evt)
        {
            EnsureSetUp();
            return UIElementsUtility.CreateEvent(evt, evt.rawType);
        }

        /// <summary>
        /// Creates a UIToolkit `EventBase` of the specified type.
        /// </summary>
        /// <param name="type">The `EventType` for the event.</param>
        /// <returns>UI Toolkit event.</returns>
        public static EventBase MakeEvent(EventType type)
        {
            var evt = new Event() { type = type };
            return CreateEvent(evt);
        }

        // NOTE:
        // Event instantiation should always include both mousePosition and delta
        // in order to correctly calculate the position to perform MouseMove/MouseDrag actions.
        // The EventHelpers methods will require code changes to provide
        // the missing parameters to Event to avoid creating improper Events.
        /// <summary>
        /// Creates an initialized event of the provided type with the specified position.
        /// </summary>
        /// <param name="type">The `EventType` for the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <returns>UI Toolkit event.</returns>
        public static EventBase MakeEvent(EventType type, Vector2 position)
        {
            var evt = new Event() { type = type, mousePosition = position };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initalized event of the specified type using the input parameters.
        /// </summary>
        /// <param name="type">The `EventType` for the event. Should be either KeyDown or KeyUp.</param>
        /// <param name="code">The `KeyCode` for the event.</param>
        /// <param name="character">The character for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>An initialized `KeyDownEvent` or `KeyUpEvent`.</returns>
        public static EventBase MakeKeyEvent(EventType type, KeyCode code = KeyCode.None, char character = '\0', EventModifiers modifiers = EventModifiers.None)
        {
            var evt = new Event() { type = type, keyCode = code, character = character, modifiers = modifiers };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized `KeyDownEvent`.
        /// </summary>
        /// <param name="code">The `KeyCode` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>An initialized `KeyDownEvent`.</returns>
        public static EventBase MakeKeyDown(KeyCode code, EventModifiers modifiers = EventModifiers.None)
        {
            var evt = new Event() { type = EventType.KeyDown, keyCode = code, character = '\0', modifiers = modifiers };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized `KeyDownEvent`.
        /// </summary>
        /// <param name="character">The character for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>An initialized `KeyDownEvent`.</returns>
        public static EventBase MakeKeyDown(char character, EventModifiers modifiers = EventModifiers.None)
        {
            var evt = new Event() { type = EventType.KeyDown, keyCode = KeyCode.None, character = character, modifiers = modifiers };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized `KeyUpEvent`.
        /// </summary>
        /// <param name="code">The `KeyCode` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>An initialized `KeyUpEvent`.</returns>
        public static EventBase MakeKeyUp(KeyCode code, EventModifiers modifiers = EventModifiers.None)
        {
            var evt = new Event() { type = EventType.KeyUp, keyCode = code, character = '\0', modifiers = modifiers };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized `PointerMoveEvent`.
        /// </summary>
        /// <param name="deltaMove">The Relative position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <param name="clickCount">The number of clicks corresponding to the event.</param>
        /// <returns>An initialized `PointerMoveEvent`.</returns>
        public static EventBase MakeMouseMoveBy(Vector2 deltaMove, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None, int clickCount = 1)
        {
            var evt = new Event() { type = EventType.MouseMove, delta = deltaMove, button = (int)button, modifiers = modifiers, clickCount = clickCount };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized `PointerMoveEvent`.
        /// </summary>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <param name="clickCount">The number of clicks corresponding to the event.</param>
        /// <returns>An initialized `PointerMoveEvent`.</returns>
        public static EventBase MakeMouseMoveTo(Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None, int clickCount = 1)
        {
            var evt = new Event() { type = EventType.MouseMove, mousePosition = position, button = (int)button, modifiers = modifiers, clickCount = clickCount };
            return CreateEvent(evt);
        }

        // MouseMove events require both position and delta to be set.
        // Ticket https://jira.unity3d.com/browse/ATTQA-14 has been created to track
        // the work required to update the other functions to calculate the missing parameter.
        /// <summary>
        /// Creates an initialized `PointerMoveEvent`.
        /// </summary>
        /// <param name="position">The Absolute position for the event. The position the mouse will move to.</param>
        /// <param name="delta">The delta by which the mouse will be moved.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>An initialized `PointerMoveEvent`.</returns>
        public static EventBase MakeMouseMoveEvent(Vector2 position, Vector2 delta, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            var evt = new Event() { type = EventType.MouseMove, mousePosition = position, delta = delta, button = (int)button, modifiers = modifiers };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized `PointerDownEvent`.
        /// </summary>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <param name="clickCount">The number of clicks corresponding to the event.</param>
        /// <returns>An initialized `PointerDownEvent`.</returns>
        public static EventBase MakeMouseDownAt(Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None, int clickCount = 1)
        {
            var evt = new Event() { type = EventType.MouseDown, mousePosition = position, button = (int)button, modifiers = modifiers, clickCount = clickCount };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized `PointerUpEvent`.
        /// </summary>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <param name="clickCount">The number of clicks corresponding to the event.</param>
        /// <returns>An initialized `PointerUpEvent`.</returns>
        public static EventBase MakeMouseUpAt(Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None, int clickCount = 1)
        {
            var evt = new Event() { type = EventType.MouseUp, mousePosition = position, button = (int)button, modifiers = modifiers, clickCount = clickCount };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized Pointer event.
        /// </summary>
        /// <param name="type">The `EventType` for the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <param name="clickCount">The number of clicks corresponding to the event.</param>
        /// <returns>An initialized Pointer event.</returns>
        public static EventBase MakeMouseEvent(EventType type, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None, int clickCount = 1)
        {
            var evt = new Event() { type = type, mousePosition = position, button = (int)button, modifiers = modifiers, clickCount = clickCount };
            return CreateEvent(evt);
        }

        /// <summary>
        /// Creates an initialized Pointer event based on a `TouchPhase`.
        /// </summary>
        /// <param name="phase"><para>The `TouchPhase` for the event.</para>
        /// <para>When <paramref name="phase"/> is `Began`, creates a `PointerDownEvent`.</para>
        /// <para>When <paramref name="phase"/> is `Moved`, creates a `PointerMoveEvent`.</para>
        /// <para>When <paramref name="phase"/> is `Ended`, creates a `PointerUpEvent`.</para>
        /// </param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <param name="fingerId">The finger for the event. Default is `0`.</param>
        /// <returns>An initialized `PointerDownEvent`, `PointerUpEvent`, or `PointerMoveEvent`.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the `TouchPhase` is not within the acceptable values.</exception>
        public static EventBase MakePointerEvent(TouchPhase phase, Vector2 position, EventModifiers modifiers = EventModifiers.None, int fingerId = 0)
        {
            var touch = MakeDefaultTouch();
            touch.fingerId = fingerId;
            touch.position = position;
            touch.phase = phase;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    return PointerDownEvent.GetPooled(touch, modifiers);
                case TouchPhase.Moved:
                    return PointerMoveEvent.GetPooled(touch, modifiers);
                case TouchPhase.Ended:
                    return PointerUpEvent.GetPooled(touch, modifiers);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Creates an initialized Pointer event based on a `PenEventType`.
        /// </summary>
        /// <param name="contactType"><para>The `PenEventType` for the event.</para>
        /// <para>When <paramref name="contactType"/> is `PenDown`, creates a `PointerDownEvent`.</para>
        /// <para>When <paramref name="contactType"/> is `NoContact`, creates a `PointerMoveEvent`.</para>
        /// <para>When <paramref name="contactType"/> is `PenUp`, creates a `PointerUpEvent`.</para>
        /// </param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>An initialized `PointerDownEvent`, `PointerUpEvent`, or `PointerMoveEvent`.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the `PenEventType` is not within the acceptable values.</exception>
        public static EventBase MakePenEvent(PenEventType contactType, Vector2 position, EventModifiers modifiers = EventModifiers.None)
        {
            var penData = new PenData();
            penData.contactType = contactType;
            penData.penStatus = PenStatus.None;
            penData.position = position;

            switch (contactType)
            {
                case PenEventType.PenDown:
                    return PointerDownEvent.GetPooled(penData, modifiers);
                case PenEventType.PenUp:
                    return PointerUpEvent.GetPooled(penData, modifiers);
                case PenEventType.NoContact:
                    return PointerMoveEvent.GetPooled(penData, modifiers);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Creates an initialized `WheelEvent`.
        /// </summary>
        /// <param name="delta">Scroll delta for the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <returns>An initialized `WheelEvent`.</returns>
        public static EventBase MakeScrollWheelEvent(Vector2 delta, Vector2 position)
        {
            var evt = new Event
            {
                type = EventType.ScrollWheel,
                delta = delta,
                mousePosition = position
            };

            return CreateEvent(evt);
        }

        /// <summary>
        /// Command enum for existing Commands.
        /// </summary>
        public enum Command
        {
            /// <summary>
            /// Cut Command.
            /// </summary>
            Cut,
            /// <summary>
            /// Copy Command.
            /// </summary>
            Copy,
            /// <summary>
            /// Paste Command.
            /// </summary>
            Paste,
            /// <summary>
            /// SelectAll Command.
            /// </summary>
            SelectAll,
            /// <summary>
            /// DeselectAll Command.
            /// </summary>
            DeselectAll,
            /// <summary>
            /// InvertSelection Command.
            /// </summary>
            InvertSelection,
            /// <summary>
            /// Duplicate Command.
            /// </summary>
            Duplicate,
            /// <summary>
            /// Rename Command.
            /// </summary>
            Rename,
            /// <summary>
            /// Delete Command.
            /// </summary>
            Delete,
            /// <summary>
            /// SoftDelete Command.
            /// </summary>
            SoftDelete,
            /// <summary>
            /// Find Command.
            /// </summary>
            Find,
            /// <summary>
            /// SelectChildren Command.
            /// </summary>
            SelectChildren,
            /// <summary>
            /// SelectPrefabRoot Command.
            /// </summary>
            SelectPrefabRoot,
            /// <summary>
            /// UndoRedoPerformed Command.
            /// </summary>
            UndoRedoPerformed,
            /// <summary>
            /// OnLostFocus Command.
            /// </summary>
            OnLostFocus,
            /// <summary>
            /// NewKeyboardFocus Command.
            /// </summary>
            NewKeyboardFocus,
            /// <summary>
            /// ModifierKeysChanged Command.
            /// </summary>
            ModifierKeysChanged,
            /// <summary>
            /// EyeDropperUpdate Command.
            /// </summary>
            EyeDropperUpdate,
            /// <summary>
            /// EyeDropperClicked Command.
            /// </summary>
            EyeDropperClicked,
            /// <summary>
            /// EyeDroppedCancelled Command.
            /// </summary>
            EyeDropperCancelled,
            /// <summary>
            /// ColorPickerChanged Command.
            /// </summary>
            ColorPickerChanged,
            /// <summary>
            /// FrameSelected Command.
            /// </summary>
            FrameSelected,
            /// <summary>
            /// FrameSelectedWithLock Command.
            /// </summary>
            FrameSelectedWithLock
        }

        #region Mappings from character to KeyCode and EventModifier
        private static KeyValuePair<char, (KeyCode, EventModifiers)> KeyCodeModifiersMapping(char character, (KeyCode, EventModifiers) mapping)
        {
            return new KeyValuePair<char, (KeyCode, EventModifiers)>(character, mapping);
        }

        internal static Dictionary<char, (KeyCode, EventModifiers)> charKeyCodeModifiersMapping = new Dictionary<char, (KeyCode, EventModifiers)>(new[]
        {
            // Lowercase Letters.
            KeyCodeModifiersMapping('a', (KeyCode.A, EventModifiers.None)),
            KeyCodeModifiersMapping('b', (KeyCode.B, EventModifiers.None)),
            KeyCodeModifiersMapping('c', (KeyCode.C, EventModifiers.None)),
            KeyCodeModifiersMapping('d', (KeyCode.D, EventModifiers.None)),
            KeyCodeModifiersMapping('e', (KeyCode.E, EventModifiers.None)),
            KeyCodeModifiersMapping('f', (KeyCode.F, EventModifiers.None)),
            KeyCodeModifiersMapping('g', (KeyCode.G, EventModifiers.None)),
            KeyCodeModifiersMapping('h', (KeyCode.H, EventModifiers.None)),
            KeyCodeModifiersMapping('i', (KeyCode.I, EventModifiers.None)),
            KeyCodeModifiersMapping('j', (KeyCode.J, EventModifiers.None)),
            KeyCodeModifiersMapping('k', (KeyCode.K, EventModifiers.None)),
            KeyCodeModifiersMapping('l', (KeyCode.L, EventModifiers.None)),
            KeyCodeModifiersMapping('m', (KeyCode.M, EventModifiers.None)),
            KeyCodeModifiersMapping('n', (KeyCode.N, EventModifiers.None)),
            KeyCodeModifiersMapping('o', (KeyCode.O, EventModifiers.None)),
            KeyCodeModifiersMapping('p', (KeyCode.P, EventModifiers.None)),
            KeyCodeModifiersMapping('q', (KeyCode.Q, EventModifiers.None)),
            KeyCodeModifiersMapping('r', (KeyCode.R, EventModifiers.None)),
            KeyCodeModifiersMapping('s', (KeyCode.S, EventModifiers.None)),
            KeyCodeModifiersMapping('t', (KeyCode.T, EventModifiers.None)),
            KeyCodeModifiersMapping('u', (KeyCode.U, EventModifiers.None)),
            KeyCodeModifiersMapping('v', (KeyCode.V, EventModifiers.None)),
            KeyCodeModifiersMapping('w', (KeyCode.W, EventModifiers.None)),
            KeyCodeModifiersMapping('x', (KeyCode.X, EventModifiers.None)),
            KeyCodeModifiersMapping('y', (KeyCode.Y, EventModifiers.None)),
            KeyCodeModifiersMapping('z', (KeyCode.Z, EventModifiers.None)),

            // Uppercase Letters.
            KeyCodeModifiersMapping('A', (KeyCode.A, EventModifiers.Shift)),
            KeyCodeModifiersMapping('B', (KeyCode.B, EventModifiers.Shift)),
            KeyCodeModifiersMapping('C', (KeyCode.C, EventModifiers.Shift)),
            KeyCodeModifiersMapping('D', (KeyCode.D, EventModifiers.Shift)),
            KeyCodeModifiersMapping('E', (KeyCode.E, EventModifiers.Shift)),
            KeyCodeModifiersMapping('F', (KeyCode.F, EventModifiers.Shift)),
            KeyCodeModifiersMapping('G', (KeyCode.G, EventModifiers.Shift)),
            KeyCodeModifiersMapping('H', (KeyCode.H, EventModifiers.Shift)),
            KeyCodeModifiersMapping('I', (KeyCode.I, EventModifiers.Shift)),
            KeyCodeModifiersMapping('J', (KeyCode.J, EventModifiers.Shift)),
            KeyCodeModifiersMapping('K', (KeyCode.K, EventModifiers.Shift)),
            KeyCodeModifiersMapping('L', (KeyCode.L, EventModifiers.Shift)),
            KeyCodeModifiersMapping('M', (KeyCode.M, EventModifiers.Shift)),
            KeyCodeModifiersMapping('N', (KeyCode.N, EventModifiers.Shift)),
            KeyCodeModifiersMapping('O', (KeyCode.O, EventModifiers.Shift)),
            KeyCodeModifiersMapping('P', (KeyCode.P, EventModifiers.Shift)),
            KeyCodeModifiersMapping('Q', (KeyCode.Q, EventModifiers.Shift)),
            KeyCodeModifiersMapping('R', (KeyCode.R, EventModifiers.Shift)),
            KeyCodeModifiersMapping('S', (KeyCode.S, EventModifiers.Shift)),
            KeyCodeModifiersMapping('T', (KeyCode.T, EventModifiers.Shift)),
            KeyCodeModifiersMapping('U', (KeyCode.U, EventModifiers.Shift)),
            KeyCodeModifiersMapping('V', (KeyCode.V, EventModifiers.Shift)),
            KeyCodeModifiersMapping('W', (KeyCode.W, EventModifiers.Shift)),
            KeyCodeModifiersMapping('X', (KeyCode.X, EventModifiers.Shift)),
            KeyCodeModifiersMapping('Y', (KeyCode.Y, EventModifiers.Shift)),
            KeyCodeModifiersMapping('Z', (KeyCode.Z, EventModifiers.Shift)),

            // Numbers.
            KeyCodeModifiersMapping('1', (KeyCode.Alpha1, EventModifiers.None)),
            KeyCodeModifiersMapping('2', (KeyCode.Alpha2, EventModifiers.None)),
            KeyCodeModifiersMapping('3', (KeyCode.Alpha3, EventModifiers.None)),
            KeyCodeModifiersMapping('4', (KeyCode.Alpha4, EventModifiers.None)),
            KeyCodeModifiersMapping('5', (KeyCode.Alpha5, EventModifiers.None)),
            KeyCodeModifiersMapping('6', (KeyCode.Alpha6, EventModifiers.None)),
            KeyCodeModifiersMapping('7', (KeyCode.Alpha7, EventModifiers.None)),
            KeyCodeModifiersMapping('8', (KeyCode.Alpha8, EventModifiers.None)),
            KeyCodeModifiersMapping('9', (KeyCode.Alpha9, EventModifiers.None)),
            KeyCodeModifiersMapping('0', (KeyCode.Alpha0, EventModifiers.None)),

            // Non-Shift Symbols.
            KeyCodeModifiersMapping('`', (KeyCode.BackQuote, EventModifiers.None)),
            KeyCodeModifiersMapping('-', (KeyCode.Minus, EventModifiers.None)),
            KeyCodeModifiersMapping('=', (KeyCode.Equals, EventModifiers.None)),
            KeyCodeModifiersMapping('[', (KeyCode.LeftBracket, EventModifiers.None)),
            KeyCodeModifiersMapping(']', (KeyCode.RightBracket, EventModifiers.None)),
            KeyCodeModifiersMapping('\\', (KeyCode.Backslash, EventModifiers.None)),
            KeyCodeModifiersMapping(';', (KeyCode.Semicolon, EventModifiers.None)),
            KeyCodeModifiersMapping('\'', (KeyCode.Quote, EventModifiers.None)),
            KeyCodeModifiersMapping(',', (KeyCode.Comma, EventModifiers.None)),
            KeyCodeModifiersMapping('.', (KeyCode.Period, EventModifiers.None)),
            KeyCodeModifiersMapping('/', (KeyCode.Slash, EventModifiers.None)),

            // Shift Symbols.
            KeyCodeModifiersMapping('~', (KeyCode.BackQuote, EventModifiers.Shift)),
            KeyCodeModifiersMapping('!', (KeyCode.Alpha1, EventModifiers.Shift)),
            KeyCodeModifiersMapping('@', (KeyCode.Alpha2, EventModifiers.Shift)),
            KeyCodeModifiersMapping('#', (KeyCode.Alpha3, EventModifiers.Shift)),
            KeyCodeModifiersMapping('$', (KeyCode.Alpha4, EventModifiers.Shift)),
            KeyCodeModifiersMapping('%', (KeyCode.Alpha5, EventModifiers.Shift)),
            KeyCodeModifiersMapping('^', (KeyCode.Alpha6, EventModifiers.Shift)),
            KeyCodeModifiersMapping('&', (KeyCode.Alpha7, EventModifiers.Shift)),
            KeyCodeModifiersMapping('*', (KeyCode.Alpha8, EventModifiers.Shift)),
            KeyCodeModifiersMapping('(', (KeyCode.Alpha9, EventModifiers.Shift)),
            KeyCodeModifiersMapping(')', (KeyCode.Alpha0, EventModifiers.Shift)),
            KeyCodeModifiersMapping('_', (KeyCode.Minus, EventModifiers.Shift)),
            KeyCodeModifiersMapping('+', (KeyCode.Equals, EventModifiers.Shift)),
            KeyCodeModifiersMapping('{', (KeyCode.LeftBracket, EventModifiers.Shift)),
            KeyCodeModifiersMapping('}', (KeyCode.RightBracket, EventModifiers.Shift)),
            KeyCodeModifiersMapping('|', (KeyCode.Backslash, EventModifiers.Shift)),
            KeyCodeModifiersMapping(':', (KeyCode.Semicolon, EventModifiers.Shift)),
            KeyCodeModifiersMapping('"', (KeyCode.Quote, EventModifiers.Shift)),
            KeyCodeModifiersMapping('<', (KeyCode.Comma, EventModifiers.Shift)),
            KeyCodeModifiersMapping('>', (KeyCode.Period, EventModifiers.Shift)),
            KeyCodeModifiersMapping('?', (KeyCode.Slash, EventModifiers.Shift)),

            // Other.
            KeyCodeModifiersMapping(' ', (KeyCode.Space, EventModifiers.None))
        });

        internal static Dictionary<char, (KeyCode, EventModifiers)> charKeyCodeModifiersMappingKeyPad = charKeyCodeModifiersMapping.CopyAndReplace(
            // Keypad alternates for numbers.
            KeyCodeModifiersMapping('1', (KeyCode.Keypad1, EventModifiers.None)),
            KeyCodeModifiersMapping('2', (KeyCode.Keypad2, EventModifiers.None)),
            KeyCodeModifiersMapping('3', (KeyCode.Keypad3, EventModifiers.None)),
            KeyCodeModifiersMapping('4', (KeyCode.Keypad4, EventModifiers.None)),
            KeyCodeModifiersMapping('5', (KeyCode.Keypad5, EventModifiers.None)),
            KeyCodeModifiersMapping('6', (KeyCode.Keypad6, EventModifiers.None)),
            KeyCodeModifiersMapping('7', (KeyCode.Keypad7, EventModifiers.None)),
            KeyCodeModifiersMapping('8', (KeyCode.Keypad8, EventModifiers.None)),
            KeyCodeModifiersMapping('9', (KeyCode.Keypad9, EventModifiers.None)),
            KeyCodeModifiersMapping('0', (KeyCode.Keypad0, EventModifiers.None)),

            // Keypad alternates for symbols.
            KeyCodeModifiersMapping('-', (KeyCode.KeypadMinus, EventModifiers.None)),
            KeyCodeModifiersMapping('=', (KeyCode.KeypadEquals, EventModifiers.None)),
            KeyCodeModifiersMapping('.', (KeyCode.KeypadPeriod, EventModifiers.None)),
            KeyCodeModifiersMapping('/', (KeyCode.KeypadDivide, EventModifiers.None)),
            KeyCodeModifiersMapping('*', (KeyCode.KeypadMultiply, EventModifiers.None)),
            KeyCodeModifiersMapping('+', (KeyCode.KeypadPlus, EventModifiers.None))
        );

        private static Dictionary<char, (KeyCode, EventModifiers)> CopyAndReplace(this Dictionary<char, (KeyCode, EventModifiers)> baseDictionary, params KeyValuePair<char, (KeyCode, EventModifiers)>[] replacementValues)
        {
            var dict = new Dictionary<char, (KeyCode, EventModifiers)>(baseDictionary);
            foreach (var (character, mapping) in replacementValues)
            {
                dict[character] = mapping;
            }
            return dict;
        }
        #endregion

        // Based on Event.KeyboardEvent, some keys should always send a function modifier.
        internal static bool ShouldSendFunctionModifier(KeyCode code)
        {
            switch (code)
            {
                case KeyCode.PageUp:
                case KeyCode.PageDown:
                case KeyCode.End:
                case KeyCode.Home:
                case KeyCode.LeftArrow:
                case KeyCode.RightArrow:
                case KeyCode.UpArrow:
                case KeyCode.DownArrow:
                case KeyCode.Print:
                case KeyCode.Insert:
                case KeyCode.Backspace:
                case KeyCode.Delete:
                case KeyCode.Help:
                case KeyCode.F1:
                case KeyCode.F2:
                case KeyCode.F3:
                case KeyCode.F4:
                case KeyCode.F5:
                case KeyCode.F6:
                case KeyCode.F7:
                case KeyCode.F8:
                case KeyCode.F9:
                case KeyCode.F10:
                case KeyCode.F11:
                case KeyCode.F12:
                    // Commented out for now as this may be a bug - https://jira.unity3d.com/browse/UUM-63632
                    //#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
                    //                case KeyCode.LeftShift:
                    //                case KeyCode.RightShift:
                    //                case KeyCode.LeftControl:
                    //                case KeyCode.RightControl:
                    //                case KeyCode.LeftAlt:
                    //                case KeyCode.RightAlt:
                    //#endif
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates an initialized `Command` event.
        /// </summary>
        /// <param name="type">The `EventType` for the event.
        /// If <paramref name="type"/> is `ExecuteCommand`, creates an `ExecuteCommandEvent`.
        /// If <paramref name="type"/> is `ValidateCommand`, creates a `ValidateCommandEvent`.
        /// </param>
        /// <param name="command">The command that should be performed.</param>
        /// <returns>An initialized `ExecuteCommand` or `ValidateCommand` event.</returns>
        public static EventBase MakeCommandEvent(EventType type, Command command)
        {
            return MakeCommandEvent(type, command.ToString());
        }

        /// <summary>
        /// Creates an initialized `Command` event.
        /// </summary>
        /// <param name="type">The `EventType` for the event.
        /// If <paramref name="type"/> is `ExecuteCommand`, creates an `ExecuteCommandEvent`.
        /// If <paramref name="type"/> is `ValidateCommand`, creates a `ValidateCommandEvent`.
        /// </param>
        /// <param name="command">The command that should be performed.</param>
        /// <returns>An initialized `ExecuteCommand` or `ValidateCommand` event.</returns>
        public static EventBase MakeCommandEvent(EventType type, string command)
        {
            var evt = new Event() { type = type, commandName = command };
            return CreateEvent(evt);
        }

        private static Touch MakeDefaultTouch()
        {
            var touch = new Touch();
            touch.fingerId = 0;
            touch.rawPosition = touch.position;
            touch.deltaPosition = Vector2.zero;
            touch.deltaTime = 0;
            touch.tapCount = 1;
            touch.pressure = 0.5f;
            touch.maximumPossiblePressure = 1;
            touch.type = TouchType.Direct;
            touch.altitudeAngle = 0;
            touch.azimuthAngle = 0;
            touch.radius = 1;
            touch.radiusVariance = 0;

            return touch;
        }
    }

    /// <summary>
    /// Class containing functionality related to simulating UI interactions within the Editor and Runtime, unless explicitly specified.
    /// </summary>
    internal static class SimulateRuntimeHelpers
    {
        /// <summary>
        /// Sends a single click to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="position">The Absolute position for the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateClick(this VisualElement ve, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.DispatchAndWaitForNextFrame(new EventBase[]
            {
                EventHelpers.MakeMouseDownAt(position, button, modifiers, 1),
                EventHelpers.MakeMouseUpAt(position, button, modifiers, 1)
            });
        }

        /// <summary>
        /// Sends a single click to the <paramref name="ve"/>'s panel.
        /// The position of the click is the center of the <paramref name="ve"/>'s worldBound.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateClick(this VisualElement ve, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.SimulateClick(ve.worldBound.center, button, modifiers);
        }

        /// <summary>
        /// Sends a double click to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="position">The Absolute position for the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateDoubleClick(this VisualElement ve, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.DispatchAndWaitForNextFrame(new EventBase[]
            {
                EventHelpers.MakeMouseDownAt(position, button, modifiers, 1),
                EventHelpers.MakeMouseUpAt(position, button, modifiers, 1),
                EventHelpers.MakeMouseDownAt(position, button, modifiers, 2),
                EventHelpers.MakeMouseUpAt(position, button, modifiers, 1)
            });
        }

        /// <summary>
        /// Sends a double click to the <paramref name="ve"/>'s panel.
        /// The position of the click is the center of the <paramref name="ve"/>'s worldBound.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateDoubleClick(this VisualElement ve, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.SimulateDoubleClick(ve.worldBound.center, button, modifiers);
        }

        /// <summary>
        /// Sends a `PointerDownEvent` to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseDownAt(this VisualElement ve, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeMouseDownAt(position, button, modifiers));
        }

        /// <summary>
        /// Sends a `PointerUpEvent` to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="button">The `MouseButton` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseUpAt(this VisualElement ve, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeMouseUpAt(position, button, modifiers));
        }

        /// <summary>
        /// Sends incremental `PointerMoveEvent`s to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="positionFrom">The Absolute starting position of the Mouse.</param>
        /// <param name="positionTo">The Absolute final position to move the Mouse to.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseMove(this VisualElement ve, Vector2 positionFrom, Vector2 positionTo, EventModifiers modifiers = EventModifiers.None)
        {
            const float steps = 9f;

            // Set the initial mouse position to the starting position.
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeMouseMoveEvent(positionFrom, Vector2.zero, modifiers: modifiers));

            var dragDistance = Vector2.Distance(positionFrom, positionTo);
            var dragSpeed = Mathf.Max(1f, dragDistance / steps);
            var normalizedDirection = (positionTo - positionFrom).normalized;

            var nextMousePosition = positionFrom;
            var delta = dragSpeed * normalizedDirection;
            for (int i = 1; i < steps; i++)
            {
                nextMousePosition += delta;
                yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeMouseMoveEvent(nextMousePosition, delta, modifiers: modifiers));
            }

            // To account for rounding errors, the last increment will be slightly bigger than the previous ones.
            delta = positionTo - nextMousePosition;
            nextMousePosition += delta;
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeMouseMoveEvent(nextMousePosition, delta, modifiers: modifiers));
        }

        /// <summary>
        /// Sends a `PointerMoveEvent` to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateMouseMoveTo(this VisualElement ve, Vector2 position, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeMouseMoveTo(position, modifiers: modifiers));
        }

        /// <summary>
        /// Sends a `PointerDownEvent`, incremental `PointerMoveEvent`s, and a `PointerUpEvent`,
        /// in that order, to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="positionFrom">The Absolute starting position of the Mouse.</param>
        /// <param name="positionTo">The Absolute final position to move the Mouse to.</param>
        /// <param name="button">The `MouseButton` for the events.</param>
        /// <param name="modifiers">The `EventModifiers` for the events.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateDragAndDrop(this VisualElement ve, Vector2 positionFrom, Vector2 positionTo, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None)
        {
            yield return ve.SimulateMouseDownAt(positionFrom, button, modifiers);
            yield return ve.SimulateMouseMove(positionFrom, positionTo, modifiers);
            yield return ve.SimulateMouseUpAt(positionTo, button, modifiers);
        }

        /// <summary>
        /// Sends a `WheelEvent` to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="delta">The delta (scroll amount) for the event.</param>
        /// <param name="position">The Absolute position for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateScrollWheel(this VisualElement ve, Vector2 delta, Vector2 position)
        {
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeScrollWheelEvent(delta, position));
        }

        /// <summary>
        /// Sends key events to the <paramref name="ve"/>'s panel to simulate typing the given text.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="text">Text to type.</param>
        /// <param name="useKeypad">Whether keypad `KeyCodes` (e.g. `Keypad0`, `KeypadMinus`)
        /// should be used instead of Alpha `KeyCodes` (e.g. `Alpha0`, `Minus`).
        /// Default is `false`.</param>
        /// <remarks>
        /// <para>Only officially supports US layout keyboard and English language.</para>
        /// <para>On Windows, Linux, and Mac runtime platforms, sends UI Toolkit key events.</para>
        /// <para>On most other runtime platforms, sets the text using the TouchScreenKeyboard.</para>
        /// <para>On Switch, sets the text directly on the TextElement.</para>
        /// <para>On Switch, if VisualElement ve is not a TextElement, throws an exception.</para>
        /// </remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateTypingText(this VisualElement ve, string text, bool useKeypad = false)
        {
            // Fetch the dictionary to use in this case.
            var dict = useKeypad ? EventHelpers.charKeyCodeModifiersMappingKeyPad : EventHelpers.charKeyCodeModifiersMapping;
            long frame = 0;

            foreach (var character in text)
            {
                KeyCode keyCode;
                EventModifiers modifiers;

                if (Application.platform == RuntimePlatform.Switch)
                {
                    // On Nintendo Switch, popping the keyboard pauses execution of the test.
                    // To avoid using the TouchScreenKeyboard, set the text directly on the TextField or TextElement.
                    if (ve is TextElement te)
                    {
                        te.text = text;
                        break;
                    }
                    else
                    {
                        throw new NotSupportedException("Only TextElement are supported for SimulateTypingText on Switch.");
                    }
                }

                if (TouchScreenTextEditorEventHandler.activeTouchScreenKeyboard != null &&
                    TouchScreenTextEditorEventHandler.activeTouchScreenKeyboard.active)
                {
                    // If a touch screen keyboard is active, send the characters directly to it.
                    // TouchScreenKeyboards don't react to KeyDown or KeyUp events.
                    frame = TouchScreenTextEditorEventHandler.Frame;

                    // Modifying the text character by character is currently unstable due to threading issues.
                    // For now, set the entire text all at once and break.
                    TouchScreenTextEditorEventHandler.activeTouchScreenKeyboard.text = text;
                    yield return EventHelpers.FrameWaiterFunction?.Invoke();
                    break;
                }

                if (!dict.TryGetValue(character, out (KeyCode, EventModifiers) mapping))
                {
                    // If a mapping isn't found, just send a KeyDown with the character and move on to the next character.
                    // This isn't foolproof but it's better than not sending anything.
                    // No KeyUp is sent in this case because KeyUp usually isn't sent for characters, only KeyCodes.
                    yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeKeyDown(character));
                    continue;
                }

                keyCode = mapping.Item1;
                modifiers = mapping.Item2;

                // In real life, both KeyDown events are sent in the same frame,
                // so that's what's done here as well.
                ve.Dispatch(EventHelpers.MakeKeyDown(keyCode, modifiers));
                yield return ve.DispatchAndWaitForNextFrame(
                    EventHelpers.MakeKeyDown(character, modifiers),
                    EventHelpers.MakeKeyUp(keyCode, modifiers));
            }

            // TouchScreenTextEditorEventHandler polls for updates every few milliseconds.
            // Wait until the next frame after the last character has been sent.
            float time = Time.realtimeSinceStartup + 1f;
            while (Time.realtimeSinceStartup < time)
            {
                if (TouchScreenTextEditorEventHandler.activeTouchScreenKeyboard == null ||
                    TouchScreenTextEditorEventHandler.Frame > frame)
                {
                    break;
                }
                yield return EventHelpers.FrameWaiterFunction?.Invoke();
            }
        }

        /// <summary>
        /// Sends key events to the <paramref name="ve"/>'s panel to simulate pressing a given key.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="code">The `KeyCode` for the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <remarks>
        /// <para>To send text, use <see cref="SimulateTypingText(VisualElement, string, bool)"/>.</para>
        /// <para>To send `Return`, use <see cref="SimulateReturnKey(VisualElement, EventModifiers)"/>.</para>
        /// <para>To send `KeypadEnter`, use <see cref="SimulateKeypadEnterKey(VisualElement, EventModifiers)"/>.</para>
        /// <para>To send `Tab`, use <see cref="SimulateTabKey(VisualElement, EventModifiers)"/>.</para>
        /// </remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateKey(this VisualElement ve, KeyCode code, EventModifiers modifiers = EventModifiers.None)
        {
            // Some keys send the FunctionKey modifier even if it isn't specified, so the helper also sends it.
            if (EventHelpers.ShouldSendFunctionModifier(code))
            {
                modifiers |= EventModifiers.FunctionKey;
            }

            yield return ve.DispatchAndWaitForNextFrame(
                EventHelpers.MakeKeyDown(code, modifiers),
                EventHelpers.MakeKeyUp(code, modifiers));
        }

        /// <summary>
        /// Sends key events to the <paramref name="ve"/>'s panel to simulate pressing the Return key.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateReturnKey(this VisualElement ve, EventModifiers modifiers = EventModifiers.None)
        {
            // In real life, both KeyDown events are sent in the same frame,
            // so that's what's done here as well.
            ve.Dispatch(EventHelpers.MakeKeyDown(KeyCode.Return, modifiers));
            yield return ve.DispatchAndWaitForNextFrame(
                EventHelpers.MakeKeyDown('\n', modifiers),
                EventHelpers.MakeKeyUp(KeyCode.Return, modifiers));
        }

        /// <summary>
        /// Sends key events to the <paramref name="ve"/>'s panel to simulate pressing the KeypadEnter key.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateKeypadEnterKey(this VisualElement ve, EventModifiers modifiers = EventModifiers.None)
        {
            // In real life, both KeyDown events are sent in the same frame,
            // so that's what's done here as well.
            ve.Dispatch(EventHelpers.MakeKeyDown(KeyCode.KeypadEnter, modifiers));
            yield return ve.DispatchAndWaitForNextFrame(
                EventHelpers.MakeKeyDown('\n', modifiers),
                EventHelpers.MakeKeyUp(KeyCode.KeypadEnter, modifiers));
        }

        /// <summary>
        /// Sends key events to the <paramref name="ve"/>'s panel to simulate pressing the Tab key.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="modifiers">The `EventModifiers` for the event.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateTabKey(this VisualElement ve, EventModifiers modifiers = EventModifiers.None)
        {
            char tabChar = '\t';
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            // On MacOS, Shift Tab sends the character 25 instead of \t.
            if ((modifiers & EventModifiers.Shift) != 0)
            {
                tabChar = (char)25;
            }
#endif
            // In real life, both KeyDown events are sent in the same frame,
            // so that's what's done here as well.
            ve.Dispatch(EventHelpers.MakeKeyDown(KeyCode.Tab, modifiers));
            yield return ve.DispatchAndWaitForNextFrame(
                EventHelpers.MakeKeyDown(tabChar, modifiers),
                EventHelpers.MakeKeyUp(KeyCode.Tab, modifiers));
        }

        /// <summary>
        /// Sends an `ExecuteCommandEvent` for the specified <paramref name="commandName"/> to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <remarks>`ExecuteCommandEvent` are only officially supported in Editor (not Runtime).</remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateExecuteCommand(this VisualElement ve, string commandName)
        {
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeCommandEvent(EventType.ExecuteCommand, commandName));
        }

        /// <summary>
        /// Sends an `ExecuteCommandEvent` for the specified <paramref name="command"/> to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="command">The Command to execute.</param>
        /// <remarks>`ExecuteCommandEvent` are only officially supported in Editor (not Runtime).</remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateExecuteCommand(this VisualElement ve, EventHelpers.Command command)
        {
            yield return ve.SimulateExecuteCommand(command.ToString());
        }

        /// <summary>
        /// Sends an `ValidateCommandEvent` for the specified <paramref name="commandName"/> to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <remarks>`ValidateCommandEvent` are only officially supported in Editor (not Runtime).</remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateValidateCommand(this VisualElement ve, string commandName)
        {
            yield return ve.DispatchAndWaitForNextFrame(EventHelpers.MakeCommandEvent(EventType.ValidateCommand, commandName));
        }

        /// <summary>
        /// Sends an `ValidateCommandEvent` for the specified <paramref name="command"/> to the <paramref name="ve"/>'s panel.
        /// Waits for the next frame after the event.
        /// After the event has been sent, disposes of the event.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the event.</param>
        /// <param name="command">The Command to execute.</param>
        /// <remarks>`ValidateCommandEvent` are only officially supported in Editor (not Runtime).</remarks>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator SimulateValidateCommand(this VisualElement ve, EventHelpers.Command command)
        {
            yield return ve.SimulateValidateCommand(command.ToString());
        }
    }

    /// <summary>
    /// Class containing functionality for dispatching and disposing of events within the Editor or at Runtime.
    /// </summary>
    internal static class DispatchRuntimeHelpers
    {
        /// <summary>
        /// Frame waiter used during event dispatching or simulation calls.
        /// </summary>
        public class FrameWaiter : IDisposable
        {
            /// <summary>
            /// Previous waiter function to restore after
            /// the current waiter function is disposed on exiting the using() pattern.
            /// </summary>
            private Func<IEnumerator> previousWaiterFunction = null;

            /// <summary>
            /// Resets the frame waiter to the previous waiter function.
            /// This will be called automatically during the using() pattern.
            /// Otherwise, the frame waiter will not be reset.
            /// </summary>
            public void Dispose()
            {
                EventHelpers.FrameWaiterFunction = previousWaiterFunction;
            }

            /// <summary>
            /// Creates an Enumerator from a void function.
            /// </summary>
            /// <param name="func">Void function.</param>
            /// <returns>Enumerator form of the void function.</returns>
            private static IEnumerator CreateFrameWaiter(Action func)
            {
                func();
                yield break;
            }

            /// <summary>
            /// Sets the frame waiter used during event dispatching or simulation calls
            /// to a different Enumerator function.
            /// </summary>
            /// <param name="func">IEnumerator function that will be set as the frame waiter.</param>
            internal FrameWaiter(Func<IEnumerator> func)
            {
                previousWaiterFunction = EventHelpers.FrameWaiterFunction;
                EventHelpers.FrameWaiterFunction = func;
            }

            /// <summary>
            /// Sets the frame waiter used during event dispatching or simulation calls
            /// to a different Enumerator function.
            /// </summary>
            /// <param name="func">Void function that will be set as the frame waiter.</param>
            internal FrameWaiter(Action func)
            {
                previousWaiterFunction = EventHelpers.FrameWaiterFunction;
                EventHelpers.FrameWaiterFunction = new Func<IEnumerator>(() => { return CreateFrameWaiter(func); });
            }
        }

        /// <summary>
        /// Sends one or more events to the <paramref name="ve"/>'s panel.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="evts">`EventBase` events to send.</param>
        public static void Dispatch(this VisualElement ve, params EventBase[] evts)
        {
            try
            {
                foreach (EventBase e in evts)
                {
                    ve.SendEvent(e);
                }
            }
            finally
            {
                foreach (EventBase e in evts)
                {
                    e.Dispose();
                }
            }
        }

        /// <summary>
        /// Sends one or more events to the Panel.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="panel">Panel to receive the events.</param>
        /// <param name="evts">`EventBase` events to send.</param>
        public static void Dispatch(this IPanel panel, params EventBase[] evts)
        {
            try
            {
                foreach (EventBase e in evts)
                {
                    panel.SendEvent(e);
                }
            }
            finally
            {
                foreach (EventBase e in evts)
                {
                    e.Dispose();
                }
            }
        }

        /// <summary>
        /// Sends one or more events to the <paramref name="ve"/>'s panel.
        /// By default, waits one frame after sending each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="ve">`VisualElement` whose panel should receive the events.</param>
        /// <param name="evts">`EventBase` events to send.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        /// <remarks>
        /// The wait function can be set with <see cref="EventHelpers.SetFrameWaiter(Action)"/> or <see cref="EventHelpers.SetFrameWaiter(Func{IEnumerator})"/>.
        /// </remarks>
        public static IEnumerator DispatchAndWaitForNextFrame(this VisualElement ve, params EventBase[] evts)
        {
            try
            {
                foreach (EventBase e in evts)
                {
                    ve.SendEvent(e);
                    yield return EventHelpers.FrameWaiterFunction?.Invoke();
                }
            }
            finally
            {
                foreach (EventBase e in evts)
                {
                    e.Dispose();
                }
            }
        }

        /// <summary>
        /// Sends one or more events to the Panel.
        /// By default, waits one frame after sending each event.
        /// After all events have been sent, disposes of the events.
        /// </summary>
        /// <param name="panel">Panel to receive the events.</param>
        /// <param name="evts">EventBase events to send.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        /// <remarks>
        /// The wait function can be set with <see cref="EventHelpers.SetFrameWaiter(Action)"/> or <see cref="EventHelpers.SetFrameWaiter(Func{IEnumerator})"/>.
        /// </remarks>
        public static IEnumerator DispatchAndWaitForNextFrame(this IPanel panel, params EventBase[] evts)
        {
            try
            {
                foreach (EventBase e in evts)
                {
                    panel.SendEvent(e);
                    yield return EventHelpers.FrameWaiterFunction?.Invoke();
                }
            }
            finally
            {
                foreach (EventBase e in evts)
                {
                    e.Dispose();
                }
            }
        }

        /// <summary>
        /// Function which executes the `IEnumerator` within the same frame.
        /// Can only be used if the <see cref="EventHelpers.frameWaiterFunction"/> is set to empty or to a method that does not
        /// yield any frame.
        /// ! WARNING !
        /// It is strongly discouraged to disable frame yielding in helper functions.
        /// Use only if you are absolutely sure that it is okay to disable frame yielding in your test,
        /// and if you understand how to set and reset the FrameWaiterFunction.
        /// Using `UnityTest` and yielding real frames is greatly preferred
        /// especially when interacting with or verifying the state of the UI.
        /// </summary>
        /// <param name="enumerator">Enumerator to be executed within the same frame.</param>
        public static void ExecuteWithinFrame(this IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                throw new NotSupportedException("Cannot disable frame yielding if Enumerator expects a frame to be yielded.");
            }

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;

                // If the current step is an IEnumerator (for example, in a nested enumerator)
                // recurse through and execute it as well.
                if (current is IEnumerator nestedEnumerator)
                {
                    nestedEnumerator.ExecuteWithinFrame();
                }
                else
                {
                    throw new NotSupportedException("Cannot disable frame yielding if Enumerator expects a frame to be yielded.");
                }
            }
        }
    }

    /// <summary>
    /// Class containing functionality related to Runtime frames.
    /// </summary>
    internal static class RuntimeFrameHelpers
    {
        // The max time to wait for a single frame to increment, 1 second.
        private static readonly double MaxTimeSeconds = 1;

        /// <summary>
        /// Function to wait for the next UI frame.
        /// Performs checks on <paramref name="visualElement"/> panel updaters and scheduler to ensure
        /// that at least one Update loop for each has elapsed.
        /// </summary>
        /// <param name="visualElement">The `VisualElement` whose panel to use for frame verification.</param>
        /// <param name="enumerator">The enumerator which will be executed during each waited frame. Default results in a `yield return null`.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        /// <exception cref="Exception">
        /// If a full update for each frame requested cannot be confirmed within the <see cref="MaxTimeSeconds"/>, throws an exception.
        /// </exception>
        public static IEnumerator UIFrameWaiter(this VisualElement visualElement, Func<IEnumerator> enumerator = null)
        {
            return visualElement.panel.UIFrameWaiter(enumerator);
        }

        /// <summary>
        /// Function to wait for the next UI frame.
        /// Performs checks on <paramref name="panel"/> updaters and scheduler to ensure
        /// that at least one Update loop for each has elapsed.
        /// </summary>
        /// <param name="panel">The `Panel` to use for frame verification.</param>
        /// <param name="enumerator">The enumerator which will be executed during each waited frame. Default results in a `yield return null`.</param>
        /// <returns>`IEnumerator` iterator.</returns>
        /// <exception cref="Exception">
        /// If a full update for each frame requested cannot be confirmed within the <see cref="MaxTimeSeconds"/>, throws an exception.
        /// </exception>
        public static IEnumerator UIFrameWaiter(this IPanel panel, Func<IEnumerator> enumerator = null)
        {
            return panel.UIFrameWaiterBase(enumerator);
        }

        private static IEnumerator UIFrameWaiterBase(this IPanel panel, Func<IEnumerator> enumerator = null)
        {
            if (panel == null)
            {
                Debug.LogError("UIFrameWaiter cannot act on a null Panel.");
                yield break;
            }

            bool wasFullLoopExecutedForFrame = false;
            double endTime = Time.realtimeSinceStartup + MaxTimeSeconds;

            // Get the starting frame state to use for comparison.
            UIFrameState prevFrame = (panel as Panel).GetFrameState();

            do
            {
                yield return enumerator?.Invoke();

                // Get the current frame state.
                UIFrameState currentFrame = (panel as Panel).GetFrameState();

                if (wasFullLoopExecutedForFrame = (currentFrame > prevFrame))
                {
                    break;
                }
            }
            while (Time.realtimeSinceStartup < endTime);

            // If no frame was incremented during this time, throw an exception.
            if (!wasFullLoopExecutedForFrame)
            {
                throw new Exception("UIFrame did not update in time.");
            }
        }

        /// <summary>
        /// Repeats the provided enumerator <paramref name="repeat"/> number of times.
        /// </summary>
        /// <param name="enumerator">Enumerator which will be repeated.</param>
        /// <param name="repeat">Number of times to repeat the enumerator. Must be greater than `1`.</param>
        /// <exception cref="NotSupportedException">Throws exception if <paramref name="repeat"/> is less than or equal to `1`.</exception>
        /// <returns>`IEnumerator` iterator.</returns>
        public static IEnumerator Repeat(Func<IEnumerator> enumerator, int repeat)
        {
            if (repeat <= 1)
            {
                throw new NotSupportedException("Parameter repeat must be greater than 1.");
            }

            for (int i = 0; i < repeat; i++)
            {
                yield return enumerator.Invoke();
            }
        }
    }

    /// <summary>
    /// Class containing IPanel extension methods.
    /// </summary>
    internal static class PanelHelpers
    {
        /// <summary>
        /// Sends event to the Panel using the BaseVisualElementPanel SendEvent.
        /// </summary>
        /// <param name="panel">Panel to receive the event.</param>
        /// <param name="evt">EventBase event to send.</param>
        public static void SendEvent(this IPanel panel, EventBase evt)
        {
            ((BaseVisualElementPanel)panel).SendEvent(evt);
        }
    }

    #endregion // IEnumerator helpers
}

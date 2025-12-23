using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Basic implementation of <see cref="AbstractUITestFixture"/>.
    /// Manages NUnit's lifetime and provides a `PanelSimulator` to simulate interactions with UI Toolkit content.
    /// Provides component-based extensibility for reusable utilities.
    /// Actual usage done via <see cref="UITestFixture"/>, <see cref="T:UnityEditor.UIElements.TestFramework.EditorWindowUITestFixture`1"/>, or <see cref="RuntimeUITestFixture"/>
    /// </summary>
    public abstract class CommonUITestFixture : AbstractUITestFixture
    {
        // Inheritance by test classes is prohibited.
        [System.Obsolete("For Internal Use Only.")]
        internal CommonUITestFixture() { }

        /// <summary>
        /// Internal property used for tracking the state of the test.
        /// The state is affected by the OneTimeSetUp, SetUp, TearDown and OneTimeTearDown calls.
        /// The expected state flow is:
        ///      [Default] ->
        ///          OneTimeSetUp() -> [Initialized] ->
        ///              (Setup() -> [DuringTest] -> TearDown() -> [Initialized]) ->
        ///          OneTimeTearDown() -> [Disposed]
        /// </summary>
        [System.Obsolete("For Internal Use Only.")]
        internal sealed override LifetimeState lifetimeState { get => m_LifetimeState; }

#pragma warning disable CS0618 // Disable warning on Internal usage
        private LifetimeState m_LifetimeState = LifetimeState.Default;
#pragma warning restore CS0618

        private List<UITestComponent> m_TestComponents = new List<UITestComponent>();

        /// <inheritdoc cref="AbstractUITestFixture.AddTestComponent(UITestComponent)"/>
        public sealed override void AddTestComponent(UITestComponent component)
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            AddTestComponent(component, this);
#pragma warning restore CS0618
        }

        [System.Obsolete("For Internal Use Only.")]
        internal void AddTestComponent(UITestComponent component, AbstractUITestFixture owner)
        {
            m_TestComponents.Add(component);

#pragma warning disable CS0618 // Disable warning on Internal usage
            switch (lifetimeState)
            {
                case LifetimeState.Initialized:
                    component.DoInitialize(owner);
                    break;
                case LifetimeState.DuringTest:
                    component.DoInitialize(owner);
                    component.DoBeforeTest();
                    break;
            }
#pragma warning restore CS0618
        }

        /// <inheritdoc cref="AbstractUITestFixture.RemoveTestComponent(UITestComponent)"/>
        public sealed override void RemoveTestComponent(UITestComponent component)
        {
            if (m_TestComponents.Remove(component))
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                switch (lifetimeState)
                {
                    case LifetimeState.DuringTest:
                        component.DoAfterTest();
                        component.DoShutdown();
                        break;
                    case LifetimeState.Initialized:
                        component.DoShutdown();
                        break;
                    default:
                        break;
                }
#pragma warning restore CS0618
            }
        }

        /// <inheritdoc cref="AbstractUITestFixture.FindTestComponent{T}"/>
        public sealed override T FindTestComponent<T>()
        {
            for (int i = 0; i < m_TestComponents.Count; ++i)
            {
                if (m_TestComponents[i] is T component)
                {
                    return component;
                }
            }

            return default;
        }

#pragma warning disable CS0618 // Disable warning on Internal usage
        void ValidateLifecycle(string message, LifetimeState expected)
        {
            if (lifetimeState != expected)
            {
                throw new InvalidOperationException(message);
            }
        }
#pragma warning restore CS0618

        PanelSimulator m_PanelSimulator;

        /// <inheritdoc cref="AbstractUITestFixture.simulate"/>
        public sealed override PanelSimulator simulate
        {
            get => m_PanelSimulator;
            set
            {
                if (value != null)
                {
#pragma warning disable CS0618 // Disable warning on Internal usage
                    ValidateLifecycle("PanelSimulator can only be set before OneTimeSetUp", LifetimeState.Default);
                    value.SetUITestFixture(this);
#pragma warning restore CS0618
                }

                if (m_PanelSimulator != null)
                {
#pragma warning disable CS0618 // Disable warning on Internal usage
                    m_PanelSimulator.SetUITestFixture(null);
#pragma warning restore CS0618
                }

                m_PanelSimulator = value;
            }
        }

        /// <summary>
        /// Clears the rootVisualElement after each test when set to `true`.
        /// </summary>
        /// <remarks>
        /// Defaults to `true`.
        /// </remarks>
        public sealed override bool clearContentAfterTest { get; set; } = true;


        [System.Obsolete("For Internal Use Only.")]
        internal ThemeStyleSheet m_ThemeStyleSheet;

        /// <inheritdoc cref="AbstractUITestFixture.themeStyleSheet"/>
        public override ThemeStyleSheet themeStyleSheet
        {
            get
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                if (m_ThemeStyleSheet != null)
                {
                    return m_ThemeStyleSheet;
                }
#pragma warning restore CS0618

                if (panel != null)
                {
                    var stylesheets = panel.visualTree.styleSheets;

                    for (int i = 0; i < stylesheets.count; ++i)
                    {
                        if (stylesheets[i] is ThemeStyleSheet theme)
                        {
                            return theme;
                        }
                    }
                }

                return null;
            }
#pragma warning disable CS0618 // Disable warning on Internal usage
            set => m_ThemeStyleSheet = value;
#pragma warning restore CS0618
        }

        // Used to query the current test status without an explicit dependency on NUnit.
        [System.Obsolete("For Internal Use Only.")]
        internal ITestStatusProvider testStatus { get; set; } = new NUnitTestStatusProvider();

        /// <inheritdoc cref="AbstractUITestFixture.debugMode"/>
        public override bool debugMode { get; set; } = false;

        /// <inheritdoc cref="AbstractUITestFixture.FixtureOneTimeSetUp()"/>
        public override void FixtureOneTimeSetUp()
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            ValidateLifecycle("OneTimeSetUp() can only be called once after fixture creation.", LifetimeState.Default);
            m_LifetimeState = LifetimeState.Initialized;
#pragma warning restore CS0618

            for (int i = 0; i < m_TestComponents.Count; ++i)
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                m_TestComponents[i].DoInitialize(this);
#pragma warning restore CS0618
            }
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureOneTimeTearDown()"/>
        public override void FixtureOneTimeTearDown()
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            if (lifetimeState != LifetimeState.Suspended)
#pragma warning restore CS0618
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                ValidateLifecycle("OneTimeTearDown() can only be called once after OneTimeSetup.", LifetimeState.Initialized);
                m_LifetimeState = LifetimeState.Disposed;
#pragma warning restore CS0618

                InvokeOnTestComponents(ComponentShutdown);
            }
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureSetUp()"/>
        public override void FixtureSetUp()
        {
            // We don't want to run further tests after a test failure in debug mode
#pragma warning disable CS0618 // Disable warning on Internal usage
            if (m_LifetimeState == LifetimeState.Suspended)
#pragma warning restore CS0618
            {
                throw new InvalidOperationException("Test failed in debug mode, stopping further tests.");
            }

#pragma warning disable CS0618 // Disable warning on Internal usage
            ValidateLifecycle("SetUp() can only be called once at the beginning of a test case", LifetimeState.Initialized);
            m_LifetimeState = LifetimeState.DuringTest;
#pragma warning restore CS0618

            EventHelpers.TestSetUp();
            EventHelpers.SetFrameWaiter(() => { });

            PanelSimulator.ResetCurrentTime();

            InvokeOnTestComponents(ComponentBeforeTest);
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureTearDown()"/>
        public override void FixtureTearDown()
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            if (m_LifetimeState != LifetimeState.Suspended)
            {
                ValidateLifecycle("TearDown() can only be called once at the end of a test case", LifetimeState.DuringTest);
                m_LifetimeState = LifetimeState.Initialized;
#pragma warning restore CS0618

                EventHelpers.TestTearDown();

#pragma warning disable CS0618 // Disable warning on Internal usage
                if (debugMode && testStatus.hasTestFailed)
                {
                    m_LifetimeState = LifetimeState.Suspended;
                }
#pragma warning restore CS0618
                else
                {
                    InvokeOnTestComponents(ComponentAfterTest);
                }
            }

            UIR.UIRenderDevice.FlushAllPendingDeviceDisposes();
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureUnitySetUp()"/>
        public override IEnumerator FixtureUnitySetUp()
        {
            yield break;
        }

        /// <inheritdoc cref="AbstractUITestFixture.FixtureUnityTearDown()"/>
        public override IEnumerator FixtureUnityTearDown()
        {
            yield break;
        }

#pragma warning disable CS0618 // Disable warning on Internal usage
        private static Action<UITestComponent> ComponentBeforeTest = (x) => x.DoBeforeTest();
        private static Action<UITestComponent> ComponentAfterTest = (x) => x.DoAfterTest();
        private static Action<UITestComponent> ComponentShutdown = (x) => x.DoShutdown();
#pragma warning restore CS0618

        void InvokeOnTestComponents(Action<UITestComponent> componentMethod)
        {
            for (int i = m_TestComponents.Count - 1; i >= 0; --i)
            {
                componentMethod(m_TestComponents[i]);
            }

        }


        [System.Obsolete("For Internal Use Only.")]
        internal void UnsuspendTestExecution()
        {
            if (lifetimeState == LifetimeState.Suspended)
            {
                for (int i = m_TestComponents.Count - 1; i >= 0; --i)
                {
                    m_TestComponents[i].DoAfterTest();
                }

                m_LifetimeState = LifetimeState.Initialized;
            }
        }

        [System.Obsolete("For Internal Use Only.")]
        internal void ShutdownAfterSuspension()
        {
            if (lifetimeState == LifetimeState.Suspended)
            {
                for (int i = m_TestComponents.Count - 1; i >= 0; --i)
                {
                    m_TestComponents[i].DoAfterTest();
                }

                for (int i = m_TestComponents.Count - 1; i >= 0; --i)
                {
                    m_TestComponents[i].DoShutdown();
                }

                m_LifetimeState = LifetimeState.Disposed;
            }
        }

        [System.Obsolete("For Internal Use Only.")]
        internal void EnsureDuringTest(string methodName)
        {
            if (lifetimeState != LifetimeState.DuringTest)
            {
                throw new System.InvalidOperationException($"{methodName} can only be called during a test.");
            }
        }

        /// <summary>
        /// Reports the status and name of the test that's currently executing.
        /// </summary>
        [System.Obsolete("For Internal Use Only.")]
        internal class NUnitTestStatusProvider : ITestStatusProvider
        {
            /// <summary>
            /// Whether the current test has a `Failed` outcome.
            /// `true` if the TestStatus is `Failed`, `false` otherwise.
            /// </summary>
            public bool hasTestFailed => TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;

            /// <summary>
            /// The name of the test that's currently running.
            /// </summary>
            public string testName => TestContext.CurrentContext.Test.Name;
        }
    }
}

using System.Runtime.CompilerServices;

#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.UI.TestFramework.Editor")] // for UI Test Framework
#endif // UNITY_EDITOR

// TODO Rework the tests in this assembly to not require internal access to use it like our users
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.UI.TestFramework.Editor.Tests")] // for UI Test Framework Tests
#endif // UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.UI.TestFramework.Runtime.Tests")] // for UI Test Framework Tests

// List of all the tests assemblies that need internal access for EventHelpers until we provide an alternative

[assembly: InternalsVisibleTo("UnityEngine.UIElements.Tests.Playmode")]
[assembly: InternalsVisibleTo("UnityEngine.UIElements.Tests.Utils")]
[assembly: InternalsVisibleTo("Unity.UI.TestFramework.Runtime.InternalAccessTests")]
[assembly: InternalsVisibleTo("UnityEngine.UIElements.Tests.Base")]
[assembly: InternalsVisibleTo("Unity.UIElements.PlayModeTests")]
[assembly: InternalsVisibleTo("Unity.UIElements.RuntimeTests.Controls")]
[assembly: InternalsVisibleTo("UnityEngine.UIElements.Tests.Controls")]
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.VisualEffectGraph.EditorTests")] // Tests/SRPTests/Projects/VisualEffectGraph_HDRP/Assets/AllTests/Editor
[assembly: InternalsVisibleTo("Unity.PlayMode.Editor.Tests")] // Modules/PlayModeEditor/Tests/UTFTests
[assembly: InternalsVisibleTo("Unity.UI.Builder.EditorTests")]
[assembly: InternalsVisibleTo("Unity.UIElements.EditorTests")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-testable")] // Tests/EditModeAndPlayModeTests/Audio/Assets/Editor/Assembly-CSharp-Editor-testable.asmdef
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Editor.Tests.UI")] // motion
[assembly: InternalsVisibleTo("Unity.Motion.Editor.Tests")] // motion
[assembly: InternalsVisibleTo("Unity.Insights.Editor.Tests")] // Modules/InsightsEditor/Tests/EditModeTests/InsightsEditor
#endif // UNITY_EDITOR

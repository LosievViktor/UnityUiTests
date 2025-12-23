using System.Runtime.CompilerServices;

// TODO Rework the tests in this assembly to not require internal access to use it like our users
[assembly: InternalsVisibleTo("Unity.UI.TestFramework.Editor.Tests")] // for UI Test Framework

// List of all the tests assemblies that need internal access for EventHelpers until we provide an alternative

[assembly: InternalsVisibleTo("Unity.UI.TestFramework.Editor.InternalAccessTests")]
[assembly: InternalsVisibleTo("Unity.UI.Builder.EditorTests")]
[assembly: InternalsVisibleTo("Unity.UIElements.EditorTests")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-testable")] // Tests/EditModeAndPlayModeTests/Audio/Assets/Editor/Assembly-CSharp-Editor-testable.asmdef
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Editor.Tests.UI")] // motion
[assembly: InternalsVisibleTo("Unity.Motion.Editor.Tests")] // motion
[assembly: InternalsVisibleTo("Unity.Insights.Editor.Tests")] // Modules/InsightsEditor/Tests/EditModeTests/InsightsEditor

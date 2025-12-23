using UnityEngine.UIElements;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// Utility class to create an `Inspector` for a given object.
    /// </summary>
    public static class InspectorTestUtility
    {
        /// <summary>
        /// Returns the Editor object created to inspect the given object.
        /// </summary>
        /// <param name="inspector">InspectorElement for which to find the Editor object.</param>
        /// <returns>The Editor object that the InspectorElement is inspecting.</returns>
        public static Editor GetEditor(this InspectorElement inspector)
        {
            return inspector.editor;
        }

        /// <summary>
        /// Returns the `VisualElement` created by the Editor to display the object's properties.
        /// IMGUI-only inspectors return an `IMGUIContainer`.
        /// </summary>
        /// <param name="inspector">`InspectorElement` for which to obtain the `VisualElement` content.</param>
        /// <returns>The `VisualElement` containing the `InspectorElement` content.</returns>
        public static VisualElement GetInspectorContent(this InspectorElement inspector)
        {
            return inspector.inspectorContent;
        }

        /// <summary>
        /// Creates an `InspectorElement` for the given object using the registered custom Editor for the object.
        /// </summary>
        /// <param name="obj">The Object for which to create an `InspectorElement`.</param>
        /// <returns>The `InspectorElement` that is inspecting the provided Object.</returns>
        public static InspectorElement CreateInspector(UnityEngine.Object obj)
        {
            var editor = Editor.CreateEditor(obj);
            return new InspectorElement(editor, InspectorElement.DefaultInspectorFramework.UIToolkit);
        }

        /// <summary>
        /// Creates an `InspectorElement` for the given object using the default generic inspector,
        /// ignoring any registered custom Editors.
        /// </summary>
        /// <param name="obj">The Object for which to create an `InspectorElement`.</param>
        /// <returns>The `InspectorElement` that is inspecting the provided Object.</returns>
        public static InspectorElement CreateDebugInspector(UnityEngine.Object obj)
        {
            var editor = Editor.CreateEditor(obj, typeof(GenericInspector));
            return new InspectorElement(editor, InspectorElement.DefaultInspectorFramework.UIToolkit);
        }
    }
}

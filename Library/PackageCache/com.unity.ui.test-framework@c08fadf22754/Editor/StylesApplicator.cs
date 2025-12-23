using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEditor.UIElements.StyleSheets;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;
using Object = UnityEngine.Object;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// Applies styles to UI in conjunction with the test fixtures.
    /// </summary>
    public class StylesApplicator : UITestComponent
    {
        List<KeyValuePair<VisualElement, StyleSheet>> m_AddedStyles = new();

        /// <summary>
        /// Creates a new style sheet from the given `ussContent` and adds it to the root visual element.
        /// Added styles are removed during TearDown.
        /// </summary>
        /// <param name="ussContent">The USS content as a string.</param>
        /// <param name="ignoreError">
        /// Whether to ignore errors encountered during processing. Defaults to `false`.
        /// </param>
        public void AddStylesToRoot(string ussContent, bool ignoreError = false)
        {
            AddStylesToElement(fixture.rootVisualElement, ussContent, ignoreError);
        }

        /// <summary>
        /// Creates a new style sheet from the given <paramref name="ussContent"/> and adds it to the visual element.
        /// Added styles are removed during TearDown.
        /// </summary>
        /// <param name="element">The `VisualElement` to add the styles to.</param>
        /// <param name="ussContent">The USS content as a string.</param>
        /// <param name="ignoreError">
        /// Whether to ignore errors encountered during processing. Defaults to `false`.
        /// </param>
        public void AddStylesToElement(VisualElement element, string ussContent, bool ignoreError = false)
        {
            var styleSheet = CreateStyleSheetFromString(ussContent, ignoreError: ignoreError);
            element.styleSheets.Add(styleSheet);
            m_AddedStyles.Add(KeyValuePair.Create(element, styleSheet));
        }

        /// <summary>
        /// Cleans up `StylesApplicator` after each test.
        /// </summary>
        protected override void AfterTest()
        {
            // Comment: Do we actually need to call the base.TearDown,
            // here since it just inherits from UITestComponent,
            // which does not appear to have an implementation anyways?
            base.AfterTest();
            ClearAddedStyles();
        }

        void ClearAddedStyles()
        {
            foreach (var s in m_AddedStyles)
            {
                s.Key.styleSheets.Remove(s.Value);
                Object.DestroyImmediate(s.Value);
            }
            m_AddedStyles.Clear();
        }

        /// <summary>
        /// Creates a new `StyleSheet` instance from a string containing USS content.
        /// </summary>
        /// <param name="contents">The USS content as a string.</param>
        /// <param name="fakePath">
        /// The fake path used internally to mimic a real asset location. By default, this is `Assets/test.uss`.
        /// </param>
        /// <param name="ignoreError">
        /// A Boolean indicating whether to ignore errors encountered during processing. 
        /// Defaults to `false`.
        /// </param>
        /// <returns>A new `StyleSheet` instance generated from the provided content.</returns>
        public static StyleSheet CreateStyleSheetFromString(string contents, string fakePath = "Assets/test.uss", bool ignoreError = false)
        {
            var sheet = ScriptableObject.CreateInstance<StyleSheet>();
            ImportFromString(sheet, contents, fakePath, ignoreError);
            return sheet;
        }

        /// <summary>
        /// Imports the given USS content into an existing <see cref="StyleSheet"/> instance.
        /// </summary>
        /// <param name="sheet">The <see cref="StyleSheet"/> instance to populate with content.</param>
        /// <param name="contents">The USS content as a string.</param>
        /// <param name="fakePath">
        /// The fake path used internally to mimic a real asset location. By default, this is `Assets/test.uss`.
        /// </param>
        /// <param name="ignoreError">
        /// Whether to ignore errors encountered during processing. Defaults to `false`.
        /// </param>
        private static void ImportFromString(StyleSheet sheet, string contents, string fakePath = "Assets/test.uss", bool ignoreError = false)
        {
            sheet.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable;

            // Ignore warnings: we are using fake paths that generate warnings at import time.
            var importer = new InMemoryStyleSheetImporter(fakePath, ignoreError, ignoreWarnings: true);
            importer.Import(sheet, contents);
        }

        // The purpose of this class is to allow a quick style sheet creation in tests
        // This by-passes the asset database, allowing style sheets to be shared by several tests cases
        // Otherwise this normally errors of files not being cleaned-up.
        class InMemoryStyleSheetImporter : StyleSheetImporterImpl
        {
            private StringBuilder m_StringBuilder;
            private bool m_IgnoreErrors;
            private bool m_IgnoreWarnings;

            public InMemoryStyleSheetImporter(string assetPath, bool ignoreErrors = false, bool ignoreWarnings = false) : base()
            {
                m_AssetPath = assetPath;
                m_StringBuilder = new StringBuilder();
                m_IgnoreErrors = ignoreErrors;
                m_IgnoreWarnings = ignoreWarnings;
            }

            public override Object DeclareDependencyAndLoad(string path, string subAssetPath = null)
            {
                // Unlike the base class, do not try to access asset import context here.
                return AssetDatabase.LoadAssetAtPath<Object>(path);
            }

            protected override void OnImportError(StyleSheetImportErrors errors)
            {
                var reactToErrors = !m_IgnoreErrors && errors.hasErrors;
                var reactToWarnings = !m_IgnoreWarnings && errors.hasWarning;

                if (!reactToErrors && !reactToWarnings)
                    return;

                if (reactToErrors)
                {
                    foreach (var error in errors)
                    {
                        if (!error.isWarning)
                            m_StringBuilder.AppendLine($"Error: {error.ToString()}");
                    }
                }

                if (reactToWarnings)
                {
                    foreach (var error in errors)
                    {
                        if (error.isWarning)
                            m_StringBuilder.AppendLine($"Warning: {error.ToString()}");
                    }
                }

                Assert.Fail($"Import failure for {assetPath} : \n{m_StringBuilder.ToString()}");
            }

            protected override void OnImportSuccess(StyleSheet asset)
            {
                // all good
            }
        }

    }
}

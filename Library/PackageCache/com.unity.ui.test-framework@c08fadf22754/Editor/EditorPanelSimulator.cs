using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;

namespace UnityEditor.UIElements.TestFramework
{
    /// <summary>
    /// A <see cref="PanelSimulator"/> with a default Editor panel.
    /// </summary>
    public sealed class EditorPanelSimulator : PanelSimulator, IDisposable
    {
        /// <summary>
        /// Creates a new Editor panel.
        /// </summary>
        public EditorPanelSimulator()
#pragma warning disable CS0618 // Disable warning on Internal usage
            : base()
#pragma warning restore CS0618
        {
            CreatePanel();
        }

        /// <inheritdoc cref="PanelSimulator.FrameUpdate(double)" />
        public sealed override void FrameUpdate(double time)
        {
#pragma warning disable CS0618 // Disable warning on Internal usage
            EnsureFrameUpdateCalledDuringTest();
            DoFrameUpdate(time);
#pragma warning restore CS0618
        }

        private ScriptableObject m_PanelOwner;

        private Vector2 m_PanelSize = PanelSimulator.GetDefaultPanelSize();

        /// <summary>
        /// The size of the `rootVisualElement` of the panel.
        /// </summary>
        public Vector2 panelSize
        {
            get => m_PanelSize;
            set
            {
                if (m_PanelSize != value)
                {
                    m_PanelSize = value;
                    if (panel != null)
                    {
                        ApplyPanelSize();
                    }
                }
            }
        }

        // should this be internal if people never need to call it?
        /// <summary>
        /// Applies the `m_PanelSize` to the panel.
        /// </summary>
        public void ApplyPanelSize()
        {
            panel.visualTree.SetSize(m_PanelSize);
        }

        private class PanelOwner : ScriptableObject { }

        /// <summary>
        /// Creates the panel and initializes the <see cref="PanelSimulator.rootVisualElement"/>.
        /// </summary>
        public void CreatePanel()
        {
            m_PanelOwner = ScriptableObject.CreateInstance<PanelOwner>();
            Panel p = new EditorPanel(m_PanelOwner);
            p.pixelsPerPoint = 1;
            p.UpdateScalingFromEditorWindow = false;
#pragma warning disable CS0618 // Disable warning on Internal usage
            SetPanel(p);
            SetRootVisualElement(p.visualTree);
#pragma warning restore CS0618
        }

        /// <summary>
        /// Disposes of the panel and releases its resources.
        /// </summary>
        public void ReleasePanel()
        {
            var p = panel as Panel;

            if (p != null)
            {
#pragma warning disable CS0618 // Disable warning on Internal usage
                SetPanel(null);
                SetRootVisualElement(null);
#pragma warning restore CS0618
                ScriptableObject.DestroyImmediate(m_PanelOwner);
                m_PanelOwner = null;
                p.Dispose();
            }
        }

        /// <summary>
        /// Recreates the panel.
        /// </summary>
        public void RecreatePanel()
        {
            ReleasePanel();
            CreatePanel();
        }

        /// <summary>
        /// Disposes of the panel and releases its resources.
        /// </summary>
        public void Dispose()
        {
            ReleasePanel();
        }
    }
}

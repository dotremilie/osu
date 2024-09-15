// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Screens.Edit.Compose.Components
{
    /// <summary>
    /// Represents a display composite containing and managing the visibility state of the selection box's drag handles.
    /// </summary>
    public partial class TransformSelectionBoxDragHandleContainer : CompositeDrawable
    {
        private Container<TransformSelectionBoxScaleHandle> scaleHandles;
        private Container<TransformSelectionBoxRotationHandle> rotationHandles;

        private readonly List<TransformSelectionBoxDragHandle> allDragHandles = new List<TransformSelectionBoxDragHandle>();

        public new MarginPadding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                scaleHandles = new Container<TransformSelectionBoxScaleHandle>
                {
                    RelativeSizeAxes = Axes.Both,
                },
                rotationHandles = new Container<TransformSelectionBoxRotationHandle>
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(-12.5f),
                },
            };
        }

        public void AddScaleHandle(TransformSelectionBoxScaleHandle handle)
        {
            bindDragHandle(handle);
            scaleHandles.Add(handle);
        }

        public void AddRotationHandle(TransformSelectionBoxRotationHandle handle)
        {
            handle.Alpha = 0;
            handle.AlwaysPresent = true;

            bindDragHandle(handle);
            rotationHandles.Add(handle);
        }

        private void bindDragHandle(TransformSelectionBoxDragHandle handle)
        {
            handle.HoverGained += updateRotationHandlesVisibility;
            handle.HoverLost += updateRotationHandlesVisibility;
            handle.MouseDown += updateRotationHandlesVisibility;
            handle.MouseUp += updateRotationHandlesVisibility;
            allDragHandles.Add(handle);
        }

        public void FlipScaleHandles(Direction direction)
        {
            foreach (var handle in scaleHandles)
            {
                if (direction == Direction.Horizontal && !handle.Anchor.HasFlag(Anchor.x1))
                    handle.Anchor ^= Anchor.x0 | Anchor.x2;
                if (direction == Direction.Vertical && !handle.Anchor.HasFlag(Anchor.y1))
                    handle.Anchor ^= Anchor.y0 | Anchor.y2;
            }
        }

        private TransformSelectionBoxRotationHandle displayedRotationHandle;
        private TransformSelectionBoxDragHandle activeHandle;

        private void updateRotationHandlesVisibility()
        {
            // if the active handle is a rotation handle and is held or hovered,
            // then no need to perform any updates to the rotation handles visibility.
            if (activeHandle is TransformSelectionBoxRotationHandle && (activeHandle?.IsHeld == true || activeHandle?.IsHovered == true))
                return;

            displayedRotationHandle?.FadeOut(TransformSelectionBoxControl.TRANSFORM_DURATION, Easing.OutQuint);
            displayedRotationHandle = null;

            // if the active handle is not a rotation handle but is held, then keep the rotation handle hidden.
            if (activeHandle?.IsHeld == true)
                return;

            activeHandle = rotationHandles.FirstOrDefault(h => h.IsHeld || h.IsHovered);
            activeHandle ??= allDragHandles.FirstOrDefault(h => h.IsHovered);

            if (activeHandle != null)
            {
                displayedRotationHandle = getCorrespondingRotationHandle(activeHandle, rotationHandles);
                displayedRotationHandle?.FadeIn(TransformSelectionBoxControl.TRANSFORM_DURATION, Easing.OutQuint);
            }
        }

        /// <summary>
        /// Gets the rotation handle corresponding to the given handle.
        /// </summary>
        [CanBeNull]
        private static TransformSelectionBoxRotationHandle getCorrespondingRotationHandle(TransformSelectionBoxDragHandle handle, IEnumerable<TransformSelectionBoxRotationHandle> rotationHandles)
        {
            if (handle is TransformSelectionBoxRotationHandle rotationHandle)
                return rotationHandle;

            return rotationHandles.SingleOrDefault(r => r.Anchor == handle.Anchor);
        }
    }
}

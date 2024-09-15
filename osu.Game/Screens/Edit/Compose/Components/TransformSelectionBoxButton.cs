// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Compose.Components
{
    public sealed partial class TransformSelectionBoxButton : TransformSelectionBoxControl, IHasTooltip
    {
        private SpriteIcon icon = null!;

        private readonly IconUsage iconUsage;

        public Action? Action;

        public TransformSelectionBoxButton(IconUsage iconUsage, string tooltip)
        {
            this.iconUsage = iconUsage;

            TooltipText = tooltip;

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(20);
            AddInternal(icon = new SpriteIcon
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.5f),
                Icon = iconUsage,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }

        protected override bool OnClick(ClickEvent e)
        {
            Circle.FlashColour(Colours.GrayF, 300);

            TriggerOperationStarted();
            Action?.Invoke();
            TriggerOperationEnded();
            return true;
        }

        protected override void UpdateHoverState()
        {
            base.UpdateHoverState();
            icon.FadeColour(!IsHeld && IsHovered ? Color4.White : Color4.Black, TRANSFORM_DURATION, Easing.OutQuint);
        }

        public LocalisableString TooltipText { get; }
    }
}
// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Screens.Play.HUD.JudgementCounter
{
    public partial class JudgementCounter : OverlayContainer
    {
        public BindableBool ShowName = new BindableBool();
        public Bindable<FillDirection> Direction = new Bindable<FillDirection>();

        public readonly JudgementCounterInfo Result;

        public JudgementCounter(JudgementCounterInfo result)
        {
            Result = result;
        }

        public OsuSpriteText ResultName = null!;
        private FillFlowContainer flowContainer = null!;
        private JudgementRollingCounter counter = null!;

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AutoSizeAxes = Axes.Both;
            InternalChild = flowContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    counter = new JudgementRollingCounter
                    {
                        Current = Result.ResultCount
                    },
                    ResultName = new OsuSpriteText
                    {
                        Font = OsuFont.Numeric.With(size: 8),
                        Text = Result.ResultInfo.Displayname
                    }
                }
            };

            var result = Result.ResultInfo.Type;

            if (result.IsBasic())
            {
                Colour = colours.ForHitResult(Result.ResultInfo.Type);
                return;
            }

            if (!result.IsBonus())
            {
                Colour = colours.PurpleLight;
                return;
            }

            Colour = colours.PurpleLighter;
        }

        protected override void LoadComplete()
        {
            ShowName.BindValueChanged(value =>
            {
                if (value.NewValue)
                {
                    ResultName.Show();
                    return;
                }

                ResultName.Hide();
            }, true);

            Direction.BindValueChanged(direction =>
            {
                flowContainer.Direction = direction.NewValue;

                if (direction.NewValue == FillDirection.Vertical)
                {
                    changeAnchor(Anchor.TopLeft);
                    return;
                }

                changeAnchor(Anchor.BottomLeft);

                void changeAnchor(Anchor anchor) => counter.Anchor = ResultName.Anchor = counter.Origin = ResultName.Origin = anchor;
            }, true);

            base.LoadComplete();
        }

        protected override void PopIn()
        {
            this.FadeInFromZero(500, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            this.FadeOut(100);
        }

        private sealed partial class JudgementRollingCounter : RollingCounter<int>
        {
            protected override OsuSpriteText CreateSpriteText()
                => base.CreateSpriteText().With(s => s.Font = s.Font.With(fixedWidth: true, size: 16));

            protected override double RollingDuration => 750;
        }
    }
}

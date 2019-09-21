using System;
using CMineNew.Render;
using CMineNew.Text;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Test{
    public class DelayViewer : StaticText{
        private long _generationDelay, _generationAmount;
        private long _fillDelay, _fillAmount;

        public DelayViewer(TrueTypeFont font) : base(font, "INIT", new Vector2(-1, 0.8f), Color4.White) {
        }


        public void AddGeneration(long delay) {
            _generationDelay += delay;
            _generationAmount++;
        }

        public void AddFill(long delay) {
            _fillDelay += delay;
            _fillAmount++;
        }

        public override void Tick(long dif, Room room) {
            var g = _generationDelay * 1000 / (double) _generationAmount;
            var f = _fillDelay * 1000 / (double) _fillAmount;
            Text = "G: " + Math.Round(g / CMine.TicksPerSecondF, 3) +
                   "ms. F: " + Math.Round(f / CMine.TicksPerSecondF, 3) + "ms.";
        }
    }
}
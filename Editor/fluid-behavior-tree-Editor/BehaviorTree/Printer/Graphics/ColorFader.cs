using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class ColorFader {
        const float FADE_DURATION = 0.8f;

        private float _fadeTime;
        private readonly Color _startColor;
        private readonly Color _endColor;

        public Color CurrentColor { get; private set; }

        public ColorFader (Color start, Color endColor) {
            _startColor = start;
            _endColor = endColor;
        }

        public void Update (bool reset) {
            if (reset) {
                _fadeTime = FADE_DURATION;
            } else {
                _fadeTime -= Time.deltaTime;
                _fadeTime = Mathf.Max(_fadeTime, 0);
            }

            CurrentColor = Color.Lerp(
                _startColor,
                _endColor,
                _fadeTime / FADE_DURATION);
        }
    }
}

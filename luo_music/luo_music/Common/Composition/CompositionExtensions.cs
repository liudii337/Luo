using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace luo_music.Common.Composition
{
    public static class CompositionExtensions
    {
        private const string TRANSLATION = "Translation";

        public static Visual GetVisual(this UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            visual.Properties.InsertVector3(TRANSLATION, Vector3.Zero);
            return visual;
        }

        public static CompositionAnimationBuilder StartBuildAnimation(this Visual visual)
        {
            return new CompositionAnimationBuilder(visual);
        }

        public static void SetTranslation(this Visual set, Vector3 value)
        {
            set.Properties.InsertVector3(TRANSLATION, value);
        }

        public static Vector3 GetTranslation(this Visual visual)
        {
            visual.Properties.TryGetVector3(TRANSLATION, out Vector3 value);
            return value;
        }

        public static string GetTranslationPropertyName(this Visual visual)
        {
            return AnimateProperties.Translation.GetPropertyValue();
        }

        public static string GetTranslationXPropertyName(this Visual visual)
        {
            return AnimateProperties.TranslationX.GetPropertyValue();
        }

        public static string GetTranslationYPropertyName(this Visual visual)
        {
            return AnimateProperties.TranslationY.GetPropertyValue();
        }

        public static string GetPropertyValue(this AnimateProperties property)
        {
            switch (property)
            {
                case AnimateProperties.Translation:
                    return TRANSLATION;

                case AnimateProperties.TranslationX:
                    return $"{TRANSLATION}.X";

                case AnimateProperties.TranslationY:
                    return $"{TRANSLATION}.Y";

                case AnimateProperties.Opacity:
                    return "Opacity";

                case AnimateProperties.RotationAngleInDegrees:
                    return "RotationAngleInDegrees";

                default:
                    throw new ArgumentException("Unknown properties");
            }
        }
    }

}

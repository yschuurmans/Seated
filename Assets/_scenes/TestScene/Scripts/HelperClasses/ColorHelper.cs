using UnityEngine;

namespace Assets.TestScene.Scripts.HelperClasses
{
    public class ColorHelper
    {
        public static void ChangeGameObjectColor(Renderer targetRenderer, Color startColor, Color endColor, double percentage)
        {
            targetRenderer.material.color = GetLerpedColor(startColor, endColor, percentage);
        }

        public static Color GetLerpedColor(Color startColor, Color endColor, double percentage)
        {
            Color tempColor = new Color();
            float r = endColor.r - startColor.r;
            float g = endColor.g - startColor.g;
            float b = endColor.b - startColor.b;
            tempColor.r = startColor.r + r * (float)percentage;
            tempColor.g = startColor.g + g * (float)percentage;
            tempColor.b = startColor.b + b * (float)percentage;
            tempColor.a = 1;
            return tempColor;
        }
    }
}

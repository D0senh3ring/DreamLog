using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using Xamarin.Forms;
using System.Linq;
using System;

namespace DreamLog.Tools
{
    public static class UIUtils
    {
        private static Dictionary<string, Color> colorCahce;

        public static string GetColorName(Color color)
        {
            if (UIUtils.colorCahce is null)
                UIUtils.SetUpColorCahce();

            if (UIUtils.colorCahce.ContainsValue(color))
                return UIUtils.colorCahce.Single(_pair => _pair.Value == color).Key;
            return null;
        }

        public static Color GetColorByName(string name)
        {
            if (UIUtils.colorCahce is null)
                UIUtils.SetUpColorCahce();

            if (UIUtils.colorCahce.TryGetValue(name, out Color output))
                return output;
            return Color.Transparent;
        }

        public static List<ViewCell> GetChildren(this ListView parent)
        {
            List<ViewCell> result = new List<ViewCell>();

            try
            {
                var itemsProperty = parent.GetType().GetRuntimeProperties().FirstOrDefault(_prop => _prop.Name.Equals("TemplatedItems"));
                var items = itemsProperty.GetValue(parent) as IEnumerable;

                foreach(var item in items)
                {
                    if (item is ViewCell cell)
                        result.Add(cell);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        public static async Task<bool> TranslateToAbsolute(this VisualElement element, double x, double y, uint length = 250, TranslationInterpolationMode mode = TranslationInterpolationMode.Linear)
        {
            if (element is null || length == 0 || (x - element.TranslationX == 0.0d && y - element.TranslationY == 0.0d))
                return false;

            switch(mode)
            {
                case TranslationInterpolationMode.Linear:
                    return await UIUtils.TranslateToLinearlyAbsoute(element, x, y, length);
                default:
                    return false;
            }
        }

        private static async Task<bool> TranslateToLinearlyAbsoute(VisualElement element, double x, double y, uint length)
        {
            uint remainingTime = length;
            uint timeStep = 10;
            double stepSizeX = x / timeStep;
            double stepSizeY = y / timeStep;

            while(remainingTime > 0)
            {
                element.TranslationX += stepSizeX;
                element.TranslationY += stepSizeY;

                if (remainingTime < timeStep)
                    remainingTime = 0;
                else
                    remainingTime -= timeStep;
                await Task.Delay((int)timeStep);
            }
            return true;
        }

        private static void SetUpColorCahce()
        {
            UIUtils.colorCahce = typeof(Color).GetFields(BindingFlags.Public | BindingFlags.Static).Where(_field => _field.FieldType == typeof(Color))
                                              .GroupBy(_field => _field.GetValue(null))
                                              .ToDictionary(_group => _group.First().Name, _group => (Color)_group.Key);
        }
    }

    public enum TranslationInterpolationMode
    {
        Linear
    }
}

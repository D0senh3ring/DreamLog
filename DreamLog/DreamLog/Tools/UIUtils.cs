using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using System.Linq;

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
            if(UIUtils.colorCahce is null)
                UIUtils.SetUpColorCahce();

            if(UIUtils.colorCahce.TryGetValue(name, out Color output))
                return output;
            return Color.Transparent;
        }

        private static void SetUpColorCahce()
        {
            UIUtils.colorCahce = typeof(Color).GetFields(BindingFlags.Public | BindingFlags.Static).Where(_field => _field.FieldType == typeof(Color))
                                              .GroupBy(_field => _field.GetValue(null))
                                              .ToDictionary(_group => _group.First().Name, _group => (Color)_group.Key);
        }
    }
}

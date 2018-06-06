using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;

namespace NNS_Z
{
    public class ColorModel : Observable
    {
        [XmlAttribute("Progress")]
        public string ProgressString
        {
            get => Progress.ConvertToString();
            set { try { Progress = Progress.ConvertFromString(value); } catch { } }
        }
        [XmlIgnore]
        public Color Progress { get => _Progress; set => Set(ref _Progress, value); }
        private Color _Progress;

        public ColorModel()
        {
            Progress = Progress.ConvertFromString("#FF2A3A56");
        }
    }

    internal static class ConvertExtensions
    {
        public static T ConvertFromString<T>(this T target, string value)
            => (T)TypeDescriptor.GetConverter(target.GetType()).ConvertFrom(value);
        public static string ConvertToString<T>(this T value)
            => (string)TypeDescriptor.GetConverter(value.GetType()).ConvertTo(value, typeof(string));
    }
}

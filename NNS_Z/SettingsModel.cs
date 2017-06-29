using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace NNS_Z
{
    [DataContract(Namespace = "")]
    public class ColorModel : BindableBase
    {
        #region Progress
        [DataMember(Name = "Progress", Order = 0)]
        public string ProgressString
        {
            get => Progress.ConvertToString();
            set { try { Progress = Progress.ConvertFromString(value); } catch { } }
        }
        private Color _Progress;
        public Color Progress { get => _Progress; set => SetProperty(ref _Progress, value); }
        #endregion

        protected override void Init()
        {
            Progress = Progress.ConvertFromString("#FF2A3A56");
        }
    }

    [DataContract(Namespace = "")]
    public class WindowModel : BindableBase
    {
        [DataMember(Name = "Top", Order = 0)]
        private double _Top;
        public double Top { get => _Top; set => SetProperty(ref _Top, value); }

        [DataMember(Name = "Left", Order = 1)]
        private double _Left;
        public double Left { get => _Left; set => SetProperty(ref _Left, value); }

        [DataMember(Name = "Width", Order = 2)]
        private double _Width;
        public double Width { get => _Width; set => SetProperty(ref _Width, value); }

        [DataMember(Name = "Height", Order = 3)]
        private double _Height;
        public double Height { get => _Height; set => SetProperty(ref _Height, value); }

        protected override void Init()
        {
            _Top = 200;
            _Left = 200;
            _Width = 330;
            _Height = 600;
        }
    }

    [DataContract(Namespace = "", Name = "Setting")]
    public class SettingsModel : BindableBase
    {
        [DataMember(Name = "SearchWord", Order = 0)]
        private string _SearchWord;
        public string SearchWord { get => _SearchWord; set => SetProperty(ref _SearchWord, value); }

        [DataMember(Order = 1)]
        public WindowModel Window { get; private set; }

        [DataMember(Order = 2)]
        public ColorModel Color { get; private set; }

        protected override void Init()
        {
            _SearchWord = "";
            Window = new WindowModel();
            Color = new ColorModel();
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

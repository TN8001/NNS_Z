using CommunityToolkit.Mvvm.ComponentModel;
using System.Xml.Serialization;

namespace NNS_Z
{
    public class WindowModel : ObservableObject
    {
        [XmlAttribute]
        public double Top { get => _Top; set => SetProperty(ref _Top, value); }
        private double _Top;

        [XmlAttribute]
        public double Left { get => _Left; set => SetProperty(ref _Left, value); }
        private double _Left;

        [XmlAttribute]
        public double Width { get => _Width; set => SetProperty(ref _Width, value); }
        private double _Width;

        [XmlAttribute]
        public double Height { get => _Height; set => SetProperty(ref _Height, value); }
        private double _Height;

        public WindowModel()
        {
            Top = double.NaN;
            Left = double.NaN;
            Width = 400;
            Height = 500;
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Xml.Serialization;

namespace NNS_Z
{
    [XmlRoot("Settings")]
    public class SettingsModel : ObservableObject
    {
        [XmlAttribute]
        public string SearchWord { get => _SearchWord; set => SetProperty(ref _SearchWord, value); }
        private string _SearchWord;

        public WindowModel Window { get; set; }

        public ColorModel Color { get; set; }

        public SettingsModel()
        {
            SearchWord = "プログラミング";
            Window = new WindowModel();
            Color = new ColorModel();
        }
    }
}

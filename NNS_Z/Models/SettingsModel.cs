using System.Xml.Serialization;

namespace NNS_Z
{
    [XmlRoot("Settings")]
    public class SettingsModel : Observable
    {
        [XmlAttribute]
        public string SearchWord { get => _SearchWord; set => Set(ref _SearchWord, value); }
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

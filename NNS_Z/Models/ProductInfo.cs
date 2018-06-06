using System;
using System.Reflection;

namespace NNS_Z
{
    ///<summary>アセンブリ情報</summary>
    public static class ProductInfo
    {
        ///<summary>アセンブリ名</summary>
        public static string Name { get; }
        ///<summary>アセンブリバージョン（Major.Minor.Build）</summary>
        public static Version Version { get; }
        ///<summary>アセンブリタイトル</summary>
        public static string Title { get; }
        ///<summary>アセンブリ説明</summary>
        public static string Description { get; }
        ///<summary>アセンブリ著作権</summary>
        public static string Copyright { get; }

        static ProductInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Name = assembly.GetName().Name;
            var version = assembly.GetName().Version;
            Version = new Version(version.Major, version.Minor, version.Build);
            Title = Get<AssemblyTitleAttribute>(assembly).Title;
            Description = Get<AssemblyDescriptionAttribute>(assembly).Description;
            Copyright = Get<AssemblyCopyrightAttribute>(assembly).Copyright;

            T Get<T>(Assembly a) where T : Attribute
                => (T)Attribute.GetCustomAttribute(a, typeof(T));
        }
    }
}

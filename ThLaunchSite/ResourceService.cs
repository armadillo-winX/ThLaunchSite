using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using ThLaunchSite.Properties;

namespace ThLaunchSite
{
    public class ResourceService : INotifyPropertyChanged
    {
        private static readonly ResourceService _current = new();
        public static ResourceService Current => _current;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// リソースを取得
        /// </summary>
        private readonly Resources _resources = new Resources();
        public Resources Resources => _resources;

        /// <summary>
        /// リソースのカルチャーを変更
        /// </summary>
        /// <param name="name">カルチャー名</param>
        public void ChangeCulture(string name)
        {
            Resources.Culture = CultureInfo.GetCultureInfo(name);
            RaisePropertyChanged("Resources");
        }
    }
}

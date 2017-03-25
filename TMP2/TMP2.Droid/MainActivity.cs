using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DotsAndBoxesFun;
using TMP2.Droid;
using Android.Media;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
[assembly: Xamarin.Forms.Dependency(typeof(AudioService))]
namespace TMP2.Droid
{
    [Activity(Label = "Dots and Boxes Fun", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }

    public class MessageAndroid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }

    public class AudioService : IAudio
    {
        public AudioService()
        {
        }

        public void PlayAudioFile(string fileName)
        {
            var player = new MediaPlayer();
            var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
            player.Prepared += (s, e) =>
            {
                player.Start();
            };
            player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            player.Completion += mediaPlayer_Completion;
            player.Prepare();
        }

        void mediaPlayer_Completion(object sender, EventArgs e)
        {
            MediaPlayer mp = (MediaPlayer)sender;
            mp.Completion -= mediaPlayer_Completion;
            mp.Release();
        }
    }   
}


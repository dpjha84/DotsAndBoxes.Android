using System;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using DotsAndBoxesFun;
using TMP2.Droid;
using Android.Media;
using Android.Content;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DotsAndBoxesFun.Views;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
[assembly: Xamarin.Forms.Dependency(typeof(AudioService))]
[assembly: Xamarin.Forms.Dependency(typeof(AndroidUserPreferences))]
namespace TMP2.Droid
{
    [Activity(Label = "Dots and Boxes Fun", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;            

            base.OnCreate(bundle);
            MobileAds.Initialize(ApplicationContext, "ca-app-pub-9351754143985661~5042360432");
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
            }
            catch (Exception)
            {
            }
            
        }
    }

    [Activity(Theme = "@style/Theme.Splash", //Indicates the theme to use for this activity
             MainLauncher = true, //Set it as boot activity
             NoHistory = true)] //Doesn't place it in back stack
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            System.Threading.Thread.Sleep(3000); //Let's wait awhile...
            this.StartActivity(typeof(MainActivity));
        }
    }

    public class MessageAndroid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
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

    public class AndroidUserPreferences : IUserPreferences
    {
        public void SetString(string key, string value)
        {
            var prefs = global::Android.App.Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            var prefEditor = prefs.Edit();

            prefEditor.PutString(key, value);
            prefEditor.Commit();
        }

        public string GetString(string key)
        {
            var prefs = global::Android.App.Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            if (prefs.Contains(key))
            {
                return prefs.GetString(key, "");
            }
            return "";
        }
    }
}


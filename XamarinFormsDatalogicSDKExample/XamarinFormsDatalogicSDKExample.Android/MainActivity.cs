using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Datalogic.Decode;
using Android.Util;
using Com.Datalogic.Device;
using Xamarin.Forms;

namespace XamarinFormsDatalogicSDKExample.Droid
{
    [Activity(Label = "XamarinFormsDatalogicSDKExample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IReadListener
    {
        private readonly string LOGTAG = typeof(MainActivity).Name;

        BarcodeManager decoder = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnResume()
        {
            base.OnResume();

            Log.Info(LOGTAG, "OnResume");

            // If the decoder instance is null, create it.
            if (decoder == null)
            {
                // Remember an onPause call will set it to null.
                decoder = new BarcodeManager();
            }

            // From here on, we want to be notified with exceptions in case of errors.
            ErrorManager.EnableExceptions(true);

            try
            {
                // add our class as a listener
                decoder.AddReadListener(this);
            }
            catch (DecodeException e)
            {
                Log.Error(LOGTAG, "Error while trying to bind a listener to BarcodeManager", e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            Log.Info(LOGTAG, "onPause");

            // If we have an instance of BarcodeManager.
            if (decoder != null)
            {
                try
                {
                    // Unregister our listener from it and free resources
                    decoder.RemoveReadListener(this);
                }
                catch (Exception e)
                {
                    Log.Error(LOGTAG, "Error while trying to remove a listener from BarcodeManager", e);
                }
            }
        }

        void IReadListener.OnRead(IDecodeResult decodeResult)
        {
            //// Change the displayed text to the current received result.
            //mBarcodeText.Text = decodeResult.Text;
            //mSymbology.Text = decodeResult.BarcodeID.ToString();
            MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "Barcode", decodeResult.Text);
        }
    }
}
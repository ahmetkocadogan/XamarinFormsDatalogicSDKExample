# XamarinFormsDatalogicSDKExample
Sample Xamarin.Forms app that uses Datalogic Xamarin SDK 

Datalogic Xamarin SDK website is https://datalogic.github.io/xamarin/

But their examples are not based on Xamarin.Forms. 

This Xamarin.Forms app is default MasterDetailApp while creating new Xamarin.Forms app.

After creating the app, here are the steps to get it working.

1 - Install Datalogic Xamarin SDK from NuGet 
https://datalogic.github.io/xamarin/quick-start/#NuGet

2 - Update Android Manifest file
https://datalogic.github.io/xamarin/quick-start/#Android%20manifest

3- Update MainActivity.cs file located in Android Project.

    3.1 - Add IReadListener Interface
    3.2 - Insert code below into MainActivity class
        private readonly string LOGTAG = typeof(MainActivity).Name;
        BarcodeManager decoder = null;
    3.3 - Add code below into MainActivity class
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
            MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "Barcode", decodeResult.Text);
        }
4- Update ItemsViewModel class to receive the barcode text ( Thanks Xamarin Forum member https://forums.xamarin.com/profile/AlessandroCaliaro )
https://forums.xamarin.com/discussion/comment/383665#Comment_383665
    4.1 - Inside ItemsViewModel Constructor, add code below
            MessagingCenter.Subscribe<App, string>(this, "Barcode", async (sender, thebarcode) => {

                Items.Clear();
                Item i = new Item() { Text = thebarcode, Description = DateTime.Now.ToString() };
                Items.Add(i);
                await DataStore.AddItemAsync(i);
            });
            
Run the app, and scan barcodes. Barcodes which decoded will appear on screen in the list. 

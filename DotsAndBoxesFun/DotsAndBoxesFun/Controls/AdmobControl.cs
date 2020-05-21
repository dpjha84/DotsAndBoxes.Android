using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DotsAndBoxesFun.Controls
{
    public class AdmobControl : ContentView
    {
        public AdmobControl()
        {
            AdUnitId = AppConstants.BannerId;
        }
        public static readonly BindableProperty AdUnitIdProperty = BindableProperty.Create(
                       nameof(AdUnitId),
                       typeof(string),
                       typeof(AdmobControl),
                       string.Empty);

        public string AdUnitId
        {
            get => (string)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }
    }
}

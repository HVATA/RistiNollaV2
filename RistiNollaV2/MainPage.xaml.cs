namespace RistiNollaV2;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent(); //Luominen
    }
    private async void ToGamePage(object sender, EventArgs e)
    {
        GamePage gamepage = new GamePage();
        await Navigation.PushAsync(gamepage);
    }
    private async void ToStatsPage(object sender, EventArgs e)
    {
        StatsPage statspage = new StatsPage();
        await Navigation.PushAsync(statspage);
    }
    //Halusin tuoda säännöt esille popup-ikkunalla, koska mielestäni säännöt olivat lyhyet ja ytimekäs popup-ikkuna on trendikästä :D
    private async void PopUpSaannot(object sender, EventArgs e)
    {
        await DisplayAlert("Säännöt", "Kumpikin pelaaja vuoron perään piirtää oman merkkinsä, joko ristin tai ympyrän." +
                            "\nTavoitteena on saada kolme omaa merkkiä samalle pysty-, vaaka- tai vinoriville. " +
                            "\nPeli päättyy aina tasapeliin ellei toinen tee virhettä. ", "OK");
    }
    private void shutDown(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}
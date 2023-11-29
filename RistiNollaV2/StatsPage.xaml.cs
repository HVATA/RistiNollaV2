using System.Collections.ObjectModel;
using System.Xml.Serialization;
namespace RistiNollaV2;

public partial class StatsPage : ContentPage
{
    ObservableCollection<PlayerInfo> kayttajat;
    ObservableCollection<PlayerInfo> tietokoneet;
    public StatsPage()
	{
		InitializeComponent();

        kayttajat = PlayerInfo.LoadFromXml("c:\\temp\\kayttajat.xml");
        Lista.ItemsSource = kayttajat;

        tietokoneet = PlayerInfo.LoadFromXml("c:\\temp\\compelaaja.xml");
        KoneLista.ItemsSource = tietokoneet;
    }
    
    private void ToMenuTwo(object sender, EventArgs e)
    {
        Navigation.RemovePage(this);
    }
}
using System.Collections.ObjectModel;
using System.Xml.Serialization;


namespace RistiNollaV2;

public partial class ModalPage : ContentPage
{
    ObservableCollection<PlayerInfo> IhmisLista;
    public PlayerInfo selectedPlayer;
    
    public TaskCompletionSource<bool> PageClosedTask { get; private set;}

    public ModalPage()
    {
        InitializeComponent();
        vvButton.IsEnabled = false;
        BindingContext = IhmisLista;
        IhmisLista = PlayerInfo.LoadFromXml("c:\\temp\\kayttajat.xml");
        picker.ItemsSource = IhmisLista;

        picker.SelectedIndexChanged += (sender, args) =>
        {
            int selectedIndex = picker.SelectedIndex;
            if (selectedIndex != -1)
            {
                selectedPlayer = IhmisLista[selectedIndex];
                vvButton.IsEnabled = true;
            }
            else
                vvButton.IsEnabled = false;
        };
        //Luo TaskCompletionSource, joka merkitsee, kun sivu suljetaan
        PageClosedTask = new TaskCompletionSource<bool>();
    }
    private async void ViimeisteleValinta(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        //Merkitse TaskCompletionSourcen suoritetuksi, kun sivu sulkeutuu
        PageClosedTask.SetResult(true);
    }
}   
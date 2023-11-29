using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Microsoft.Maui.Controls;
using System.Timers;
using Microsoft.Maui.Controls.Compatibility.Platform;
using System.Net.Mail;
using System.Windows.Input;

namespace RistiNollaV2;

public partial class GamePage : ContentPage
{
    public ObservableCollection<PlayerInfo> IhmisLista;
    public ObservableCollection<PlayerInfo> KoneLista;
    public Peli pelaajaX;
    public Peli pelaajaO;
    public Peli Voittaja;
    public Peli pMikaPeli;
    public int IRLVarmistus = 0; //varmistetaan ett‰ pelaaja on ihminen
    public Peli peli;
    public System.Timers.Timer ajastin;
    public int kestoSek;
    public int kestoMin;
    public int kestoSekYht;
    public int iMikaPeli = 0;
    public System.Timers.Timer mietintaTime = new System.Timers.Timer();
    public List<int> pisteet = new List<int>();
    public string ekavapaa = "";


    public GamePage()
    {
        InitializeComponent();
        peli = new Peli();
        peli.iLaskuri = 1;
        IhmisLista = new ObservableCollection<PlayerInfo>();
        KoneLista = new ObservableCollection<PlayerInfo>();
        string polku = @"C:\temp";

        if (!Directory.Exists(polku))
            Directory.CreateDirectory(polku);

        if (File.Exists("c:\temp\\kayttajat.xml"))
        {
            IhmisLista = DeserializeXML();
        }
        if (File.Exists("c:\temp\\compelaaja.xml"))
        {
            KoneLista = ComDeserializeXML();
        }
        if (KoneLista.Count == 0)
        {
            PlayerInfo newUser = new PlayerInfo();
            newUser.Etunimi = "COM";
            newUser.Sukunimi = "Tietokone";
            newUser.Nimi = newUser.LuoNimi(newUser.Etunimi, newUser.Sukunimi);
            newUser.Syntymavuosi = "2023";
            KoneLista.Add(newUser);
            SendUserCOM(KoneLista);
        }
        foreach (PlayerInfo p in KoneLista)
        {
            if (p.Nimi == "COM Tietokone")
            {
                peli.Tietokone = p;
            }
        }
        ajastin = new System.Timers.Timer(1000);
        ajastin.Elapsed += ajastin_Tick;
        kestoMin = 0;
        kestoSek = 0; //kestomuuttuja
        kestoSekYht = 0;
        MultiVaiSingle();
    }
    private void ajastin_Tick(object sender, ElapsedEventArgs e)
    {//lis‰t‰‰n sekunnit kestomuuttujaan
        if (kestoSek < 59)
        {
            kestoSekYht++;
            kestoSek++;
        }
        else
        {
            kestoMin++;
            kestoSek = 0;
        }
    }
    public async void MultiVaiSingle()
    {
        bool valitsePeliMuoto = await DisplayAlert("PeliMuoto", "", "Kaksinpeli",
           "Yksinpeli");
        if (valitsePeliMuoto)
        {
            iMikaPeli++;
        }
        else
        {
            iMikaPeli++;
            iMikaPeli++;
        }
        KayttajaValinta();
    }
    public async void KayttajaValinta() //Keskener‰inen, puuttuu tietokonepelaajan-valinta, aika ei riitt‰nyt
    {
        if (iMikaPeli == 1) //Singleplayer
        {
            bool UserSelection = await DisplayAlert("Profiilin valinta", "Haluatko luoda uuden k‰ytt‰j‰profiilin vai k‰ytt‰‰ vanhaa?", "Luo uusi k‰ytt‰j‰",
           "K‰yt‰ olemassaolevaa k‰ytt‰j‰‰");

            if (UserSelection)
            {   //T‰ss‰ luodaan uutta k‰ytt‰j‰‰!
                PlayerInfo newUser = new PlayerInfo();
                string uusiEnimi = await DisplayPromptAsync("", "Anna etunimi");
                newUser.Etunimi = uusiEnimi;
                string uusiSnimi = await DisplayPromptAsync("", "Anna sukunimi");
                newUser.Sukunimi = uusiSnimi;
                newUser.Nimi = newUser.LuoNimi(newUser.Etunimi, newUser.Sukunimi);
                string uusiSyntymavuosi = await DisplayPromptAsync("", "Anna syntym‰vuosi");
                newUser.Syntymavuosi = uusiSyntymavuosi;
                IhmisLista.Add(newUser);
                peli.pelaajaX = newUser; //lis‰t‰‰n uusi pelaaja aktiivikseksi k‰ytt‰j‰ksi.
                EntryPelaaja1.SetBinding(Entry.TextProperty, new Binding("Etunimi") { Source = peli.pelaajaX });
                IRLVarmistus++;
            }
            else
            {
                ModalPage modalPage = new ModalPage();
                await Navigation.PushModalAsync(modalPage);
                await modalPage.PageClosedTask.Task;
                peli.pelaajaX = modalPage.selectedPlayer; //lis‰t‰‰n haettu pelaaja aktiivikseksi k‰ytt‰j‰ksi.
                EntryPelaaja1.SetBinding(Entry.TextProperty, new Binding("Etunimi") { Source = peli.pelaajaX });
                IRLVarmistus++;
            }
            peli.vuorossa = peli.pelaajaX;
            bool OpponentSelection = await DisplayAlert("Vastustajan valinta", "", "Luo uusi k‰ytt‰j‰",
               "Olemassa oleva k‰ytt‰j‰");

            if (OpponentSelection)
            {
                PlayerInfo newUser = new PlayerInfo();
                string uusiEnimi = await DisplayPromptAsync("", "Anna etunimi");
                newUser.Etunimi = uusiEnimi;
                string uusiSnimi = await DisplayPromptAsync("", "Anna sukunimi");
                newUser.Sukunimi = uusiSnimi;
                newUser.Nimi = newUser.LuoNimi(newUser.Etunimi, newUser.Sukunimi);
                string uusiSyntymavuosi = await DisplayPromptAsync("", "Anna syntym‰vuosi");
                newUser.Syntymavuosi = uusiSyntymavuosi;
                IhmisLista.Add(newUser);
                peli.pelaajaO = newUser; //lis‰t‰‰n uusi pelaaja aktiivikseksi k‰ytt‰j‰ksi.
                EntryPelaaja2.SetBinding(Entry.TextProperty, new Binding("Etunimi") { Source = peli.pelaajaO });
                IRLVarmistus++;
                GameStart();
            }
            else
            {

                ModalPage modalPage = new ModalPage();
                await Navigation.PushModalAsync(modalPage);
                await modalPage.PageClosedTask.Task;
                peli.pelaajaO = modalPage.selectedPlayer;


                EntryPelaaja2.SetBinding(Entry.TextProperty, new Binding("Etunimi") { Source = peli.pelaajaO });
                GameStart();
                IRLVarmistus++; //varmistetaan ett‰ pelaaja on ihminen
            }
            SendUsers(IhmisLista);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        else if (iMikaPeli == 2)//Multiplayer
        {
            bool UserSelection = await DisplayAlert("Profiilin valinta", "Haluatko luoda uuden k‰ytt‰j‰profiilin vai k‰ytt‰‰ vanhaa?", "Luo uusi k‰ytt‰j‰",
           "K‰yt‰ olemassaolevaa k‰ytt‰j‰‰");

            if (UserSelection)
            {   //T‰ss‰ luodaan uutta k‰ytt‰j‰‰!
                PlayerInfo newUser = new PlayerInfo();
                string uusiEnimi = await DisplayPromptAsync("", "Anna etunimi");
                newUser.Etunimi = uusiEnimi;
                string uusiSnimi = await DisplayPromptAsync("", "Anna sukunimi");
                newUser.Sukunimi = uusiSnimi;
                newUser.Nimi = newUser.LuoNimi(newUser.Etunimi, newUser.Sukunimi);
                string uusiSyntymavuosi = await DisplayPromptAsync("", "Anna syntym‰vuosi");
                newUser.Syntymavuosi = uusiSyntymavuosi;
                IhmisLista.Add(newUser);
                peli.pelaajaX = newUser; //lis‰t‰‰n uusi pelaaja aktiivikseksi k‰ytt‰j‰ksi.
                EntryPelaaja1.SetBinding(Entry.TextProperty, new Binding("Etunimi") { Source = peli.pelaajaX });
                IRLVarmistus++;
            }
            else
            {
                ModalPage modalPage = new ModalPage();
                await Navigation.PushModalAsync(modalPage);
                await modalPage.PageClosedTask.Task;
                peli.pelaajaX = modalPage.selectedPlayer; //lis‰t‰‰n haettu pelaaja aktiivikseksi k‰ytt‰j‰ksi.
                EntryPelaaja1.SetBinding(Entry.TextProperty, new Binding("Etunimi") { Source = peli.pelaajaX });
                IRLVarmistus++;
            }
            SendUsers(IhmisLista);
            peli.vuorossa = peli.pelaajaX;
            peli.pelaajaO = peli.Tietokone;
            EntryPelaaja2.SetBinding(Entry.TextProperty, new Binding("Etunimi") { Source = peli.pelaajaO });
            GameStart();
        }

    }
    public void SendUserCOM(ObservableCollection<PlayerInfo> KoneLista)
    {
        try
        {   //Tarkista, onko tiedosto jo olemassa
            string filePath = "c:\\temp\\comPelaaja.xml";

            //Jos tiedostoa ei ole olemassa, luo uusi tiedosto ja tallenna tiedot siihen
            XmlSerializer saver = new XmlSerializer(KoneLista.GetType());
            var sw = new StreamWriter(filePath);
            saver.Serialize(sw, KoneLista);
            sw.Close();

        }
        catch (Exception e)
        {
            DisplayAlert("Virhe", e.Message, "OK");
        }
    }
    public void SendUsers(ObservableCollection<PlayerInfo> IhmisLista)
    {
        try
        {   //Tarkista, onko tiedosto jo olemassa
            string filePath = "c:\\temp\\kayttajat.xml";
            if (File.Exists(filePath))
            {
                //Luetaan sis‰ltˆ jos olemassa
                XmlSerializer loader = new XmlSerializer(typeof(ObservableCollection<PlayerInfo>));
                var sr = new StreamReader(filePath);
                var existingData = (ObservableCollection<PlayerInfo>)loader.Deserialize(sr);
                sr.Close();



                //Lis‰‰ uusi k‰ytt‰j‰ olemassa oleviin tietoihin
                foreach (var newUser in IhmisLista)
                    existingData.Add(newUser);

                //Tallennetaan p‰ivitetyt tiedot
                XmlSerializer saver = new XmlSerializer(existingData.GetType());
                var sw = new StreamWriter(filePath);
                saver.Serialize(sw, existingData);
                sw.Close();

            }
            else
            {
                //Jos tiedostoa ei ole olemassa, luo uusi tiedosto ja tallenna tiedot siihen
                XmlSerializer saver = new XmlSerializer(IhmisLista.GetType());
                var sw = new StreamWriter(filePath);
                saver.Serialize(sw, IhmisLista);
                sw.Close();
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Virhe", e.Message, "OK");
        }
    }
    public void SendUsersAgain()
    {
        try
        {   //Tarkista, onko tiedosto jo olemassa
            string filePath = "c:\\temp\\kayttajat.xml";
            if (File.Exists(filePath))
            {
                //Luetaan sis‰ltˆ jos olemassa
                XmlSerializer loader = new XmlSerializer(typeof(ObservableCollection<PlayerInfo>));
                var sr = new StreamReader(filePath);
                var existingData = (ObservableCollection<PlayerInfo>)loader.Deserialize(sr);
                sr.Close();



                //Tee muutokset t‰ss‰
                foreach (PlayerInfo p in existingData)
                {
                    if (p.Nimi == peli.pelaajaX.Nimi)
                    {
                        p.Voitot = peli.pelaajaX.Voitot;
                        p.Haviot = peli.pelaajaX.Haviot;
                        p.Tasapelit = peli.pelaajaX.Tasapelit;
                        p.PelienYhteisKesto = peli.pelaajaX.PelienYhteisKesto;
                    }
                    if (p.Nimi == peli.pelaajaO.Nimi)
                    {
                        p.Voitot = peli.pelaajaO.Voitot;
                        p.Haviot = peli.pelaajaO.Haviot;
                        p.Tasapelit = peli.pelaajaO.Tasapelit;
                        p.PelienYhteisKesto = peli.pelaajaO.PelienYhteisKesto;
                    }
                }

                //Tallennetaan p‰ivitetyt tiedot
                XmlSerializer saver = new XmlSerializer(existingData.GetType());
                var sw = new StreamWriter(filePath);
                saver.Serialize(sw, existingData);
                sw.Close();

            }
            else
            {
                //Jos tiedostoa ei ole olemassa, luo uusi tiedosto ja tallenna tiedot siihen
                XmlSerializer saver = new XmlSerializer(IhmisLista.GetType());
                var sw = new StreamWriter(filePath);
                saver.Serialize(sw, IhmisLista);
                sw.Close();
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Virhe", e.Message, "OK");
        }
    }
    public void SendUserComAgain()
    {
        try
        {   //Tarkista, onko tiedosto jo olemassa
            string filePath = "c:\\temp\\compelaaja.xml";
            if (File.Exists(filePath))
            {
                //Luetaan sis‰ltˆ jos olemassa
                XmlSerializer loader = new XmlSerializer(typeof(ObservableCollection<PlayerInfo>));
                var sr = new StreamReader(filePath);
                var existingData = (ObservableCollection<PlayerInfo>)loader.Deserialize(sr);
                sr.Close();



                //Tee muutokset t‰ss‰
                foreach (PlayerInfo p in existingData)
                {
                    if (p.Nimi == peli.pelaajaO.Nimi)
                    {
                        p.Voitot = peli.pelaajaO.Voitot;
                        p.Haviot = peli.pelaajaO.Haviot;
                        p.Tasapelit = peli.pelaajaO.Tasapelit;
                        p.PelienYhteisKesto = peli.pelaajaO.PelienYhteisKesto;
                    }
                }

                //Tallennetaan p‰ivitetyt tiedot
                XmlSerializer saver = new XmlSerializer(existingData.GetType());
                var sw = new StreamWriter(filePath);
                saver.Serialize(sw, existingData);
                sw.Close();

            }
            else
            {
                //Jos tiedostoa ei ole olemassa, luo uusi tiedosto ja tallenna tiedot siihen
                XmlSerializer saver = new XmlSerializer(KoneLista.GetType());
                var sw = new StreamWriter(filePath);
                saver.Serialize(sw, KoneLista);
                sw.Close();
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Virhe", e.Message, "OK");
        }
    }
    public ObservableCollection<PlayerInfo> DeserializeXML()
    {
        string filePath = "c:\\temp\\kayttajat.xml";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            XmlSerializer bringer = new XmlSerializer(typeof(ObservableCollection<PlayerInfo>));
            object obj = bringer.Deserialize(sr);
            sr.Close();
            return (ObservableCollection<PlayerInfo>)obj;
        }
        else
            return null;
    }
    public ObservableCollection<PlayerInfo> ComDeserializeXML()
    {
        string filePath = "c:\\temp\\compelaaja.xml";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            XmlSerializer bringer = new XmlSerializer(typeof(ObservableCollection<PlayerInfo>));
            object obj = bringer.Deserialize(sr);
            sr.Close();
            return (ObservableCollection<PlayerInfo>)obj;
        }
        else
            return null;
    }
    public async void GameStart()
    {
        await DisplayAlert("", "Peli alkaa", "Ok");
        ajastin.Start();

    }
    public void TutkiSiirto()
    {
        pisteet.Clear();
        int iLask = 0;
        if (one.Source.ToString().Contains("nolla.png"))//1
        {
            iLask++;
        }
        if (two.Source.ToString().Contains("nolla.png"))//2
        {
            iLask++;
        }
        if (thr.Source.ToString().Contains("nolla.png"))//3
        {
            iLask++;
        }
        pisteet.Add(iLask);
        iLask = 0;
        if (fou.Source.ToString().Contains("nolla.png"))//4
        {
            iLask++;
        }
        if (fiv.Source.ToString().Contains("nolla.png"))//5
        {
            iLask++;
        }
        if (six.Source.ToString().Contains("nolla.png"))//6
        {
            iLask++;
        }
        pisteet.Add(iLask);
        iLask = 0;
        if (sev.Source.ToString().Contains("nolla.png"))//7
        {
            iLask++;
        }
        if (eig.Source.ToString().Contains("nolla.png"))//8
        {
            iLask++;
        }
        if (nin.Source.ToString().Contains("nolla.png"))//9
        {
            iLask++;
        }
        pisteet.Add(iLask);
        iLask = 0;
        if (one.Source.ToString().Contains("nolla.png"))//1
        {
            iLask++;
        }
        if (fou.Source.ToString().Contains("nolla.png"))//4
        {
            iLask++;
        }
        if (sev.Source.ToString().Contains("nolla.png"))//7
        {
            iLask++;
        }
        pisteet.Add(iLask);
        iLask = 0;
        if (two.Source.ToString().Contains("nolla.png"))//2
        {
            iLask++;
        }
        if (fiv.Source.ToString().Contains("nolla.png"))//5
        {
            iLask++;
        }
        if (eig.Source.ToString().Contains("nolla.png"))//8
        {
            iLask++;
        }
        pisteet.Add(iLask);
        iLask = 0;
        if (thr.Source.ToString().Contains("nolla.png"))//3
        {
            iLask++;
        }
        if (six.Source.ToString().Contains("nolla.png"))//6
        {
            iLask++;
        }
        if (nin.Source.ToString().Contains("nolla.png"))//9
        {
            iLask++;
        }
        pisteet.Add(iLask);
        iLask = 0;
        if (one.Source.ToString().Contains("nolla.png"))//1
        {
            iLask++;
        }
        if (fiv.Source.ToString().Contains("nolla.png"))//5
        {
            iLask++;
        }
        if (nin.Source.ToString().Contains("nolla.png"))//9
        {
            iLask++;
        }
        pisteet.Add(iLask);
        iLask = 0;
        if (thr.Source.ToString().Contains("nolla.png"))//3
        {
            iLask++;
        }
        if (fiv.Source.ToString().Contains("nolla.png"))//5
        {
            iLask++;
        }
        if (sev.Source.ToString().Contains("nolla.png"))//7
        {
            iLask++;
        }
        pisteet.Add(iLask);
    }
    public void TeeSiirto()
    {
        bool vuoroOhi = false;
        foreach (int i in pisteet)
        {
            if(vuoroOhi == false)
            {
                if (pisteet[i] == 2)
                {
                    if (VoikoTahanLaittaa(i) == true)
                    {
                        DisplayAlert("ok", "ok", "ok");
                    }
                }
                else if (OnkoTilaa() == true)
                {
                    if (ekavapaa == "one")
                    {
                        one.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "two")
                    {
                        two.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "thr")
                    {
                        thr.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "fou")
                    {
                        fou.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "fiv")
                    {
                        fiv.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "six")
                    {
                        six.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "sev")
                    {
                        sev.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "eig")
                    {
                        eig.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                    else if (ekavapaa == "nin")
                    {
                        nin.Source = ImageSource.FromFile("nolla.png");
                        vuoroOhi = true;
                    }
                }
            }
            
        }
    }
    public bool VoikoTahanLaittaa(int i)
    {
        if(i == 0)
        {
            if (one.Source.ToString().Contains("eimitaan.png"))//1
            {
                one.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (two.Source.ToString().Contains("eimitaan.png"))//2
            {
                two.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (thr.Source.ToString().Contains("eimitaan.png"))//3
            {
                thr.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        else if (i == 1)
        {
            if (fou.Source.ToString().Contains("eimitaan.png"))//1
            {
                fou.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (fiv.Source.ToString().Contains("eimitaan.png"))//2
            {
                fiv.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (six.Source.ToString().Contains("eimitaan.png"))//3
            {
                six.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        else if (i == 2)
        {
            if (sev.Source.ToString().Contains("eimitaan.png"))//1
            {
                sev.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (eig.Source.ToString().Contains("eimitaan.png"))//2
            {
                eig.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (nin.Source.ToString().Contains("eimitaan.png"))//3
            {
                nin.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        else if (i == 3)
        {
            if (one.Source.ToString().Contains("eimitaan.png"))//1
            {
                one.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (fou.Source.ToString().Contains("eimitaan.png"))//2
            {
                fou.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (sev.Source.ToString().Contains("eimitaan.png"))//3
            {
                sev.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        else if (i == 4)
        {
            if (two.Source.ToString().Contains("eimitaan.png"))//1
            {
                two.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (fiv.Source.ToString().Contains("eimitaan.png"))//2
            {
                fiv.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (eig.Source.ToString().Contains("eimitaan.png"))//3
            {
                eig.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        else if (i == 5)
        {
            if (thr.Source.ToString().Contains("eimitaan.png"))//1
            {
                thr.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (six.Source.ToString().Contains("eimitaan.png"))//2
            {
                six.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (nin.Source.ToString().Contains("eimitaan.png"))//3
            {
                nin.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        else if (i == 6)
        {
            if (one.Source.ToString().Contains("eimitaan.png"))//1
            {
                one.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (fiv.Source.ToString().Contains("eimitaan.png"))//2
            {
                fiv.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (nin.Source.ToString().Contains("eimitaan.png"))//3
            {
                nin.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        else if (i == 7)
        {
            if (thr.Source.ToString().Contains("eimitaan.png"))//1
            {
                thr.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (fiv.Source.ToString().Contains("eimitaan.png"))//2
            {
                fiv.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
            else if (sev.Source.ToString().Contains("eimitaan.png"))//3
            {
                sev.Source = ImageSource.FromFile("nolla.png");
                return true;
            }
        }
        return false;
    }
    public async Task Pohdinta()
    {
        Random rnd = new Random();
        int aika = rnd.Next(500, 2001);
        await Task.Delay(aika);
        TutkiSiirto();
        TeeSiirto();
        peli.VaihdaVuoroa();
        await DisplayAlert("Vuoron vaihto", $"Pelaaja {peli.vuorossa.Etunimi} vuorossa", "ok");
    }
    public async Task<bool> Jatketaanko()
    {
        if (LoytyykoVoittaja() == false)
        {
            if (OnkoTilaa() == true)
            {
                if (iMikaPeli == 2)
                {
                    peli.VaihdaVuoroa();
                    await DisplayAlert("Vuoron vaihto", $"Pelaaja {peli.vuorossa.Etunimi} vuorossa", "ok");
                    await Pohdinta();
                    return true;
                }
                else
                {
                    peli.VaihdaVuoroa();
                    await DisplayAlert("Vuoron vaihto", $"Pelaaja {peli.vuorossa.Etunimi} vuorossa", "ok");
                    return true;
                }

            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }
    public bool LoytyykoVoittaja()
    {   //jos alkiot (1, 2, 3) tai (4, 5, 6) tai (7, 8, 9) tai (1, 4, 7) tai (2, 5, 8) tai (3, 6, 9) tai (1, 5, 9) tai (3, 5, 7) ovat samat,
        //voittaja lˆytyi
        if (one.Source.ToString().Contains("nolla.png")
            && two.Source.ToString().Contains("nolla.png")
            && thr.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (one.Source.ToString().Contains("aksa.png")
            && two.Source.ToString().Contains("aksa.png")
            && thr.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else if (fou.Source.ToString().Contains("nolla.png")
            && fiv.Source.ToString().Contains("nolla.png")
            && six.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (fou.Source.ToString().Contains("aksa.png")
            && fiv.Source.ToString().Contains("aksa.png")
            && six.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else if (sev.Source.ToString().Contains("nolla.png")
            && eig.Source.ToString().Contains("nolla.png")
            && nin.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO; ;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (sev.Source.ToString().Contains("aksa.png")
            && eig.Source.ToString().Contains("aksa.png")
            && nin.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else if (one.Source.ToString().Contains("nolla.png")
            && fou.Source.ToString().Contains("nolla.png")
            && sev.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (one.Source.ToString().Contains("aksa.png")
            && fou.Source.ToString().Contains("aksa.png")
            && sev.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else if (two.Source.ToString().Contains("nolla.png")
            && fiv.Source.ToString().Contains("nolla.png")
            && eig.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (two.Source.ToString().Contains("aksa.png")
            && fiv.Source.ToString().Contains("aksa.png")
            && eig.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else if (thr.Source.ToString().Contains("nolla.png")
            && six.Source.ToString().Contains("nolla.png")
            && nin.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (thr.Source.ToString().Contains("aksa.png")
            && six.Source.ToString().Contains("aksa.png")
            && nin.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else if (one.Source.ToString().Contains("nolla.png")
            && fiv.Source.ToString().Contains("nolla.png")
            && nin.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (one.Source.ToString().Contains("aksa.png")
            && fiv.Source.ToString().Contains("aksa.png")
            && nin.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else if (thr.Source.ToString().Contains("nolla.png")
            && fiv.Source.ToString().Contains("nolla.png")
            && sev.Source.ToString().Contains("nolla.png")) //0
        {
            peli.Voittaja = peli.pelaajaO;
            peli.pelaajaO.Voitot++;
            peli.pelaajaX.Haviot++;
            return true;
        }
        else if (thr.Source.ToString().Contains("aksa.png")
            && fiv.Source.ToString().Contains("aksa.png")
            && sev.Source.ToString().Contains("aksa.png")) //X
        {
            peli.Voittaja = peli.pelaajaX;
            peli.pelaajaX.Voitot++;
            peli.pelaajaO.Haviot++;
            return true;
        }
        else
            return false;
    }
    public bool OnkoTilaa()
    {
        if (one.Source.ToString().Contains("eimitaan.png"))//Jos lˆytyy ainakin yksi button, jonka tila on haluttu, palauta true
        {
            ekavapaa = "one";
            return true;
        }
        if (two.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "two";
            return true;
        }

        if (thr.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "thr";
            return true;
        }

        if (fou.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "fou";
            return true;
        }

        if (fiv.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "fiv";
            return true;
        }

        if (six.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "six";
            return true;
        }

        if (sev.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "sev";
            return true;
        }

        if (eig.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "eig";
            return true;
        }
        if (nin.Source.ToString().Contains("eimitaan.png"))
        {
            ekavapaa = "nin";
            return true;
        }

        return false; //Jos kaikki buttonit ovat halutussa tilassa, palauta false
    }
    public async void PaataPeli()
    {
        ajastin.Stop();
        peli.pelaajaO.PelienYhteisKesto += kestoSekYht;
        peli.pelaajaX.PelienYhteisKesto += kestoSekYht;
        if (iMikaPeli == 1)
        {
            if (LoytyykoVoittaja() == true)
            {
                await DisplayAlert("Peli on p‰‰ttynyt", $"Voittaja on {peli.Voittaja.Etunimi} \nPelin kesto oli: {kestoMin} minuuttia {kestoSek} sekunttia", "Ok");
                if (peli.Voittaja == peli.pelaajaO)
                {
                    peli.pelaajaO.Voitot--;
                    peli.pelaajaX.Haviot--;//est‰‰ pisteiden tuplaantumisen
                }
                else if (peli.Voittaja == peli.pelaajaX)
                {
                    peli.pelaajaX.Voitot--;
                    peli.pelaajaO.Haviot--;//est‰‰ pisteiden tuplaantumisen
                }

            }
            else if (OnkoTilaa() == false)
            {
                peli.pelaajaO.Tasapelit++;
                peli.pelaajaX.Tasapelit++;
                await DisplayAlert("Peli on p‰‰ttynyt", $"Peli p‰‰ttyi tasapeliin \nPelin kesto oli: {kestoMin} minuuttia {kestoSek} sekunttia", "Ok");
            }
            // Tallenna k‰ytt‰jien tiedot
            SendUsersAgain();
            Navigation.RemovePage(this);
        }
        else if (iMikaPeli == 2)
        {
            if (LoytyykoVoittaja() == true)
            {
                await DisplayAlert("Peli on p‰‰ttynyt", $"Voittaja on {peli.Voittaja.Etunimi} \nPelin kesto oli: {kestoMin} minuuttia {kestoSek} sekunttia", "Ok");
                if (peli.Voittaja == peli.pelaajaO)
                {
                    peli.pelaajaO.Voitot--;
                    peli.pelaajaX.Haviot--;//est‰‰ pisteiden tuplaantumisen
                }
                else if (peli.Voittaja == peli.pelaajaX)
                {
                    peli.pelaajaX.Voitot--;
                    peli.pelaajaO.Haviot--;//est‰‰ pisteiden tuplaantumisen
                }

            }
            else if (OnkoTilaa() == false)
            {
                peli.pelaajaO.Tasapelit++;
                peli.pelaajaX.Tasapelit++;
                await DisplayAlert("Peli on p‰‰ttynyt", $"Peli p‰‰ttyi tasapeliin \nPelin kesto oli: {kestoMin} minuuttia {kestoSek} sekunttia", "Ok");
            }
            // Tallenna k‰ytt‰jien tiedot
            SendUsersAgain();
            SendUserComAgain();
            Navigation.RemovePage(this);
        }

    }
    public async void oneBtnClicked(object sender, EventArgs e)
    {
        if (one.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                one.Source = ImageSource.FromFile("aksa.png");
            else
                one.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void twoBtnClicked(object sender, EventArgs e)
    {
        if (two.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                two.Source = ImageSource.FromFile("aksa.png");
            else
                two.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void thrBtnClicked(object sender, EventArgs e)
    {
        if (thr.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                thr.Source = ImageSource.FromFile("aksa.png");
            else
                thr.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void fouBtnClicked(object sender, EventArgs e)
    {
        if (fou.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                fou.Source = ImageSource.FromFile("aksa.png");
            else
                fou.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void fivBtnClicked(object sender, EventArgs e)
    {
        if (fiv.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                fiv.Source = ImageSource.FromFile("aksa.png");
            else
                fiv.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void sixBtnClicked(object sender, EventArgs e)
    {
        if (six.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                six.Source = ImageSource.FromFile("aksa.png");
            else
                six.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void sevBtnClicked(object sender, EventArgs e)
    {
        if (sev.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                sev.Source = ImageSource.FromFile("aksa.png");
            else
                sev.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void eigBtnClicked(object sender, EventArgs e)
    {
        if (eig.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                eig.Source = ImageSource.FromFile("aksa.png");
            else
                eig.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    public async void ninBtnClicked(object sender, EventArgs e)
    {
        if (nin.Source.ToString().Contains("eimitaan.png"))
        {
            if (peli.vuorossa == peli.pelaajaX)
                nin.Source = ImageSource.FromFile("aksa.png");
            else
                nin.Source = ImageSource.FromFile("nolla.png");

            if (await Jatketaanko() == false)
                PaataPeli();
        }
    }
    private void ToMenu(object sender, EventArgs e)
    {
        ajastin.Stop();
        Navigation.RemovePage(this);
    }
}
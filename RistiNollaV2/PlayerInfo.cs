using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RistiNollaV2
{
    public class PlayerInfo
    {
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Nimi { get; set; }
        public string Syntymavuosi { get; set; }
        public int Voitot { get; set; }
        public int Haviot { get; set; }
        public int Tasapelit { get; set; }
        public int PelienYhteisKesto { get; set; }
        public string LuoNimi(string Etunimi, string Sukunimi)
        {
            Nimi = Etunimi + " " + Sukunimi;
            return Nimi;
        }
       
        //Lisäsin tohon alle tommosen koodin pätkän, joka helpottaa käyttäjien hakemista tiedostosta.
        //EI tarvii erikseen aina erikseen tehä funktiota
        public static ObservableCollection<PlayerInfo> LoadFromXml(string filePath)
        {
            ObservableCollection<PlayerInfo> result = new ObservableCollection<PlayerInfo>();
            if (File.Exists(filePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<PlayerInfo>));
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        result = (ObservableCollection<PlayerInfo>)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Virhe tiedoston lukemisessa: {ex.Message}");
                }
            }
            return result;
        }
    }
}

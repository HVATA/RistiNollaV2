using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RistiNollaV2
{
    public class Peli
    {
        public PlayerInfo pelaajaX { get; set; }
        public PlayerInfo pelaajaO { get; set; }
        public PlayerInfo Tietokone { get; set; }
        public PlayerInfo vuorossa { get; set; }
        public PlayerInfo Voittaja { get; set; }
        public int iLaskuri { get; set; }
        public void VaihdaVuoroa()
        {

            if (vuorossa == pelaajaX)
            {
                vuorossa = pelaajaO;
                iLaskuri++;
            }
            else if (vuorossa == pelaajaO)
            {
                vuorossa = pelaajaX;
                iLaskuri--;
            }


        }
    }
}

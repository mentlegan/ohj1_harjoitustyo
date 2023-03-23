using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Save_the_Astrobabies : PhysicsGame
{
    /// <summary>
    /// Author @Harri Keränen, @version 1.0
    /// Pelissä kerätään pisteitä
    /// Pisteitä saa keräämällä vauvoja ja tuhoamalla vihollisia
    /// Pelin voittaa, kun kerätään 20 pistettä
    /// Pelin häviää, kun pelaaja saa osuman vihollisen ammuksesta
    /// Lista, funktio ja silmukka TODO-kommenteissa koodin lopussa
    /// </summary>

    /// attribuutteja
    private PhysicsObject vauva;
    private PhysicsObject pelaaja;
    private PhysicsObject vihollinen;
    private IntMeter Pisteet;
    /// attribuutteja


    public override void Begin()
    {
        LuoKentta(); ///luo kentän, pelaajan, ja vihollisen pelin alkuun
        AsetaOhjaimet(); ///luo ohjaimet peliin
        LisaaLaskurit(); ///luo peliin pistelaskurin, jonka avulla peli voitetaan
        LisaaAjastimet(); ///kerättävien vauvojen syntymisajastimen luova aliohjelma
    }


    /// <summary>
    /// pelin voitto: pelaaja kerää 20 pistettä tuhoamalla vihollisia tai keräämällä vauvoja
    /// </summary>
    private void PelinVoitto()
    {
        IsPaused = true;
        MultiSelectWindow Valikko = new MultiSelectWindow("Voitit!", "Lopeta");
        ///luodaan ikkuna, jossa ilmoitetaan pelaajan voittaneen
        Valikko.ItemSelected += PainaValikonNappia;
        Add(Valikko);
    }
    

    /// <summary>
    /// pelin häviö: pelaaja saa osuman vihollisen ammuksesta 
    /// </summary>
    private void PelinHavio()
    {
        IsPaused = true;
        MultiSelectWindow Valikko = new MultiSelectWindow("Hävisit!", "Lopeta");
        ///aliohjelma luo ikkunan, jossa ilmoitetaan pelaajan hävinneen
        Valikko.ItemSelected += PainaValikonNappia;
        Add(Valikko);
    }


    /// <summary>
    /// pelin voiton tai häviön yhteydessä oleva ikkuna
    /// </summary>
    /// <param name="valinta">valintapainike</param>
    private void PainaValikonNappia(int valinta)
    {
        switch (valinta)
        {
            case 0:
                //Lopeta
                Exit();
                break;
        }
    }


    /// <summary>
    ///luodaan alkuasetelma: kenttä, pelaaja, ja yksi vihollinen 
    /// </summary>
    private void LuoKentta()
    {
        vihollinen = LuoVihollinen(Level.Right - 30.0, 0.0 ); ///luodaan vihollinen alkuasetelmaan
    
        pelaaja = LuoPelaaja(Level.Left + 30.0, 0.0 ); ///luodaan pelaaja
    
        Level.BackgroundColor = Color.Black; ///tausta on musta, koska pelin tarina sijoittuu avaruuteen
    }


    /// <summary>
    /// luodaan pelaajan ohjaama olio (sininen alus) kentän vasemmalle puolelle 
    /// </summary>
    /// <param name="x">Pelaajan X-koordinaatti</param>
    /// <param name="y">Pelaajan Y-koordinaatti</param>
    /// <returns></returns>
    private PhysicsObject LuoPelaaja(double x, double y)
    {
        PhysicsObject pelaaja = PhysicsObject.CreateStaticObject(100.0, 50.0);
        pelaaja.Image = LoadImage ("pelaajanalus");
        pelaaja.X = x;
        pelaaja.Y = y;
        pelaaja.Restitution = 0.0;
        pelaaja.Tag = "pelaaja"; ///asetetaan pelaajan tyypille tagi
        AddCollisionHandler(pelaaja, "vihuammus", CollisionHandler.DestroyBoth); ///pelaajan alus tuhoutuu saadessaan osuman vihollisen ammuksesta
        pelaaja.Destroyed += PelinHavio; ///pelin häviää, jos pelaaja tuhoutuu (toteuttaa pelin häviön aliohjelman)
        Add(pelaaja);
        return pelaaja;
    }


    /// <summary>
    /// pelaajan ampumisen asetukset
    /// </summary>
    /// <param name="heittavaOlio">Olio, joka ampuu</param>
    /// <param name="tagi">Ammuksen tagi</param>
    private void Ammu(PhysicsObject heittavaOlio, string tagi)
    {
        PhysicsObject heitettava = new PhysicsObject(25,5,Shape.Rectangle); ///ammusten koko ja muoto
        heitettava.Color = Color.Green; ///vihreä lasersäde
        heitettava.Position = heittavaOlio.Position; ///ammus syntyy ampujan kohdalle
        heitettava.Hit(new Vector(1000, 0));
        heitettava.Tag = tagi;
        heitettava.MaximumLifetime = TimeSpan.FromSeconds(1.5); ///ammukset ehtivät tässä aikavälissä juuri ja juuri vihollisen korkeudelle
        heitettava.CanRotate = false; ///ammukset eivät pyöri osuessaaan johonkin
        heitettava.Restitution = 0;
        Add(heitettava, 1);
    }


    /// <summary>
    /// vihollisen, ja niihin liittyvien tekijöiden luova aliohjelma kokonaisuudessaan 
    /// </summary>
    /// <param name="x">Vihollisen X-koordinaatti</param>
    /// <param name="y">Vihollisen Y-koordinaatti</param>
    /// <returns></returns>
    private PhysicsObject LuoVihollinen(double x, double y)
    {
        PhysicsObject vihollinen = new PhysicsObject(100.0, 50.0); ///luodaan viholliselle tyyppi ja koko (pelin vaikeusastetta ajatellen viholliset ovat saman kokoisia pelaajan kanssa)
        vihollinen.Image = LoadImage("vihualus");
        vihollinen.X = 600.0;
        vihollinen.Y = RandomGen.SelectOne(-350, -300, -250, 200, 150, 100, 50, 0, 50, 100, 150, 200, 250, 300, 350); ///viholliset syntyvät satunnaiseen korkeuteen
        vihollinen.Restitution = 0.0;
        vihollinen.CollisionIgnoreGroup = 1; ///viholliset eivät voi törmätä toisiinsa
        vihollinen.CanRotate = false;
        vihollinen.Tag = "vihollinen"; ///tagi viholliselle

        PathFollowerBrain vihuAivot = new PathFollowerBrain(); ///vihollisen tekoäly
        List<Vector> polku = new List<Vector>(); ///listarakenne koordinaateille, joihin viholliset liikkuvat kentällä
        polku.Add(new Vector(460, 350)); ///lisätään pelikentän koordinaatit listaan
        polku.Add(new Vector(460,-350));
        vihuAivot.Path = polku; ///käytetään listaa vihollisen tekoälyn polkuna
        vihuAivot.Loop = true; ///viholliset jatkavat kulkuaan loputtomiin
        vihuAivot.Speed = RandomGen.SelectOne (100, 150, 200, 250); ///vihollisilla on satunnainen liikkumisnopeus
        vihollinen.Brain = vihuAivot;
        AddCollisionHandler(vihollinen, "pelaajanammus", CollisionHandler.DestroyBoth); ///pelaaja voi tuhota vihollisen ampumalla sitä, samalla tuhoten myös ammuksen
                                                                                        ///ts. pelaaja ei voi tuhota useampaa vihollista yhdellä ammuksella
        Timer ammusajastin = new Timer(); ///ammusajastin = vihollisen ampumisen ajastin
        ammusajastin.Interval = RandomGen.SelectOne(1.0, 1.5, 2.0, 2.5); ///viholliset ampuvat satunnaisella nopeudella
        ammusajastin.Timeout += delegate { VihuAmpuu(vihollinen,"vihuammus"); };
        ammusajastin.Start();
        vihollinen.Destroyed += delegate { ammusajastin.Stop(); }; ///ampuminen loppuu, kun vihollinen tuhoutuu
        vihollinen.Destroyed += delegate { Pisteet.AddValue(1); }; ///vihollisen tuhoamisesta saa pisteen
    
        Add(vihollinen);
        return vihollinen;
    }


    /// <summary>
    /// vihollisten ampuminen omana aliohjelmanaan
    /// </summary>
    /// <param name="heittavaOlio">Olio, joka ampuu</param>
    /// <param name="tagi">ammuksen tagi</param>
    private void VihuAmpuu(PhysicsObject heittavaOlio, string tagi)
    {
        PhysicsObject vheitettava = new PhysicsObject(25, 5, Shape.Rectangle); ///vheitettava = vihollisen ammus
        vheitettava.Color = Color.Red; ///punainen lasersäde
        vheitettava.Position = heittavaOlio.Position;
        vheitettava.Hit(new Vector(-750, 0));
        vheitettava.Tag = tagi;
        vheitettava.MaximumLifetime = TimeSpan.FromSeconds(1.5); ///ammukset ehtivät tässä aikavälissä juuri ja juuri pelaajan korkeudelle
        vheitettava.CanRotate = false;
        vheitettava.Restitution = 0;
        vheitettava.CollisionIgnoreGroup = 1; ///vihollisten ammukset eivät osu toisiinsa, tai vauvoihin
        Add(vheitettava, 1);
    }


    /// <summary>
    /// ohjainten asetus: liikkuminen, ampuminen, ohjeet, pause
    /// </summary>
    private void AsetaOhjaimet()
    {
        Vector nopeusYlos = new Vector(0, 300); /// pelaajan nopeus ylös
        Vector nopeusAlas = new Vector(0, -300); /// pelaajan nopeus alas

        Keyboard.Listen(Key.W, ButtonState.Down, AsetaNopeus, "Liiku ylös", pelaaja, nopeusYlos); ///pelaaja liikkuu
        Keyboard.Listen(Key.W, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero); ///pelaaja ei liiku, kun näppäintä ei enää paineta
        Keyboard.Listen(Key.S, ButtonState.Down, AsetaNopeus, "Liiku alas", pelaaja, nopeusAlas); ///pelaaja liikkuu
        Keyboard.Listen(Key.S, ButtonState.Released, AsetaNopeus, null, pelaaja, Vector.Zero); ///pelaaja ei liiku, kun näppäintä ei enää paineta

        Keyboard.Listen(Key.P, ButtonState.Pressed, Ammu, "Ammu", pelaaja, "pelaajanammus"); ///pelaaja voi ampua P-näppäimellä
        
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Ohjeet"); ///pelin ohjaimet saa näkyviin
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli"); ///pelin voi lopettaa
    }


    /// <summary>
    /// asetuksia pelaajan liikkumiselle 
    /// </summary>
    /// <param name="pelaaja">Pelaajan alus, joka liikkuu</param>
    /// <param name="nopeus">Vektori liikkumisen nopeudelle</param>
    private void AsetaNopeus(PhysicsObject pelaaja, Vector nopeus)
    {
        if ((nopeus.Y < 0) && (pelaaja.Bottom < Level.Bottom)) ///jos pelaaja saavuttaa kentän alareunan
        {
            pelaaja.Velocity = Vector.Zero; ///pelaajaa estetään liikkumasta kentän ulkopuolelle
            return;
        }
        if ((nopeus.Y > 0) && (pelaaja.Top > Level.Top)) ///vastaavasti jos pelaaja saavuttaa yläreunan
        {
            pelaaja.Velocity = Vector.Zero; ///pelaajan nopeus muuttuu nollaksi
            return;
        } ///pelaaja ei voi liikkua kentän ulkopuolelle
        pelaaja.Velocity = nopeus; ///säätää pelaajan nopeuden
    }


    /// <summary>
    /// luodaan pistelaskuri
    /// </summary>
    /// <param name="x">X-koordinaatti</param>
    /// <param name="y">Y-koordinaatti</param>
    /// <returns></returns>
    private IntMeter LuoPisteLaskuri(double x, double y)
    {
        IntMeter laskuri = new IntMeter(0); ///laskuri alkaa nollasta
        laskuri.MaxValue = 20; ///laskurin enimmäisarvo on 20
        laskuri.UpperLimit += PelinVoitto; ///laskurin saadessa enimmäisarvonsa peli päättyy voittoon
        Label naytto = new Label();
        naytto.BindTo(laskuri);
        naytto.X = -470;
        naytto.Y = 350;
        naytto.TextColor = Color.White;
        naytto.BorderColor = Level.Background.Color;
        naytto.Color = Level.Background.Color;
        
        Add(naytto);
        return laskuri;
    }


    /// <summary>
    /// lisää pistelaskurin peliin
    /// </summary>
    private void LisaaLaskurit()
    {
        Pisteet = LuoPisteLaskuri(Screen.Left + 100.0, Screen.Top - 100.0);
    }


    /// <summary>
    /// automaattisesti resetoituva ajastin vauvojen syntymiselle
    /// </summary>
    private void LisaaAjastimet()
    {
        Timer vavAjastin = new Timer(); ///VavAjastin = vauvojen ajastin
        vavAjastin.Interval = 2.5; ///vauvoja syntyy 2.5 sekunnin aikavälillä
        vavAjastin.Timeout += LuoVauva;
        vavAjastin.Start();
    }


    /// <summary>
    /// luodaan vauvat peliin
    /// </summary>
    private void LuoVauva()
    {
        vauva = new PhysicsObject(30.0, 30.0); ///vauvalle tyyppi ja koko
        vauva.Image = LoadImage("vaavi");
        vauva.X = 600.0; ///vauvat syntyvät hieman kentän ulkopuolelle oikealle puolelle
        vauva.Y = RandomGen.SelectOne( -350, -300, -250, 200, 150, 100, 50, 0, 50, 100, 150, 200, 250, 300, 350); ///vauvat syntyvät satunnaiseen korkeuteen
        vauva.Restitution = 0.0;
        vauva.Tag = "vauva"; ///tagi vauvoille
        vauva.CollisionIgnoreGroup = 1; ///vauvat eivät osu toisiinsa
        AddCollisionHandler(pelaaja, vauva, CollisionHandler.DestroyTarget); ///vauva katoaa kentältä pelaajan alukseen, kun alus ja vauva kohtaavat
        AddCollisionHandler(pelaaja, vauva, CollisionHandler.AddMeterValue(Pisteet, 1)); ///vauvan kerääminen antaa pisteen
        vauva.Destroyed += delegate { LuoVihollinen(Level.Right - 30.0, 0.0); }; ///vauvan kerääminen spawnaa vihollisen
        Add(vauva);

        Vector impulssi = new Vector(-300.0, 0.0); ///vauvojen nopeus ja suunta, kun ne syntyvät peliin: lentävät oikealta vasemmalle kohti pelaajaa
        vauva.Hit(impulssi); ///asetetaan vauvoille edellä mainittu vektori impulssi
    }


    // TODO Funktio ja silmukka: https://tim.jyu.fi/answers/kurssit/tie/ohj1/2019k/demot/demo11?answerNumber=2&task=taulukot&user=keranhos
    // TODO Taulukko, funktio, ja silmukka: https://tim.jyu.fi/answers/kurssit/tie/ohj1/2019k/demot/demo9?answerNumber=6&task=D9T1&user=keranhos
    // TODO Lista ja funktio: https://tim.jyu.fi/answers/kurssit/tie/ohj1/2019k/demot/demo8?answerNumber=3&task=d8teht1&user=keranhos


}
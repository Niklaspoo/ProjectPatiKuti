using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Linq;

namespace ProjectPatiKuti
{
    /// @author gr301861
    /// @version 14.11.2024
    /// <summary>
    /// 
    /// </summary>
    public class ProjectPatiKuti : PhysicsGame
    {
        /// <summary>
        /// Pelaajan ammus.
        /// </summary>
        private Cannon fireball;
        /// <summary>
        /// Pelaajan nopeus.
        /// </summary>
        private Vector pelaajanNopeus = new Vector(0, 0);
        /// <summary>
        /// Pelaajan fysiikkaobjekti.
        /// </summary>
        private PhysicsObject pelaaja;
        /// <summary>
        /// Vihollisen fysiikkaobjekti.
        /// </summary>
        private PhysicsObject vihulaiset;
        /// <summary>
        /// Onko pelaaja kuolematon.
        /// </summary>
        private bool kuolematon;
        /// <summary>
        /// Voiko pelaaja käyttää dash:ia
        /// </summary>
        private bool canDash = true;
        /// <summary>
        /// Pelaajan toinen hahmon kuva.
        /// </summary>
        private Image bobble = LoadImage("bobble");
        /// <summary>
        /// Nykyinen aalto.
        /// </summary>
        private int waves = 1;
        /// <summary>
        /// Kuinka monta vihollista on hengissä.
        /// </summary>
        private int vihujaHengissä;
        /// <summary>
        /// Kuinka monta vihollista on luotu.
        /// </summary>
        private int vihuCount;
        /// <summary>
        /// Maaston kuva.
        /// </summary>
        private Image maasto = LoadImage("Maasto.png");
        /// <summary>
        /// Kompassin kuva.
        /// </summary>
        private Image kompassikuva = LoadImage("kompassi");
        /// <summary>
        /// Vihollisten määrän mittari.
        /// </summary>
        private DoubleMeter vihujaNyt;
        /// <summary>
        /// Pelaajan ammuksen kuvat.
        /// </summary>
        private Image[] fire = LoadImages("plasma1", "plasma2", "plasma3");
        /// <summary>
        /// Ammusanimaation kuvat.
        /// </summary>
        private Image[] bullet = LoadImages("ammus1", "ammus2");
        /// <summary>
        /// Lima-animaation kuvat.
        /// </summary>
        private Image[] lima = LoadImages("lima1", "lima2", "lima3", "lima4", "lima3", "lima2", "lima7");
        /// <summary>
        /// Harvinaisuus.
        /// </summary>
        private string harvinaisuus1;
        /// <summary>
        /// Harvinaisuus.
        /// </summary>
        private string harvinaisuus2;
        /// <summary>
        /// Päivitys.
        /// </summary>
        private string upgrade1;
        /// <summary>
        /// Päivitys.
        /// </summary>
        private string upgrade2;
        /// <summary>
        /// Pelaajan nopeuskerroin.
        /// </summary>
        private double speed = 1;
        /// <summary>
        /// Dash:in viive.
        /// </summary>
        double dashDelay = 1;
        /// <summary>
        /// Pelaajan "elämän" mittari.
        /// </summary>
        DoubleMeter pelaajaHp;
        /// <summary>
        /// Pelaajan lifesteal.
        /// </summary>
        private double LS;
        /// <summary>
        /// Pelaajan dodge todennäköisyys.
        /// </summary>
        private double dodge;
        /// <summary>
        /// Ammuksen koko.
        /// </summary>
        private double bulletSize = 1;
        /// <summary>
        /// I-framejen kesto.
        /// </summary>
        private double invincibilityTime = 0.1;
        /// <summary>
        /// Dash:in kesto.
        /// </summary>
        private double dashTime = 0.2;
        /// <summary>
        /// Pelaajan oikealle animaatio.
        /// </summary>
        private Image[] bobbleO = LoadImages("bobbleO1", "bobbleO2", "bobbleO3", "bobbleO4", "bobbleO5", "bobbleO6");
        /// <summary>
        /// Pelaajan vasemmalle animaatio.
        /// </summary>
        private Image[] bobbleV = LoadImages("bobbleV1", "bobbleV2", "bobbleV3", "bobbleV4", "bobbleV5", "bobbleV6");
        /// <summary>
        /// Animaatiosuunnan apumuuttuja.
        /// </summary>
        private String suunnat = "";
        /// <summary>
        /// Viimeisin näytön leveys.
        /// </summary>
        private double previousScreenWidth = 0;
        /// <summary>
        /// Viimeisin näytön korkeus.
        /// </summary>
        private double previousScreenHeight = 0;
        /// <summary>
        /// Visualisoi i-framet/dash:in ovaali
        /// </summary>
        private GameObject dashOval;




        /// <summary>
        /// Alustaa pelin ja näyttää päävalikon.
        /// </summary>

        public override void Begin()
        {
            if (IsFullScreen==true)
            {
                IsFullScreen = true;
            }

            IsFullScreen = true;
            Level.Size = Screen.Size;
            Level.Background.Image = LoadImage("startscreen");
            Level.Background.FitToLevel();
            MultiSelectWindow aloitus = new MultiSelectWindow("ProjectPK", "Start a new run", "QUIT");
            aloitus.AddItemHandler(0, aloitaPeli);
            aloitus.AddItemHandler(1, Exit);
            Add(aloitus);
        }
        
        
        /// <summary>
        /// Aloittaa pelin luomalla pelaajan, viholliset ja käyttöliittymän elementit.
        /// </summary>
        public void aloitaPeli()
        {
            luoPelaaja();
            LuoElamalaskuri();
            pelaajanNopeus = new Vector(0, 0);
            pelaaja.Velocity = pelaajanNopeus;
            Camera.Follow(pelaaja);
            pelaaja.Position = new Vector(0, 0);
            luoVihu();
            luoVihu();
            canDash = true;
            MediaPlayer.Play("PPK_Sountrack_Vol_1__BGPMLBT_Bullets_Go_Past_Me_Like_Bullet_Trains");
            MediaPlayer.IsRepeating = true;
            Level.Background.Image = maasto;
            Level.Background.TileToLevel();
            kompassi();
            LuoVihuLaskuri();
            Label vihut = new Label("Enemies left:");
            vihut.BorderColor = Color.Black;
            vihut.Color = Color.FromHexCode("#b0affa");
            vihut.Tag = "vihut";
            vihut.X = (Screen.Width / 2) - 150 - 155;
            vihut.Y = Screen.Top - 20 + 5;
            Add(vihut);
            
            
        }
        
        
        /// <summary>
        /// Luo vihollisten määrän laskurin.
        /// </summary>
        void LuoVihuLaskuri()
        {
            vihujaNyt = new DoubleMeter(vihujaHengissä);
            vihujaNyt.MaxValue = vihuCount;
            ProgressBar vihujapalkki = new ProgressBar(150, 20);
            vihujapalkki.Tag = "vihujapalkki";
            vihujapalkki.X = (Screen.Width / 2) - 150;
            vihujapalkki.Y = Screen.Top - 20;
            vihujapalkki.Angle = Angle.StraightAngle;
            vihujapalkki.BarColor = Color.FromHexCode("#700439");
            vihujapalkki.Color = Color.Black;
            vihujapalkki.BindTo(vihujaNyt);
            Add(vihujapalkki);
            
        }
        
        
        /// <summary>
        /// Luo uuden vihollisen pelimaailmaan.
        /// </summary>
        private void luoVihu()
        {
            vihulaiset = new PhysicsObject(80, 57);
            vihulaiset.Shape = Shape.Circle;
            vihulaiset.Color = Color.Red;
            vihulaiset.Animation = new Animation(lima);
            vihulaiset.Animation.FPS = 10;
            vihulaiset.Animation.Start();
            vihulaiset.Tag = "vihu";
            AssaultRifle ase = new AssaultRifle(0, 0);
            Vector syntymäPaikka;
            do
            {
                double x = RandomGen.NextDouble(Level.Left + 100, Level.Right - 100);
                double y = RandomGen.NextDouble(Level.Bottom + 100, Level.Top - 100);
                syntymäPaikka = new Vector(x, y);
            } while (Vector.Distance(syntymäPaikka, pelaaja.Position) < 500);
            vihulaiset.Position = syntymäPaikka;
            ase.X = vihulaiset.X;
            ase.Y = vihulaiset.Y;
            ase.Tag = "weapon";
            vihulaiset.CanRotate = false;
            Add(vihulaiset);
            vihulaiset.Add(ase);
            vihuCount += 1;
            vihujaHengissä += 1;
            Timer.CreateAndStart(1, delegate
            {
                vihuTahtaa();
                PhysicsObject ammus = ase.Shoot();
                ammus.Tag = "ammus";
                ammus.Animation = new Animation(bullet);
                ammus.Animation.Start();
                ammus.Size = new Vector(51, 25);
                ammus.AddCollisionIgnoreGroup(1);
            });

            ase.ProjectileCollision = AmmusOsui;
            FollowerBrain vihunAivot = new FollowerBrain(pelaaja);
            vihunAivot.DistanceFar = 1000;
            vihunAivot.FarBrain = new RandomMoverBrain();
            vihunAivot.Speed = 100;
            vihulaiset.Brain = vihunAivot;
        }

        /// <summary>
        /// Luo pelaajan pelimaailmaan.
        /// </summary>
        public void luoPelaaja()
        {
            pelaaja = new PhysicsObject(64, 85);
            pelaaja.Shape = Shape.Circle;
            pelaaja.Color = Color.Blue;
            pelaaja.Velocity = pelaajanNopeus;
            pelaaja.Position = new Vector(0, 0);
            pelaaja.Image = bobble;
            pelaaja.CanRotate = false;
            fireball = new Cannon(0, 0);
            fireball.IsVisible = false;
            pelaaja.Restitution = 0;
            Add(pelaaja);
            pelaaja.Add(fireball);
            fireball.ProjectileCollision = fireballOsui;
            lisaaOhjaimet();
            luoReunat();
            dashOval = new GameObject(70, 90, Shape.Circle);
            dashOval.Color = Color.White;
            dashOval.Tag = "dashOval";
            dashOval.IsVisible = false;
            dashOval.Image = LoadImage("sproinkle");
            pelaaja.Add(dashOval); 
        }
        
        
        /// <summary>
        /// Luo pelaajan "elämän" mittarin.
        /// </summary>
        private void LuoElamalaskuri()
        {
            pelaajaHp = new DoubleMeter(10);
            pelaajaHp.MaxValue = 10;
            ProgressBar elamapalkki = new ProgressBar(150, 20);
            elamapalkki.Tag = "elamapalkki";
            elamapalkki.X = Screen.Left + 150;
            elamapalkki.Y = Screen.Top - 20;
            elamapalkki.Angle = Angle.StraightAngle;
            elamapalkki.Color = Color.Black;
            elamapalkki.BarColor = Color.FromHexCode("#1a8f39");
            elamapalkki.BindTo(pelaajaHp);
            Add(elamapalkki);
        }
        
        
        /// <summary>
        /// Luo pelimaailman reunat.
        /// </summary>
        private void luoReunat()
        {

            Level.Size = new Vector(10000, 10000);
            Level.CreateBorders(0, true);

        }
        
        
        /// <summary>
        /// Lisää nappuloille toiminnot.
        /// </summary>
        private void lisaaOhjaimet()
        {
            Keyboard.Listen(Key.W, ButtonState.Pressed, liiku, "move", new Vector(0, 500 * speed));
            Keyboard.Listen(Key.W, ButtonState.Released, liiku, "move", new Vector(0, -500 * speed));
            Keyboard.Listen(Key.D, ButtonState.Pressed, liiku, "move", new Vector(500 * speed, 0));
            Keyboard.Listen(Key.D, ButtonState.Released, liiku, "move", new Vector(-500 * speed, 0));
            Keyboard.Listen(Key.S, ButtonState.Pressed, liiku, "move", new Vector(0, -500 * speed));
            Keyboard.Listen(Key.S, ButtonState.Released, liiku, "move", new Vector(0, 500 * speed));
            Keyboard.Listen(Key.A, ButtonState.Pressed, liiku, "move", new Vector(-500 * speed, 0));
            Keyboard.Listen(Key.A, ButtonState.Released, liiku, "move", new Vector(500 * speed, 0));
            Keyboard.Listen(Key.F11, ButtonState.Released, fullScreen,"fs");
            Keyboard.Listen(Key.Q, ButtonState.Down, delegate { if (Camera.ZoomFactor > 0.5) { Camera.ZoomFactor -= 0.01; } }, "Zoom out");
            Keyboard.Listen(Key.E, ButtonState.Down, delegate { if (Camera.ZoomFactor < 1) { Camera.ZoomFactor += 0.01; } }, "Zoom in");
            Mouse.Listen(MouseButton.Left, ButtonState.Down, ammu, "fire", 0);
            Keyboard.Listen(Key.M, ButtonState.Released, delegate
            {
                if (MediaPlayer.Volume > 0.0)
                {
                    MediaPlayer.Volume = 0.0;
                }
                else
                {
                    MediaPlayer.Volume = 1.0;
                }
                
            }, "Pause music");
            Mouse.ListenMovement(0.1, tahtaa, "aim");
            Keyboard.Listen(Key.Space, ButtonState.Pressed, dash, "dash");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, menu, "menu");
        }
        /// <summary>
        /// Tarkistaa onko peli koko näytön tilassa.
        /// </summary>
        private void fullScreen()
        {
            if (IsFullScreen)
            {
                IsFullScreen = false;
            }
            else
            {
                IsFullScreen = true;
            }
            skaalaaUi();
        }
        
        /// <summary>
        /// Skaalaa käyttöliittymä elementtien koot peli-ikkunan kokoon nähden sopiviksi. 
        /// </summary>
        private void skaalaaUi()
        {
            var elamapalkki = GetObjectsWithTag("elamapalkki")
                .Cast<ProgressBar>().FirstOrDefault();
            if (elamapalkki != null)
            {
                elamapalkki.X = Screen.Left + 150;
                elamapalkki.Y = Screen.Top - 20;
            }
            
            var vihujapalkki = GetObjectsWithTag("vihujapalkki")
                .Cast<ProgressBar>().FirstOrDefault();
            if (vihujapalkki != null)
            {
                vihujapalkki.X = (Screen.Width / 2) - 150;
                vihujapalkki.Y = Screen.Top - 20;
            }
            
            var kompassi = GetObjectsWithTag("kompassi")
                .Cast<Widget>().FirstOrDefault();
            if (kompassi != null)
            {
                kompassi.Position = new Vector(Screen.Left + 100, Screen.Top - 100);
            }
            
            var vihutLabel = GetObjectsWithTag("vihut")
                .Cast<Label>().FirstOrDefault();
            if (vihutLabel != null)
            {
                vihutLabel.X = (Screen.Width / 2) - 150 - 155;
                vihutLabel.Y = Screen.Top - 20 + 3;
            }
        }
        
        
        /// <summary>
        /// Luo pelaajan liikkeen animaation.
        /// </summary>
        private void anim()
        {
            var vel = pelaaja.Velocity;
            if (vel.Magnitude <= 0) return;

           
            if (vel.X > 0 && suunnat != "oikea")
            {
                pelaaja.Animation = new Animation(bobbleO) { FPS = 10 };
                suunnat = "oikea";
            }
            else if (vel.X < 0 && suunnat != "vasen")
            {
                pelaaja.Animation = new Animation(bobbleV) { FPS = 10 };
                suunnat = "vasen";
            }

            PlayPlayerAnimation();
        }
        
        
        /// <summary>
        /// Näyttää pelin valikon. Luo myös valikon toiminnallisuuden.
        /// </summary>
        public void menu()
        {
            ClearControls();
            IsPaused = true;
            pelaaja.Velocity = new Vector(0, 0);
            pelaajanNopeus = new Vector(0, 0);
            MultiSelectWindow menu = new MultiSelectWindow("Menu", "Resume", "Keybinds", "Quit");
            menu.AddItemHandler(0, delegate { IsPaused = false; lisaaOhjaimet(); });
            menu.AddItemHandler(1, delegate {
                MultiSelectWindow keybindsWindow = new MultiSelectWindow(
                    "Keybinds",
                    "WASD: move",
                    "LMB: shoot",
                    "SPACE: dash",
                    "Q & E: zoom",
                    "M: Mute music",
                    "OK"
                );
                keybindsWindow.AddItemHandler(0, () => { IsPaused = false; lisaaOhjaimet(); });
                keybindsWindow.AddItemHandler(1, () => { IsPaused = false; lisaaOhjaimet(); });
                keybindsWindow.AddItemHandler(2, () => { IsPaused = false; lisaaOhjaimet(); });
                keybindsWindow.AddItemHandler(3, () => { IsPaused = false; lisaaOhjaimet(); });
                keybindsWindow.AddItemHandler(4, () => { IsPaused = false; lisaaOhjaimet(); });
                keybindsWindow.AddItemHandler(5, () => { IsPaused = false; lisaaOhjaimet(); });
                Add(keybindsWindow);
            });
            menu.AddItemHandler(2, Exit);
            Add(menu);
        }

        /// <summary>
        /// Tarkistaa tulipallon osumisen kohteeseen.
        /// </summary>
        /// <param name="projectile">Tulipallo-objekti.</param>
        /// <param name="kohde">Kohde, johon tulipallo osui.</param>
        void fireballOsui(PhysicsObject projectile, PhysicsObject kohde)
        {
            projectile.Destroy();
            if (kohde.Tag.ToString() == "vihu")
            {
                kohde.Destroy();
                vihujaHengissä -= 1;
                vihujaNyt.Value = vihujaHengissä;
                if (LS > 0)
                {
                    pelaajaHp.Value += LS;
                    if (pelaajaHp.Value > pelaajaHp.MaxValue)
                    {
                        pelaajaHp.Value = pelaajaHp.MaxValue;
                    }
                }
                if (vihujaHengissä == 0)
                {
                    vihuCount = 0;
                    waves += 1;
                    uusiWave(waves);

                }
            }
        }
        
        
        /// <summary>
        /// Luo uuden aallon ja antaa pelaajalle päivitysvaihtoehdot.
        /// </summary>
        /// <param name="wave">Nykyinen aalto.</param>
        private void uusiWave(int wave)
        {
            
            ClearControls();
            IsPaused = true;
            pelaaja.Velocity = new Vector(0, 0);
            pelaajanNopeus = new Vector(0, 0);
            canDash = true;
            kuolematon = false;
            pelaajaHp.Value = pelaajaHp.MaxValue;
            harvinaisuus1 = Rarity();
            harvinaisuus2 = Rarity();
            upgrade1 = upgrade(harvinaisuus1);
            while (upgrade1 != upgrade(harvinaisuus2))
            {
                upgrade2 = upgrade(harvinaisuus2);
            }
            MultiSelectWindow uusWave = new MultiSelectWindow("Wave " + (wave - 1) + " complete. Choose your upgrade!", harvinaisuus1+": "+upgrade1, harvinaisuus2 + ": " + upgrade2);
            PushButton[] nappulat = uusWave.Buttons;
            if (harvinaisuus1 == "Legendary")
            { nappulat[0].Color = Color.Gold; }
            if (harvinaisuus2 == "Legendary")
            { nappulat[1].Color = Color.Gold; }
            if (harvinaisuus1 == "Epic")
            { nappulat[0].Color = Color.Purple; }
            if (harvinaisuus2 == "Epic")
            { nappulat[1].Color = Color.Purple; }
            if (harvinaisuus1 == "Rare")
            { nappulat[0].Color = Color.Blue; }
            if (harvinaisuus2 == "Rare")
            { nappulat[1].Color = Color.Blue; }
            if (harvinaisuus1 == "Uncommon")
            { nappulat[0].Color = Color.Green; }
            if (harvinaisuus2 == "Uncommon")
            { nappulat[1].Color = Color.Green; }
            if (harvinaisuus1 == "Common")
            { nappulat[0].Color = Color.Gray; }
            if (harvinaisuus2 == "Common")
            { nappulat[1].Color = Color.Gray; }

            uusWave.DefaultCancel = -1;
            uusWave.AddItemHandler(0, delegate { ApplyUpgrade(upgrade1, harvinaisuus1); pelaaja.Velocity = new Vector(0, 0);
                pelaajanNopeus = new Vector(0, 0); IsPaused = false; lisaaOhjaimet(); });
            uusWave.AddItemHandler(1, delegate { ApplyUpgrade(upgrade2, harvinaisuus2); pelaaja.Velocity = new Vector(0, 0);
                pelaajanNopeus = new Vector(0, 0); IsPaused = false; lisaaOhjaimet(); });
            Add(uusWave);
            while (vihuCount < wave + 2)
            {
                luoVihu();
            }
            if (vihuCount == wave + 2) { LuoVihuLaskuri(); }
            foreach (var ammus in GetObjectsWithTag("ammus"))
            {
                ammus.Destroy();
            }

        }
        
        
        /// <summary>
        /// Antaa pelaajalle päivityksen, joka sopii harvinaisuuteen.
        /// </summary>
        /// <param name="upgrade">Päivityksen nimi.</param>
        /// <param name="rarity">Päivityksen harvinaisuus.</param>
        private void ApplyUpgrade(string upgrade, string rarity)
        {
            if (upgrade == "Speed")
            {
                if (rarity == "Common")
                {
                    speed += 0.1;
                }
                if (rarity == "Uncommon")
                {
                    speed += 0.15;
                }
                if (rarity == "Rare")
                {
                    speed += 0.3;
                }

            }
            else if (upgrade == "Firerate")
            {
                if (rarity == "Common")
                {
                    fireball.FireRate += 0.1;
                }
                if (rarity == "Uncommon")
                {
                    fireball.FireRate += 0.15;
                }
                if (rarity == "Rare")
                {
                    fireball.FireRate += 0.3;
                }
            }
            else if (upgrade == "Dash delay")
            {
                if (rarity == "Common")
                {
                    dashDelay -= 0.05;
                }
                if (rarity == "Uncommon")
                {
                    dashDelay -= 0.1;
                }
                if (rarity == "Rare")
                {
                    dashDelay -= 0.15;
                }
            }
            else if (upgrade == "Health")
            {
                if (rarity == "Common")
                {
                    pelaajaHp.MaxValue += 1;
                    pelaajaHp.Value += 1;
                }
                if (rarity == "Uncommon")
                {
                    pelaajaHp.MaxValue += 2;
                    pelaajaHp.Value += 2;
                }
                if (rarity == "Rare")
                {
                    pelaajaHp.MaxValue += 3;
                    pelaajaHp.Value += 3;
                }

            }
            else if (upgrade == "Bullet speed")
            {
                if (rarity == "common")
                {
                    fireball.Power.Value += 100;
                }
                if (rarity == "Uncommon")
                {
                    fireball.Power.Value += 200;
                }
                if (rarity == "Rare")
                {
                    fireball.Power.Value += 300;
                }
            }
            else if (upgrade == "Lifesteal")
            {
                if (rarity == "Epic")
                {
                    LS += 0.5;
                }
                if (rarity == "Legendary")
                {
                    LS += 0.8;
                }
            }
            else if (upgrade == "Dodge")
            {
                if (rarity == "Epic")
                {
                    dodge += 5;
                }
                if (rarity == "Legendary")
                {
                    dodge += 10;
                }
            }
            else if (upgrade == "Bullet size")
            {
                if (rarity == "Epic")
                {
                    bulletSize += 0.1;
                }
                if (rarity == "Legendary")
                {
                    bulletSize += 0.2;
                }
            }
            else if (upgrade == "Invincibility time")
            {
                if (rarity == "Epic")
                {
                    invincibilityTime += 0.1;
                }
                if (rarity == "Legendary")
                {
                    invincibilityTime += 0.2;
                }
            }
            else if (upgrade == "Dash time")
            {
                if (rarity == "Epic")
                {
                    dashTime += 0.05;
                }
                if (rarity == "Legendary")
                {
                    dashTime += 0.1;
                }
            }



        }
        
        
        /// <summary>
        /// Tarkistaa ammuksen osumisen kohteeseen.
        /// </summary>
        /// <param name="ammus">Ammus-objekti.</param>
        /// <param name="kohde">Kohde, johon ammus osui.</param>
        void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
        {
            ammus.Restitution = 0;
            ammus.Destroy();

            if (kohde == pelaaja)
            {
                if (kuolematon)
                {
                    return;
                }
                if (dodge > 0)
                {
                    if (RandomGen.NextInt(1, 101) >= dodge)
                    {
                        MessageDisplay.Add("Dodged!");
                        return;
                    }
                }
                pelaajaHp.Value -= 1+(waves*0.6);
                kuolematon = true;
                Timer.SingleShot(invincibilityTime, () => kuolematon = false);
                if (pelaajaHp.Value <= 0)
                {
                    pelaaja.Destroy();
                    ResetGameState();
                    ClearAll();
                    ClearAll();
                    ClearAll();
                    Begin();
                }
            }
        }
        
        
        /// <summary>
        /// Nollaa pelin tilan.
        /// </summary>
        private void ResetGameState()
        {
            waves = 1;
            vihujaHengissä = 0;
            vihuCount = 0;
        }
        
        
        /// <summary>
        /// Käsittelee pelaajan tähtäämisen.
        /// </summary>
        private void tahtaa()
        {
            Vector suunta = (Mouse.PositionOnWorld - fireball.AbsolutePosition).Normalize();
            fireball.Angle = suunta.Angle;
        }
        
        
        /// <summary>
        /// K�sittelee pelaajan ampumisen.
        /// </summary>
        /// <param name="suunta">Ampumisen suunta.</param>
        private void ammu(int suunta)
        {
            PhysicsObject kuti = fireball.Shoot();
            if (kuti != null)
            {
                kuti.AddCollisionIgnoreGroup(1);
                kuti.Size *= 4;
                kuti.Size *= bulletSize;
                kuti.Animation = new Animation(fire);
                kuti.Animation.FPS = 3;
                kuti.Animation.Start();
                kuti.Tag = "kuti";
               
                //ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
            }
        }
        
        
        /// <summary>
        /// Käsittelee vihollisen tähtäämisen pelaajaan.
        /// </summary>
        private void vihuTahtaa()
        {
            foreach (var vihu in GetObjectsWithTag("vihu"))
            {
                AssaultRifle ase = vihu.GetChildObjects<AssaultRifle>().FirstOrDefault();
                if (ase != null)
                {
                    Vector suunta = (pelaaja.Position - vihu.Position).Normalize();
                    ase.Angle = suunta.Angle;
                }
            }
        }

        /// <summary>
        /// Aloittaa pelaajan animaation, jos se ei ole jo käynnissä.
        /// </summary>
        private void PlayPlayerAnimation()
        {
            if (pelaaja.Animation != null && !pelaaja.Animation.IsPlaying)
            {
                pelaaja.Animation.Start();
            }
        }
        
        
        /// <summary>
        /// Käsittelee pelaajan dash:in.
        /// </summary>
        /// 
        public void dash()
        {
            if (!canDash || IsPaused || pelaaja == null || pelaaja.IsDestroyed) return;
            canDash = false;
            pelaaja.Push(pelaajanNopeus * 200);
            kuolematon = true;
            dashOval.IsVisible = true; // Show oval

            Timer.SingleShot(dashTime, () => {
                if (!IsPaused && pelaaja != null && !pelaaja.IsDestroyed)
                    pelaaja.Velocity = pelaajanNopeus;
                dashOval.IsVisible = false; // Hide oval after dash
            });
            Timer.SingleShot(dashTime, () => {
                if (!IsPaused) kuolematon = false;
            });
            if (dashDelay < 0.05) { dashDelay = 0.05; }
            Timer.SingleShot(dashDelay, () => {
                if (!IsPaused) canDash = true;
            });
        }
        
        
        /// <summary>
        /// Käsittelee pelaajan liikkumisen.
        /// </summary>
        /// <param name="nopeus">Liikkumisnopeus.</param>
        private void liiku(Vector nopeus)
        {
            if ((Math.Abs(pelaajanNopeus.X) == 250 * speed) && (Math.Abs(pelaajanNopeus.Y) == 250 * speed))
            {
                pelaajanNopeus = pelaajanNopeus * 2;
            }
            pelaajanNopeus += nopeus;
            if ((Math.Abs(pelaajanNopeus.X) == 500 * speed) && (Math.Abs(pelaajanNopeus.Y) == 500 * speed))
            {
                pelaajanNopeus = pelaajanNopeus / 2;
            }
            pelaaja.Velocity = pelaajanNopeus;
            
        }
        
        
        /// <summary>
        /// Luo kompassin, joka näyttää lähimmän vihollisen suunnan.
        /// </summary>
        private void kompassi()
        {
            
            Widget kompassi = new Widget(100, 100);
            kompassi.Tag = "kompassi";
            kompassi.Image = kompassikuva;
            Add(kompassi);
            kompassi.Position = new Vector(Screen.Left + 100, Screen.Top - 100);

            Timer.SingleShot(0.1, () => päivitäKompassi(kompassi));
        }
        
        
        /// <summary>
        /// Päivittää kompassin osoittamaan lähimmän vihollisen suuntaan.
        /// </summary>
        /// <param name="kompassi">Kompassi-widget.</param>
        private void päivitäKompassi(Widget kompassi)
        {
            
            anim();
            PlayPlayerAnimation();
            if (pelaaja.Velocity.Magnitude == 0)
            {
                pelaaja.Animation.Stop();
            }
            GameObject lähinVihu = löydäLähinVihu();
            if (lähinVihu != null)
            {
                Vector suunta = (lähinVihu.Position - pelaaja.Position).Normalize();
                kompassi.Angle = suunta.Angle;
            }
            Timer.SingleShot(0.1, () => päivitäKompassi(kompassi));
            if (Screen.Width != previousScreenWidth || Screen.Height != previousScreenHeight)
            {
                skaalaaUi();
                previousScreenWidth = Screen.Width;
                previousScreenHeight = Screen.Height;
            }

        }
        
        
        /// <summary>
        /// Etsii lähimmän vihollisen pelaajasta.
        /// </summary>
        /// <returns>Lähin vihollinen.</returns>
        private GameObject löydäLähinVihu()
        {
            GameObject lähinVihu = null;
            double lähinEtäisyys = double.MaxValue;
            foreach (var vihu in GetObjectsWithTag("vihu"))
            {
                double etäisyys = (vihu.Position - pelaaja.Position).Magnitude;
                if (etäisyys < lähinEtäisyys)
                {
                    lähinEtäisyys = etäisyys;
                    lähinVihu = vihu;
                }
            }
            return lähinVihu;
        }
        
        
        /// <summary>
        /// Arpoo harvinaisuuden.
        /// </summary>
        /// <returns>Harvinaisuus merkkijonona.</returns>
        private static string Rarity()
        {
            switch
                (RandomGen.NextInt(1, 101))
            {
                case var n when (n >= 1 && n <= 5):
                    return "Legendary";
                case var n when (n >= 6 && n <= 15):
                    return "Epic";
                case var n when (n >= 16 && n <= 35):
                    return "Rare";
                case var n when (n >= 36 && n <= 75):
                    return "Uncommon";
                default:
                    return "Common";

            }
        }
        
        
        /// <summary>
        /// Valitsee päivityksen harvinaisuuden perusteella.
        /// </summary>
        /// <param name="rarity">Harvinaisuus.</param>
        /// <returns>P�ivityksen nimi.</returns>
        private string upgrade(string rarity)
        {
            int d = 6;
            int dd = 6;
            if (dashDelay <= 0.05) { dd = 5; }
            if (dodge > 60) { d = 5; }
            if (rarity == "Common")
            {
                int i = RandomGen.NextInt(1, dd);
                if (i == 1) { return "Speed"; }
                if (i == 2) { return "Bullet speed"; }
                if (i == 3) { return "Firerate"; }
                if (i == 4) { return "Health"; }
                if (i == 5) { return "Dash delay"; }
            }
            else if (rarity == "Uncommon")
            {
                int i = RandomGen.NextInt(1, dd);
                if (i == 1) { return "Speed"; }
                if (i == 2) { return "Bullet speed"; }
                if (i == 3) { return "Firerate"; }
                if (i == 4) { return "Health"; }
                if (i == 5) { return "Dash delay"; }
            }
            else if (rarity == "Rare")
            {
                int i = RandomGen.NextInt(1, dd);
                if (i == 1) { return "Speed"; }
                if (i == 2) { return "Bullet speed"; }
                if (i == 3) { return "Firerate"; }
                if (i == 4) { return "Health"; }
                if (i == 5) { return "Dash delay"; }
            }
            else if (rarity == "Epic")
            {
                int i = RandomGen.NextInt(1, d);
                if (i == 1) { return "Lifesteal"; }
                if (i == 2) { return "Dash time"; }
                if (i == 3) { return "Bullet size"; }
                if (i == 4) { return "Invincibility time"; }
                if (i == 5) { return "Dodge"; }
            }
            else if (rarity == "Legendary")
            {
                int i = RandomGen.NextInt(1, d);
                if (i == 1) { return "Lifesteal"; }
                if (i == 2) { return "Dash time"; }
                if (i == 3) { return "Bullet size"; }
                if (i == 4) { return "Invincibility time"; }
                if (i == 5) { return "Dodge"; }
            }
            return "Speed";
        }
        
        
    }
}
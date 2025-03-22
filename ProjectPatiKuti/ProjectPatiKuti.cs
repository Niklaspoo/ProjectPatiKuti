using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
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
        private AssaultRifle ase;
        private Cannon fireball;
        private Vector pelaajanNopeus = new Vector(0, 0);
        private PhysicsObject pelaaja;
        private PhysicsObject vihu;
        private bool kuolematon = false;
        private bool canDash = true;
        private Image blazoid = LoadImage("blazoid");
        private Image bobble = LoadImage("bobble");
        private int wave = 1;
        private int vihuHp = 0;
        private int vihujaHengissä = 0;
        private int vihuCount = 0;
        private Image maasto = LoadImage("Maasto.png");
        private Image kompassikuva = LoadImage("kompassi");
        private DoubleMeter vihujaNyt;
        private Image[] fire = LoadImages("plasma1", "plasma2", "plasma3");
        private Image[] bullet = LoadImages("ammus1", "ammus2");
        private Image[] lima = LoadImages("lima1", "lima2", "lima3", "lima4", "lima3", "lima2", "lima7");
        private string harvinaisuus1;
        private string harvinaisuus2;
        private string upgrade1;
        private string upgrade2;
        private double speed = 1;



        public override void Begin()
        {
            IsFullScreen = true;
            MultiSelectWindow aloitus = new MultiSelectWindow("ProjectPK", "Start a new run", "QUIT");
            aloitus.AddItemHandler(0, aloitaPeli);
            aloitus.AddItemHandler(1, Exit);
            Add(aloitus);
        }
        public void aloitaPeli()
        {

            luoPelaaja();
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
            vihut.X = (Screen.Width / 2) - 150 - 155;
            vihut.Y = Screen.Top - 20+3;
            Add(vihut);
        }
        void LuoVihuLaskuri()
        {
            vihujaNyt = new DoubleMeter(vihujaHengissä);
            vihujaNyt.MaxValue = vihuCount;
            ProgressBar vihujapalkki = new ProgressBar(150, 20);
            vihujapalkki.X = (Screen.Width / 2) - 150;
            vihujapalkki.Y = Screen.Top - 20;
            vihujapalkki.Angle = Angle.StraightAngle;
            vihujapalkki.BorderColor = Color.Black;
            vihujapalkki.BindTo(vihujaNyt);
            Add(vihujapalkki);
            
        }
        private void luoVihu()
        {
            vihuHp = wave * 1;
            vihu = new PhysicsObject(80, 57);
            vihu.Shape = Shape.Circle;
            vihu.Color = Color.Red;
            vihu.Animation = new Animation(lima);
            vihu.Animation.FPS = 10;
            vihu.Animation.Start();
            vihu.Tag = "vihu";
            AssaultRifle ase = new AssaultRifle(0, 0);
            Vector syntymäPaikka;
            do
            {
                double x = RandomGen.NextDouble(Level.Left + 100, Level.Right - 100);
                double y = RandomGen.NextDouble(Level.Bottom + 100, Level.Top - 100);
                syntymäPaikka = new Vector(x, y);
            } while (Vector.Distance(syntymäPaikka, pelaaja.Position) < 500);
            vihu.Position = syntymäPaikka;
            ase.X = vihu.X;
            ase.Y = vihu.Y;
            ase.Tag = "weapon";
            vihu.CanRotate = false;
            Add(vihu);
            vihu.Add(ase);
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
            vihu.Brain = vihunAivot;
        }


        public void luoPelaaja()
        {
            pelaaja = new PhysicsObject(64, 85);
            pelaaja.Shape = Shape.Circle;
            pelaaja.Color = Color.Blue;
            pelaaja.Velocity = pelaajanNopeus;
            pelaaja.Position = new Vector(0, 0);
            pelaaja.Image = bobble;
            //pelaaja.Image = bobble;
            pelaaja.CanRotate = false;
            fireball = new Cannon(0, 0);

            Add(pelaaja);
            pelaaja.Add(fireball);
            fireball.ProjectileCollision = fireballOsui;

            lisaaOhjaimet();
            luoReunat();

        }
        private void luoReunat()
        {

            Level.Size = new Vector(10000, 10000);
            Level.CreateBorders(0, true);

        }
        private void lisaaOhjaimet()
        {
            Keyboard.Listen(Jypeli.Key.W, ButtonState.Pressed, liiku, "move", new Vector(000, 500*speed));
            Keyboard.Listen(Jypeli.Key.W, ButtonState.Released, liiku, "move", new Vector(0, -500*speed));
            Keyboard.Listen(Jypeli.Key.D, ButtonState.Pressed, liiku, "move", new Vector(500*speed, 0));
            Keyboard.Listen(Jypeli.Key.D, ButtonState.Released, liiku, "move", new Vector(-500*speed, 0));
            Keyboard.Listen(Jypeli.Key.S, ButtonState.Pressed, liiku, "move", new Vector(0, -500*speed));
            Keyboard.Listen(Jypeli.Key.S, ButtonState.Released, liiku, "move", new Vector(0, 500*speed));
            Keyboard.Listen(Jypeli.Key.A, ButtonState.Pressed, liiku, "move", new Vector(-500*speed, 0));
            Keyboard.Listen(Jypeli.Key.A, ButtonState.Released, liiku, "move", new Vector(500*speed, 0));
            Keyboard.Listen(Jypeli.Key.Q, ButtonState.Down, delegate{ if (Camera.ZoomFactor > 0.5) { Camera.ZoomFactor -= 0.01; } }, "Zoom out");
            Keyboard.Listen(Jypeli.Key.E, ButtonState.Down, delegate { if (Camera.ZoomFactor < 1) { Camera.ZoomFactor += 0.01; } }, "Zoom in");
            Mouse.Listen(Jypeli.MouseButton.Left, ButtonState.Down, ammu, "fire", 0);
            Mouse.ListenMovement(0.1, tahtaa, "aim");
            Keyboard.Listen(Jypeli.Key.Space, ButtonState.Pressed, dash, "dash");
            Keyboard.Listen(Jypeli.Key.Escape, ButtonState.Pressed, menu, "menu");
            //Keyboard.Listen(Key.Enter, ButtonState.Pressed, () => uusiWave(wave), "");
        }

        public void menu()
        {
            ClearControls();
            IsPaused = true;
            pelaaja.Velocity = new Vector(0, 0);
            pelaajanNopeus = new Vector(0, 0);
            MultiSelectWindow menu = new MultiSelectWindow("Menu", "Resume", "Quit");
            menu.AddItemHandler(0, delegate { IsPaused = false; lisaaOhjaimet(); });
            menu.AddItemHandler(1, Exit);
            Add(menu);
        }

        void fireballOsui(PhysicsObject fireball, PhysicsObject kohde)
        {
            fireball.Destroy();
            if (kohde.Tag.ToString() == "vihu")
            {
                kohde.Destroy();
                vihujaHengissä -= 1;
                vihujaNyt.Value = vihujaHengissä;
                if (vihujaHengissä == 0)
                {
                    vihuCount = 0;
                    wave += 1;
                    uusiWave(wave);

                }
            }
        }
        private void uusiWave(int wave)
        {

            ClearControls();
            IsPaused = true;
            pelaaja.Velocity = new Vector(0, 0);
            pelaajanNopeus = new Vector(0, 0);
            harvinaisuus1 = Rarity();
            harvinaisuus2 = Rarity();
            upgrade1 = upgrade(harvinaisuus1);
            upgrade2 = upgrade(harvinaisuus2);
            MultiSelectWindow uusWave = new MultiSelectWindow("Wave " + (wave - 1) + " complete. Choose your upgrade!", harvinaisuus1+": "+upgrade1, harvinaisuus2 + ": " + upgrade2);
            PushButton[] nappulat = uusWave.Buttons;
            if (harvinaisuus1 == "Legendary")
            {
                nappulat[0].Color = Color.Gold;
            }
            if (harvinaisuus2 == "Legendary")
            {
                nappulat[1].Color = Color.Gold;
            }
            if (harvinaisuus1 == "Epic")
            {
                nappulat[0].Color = Color.Purple;
            }
            if (harvinaisuus2 == "Epic")
            {
                nappulat[1].Color = Color.Purple;
            }
            if (harvinaisuus1 == "Rare")
            {
                nappulat[0].Color = Color.Blue;
            }
            if (harvinaisuus2 == "Rare")
            {
                nappulat[1].Color = Color.Blue;
            }
            if (harvinaisuus1 == "Uncommon")
            {
                nappulat[0].Color = Color.Green;
            }
            if (harvinaisuus2 == "Uncommon")
            {
                nappulat[1].Color = Color.Green;
            }
            if (harvinaisuus1 == "Common")
            {
                nappulat[0].Color = Color.Gray;
            }
            if (harvinaisuus2 == "Common")
            {
                nappulat[1].Color = Color.Gray;
            }

            uusWave.DefaultCancel = -1;
            uusWave.AddItemHandler(0, delegate { ApplyUpgrade(upgrade1, harvinaisuus1); IsPaused = false; lisaaOhjaimet(); });
            uusWave.AddItemHandler(1, delegate { ApplyUpgrade(upgrade2, harvinaisuus2); IsPaused = false; lisaaOhjaimet(); });
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

        }
        void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
        {
            ammus.Destroy();

            if (kohde == pelaaja)
            {
                if (kuolematon)
                {
                    return;
                }
                pelaaja.Destroy();
                ResetGameState();
                ClearAll();
                ClearAll();
                ClearAll();
                Begin();

            }
        }
        private void ResetGameState()
        {
            wave = 1;
            vihuHp = 0;
            vihujaHengissä = 0;
            vihuCount = 0;
        }
        private void tahtaa()
        {
            Vector suunta = (Mouse.PositionOnWorld - fireball.AbsolutePosition).Normalize();
            fireball.Angle = suunta.Angle;
        }
        private void ammu(int suunta)
        {
            PhysicsObject kuti = fireball.Shoot();
            if (kuti != null)
            {
                kuti.AddCollisionIgnoreGroup(1);
                kuti.Size *= 4;
                kuti.Animation = new Animation(fire);
                kuti.Animation.FPS = 3;
                kuti.Animation.Start();
                kuti.Tag = "kuti";
               
                //ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
            }
        }
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

        public void dash()
        {
            if (!canDash) return;
            canDash = false;
            pelaaja.Push(pelaajanNopeus * 200);
            kuolematon = true;
            Timer.SingleShot(0.2, () => pelaaja.Velocity = pelaajanNopeus);
            Timer.SingleShot(0.2, () => kuolematon = false);
            Timer.SingleShot(1, () => canDash = true);
        }
        private void liiku(Vector nopeus)
        {
            if ((Math.Abs(pelaajanNopeus.X) == 250*speed) && (Math.Abs(pelaajanNopeus.Y) == 250*speed))
            {
                pelaajanNopeus = pelaajanNopeus * 2;
            }
            pelaajanNopeus += nopeus;
            if ((Math.Abs(pelaajanNopeus.X) == 500*speed) && (Math.Abs(pelaajanNopeus.Y) == 500*speed))
            {
                pelaajanNopeus = pelaajanNopeus / 2;
            }
            pelaaja.Velocity = pelaajanNopeus;
        }
        private void kompassi()
        {
            Widget kompassi = new Widget(100, 100);
            kompassi.Image = kompassikuva;
            Add(kompassi);
            kompassi.Position = new Vector(Screen.Left + 100, Screen.Top - 100);

            Timer.SingleShot(0.1, () => päivitäKompassi(kompassi));
        }
        private void päivitäKompassi(Widget kompassi)
        {
            GameObject lähinVihu = löydäLähinVihu();
            if (lähinVihu != null)
            {
                Vector suunta = (lähinVihu.Position - pelaaja.Position).Normalize();
                kompassi.Angle = suunta.Angle;
            }
            Timer.SingleShot(0.1, () => päivitäKompassi(kompassi));

        }
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
        private static string Rarity()
        {
            switch
                (RandomGen.NextInt(1, 101))
            {
                case int n when (n >= 1 && n <= 5):
                    return "Legendary";
                case int n when (n >= 6 && n <= 15):
                    return "Epic";
                case int n when (n >= 16 && n <= 35):
                    return "Rare";
                case int n when (n >= 36 && n <= 75):
                    return "Uncommon";
                default:
                    return "Common";

            }
        }
        private string upgrade(string rarity)
        {
            int i = RandomGen.NextInt(1, 6);
            if (rarity == "Common")
            {
                if (i == 1) { return "Speed"; }
                if (i == 2) { return "Damage"; }
                if (i == 3) { return "Firerate"; }
                if (i == 4) { return "Health"; }
                if (i == 5) {return "Dash delay"; }
            }
            else if (rarity == "Uncommon")
            {
                if (i == 1) { return "Speed"; }
                if (i == 2) { return "Damage"; }
                if (i == 3) { return "Firerate"; }
                if (i == 4) { return "Health"; }
                if (i == 5) { return "Dash delay"; }
            }
            else if (rarity == "Rare")
            {
                if (i == 1) { return "Speed"; }
                if (i == 2) { return "Damage"; }
                if (i == 3) { return "Firerate"; }
                if (i == 4) { return "Health"; }
                if (i == 5) { return "Dash delay"; }
            }
            else if (rarity == "Epic")
            {
                if (i == 1) { return "Lifesteal"; }
                if (i == 2) { return "Dodge"; }
                if (i == 3) { return "Bullet size"; }
                if (i == 4) { return "Invincibility time"; }
                if (i == 5) { return "Dash time"; }
            }
            else if (rarity == "Legendary")
            {
                if (i == 1) { return "Lifesteal"; }
                if (i == 2) { return "Dodge"; }
                if (i == 3) { return "Bullet size"; }
                if (i == 4) { return "Invincibility time"; }
                if (i == 5) { return "Dash time"; }
            }
            return "Speed"; // Default return value
        }
    }
}
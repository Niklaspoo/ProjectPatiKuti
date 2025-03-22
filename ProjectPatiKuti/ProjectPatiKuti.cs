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
        private int vihujaHengiss‰ = 0;
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
        double dashDelay = 1;
        DoubleMeter pelaajaHp;
        private double LS = 0;
        private double dodge = 0;
        private double bulletSize = 1;
        private double invincibilityTime = 0.1;
        private double dashTime = 0.2;
        private Image[] bobbleO = LoadImages("bobbleO1","bobbleO2","bobbleO3","bobbleO4", "bobbleO5", "bobbleO6");
        private Image[] bobbleV = LoadImages("bobbleV1", "bobbleV2", "bobbleV3", "bobbleV4", "bobbleV5", "bobbleV6");
        private Image[] bobbleT = LoadImages("bobbleT1", "bobbleT2", "bobbleT3", "bobbleT4", "bobbleT5", "bobbleT6");
        private Image[] bobbleE = LoadImages("bobbleE1", "bobbleE2", "bobbleE3", "bobbleE4", "bobbleE5", "bobbleE6");



        public override void Begin()
        {
            IsFullScreen = false;
            MultiSelectWindow aloitus = new MultiSelectWindow("ProjectPK", "Start a new run", "QUIT");
            aloitus.AddItemHandler(0, aloitaPeli);
            aloitus.AddItemHandler(1, Exit);
            Add(aloitus);
        }
        public void aloitaPeli()
        {
            luoPelaaja();
            LuoElamalaskuri(); // Initialize the health meter
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
            vihut.Y = Screen.Top - 20 + 3;
            Add(vihut);
        }
        void LuoVihuLaskuri()
        {
            vihujaNyt = new DoubleMeter(vihujaHengiss‰);
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
            Vector syntym‰Paikka;
            do
            {
                double x = RandomGen.NextDouble(Level.Left + 100, Level.Right - 100);
                double y = RandomGen.NextDouble(Level.Bottom + 100, Level.Top - 100);
                syntym‰Paikka = new Vector(x, y);
            } while (Vector.Distance(syntym‰Paikka, pelaaja.Position) < 500);
            vihu.Position = syntym‰Paikka;
            ase.X = vihu.X;
            ase.Y = vihu.Y;
            ase.Tag = "weapon";
            vihu.CanRotate = false;
            Add(vihu);
            vihu.Add(ase);
            vihuCount += 1;
            vihujaHengiss‰ += 1;
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
            pelaaja.Restitution = 0;
            Add(pelaaja);
            pelaaja.Add(fireball);
            fireball.ProjectileCollision = fireballOsui;

            lisaaOhjaimet();
            luoReunat();

        }
        private void LuoElamalaskuri()
        {
            pelaajaHp = new DoubleMeter(10);
            pelaajaHp.MaxValue = 10;
            ProgressBar elamapalkki = new ProgressBar(150, 20);
            elamapalkki.X = Screen.Left + 150;
            elamapalkki.Y = Screen.Top - 20;
            elamapalkki.Angle = Angle.StraightAngle;
            elamapalkki.BorderColor = Color.Black;
            elamapalkki.BindTo(pelaajaHp);
            Add(elamapalkki);
        }
        private void luoReunat()
        {

            Level.Size = new Vector(10000, 10000);
            Level.CreateBorders(0, true);

        }
        private void lisaaOhjaimet()
        {
            Keyboard.Listen(Jypeli.Key.W, ButtonState.Pressed, liiku, "move", new Vector(0, 500 * speed));
            Keyboard.Listen(Jypeli.Key.W, ButtonState.Released, liiku, "move", new Vector(0, -500 * speed));
            Keyboard.Listen(Jypeli.Key.W, ButtonState.Pressed, () => anim(bobbleT), "move");
            Keyboard.Listen(Jypeli.Key.D, ButtonState.Pressed, liiku, "move", new Vector(500 * speed, 0));
            Keyboard.Listen(Jypeli.Key.D, ButtonState.Released, liiku, "move", new Vector(-500 * speed, 0));
            Keyboard.Listen(Jypeli.Key.D, ButtonState.Pressed, () => anim(bobbleO), "move");
            Keyboard.Listen(Jypeli.Key.S, ButtonState.Pressed, liiku, "move", new Vector(0, -500 * speed));
            Keyboard.Listen(Jypeli.Key.S, ButtonState.Released, liiku, "move", new Vector(0, 500 * speed));
            Keyboard.Listen(Jypeli.Key.S, ButtonState.Pressed, () => anim(bobbleE), "move");
            Keyboard.Listen(Jypeli.Key.A, ButtonState.Pressed, liiku, "move", new Vector(-500 * speed, 0));
            Keyboard.Listen(Jypeli.Key.A, ButtonState.Released, liiku, "move", new Vector(500 * speed, 0));
            Keyboard.Listen(Jypeli.Key.A, ButtonState.Pressed, () => anim(bobbleV), "move");
            Keyboard.Listen(Jypeli.Key.Q, ButtonState.Down, delegate { if (Camera.ZoomFactor > 0.5) { Camera.ZoomFactor -= 0.01; } }, "Zoom out");
            Keyboard.Listen(Jypeli.Key.E, ButtonState.Down, delegate { if (Camera.ZoomFactor < 1) { Camera.ZoomFactor += 0.01; } }, "Zoom in");
            Mouse.Listen(Jypeli.MouseButton.Left, ButtonState.Down, ammu, "fire", 0);
            Mouse.ListenMovement(0.1, tahtaa, "aim");
            Keyboard.Listen(Jypeli.Key.Space, ButtonState.Pressed, dash, "dash");
            Keyboard.Listen(Jypeli.Key.Escape, ButtonState.Pressed, menu, "menu");
        }
        private void anim(Image[] suunta)
        {
            if (pelaaja.Velocity.Magnitude > 0)
            {
                pelaaja.Animation = new Animation(suunta);
                pelaaja.Animation.FPS = 10;
                pelaaja.Animation.Start();
            }
            
        }
        public void menu()
        {
            ClearControls();
            IsPaused = true;
            pelaaja.Velocity = new Vector(0, 0);
            pelaajanNopeus = new Vector(0, 0);
            MultiSelectWindow menu = new MultiSelectWindow("Menu", "Resume", "Keybinds", "Quit");
            menu.AddItemHandler(0, delegate { IsPaused = false; lisaaOhjaimet(); });
            menu.AddItemHandler(1, delegate { IsPaused = false; lisaaOhjaimet(); MessageDisplay.Add("WASD: move, LMB: shoot, SPACE: dash, Q & E: zoom");  });
            menu.AddItemHandler(2, Exit);
            Add(menu);
        }

        void fireballOsui(PhysicsObject fireball, PhysicsObject kohde)
        {
            fireball.Destroy();
            if (kohde.Tag.ToString() == "vihu")
            {
                kohde.Destroy();
                vihujaHengiss‰ -= 1;
                vihujaNyt.Value = vihujaHengiss‰;
                if (LS > 0)
                {
                    pelaajaHp.Value += LS;
                    if (pelaajaHp.Value > pelaajaHp.MaxValue)
                    {
                        pelaajaHp.Value = pelaajaHp.MaxValue;
                    }
                }
                if (vihujaHengiss‰ == 0)
                {
                    vihuCount = 0;
                    wave += 1;
                    uusiWave(wave);

                }
            }
        }
        private void uusiWave(int wave)
        {
            pelaajaHp.Value = pelaajaHp.MaxValue;
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
                pelaajaHp.Value -= 1;
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
                return;
            }
        }
        private void ResetGameState()
        {
            wave = 1;
            vihuHp = 0;
            vihujaHengiss‰ = 0;
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
                kuti.Size *= bulletSize;
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
            Timer.SingleShot(dashTime, () => pelaaja.Velocity = pelaajanNopeus);
            Timer.SingleShot(dashTime, () => kuolematon = false);
            if (dashDelay < 0.05) { dashDelay = 0.05; };
            Timer.SingleShot(dashDelay, () => canDash = true);
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

            Timer.SingleShot(0.1, () => p‰ivit‰Kompassi(kompassi));
        }
        private void p‰ivit‰Kompassi(Widget kompassi)
        {
            if (pelaaja.Velocity.Magnitude == 0)
            {
                pelaaja.Animation.Stop();
            }
            GameObject l‰hinVihu = lˆyd‰L‰hinVihu();
            if (l‰hinVihu != null)
            {
                Vector suunta = (l‰hinVihu.Position - pelaaja.Position).Normalize();
                kompassi.Angle = suunta.Angle;
            }
            Timer.SingleShot(0.1, () => p‰ivit‰Kompassi(kompassi));

        }
        private GameObject lˆyd‰L‰hinVihu()
        {
            GameObject l‰hinVihu = null;
            double l‰hinEt‰isyys = double.MaxValue;
            foreach (var vihu in GetObjectsWithTag("vihu"))
            {
                double et‰isyys = (vihu.Position - pelaaja.Position).Magnitude;
                if (et‰isyys < l‰hinEt‰isyys)
                {
                    l‰hinEt‰isyys = et‰isyys;
                    l‰hinVihu = vihu;
                }
            }
            return l‰hinVihu;
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
                if (i == 5) {return "Dash delay"; }
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
            return "Speed"; // Default return value
        }
    }
}
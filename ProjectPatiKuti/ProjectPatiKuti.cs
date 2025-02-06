using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
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
        private Image bobble = LoadImage("bobble1");
        private int wave = 1;
        private int vihuHp = 0;
        private int vihujaHengissä = 0;
        private int vihuCount = 0;
        private Image maasto = LoadImage("Maasto");
        public override void Begin()
        {
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
            luoVihu();
            luoVihu();
            canDash = true;
            MediaPlayer.Play("PPK_Sountrack_Vol_1__BGPMLBT_Bullets_Go_Past_Me_Like_Bullet_Trains");
            MediaPlayer.IsRepeating = true;
            Level.Background.Image = maasto;
            Level.Background.TileToLevel();
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, delegate 
            {
                
            }, "Menu");
        }
        private void luoVihu()
        {
            vihuHp = wave * 1;
            vihu = new PhysicsObject(40, 40);
            vihu.Shape = Shape.Circle;
            vihu.Color = Color.Red;
            vihu.Image = blazoid;
            vihu.Tag = "vihu";
            AssaultRifle ase = new AssaultRifle(30, 30);
            
            
            vihu.Position = RandomGen.NextVector(pelaaja.X + pelaaja.Y, 400);
            ase.X = vihu.X;
            ase.Y = vihu.Y;
            ase.Tag = "weapon";
            vihu.CanRotate = false;
            Add(vihu);
            vihu.Add(ase);
            vihuCount += 1;
            vihujaHengissä += 1;
            
            Timer.CreateAndStart(1, delegate { ase.Shoot(); });
            ase.ProjectileCollision = AmmusOsui;
        }
        

        public void luoPelaaja()
        {
            pelaaja = new PhysicsObject(41, 55); 
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
            Level.CreateBorders(0,true);
            
        }
        private void lisaaOhjaimet()
        {
            Keyboard.Listen(Key.W, ButtonState.Pressed, liiku, "move", new Vector(000, 500));
            Keyboard.Listen(Key.W, ButtonState.Released, liiku, "move", new Vector(0, -500));
            Keyboard.Listen(Key.D, ButtonState.Pressed, liiku, "move", new Vector(500, 0));
            Keyboard.Listen(Key.D, ButtonState.Released, liiku, "move", new Vector(-500, 0));
            Keyboard.Listen(Key.S, ButtonState.Pressed, liiku, "move", new Vector(0, -500));
            Keyboard.Listen(Key.S, ButtonState.Released, liiku, "move", new Vector(0, 500));
            Keyboard.Listen(Key.A, ButtonState.Pressed, liiku, "move", new Vector(-500, 0));
            Keyboard.Listen(Key.A, ButtonState.Released, liiku, "move", new Vector(500, 0));
            Keyboard.Listen(Key.W, ButtonState.Down, vihuTahtaa, "");
            Keyboard.Listen(Key.A, ButtonState.Down, vihuTahtaa, "");
            Keyboard.Listen(Key.S, ButtonState.Down, vihuTahtaa, "");
            Keyboard.Listen(Key.D, ButtonState.Down, vihuTahtaa, "");
            Mouse.Listen(MouseButton.Left, ButtonState.Pressed, ammu, "fire", 0);
            Mouse.ListenMovement(0.1, tahtaa, "aim");
            Mouse.ListenMovement(0.1, vihuTahtaa, "aim");
            Keyboard.Listen(Key.Space, ButtonState.Pressed, dash, "dash");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, menu, "menu");
        }
       
        public void menu()
        {
            ClearControls();
            IsPaused = true;
            pelaaja.Velocity = new Vector(0, 0);
            MultiSelectWindow menu = new MultiSelectWindow("Menu", "Resume", "Quit");
            menu.AddItemHandler(0, delegate { IsPaused = false; lisaaOhjaimet(); });
            menu.AddItemHandler(1, Exit);
            Add(menu);
        }
        private void ammu(int suunta)
        {
            fireball.Shoot();
        }

        void fireballOsui(PhysicsObject fireball, PhysicsObject kohde)
        {
            fireball.Destroy();
            if (kohde.Tag.ToString() == "vihu")
            {
                kohde.Destroy();
                vihujaHengissä -= 1;
                if (vihujaHengissä == 0)
                {
                    vihuCount = 0;
                    wave += 1;

                    while (vihuCount < wave + 2)
                    {
                        luoVihu();
                    }
                }
            }
        }
        void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
        {
            ammus.Destroy();
            if (kohde==pelaaja)
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
            if(!canDash) return;
            canDash = false;
            pelaaja.Push(pelaajanNopeus * 100);
            kuolematon = true;
            Timer.SingleShot(0.2, () => pelaaja.Velocity = pelaajanNopeus);
            Timer.SingleShot(0.2, () => kuolematon=false);
            Timer.SingleShot(1, () => canDash=true);      
        }
        private void liiku(Vector nopeus)
        {
            if ((Math.Abs(pelaajanNopeus.X) == 250) && (Math.Abs(pelaajanNopeus.Y) == 250))
            {
                pelaajanNopeus = pelaajanNopeus * 2;
            }
            pelaajanNopeus += nopeus;
            if ((Math.Abs(pelaajanNopeus.X) == 500) && (Math.Abs(pelaajanNopeus.Y) == 500))
            {
                pelaajanNopeus = pelaajanNopeus / 2;
            }
            pelaaja.Velocity = pelaajanNopeus;
        }
    }
}
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

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
        private Vector pelaajanNopeus = new Vector(0, 0); 
        private PhysicsObject pelaaja;
        private PhysicsObject vihu;
        private bool kuolematon = false;
        private bool canDash = true;
        private bool i;
        private Image blazoid = LoadImage("blazoid");
        public override void Begin()
        {
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Exit PPK");
            luoPelaaja();
            luoVihu();
            
        }
        private void luoVihu()
        {
            
            vihu = new PhysicsObject(40, 40);
            vihu.Shape = Shape.Circle;
            vihu.Color = Color.Blue;
            ase = new AssaultRifle(30, 30);
            vihu.Image = blazoid;
            vihu.X = 100;
            vihu.Y = 100;
            ase.X = vihu.X;
            ase.Y = vihu.Y;
            vihu.CanRotate = false;
            Add(vihu);
            
            vihu.Add(ase);
            
        }
        

        public void luoPelaaja()
        {
            pelaaja = new PhysicsObject(40, 40); 
            pelaaja.Shape = Shape.Circle;
            pelaaja.Color = Color.Red;
            pelaaja.Velocity = pelaajanNopeus;
            pelaaja.Position = new Vector(0, 0);
            Add(pelaaja);
            lisaaOhjaimet();
            luoReunat();
        }
        private void luoReunat()
        {
            PhysicsObject reuna1 = Level.CreateLeftBorder();
            reuna1.Restitution = 0.0;
            PhysicsObject reuna2 = Level.CreateRightBorder();
            reuna2.Restitution = 0.0;
            PhysicsObject reuna3 = Level.CreateTopBorder();
            reuna3.Restitution = 0.0;
            PhysicsObject reuna4 = Level.CreateBottomBorder();
            reuna4.Restitution = 0.0;
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
            Keyboard.Listen(Key.W, ButtonState.Down, tahtaa , "");
            Keyboard.Listen(Key.A, ButtonState.Down, tahtaa, "");
            Keyboard.Listen(Key.S, ButtonState.Down, tahtaa, "");
            Keyboard.Listen(Key.D, ButtonState.Down, tahtaa, "");
            Keyboard.Listen(Key.Space, ButtonState.Pressed, dash, "dash");
        }
        private void tahtaa()
        {
            Vector suunta = (pelaaja.Position - vihu.Position).Normalize();
            ase.Angle = suunta.Angle;
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

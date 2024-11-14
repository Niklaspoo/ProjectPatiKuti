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
        private Vector pelaajanNopeus = new Vector(0, 0); 
        private PhysicsObject pelaaja; 
        public override void Begin()
        {
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Exit PPK");
            luoPelaaja();
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
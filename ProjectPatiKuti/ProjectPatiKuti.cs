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
        public override void Begin()
        {
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Exit PPK");
        }
    }

}

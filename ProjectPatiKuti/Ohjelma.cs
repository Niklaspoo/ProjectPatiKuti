﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace ProjectPatiKuti
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using var game = new ProjectPatiKuti();
            game.Run();
        }
    }
}

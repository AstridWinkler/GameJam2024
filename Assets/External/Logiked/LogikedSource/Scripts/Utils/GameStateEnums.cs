using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// [WIP] les états possibles pour le jeu
/// </summary>
public enum GameClockState
{
    InAnimation, mob, control, InLoading, ActivePhysics, GamePaused, GameControlInputs, InPauseMenu
}

/// <summary>
/// [WIP] les états possibles pour le jeu
/// </summary>
public enum GameState
{
    MainMenu, InGame
}


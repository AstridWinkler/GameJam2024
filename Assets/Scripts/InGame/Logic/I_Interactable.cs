using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

interface I_Interactable
{
    bool CallPlayerAction(PlayerBase source);
    bool CanPlayerAction();
}
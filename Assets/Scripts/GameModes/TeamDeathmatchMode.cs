﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple team deathmatch mode
/// </summary>
public class TeamDeathmatchMode : SimpleTeamGameMode {

    public override GameModes Mode {
        get { return GameModes.TEAM_DEATHMATCH; }
    }

    public override int MinPlayers {
        get { return 1; }
    }

    public override int MaxPlayers {
        get { return 16; }
    }

    public override IGameLevel level {
        get {
            throw new System.NotImplementedException();
        }
        set {
            throw new System.NotImplementedException();
        }
    }

    public override string GameModeName {
        get { return "Team Deathmatch"; }
    }
}

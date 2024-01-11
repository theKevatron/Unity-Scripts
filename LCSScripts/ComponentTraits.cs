using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Enums used in "BuilingItem", "BuildingActivatable", and "WorkStation" classes

// Status: 0 = Undefined, 1 = Full, 2 = Split
public enum Status 
{ 
    Undefined = 0,
    Full = 1,
    Split = 2
};
// Quality: 0 = Undefined, 1 = Pulp, 2 = Saw, 3 = Veneer
public enum Quality
{
    Undefined = 0,
    Pulp = 1,
    Saw = 2,
    Veneer = 3
}
// Style: 0 = Undefined, 1 = Round, 2 = D, 3 = Square
public enum Style 
{
    Undefined = 0,
    Square = 1,
    D = 2,
    Round = 3
};
// Type: 0 = Undefined, 1 = Sill, 2 = Wall, 3 = Top, 4 = Gable, 5 = Ridge, 6 = Board
public enum Type 
{
    Undefined = 0,
    Sill = 1,
    Wall = 2,
    Top = 3,
    Gable = 4,
    Ridge = 5,
    Board = 6
};

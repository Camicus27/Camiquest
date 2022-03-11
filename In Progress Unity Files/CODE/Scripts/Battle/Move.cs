using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public string moveName = "Fireball";
    public string moveDescription = "Shoots a fireball at one enemy. Deals 10 damage.";
    public string type = "FIRE";
    public bool isHealing = false;
    public int damage = 10;
    public int MPCost = 5;
    public float critChance = 0.05f;

    public Move(string name, string description, string type, int damage, int MP)
    {
        moveName = name;
        moveDescription = description;
        this.type = type;
        this.damage = damage;
        MPCost = MP;
        isHealing = false;
    }

    public Move(string name, string description, string type, int damage, int MP, bool heals)
    {
        moveName = name;
        moveDescription = description;
        this.type = type;
        this.damage = damage;
        MPCost = MP;
        isHealing = heals;
    }

    public override bool Equals(object obj)
    { return moveName == (obj as Move).moveName; }
    public override int GetHashCode()
    { return moveName.GetHashCode(); }

}

public static class Moves
{
    private static Move[] kineticMoves = {
        /*[0]*/new Move("Sword", "A basic sword.", "KINETIC", 15, 0),
        /*[1]*/new Move("Bash", "Violently hurl yourself at your enemy.", "KINETIC", 20, 0),
        /*[2]*/new Move("Smash", "Smash enemies using large, deadly fists.", "KINETIC", 30, 0),
        /*[3]*/new Move("Head Mash", "Ram your entire head directly into your enemy.", "KINETIC", 50, 10),
        /*[4]*/new Move("Sharpened Sword", "A basic sword, but sharpened!", "KINETIC", 75, 0),
    };
    private static Move[] fireMoves = {
        /*[0]*/new Move("Fire Shot", "Shoots a small ball of fire. Careful, it's hot.", "FIRE", 20, 10),
        /*[1]*/new Move("Flame Dash", "Charge your enemy with some EXTRA heat!", "FIRE", 35, 25),
        /*[2]*/new Move("Firey Fracture", "Launch a charge of flaming energy that fractures into firey shrapnels.", "FIRE", 50, 60),
        /*[3]*/new Move("Stoke the Fire", "Rekindle the flame of life.", "FIRE", 40, 0, true),    /*Only to be used by fire enemies*/
    };
    private static Move[] waterMoves = {
        /*[0]*/new Move("Water Stream", "Sprays a powerfully concentrated stream of water at your enemy. Hopefully that's water..", "WATER", 20, 10),
        /*[1]*/new Move("Water Bolt", "Fire a concentrated bolt of water at your enemy.", "WATER", 35, 25),
        /*[2]*/new Move("Power Wave", "Thrust a powerful wave of water toward your enemy.", "WATER", 45, 50),
        
    };
    private static Move[] toxicMoves = {
        /*[0]*/new Move("Sludge Bomb", "Toss a large ball of toxic sludge at your enemy. Gross.", "TOXIC", 35, 25),
        /*[1]*/new Move("Toxic Cleanse", "A toxic move that actually isn't that toxic. Cleanse your wounds!", "TOXIC", 100, 80, true),
    };
    private static Move[] undeadMoves = {
        /*[0]*/new Move("Undying Mesmer", "Conjure a window from our world into the grisly sight of the undead one.", "UNDEAD", 35, 25),
        /*[1]*/new Move("Incorporeal Shot", "What bullets are better than those you can't touch! How do they deal damage?", "UNDEAD", 50, 50),
        /*[2]*/new Move("Undead Encouragement", "When you're feeling too weak to continue fighting, some ancestors are willing to give you a little encouragement!", "UNDEAD", 110, 85, true),
    };
    private static Move[] pureMoves = {
        /*[0]*/new Move("Holy Light", "Emits a purifying light to cleanse the corrupt of this world.", "PURE", 30, 25),
        /*[1]*/new Move("Healing Pool", "Spawns a small health pool to heal the caster. I hear it's exfoliating!", "PURE", 100, 75, true),
        /*[2]*/new Move("Holy Beam", "Emits an immense purifying beam of light to cleanse everything corrupt in this world.", "PURE", 50, 80),
        /*[3]*/new Move("Holy Hand Grenade", "\"Oh Lord, bless this thy hand grenade, that with it, thou mayest blow thine enemies to tiny bits in thy mercy.\"", "PURE", 69, 100),
    };
    private static Move[] earthMoves = {
        /*[0]*/new Move("Rock Toss", "Hurl a large rock at your enemy. They'll never see it coming.", "EARTH", 20, 10),
        /*[1]*/new Move("Mud Blitz", "Blast the enemy with an intense bombardment of mud.", "EARTH", 35, 25),
        /*[2]*/new Move("Earth Tremble", "Like earthquake, but it's original.", "EARTH", 40, 50),
        
    };


    public static Move GetSpecificMove(string moveType, int moveID)
    {
        if (moveType == "RUNAWAY")
        {
            return new Move("Run Away", "RUN AWAY", "RUNAWAY", 0, 0);
        }

        switch (moveType)
        {
            case "KINETIC":
                return kineticMoves[moveID];
            case "FIRE":
                return fireMoves[moveID];
            case "WATER":
                return waterMoves[moveID];
            case "TOXIC":
                return toxicMoves[moveID];
            case "UNDEAD":
                return undeadMoves[moveID];
            case "PURE":
                return pureMoves[moveID];
            case "EARTH":
                return earthMoves[moveID];
        }
        return null;
    }

    public static Move GetRandomMoveOfType(string moveType)
    {
        int number;
        switch (moveType)
        {
            case "KINETIC":
                number = Random.Range(1, kineticMoves.Length);
                return kineticMoves[number];
            case "FIRE":
                number = Random.Range(1, fireMoves.Length);
                return fireMoves[number];
            case "WATER":
                number = Random.Range(1, waterMoves.Length);
                return waterMoves[number];
            case "TOXIC":
                number = Random.Range(0, toxicMoves.Length);
                return toxicMoves[number];
            case "UNDEAD":
                number = Random.Range(0, undeadMoves.Length);
                return undeadMoves[number];
            case "PURE":
                number = Random.Range(1, pureMoves.Length);
                return pureMoves[number];
            case "EARTH":
                number = Random.Range(1, earthMoves.Length);
                return earthMoves[number];
        }
        return null;
    }

    public static Move GetRandomMove()
    {
        int number;
        switch (Random.Range(0, 7))
        {
            case 0:
                number = Random.Range(0, kineticMoves.Length);
                return kineticMoves[number];
            case 1:
                number = Random.Range(0, fireMoves.Length);
                return fireMoves[number];
            case 2:
                number = Random.Range(0, waterMoves.Length);
                return waterMoves[number];
            case 3:
                number = Random.Range(0, toxicMoves.Length);
                return toxicMoves[number];
            case 4:
                number = Random.Range(0, undeadMoves.Length);
                return undeadMoves[number];
            case 5:
                number = Random.Range(0, pureMoves.Length);
                return pureMoves[number];
            case 6:
                number = Random.Range(0, earthMoves.Length);
                return earthMoves[number];
        }
        return null;
    }
}
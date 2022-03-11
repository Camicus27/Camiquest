using System.Collections;
using System.Collections.Generic;

public class TypeChart
{
    static float[][] chart =
    {                                   /*  defend  */
        // attack                KIN      FIR    WAT    TOX    UND    PUR   EARTH
        /*KINETIC*/ new float[] { .9f,     1f,    1f,    1f,    1.3f,  .6f,    .6f },
        /*FIRE*/    new float[] { 1.1f,   .6f,   .6f,    1.5f,  2f,     1f,    .6f },
        /*WATER*/   new float[] { 1.1f,   1.8f,   1f,    .6f,   1f,     1f,    1.75f },
        /*TOXIC*/   new float[] { 1.1f,    1f,    1.75f, .6f,   .8f,   1.75f,  1f },
        /*UNDEAD*/  new float[] { 1.1f,   .6f,    1f,    1f,    1f,    .4f,    1f },
        /*PURE*/    new float[] { .8f,     1f,    1f,    2f,    1.5f,   1f,    1f },
        /*EARTH*/   new float[] { 1.1f,    1f,   .6f,    1.3f,  1f,     1f,    1f }
    };

    public static float GetEffectiveness(string attackType, string defenseType)
    {
        int row = 0;
        int col = 0;

        switch (attackType)
        {   case "FIRE":
                row = 1; break;
            case "WATER":
                row = 2; break;
            case "TOXIC":
                row = 3; break;
            case "UNDEAD":
                row = 4; break;
            case "PURE":
                row = 5; break;
            case "EARTH":
                row = 6; break; }
        switch (defenseType)
        {   case "FIRE":
                col = 1; break;
            case "WATER":
                col = 2; break;
            case "TOXIC":
                col = 3; break;
            case "UNDEAD":
                col = 4; break;
            case "PURE":
                col = 5; break;
            case "EARTH":
                col = 6; break; }

        return chart[row][col];
    }
}

namespace Lib.ProblemSolving;

public static class Challenge2
{
    public static int DiceFacesCalculator(int dice1, int dice2, int dice3)
    {
        if (dice1 > 6 || dice1 < 1)
        {
            throw new Exception("Dice out of number range");
        }
        return 0;
    }
}
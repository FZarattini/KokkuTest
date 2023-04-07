using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    public class Grid
    {
        public List<GridBox> grids = new List<GridBox>();
        public int xLength;
        public int yLength;
        public Grid(int Lines, int Columns)
        {
            xLength = Columns;
            yLength = Lines;
            Console.WriteLine("The battle field has been created\n");

            for (int i = 0; i < Lines; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    GridBox newBox = new GridBox(j, i, false, (i * Columns + j), '\0');
                    grids.Add(newBox);
                    Console.Write($"{newBox.Index}\n");
                }
            }
        }

        // prints the matrix that indicates the tiles of the battlefield
        //public void drawBattlefield(int Lines, int Columns)
        public void drawBattlefield()
        {
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    GridBox currentgrid = grids[i * yLength + j];
                    if (currentgrid.ocupied)
                    {
                        //if()
                        Console.Write($"[{currentgrid.occupyingCharacter}]\t");
                    }
                    else
                    {
                        Console.Write($"[ ]\t");
                    }
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
        }

    }
}

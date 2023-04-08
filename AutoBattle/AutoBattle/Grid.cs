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
        public void drawBattlefield()
        {
            for(int i = 0; i < yLength; i++)
            {
                for(int j = 0; j < xLength; j++)
                {
                    GridBox currentBox = grids[(i * xLength) + j];

                    if (currentBox.ocupied)
                    {
                        Console.Write($"[{currentBox.occupyingCharacter}]\t");
                    }
                    else
                    {
                        //Console.Write($"[{currentBox.yIndex} {currentBox.xIndex}]\t");
                        Console.Write($"[ ]\t");
                    }
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
        }
    }
}

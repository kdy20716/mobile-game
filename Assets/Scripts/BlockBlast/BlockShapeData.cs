using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast
{
    [System.Serializable]
    public class BlockShape
    {
        public int[,] matrix;
        public Color blockColor;
        public bool isBomb;
        public string shapeName;

        public int Rows => matrix.GetLength(0);
        public int Cols => matrix.GetLength(1);

        public BlockShape(int[,] m, Color col, bool bomb, string name)
        {
            matrix = m;
            blockColor = col;
            isBomb = bomb;
            shapeName = name;
        }

        public void Rotate90Clockwise()
        {
            int oldR = Rows;
            int oldC = Cols;
            int[,] rotated = new int[oldC, oldR];

            for (int r = 0; r < oldR; r++)
            {
                for (int c = 0; c < oldC; c++)
                {
                    rotated[c, oldR - 1 - r] = matrix[r, c];
                }
            }
            matrix = rotated;
        }
    }

    public static class BlockShapeData
    {
        public static readonly Color[] Palette = new Color[]
        {
            new Color(0.22f, 0.74f, 0.97f), // Cyan
            new Color(0.96f, 0.62f, 0.04f), // Amber
            new Color(0.06f, 0.73f, 0.51f), // Emerald
            new Color(0.93f, 0.28f, 0.60f), // Pink
            new Color(0.55f, 0.36f, 0.96f), // Purple
            new Color(0.98f, 0.45f, 0.09f), // Orange
        };

        public static readonly Color BombColor = new Color(0.94f, 0.27f, 0.27f); // Red Bomb

        // 18 Standard Block Shapes
        public static readonly int[][,] ShapeTemplates = new int[][,]
        {
            // 1x1 Dot
            new int[,] { { 1 } },

            // 1x2, 1x3, 1x4 Bars
            new int[,] { { 1, 1 } },
            new int[,] { { 1 }, { 1 } },
            new int[,] { { 1, 1, 1 } },
            new int[,] { { 1 }, { 1 }, { 1 } },
            new int[,] { { 1, 1, 1, 1 } },
            new int[,] { { 1 }, { 1 }, { 1 }, { 1 } },

            // Squares
            new int[,] { { 1, 1 }, { 1, 1 } },
            new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } },

            // Corners (2x2)
            new int[,] { { 1, 1 }, { 1, 0 } },
            new int[,] { { 1, 1 }, { 0, 1 } },
            new int[,] { { 1, 0 }, { 1, 1 } },
            new int[,] { { 0, 1 }, { 1, 1 } },

            // Large L (3x3)
            new int[,] { { 1, 0, 0 }, { 1, 0, 0 }, { 1, 1, 1 } },
            new int[,] { { 0, 0, 1 }, { 0, 0, 1 }, { 1, 1, 1 } },

            // T Shapes
            new int[,] { { 1, 1, 1 }, { 0, 1, 0 } },
            new int[,] { { 0, 1, 0 }, { 1, 1, 1 } },
            new int[,] { { 1, 0 }, { 1, 1 }, { 1, 0 } },
        };

        public static BlockShape GetRandomShape(float bombChance = 0.15f)
        {
            int shapeIdx = Random.Range(0, ShapeTemplates.Length);
            int[,] template = (int[,])ShapeTemplates[shapeIdx].Clone();

            bool isBomb = Random.value < bombChance;
            Color col = isBomb ? BombColor : Palette[Random.Range(0, Palette.Length)];

            return new BlockShape(template, col, isBomb, $"Shape_{shapeIdx}");
        }
    }
}

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
            // UI 좌표계(row0=화면 상단) 기준 시계방향 90도
            // rotated[c, oldR-1-r] = matrix[r, c]
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
        // 🍬 Sweet Macaron & Pastel Jelly Color Palette
        public static readonly Color[] Palette = new Color[]
        {
            new Color(0.36f, 0.77f, 1.0f),   // Sky Milk Blue (#5CC4FF)
            new Color(1.0f, 0.75f, 0.26f),   // Mango Sorbet Yellow (#FFC043)
            new Color(0.31f, 0.88f, 0.71f),  // Apple Soda Mint (#4EE0B5)
            new Color(1.0f, 0.48f, 0.64f),   // Strawberry Milk Pink (#FF7AA2)
            new Color(0.65f, 0.49f, 1.0f),   // Berry Lavender Purple (#A67CFF)
            new Color(1.0f, 0.56f, 0.42f),   // Peach Coral Orange (#FF8F6B)
        };

        public static readonly Color BombColor = new Color(1.0f, 0.30f, 0.41f); // Sweet Cherry Red (#FF4D69)

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

        public static BlockShape GetRandomShape(float bombChance = 0f)
        {
            int shapeIdx = Random.Range(0, ShapeTemplates.Length);
            int[,] template = (int[,])ShapeTemplates[shapeIdx].Clone();

            bool isBomb = false;
            Color col = Palette[Random.Range(0, Palette.Length)];

            return new BlockShape(template, col, isBomb, $"Shape_{shapeIdx}");
        }
    }
}

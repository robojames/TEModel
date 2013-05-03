using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEModel
{
    class Node
    {
        public int ID;
        public float x_Position;
        public float y_Position;
        public float delta_Y;
        public float delta_X;

        public Node(float X, float Y, float DY, float DX, int ID)
        {
            delta_X = DX;
            delta_Y = DY;
            x_Position = X;
            y_Position = Y;
            this.ID = ID;
        }
    }
}

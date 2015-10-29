using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCV_HW_2
{
    public class LoG_Operator
    {

        public int[,] Mask { get; set; }
        private int K { get; set; }
        private int Scale { get; set; }
        private int Sigma { get; set; }
        
        public LoG_Operator(int m, int sigma, int constantK)
        {
            int x = -m;
            int y = -m;
            K = constantK;
            Sigma = sigma;
            Scale = (m*2) + 1;

            Operate(x, y);
            PrintMask();

        }

        private void PrintMask()
        {
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    Console.Write(String.Format("|{0}|", Mask[j, i]));
                }
            }
        }


        private void Operate(int x, int y)
        {
            Mask = new int[Scale, Scale];
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    SetCell(i, j, x, y);
                    y++;
                }
                x++;
            }
        }



        private void SetCell(int i, int j, int xval, int yval)
        {
            int rsquared = xval*xval + yval*yval;
            var firstpart = K*((rsquared - (Sigma*Sigma))/( Sigma * Sigma * Sigma * Sigma));
            var power = -1*(rsquared/(2*Sigma*Sigma));
            var secondpart = Math.Pow(Math.E, power);
            var value = firstpart*secondpart;
            Mask[i, j] = (int)value;

        }
    }
}

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
        private double Sigma { get; set; }
        
        public LoG_Operator(int m, double sigma)
        {
            int x = -m;
            int y = -m;
           
            Sigma = sigma;
            Scale = (m*2) + 1;
            GetK();
            Operate(x, y);
            PrintMask();
            CheckMask();

        }

        //required to interactively scale results such that they are reasonable
        private void GetK()
        {
            K = 50;
            while(Math.Abs(GetSlidesValue(0, 0)) < (Scale/3)*(Scale/3))
                K += 10;

        }

        private void CheckMask()
        {
            var total = 0;
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    total += Mask[i, j];
                }
            }
            Console.WriteLine(String.Format("Sum of mask: {0}", total));
        }

        private void PrintMask()
        {
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    Console.Write(String.Format("|{0}|", Mask[i, j]));
                }
                Console.WriteLine("");
            }
        }


        private void Operate(int x, int y)
        {
            var xorig = x;
            //x and y represent top left
            Mask = new int[Scale, Scale];
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    SetCell(i, j, x, y);
                    x++;
                }
                y++;
                x = xorig;
            }
        }



        private void SetCell(int i, int j, int xval, int yval)
        {
            var value = GetSlidesValue(xval, yval);
            Mask[i, j] = (int)value;
        }

        private double GetSlidesValue(int xval, int yval)
        {
            int rsquared = (xval*xval) + (yval*yval);
            var firstpart = ((rsquared - (Sigma*Sigma))/(Sigma*Sigma*Sigma*Sigma));
            var power = -1 * (rsquared/(2*Sigma*Sigma));
            var secondpart = Math.Pow(Math.E, power);
            var value = K*firstpart*secondpart;
            return value;
        }
    }
}

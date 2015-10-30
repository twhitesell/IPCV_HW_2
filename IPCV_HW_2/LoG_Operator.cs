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
        private int M { get; set; }





        #region Construct





        public LoG_Operator(int m, double sigma)
        {
            M = m;
            int x = -m;
            int y = -m;
           
            Sigma = sigma;
            Scale = (m*2) + 1;
            GetK();
            CreateOperator(x, y);
            var total = CheckMask();
            while (total != 0)
            {
                ScaleMask(total);
                total = CheckMask();
            }

        }

        private void ScaleMask(int total)
        {
            if (total > 0)
            {
                ReduceMask(total);
            }
            if (total < 0)
            {
                EnhanceMask(total);
            }


        }

        /// <summary>
        /// moves closer to 0 coefficient without skew
        /// </summary>
        private void EnhanceMask(int total)
        {
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    if (Mask[i, j] < 0)
                    {
                        Mask[i, j] = (int)(Mask[i,j] * .95);
                    }
                }
            }
        }

        /// <summary>
        /// moves closer to 0 coeffiicent sum without skew
        /// </summary>
        private void ReduceMask(int total)
        {
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    if (Mask[i,j] > 0)
                    {
                        Mask[i, j] = (int)(Mask[i,j] * .95);
                    }
                }
            }
        }

        //required to interactively scale results such that they are reasonable
        private void GetK()
        {
            K = 50;
            while(Math.Abs(GetSlidesValue(0, 0)) < (Scale*2))
                K += 10;

        }

        /// <summary>
        /// ensures that coefficients sum to 0
        /// </summary>
        /// <returns></returns>
        private int CheckMask()
        {
            var total = 0;
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    total += Mask[i, j];
                }
            }
            if (total > 0 && total < 5)
            {
                var center = (Scale - 1)/2;
                Mask[center, center] -= total;
                Console.WriteLine(String.Format("Sum of mask: {0}", 0));
                Console.WriteLine(String.Format("Mask:"));
                PrintMask();
                return 0;
            }
            else if (total < 0 && total > -5)
            {
                var center = (Scale - 1) / 2;
                Mask[center, center] += total;
                Console.WriteLine(String.Format("Sum of mask: {0}", 0));
                Console.WriteLine(String.Format("Mask:\n"));
                PrintMask();
                return 0;
            }
            else
            {
                return total;
            }
        }


        /// <summary>
        /// prints the mask
        /// </summary>
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


        /// <summary>
        /// instantiates size and fills matrix operator
        /// </summary>
        private void CreateOperator(int x, int y)
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



#endregion


        /// <summary>
        /// performs overall convolution
        /// </summary>
        public int[,] Convolve(Bitmap original, int[,] padded)
        {
            int[,] arr = new int[original.Width, original.Height];
            int i = 0, j = 0;
            for (int x = M + 1; x < original.Width + M; x++)
            {
                for (int y = M + 1; y < original.Height + M; y++)
                {
                    arr[i, j] = ConvolveAtPoint(padded, x, y);
                    j++;
                }
                j = 0;
                i++;
            }
            return arr;
        }


        /// <summary>
        /// performs convolution at point
        /// </summary>
        private int ConvolveAtPoint(int[,] padded, int x, int y)
        {
            
            var value = 0;
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    value += padded[x - M + i, y - M + j] * Mask[i,j];
                    
                }
            }



            return value;
        }
    }
}

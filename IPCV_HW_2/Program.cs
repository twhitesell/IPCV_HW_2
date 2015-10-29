using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace IPCV_HW_2
{
    public class Program
    {
        private static bool continueRun = true;

        private static string inputfile;
        private static string outputfile;
        private static int lowthreshold = 0;
        private static int highthreshold = 255;
        
        private static Size size;


        /// <summary>
        /// entry point
        /// </summary>
        static void Main(string[] args)
        {
            Write("Welcome to LoG Utility!");

            //GetInputFilename();
            //GetOutputFilename();
            var sigma = GetSigma();
            int m = GetM(sigma);

            var op = new LoG_Operator(m, sigma);

            var continueRun = Reprocess();

            while (continueRun)
            {
                //RefreshParameters();

                sigma = GetSigma();
                m = GetM(sigma);

                op = new LoG_Operator(m, sigma);
                continueRun = Reprocess();
            }

            SignalGrandExit();

        }

        private static int GetM(double sigma)
        {
            int res = (int) (6*sigma);
            if (res <= 3) return 3;
            if (res%2 == 0) return res + 1;
            return res;

        }

        #region PROCESS_IMAGE



        /// <summary>
        /// processes the histogram equalization
        /// </summary>
        private static bool ProcessImage()
        {
            try
            {
                // Open an Image file, and get its bitmap
                var currentdir = Directory.GetCurrentDirectory();
                Image myImage = Image.FromFile(inputfile);
                var bitmap = new Bitmap(myImage);

                //create output bitmap of he same size
                var output = new Bitmap(size.Width, size.Height);

                for (int x = 0; x < myImage.Width; x++)
                {
                    for (int y = 0; y < myImage.Height; y++)
                    {
                        var pi = bitmap.GetPixel(x, y);
                        CreateBinaryImage(pi, output, x, y);
                    }
                }


                output.Save(currentdir + "\\" + outputfile);
                Write(String.Format("File: {0} generated ok.", outputfile));
                return true;
            }
            catch (Exception e)
            {
                Write(String.Format("Exception thrown: {0}", e.Message));
                return false;
            }
        }





        /// <summary>
        /// counts occurrences of each possible intensity
        /// </summary>
        private static void ObtainEachPixelColor()
        {

            Image myImage = Image.FromFile(inputfile);
            var bitmap = new Bitmap(myImage);

            size = bitmap.Size;

            //index across each position in the 2-d image
            for (int x = 0; x < myImage.Width; x++)
            {
                for (int y = 0; y < myImage.Height; y++)
                {
                    var pi = bitmap.GetPixel(x, y);
                }
            }
        }


        /// <summary>
        /// requires the argb color of the pixel, the output image to set into, and the position of each pixel to operate upon
        /// results in the output Bitmap object
        /// </summary>
        private static void CreateBinaryImage(Color pi, Bitmap output, int x, int y)
        {
            int grayscale = GetGrayscale(pi);
            //if the grayscale value meets some criteria
            if (conditionIsTrue(grayscale))
            {
                //set the corresponding output pixel
                output.SetPixel(x, y, Color.White);
            }
            else
            {
                output.SetPixel(x, y, Color.Black);
            }
        }


        /// <summary>
        /// probably some threshold or zero crossing detection
        /// </summary>
        /// <param name="grayscale"></param>
        /// <returns></returns>
        private static bool conditionIsTrue(int grayscale)
        {
            return grayscale > lowthreshold && grayscale < highthreshold;
        }

        /// <summary>
        /// here we get the grayscale intensity
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static int GetGrayscale(Color pi)
        {
            return (pi.R + pi.G + pi.B) / 3;
        }




        #endregion







        #region Utility


        private static double GetSigma()
        {
            while (true)
            {

                Write("Enter value of Sigma (0.0 - 5.0):");
                var c = Console.ReadLine();
                double v;
                if (double.TryParse(c, out v))
                {
                    if (v > 0 && v <= 5)
                        return v;
                }

            }
        }



        /// <summary>
        /// determines whether or not we continue
        /// </summary>
        private static bool Reprocess()
        {
            Write("");
            Write("Enter 'Q' to quit, or any other key to continue.");
            var key = Console.ReadKey(false);
            if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                return false;
            return true;
        }




        /// <summary>
        /// gets name of output file
        /// </summary>
        private static void GetOutputFilename()
        {
            Write("Please enter the output file name only:");
            outputfile = Console.ReadLine();
        }






        /// <summary>
        /// gets input filename
        /// </summary>
        private static void GetInputFilename()
        {
            while (true)
            {
                Write("Please enter relative file name:");
                var line = Console.ReadLine();
                if (!File.Exists(line))
                {
                    Write("No such file.");
                    continue;
                }
                inputfile = line;
                break;
            }
        }


        /// <summary>
        /// prints to console
        /// </summary>
        private static void Write(String s)
        {
            Console.WriteLine(s);
        }




        private static void SignalGrandExit()
        {
            int b = 1;
            for (int i = b; b < 10000; i++)
            {
                string s = "";
                for (int a = 0; a < b; a++)
                {
                    s += "*";
                }

                b *= 2;
                Write(s);
            }

        }


        #endregion
    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

//Brian Tran
//October 2014
//Assignment 3: Image Processing
//To perform various processes on an image

namespace ImageProcessing
{
    public partial class frmMain : Form
    {
        private Color[,] original; //this is the original picture - never change the values stored in this array
        private Color[,] transformedPic;  //transformed picture that is displayed

        public frmMain()
        {
            InitializeComponent();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            //this method reads in a picture file and stores it in an array

            //try catch should handle any errors for invalid picture files
            try
            {
                //open the file dialog to select a picture file
                OpenFileDialog fd = new OpenFileDialog();

                //create a bitmap to store the file in
                Bitmap bmp;

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    //store the selected file into a bitmap
                    bmp = new Bitmap(fd.FileName);

                    //create the arrays that store the colours for the image
                    //the size of the arrays is based on the height and width of the bitmap
                    //initially both the original and transformedPic arrays will be identical
                    original = new Color[bmp.Height, bmp.Width];
                    transformedPic = new Color[bmp.Height, bmp.Width];

                    //load each color into a color array
                    for (int i = 0; i < bmp.Height; i++)//each row
                    {
                        for (int j = 0; j < bmp.Width; j++)//each column
                        {
                            //assign the colour in the bitmap to the array
                            original[i, j] = bmp.GetPixel(j, i);
                            transformedPic[i, j] = original[i, j];
                        }
                    }
                    //this will cause the form to be redrawn and OnPaint() will be called
                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Loading Picture File.\n" + ex.Message);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //this method draws the transformed picture
            //what ever is stored in transformedPic array will
            //be displayed on the form

            base.OnPaint(e);

            Graphics g = e.Graphics;

            //only draw if picture is transformed
            if (transformedPic != null)
            {
                //get height and width of the transfrormedPic array
                int height = transformedPic.GetUpperBound(0) + 1;
                int width = transformedPic.GetUpperBound(1) + 1;

                //create a new Bitmap to be dispalyed on the form
                Bitmap newBmp = new Bitmap(width, height);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        //loop through each element transformedPic and set the 
                        //colour of each pixel in the bitmalp
                        newBmp.SetPixel(j, i, transformedPic[i, j]);
                    }
                }
                //call DrawImage to draw the bitmap
                g.DrawImage(newBmp, (ClientSize.Width - width) / 2, (ClientSize.Height + 20 - height) / 2, width, height);
            }            
        }

        private void mnuProcessReset_Click(object sender, EventArgs e)
        {
            transformedPic = new Color[original.GetLength(0), original.GetLength(1)];

            for (int i = 0; i < transformedPic.GetLength(0); i++)
            {
                for (int j = 0; j < transformedPic.GetLength(1); j++)
                {
                    transformedPic[i, j] = original[i, j];
                }
            }

            this.Refresh();
        }

        private void mnuProcessInvert_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                //Store colour intensity
                int Red, Green, Blue;

                //Loop through each colour element
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        //Get the opposite value for R, G, B
                        Red = 255 - transformedPic[i, j].R;
                        Green = 255 - transformedPic[i, j].G;
                        Blue = 255 - transformedPic[i, j].B;

                        //Recombine colours
                        transformedPic[i, j] = Color.FromArgb(Red, Green, Blue);
                    }
                }

                this.Refresh();
            }
        }

        private void mnuProcessDarken_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                //Store colour intensity
                int Red, Green, Blue;

                int Height = transformedPic.GetLength(0);
                int Width = transformedPic.GetLength(1);

                //Loop through each colour element in transformed pic
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        //Subtract 10 from R, G, B
                        Red = transformedPic[i, j].R - 10;
                        if (Red < 0) Red = 0;

                        Blue = transformedPic[i, j].B - 10;
                        if (Blue < 0) Blue = 0;

                        Green = transformedPic[i, j].G - 10;
                        if (Green < 0) Green = 0;

                        //Recombine colour based on new values and assign to transformed pic
                        transformedPic[i, j] = Color.FromArgb(Red, Green, Blue);
                    }
                }

                //Redraw picture
                this.Refresh();
            }
        }

        private void mnuProcessWhiten_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                //Store colour intensity
                int Red, Green, Blue;

                int Height = transformedPic.GetLength(0);
                int Width = transformedPic.GetLength(1);

                //Loop through each colour element in transformed pic
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        //Add 10 from R, G, B
                        Red = transformedPic[i, j].R + 10;
                        if (Red > 255) Red = 255;

                        Blue = transformedPic[i, j].B + 10;
                        if (Blue > 255) Blue = 255;

                        Green = transformedPic[i, j].G + 10;
                        if (Green > 255) Green = 255;

                        //Recombine colour based on new values and assign to transformed pic
                        transformedPic[i, j] = Color.FromArgb(Red, Green, Blue);
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessFlipX_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                Color Temp;
                int Length = transformedPic.GetLength(1) - 1;

                //Loop through half of the columns and all of the rows
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    //Do not loop less than Length because it will iss the middle column
                    for (int j = 0; j < transformedPic.GetLength(1) / 2; j++)
                    {
                        //Store colour in a temp and switch with opposite side
                        Temp = transformedPic[i, j];
                        transformedPic[i, j] = transformedPic[i, Length - j];
                        transformedPic[i, Length - j] = Temp;
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessFlipY_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                Color Temp;
                int Length = transformedPic.GetLength(0) - 1;

                //Loop through half of the rows and all of the columns
                for (int i = 0; i < transformedPic.GetLength(0) / 2; i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        //Store colour in a temp and switch with opposite side
                        Temp = transformedPic[i, j];
                        transformedPic[i, j] = transformedPic[Length - i, j];
                        transformedPic[Length - i, j] = Temp;
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessMirrorH_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                Color[,] TempPic = new Color[transformedPic.GetLength(0), transformedPic.GetLength(1)];

                //Copy transformed pic into temp pic
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        TempPic[i, j] = transformedPic[i, j];
                    }
                }

                //Resize transformed pic
                transformedPic = new Color[TempPic.GetLength(0), TempPic.GetLength(1) * 2];
                int Length = transformedPic.GetLength(1) - 1;

                //Copy temp pic over to transformed pic (double columns) and mirror horizontal
                for (int i = 0; i < TempPic.GetLength(0); i++)
                {
                    for (int j = 0; j < TempPic.GetLength(1); j++)
                    {
                        transformedPic[i, j] = TempPic[i, j];
                        transformedPic[i, Length - j] = TempPic[i, j];
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessMirrorV_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                Color[,] TempPic = new Color[transformedPic.GetLength(0), transformedPic.GetLength(1)];

                //Copy transformed pic into temp pic
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        TempPic[i, j] = transformedPic[i, j];
                    }
                }

                //Resize transformed pic
                transformedPic = new Color[TempPic.GetLength(0) * 2, TempPic.GetLength(1)];
                int Length = transformedPic.GetLength(0) - 1;

                //Copy temp pic over to transformed pic (double rows) and mirror vertical
                for (int i = 0; i < TempPic.GetLength(0); i++)
                {
                    for (int j = 0; j < TempPic.GetLength(1); j++)
                    {
                        transformedPic[i, j] = TempPic[i, j];
                        transformedPic[Length - i, j] = TempPic[i, j];
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessScale50_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                //Check for minimum 1 by 1 size
                if (transformedPic.GetLength(0) / 2 >= 1 && transformedPic.GetLength(1) / 2 >= 1)
                {
                    Color[,] TempPic = new Color[transformedPic.GetLength(0), transformedPic.GetLength(1)];

                    //Copy tranformed pic into temp pic
                    for (int i = 0; i < transformedPic.GetLength(0); i++)
                    {
                        for (int j = 0; j < transformedPic.GetLength(1); j++)
                        {
                            TempPic[i, j] = transformedPic[i, j];
                        }
                    }

                    //Resize transformed pic
                    transformedPic = new Color[TempPic.GetLength(0) / 2, TempPic.GetLength(1) / 2];

                    //Copy temp pic over to transformed pic (half the rows and half the columns)
                    //by taking every other pixel
                    for (int i = 0; i < transformedPic.GetLength(0); i++)
                    {
                        for (int j = 0; j < transformedPic.GetLength(1); j++)
                        {
                            transformedPic[i, j] = TempPic[i * 2, j * 2];
                        }
                    }

                    //Redraw
                    this.Refresh();
                }
            }
        }

        private void mnuProcessScale200_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                Color[,] TempPic = new Color[transformedPic.GetLength(0), transformedPic.GetLength(1)];
                //Copy transformed pic back into temp pic
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        TempPic[i, j] = transformedPic[i, j];
                    }
                }

                //Copy transformed pic over to temporary pic (double the rows and double the columns)
                //by averaging every other pixel (even lines avg right and left, odd lines avg top and bottom)
                double Red, Green, Blue;
                //Resize transformed pic
                transformedPic = new Color[TempPic.GetLength(0) * 2, TempPic.GetLength(1) * 2];

                //Loop all present pixels first
                for (int i = 0; i < transformedPic.GetLength(0); i += 2)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j += 2)
                    {
                        transformedPic[i, j] = TempPic[i / 2, j / 2];
                    }
                }

                //Now loop empty pixels in even rows (some pixels filled)
                for (int i = 0; i < transformedPic.GetLength(0); i +=2)
                {
                    for (int j = 1; j < transformedPic.GetLength(1); j += 2)
                    {
                        //Check bottom edge of image (cannot average)
                        if (i + 1 > transformedPic.GetLength(0) - 1)
                        {
                            transformedPic[i, j] = transformedPic[i - 1, j];
                        }
                        //Check right edge of image (cannot average)
                        else if (j + 1 > transformedPic.GetLength(1) - 1)
                        {
                            transformedPic[i, j] = transformedPic[i, j - 1];
                        }
                        //For all other cases, average pixel colours to the left and right
                        else
                        {
                            Red = (transformedPic[i, j - 1].R + transformedPic[i, j + 1].R) / 2 + 0.5;
                            Blue = (transformedPic[i, j - 1].B + transformedPic[i, j + 1].B) / 2 + 0.5;
                            Green = (transformedPic[i, j - 1].G + transformedPic[i, j + 1].G) / 2 + 0.5;

                            //Recombine colour based on new values
                            transformedPic[i, j] = Color.FromArgb((int)Red, (int)Green, (int)Blue);
                        }
                    }
                }

                //Now loop empty pixels in odd rows (whole row empty)
                for (int i = 1; i < transformedPic.GetLength(0); i += 2)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        //Check bottom edge of image (cannot average)
                        if (i + 1 > transformedPic.GetLength(0) - 1)
                        {
                            transformedPic[i, j] = transformedPic[i - 1, j];
                        }
                        //Check right edge of image (cannot average)
                        else if (j + 1 > transformedPic.GetLength(1) - 1)
                        {
                            transformedPic[i, j] = transformedPic[i, j - 1];
                        }
                        //For all other cases, average pixel colours to the top and bottom
                        else
                        {
                            Red = (transformedPic[i - 1, j].R + transformedPic[i + 1, j].R) / 2 + 0.5;
                            Blue = (transformedPic[i - 1, j].B + transformedPic[i + 1, j].B) / 2 + 0.5;
                            Green = (transformedPic[i - 1, j].G + transformedPic[i + 1, j].G) / 2 + 0.5;

                            //Recombine colour based on new values
                            transformedPic[i, j] = Color.FromArgb((int)Red, (int)Green, (int)Blue);
                        }
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessRotate_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                //Switch length and width in temporary pic
                Color[,] TempPic = new Color[transformedPic.GetLength(1), transformedPic.GetLength(0)];
                int Length = transformedPic.GetLength(0) - 1;

                //Loop all rows and columns
                for (int i = 0; i < TempPic.GetLength(0); i++)
                {
                    for (int j = 0; j < TempPic.GetLength(1); j++)
                    {
                        TempPic[i, j] = transformedPic[Length - j, i];
                    }
                }

                //Resize transformed pic
                transformedPic = new Color[TempPic.GetLength(0), TempPic.GetLength(1)];

                //Copy temporary pic back into transformed pic
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        transformedPic[i, j] = TempPic[i, j];
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessRotateLeft_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                //Switch length and width in temporary pic
                Color[,] TempPic = new Color[transformedPic.GetLength(1), transformedPic.GetLength(0)];
                int Length = transformedPic.GetLength(1) - 1;

                //Loop all rows and columns
                for (int i = 0; i < TempPic.GetLength(0); i++)
                {
                    for (int j = 0; j < TempPic.GetLength(1); j++)
                    {
                        TempPic[i, j] = transformedPic[j, Length - i];
                    }
                }

                //Resize transformed pic
                transformedPic = new Color[TempPic.GetLength(0), TempPic.GetLength(1)];

                //Copy temporary pic back into transformed pic
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        transformedPic[i, j] = TempPic[i, j];
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void mnuProcessBlur_Click(object sender, EventArgs e)
        {
            if (transformedPic != null)
            {
                //Blur by getting average of pixels to the left, right, top, bottom
                //Use double in order to round colour properly
                double Red, Green, Blue;
                int PixelsAveraged;
                int Height = transformedPic.GetLength(0) - 1;
                int Width = transformedPic.GetLength(1) - 1;

                //Copy colours over to temp
                Color[,] TempPic = new Color[Height + 1, Width + 1];
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        TempPic[i, j] = transformedPic[i, j];
                    }
                }

                //Check and get averages
                for (int i = 0; i < transformedPic.GetLength(0); i++)
                {
                    for (int j = 0; j < transformedPic.GetLength(1); j++)
                    {
                        Red = 0;
                        Green = 0;
                        Blue = 0;
                        PixelsAveraged = 1;

                        //Get RGB values of current pixel
                        Red = TempPic[i, j].R;
                        Blue = TempPic[i, j].B;
                        Green = TempPic[i, j].G;

                        //Check right of pixel
                        if (j + 1 <= Width)
                        {
                            Red = Red + TempPic[i, j + 1].R;
                            Blue = Blue + TempPic[i, j + 1].B;
                            Green = Green + TempPic[i, j + 1].G;
                            PixelsAveraged++;
                        }
                        //Check bottom of pixel
                        if (i + 1 <= Height)
                        {
                            Red = Red + TempPic[i + 1, j].R;
                            Blue = Blue + TempPic[i + 1, j].B;
                            Green = Green + TempPic[i + 1, j].G;
                            PixelsAveraged++;
                        }
                        //Check left of pixel
                        if (j - 1 >= 0)
                        {
                            Red = Red + TempPic[i, j - 1].R;
                            Blue = Blue + TempPic[i, j - 1].B;
                            Green = Green + TempPic[i, j - 1].G;
                            PixelsAveraged++;
                        }
                        //Check top of pixel
                        if (i - 1 >= 0)
                        {
                            Red = Red + TempPic[i - 1, j].R;
                            Blue = Blue + TempPic[i - 1, j].B;
                            Green = Green + TempPic[i - 1, j].G;
                            PixelsAveraged++;
                        }
                        //Check top left
                        if (i - 1 >= 0 && j - 1 >= 0)
                        {
                            Red = Red + TempPic[i - 1, j - 1].R;
                            Blue = Blue + TempPic[i - 1, j - 1].B;
                            Green = Green + TempPic[i - 1, j - 1].G;
                            PixelsAveraged++;
                        }
                        //Check top right
                        if (i - 1 >= 0 && j + 1 <= Width)
                        {
                            Red = Red + TempPic[i - 1, j + 1].R;
                            Blue = Blue + TempPic[i - 1, j + 1].B;
                            Green = Green + TempPic[i - 1, j + 1].G;
                            PixelsAveraged++;
                        }
                        //Check bottom left
                        if (i + 1 <= Height && j - 1 >= 0)
                        {
                            Red = Red + TempPic[i + 1, j - 1].R;
                            Blue = Blue + TempPic[i + 1, j - 1].B;
                            Green = Green + TempPic[i + 1, j - 1].G;
                            PixelsAveraged++;
                        }
                        //Check bottom right
                        if (i + 1 <= Height && j + 1 <= Width)
                        {
                            Red = Red + TempPic[i + 1, j + 1].R;
                            Blue = Blue + TempPic[i + 1, j + 1].B;
                            Green = Green + TempPic[i + 1, j + 1].G;
                            PixelsAveraged++;
                        }

                        //Get averages and add 0.5 to round properly
                        //Otherwise, the image gradually becomes darker
                        Red = Red / PixelsAveraged + 0.5;
                        Blue = Blue / PixelsAveraged + 0.5;
                        Green = Green / PixelsAveraged + 0.5;

                        //Recombine colour based on new values
                        transformedPic[i, j] = Color.FromArgb((int)Red, (int)Green, (int)Blue);
                    }
                }

                //Redraw
                this.Refresh();
            }
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            //Hotkeys
            if (e.KeyCode == Keys.D)
            {
                mnuProcessDarken.PerformClick();
            }
            else if (e.KeyCode == Keys.R)
            {
                mnuProcessReset.PerformClick();
            }
            else if (e.KeyCode == Keys.I)
            {
                mnuProcessInvert.PerformClick();
            }
            else if (e.KeyCode == Keys.W)
            {
                mnuProcessWhiten.PerformClick();
            }
            else if (e.KeyCode == Keys.X)
            {
                mnuProcessFlipX.PerformClick();
            }
            else if (e.KeyCode == Keys.Y)
            {
                mnuProcessFlipY.PerformClick();
            }
            else if (e.KeyCode == Keys.H)
            {
                mnuProcessMirrorH.PerformClick();
            }
            else if (e.KeyCode == Keys.V)
            {
                mnuProcessMirrorV.PerformClick();
            }
            else if (e.KeyCode == Keys.F)
            {
                mnuProcessScale50.PerformClick();
            }
            else if (e.KeyCode == Keys.T)
            {
                mnuProcessScale200.PerformClick();
            }
            else if (e.KeyCode == Keys.N)
            {
                mnuProcessRotate.PerformClick();
            }
            else if (e.KeyCode == Keys.L)
            {
                mnuProcessRotateLeft.PerformClick();
            }
            else if (e.KeyCode == Keys.B)
            {
                mnuProcessBlur.PerformClick();
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}

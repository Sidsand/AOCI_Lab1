using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Лабораторная_1
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> sourceImage; //глобальная переменная
        private VideoCapture capture;
        int frameCounter = 0;
        double cannyThreshold=20.0;
        double cannyThresholdLinking=10.0;
        byte color1;
        byte color2;
        byte color3;
        byte color4;

        public Form1()
        {
            InitializeComponent();
        }

        private void load_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла
            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;
                sourceImage = new Image<Bgr, byte>(fileName).Resize(640, 480, Inter.Linear);

                imageBox1.Image = sourceImage;
                panel4.Visible = true;
            }
        }

        private void WebC_Click(object sender, EventArgs e)
        {
            //panel4.Visible = false;
            // инициализация веб-камеры
            capture = new VideoCapture();
            capture.ImageGrabbed += ProcessFrame;
            capture.Start(); // начало обработки видеопотока
        }

        // захват кадра из видеопотока
        private void ProcessFrame(object sender, EventArgs e)
        {
            
            var frame = new Mat();
            capture.Retrieve(frame); // получение текущего кадра
            imageBox1.Image = frame;

            Image<Bgr, byte> sourceImage = frame.ToImage<Bgr, byte>();

            Image<Gray, byte> grayImage = sourceImage.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrUp();

            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);



            if (checkBox2.Checked == true)
            {
                //cannyEdges._Dilate(1);

                var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
                var resultImage = sourceImage.Sub(cannyEdgesBgr); // попиксельное вычитание

                //обход по каналам
                for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                    for (int x = 0; x < resultImage.Width; x++)
                        for (int y = 0; y < resultImage.Height; y++) // обход по пискелям
                        {
                            // получение цвета пикселя
                            byte color = resultImage.Data[y, x, channel];
                            if (color <= 64)
                                color = color1;
                            else if (color <= 128)
                                color = color2;
                            else if (color <= 192)
                                color = color3;
                            else
                                color = color4;
                            resultImage.Data[y, x, channel] = color; // изменение цвета пикселя
                        }

                imageBox2.Image = resultImage;
            }
            else { imageBox2.Image = cannyEdges; }


        }
     
        private void Video_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла
            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;
                
                capture = new VideoCapture(fileName);
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var frame = capture.QueryFrame();
            imageBox1.Image = frame;


            Image<Bgr, byte> sourceImage = frame.ToImage<Bgr, byte>();

            Image<Gray, byte> grayImage = sourceImage.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrUp();

            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);



            if (checkBox3.Checked == true)
            {
                //cannyEdges._Dilate(1);

                var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
                var resultImage = sourceImage.Sub(cannyEdgesBgr); // попиксельное вычитание

                //обход по каналам
                for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                    for (int x = 0; x < resultImage.Width; x++)
                        for (int y = 0; y < resultImage.Height; y++) // обход по пискелям
                        {
                            // получение цвета пикселя
                            byte color = resultImage.Data[y, x, channel];
                            if (color <= 64)
                                color = color1;
                            else if (color <= 128)
                                color = color2;
                            else if (color <= 192)
                                color = color3;
                            else
                                color = color4;
                            resultImage.Data[y, x, channel] = color; // изменение цвета пикселя
                        }

                imageBox2.Image = resultImage;
            }
            else { imageBox2.Image = cannyEdges; }

            frameCounter++;
            if (frameCounter >= capture.GetCaptureProperty(CapProp.FrameCount))
            {
                timer1.Enabled = false;
            
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            capture.Stop(); // остановка обработки видеопотока
            imageBox1.Image = null;
            imageBox2.Image = null;
        }

        

        public void trackBar1_Scroll(object sender, EventArgs e)
        {
            cannyThreshold = Convert.ToDouble(trackBar1.Value);
            imageBox2.Image = process(sourceImage);
            if (checkBox1.Checked == true)
            {
                imageBox2.Image = processBGR(sourceImage);
            }
        }

        public void trackBar2_Scroll(object sender, EventArgs e)
        {
            cannyThresholdLinking = Convert.ToDouble(trackBar2.Value);
            imageBox2.Image = process(sourceImage);
            if (checkBox1.Checked == true)
            {
                imageBox2.Image = processBGR(sourceImage);
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            color1 = Convert.ToByte(trackBar3.Value);
            imageBox2.Image = processBGR(sourceImage);
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            color2 = Convert.ToByte(trackBar4.Value);
            imageBox2.Image = processBGR(sourceImage);
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            color3 = Convert.ToByte(trackBar5.Value);
            imageBox2.Image = processBGR(sourceImage);
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            color4 = Convert.ToByte(trackBar6.Value);
            imageBox2.Image = processBGR(sourceImage);
        }

        private Image<Gray, byte> process(Image<Bgr, byte> sourceImage)
        {
            //var frame = new Mat();
            //capture.Retrieve(frame); // получение текущего кадра
            //imageBox1.Image = frame;

            //sourceImage = frame.ToImage<Bgr, byte>();

            Image<Gray, byte> grayImage = sourceImage.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrUp();

            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);
            
            return (cannyEdges.Resize(640, 480, Inter.Linear));
        }

        private Image<Bgr, byte> processBGR(Image<Bgr, byte> sourceImage)
        {
            var frame = new Mat();
            capture.Retrieve(frame); // получение текущего кадра
            imageBox1.Image = frame;

            sourceImage = frame.ToImage<Bgr, byte>();

            Image<Gray, byte> grayImage = sourceImage.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrUp();


            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);
            //cannyEdges._Dilate(1);

            var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
            var resultImage = sourceImage.Sub(cannyEdgesBgr); // попиксельное вычитание

            //обход по каналам
            for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                for (int x = 0; x < resultImage.Width; x++)
                    for (int y = 0; y < resultImage.Height; y++) // обход по пискелям
                    {
                        // получение цвета пикселя
                        byte color = resultImage.Data[y, x, channel];
                        if (color <= 64)
                            color = color1;
                        else if (color <= 128)
                            color = color2;
                        else if (color <= 192)
                            color = color3;
                        else
                            color = color4;
                        resultImage.Data[y, x, channel] = color; // изменение цвета пикселя
                    }

            return (resultImage.Resize(640, 480, Inter.Linear));
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Image<Gray, byte> grayImage = sourceImage.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrUp();

            cannyThreshold = trackBar1.Value;
            cannyThresholdLinking = trackBar2.Value;

            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);
            imageBox2.Image = cannyEdges;

            if (checkBox1.Checked == true)
            {
                //cannyEdges._Dilate(1);

                var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
                var resultImage = sourceImage.Sub(cannyEdgesBgr); // попиксельное вычитание

                //обход по каналам
                for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                    for (int x = 0; x < resultImage.Width; x++)
                        for (int y = 0; y < resultImage.Height; y++) // обход по пискелям
                        {
                            // получение цвета пикселя
                            byte color = resultImage.Data[y, x, channel];
                            if (color <= 64)
                                color = color1;
                            else if (color <= 128)
                                color = color2;
                            else if (color <= 192)
                                color = color3;
                            else
                                color = color4;
                            resultImage.Data[y, x, channel] = color; // изменение цвета пикселя
                        }

                imageBox2.Image = resultImage;
            }
        }

        
    }
}

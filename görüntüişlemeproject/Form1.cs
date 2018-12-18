using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Ports;  // arduino için input-output
using AForge;
using AForge.Video;    // new frame için
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Vision;
using AForge.Vision.Motion;
using AForge.Math.Geometry;
using Point = System.Drawing.Point;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection tumkameralar;  //bilgisayarda ki tüm kameralar dizi olur
        private VideoCaptureDevice kamera;            //kullanılan kamera
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            kamera = new VideoCaptureDevice(tumkameralar[camBox.SelectedIndex].MonikerString); //tüm kameralar açılıyor(liste olarak) ve kullanılan kemara seçilir.

            kamera.NewFrame += new NewFrameEventHandler(kamera_NewFrame); //ekranda çıkan tüm yeni karelerin hepsi işlenir(gerçek zamanlı), çıkan karede işlem yapabiliriz.

            kamera.DesiredFrameRate = 20;
            kamera.DesiredFrameSize = new Size(350, 240);
            kamera.Start();
        }
        void kamera_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone(); //2 görüntü oluşturdum.İkiside aynı görüntü.
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone(); //image1 radio button da kullandım.
            kaynakBox.Image = image; //ilk görüntüyü kaynak box a atadım.


            if (radioButton1.Checked)
            {
                EuclideanColorFiltering filter = new EuclideanColorFiltering();  //filtre oluşturdum.
                filter.CenterColor = new RGB(Color.FromArgb(215, 25, 25));
                filter.Radius = 100; //radius çevresi içerisi boyunca kırmızı algılanacak
                filter.ApplyInPlace(image1);   //image1 görüntü üzerine filtre uyguladım. Arka plan siyah oldu
                nesnebul(image1);
            }
            if (radioButton2.Checked)
            {
                EuclideanColorFiltering filtre = new EuclideanColorFiltering();

                filtre.CenterColor = new RGB(Color.FromArgb(30, 215, 30));
                filtre.Radius = 100;

                filtre.ApplyInPlace(image1);

                nesnebul(image1); //görüntünün işlenmiş halini nesne bul a gönderdim.

            }
            if (radioButton3.Checked)
            {
                EuclideanColorFiltering filtre = new EuclideanColorFiltering();

                filtre.CenterColor = new RGB(Color.FromArgb(30, 144, 255));
                filtre.Radius = 100;

                filtre.ApplyInPlace(image1);

                nesnebul(image1);
            }
        }
        public void nesnebul(Bitmap image1) //işlenmiş görüntü
        {

            BlobCounter blobCounter = new BlobCounter();  // obje say. Görüntü işlemede blobları kullanarak şekil tespiti sağlanabilir.
            blobCounter.MinWidth = 5;  //ne kadar küçük olursa hassasiyet artar. çerçevenin içine alır
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true; //filtre uygula
            blobCounter.ProcessImage(image1);  //Blob bir resmin içinde bulunan görüntü parçalarıdır.Blob counter'ı image1 yani işlenmiş görüntü üzerine uyguladım.tekrar işledim.
            Rectangle[] rects = blobCounter.GetObjectsRectangles();  //oluşan her bir kareden bilgi alıp,diziye attım.


            islemBox.Image = image1;  //işlenmiş son görüntüyü işlem box a attım


            foreach (Rectangle recs in rects)
            {

                if (rects.Length > 0) //karelerden bilgiler alınmışsa (eğer)
                {
                    Rectangle objectRect = rects[0]; //ilk yakalanan kare obje oldu. 

                    Graphics g = kaynakBox.CreateGraphics(); //Kontrol için bir Grafik nesnesi oluşturma

                    using (Pen pen = new Pen(Color.FromArgb(250, 0, 0), 2)) //kaynakbox ta objeyi çizen diktörtgenin rengi ve kalınlığı 
                    {
                        g.DrawRectangle(pen, objectRect);
                    }

                    int objectX = objectRect.X + (objectRect.Width / 2);  //yeniden boyutlandırma için
                    int objectY = objectRect.Y + (objectRect.Height / 2);

                    g.Dispose(); //kontrolde kalması için  yaptım





                    if (objectX < 106 && objectY < 91) //320:275 lik box boyutum bu nedenle 9 kutu için yaklaşık değer verdim
                    {

                        serialPort1.Write("1");    

                    }
                    else if ((objectX > 106 && objectX < 212) && (objectY < 91))
                    {

                        serialPort1.Write("2");
                    }
                    else if ((objectX > 212 && objectX < 320) && (objectY < 91))
                    {

                        serialPort1.Write("3");
                    }
                    else if ((objectX < 106) && (objectY > 91 && objectY < 182))
                    {

                        serialPort1.Write("4");
                    }
                    else if ((objectX > 106 && objectX < 212) && (objectY > 91 && objectY < 182))
                    {

                        serialPort1.Write("5");
                    }
                    else if ((objectX > 212 && objectX < 320) && (objectY > 91 && objectY < 182))
                    {

                        serialPort1.Write("6");
                    }
                    else if ((objectX < 106) && (objectY > 182 && objectY < 275))
                    {

                        serialPort1.Write("7");
                    }
                    else if ((objectX > 106 && objectX < 212) && (objectY > 182 && objectY < 275))
                    {

                        serialPort1.Write("8");
                    }
                    else if ((objectX > 212 && objectX < 320) && (objectY > 182 && objectY < 275))
                    {

                        serialPort1.Write("9");
                    }


                }
            }
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void camBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void butonabaglan_Click(object sender, EventArgs e)
        {
            //arduino için:
            serialPort1.PortName = portBox.SelectedItem.ToString(); //port box ta seçilen string benim port'umdur.
            serialPort1.BaudRate = 9600; //haberleşme hızı
            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                toolStripLabel1.Text = portBox.SelectedItem.ToString() + "portuna bağlandı"; //string buraya geldi.(serial port name)

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            portBox.DataSource = SerialPort.GetPortNames();  //arduino'mdaki tüm port boxlar listelendi
            int sayi = portBox.Items.Count;

            if (sayi == 0)
            {
                toolStripLabel1.Text = "Port Bulunamadı.Kontrol et!";
                portBox.Enabled = false;
                butonabaglan.Enabled = false; //aktif olmadı
            }

            else
            {

                toolStripLabel1.Text = sayi + "Tane Port var";
            }


            tumkameralar = new FilterInfoCollection(FilterCategory.VideoInputDevice); //bilgisayarımdaki tüm kameralar toplnıp değişkene atandı

            foreach (FilterInfo VideoCaptureDevice in tumkameralar)
            {

                camBox.Items.Add(VideoCaptureDevice.Name); //liste halinde gösterildi

            }

            camBox.SelectedIndex = 0; //ilk ekran açılınca kutu boş gözüküyor , biz seçiyoruz, bunu yazmazsak.

        }

    }
}

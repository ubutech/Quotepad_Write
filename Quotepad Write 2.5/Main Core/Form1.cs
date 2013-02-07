using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Ink;

namespace Quotepad_Write_2._5
{

    public enum ApplicationMode
    {
        Ink,
        StrokeErase
    }

    public partial class Form1 : Form
    {
        private InkCollector myInkCollector = null;

        private const float HitTestRadius = 30;
        private const int StrokePointRadius = 3;
        private const int XPacketIndex = 0;
        private const int YPacketIndex = 1;
        ApplicationMode mode = ApplicationMode.Ink;

        public Form1()
        {
            InitializeComponent();

            myInkCollector = new InkCollector(this.Handle);
            myInkCollector.DefaultDrawingAttributes.Color = Color.Black;
            myInkCollector.DefaultDrawingAttributes.Width = trackBar1.Value;
            myInkCollector.Enabled = true;
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        private bool KeyIsDown(Keys key)
        {
            return (GetAsyncKeyState(key) < 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gradientPanel2.Visible = false;
            gradientPanel3.Visible = false;
        }

        private void gradientNavigationButton5_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                myInkCollector.DefaultDrawingAttributes.Color = colorDialog1.Color;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            myInkCollector.DefaultDrawingAttributes.Width = trackBar1.Value;
        }

        private void gradientNavigationButton6_Click(object sender, EventArgs e)
        {
            myInkCollector.Enabled = false;
            this.Dispose();
        }

        private void gradientNavigationButton4_Click(object sender, EventArgs e)
        {
            if (gradientPanel3.Visible == true)
            {
                gradientPanel2.Visible = false;
            }
            else
            {
                if (gradientPanel2.Visible == false)
                {
                    gradientPanel2.Visible = true;
                    gradientNavigationButton4.StayActiveAfterClick = true;
                }
                else
                {
                    gradientPanel2.Visible = false;
                    gradientNavigationButton4.StayActiveAfterClick = false;
                }
            }
        }

        private void gradientNavigationButton3_Click(object sender, EventArgs e)
        {
            myInkCollector.Ink.DeleteStrokes();
            this.Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (gradientPanel1.Visible == true)
                {
                    gradientPanel2.Visible = false;
                    gradientNavigationButton4.Active = false;
                    gradientPanel3.Visible = false;
                    gradientNavigationButton8.Active = false;
                    gradientPanel1.Visible = false;
                }
                else
                {
                    gradientPanel1.Visible = true;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                
            }
        }

        private void gradientNavigationButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            saveFileDialog1.Title = "Save Ink Component";
            saveFileDialog1.Filter = "Ink Serialized Format files (*.isf)|*.isf|PNG image(*.png)|*.png";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.DefaultExt = "isf";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        String filename = saveFileDialog1.FileName.ToLower();
                        String extensionlessFilename = Path.GetFileNameWithoutExtension(filename);
                        String extension = Path.GetExtension(filename);

                        switch (extension)
                        {
                            case ".isf":
                            default:
                                SaveISF(myStream);
                                break;

                            case ".png":
                                Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                                Graphics graphics = Graphics.FromImage(printscreen as Image);
                                Thread.Sleep(3000);
                                graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
                                printscreen.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                        }
                    }
                }
        }

        private void gradientNavigationButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Title = "Open Ink Component";
            openFileDialog1.Filter = "Ink Serialized Format files (*.isf)|*.isf";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                {
                    String filename = openFileDialog1.FileName.ToLower();
                    String extension = filename.Substring(filename.LastIndexOf('.'));

                        switch (extension)
                        {
                            default:
                                LoadISF(myStream);
                                break;
                        }
                }
            }
        }

        private void SaveISF(Stream s)
        {
            byte[] isf;
            isf = myInkCollector.Ink.Save(PersistenceFormat.InkSerializedFormat);
            s.Write(isf, 0, isf.Length);
        }

        private void LoadISF(Stream s)
        {
            Ink loadedInk = new Ink();
            Byte[] isfBytes = new byte[s.Length];
            if (s.Read(isfBytes, 0, isfBytes.Length) == s.Length)
            {
                loadedInk.Load(isfBytes);
                myInkCollector.Enabled = false;
                s.Close();
            }        

            myInkCollector.Ink = loadedInk;
            myInkCollector.Enabled = true;
            this.Invalidate();
        }

        private void gradientNavigationButton7_Click(object sender, EventArgs e)
        {
            Main_Core.QuotepadInfoBox form2 = new Main_Core.QuotepadInfoBox();
            form2.ShowDialog();
        }

        private void gradientNavigationButton8_Click(object sender, EventArgs e)
        {
            if (gradientPanel2.Visible == true)
            {
                gradientPanel3.Visible = false;
            }
            else
            {
                if (gradientPanel3.Visible == false)
                {
                    gradientPanel3.Visible = true;
                    gradientNavigationButton8.StayActiveAfterClick = true;
                }
                else
                {
                    gradientPanel3.Visible = false;
                    gradientNavigationButton8.StayActiveAfterClick = false;
                }
            }
        }

        private void gradientNavigationButton9_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog2.Title = "Choose your Background Image";
            openFileDialog2.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";

            try
            {
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    this.BackgroundImage = new Bitmap(openFileDialog2.FileName);
                    this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed to load backgroundimage");
            }
        }

        private void gradientNavigationButton10_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                this.BackColor = colorDialog2.Color;
                this.BackgroundImage = null;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyIsDown(Keys.S))
            {
                gradientNavigationButton2_Click(null, null);
            }

            if (KeyIsDown(Keys.O))
            {
                gradientNavigationButton1_Click(null, null);
            }
        }
    
    }
}

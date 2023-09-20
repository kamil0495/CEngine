using CEngine.Files.BMD;
using CEngine.Files.PCX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CEngine.Files.BMD.BmdFile;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace BmdEdit
{
    public partial class Form1 : Form
    {
        List<Button> buttonList;
        private Random rnd = new Random();
        private string CurrentPath="";
        private BmdFile file;
        bool inchange=false;
        public Form1()
        {
            InitializeComponent();
        }


        private void zapiszJakoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog oSaveFileDialog = new SaveFileDialog();
            oSaveFileDialog.Title = "Zapisz plik BMD";
            oSaveFileDialog.Filter = "BMD files|*.bmd";
            if (oSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    file.CreateRawBMD();
                    file.CreateSemiBMD(GetPalletes(), false);
                    file.Save(oSaveFileDialog.FileName);
                    WczytajBMD(oSaveFileDialog.FileName);
                    MessageBox.Show("Zapisano pomyślnie");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                

            }
            
        }

        private void zapiszToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            file.CreateRawBMD();
            file.CreateSemiBMD(GetPalletes(),false);
            file.Save(CurrentPath);
            WczytajBMD(CurrentPath);
        }

        //nowy
        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            file = new BmdFile();
            file.CreateSemiBMD();
        }

        //otworz
        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Otówrz plik BMD";
            theDialog.Filter = "BMD files|*.bmd";
            //theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    WczytajBMD(theDialog.FileName);
                    this.Text = "Bmd editor - " + theDialog.SafeFileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void WczytajBMD(string path)
        {
            listBox1.Items.Clear();
            CurrentPath= path;
            FileStream fileStream = new FileStream(CurrentPath, FileMode.Open);
            RawBmdFile rawBmd = new RawBmdFileReader().read(fileStream);
            file = new BmdFile { rawBmdFile = rawBmd };
            file.CreateSemiBMD(GetPalletes(),checkBox1.Checked);
            //byte[] palette = File.ReadAllBytes(pathPcx);//generateRandomPalette();



            for (int i = 0; i < file.getSize(); i++)
            {
                listBox1.Items.Add(file.semiFile.frames[i]);
            }
        }

        private void oProgramieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new O_programie().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 256; i++)
            {
                buttonList[i].BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonList = new List<Button>();
            int x = 5;
            int y = 0;
            for (int i= 0; i < 256; i++)
            {
                if (i % 8 == 0)
                {
                    x = 5;
                    y += 23;
                }
                Button b = new Button();
                b.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                b.Location = new System.Drawing.Point(x, y);
                b.Size = new System.Drawing.Size(20, 20);
                b.UseVisualStyleBackColor = false;
                b.Tag = i;
                b.Click += B_Click;
                buttonList.Add(b);
                groupBox2.Controls.Add(b);
                x += 23;
            }

            file = new BmdFile();
            file.CreateSemiBMD();
        }

        private void B_Click(object sender, EventArgs e)
        {
            
        }

        private Color[] GetPalletes()
        {
            Color[] p = new Color[256];
            for (int i = 0; i < buttonList.Count; i++)
            {
                p[i] = buttonList[i].BackColor;
            }
            return p;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (file != null) { 
                if (listBox1.SelectedItem != null)
                {
                    file.ChangeColorsSemi(GetPalletes(), checkBox1.Checked);
                    if (((BMDImage)listBox1.SelectedItem).Bitmap!=null)
                    pictureBox1.Image = ((BMDImage)listBox1.SelectedItem).GetBitmap(checkBox2.Checked);
                }
                
            }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                inchange = true;
                
                numericUpDown1.Value= ((BMDImage)listBox1.SelectedItem).x;
                numericUpDown2.Value = ((BMDImage)listBox1.SelectedItem).y;
                comboBox1.Text= ((BMDImage)listBox1.SelectedItem).ToTypeString();
                if (((BMDImage)listBox1.SelectedItem).Bitmap != null) 
                { 
                    pictureBox1.Image = ((BMDImage)listBox1.SelectedItem).GetBitmap(checkBox2.Checked);
                    labelS.Text = "Szerokość: " + ((BMDImage)listBox1.SelectedItem).Bitmap.Width;
                    labelW.Text = "Wysokość: " + ((BMDImage)listBox1.SelectedItem).Bitmap.Height;
                }
                else
                {
                    labelS.Text = "Szerokość: 0";
                    labelW.Text = "Wysokość: 0";
                    pictureBox1.Image = null;
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            inchange = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Otówrz plik pcx";
            theDialog.Filter = "PCX files|*.pcx";
            //theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox1.Text=(theDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FileStream fileStream;
            //textBox1.Text = "C:\\Users\\kamil\\Documents\\LIb\\data\\engine2d\\bin\\palettes\\landscapes\\tree01.pcx";
            //fileStream = new FileStream(textBox1.Text, FileMode.Open);
            try
            {
                byte[] numArray = File.ReadAllBytes(textBox1.Text);
                int index1 = 133;
                for (int index2 = 0; index2 < 256; ++index2)
                {
                    buttonList[index2].BackColor = Color.FromArgb((int)numArray[index1], (int)numArray[index1 + 1], (int)numArray[index1 + 2]);
                    index1 += 3;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message);
            }
            finally
            {
                try
                {
                    //fileStream.Close();
                    if (file!=null)
                    file.ChangeColorsSemi(GetPalletes(), checkBox1.Checked);
                    listBox1.SelectedIndex = listBox1.SelectedIndex;
                }
                catch { }
                
                    
                    
            }
        }

        

        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog oSaveFileDialog = new SaveFileDialog();
            oSaveFileDialog.Title = "Zapisz plik PNG";
            oSaveFileDialog.Filter = "PNG files|*.png";
            if (oSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(oSaveFileDialog.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Otówrz plik png";
            theDialog.Filter = "PNG files|*.png";
            //theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = Image.FromFile(theDialog.FileName);
                    ((BMDImage)listBox1.SelectedItem).Bitmap = new Bitmap(pictureBox1.Image);
                    ((BMDImage)listBox1.SelectedItem).GeneratePixels(GetPalletes());
                    ((BMDImage)listBox1.SelectedItem).GenerateBitmap(GetPalletes(), checkBox1.Checked);
                    pictureBox1.Image = ((BMDImage)listBox1.SelectedItem).GetBitmap(checkBox2.Checked);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (inchange) return;
            if (((BMDImage)listBox1.SelectedItem).x != Convert.ToInt32(numericUpDown1.Value))
            {
                ((BMDImage)listBox1.SelectedItem).x = Convert.ToInt32(numericUpDown1.Value);
                if (checkBox2.Checked) pictureBox1.Image = ((BMDImage)listBox1.SelectedItem).GetBitmap(checkBox2.Checked);
            }
            
            
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (inchange) return;
            if (((BMDImage)listBox1.SelectedItem).y != Convert.ToInt32(numericUpDown2.Value))
            {
                ((BMDImage)listBox1.SelectedItem).y = Convert.ToInt32(numericUpDown2.Value);
                if (checkBox2.Checked) pictureBox1.Image = ((BMDImage)listBox1.SelectedItem).GetBitmap(checkBox2.Checked);
            }
                

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inchange) return;
            ((BMDImage)listBox1.SelectedItem).SetType(comboBox1.Text);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (((BMDImage)listBox1.SelectedItem).Bitmap != null)
                {
                    pictureBox1.Image = ((BMDImage)listBox1.SelectedItem).GetBitmap(checkBox2.Checked);
                }
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Otówrz plik png";
            theDialog.Filter = "PNG files|*.png";
            //theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    
                    BMDImage bi = new BMDImage
                    {
                        Bitmap= new Bitmap(Image.FromFile(theDialog.FileName)),
                        x=0,
                        y=0,
                        Number=listBox1.Items.Count+1,
                        Type=4
                    };
                    bi.GeneratePixels(GetPalletes());
                    bi.GenerateBitmap(GetPalletes(), checkBox1.Checked);
                    file.semiFile.frames.Add(bi);
                    listBox1.SelectedIndex=listBox1.Items.Add(bi);

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Otówrz pliki png";
            theDialog.Filter = "PNG files|*.png";
            theDialog.Multiselect = true;
            //theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                foreach(string FileName in theDialog.FileNames)
                {
                    try
                    {

                        BMDImage bi = new BMDImage
                        {
                            Bitmap = new Bitmap(Image.FromFile(FileName)),
                            x = 0,
                            y = 0,
                            Number = listBox1.Items.Count + 1,
                            Type = 4
                        };
                        bi.GeneratePixels(GetPalletes());
                        bi.GenerateBitmap(GetPalletes(), checkBox1.Checked);
                        file.semiFile.frames.Add(bi);
                        listBox1.SelectedIndex = listBox1.Items.Add(bi);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }
    }
}

using CEngine.Files.BMD;
using static CEngine.Files.BMD.BmdFile;

namespace CulturesEdit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string pathBmd = "C:\\Users\\kamil\\Documents\\LIb\\data\\engine2d\\bin\\bobs\\ls_ground_s.bmd";
            string pathPcx = "C:\\Users\\kamil\\Documents\\LIb\\data\\engine2d\\bin\\palettes\\landscapes\\tree01.pcx";

            FileStream fileStream = new FileStream(pathBmd, FileMode.Open);            
            RawBmdFile rawBmd = new RawBmdFileReader().read(fileStream);
            BmdFile bmd = new BmdFile { rawBmdFile = rawBmd };

            byte[] palette = File.ReadAllBytes(pathPcx);//generateRandomPalette();

            for (int i = 0; i < bmd.getSize(); i++)
            {
                Bitmap img = bmd.getFrame(i, palette, Type4AlphaInterpretation.ALPHA);
                if (img != null)
                {
                    img.Save("/test" + i + ".png");
                }
            }

        }
    }
}
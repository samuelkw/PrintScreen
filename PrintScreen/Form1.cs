using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PrintScreen
{
    public partial class frmPrincipal : Form
    {

        //[DllImport("User32.dll")]
        //public static extern IntPtr GetDC(IntPtr hwnd);
        //[DllImport("User32.dll")]
        //public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        //public static IntPtr desktopPtr;

        public static DirectoryInfo dirDest;
        public static DirectoryInfo subdir;
        public static FileInfo arqImg;
        public static FileInfo arqDest;
        public static DirectoryInfo dirList;
        public static String[] lista;
        public static FileInfo arqList;
        public static int idx = 0;
        public static int tam = 0;

        public static Rectangle resolucao;
        public static int altura;
        public static int largura;


        public frmPrincipal()
        {
            InitializeComponent();
            initAppConfigReader();
            //desktopPtr = GetDC(IntPtr.Zero);
            lblMsg.Text = "";
            resolucao = Screen.PrimaryScreen.Bounds;
            altura = resolucao.Size.Height;
            largura = resolucao.Size.Width;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "Gerando PrintScreen";
            txtList.Enabled = false;
            btnGo.Enabled = false;
            btnAvancar.Enabled = false;
            btnVoltar.Enabled = false;
            String strDir = dirDest.FullName;
            String strArq="";
            if (!strDir.EndsWith("\\")) strDir += "\\";
            strDir += txtList.Text;
            strArq = txtList.Text;
            subdir = new DirectoryInfo(strDir);
            if (!subdir.Exists) subdir.Create();
            if (!strDir.EndsWith("\\")) strDir += "\\";
            arqImg = new FileInfo(strDir+strArq+".jpg");
            int count=0;
            while (arqImg.Exists)
            {
                arqImg = new FileInfo(strDir+strArq+"_"+count+".jpg");
                count++;
            }
            if (prntScr(arqImg))
            {
                lblMsg.Text = "PrintScreen gerado em: "+ arqImg.FullName;
            }
            else
            {
                lblMsg.Text = "Erro ao tentar gerar PrintScreen!";
            }
            txtList.Enabled = true;
            btnGo.Enabled = true;
            btnAvancar.Enabled = true;
            btnVoltar.Enabled = true;
        }

        private void btnAvancar_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            txtList.Enabled = false;
            btnGo.Enabled = false;
            btnAvancar.Enabled = false;
            btnVoltar.Enabled = false;
            nextIdx();
            txtList.Enabled = true;
            btnGo.Enabled = true;
            btnAvancar.Enabled = true;
            btnVoltar.Enabled = true;
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            txtList.Enabled = false;
            btnGo.Enabled = false;
            btnAvancar.Enabled = false;
            btnVoltar.Enabled = false;
            prevIdx();
            txtList.Enabled = true;
            btnGo.Enabled = true;
            btnAvancar.Enabled = true;
            btnVoltar.Enabled = true;
        }

        public void initAppConfigReader()
        {
            System.Configuration.AppSettingsReader appCfg = new System.Configuration.AppSettingsReader();
            try
            {
                String dir0 = appCfg.GetValue("pathDest", typeof(String)).ToString();
                String dir1 = appCfg.GetValue("dirDest", typeof(String)).ToString();
                if (!dir0.EndsWith("\\")) dir0 = dir0 + "\\";
                dirDest = new DirectoryInfo(dir0+dir1);
                if (!dirDest.Exists) dirDest.Create();
            }
            catch { System.Environment.Exit(1); }
            try
            {
                String dir = appCfg.GetValue("pathList", typeof(String)).ToString();
                String arq = appCfg.GetValue("arqList", typeof(String)).ToString();
                Application.DoEvents();
                dirList = new DirectoryInfo(dir);
                if (!dir.EndsWith("\\")) dir = dir + "\\";
                arqList = new FileInfo(dir + arq);
                if (!arqList.Exists) { System.Environment.Exit(1); }
                Application.DoEvents();
                lista = System.IO.File.ReadAllLines(arqList.FullName);
                //System.Threading.Thread.Sleep(2 * 1000);
                Application.DoEvents();
                tam = lista.Length;
                txtList.Text = lista[0];
            }
            catch { System.Environment.Exit(1); }
        }

        public void nextIdx()
        {
            idx++;
            if (idx >= tam)
            {
                lblMsg.Text = "Fim da lista, reiniciando a listagem...";
                idx = 0;
            }
            txtList.Text = lista[idx];
        }

        public void prevIdx()
        {
            idx--;
            if (idx <= 0)
            {
                idx = tam-1;
                lblMsg.Text = "Indo para o último registro da listagem...";
            }
            txtList.Text = lista[idx];
        }

        public bool prntScr(FileInfo arquivo)
        {
            try
            {
                Bitmap printscreen = new Bitmap(largura, altura);
                //Bitmap como imagem
                Graphics graphics = Graphics.FromImage(printscreen as Image);
                //Pos inicial do bitmap
                int esquerda = 0;
                int topo = 0;
                /*Associção da imagem Bitmap ao objecto graphics, com a informação de onde começa a imagem, e qual tamanho que terá */
                graphics.CopyFromScreen(esquerda, topo, 0, 0, printscreen.Size);
                //System.Threading.Thread.Sleep(3 * 1000);
                Application.DoEvents();
                //Caminho aonde saltar, o nome da imagem, a extensão e o formato
                //System.Threading.Thread.Sleep(3 * 1000);
                printscreen.Save(arquivo.FullName, ImageFormat.Jpeg);
                System.Threading.Thread.Sleep(3 * 1000);
                return true;
            }
            catch { return false; }
        }



    }
}

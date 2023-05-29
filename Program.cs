using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.IO.Pipes;

namespace ImageViewer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // 设置高 DPI 缩放模式
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
            }
            Application.Run(new MainForm());
        }

        [DllImport("SHCore.dll", SetLastError = true)]
        private static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }
    }

    public class MainForm : Form
    {
        private PictureBox pictureBox;
        private System.Windows.Forms.Button Button_L1;
        private System.Windows.Forms.Button Button_L2;
        private System.Windows.Forms.Button Button_L3;
        private System.Windows.Forms.Button Button_L4;
        private System.Windows.Forms.Button Button_R1;
        private System.Windows.Forms.Button Button_R2;
        private System.Windows.Forms.Button Button_R3;
        private System.Windows.Forms.Button Button_R4;
        private string currentFolderPath;

        public MainForm()
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();

            Text = "图片分类器";

            this.WindowState = FormWindowState.Maximized;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            pictureBox = new PictureBox();
            pictureBox.Size = new Size(this.Width - Width / 4, this.Height);
            pictureBox.Location = new Point(Width / 8, 0);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //左侧按钮
            //按钮1
            Button_L1 = new System.Windows.Forms.Button();
            Button_L1.Text = "选择\n待分类目录";
            Button_L1.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            Button_L1.Size = new Size(Width / 8, Height / 4);
            Button_L1.Click += new EventHandler(Button_L_1_Click);
            void Button_L_1_Click(object sender, EventArgs e)
            {
                SelectPath(sender, e, 4);
            }
            Button_L1.MouseDown += ShowStatusOnButtonL1RightClick;

            //按钮2
            Button_L2 = new System.Windows.Forms.Button();
            Button_L2.Text = "选择\n目录1路径";
            Button_L2.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            Button_L2.Size = new Size(Width / 8, Height / 4);
            Button_L2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Button_L2.Location = new Point(Button_L2.Width - Width / 8, Height / 4);
            Button_L2.Click += new EventHandler(Button_L_2_Click);
            void Button_L_2_Click(object sender, EventArgs e)
            {
                SelectPath(sender, e, 1);
            }

            //按钮3
            Button_L3 = new System.Windows.Forms.Button();
            Button_L3.Text = "选择\n目录2路径";
            Button_L3.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            Button_L3.Size = new Size(Width / 8, Height / 4);
            Button_L3.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Button_L3.Location = new Point(Button_L3.Width - Width / 8, Height / 2);
            Button_L3.Click += new EventHandler(Button_L_3_Click);
            void Button_L_3_Click(object sender, EventArgs e)
            {
                SelectPath(sender, e, 2);
            }
            //按钮4
            Button_L4 = new System.Windows.Forms.Button();
            Button_L4.Text = "选择\n目录3路径";
            Button_L4.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            Button_L4.Size = new Size(Width / 8, Height / 4);
            Button_L4.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Button_L4.Location = new Point(Button_L4.Width - Width / 8, Height / 4 * 3);
            Button_L4.Click += new EventHandler(Button_L_4_Click);
            void Button_L_4_Click(object sender, EventArgs e)
            {
                SelectPath(sender, e, 3);
            }

            //右侧按钮
            //按钮1
            Button_R1 = new System.Windows.Forms.Button();
            Button_R1.Text = "目录1";
            Button_R1.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            Button_R1.Dock = DockStyle.Right;
            Button_R1.Size = new Size(Width / 8, Height / 4);
            Button_R1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_R1.Location = new Point(this.ClientSize.Width - Button_R1.Width, Height / 4);
            Button_R1.Click += new EventHandler(Button_1_Click);
            void Button_1_Click(object sender, EventArgs e)
            {
                CopyToDir(FolderPath1);
                GetFileCount(4);
                Button_L1.Text = $"待分类目录\n\n总数：{count4}";
                count4 = 0;
                GetFileCount(1);
                Button_R1.Text = $"移动到\n{ShortPath1}\n\n总数：{count1}";
                count1 = 0;
                LoadLatestImage();
            }

            //按钮2
            Button_R2 = new System.Windows.Forms.Button();
            Button_R2.Text = "目录2";
            Button_R2.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            Button_R2.Dock = DockStyle.Right;
            Button_R2.Size = new Size(Width / 8, Height / 4);
            Button_R2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_R2.Location = new Point(this.ClientSize.Width - Button_R2.Width, Height / 2);
            Button_R2.Click += new EventHandler(Button_2_Click);
            void Button_2_Click(object sender, EventArgs e)
            {
                CopyToDir(FolderPath2);
                GetFileCount(4);
                Button_L1.Text = $"待分类目录\n\n总数：{count4}";
                count4 = 0;
                GetFileCount(2);
                Button_R2.Text = $"移动到\n{ShortPath2}\n\n总数：{count2}";
                count2 = 0;
                LoadLatestImage();
            }

            //按钮3
            Button_R3 = new System.Windows.Forms.Button();
            Button_R3.Text = "目录3";
            Button_R3.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            Button_R3.Dock = DockStyle.Right;
            Button_R3.Size = new Size(Width / 8, Height / 4);
            Button_R3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_R3.Location = new Point(this.ClientSize.Width - Button_R3.Width, Height / 4 * 3);
            Button_R3.Click += new EventHandler(Button_3_Click);
            void Button_3_Click(object sender, EventArgs e)
            {
                CopyToDir(FolderPath3);
                GetFileCount(4);
                Button_L1.Text = $"待分类目录\n\n总数：{count4}";
                count4 = 0;
                GetFileCount(3);
                Button_R3.Text = $"移动到\n{ShortPath3}\n\n总数：{count3}";
                count3 = 0;
                LoadLatestImage();
            }

            //按钮4
            Button_R4 = new System.Windows.Forms.Button();
            Button_R4.Text = "删除";
            Button_R4.Font = new Font("微软雅黑", 20, FontStyle.Bold);
            Button_R4.ForeColor = Color.Red;
            Button_R4.Dock = DockStyle.Right;
            Button_R4.Size = new Size(Width / 8, Height / 4);
            Button_R4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_R4.Location = new Point(this.ClientSize.Width - Button_R4.Width, 0);
            Button_R4.Click += new EventHandler(Button_4_Click);
            void Button_4_Click(object sender, EventArgs e)
            {
                MoveToRecycleBin(FilePath);
                GetFileCount(4);
                Button_L1.Text = $"待分类目录\n\n总数：{count4}";
                count4 = 0;
            }

            //添加到窗体
            Controls.Add(Button_L1);
            Controls.Add(Button_L2);
            Controls.Add(Button_L3);
            Controls.Add(Button_L4);
            Controls.Add(Button_R1);
            Controls.Add(Button_R2);
            Controls.Add(Button_R3);
            Controls.Add(Button_R4);
            Controls.Add(pictureBox);

            currentFolderPath = "";
        }

        public static string FilePath;
        public static string FolderPath1;
        public static string FolderPath2;
        public static string FolderPath3;
        public static string ShortPath1;
        public static string ShortPath2;
        public static string ShortPath3;
        public static int count1 = 0;
        public static int count2 = 0;
        public static int count3 = 0;
        public static int count4 = 0;

        public int GetFileCount(short num)
        {
            if (num == 1)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FolderPath1);
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                {
                    count1++;
                }
            }
            else if (num == 2)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FolderPath2);
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                {
                    count2++;
                }
            }
            else if (num == 3)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FolderPath3);
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                {
                    count3++;
                }
            }
            else if (num == 4)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(currentFolderPath);
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                {
                    count4++;
                }
            }
            return 0;
        }

        private void ShowStatusOnButtonL1RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MessageBox.Show($"待分类目录\n{currentFolderPath}\n\n当前文件\n{FilePath}\n\n移动到\n{FolderPath1}\n{FolderPath2}\n{FolderPath3}");
            }
        }

        public void CopyToDir(string Dirpath)
        {
            string fileName = Path.GetFileName(FilePath);
            string newFilePath = Path.Combine(Dirpath, fileName);
            File.Move(FilePath, newFilePath);
            LoadLatestImage();

        }

        public static void GetShortPathName(short value)
        {
            if (value == 1)
            {
                var input = FolderPath1;
                var words = input.Replace('\\', '*').Split('*');
                var result = words.LastOrDefault(w => !string.IsNullOrWhiteSpace(w));
                ShortPath1 = result ?? "";
            }
            else if (value == 2)
            {
                var input = FolderPath2;
                var words = input.Replace('\\', '*').Split('*');
                var result = words.LastOrDefault(w => !string.IsNullOrWhiteSpace(w));
                ShortPath2 = result ?? "";
            }
            else if (value == 3)
            {
                var input = FolderPath3;
                var words = input.Replace('\\', '*').Split('*');
                var result = words.LastOrDefault(w => !string.IsNullOrWhiteSpace(w));
                ShortPath3 = result ?? "";
            }
        }

        private void SelectPath(object sender, EventArgs e, short value)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK && value == 1)
            {
                FolderPath1 = dialog.SelectedPath;
                GetShortPathName(1);
                GetFileCount(1);
                Button_R1.Text = $"移动到\n{ShortPath1}\n\n总数：{count1}";
                count1 = 0;
            }
            else if (result == DialogResult.OK && value == 2)
            {
                FolderPath2 = dialog.SelectedPath;
                GetShortPathName(2);
                GetFileCount(2);
                Button_R2.Text = $"移动到\n{ShortPath2}\n\n总数：{count2}";
                count2 = 0;
            }
            else if (result == DialogResult.OK && value == 3)
            {
                FolderPath3 = dialog.SelectedPath;
                GetShortPathName(3);
                GetFileCount(3);
                Button_R3.Text = $"移动到\n{ShortPath3}\n\n总数：{count3}";
                count3 = 0;
            }
            else if (result == DialogResult.OK && value == 4)
            {
                currentFolderPath = dialog.SelectedPath;
                LoadLatestImage();
                GetFileCount(4);
                Button_L1.Text = $"待分类目录\n\n总数：{count4}";
                count4 = 0;
            }
        }

        public void MoveToRecycleBin(string filePath)
        {
            ReleaseFile(FilePath);
            static void ReleaseFile(string filePath)
            {
                  using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        // 关闭文件流以释放文件句柄，并清理缓冲区
                        fileStream.Close();
                    }
            }
            FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            LoadLatestImage();
        }

        private short invalidFileCount = 1;
        private void LoadLatestImage()
        {
            if (!string.IsNullOrEmpty(currentFolderPath))
            {
                string[] pngFiles = Directory.GetFiles(currentFolderPath, "*.png");
                string[] jpgFiles = Directory.GetFiles(currentFolderPath, "*.jpg");
                string[] imageFiles = pngFiles.Concat(jpgFiles).ToArray(); // 合并成一个数组
                if (imageFiles.Length > 0)
                {
                    Array.Sort(imageFiles);
                    string latestImageFile = imageFiles[imageFiles.Length - invalidFileCount];
                    FilePath = latestImageFile;
                    using (FileStream stream = new FileStream(latestImageFile, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            Image image = Image.FromStream(stream);
                            pictureBox.Image = image;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show($"{FilePath}\n\n为无效的图像文件,已跳过\n\n异常： " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            invalidFileCount++;
                            LoadLatestImage();
                        }
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System;
using Newtonsoft.Json;

namespace BGLeopold
{
    public class EDTDrawer
    {
        private static Vector2 gap = new Vector2(1f, 1f);
        private static Vector2 cellSize = new Vector2(375f, 45f);
        private static Vector2 cellsOffset = new Vector2(37f, 37f);
        private static Font font = new Font("Arial", 11f, FontStyle.Regular);
        private static Font boldFont = new Font("Arial", 11f, FontStyle.Bold);

        private Graphics graphics;
        private ModulesPen modulesPen;

        private Image image;

        public EDTDrawer(string imgPath)
        {
            image = Image.FromFile(imgPath);
            graphics = Graphics.FromImage(image);
            LoadModulesPen();
        }

        public void SaveImage(string filename)
        {
            image.Save(filename);
        }

        private void SaveModulesPen()
        {
            string json = JsonConvert.SerializeObject(modulesPen);
            if(json != "{}")
                File.WriteAllText("colors.json", json);
        }

        private void LoadModulesPen()
        {
            if (!File.Exists("colors.json"))
            {
                modulesPen = new ModulesPen();
                return;
            }
            string json = File.ReadAllText("colors.json");
            modulesPen = (ModulesPen)JsonConvert.DeserializeObject(json, typeof(ModulesPen));
            //modulesPen = new ModulesPen();//uncomment this line to change color
        }

        #region Draw

        private Rectangle CalculateRectangle(in float startLine, in float endLine, in float colummn)
        {
            Point point = new Point((int)(cellsOffset.x + colummn * (cellSize.x + gap.x)), (int)(cellsOffset.y + startLine * (cellSize.y + gap.y)));
            Size size = new Size((int)cellSize.x, (int)((endLine - startLine) * (cellSize.y + gap.y)));
            return new Rectangle(point, size);
        }

        public void DrawModuleRectangle(string moduleName, in float startLine, in float endLine, in float colummn)
        {
            Color color = Color.Red;
            if(!modulesPen.TryGetColor(moduleName, out color))
            {
                int R, G, B;
                do
                {
                    R = Random.Rand(0, 255);
                    G = Random.Rand(0, 255);
                    B = Random.Rand(0, 255);
                } while ((R + G + B) / 3 > 200 || (R + G + B) / 3 < 40);
                color = Color.FromArgb(255, R, G, B);
                modulesPen.AddModuleColor(moduleName, color);
                SaveModulesPen();
            }

            SolidBrush brush = new SolidBrush(color);
            graphics.FillRectangle(brush, CalculateRectangle(startLine, endLine, colummn));
        }

        public void DrawString(string moduleName, string text, in float startLine, in float endLine, in float colummn)
        {
            if (text == null || text == string.Empty)
                return;
            DrawString(moduleName, text, CalculateRectangle(startLine, endLine, colummn));
        }

        private void DrawString(string moduleName, string text, Rectangle rec)
        {
            List<string> tmpSentences = new List<string>();

            StringBuilder sb = new StringBuilder(text.Length == 1 ? text : string.Empty);
            char c;
            for (int i = 0; i < text.Length; i++)
            {
                c = text[i];
                if (c == '\n')
                {
                    string s = sb.ToString();
                    if (s != null && s != string.Empty)
                        tmpSentences.Add(s);
                    sb.Clear();
                    continue;
                }
                sb.Append(c);
            }
            tmpSentences.Add(sb.ToString());

            List<string> sentences = new List<string>();
            int nbLineInBold = 1;
            for (int i = 0; i < tmpSentences.Count; i++)
            {
                string tmpText = tmpSentences[i];
                SizeF textSize = graphics.MeasureString(tmpText, (i == nbLineInBold - 1) ? boldFont : font);
                if(textSize.Width > rec.Width * 0.8f)
                {
                    if (textSize.Width > rec.Width * 1.6f)
                        continue;

                    int min = Math.Max(0, (tmpText.Length / 2) - 2);
                    int middle = -1;
                    for (int j = tmpText.Length - 2; j >=  min; j--)
                    {
                        c = tmpText[j];
                        if(c == ' ' || c == ':' || c == '/' || c == '-')
                        {
                            string s1 = tmpText.Substring(0, j + 1);
                            string s2 = tmpText.Substring(j + 1);
                            float s1W = graphics.MeasureString(s1, (i == nbLineInBold - 1) ? boldFont : font).Width;
                            float s2W = graphics.MeasureString(s2, (i == nbLineInBold - 1) ? boldFont : font).Width;
                            if (!(s1W > rec.Width * 0.8f || s2W > rec.Width * 0.8f))
                            {
                                middle = j;
                                break;
                            }
                        }
                    }
                    if(middle == -1)
                        middle = Math.Max(0, (tmpText.Length / 2) - 1);

                    sentences.Add(tmpText.Substring(0, middle + 1));
                    sentences.Add(tmpText.Substring(middle + 1));
                    if (i == 0)
                        nbLineInBold++;
                }
                else
                {
                    sentences.Add(tmpText);
                }
            }
            tmpSentences.Clear();

            Point center = new Point(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
            float offsetY = -(sentences.Count * graphics.MeasureString(sentences[0], font).Height) * 0.5f - 3f;
            SolidBrush brush = new SolidBrush(Color.Black);

            float addToY = 0f;
            for (int i = 0; i < nbLineInBold; i++)
            {
                graphics.DrawString(sentences[i], boldFont, brush, center.X - graphics.MeasureString(sentences[i], boldFont).Width * 0.5f, center.Y + offsetY + addToY);
                addToY += graphics.MeasureString(sentences[i], boldFont).Height;
            }
            for (int i = nbLineInBold; i < sentences.Count; i++)
            {
                graphics.DrawString(sentences[i], font, brush, center.X - graphics.MeasureString(sentences[i], font).Width * 0.5f, center.Y + offsetY + addToY);
                addToY += graphics.MeasureString(sentences[i], font).Height;
            }
        }

        #endregion

        private class ModulesPen
        {
            public List<string> modulesName { get; set; }
            public List<Color> modulesColors { get; set; }

            public ModulesPen()
            {
                modulesName = new List<string>();
                modulesColors = new List<Color>();
            }

            public bool AddModuleColor(string moduleName, in Color color)
            {
                if(!TryGetColor(moduleName, out Color _))
                {
                    modulesName.Add(moduleName);
                    modulesColors.Add(color);
                    return true;
                }
                return false;
            }

            public bool TryGetColor(string name, out Color color)
            {
                for (int i = 0; i < modulesName.Count; i++)
                {
                    if (EvaluateSameModule(name, modulesName[i]))
                    {
                        color = modulesColors[i];
                        return true;
                    }
                }
                color = default;
                return false;
            }

            private bool EvaluateSameModule(string name1, string name2) => StringDist(name1, name2) < 0.3f;

            private float StringDist(string a, string b)
            {
                int end = Math.Min(a.Length, b.Length);
                float dist = 0f;
                for (int i = 0; i < end; i++)
                {
                    if (a[i] != b[i])
                        dist += 1f;
                }
                dist += Math.Abs(a.Length - b.Length);
                return dist / Math.Max(a.Length, b.Length);
            }
        }
    }
}

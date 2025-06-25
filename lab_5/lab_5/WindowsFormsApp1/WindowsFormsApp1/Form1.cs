using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Button button1;
        private Button button2;
        private Button button3;

        // Перелік режимів: нічого, крива Без’є або фрактал
        private enum DrawMode { None, Bezier, Fractal }
        private DrawMode currentMode = DrawMode.None;

        public Form1()
        {
            InitializeComponent();

            // Ініціалізація кнопок
            button1 = new Button() { Text = "Bezier", Location = new Point(10, 10) };
            button2 = new Button() { Text = "Fractal", Location = new Point(100, 10) };
            button3 = new Button() { Text = "Clear", Location = new Point(190, 10) };

            // Прив'язка подій натискань
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;

            // Додавання кнопок на форму
            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(button3);

            // Подія малювання
            this.Paint += Form1_Paint;

            // Основні властивості форми
            this.Width = 600;
            this.Height = 550;
            this.DoubleBuffered = true; // для зменшення мерехтіння
        }

        // Обробник малювання форми
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (currentMode == DrawMode.Bezier)
            {
                DrawBezier(g); // Малюємо криву Без’є
            }
            else if (currentMode == DrawMode.Fractal)
            {
                DrawSierpinskiCarpet(g, 50, 60, 400, 400, 3); // Малюємо фрактал
            }
        }

        // Обробники кнопок
        private void button1_Click(object sender, EventArgs e)
        {
            currentMode = DrawMode.Bezier;
            this.Invalidate(); // перерисовка
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentMode = DrawMode.Fractal;
            this.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentMode = DrawMode.None;
            this.Invalidate();
        }

        // Малювання кривої Без’є з поступовою зміною кольору та товщини
        private void DrawBezier(Graphics g)
        {
            // Вихідні контрольні точки кривої Без’є
            PointF P1 = new PointF(100, 400);
            PointF P2 = new PointF(200, 100);
            PointF P3 = new PointF(300, 100);
            PointF P4 = new PointF(400, 400);

            List<PointF> points = new List<PointF>();

            // Обчислюємо точки кривої
            for (float t = 0; t <= 1.0f; t += 0.01f)
            {
                float x = (float)(
                    Math.Pow(1 - t, 3) * P1.X +
                    3 * Math.Pow(1 - t, 2) * t * P2.X +
                    3 * (1 - t) * t * t * P3.X +
                    t * t * t * P4.X
                );

                float y = (float)(
                    Math.Pow(1 - t, 3) * P1.Y +
                    3 * Math.Pow(1 - t, 2) * t * P2.Y +
                    3 * (1 - t) * t * t * P3.Y +
                    t * t * t * P4.Y
                );

                points.Add(new PointF(x, y));
            }

            // Малюємо лінії між точками з поступовою зміною кольору і товщини
            for (int i = 0; i < points.Count - 1; i++)
            {
                // Колір градієнта — від синього до червоного
                int r = (int)(255 * i / (float)points.Count);
                int b = 255 - r;
                Color color = Color.FromArgb(r, 0, b);

                // Товщина лінії змінюється від 1 до 3
                float thickness = 1 + 2 * (i / (float)points.Count);

                using (Pen pen = new Pen(color, thickness))
                {
                    g.DrawLine(pen, points[i], points[i + 1]);
                }
            }
        }

        // Рекурсивне малювання фрактала "Килим Серпінського"
        private void DrawSierpinskiCarpet(Graphics g, float x, float y, float width, float height, int order)
        {
            if (order == 0)
            {
                g.FillRectangle(Brushes.Black, x, y, width, height);
                return;
            }

            float w = width / 3f;
            float h = height / 3f;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    bool isCenter = (i == 1 && j == 1); // пропускаємо центр

                    if (isCenter)
                        continue;

                    // Рекурсивно малюємо підпрямокутки
                    DrawSierpinskiCarpet(g, x + i * w, y + j * h, w, h, order - 1);
                }
            }
        }
    }
}

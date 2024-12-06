using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class GraphForm : Form
{
    public GraphForm()
    {
        this.Text = "Графік функції";
        this.BackColor = Color.White;
        this.Size = new Size(800, 600);
        this.Resize += (sender, e) => this.Invalidate(); // Перемальовуємо графік при зміні розміру вікна
        this.Paint += DrawGraph;
    }

    // Обчислення значення функції
    private float CalculateY(float x)
    {
        // Обробка крайового випадку для x = 0
        if (x == 0) return float.NaN; // Повертаємо NaN для x = 0
        return (float)(Math.Pow(Math.Cos(x), 2) / (Math.Pow(x, 2) + 1));
    }

    private void DrawGraph(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Встановлення параметрів графіка
        float xStart = 3.8f; // Початкове значення x
        float xEnd = 7.6f;   // Кінцеве значення x
        float step = 0.6f;   // Крок для розрахунку функції

        // Розміри форми
        int width = this.ClientSize.Width;
        int height = this.ClientSize.Height;

        // Поля відступу
        int padding = 50;

        // Розміри області для графіка
        RectangleF graphArea = new RectangleF(padding, padding, width - 2 * padding, height - 2 * padding);

        // Малюємо осі координат
        Pen axisPen = new Pen(Color.Black, 2);
        g.DrawLine(axisPen, graphArea.Left, graphArea.Top + graphArea.Height / 2, graphArea.Right, graphArea.Top + graphArea.Height / 2); // X-вісь
        g.DrawLine(axisPen, graphArea.Left + graphArea.Width / 2, graphArea.Top, graphArea.Left + graphArea.Width / 2, graphArea.Bottom); // Y-вісь

        // Функція для масштабування координат
        Func<float, float, float, float> scaleX = (x, xMinVal, xMaxVal) =>
            graphArea.Left + (x - xMinVal) / (xMaxVal - xMinVal) * graphArea.Width;

        Func<float, float, float, float> scaleY = (y, yMinVal, yMaxVal) =>
            graphArea.Bottom - (y - yMinVal) / (yMaxVal - yMinVal) * graphArea.Height;

        // Визначення мінімальних і максимальних значень функції для масштабування
        float yMin = float.MaxValue;
        float yMax = float.MinValue;

        for (float x = xStart; x <= xEnd; x += step)
        {
            float y = CalculateY(x);
            if (!float.IsNaN(y))
            {
                if (y < yMin) yMin = y;
                if (y > yMax) yMax = y;
            }
        }

        // Малювання графіка
        Pen graphPen = new Pen(Color.Blue, 2);
        PointF? prevPoint = null;

        for (float x = xStart; x <= xEnd; x += step)
        {
            float y = CalculateY(x);
            if (!float.IsNaN(y))
            {
                float scaledX = scaleX(x, xStart, xEnd);
                float scaledY = scaleY(y, yMin, yMax);

                PointF currentPoint = new PointF(scaledX, scaledY);
                if (prevPoint != null)
                {
                    g.DrawLine(graphPen, prevPoint.Value, currentPoint);
                }
                prevPoint = currentPoint;
            }
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new GraphForm());
    }
}

namespace Billiard;

public partial class Form1 : Form
{ 
    private readonly Random _random = new(); 

    private const int _numberOfBalls = 4;
    private const double _frictionCoefficient = 0.009;  

    private const int ballWidth = 50;
    private const int ballHeight = 50;

    private int[] ballPosX = new int[_numberOfBalls];
    private int[] ballPosY = new int[_numberOfBalls];

    private double[] moveStepX = new double[_numberOfBalls];
    private double[] moveStepY = new double[_numberOfBalls];

    [Obsolete]
    public Form1()
    {
        InitializeComponent(); 

        for (var i = 0; i < _numberOfBalls; i++)
        {
            moveStepX[i] = _random.Next(15, 100);
            moveStepY[i] = _random.Next(15, 100);
            ballPosX[i] = _random.Next(0, this.ClientSize.Width);
            ballPosY[i] = _random.Next(0, this.ClientSize.Height);
        } 

        this.SetStyle(
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint,
            true
        );

        this.UpdateStyles(); 

        for (var index = 0; index < _numberOfBalls; index++)
            ThreadPool.QueueUserWorkItem(new WaitCallback(MoveBall), index);
    }

    private void PaintCircle(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode =
            System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        e.Graphics.Clear(this.BackColor);


        for (var i = 0; i < _numberOfBalls; i++)
        {
            e.Graphics.FillEllipse(Brushes.Red,
                ballPosX[i], ballPosY[i],
                ballWidth, ballHeight);

            e.Graphics.DrawEllipse(Pens.Black,
                ballPosX[i], ballPosY[i],
                ballWidth, ballHeight);
        }
    }

    private void MoveBall(object obj)
    {
        if (obj == null)
            return;
        var tuple = (int)obj;
        while (true)
        {
            if ((int)moveStepX[tuple] == 1 || (int)moveStepY[tuple] == 1)
            {  
                var result = MessageBox.Show($"Скорость шара номер-{tuple + 1} равна нулю. Хотите продолжить?", "Предупреждение", MessageBoxButtons.YesNo); 
                if (result == DialogResult.Yes)
                { 
                    moveStepX[tuple] = _random.Next(25, 100);
                    moveStepY[tuple] = _random.Next(25, 100);
                    ballPosX[tuple] = _random.Next(0, this.ClientSize.Width);
                    ballPosY[tuple] = _random.Next(0, this.ClientSize.Height); 
                }
                else
                { 
                    break;
                } 
            }

            ballPosX[tuple] += (int)moveStepX[tuple];
            if (
                ballPosX[tuple] < 0 ||
                ballPosX[tuple] + ballWidth > this.ClientSize.Width
                )
            {
                moveStepX[tuple] = -moveStepX[tuple];
            }

            ballPosY[tuple] += (int)moveStepY[tuple];
            if (
                ballPosY[tuple] < 0 ||
                ballPosY[tuple] + ballHeight > this.ClientSize.Height
                )
            {
                moveStepY[tuple] = -moveStepY[tuple];
            }

            ReduceSpeed(tuple);  
             
            this.Invoke((MethodInvoker)this.Refresh); 
            Thread.Sleep(30);
        }
    }

    private void ReduceSpeed(int index)
    {
        moveStepX[index] -= (moveStepX[index] * _frictionCoefficient);
        moveStepY[index] -= (moveStepY[index] * _frictionCoefficient);
    }
} 
   
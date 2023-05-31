using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
//using Microsoft.VisualBasic.Devices;
//using static System.Formats.Asn1.AsnWriter;
//using System.Reflection.Metadata;
using System;
using System.CodeDom;

namespace SnakeGameCS
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 座標用クラス
        /// </summary>
        class snakePos
        {
            public int x { get; set; }
            public int y { get; set; }
            public snakePos(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }
        // スネーク座標
        List<snakePos> pSnakePos = new List<snakePos> {
            new snakePos( 15, 24 ),
            new snakePos( 16, 24 ),
            new snakePos( 17, 24 ),
            new snakePos( 18, 24 ) };
        // 餌座標
        snakePos pFoodPos = new snakePos(1, 1);

        // const値
        const int _size = 20;
        const int _width = 32;
        const int _height = 32;

        int nScore = 0;                  // スコア
        int nSpeed = 200;                // 進行速度
        //int nAniFlg = 0;                 // アニメーション
        int nNextDirectionMove = 0;      // 先行入力用移動方向
        int nDirectionMove = 0;          // 移動方向

        // タイマー
        System.Windows.Forms.Timer _timer = null;
        // ゲーム開始フラグ
        bool bStartFlg = false;

        // マップ
        Map pMap = null;

        // 効果音再生用
        private System.Media.SoundPlayer _player = new System.Media.SoundPlayer(".\\se_get_1.wav");

        /// <summary>
        /// インストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            pMap = new Map(_width, _height, _size);

            this.Width = _width * _size + 40;
            this.Height = _height * _size + 80;
            // ダブルバッファリング
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Text = "SnakeGameCS";

            btnStart.Location = new System.Drawing.Point(10, 5);
            label1.Location = new System.Drawing.Point(110, 5);
            lblScore.Location = new System.Drawing.Point(165, 5);

            pictureBox1.Location = new System.Drawing.Point(10, 34);
            pictureBox1.Width = _width * _size;
            pictureBox1.Height = _height * _size;

        }
        /// <summary>
        /// 開始ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (bStartFlg == false)
            {
                // タイマーの間隔(ミリ秒)
                _timer = new System.Windows.Forms.Timer();
                _timer.Tick += new EventHandler(tickHandler);
                _timer.Interval = nSpeed;
                _timer.Start();
                bStartFlg = true;
                btnStart.Text = "停止";

                // 開始初期値＝左方向
                nNextDirectionMove = 1;
                // 餌座標演算
                makeFood();
            }
            else
            {
                _timer.Stop();
                bStartFlg = false;
                btnStart.Text = "開始";
            }
            //label1.Focus();
        }
        /// <summary>
        /// タイマー割り込みイベント
        /// メイン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tickHandler(object sender, EventArgs e)
        {
            try
            { 
            snakePos pos = pSnakePos[0];
            int mx = pos.x;
            int my = pos.y;

            //先行入力処理
            switch (nNextDirectionMove)
            {
                case 1:                                 // 左方向
                    if (pMap.getMap(pos.y, pos.x - 1) == 0)         // 道判定（壁判定）
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 1;
                    }
                    break;
                case 2:                                 // 右方向
                    if (pMap.getMap(pos.y, pos.x + 1) == 0)        // 道判定（壁判定）
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 2;
                    }
                    break;
                case 3:                                 // 上方向
                    if (pMap.getMap(pos.y - 1, pos.x) == 0)        // 道判定（壁判定）
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 3;
                    }
                    break;
                case 4:                                 // 下方向
                    if (pMap.getMap(pos.y + 1, pos.x) == 0)        // 道判定（壁判定）
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 4;
                    }
                    break;

            }
            // 移動
            switch (nDirectionMove)
            {
                case 1:
                    if (pos.x <= 0)                        // ワープ判定
                        mx = _width - 1;
                    else if (pMap.getMap(pos.y, pos.x - 1) == 0)
                        mx -= 1;
                    break;
                case 2:
                    if (pos.x >= _width - 1)                        // ワープ判定
                        mx = 0;
                    else if (pMap.getMap(pos.y, pos.x + 1) == 0)
                        mx += 1;
                    break;
                case 3:
                    if (pMap.getMap(pos.y - 1, pos.x) == 0)
                        my -= 1;
                    break;
                case 4:
                    if (pMap.getMap(pos.y + 1, pos.x) == 0)
                        my += 1;
                    break;
            }
            // 移動処理
            if (mx != pos.x || my != pos.y)
            {
                snakePos p = new snakePos(mx, my);
                pSnakePos.Insert(0, p);
                pSnakePos.RemoveAt(pSnakePos.Count - 1);
                pictureBox1.Invalidate();        //PictureBox更新
            }
            // 当たり判定
            if (bingo() == true)
            {
                if (bStartFlg == true)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                    bStartFlg = false;
                    // ゲームオーバー
                    MessageBox.Show(this, "ゲームオーバー", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
            }
            catch(Exception ex)
            {
                // 例外処理
                return;
            }
        }

        /// <summary>
        /// スクリーン描画イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // 背景
            g.FillRectangle(Brushes.Black, 0, 0, pictureBox1.Width, pictureBox1.Height);
            // 壁描画
            pMap.drawMap(g);
            if (bStartFlg == true) drawFood(g);
            drawMe(g);
        }
        /// <summary>
        /// 食べ物位置計算
        /// </summary>
        private void makeFood()
        {
            List<snakePos> fPos = new List<snakePos>();

            // 位置演算
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (pMap.getMap(y, x) == 0)
                    {
                        // スネーク自分判定
                        int myp = 0;
                        for (int i = 0; i < pSnakePos.Count - 1; i++)
                        {
                            if (pSnakePos[i].x == x && pSnakePos[i].y == y)
                                myp++;
                        }
                        if (myp == 0)
                        {
                            // 道（map=0）を抽出
                            snakePos p = new snakePos(x, y);
                            fPos.Add(p);
                        }
                    }
                }
            }
            // 餌位置を道（map=0）からランダムに選択
            var rand = new Random();
            int num = rand.Next(minValue: 0, maxValue: fPos.Count - 1);
            snakePos pos = fPos[num];
            pFoodPos.x = pos.x;
            pFoodPos.y = pos.y;
        }
        /// <summary>
        /// 餌描画
        /// </summary>
        /// <param name="g"></param>
        private void drawFood(Graphics g)
        {
            g.FillEllipse(Brushes.Red, pFoodPos.x * _size + (_size / 3), pFoodPos.y * _size + (_size / 3), _size / 2, _size / 2);
        }

        /// <summary>
        /// 自分描画
        /// </summary>
        /// <param name="g"></param>
        public void drawMe(Graphics g)
        {
            g.FillEllipse(Brushes.Yellow, pSnakePos[0].x * _size, pSnakePos[0].y * _size, 20f, 20f);
            for (int i = 1; i < pSnakePos.Count; i++)
                g.FillRectangle(Brushes.Bisque, pSnakePos[i].x * _size, pSnakePos[i].y * _size, _size, _size);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //キーボード押下げ時の判定
            switch (e.KeyData)
            {
                case Keys.Left:
                    nNextDirectionMove = 1;
                    break;
                case Keys.Right:
                    nNextDirectionMove = 2;
                    break;
                case Keys.Up:
                    nNextDirectionMove = 3;
                    break;
                case Keys.Down:
                    nNextDirectionMove = 4;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// ボタンにフォーカスが移動しているとアローキーイベントが発生しない為
        /// ボタンイベントとして追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //キーボード押下げ時の判定
            switch (e.KeyData)
            {
                case Keys.Left:
                    nNextDirectionMove = 1;
                    break;
                case Keys.Right:
                    nNextDirectionMove = 2;
                    break;
                case Keys.Up:
                    nNextDirectionMove = 3;
                    break;
                case Keys.Down:
                    nNextDirectionMove = 4;
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// フォームクロージングイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bStartFlg == true && _timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        private bool bingo()
        {
            snakePos myPos = pSnakePos[0];

            // 餌・当たり判定
            if (pFoodPos.x == myPos.x && pFoodPos.y == myPos.y)
            {
                snakePos p = pSnakePos[pSnakePos.Count - 1];
                pSnakePos.Add(p);

                // スコア加算
                nScore++;
                lblScore.Text = nScore.ToString();
                // ゲームスピード調整
                nSpeed -= 5;
                _timer.Interval = nSpeed;
                //document.getElementById("gameScore").textContent = " Score: " + score + "  Speed:" + sp;

                // 効果音
                _player.Play();

                // 新たな餌演算
                makeFood();
            }

            // スネーク・当たり判定
            for (int i = 1; i < pSnakePos.Count; i++)
            {
                snakePos p = pSnakePos[i];
                if (p.x == myPos.x && p.y == myPos.y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
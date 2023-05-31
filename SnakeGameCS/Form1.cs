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
        /// ���W�p�N���X
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
        // �X�l�[�N���W
        List<snakePos> pSnakePos = new List<snakePos> {
            new snakePos( 15, 24 ),
            new snakePos( 16, 24 ),
            new snakePos( 17, 24 ),
            new snakePos( 18, 24 ) };
        // �a���W
        snakePos pFoodPos = new snakePos(1, 1);

        // const�l
        const int _size = 20;
        const int _width = 32;
        const int _height = 32;

        int nScore = 0;                  // �X�R�A
        int nSpeed = 200;                // �i�s���x
        //int nAniFlg = 0;                 // �A�j���[�V����
        int nNextDirectionMove = 0;      // ��s���͗p�ړ�����
        int nDirectionMove = 0;          // �ړ�����

        // �^�C�}�[
        System.Windows.Forms.Timer _timer = null;
        // �Q�[���J�n�t���O
        bool bStartFlg = false;

        // �}�b�v
        Map pMap = null;

        // ���ʉ��Đ��p
        private System.Media.SoundPlayer _player = new System.Media.SoundPlayer(".\\se_get_1.wav");

        /// <summary>
        /// �C���X�g���N�^
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// �t�H�[�����[�h
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            pMap = new Map(_width, _height, _size);

            this.Width = _width * _size + 40;
            this.Height = _height * _size + 80;
            // �_�u���o�b�t�@�����O
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
        /// �J�n�{�^���N���b�N�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (bStartFlg == false)
            {
                // �^�C�}�[�̊Ԋu(�~���b)
                _timer = new System.Windows.Forms.Timer();
                _timer.Tick += new EventHandler(tickHandler);
                _timer.Interval = nSpeed;
                _timer.Start();
                bStartFlg = true;
                btnStart.Text = "��~";

                // �J�n�����l��������
                nNextDirectionMove = 1;
                // �a���W���Z
                makeFood();
            }
            else
            {
                _timer.Stop();
                bStartFlg = false;
                btnStart.Text = "�J�n";
            }
            //label1.Focus();
        }
        /// <summary>
        /// �^�C�}�[���荞�݃C�x���g
        /// ���C������
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

            //��s���͏���
            switch (nNextDirectionMove)
            {
                case 1:                                 // ������
                    if (pMap.getMap(pos.y, pos.x - 1) == 0)         // ������i�ǔ���j
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 1;
                    }
                    break;
                case 2:                                 // �E����
                    if (pMap.getMap(pos.y, pos.x + 1) == 0)        // ������i�ǔ���j
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 2;
                    }
                    break;
                case 3:                                 // �����
                    if (pMap.getMap(pos.y - 1, pos.x) == 0)        // ������i�ǔ���j
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 3;
                    }
                    break;
                case 4:                                 // ������
                    if (pMap.getMap(pos.y + 1, pos.x) == 0)        // ������i�ǔ���j
                    {
                        nNextDirectionMove = 0;
                        nDirectionMove = 4;
                    }
                    break;

            }
            // �ړ�
            switch (nDirectionMove)
            {
                case 1:
                    if (pos.x <= 0)                        // ���[�v����
                        mx = _width - 1;
                    else if (pMap.getMap(pos.y, pos.x - 1) == 0)
                        mx -= 1;
                    break;
                case 2:
                    if (pos.x >= _width - 1)                        // ���[�v����
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
            // �ړ�����
            if (mx != pos.x || my != pos.y)
            {
                snakePos p = new snakePos(mx, my);
                pSnakePos.Insert(0, p);
                pSnakePos.RemoveAt(pSnakePos.Count - 1);
                pictureBox1.Invalidate();        //PictureBox�X�V
            }
            // �����蔻��
            if (bingo() == true)
            {
                if (bStartFlg == true)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                    bStartFlg = false;
                    // �Q�[���I�[�o�[
                    MessageBox.Show(this, "�Q�[���I�[�o�[", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
            }
            catch(Exception ex)
            {
                // ��O����
                return;
            }
        }

        /// <summary>
        /// �X�N���[���`��C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // �w�i
            g.FillRectangle(Brushes.Black, 0, 0, pictureBox1.Width, pictureBox1.Height);
            // �Ǖ`��
            pMap.drawMap(g);
            if (bStartFlg == true) drawFood(g);
            drawMe(g);
        }
        /// <summary>
        /// �H�ו��ʒu�v�Z
        /// </summary>
        private void makeFood()
        {
            List<snakePos> fPos = new List<snakePos>();

            // �ʒu���Z
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (pMap.getMap(y, x) == 0)
                    {
                        // �X�l�[�N��������
                        int myp = 0;
                        for (int i = 0; i < pSnakePos.Count - 1; i++)
                        {
                            if (pSnakePos[i].x == x && pSnakePos[i].y == y)
                                myp++;
                        }
                        if (myp == 0)
                        {
                            // ���imap=0�j�𒊏o
                            snakePos p = new snakePos(x, y);
                            fPos.Add(p);
                        }
                    }
                }
            }
            // �a�ʒu�𓹁imap=0�j���烉���_���ɑI��
            var rand = new Random();
            int num = rand.Next(minValue: 0, maxValue: fPos.Count - 1);
            snakePos pos = fPos[num];
            pFoodPos.x = pos.x;
            pFoodPos.y = pos.y;
        }
        /// <summary>
        /// �a�`��
        /// </summary>
        /// <param name="g"></param>
        private void drawFood(Graphics g)
        {
            g.FillEllipse(Brushes.Red, pFoodPos.x * _size + (_size / 3), pFoodPos.y * _size + (_size / 3), _size / 2, _size / 2);
        }

        /// <summary>
        /// �����`��
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
            //�L�[�{�[�h���������̔���
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
        /// �{�^���Ƀt�H�[�J�X���ړ����Ă���ƃA���[�L�[�C�x���g���������Ȃ���
        /// �{�^���C�x���g�Ƃ��Ēǉ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //�L�[�{�[�h���������̔���
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
        /// �t�H�[���N���[�W���O�C�x���g
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
        /// �����蔻��
        /// </summary>
        private bool bingo()
        {
            snakePos myPos = pSnakePos[0];

            // �a�E�����蔻��
            if (pFoodPos.x == myPos.x && pFoodPos.y == myPos.y)
            {
                snakePos p = pSnakePos[pSnakePos.Count - 1];
                pSnakePos.Add(p);

                // �X�R�A���Z
                nScore++;
                lblScore.Text = nScore.ToString();
                // �Q�[���X�s�[�h����
                nSpeed -= 5;
                _timer.Interval = nSpeed;
                //document.getElementById("gameScore").textContent = " Score: " + score + "  Speed:" + sp;

                // ���ʉ�
                _player.Play();

                // �V���ȉa���Z
                makeFood();
            }

            // �X�l�[�N�E�����蔻��
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
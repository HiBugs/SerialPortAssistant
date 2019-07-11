using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;//设置串口号
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text, 10);//十进制数据转换，设置波特率
                serialPort1.Open();//打开串口
                button1.Enabled = false;//打开串口按钮不可用
                button2.Enabled = true;//关闭串口按钮可用
            }
            catch
            {
                MessageBox.Show("端口错误,请检查串口！", "错误");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < 10; i++)
            {
                comboBox1.Items.Add("COM" + i.ToString());
            }
            comboBox1.Text = "COM5";//串口号默认值
            comboBox2.Text = "9600";//波特率默认值

            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);//添加事件处理程序
        }
        public class MyString
        {
            private string Value { get; set; }
            public MyString(string value)
            {
                this.Value = value;
            }
            public MyString(char value)
            {
                this.Value = value.ToString();
            }
            //  以下为互相隐式转换
            public static implicit operator MyString(char value)
            {
                return new MyString(value);
            }
            public static implicit operator char(MyString value)
            {
                return Convert.ToChar(value.Value);
            }
            public static implicit operator String(MyString value)
            {
                return value.Value.ToString();
            }
            public static implicit operator MyString(String value)
            {
                return new MyString(value);
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            serialPort1.Encoding = System.Text.Encoding.GetEncoding("GB2312");
            if (!radioButton3.Checked)//如果接收模式为字符模式
            {
                string str = serialPort1.ReadExisting();//字符串方式读
                
                textBox1.AppendText(str);//添加内容textBox文本框中依次向后显示
            }
            else //如果接收模式为数值接收
            {
                //易出现异常：由于线程退出或应用程序请求，已中止 I/O 操作
                //加入异常处理
                try
                {
                    int data;
                    data = serialPort1.ReadByte();
                    // string str = Convert.ToString(data, 16).ToUpper();//转换为大写十六进制字符串
                    //textBox1.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + " ");//空位补“0”     
                    string str = Convert.ToString(data);
                    textBox1.AppendText(str);

                }
                catch
                {
                    this.Close();//关闭当前窗体
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();//关闭串口
                button1.Enabled = true;//打开串口按钮可用
                button2.Enabled = false;//关闭串口按钮不可用
            }
            catch
            {
                MessageBox.Show("串口关闭错误！","错误");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Encoding = System.Text.Encoding.GetEncoding("GB2312");
            byte[] Data = new byte[1];
            if (serialPort1.IsOpen)//判断串口是否打开，如果打开执行下一步操作
            {
                if (textBox2.Text != "")
                {
                    if (!radioButton1.Checked)//如果发送模式是字符模式
                    {
                        try
                        {
                            serialPort1.WriteLine(textBox2.Text);//写数据
                        }
                        catch
                        {
                            MessageBox.Show("串口数据写入错误！", "错误");//出错提示
                            serialPort1.Close();
                            button1.Enabled = true;//打开串口按钮可用
                            button2.Enabled = false;//关闭串口按钮不可用
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (textBox2.Text.Length - textBox2.Text.Length % 2) / 2; i++)//取余3运算作用是防止用户输入的字符为奇数个
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(i * 2, 2), 16);
                            serialPort1.Write(Data, 0, 1);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）
                        }
                        if (textBox2.Text.Length % 2 != 0)//剩下一位单独处理
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(textBox2.Text.Length - 1, 1), 16);//单独发送B（0B）
                            serialPort1.Write(Data, 0, 1);//发送
                        }

                    }
                }
                else
                {
                    MessageBox.Show("发送内容为空，请输入数据！", "错误");
                }
            }
            else
            {
                MessageBox.Show("请打开串口！", "错误");
            }
        }
        
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
                button3.Enabled = true;//只有当发送文本框内的有内容时才可以发送
            else
                button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text="";//清屏
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }
}

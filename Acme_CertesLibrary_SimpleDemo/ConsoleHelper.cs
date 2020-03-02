using System;
using System.Text;

namespace Acme_CertesLibrary_SimpleDemo
{
    /// <summary>
    /// 创建时间：2016-07-24
    /// 创建者：shiyousan.com
    /// 描述：控制台帮助类
    /// </summary>
    public class ConsoleHelper
    {
        /// <summary>
        /// 输出带有颜色的文本
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void WriteLineByColor(string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        #region 绘制分割线

        /// <summary>
        /// 绘制控制台应用程序界面一半的分割线
        /// </summary>
        public static void DrawHalfSplitLine()
        {
            DrawSplitLine('-', Console.BufferWidth / 2);
        }
        /// <summary>
        /// 绘制控制台应用程序界面一半的分割线
        /// <param name="separator">指定绘制的符号</param>
        /// </summary>
        public static void DrawHalfSplitLine(char separator)
        {
            DrawSplitLine(separator, Console.BufferWidth / 2);
        }
        /// <summary>
        /// 绘制控制台应用程序界面一半的分割线
        /// <param name="color">分割线颜色</param>
        /// </summary>
        public static void DrawHalfSplitLine(ConsoleColor color)
        {
            DrawSplitLine('-', Console.BufferWidth / 2, color);
        }
        /// <summary>
        /// 绘制控制台应用程序界面一半的分割线
        /// <param name="separator">指定绘制的符号</param>
        /// <param name="color">分割线颜色</param>
        /// </summary>
        public static void DrawHalfSplitLine(char separator, ConsoleColor color)
        {
            DrawSplitLine(separator, Console.BufferWidth / 2, color);
        }
        /// <summary>
        /// 绘制分割线
        /// </summary>
        public static void DrawSplitLine()
        {
            DrawSplitLine('-');
        }
        /// <summary>
        /// 绘制分割线
        /// </summary>
        /// <param name="separator">指定绘制的符号</param>
        public static void DrawSplitLine(char separator)
        {
            DrawSplitLine(separator, Console.BufferWidth);
        }
        /// <summary>
        /// 绘制分割线
        /// </summary>
        /// <param name="lineWidth">分割线宽度（单位1字符）</param>
        public static void DrawSplitLine(int lineWidth)
        {
            DrawSplitLine('-', lineWidth);
        }
        /// <summary>
        /// 绘制分割线
        /// </summary>
        /// <param name="color">分割线颜色</param>
        public static void DrawSplitLine(ConsoleColor color)
        {
            DrawSplitLine('-', color);
        }
        /// <summary>
        /// 绘制分割线
        /// </summary>
        /// <param name="separator">指定绘制的符号</param>
        /// <param name="color">分割线颜色</param>
        public static void DrawSplitLine(char separator, ConsoleColor color)
        {
            DrawSplitLine(separator, Console.BufferWidth, color);
        }

        /// <summary>
        /// 绘制分割线
        /// </summary>
        /// <param name="separator">指定绘制的符号</param>
        /// <param name="lineWidth">分割线宽度（单位1字符）</param>
        /// <param name="color">分割线颜色</param>
        public static void DrawSplitLine(char separator, int lineWidth, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            DrawSplitLine(separator, lineWidth);
            Console.ResetColor();
        }

        /// <summary>
        /// 绘制分割线
        /// </summary>
        /// <param name="separator">指定绘制的符号</param>
        /// <param name="lineWidth">分割线宽度（单位1字符）</param>
        public static void DrawSplitLine(char separator, int lineWidth)
        {
            for (int i = 0; i < lineWidth; i++)
            {
                Console.Write(separator);
            }
            Console.WriteLine("\r");
        }
        #endregion

        /// <summary>
        /// 绘制进度
        /// </summary>
        /// <param name="left">光标的列位置</param>
        /// <param name="top">光标的行位置</param>
        /// <param name="index">当前进度索引</param>
        /// <param name="count">当前任务总数</param>
        public static void DrawProgress(int left, int top, int index, int count)
        {
            int cursorTop = Console.CursorTop;
            int cursorLeft = Console.CursorLeft;
            //ConsoleColor colorBack = Console.BackgroundColor;
            //ConsoleColor colorFore = Console.ForegroundColor;
            //更新进度百分比,原理同上.                
            Console.ForegroundColor = ConsoleColor.Green;
            //清除当前行
            Console.SetCursorPosition(left, top);
            Console.Write("{0}/{1}", index, count);
            if (index == count)
            {
                Console.Write("---完成---{0}", DateTime.Now);
            }
            Console.ResetColor();

            Console.CursorTop = cursorTop;
            Console.CursorLeft = cursorLeft;
            //Console.ForegroundColor = colorFore;
        }

        /// <summary>
        /// 计算真实长度
        /// 网上的方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static int TrueLength(string str)
        {
            int lenTotal = 0;
            int n = str.Length;
            string strWord = "";
            int asc;
            for (int i = 0; i < n; i++)
            {
                strWord = str.Substring(i, 1);
                asc = Convert.ToChar(strWord);
                if (asc < 0 || asc > 127)
                {
                    lenTotal = lenTotal + 2;
                }
                else
                {
                    lenTotal = lenTotal + 1;
                }
            }
            return lenTotal;
        }

        /// <summary>
        /// 获取右侧算起真实长度
        /// 网上获取
        /// </summary>
        /// <param name="strOriginal"></param>
        /// <param name="maxTrueLength"></param>
        /// <param name="chrPad"></param>
        /// <param name="blnCutTail"></param>
        /// <returns></returns>
        public static string PadRightTrueLen(string strOriginal, int maxTrueLength, char chrPad, bool blnCutTail)
        {
            string strNew = strOriginal;
            if (strOriginal == null || maxTrueLength <= 0)
            {
                strNew = "";
                return strNew;
            }

            int trueLen = TrueLength(strOriginal);
            if (trueLen > maxTrueLength)
            {
                if (blnCutTail)
                {
                    for (int i = strOriginal.Length - 1; i > 0; i--)
                    {
                        strNew = strNew.Substring(0, i);
                        if (TrueLength(strNew) == maxTrueLength)
                        {
                            break;
                        }
                        else if (TrueLength(strNew) < maxTrueLength)
                        {
                            strNew += chrPad.ToString();
                            break;
                        }
                    }
                }
            }
            else
            {
                // 填充
                for (int i = 0; i < maxTrueLength - trueLen; i++)
                {
                    strNew += chrPad.ToString();
                }
            }
            return strNew;
        }

        /// <summary>
        /// 清除控制台行内容
        /// </summary>
        /// <param name="cursorTop">当前top坐标</param>
        /// <param name="stringLength">要清除的字符串</param>
        public static void ClearLine(int cursorTop, string text)
        {
            int stringLength = string.IsNullOrWhiteSpace(text) ? 0 : Encoding.Default.GetBytes(text).Length;
            ClearLine(cursorTop, stringLength);
        }

        /// <summary>
        /// 清除控制台行内容
        /// </summary>
        /// <param name="cursorTop">当前top坐标</param>
        /// <param name="stringLength">要清除的字符串长度</param>
        public static void ClearLine(int cursorTop, int stringLength = 0)
        {
            if (stringLength == 0)
            {
                stringLength = Console.WindowWidth;
            }
            Console.SetCursorPosition(0, cursorTop);
            StringBuilder clearPlaceholder = new StringBuilder();
            clearPlaceholder.Insert(0, " ", stringLength);
            Console.Write(clearPlaceholder.ToString());
            Console.SetCursorPosition(0, cursorTop);
        }
    }
}

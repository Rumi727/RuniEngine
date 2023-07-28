#nullable enable
using System.Text;
using System;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace RuniEngine
{
    public static class StringUtility
    {
        public static string ConstEnvironmentVariable(this string value)
        {
            value = value.Replace("%DataPath%", Application.dataPath);
            value = value.Replace("%StreamingAssetsPath%", Application.streamingAssetsPath);
            value = value.Replace("%PersistentDataPath%", Application.persistentDataPath);

            value = value.Replace("%CompanyName%", Application.companyName);
            value = value.Replace("%ProductName%", Application.productName);
            value = value.Replace("%Version%", Application.version);

            return value;
        }

        /// <summary>
        /// (text = "AddSpacesToSentence") = "Add Spaces To Sentence"
        /// </summary>
        /// <param name="text">텍스트</param>
        /// <param name="preserveAcronyms">약어(준말) 보존 (true = (UnscaledFPSDeltaTime = Unscaled FPS Delta Time), false = (UnscaledFPSDeltaTime = Unscaled FPSDelta Time))</param>
        /// <returns></returns>
        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) || (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        #region KeyCode to String
        /// <summary>
        /// (keyCode = KeyCode.RightArrow) = "→"
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static string KeyCodeToString(this KeyCode keyCode, bool simply = false)
        {
            string text = keyCode switch
            {
                KeyCode.Escape => "ESC",
                KeyCode.Return when !simply => "Enter",
                KeyCode.Return when simply => "↵",
                KeyCode.Alpha0 => "0",
                KeyCode.Alpha1 => "1",
                KeyCode.Alpha2 => "2",
                KeyCode.Alpha3 => "3",
                KeyCode.Alpha4 => "4",
                KeyCode.Alpha5 => "5",
                KeyCode.Alpha6 => "6",
                KeyCode.Alpha7 => "7",
                KeyCode.Alpha8 => "8",
                KeyCode.Alpha9 => "9",
                KeyCode.AltGr when simply => "AG",
                KeyCode.Ampersand => "&",
                KeyCode.Asterisk => "*",
                KeyCode.At => "@",
                KeyCode.BackQuote => "`",
                KeyCode.Backslash => "\\",
                KeyCode.Caret => "^",
                KeyCode.Colon => ":",
                KeyCode.Comma => ",",
                KeyCode.Dollar => "$",
                KeyCode.DoubleQuote => "\"",
                KeyCode.Equals => "=",
                KeyCode.Exclaim => "!",
                KeyCode.Greater => ">",
                KeyCode.Hash => "#",
                KeyCode.Keypad0 when !simply => "Keypad 0",
                KeyCode.Keypad1 when !simply => "Keypad 1",
                KeyCode.Keypad2 when !simply => "Keypad 2",
                KeyCode.Keypad3 when !simply => "Keypad 3",
                KeyCode.Keypad4 when !simply => "Keypad 4",
                KeyCode.Keypad5 when !simply => "Keypad 5",
                KeyCode.Keypad6 when !simply => "Keypad 6",
                KeyCode.Keypad7 when !simply => "Keypad 7",
                KeyCode.Keypad8 when !simply => "Keypad 8",
                KeyCode.Keypad9 when !simply => "Keypad 9",
                KeyCode.KeypadDivide when !simply => "Keypad /",
                KeyCode.KeypadEnter when !simply => "Keypad ↵",
                KeyCode.KeypadEquals when !simply => "Keypad =",
                KeyCode.KeypadMinus when !simply => "Keypad -",
                KeyCode.KeypadMultiply when !simply => "Keypad *",
                KeyCode.KeypadPeriod when !simply => "Keypad .",
                KeyCode.KeypadPlus when !simply => "Keypad +",
                KeyCode.Keypad0 when simply => "K0",
                KeyCode.Keypad1 when simply => "K1",
                KeyCode.Keypad2 when simply => "K2",
                KeyCode.Keypad3 when simply => "K3",
                KeyCode.Keypad4 when simply => "K4",
                KeyCode.Keypad5 when simply => "K5",
                KeyCode.Keypad6 when simply => "K6",
                KeyCode.Keypad7 when simply => "K7",
                KeyCode.Keypad8 when simply => "K8",
                KeyCode.Keypad9 when simply => "K9",
                KeyCode.KeypadDivide when simply => "K/",
                KeyCode.KeypadEnter when simply => "K↵",
                KeyCode.KeypadEquals when simply => "K=",
                KeyCode.KeypadMinus when simply => "K-",
                KeyCode.KeypadMultiply when simply => "K*",
                KeyCode.KeypadPeriod when simply => "K.",
                KeyCode.KeypadPlus when simply => "K+",
                KeyCode.LeftApple => "Left Command",
                KeyCode.LeftBracket => "[",
                KeyCode.LeftCurlyBracket => "{",
                KeyCode.LeftParen => "(",
                KeyCode.LeftWindows when simply => "LW",
                KeyCode.Less => "<",
                KeyCode.Minus => "-",
                KeyCode.Mouse0 when !simply => "Left Mouse",
                KeyCode.Mouse1 when !simply => "Right Mouse",
                KeyCode.Mouse2 when !simply => "Middle Mouse",
                KeyCode.Mouse0 when simply => "LM",
                KeyCode.Mouse1 when simply => "RM",
                KeyCode.Mouse2 when simply => "MM",
                KeyCode.Mouse3 when simply => "M3",
                KeyCode.Mouse4 when simply => "M4",
                KeyCode.Mouse5 when simply => "M5",
                KeyCode.Mouse6 when simply => "M6",
                KeyCode.Percent => "%",
                KeyCode.Period => ".",
                KeyCode.Pipe => "|",
                KeyCode.Plus => "+",
                KeyCode.Print when !simply => "Print Screen",
                KeyCode.Print when simply => "PS",
                KeyCode.Question => "?",
                KeyCode.Quote => "'",
                KeyCode.RightApple => "Right Command",
                KeyCode.RightBracket => "]",
                KeyCode.RightCurlyBracket => "}",
                KeyCode.RightParen => ")",
                KeyCode.RightWindows when simply => "LW",
                KeyCode.Semicolon => ";",
                KeyCode.Slash => "/",
                KeyCode.Space when simply => "␣",
                KeyCode.SysReq when !simply => "Print Screen",
                KeyCode.SysReq when simply => "PS",
                KeyCode.Tilde => "~",
                KeyCode.Underscore => "_",
                KeyCode.UpArrow => "↑",
                KeyCode.DownArrow => "↓",
                KeyCode.LeftArrow => "←",
                KeyCode.RightArrow => "→",
                KeyCode.LeftControl when !simply => "Left Ctrl",
                KeyCode.RightControl when !simply => "Right Ctrl",
                KeyCode.LeftControl when simply => "LC",
                KeyCode.RightControl when simply => "RC",
                KeyCode.LeftAlt when simply => "LA",
                KeyCode.RightAlt when simply => "RA",
                KeyCode.LeftShift when simply => "L⇧",
                KeyCode.RightShift when simply => "R⇧",
                KeyCode.Backspace when simply => "B←",
                KeyCode.Delete when simply => "D←",
                KeyCode.PageUp when simply => "P↑",
                KeyCode.PageDown when simply => "P↓",
                _ => keyCode.ToString().AddSpacesToSentence(),
            };
            return text;
        }
        #endregion

        #region To Bar
        /// <summary>
        /// (value = 5, max = 10, length = 10) = "■■■■■□□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToBar(this int value, int max, int length, string fill = "■", string half = "▣", string empty = "□") => ToBar((double)value, max, length, fill, half, empty);

        /// <summary>
        /// (value = 5.5, max = 10, length = 10) = "■■■■■▣□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToBar(this float value, float max, int length, string fill = "■", string half = "▣", string empty = "□") => ToBar((double)value, max, length, fill, half, empty);

        /// <summary>
        /// (value = 5.5, max = 10, length = 10) = "■■■■■▣□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToBar(this double value, double max, int length, string fill = "■", string half = "▣", string empty = "□")
        {
            string text = "";

            for (double i = 0.5; i < length + 0.5; i++)
            {
                if (value / max >= i / length)
                    text += fill;
                else
                {
                    if (value / max >= (i - 0.5) / length)
                        text += half;
                    else
                        text += empty;
                }
            }
            return text;
        }
        #endregion

        public static string DataSizeToString(this long byteSize) => ByteTo(byteSize, out string space) + space;

        /// <summary>
        /// 데이터 크기를(바이트) 문자열로 바꿔줍니다 (B, KB, MB, GB, TB, PB, EB, ZB, YB)
        /// </summary>
        /// <param name="byteSize">계산할 용량</param>
        /// <param name="digits">계산된 용량에서 표시할 소수점 자리수</param>
        /// <returns>계산된 용량</returns>
        public static string DataSizeToString(this long byteSize, int digits) => ByteTo(byteSize, out string space).Round(digits) + space;

        /// <summary>
        /// 데이터 크기를(바이트) 적절하게 바꿔줍니다 (B, KB, MB, GB, TB, PB, EB, ZB, YB)
        /// </summary>
        /// <param name="byteSize">계산할 용량</param>
        public static double ByteTo(long byteSize, out string space)
        {
            int loopCount = 0;
            double size = byteSize;

            while (size > 1024.0)
            {
                size /= 1024.0;
                loopCount++;
            }

            if (loopCount == 0)
                space = "B";
            else if (loopCount == 1)
                space = "KB";
            else if (loopCount == 2)
                space = "MB";
            else if (loopCount == 3)
                space = "GB";
            else if (loopCount == 4)
                space = "TB";
            else if (loopCount == 5)
                space = "PB";
            else if (loopCount == 6)
                space = "EB";
            else if (loopCount == 7)
                space = "ZB";
            else
                space = "YB";

            return size;
        }

        public static double ByteTo(long byteSize, DataSizeType dataSizeType, out string space)
        {
            double size = byteSize / Math.Pow(1024, (int)dataSizeType);
            space = dataSizeType.ToString().ToUpper();

            return size;
        }

        public static string[] QuotedSplit(this string text, string separator) => text.QuotedSplit2(separator).ToArray();

        //https://codereview.stackexchange.com/a/166801
        static IEnumerable<string> QuotedSplit2(this string text, string separator)
        {
            const char quote = '\"';

            StringBuilder sb = new StringBuilder(text.Length);
            int counter = 0;
            while (counter < text.Length)
            {
                // if starts with delmiter if so read ahead to see if matches
                if (separator[0] == text[counter] && separator.SequenceEqual(ReadNext(text, counter, separator.Length)))
                {
                    yield return sb.ToString();

                    sb.Clear();
                    counter += separator.Length; // Move the counter past the delimiter 
                }
                else if (text[counter] == quote) // if we hit a quote read until we hit another quote or end of string
                {
                    sb.Append(text[counter++]);
                    while (counter < text.Length && text[counter] != quote)
                        sb.Append(text[counter++]);

                    // if not end of string then we hit a quote add the quote
                    if (counter < text.Length)
                        sb.Append(text[counter++]);
                }
                else
                    sb.Append(text[counter++]);
            }

            if (sb.Length > 0)
                yield return sb.ToString();
        }

        static IEnumerable<char> ReadNext(string str, int currentPosition, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (currentPosition + i >= str.Length)
                    yield break;
                else
                    yield return str[currentPosition + i];
            }
        }

        public enum DataSizeType
        {
            b,
            kb,
            mb,
            gb,
            tb,
            pb,
            eb,
            zb,
            yb
        }

        public static string ToSummaryString(this Exception e) => $"{e.GetType().Name}: {e.Message}\n\n{e.StackTrace.Substring(5)}";
    }
}

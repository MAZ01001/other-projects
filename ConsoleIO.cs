using System;                         //~ for datatypes/namespaces, Console object, ConsoleColor, Convert object and exceptions (the basics)
using System.Threading;               //~ for Sleep (time delay)
using System.Globalization;           //~ for NumberFormatInfo (NumberFormatting)
using System.Collections.Generic;     //~ for List<T>
using System.Runtime.InteropServices; //~ for DllImports (fullscreen)

namespace ConsoleIO{
    //~ If you're reading this, don't forget to document your code with these → https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments#recommended-tags
    /// <summary>A lot of static methods for creating a console based text adventure (needs 64bit on win7+)</summary>
    public static class Run{
        /// <summary>
        ///     get or set the cursor height in percent [1 to 100]
        ///     <br/>see also:
        ///     <br/><seealso cref="CursorShow"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">[set] if the cursor size is not in range [1 to 100]</exception>
        public static Int32 CursorSize{
            get => Console.CursorSize;
            set{
                if(value is < 1 or > 100) throw new ArgumentOutOfRangeException(nameof(value), "must be in range [1 to 100]");
                Console.CursorSize = value;
            }
        }
        /// <summary>
        ///     Used as directions for:
        ///     <br/><seealso cref="Scroll"/>
        ///     <br/><seealso cref="ScrollAnim"/>
        ///     <br/><seealso cref="ScrollOnce"/>
        /// </summary>
        public enum ScrollDirection : byte{ up, down, left, right }
        /// <summary>
        ///     [private] used by
        ///     <br/><seealso cref="ScrollAnim"/>
        /// </summary>
        /// <param name="dir">[Optional] The direction in wich to scroll - defaults to <see cref="ScrollDirection.up"></param>
        /// <param name="dir">[Optional] Whether to move the cursor in scroll direction or not (only moves if possible) - defaults to true</param>
        private static void ScrollOnce(ScrollDirection dir = ScrollDirection.up, bool cm = true){
            switch(dir){
                case ScrollDirection.up:
                    if(cm && CursorY > 1) CursorY--;
                    Console.MoveBufferArea(1, 2, InnerWidth, InnerHeight - 1, 1, 1, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
                case ScrollDirection.down:
                    if(cm && CursorY < InnerHeight) CursorY++;
                    Console.MoveBufferArea(1, 1, InnerWidth, InnerHeight - 1, 1, 2, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
                case ScrollDirection.left:
                    if(cm && CursorX > 1) CursorX--;
                    Console.MoveBufferArea(2, 1, InnerWidth - 1, InnerHeight, 1, 1, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
                case ScrollDirection.right:
                    if(cm && CursorX < InnerWidth) CursorX++;
                    Console.MoveBufferArea(1, 1, InnerWidth - 1, InnerHeight, 2, 1, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
            }
        }
        /// <summary>scrolls the content inside borders by <paramref name="amount"/> in <see cref="direction"/> with <paramref name="ms"/> delay</summary>
        /// <param name="amount">[Optional] how moch rows to scroll - [1 to (<see cref="InnerHeight"/> - 1)] - defaults to 1</param>
        /// <param name="ms">[Optional] time in milliseconds for delay - must be a positive integer - defaults to 10</param>
        /// <param name="cursorMove">[Optional] if true moves the cursor along with the scrolling - defaults to true</param>
        /// <param name="direction">[Optional] in which direction to scroll - defaults to <see cref="ScrollDirection.up"/></param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="amount"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="cursorMove"/> is true but can not move the cursor <paramref name="amount"/> in <paramref name="direction"/></exception>
        public static void ScrollAnim(int amount = 1, int ms = 10, bool cursorMove = true, ScrollDirection direction = ScrollDirection.up){
            if(amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "must be a positive integer");
            if(ms < 0) throw new ArgumentOutOfRangeException(nameof(ms), "must be a positive integer");
            try{
                if(ms == 0){
                    Scroll(amount, cursorMove, direction);
                    return;
                }
                for(; amount > 0; amount--){
                    ScrollOnce(direction, cursorMove);
                    Wait(ms);
                }
            }catch(ArgumentOutOfRangeException){ throw new ArgumentOutOfRangeException(nameof(cursorMove), "can not move the cursor that much with [cursorMove] on"); }
        }
        /// <summary>scrolls the content inside borders by <paramref name="amount"/> in <see cref="direction"/></summary>
        /// <param name="amount">[Optional] how many rows to scroll - [1 to (<see cref="InnerHeight"/> - 1)] - defaults to 1</param>
        /// <param name="cursorMove">[Optional] if true moves the cursor along with the scrolling - defaults to true</param>
        /// <param name="direction">[Optional] in which direction to scroll - defaults to <see cref="ScrollDirection.up"/></param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="amount"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="cursorMove"/> is true but can not move the cursor <paramref name="amount"/> in <paramref name="direction"/></exception>
        public static void Scroll(int amount = 1, bool cursorMove = true, ScrollDirection direction = ScrollDirection.up){
            if(amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "must be a positive integer");
            switch(direction){
                case ScrollDirection.up:
                    if(cursorMove){
                        int _check = CursorY - amount;
                        if(_check < 1) throw new ArgumentOutOfRangeException(nameof(cursorMove), "can not move the cursor that much with [cursorMove] on");
                        else CursorY = _check;
                    } Console.MoveBufferArea(1, 1 + amount, InnerWidth, InnerHeight - amount, 1, 1, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
                case ScrollDirection.down:
                    if(cursorMove){
                        int _check = CursorY + amount;
                        if(_check > InnerHeight) throw new ArgumentOutOfRangeException(nameof(cursorMove), "can not move the cursor that much with [cursorMove] on");
                        else CursorY = _check;
                    } Console.MoveBufferArea(1, 1, InnerWidth, InnerHeight - amount, 1, 1 + amount, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
                case ScrollDirection.left:
                    if(cursorMove){
                        int _check = CursorX - amount;
                        if(_check < 1) throw new ArgumentOutOfRangeException(nameof(cursorMove), "can not move the cursor that much with [cursorMove] on");
                        else CursorX = _check;
                    } Console.MoveBufferArea(1 + amount, 1, InnerWidth - amount, InnerHeight, 1, 1, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
                case ScrollDirection.right:
                    if(cursorMove){
                        int _check = CursorX + amount;
                        if(_check > InnerWidth) throw new ArgumentOutOfRangeException(nameof(cursorMove), "can not move the cursor that much with [cursorMove] on");
                        else CursorX = _check;
                    } Console.MoveBufferArea(1, 1, InnerWidth - amount, InnerHeight, 1 + amount, 1, ' ', Console.ForegroundColor, Console.BackgroundColor);
                break;
            }
        }
        /// <summary>plays a sound with the given frequency and duration (at system volume!)</summary>
        /// <param name="freq">[Optional] the frequancy of the tone in Hertz - [37 to 32'767] - defaults to 800</param>
        /// <param name="time">[Optional] the duration of the tone in milliseconds - must be a positive integer - defaults to 200</param>
        /// <param name="wait">[Optional] if true waits for the tone to end - defaults to true</param>
        /// <exception cref="freq">if <paramref name="freq"/> is not in range [37 to 32'767]</exception>
        /// <exception cref="time">if <paramref name="time"/> is not a positive integer</exception>
        public static void PlayTone(Int32 freq = 800, Int32 time = 200, bool wait = true){
            if(freq is < 37 or > 32_767) throw new ArgumentOutOfRangeException(nameof(freq), "must be in range [37 to 32'767]");
            if(time < 0) throw new ArgumentOutOfRangeException(nameof(time), "must be a positive integer");
            if(time == 0) return;
            //~ no volume option - better options with [new System.Media.SoundPlayer(@"c:\mywavfile.wav").Play();] but this needs a soundfile ~ or byte stream !
            Console.Beep(freq, time);
            if(wait) Wait(time);
        }
        /// <summary>
        ///     get or set the cursor position from the left [1 to innerWidth]
        ///     <br/>see also:
        ///     <br/><seealso cref="CursorY"/>
        ///     <br/><seealso cref="SetCursorPos"/>
        ///     <br/><seealso cref="InnerWidth"/>
        ///     <br/><seealso cref="InnerHeight"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        ///     <br/><seealso cref="LargestWidth"/>
        ///     <br/><seealso cref="LargestHeight"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">[set] if the cursors X position is not in range [1 to <see cref="InnerWidth"/>]</exception>
        public static Int32 CursorX{
            get => Console.CursorLeft - Console.WindowLeft;
            set{
                if(value < 1 || value > InnerWidth) throw new ArgumentOutOfRangeException(nameof(value), "must be in range [1 to innerWidth]");
                Console.CursorLeft = Console.WindowLeft + value;
            }
        }
        /// <summary>
        ///     get or set the cursor position from the top [1 to innerHeight]
        ///     <br/>see also:
        ///     <br/><seealso cref="CursorX"/>
        ///     <br/><seealso cref="SetCursorPos"/>
        ///     <br/><seealso cref="InnerWidth"/>
        ///     <br/><seealso cref="InnerHeight"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        ///     <br/><seealso cref="LargestWidth"/>
        ///     <br/><seealso cref="LargestHeight"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">[set] if the cursors Y position is not in range [1 to <see cref="InnerHeight"/>]</exception>
        public static Int32 CursorY{
            get => Console.CursorTop - Console.WindowTop;
            set{
                if(value < 1 || value > InnerHeight) throw new ArgumentOutOfRangeException(nameof(value), "must be in range [1 to innerHeight]");
                Console.CursorTop = Console.WindowTop + value;
            }
        }
        /// <summary>
        ///     set the position of the cursor in the console
        ///     <br/>see also:
        ///     <br/><seealso cref="CursorX"/>
        ///     <br/><seealso cref="CursorY"/>
        ///     <br/><seealso cref="InnerWidth"/>
        ///     <br/><seealso cref="InnerHeight"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        ///     <br/><seealso cref="LargestWidth"/>
        ///     <br/><seealso cref="LargestHeight"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        /// <param name="col">column index (from left) - [1 to <see cref="GetInnerWidth"/>]</param>
        /// <param name="row">row index (from top) - [1 to <see cref="GetInnerHeight"/>]</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1 to <see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1 to <see cref="GetInnerHeight"/>]</exception>
        public static void SetCursorPos(Int32 col, Int32 row){
            if(col < 1 || col > InnerWidth) throw new ArgumentOutOfRangeException(nameof(col), "must be in range [1 to innerWidth]");
            if(row < 1 || row > InnerHeight) throw new ArgumentOutOfRangeException(nameof(row), "must be in range [1 to innerHeight]");
            Console.SetCursorPosition(col, row);
        }
        /// <summary>
        ///     get the maximum width the console could be set at
        ///     <br/>for <see cref="SetConsoleSize"/>
        ///     <br/>see also:
        ///     <br/><seealso cref="LargestHeight"/>
        ///     <br/><seealso cref="InnerWidth"/>
        ///     <br/><seealso cref="InnerHeight"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        ///     <br/><seealso cref="CursorX"/>
        ///     <br/><seealso cref="CursorY"/>
        ///     <br/><seealso cref="SetCursorPos"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        public static Int32 LargestWidth => Console.LargestWindowWidth;
        /// <summary>
        ///     get the maximum height the console could be set at
        ///     <br/>for <see cref="SetConsoleSize"/>
        ///     <br/>see also:
        ///     <br/><seealso cref="LargestWidth"/>
        ///     <br/><seealso cref="InnerWidth"/>
        ///     <br/><seealso cref="InnerHeight"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        ///     <br/><seealso cref="CursorX"/>
        ///     <br/><seealso cref="CursorY"/>
        ///     <br/><seealso cref="SetCursorPos"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        public static Int32 LargestHeight => Console.LargestWindowHeight;
        /// <summary>
        ///     get or set the console width (including the 1 width border on each side)
        ///     <br/><i>the border is overwritten with spaces and the cursor is reset to [0,0]</i>
        ///     <br/>see also:
        ///     <br/><seealso cref="InnerHeight"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        ///     <br/><seealso cref="DrawBorder"/>
        ///     <br/><seealso cref="LargestWidth"/>
        ///     <br/><seealso cref="LargestHeight"/>
        ///     <br/><seealso cref="CursorX"/>
        ///     <br/><seealso cref="CursorY"/>
        ///     <br/><seealso cref="SetCursorPos"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">[set] if the window width is not in range [3 to <see cref="LargestWidth"/>]</exception>
        public static Int32 InnerWidth{
            get => Console.WindowWidth;
            set{
                if(value < 3 || value > LargestWidth) throw new ArgumentOutOfRangeException(nameof(value), "must be in range [3 to LargestWidth]");
                Console.WindowWidth = value;
                DrawBorder(' ');
                SetCursorPos(0, 0);
            }
        }
        /// <summary>
        ///     get or set the console height (including the 1 width border on each side)
        ///     <br/><i>the border is overwritten with spaces and the cursor is reset to [0,0]</i>
        ///     <br/>see also:
        ///     <br/><seealso cref="InnerWidth"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        ///     <br/><seealso cref="DrawBorder"/>
        ///     <br/><seealso cref="LargestWidth"/>
        ///     <br/><seealso cref="LargestHeight"/>
        ///     <br/><seealso cref="CursorX"/>
        ///     <br/><seealso cref="CursorY"/>
        ///     <br/><seealso cref="SetCursorPos"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">[set] if the window height is not in range [3 to <see cref="LargestHeight"/>]</exception>
        public static Int32 InnerHeight{
            get => Console.WindowHeight;
            set{
                if(value < 3 || value > LargestHeight) throw new ArgumentOutOfRangeException(nameof(value), "must be in range [3 to LargestHeight]");
                Console.WindowHeight = value;
                DrawBorder(' ');
                SetCursorPos(0, 0);
            }
        }
        /// <summary>
        ///     sets the size of the console window
        ///     <br/><i>the border is overwritten with spaces and the cursor is reset to [0,0]</i>
        ///     <br/>see also:
        ///     <br/><seealso cref="InnerWidth"/>
        ///     <br/><seealso cref="InnerHeight"/>
        ///     <br/><seealso cref="LargestWidth"/>
        ///     <br/><seealso cref="LargestHeight"/>
        ///     <br/><seealso cref="DrawBorder"/>
        ///     <br/><seealso cref="CursorX"/>
        ///     <br/><seealso cref="CursorY"/>
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="RevertFullscreen"/>
        /// </summary>
        /// <param name="width">the new width of the console - [3 to <see cref="LargestWidth"/>]</param>
        /// <param name="height">the new height of the console - [3 to <see cref="LargestHeight"/>]</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="width"/> is not in range [3 to <see cref="LargestWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="height"/> is not in range [3 to <see cref="LargestHeight"/>]</exception>
        public static void SetConsoleSize(Int32 width, Int32 height){
            if(width < 3 || width > LargestWidth) throw new ArgumentOutOfRangeException(nameof(width), "must be in range [3 to LargestWidth]");
            if(height < 3 || height > LargestHeight) throw new ArgumentOutOfRangeException(nameof(height), "must be in range [3 to LargestHeight]");
            Console.SetWindowSize(width, height);
            DrawBorder(' ');
            SetCursorPos(0, 0);
        }
        /// <summary>
        ///     waits for input and returns it
        ///     <br/>custom input mode, can read [A-Z], [0-9], [Numpad0-9], space, '_', delete(left/right), move cursor(left/right/start/end) and tabulator(auto 1-4 spaces)
        ///     <br/>input ends either if <see cref="MaxInputSize"/> is reached or if ESC or Enter is pressed
        ///     <br/>see also:
        ///     <br/><seealso cref="GetNum"/>
        ///     <br/><seealso cref="GetIntNum"/>
        ///     <br/><seealso cref="GetSingleNum"/>
        ///     <br/><seealso cref="GetChar"/>
        ///     <br/><seealso cref="ClearInput"/>
        /// </summary>
        /// <param name="MaxInputSize">maximum characters allowet to enter - defaults to 10'000</param>
        /// <return>the string inputed</return>
        public static string GetInput(ushort MaxInputSize = 10_000){
            int _c = CursorX,
                _r = CursorY,
                _i = 0;
            string _s = "";
            bool _end = false;
            ConsoleKeyInfo _last;
            do{
                ClearInput();
                CursorShow = true;
                _last = Console.ReadKey(true);
                CursorShow = false;
                switch(_last.Key){
                    case ConsoleKey.Backspace:
                        if(_i > 0){
                            Text(new string(' ', _s.Length), true, _c, _r);
                            _s = _s.Substring(0, _i - 1) + _s.Substring(_i);
                            _i--;
                        }
                    break;
                    case ConsoleKey.Delete:
                        if(_i < _s.Length){
                            Text(new string(' ', _s.Length), true, _c, _r);
                            _s = _s.Substring(0, _i) + _s.Substring(_i + 1);
                        }
                    break;
                    case ConsoleKey.LeftArrow:
                        if(_i > 0) _i--;
                    break;
                    case ConsoleKey.RightArrow:
                        if(_i < _s.Length) _i++;
                    break;
                    case ConsoleKey.Home:
                    case ConsoleKey.UpArrow:
                        _i = 0;
                    break;
                    case ConsoleKey.End:
                    case ConsoleKey.DownArrow:
                        _i = _s.Length;
                    break;
                    case ConsoleKey.Tab:
                        int _tmp_t = 4 - (((_c + _i) % InnerWidth + 1) % 4);
                        _s = _s.Substring(0, _i) + new String((char)ConsoleKey.Spacebar, _tmp_t) + _s.Substring(_i);
                        _i += _tmp_t;
                    break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Escape:
                        _end = true;
                    break;
                    default:
                        if(
                            (_last.Key >= ConsoleKey.A && _last.Key <= ConsoleKey.Z)
                            || (_last.Key >= ConsoleKey.D0 && _last.Key <= ConsoleKey.D9)
                            || (_last.Key >= ConsoleKey.NumPad0 && _last.Key <= ConsoleKey.NumPad9)
                            || _last.Key == ConsoleKey.Spacebar
                            || _last.KeyChar == '_'
                            || _last.KeyChar == '-'
                            || _last.KeyChar == '.'
                            || _last.KeyChar == ','
                            || _last.KeyChar == '\''
                        ){
                            _s = _s.Substring(0, _i) + _last.KeyChar + _s.Substring(_i);
                            _i++;
                        }
                    break;
                }
                SetColor(Colors.Yellow);
                Text(_s, true, _c, _r);
                SetColor();
                CursorX = (((_c - 1) + _i) % InnerWidth) + 1;
                CursorY = (_r + (int)(((_c - 1) + _i) / InnerWidth));
            }while(!_end && _s.Length < MaxInputSize);
            ClearInput();
            return _s;
        }
        /// <summary>
        ///     waits for keypress and returns it
        ///     <br/>see also:
        ///     <br/><seealso cref="GetNum"/>
        ///     <br/><seealso cref="GetIntNum"/>
        ///     <br/><seealso cref="GetSingleNum"/>
        ///     <br/><seealso cref="GetInput"/>
        ///     <br/><seealso cref="ClearInput"/>
        /// </summary>
        /// <param name="hide">if true does not show pressed char in console - defaults to false → shows char in console</param>
        /// <return>the key pressed</return>
        public static char GetChar(bool hide = false){
            ConsoleKeyInfo _tmp_c = Console.ReadKey(true);
            if(!hide && _tmp_c.Key != ConsoleKey.Enter) Text("" + _tmp_c.KeyChar);
            return _tmp_c.KeyChar;
        }
        /// <summary>
        ///     waits for input, converts it and then returns the number
        ///     <br/>see also:
        ///     <br/><seealso cref="GetNum"/>
        ///     <br/><seealso cref="GetIntNum"/>
        ///     <br/><seealso cref="GetChar"/>
        ///     <br/><seealso cref="GetInput"/>
        ///     <br/><seealso cref="ClearInput"/>
        /// </summary>
        /// <param name="hide">if true does not show pressed char in console - defaults to false → shows char in console (also on error)</param>
        /// <return>the number</return>
        /// <exception cref="FormatException">if the input is not a number</exception>
        public static byte GetSingleNum(bool hide = false){
            char _tmp = GetChar(hide);
            if(_tmp < '0' || _tmp > '9') throw new FormatException("input is not a number");
            return (byte)(_tmp - '0');
        }
        /// <summary>
        ///     waits for input, converts it and then returns the number
        ///     <br/>see also:
        ///     <br/><seealso cref="GetNum"/>
        ///     <br/><seealso cref="GetSingleNum"/>
        ///     <br/><seealso cref="GetChar"/>
        ///     <br/><seealso cref="GetInput"/>
        ///     <br/><seealso cref="ClearInput"/>
        /// </summary>
        /// <return>the number</return>
        /// <exception cref="FormatException">if the input is in wrong format</exception>
        /// <exception cref="OverflowException">if the input is too long to be converted</exception>
        public static int GetIntNum() => Convert.ToInt32(GetInput());
        /// <summary>
        ///     waits for input, converts it and then returns the number
        ///     <br/>see also:
        ///     <br/><seealso cref="GetIntNum"/>
        ///     <br/><seealso cref="GetSingleNum"/>
        ///     <br/><seealso cref="GetChar"/>
        ///     <br/><seealso cref="GetInput"/>
        ///     <br/><seealso cref="ClearInput"/>
        /// </summary>
        /// <return>the number</return>
        /// <exception cref="FormatException">if the input is in wrong format</exception>
        /// <exception cref="OverflowException">if the input is too long to be converted</exception>
        public static double GetNum() => Convert.ToDouble(GetInput());
        /// <summary>
        ///     clears the input stream
        ///     <br/>see also:
        ///     <br/><seealso cref="GetNum"/>
        ///     <br/><seealso cref="GetIntNum"/>
        ///     <br/><seealso cref="GetSingleNum"/>
        ///     <br/><seealso cref="GetChar"/>
        ///     <br/><seealso cref="GetInput"/>
        /// </summary>
        public static void ClearInput(){ while(Console.KeyAvailable)GetChar(true); }
        /// <summary>
        ///     [private] imported function to set window size (maximize/minimize/restore/hidden/...)
        ///     <br/>used for <see cref="SetFullscreen"/> and <see cref="RevertFullscreen"/>
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //~ https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow#parameters
        private const int W_MAXIMIZE = 3;
        private const int W_RESTORE = 9;
        /// <summary>
        ///     [private] imported function to get the console window process handle
        ///     <br/>used for <see cref="ShowWindow"/>'s first parameter
        /// </summary>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        /// <summary>
        ///     make window fullscreen
        ///     <br/>see also:
        ///     <br/><seealso cref="RevertFullscreen"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        /// </summary>
        public static void SetFullscreen() => ShowWindow(GetConsoleWindow(), W_MAXIMIZE);
        /// <summary>
        ///     turn fullscreen off and restores the original window position and size
        ///     <br/>see also:
        ///     <br/><seealso cref="SetFullscreen"/>
        ///     <br/><seealso cref="SetConsoleSize"/>
        /// </summary>
        public static void RevertFullscreen() => ShowWindow(GetConsoleWindow(), W_RESTORE);
        /// <summary>
        ///     get or set visibility of the console cursor
        ///     <br/>see also:
        ///     <br/><seealso cref="CursorSize"/>
        /// </summary>
        public static bool CursorShow{
            get => Console.CursorVisible;
            set => Console.CursorVisible = value;
        }
        /// <summary>set the title of the console window</summary>
        /// <param name="title">title</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="title"/> is longer than 24'500 characters</exception>
        public static void SetTitle(string title){
            if(title.Length > 24_500) throw new ArgumentOutOfRangeException(nameof(title), "must not be more than 24'500 characters long");
            Console.Title = title;
        }
        /// <summary>used as Colors for: <seealso cref="SetColor"/></summary>
        public enum Colors : byte{
            Black       = ConsoleColor.Black,       // 0
            DarkBlue    = ConsoleColor.DarkBlue,    // 1
            DarkGreen   = ConsoleColor.DarkGreen,   // 2
            DarkCyan    = ConsoleColor.DarkCyan,    // 3
            DarkRed     = ConsoleColor.DarkRed,     // 4
            DarkMagenta = ConsoleColor.DarkMagenta, // 5
            DarkYellow  = ConsoleColor.DarkYellow,  // 6
            Gray        = ConsoleColor.Gray,        // 7
            DarkGray    = ConsoleColor.DarkGray,    // 8
            Blue        = ConsoleColor.Blue,        // 9
            Green       = ConsoleColor.Green,       // A (10)
            Cyan        = ConsoleColor.Cyan,        // B (11)
            Red         = ConsoleColor.Red,         // C (12)
            Magenta     = ConsoleColor.Magenta,     // D (13)
            Yellow      = ConsoleColor.Yellow,      // E (14)
            White       = ConsoleColor.White        // F (15)
        }
        /// <summary>
        ///     get or set the console background color
        ///     <br/>see also:
        ///     <br/><seealso cref="ForegroundColor"/>
        ///     <br/><seealso cref="Colors"/>
        ///     <br/><seealso cref="SetColor"/>
        ///     <br/><seealso cref="ResetColor"/>
        /// </summary>
        public static Colors BackgroundColor{
            get => (Colors)Console.BackgroundColor;
            set => Console.BackgroundColor = (ConsoleColor)value;
        }
        /// <summary>
        ///     get or set the console foreground color
        ///     <br/>see also:
        ///     <br/><seealso cref="BackgroundColor"/>
        ///     <br/><seealso cref="Colors"/>
        ///     <br/><seealso cref="SetColor"/>
        ///     <br/><seealso cref="ResetColor"/>
        /// </summary>
        public static Colors ForegroundColor{
            get => (Colors)Console.ForegroundColor;
            set => Console.ForegroundColor = (ConsoleColor)value;
        }
        /// <summary>
        ///     set the fore- and background color for the console
        ///     <br/>see also:
        ///     <br/><seealso cref="ForegroundColor"/>
        ///     <br/><seealso cref="BackgroundColor"/>
        ///     <br/><seealso cref="Colors"/>
        ///     <br/><seealso cref="ResetColor"/>
        /// </summary>
        /// <param name="foregroundColor">foreground color - defaults to <see cref="Colors.Green"/></param>
        /// <param name="backgroundColor">background color - defaults to <see cref="Colors.Black"/></param>
        public static void SetColor(Colors foregroundColor = Colors.Green, Colors backgroundColor = Colors.Black){
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
        /// <summary>
        ///     resets the console color to original values
        ///     <br/>see also:
        ///     <br/><seealso cref="ForegroundColor"/>
        ///     <br/><seealso cref="BackgroundColor"/>
        ///     <br/><seealso cref="Colors"/>
        ///     <br/><seealso cref="SetColor"/><br/>
        /// </summary>
        public static void ResetColor() => Console.ResetColor();
        /// <summary>[private] cursor save slot list (each int[2])</summary>
        private static List<int[]> cursorSave = new List<int[]>(4); //~ 4 empty slots
        /// <summary>
        ///     saves current cursor position in cursor save slot list
        ///     <br/>see also:
        ///     <br/><seealso cref="LoadCursor"/>
        ///     <br/><seealso cref="ClearSavedCursors"/>
        /// </summary>
        /// <param name="index">save slot index - overrides - defaults to -1 → auto add to end</param>
        /// <return>the index of the save slot used</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is not an available save slot</exception>
        public static int SaveCursor(int index = -1){
            if(index == -1 || index == cursorSave.Count){
                cursorSave.Add(new int[2]{ CursorX, CursorY });
                return cursorSave.Count - 1;
            }else if(index < 0 || index >= cursorSave.Count) throw new ArgumentOutOfRangeException(nameof(index), "must be an index of a possible cursor save slot");
            cursorSave[index] = new int[2]{ CursorX, CursorY };
            return index;
        }
        /// <summary>
        ///     loads cursor save slot
        ///     <br/>see also:
        ///     <br/><seealso cref="SaveCursor"/>
        ///     <br/><seealso cref="ClearSavedCursors"/>
        /// </summary>
        /// <param name="index">save slot index</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is not an existing save slot</exception>
        public static void LoadCursor(int index){
            if(index < 0 || index >= cursorSave.Count) throw new ArgumentOutOfRangeException(nameof(index), "must be an index of an existing cursor save slot");
            CursorX = cursorSave[index][0];
            CursorY = cursorSave[index][1];
        }
        /// <summary>
        ///     clear all cursor save slots
        ///     <br/>see also:
        ///     <br/><seealso cref="SaveCursor"/>
        ///     <br/><seealso cref="LoadCursor"/>
        /// </summary>
        public static void ClearSavedCursors() => cursorSave.Clear();
        /// <summary>holds the program for the given delay in milliseconds</summary>
        /// <param name="ms">time in milliseconds for delay - must be positive integer</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        public static void Wait(int ms){
            if(ms < 0) throw new ArgumentOutOfRangeException(nameof(ms), "must be a positive integer");
            if(ms > 0) Thread.Sleep(ms);
        }
        /// <summary>
        ///     draws the border in the console with custom chars
        ///     <br/>custom [left / top / bottom / right] bars and [top-left / bottom-left / top-right / bottom-right] corners in that order
        /// </summary>
        /// <param name="left">char for the left (vertical) bar of the console excluding the corners</param>
        /// <param name="top">char for the top (horizontal) bar of the console excluding the corners</param>
        /// <param name="bottom">char for the bottom (horizontal) bar of the console excluding the corners</param>
        /// <param name="right">char for the right (vertical) bar of the console excluding the corners</param>
        /// <param name="topleft">char for the topleft corner of the console</param>
        /// <param name="bottomleft">char for the bottomleft corner of the console</param>
        /// <param name="topright">char for the topright corner of the console</param>
        /// <param name="bottomright">char for the bottomright corner of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r' or '\n' as value</exception>
        public static void DrawBorder(char left, char top, char bottom, char right, char topLeft, char bottomLeft, char topRight, char bottomRight){
            if(left        is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(left));
            if(top         is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(top));
            if(bottom      is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(bottom));
            if(right       is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(right));
            if(topLeft     is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(topLeft));
            if(bottomLeft  is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(bottomLeft));
            if(topRight    is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(topRight));
            if(bottomRight is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(bottomRight));
            Console.MoveBufferArea(InnerWidth + 1, 0, 1, 1, 0, 0, topRight, (ConsoleColor)ForegroundColor, (ConsoleColor)BackgroundColor);
            Console.MoveBufferArea(0, InnerHeight + 1, 1, 1, 0, 0, bottomLeft, (ConsoleColor)ForegroundColor, (ConsoleColor)BackgroundColor);
            Console.MoveBufferArea(InnerWidth + 1, InnerHeight + 1, 1, 1, 0, 0, bottomRight, (ConsoleColor)ForegroundColor, (ConsoleColor)BackgroundColor);
            Console.MoveBufferArea(1, InnerHeight + 1, InnerWidth, 1, 1, 0, bottom, (ConsoleColor)ForegroundColor, (ConsoleColor)BackgroundColor);
            SetCursorPos(1, 0);
            Console.Write(new String(top, InnerWidth));
            Console.MoveBufferArea(InnerWidth + 1, 1, 1, InnerHeight, 0, 1, right, (ConsoleColor)ForegroundColor, (ConsoleColor)BackgroundColor);
            for(int i = 1; i <= InnerHeight; i++){
                SetCursorPos(0, i);
                Console.Write(left);
            }
            SetCursorPos(0, 0);
            Console.Write(topLeft);
            SetCursorPos(1, 1);
        }
        /// <summary>
        ///     draws the border in the console with custom chars
        ///     <br/>custom [horizontal / vertical] bars and [top-left / bottom-left / top-right / bottom-right] corners in that order
        /// </summary>s
        /// <param name="horizontal">char for the horizontal bars (top / bottom) of the console excluding the corners</param>
        /// <param name="vertical">char for the vertical bars (left / right) of the console excluding the corners</param>
        /// <param name="topleft">char for the topleft corner of the console</param>
        /// <param name="bottomleft">char for the bottomleft corner of the console</param>
        /// <param name="topright">char for the topright corner of the console</param>
        /// <param name="bottomright">char for the bottomright corner of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r' or '\n' as value</exception>
        public static void DrawBorder(char horizontal, char vertical, char topLeft, char bottomLeft, char topRight, char bottomRight){
            if(horizontal  is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(horizontal));
            if(vertical    is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(vertical));
            if(topLeft     is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(topLeft));
            if(bottomLeft  is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(bottomLeft));
            if(topRight    is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(topRight));
            if(bottomRight is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(bottomRight));
            DrawBorder(vertical, horizontal, horizontal, vertical, topLeft, bottomLeft, topRight, bottomRight);
        }
        /// <summary>
        ///     draws the border in the console with custom chars
        ///     <br/>custom [left / top / bottom / right] bars and corners in that order
        /// </summary>
        /// <param name="left">char for the left (vertical) bar of the console excluding the corners</param>
        /// <param name="top">char for the top (horizontal) bar of the console excluding the corners</param>
        /// <param name="bottom">char for the bottom (horizontal) bar of the console excluding the corners</param>
        /// <param name="right">char for the right (vertical) bar of the console excluding the corners</param>
        /// <param name="corners">char for all corners of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r', '\n', or '\t' as value</exception>
        public static void DrawBorder(char left, char top, char bottom, char right, char corners){
            if(left    is '\r' or '\n' or '\t')throw new ArgumentException(@"must not be '\r', '\n', or '\t'", nameof(left));
            if(top     is '\r' or '\n' or '\t')throw new ArgumentException(@"must not be '\r', '\n', or '\t'", nameof(top));
            if(bottom  is '\r' or '\n' or '\t')throw new ArgumentException(@"must not be '\r', '\n', or '\t'", nameof(bottom));
            if(right   is '\r' or '\n' or '\t')throw new ArgumentException(@"must not be '\r', '\n', or '\t'", nameof(right));
            if(corners is '\r' or '\n' or '\t')throw new ArgumentException(@"must not be '\r', '\n', or '\t'", nameof(corners));
            DrawBorder(left, top, bottom, right, corners, corners, corners, corners);
        }
        /// <summary>
        ///     draws the border in the console with custom chars
        ///     <br/>custom horizontal/vertical-bars and corners in that order
        /// </summary>
        /// <param name="horizontal">char for the horizontal bars (top / bottom) of the console excluding the corners</param>
        /// <param name="vertical">char for the vertical bars (left / right) of the console excluding the corners</param>
        /// <param name="corners">char for all corners of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r' or '\n' as value</exception>
        public static void DrawBorder(char horizontal, char vertical, char corners){
            if(horizontal is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(horizontal));
            if(vertical   is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(vertical));
            if(corners    is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(corners));
            DrawBorder(vertical, horizontal, horizontal, vertical, corners, corners, corners, corners);
        }
        /// <summary>
        ///     draws the border in the console with custom chars
        ///     <br/>custom bars and corners in that order
        /// </summary>
        /// <param name="left">char for all bars of the console excluding the corners</param>
        /// <param name="corners">char for all corners of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r' or '\n' as value</exception>
        public static void DrawBorder(char bars, char corners){
            if(bars    is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(bars));
            if(corners is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(corners));
            DrawBorder(bars, bars, bars, bars, corners, corners, corners, corners);
        }
        /// <summary>draws the border in the console with custom char</summary>
        /// <param name="frame">char for the border of the console</param>
        /// <exception cref="ArgumentException">if <paramref name="frame"/> has '\r' or '\n' as value</exception>
        public static void DrawBorder(char frame){
            if(frame is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(frame));
            DrawBorder(frame, frame, frame, frame, frame, frame, frame, frame);
        }
        /// <summary>
        ///     prints text animated without finishing line break, auto bound to inner border
        ///     <br/>can read '\n', '\r', '\t' and '\b'
        ///     <br/>'\0' does not print anything nor move the cursor but still delays
        ///     <br/>clears input before and after printing and if <see cref="interrupt"/> is pressed during printing sets <see cref="ms"/> to 0 and returns true
        ///     <br/>see also:
        ///     <br/><seealso cref="AnimHor"/>
        ///     <br/><seealso cref="Text"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="ms">time in milliseconds to wait each character while printing - must be a positive integer - defaults to 20</param>
        /// <param name="interrupt">if this character is pressed during printing sets <see cref="ms"/> to 0 - defaults to ' '</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - defaults to true</param>
        /// <param name="col">column index (left) where to start printing - [1 to <see cref="InnerWidth"/>] - defaults to 0 → use current</param>
        /// <param name="row">row index (top) where to start printing - [1 to <see cref="InnerHeight"/>] - defaults to 0 → use current</param>
        /// <return>true if printing was interrupted by pressing the <see cref="interrupt"/> key or <see cref="ms"/> is 0</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1 to <see cref="InnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1 to <see cref="InnerHeight"/>]</exception>
        public static bool Anim(string outpt, int ms = 20, char interrupt = ' ', bool scrolling = true, int col = 0, int row = 0){
            if(ms < 0) throw new ArgumentOutOfRangeException(nameof(ms), "must be a positive integer");
            if(col == 0) col = CursorX;
            if(row == 0) row = CursorY;
            if(col < 1 || col > InnerWidth) throw new ArgumentOutOfRangeException(nameof(col), "must be in range [1 to innerWidth]");
            if(row < 1 || row > InnerHeight) throw new ArgumentOutOfRangeException(nameof(row), "must be in range [1 to innerHeight]");
            SetCursorPos(col, row);
            ClearInput();
            //// Array.ForEach<string>(outpt.Split('\n'), (string line) => {});
            foreach(char key in outpt) switch(key){
                case '\n':
                    //// SetCursorPos(CursorX, (CursorY + 1) % InnerHeight);
                    if(CursorY >= InnerHeight){
                        if(scrolling) ScrollAnim((InnerHeight - CursorY) + 1);
                        else SetCursorPos(CursorX, 1);
                    }else SetCursorPos(CursorX, CursorY + 1);
                break;
                case '\r':
                    SetCursorPos(1,CursorY);
                break;
                case '\t':
                    int _tmp_c = CursorX + (4 - ((CursorX + 1) % 4));
                    if(_tmp_c > InnerWidth) SetCursorPos(_tmp_c - InnerWidth, CursorY + 1);
                    else SetCursorPos(_tmp_c, CursorY);
                break;
                case '\b':
                    int _tmp_b = CursorX - 1;
                    if(_tmp_b < 1) SetCursorPos(InnerWidth, CursorY - 1);
                    else SetCursorPos(_tmp_b, CursorY);
                break;
                default:
                    if(key != '\0'){
                        if(CursorX >= InnerWidth){
                            Console.Write(key);
                            if(CursorY >= InnerHeight){
                                if(scrolling) ScrollAnim((InnerHeight - CursorY) + 1);
                                else SetCursorPos(1, 1);
                            }else SetCursorPos(1, CursorY + 1);
                        }else Console.Write(key);
                    }
                    if(ms > 0){
                        Wait(ms);
                        if(Console.KeyAvailable)
                            if(GetChar(true) == interrupt) ms = 0;
                    }
                break;
            }
            ClearInput();
            return ms == 0;
        }
        /// <summary>
        ///     prints text animated from top to bottom without finishing line break, auto bound to inner border
        ///     <br/>auto converts '\n', '\r', '\t' and '\b' to be on the vertical axis
        ///     <br/>'\0' does not print anything nor move the cursor but still delays
        ///     <br/>clears input before and after printing and if <see cref="interrupt"/> is pressed during printing sets <see cref="ms"/> to 0 and returns true
        ///     <br/>see also:
        ///     <br/><seealso cref="Anim"/>
        ///     <br/><seealso cref="Text"/>
        ///     <br/><seealso cref="TextHor"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="ms">time in milliseconds to wait each character while printing - must be a positive integer - defaults to 20</param>
        /// <param name="interrupt">if this character is pressed during printing sets <see cref="ms"/> to 0 - defaults to ' '</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - defaults to true</param>
        /// <param name="col">column index (left) where to start printing - [1 to <see cref="InnerWidth"/>] - defaults to 0 → use current</param>
        /// <param name="row">row index (top) where to start printing - [1 to <see cref="InnerHeight"/>] - defaults to 0 → use current</param>
        /// <return>true if printing was interrupted by pressing the <see cref="interrupt"/> key or <see cref="ms"/> is 0</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1 to <see cref="InnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1 to <see cref="InnerHeight"/>]</exception>
        public static bool AnimHor(string outpt, int ms = 20, char interrupt = ' ', bool scrolling = true, int col = 0, int row = 0){
            if(ms < 0) throw new ArgumentOutOfRangeException(nameof(ms), "must be a positive integer");
            if(col == 0)col = CursorX;
            if(row == 0)row = CursorY;
            if(col < 1 || col > InnerWidth) throw new ArgumentOutOfRangeException(nameof(col), "must be in range [1 to innerWidth]");
            if(row < 1 || row > InnerHeight) throw new ArgumentOutOfRangeException(nameof(row), "must be in range [1 to innerHeight]");
            SetCursorPos(col, row);
            ClearInput();
            foreach(char key in outpt) switch(key){
                case '\n':
                    if(CursorX >= InnerWidth){
                        if(scrolling) ScrollAnim((InnerWidth - CursorX) + 1);
                        else SetCursorPos(1, CursorY);
                    }else SetCursorPos(CursorX + 1, CursorY);
                break;
                case '\r':
                    SetCursorPos(CursorX, 1);
                break;
                case '\t':
                    int _tmp_r = CursorY + (4 - ((CursorY + 1) % 4));
                    if(_tmp_r > InnerHeight) SetCursorPos(CursorX + 1, _tmp_r - InnerHeight);
                    else SetCursorPos(CursorX, _tmp_r);
                break;
                case '\b':
                    int _tmp_b = CursorY - 1;
                    if(_tmp_b < 1) SetCursorPos(CursorX - 1, InnerHeight);
                    else SetCursorPos(CursorX, _tmp_b);
                break;
                default:
                    if(key != '\0'){
                        if(CursorY >= InnerHeight){
                            Console.Write(key);
                            if(CursorX >= InnerWidth){
                                if(scrolling) ScrollAnim((InnerWidth - CursorX) + 1);
                                else SetCursorPos(1, 1);
                            }else SetCursorPos(CursorX, 1);
                        }else{
                            Console.Write(key);
                            SetCursorPos(CursorX - 1, CursorY + 1);
                        }
                    }
                    if(ms > 0){
                        Wait(ms);
                        if(Console.KeyAvailable)
                            if(GetChar(true) == interrupt) ms = 0;
                    }
                break;
            }
            ClearInput();
            return ms == 0;
        }
        /// <summary>
        ///     prints text without finishing line break, auto bound to inner border
        ///     <br/>see also:
        ///     <br/><seealso cref="Anim"/>
        ///     <br/><seealso cref="TextHor"/>
        ///     <br/><seealso cref="AnimHor"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - defaults to true</param>
        /// <param name="col">column index (left) where to start printing - [1 to <see cref="InnerWidth"/>] - defaults to 0 → use current</param>
        /// <param name="row">row index (top) where to start printing - [1 to <see cref="InnerHeight"/>] - defaults to 0 → use current</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1 to <see cref="InnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1 to <see cref="InnerHeight"/>]</exception>
        public static void Text(string outpt, bool scrolling = true, int col = 0, int row = 0){
            if(col == 0) col = CursorX;
            if(row == 0) row = CursorY;
            if(col < 1 || col > InnerWidth) throw new ArgumentOutOfRangeException(nameof(col), "must be in range [1 to innerWidth]");
            if(row < 1 || row > InnerHeight) throw new ArgumentOutOfRangeException(nameof(row), "must be in range [1 to innerHeight]");
            Anim(outpt, 0, ' ', scrolling, col, row);
        }
        /// <summary>
        ///     prints text from top to bottom without finishing line break, auto bound to inner border<
        ///     <br/>auto converts '\n' and '\r' to be on the vertical axis
        ///     <br/>see also:
        ///     <br/><seealso cref="AnimHor"/>
        ///     <br/><seealso cref="Text"/>
        ///     <br/><seealso cref="Anim"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - defaults to true</param>
        /// <param name="col">column index (left) where to start printing - [1 to <see cref="InnerWidth"/>] - defaults to 0 → use current</param>
        /// <param name="row">row index (top) where to start printing - [1 to <see cref="InnerHeight"/>] - defaults to 0 → use current</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1 to <see cref="InnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1 to <see cref="InnerHeight"/>]</exception>
        public static void TextHor(string outpt, bool scrolling = true, int col = 0, int row = 0){
            if(col == 0) col = CursorX;
            if(row == 0) row = CursorY;
            if(col < 1 || col > InnerWidth) throw new ArgumentOutOfRangeException(nameof(col), "must be in range [1 to innerWidth]");
            if(row < 1 || row > InnerHeight) throw new ArgumentOutOfRangeException(nameof(row), "must be in range [1 to innerHeight]");
            AnimHor(outpt, 0, ' ', scrolling, col, row);
        }
        /// <summary>
        ///     clears a single column of the console by printing <paramref name="ch"/> repeadedly, excluding borders
        ///     <br/>if on last column set the cursor to [1, 1], else to the start of next column, afterwards
        ///     <br/>see also:
        ///     <br/><seealso cref="ClearRow"/>
        ///     <br/><seealso cref="ClearAll"/>
        ///     <br/><seealso cref="ClearAllAnim"/>
        ///     <br/><seealso cref="ClearConsoleAll"/>
        /// </summary>
        /// <param name="col">column index (top) of line to clear - [1 to <see cref="InnerWidth"/>]</param>
        /// <param name="ch">char to clear the line with - defaults to ' '</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1 to <see cref="InnerWidth"/>]</exception>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        public static void ClearCol(Int32 col, char ch = ' '){
            if(col < 1 || col > InnerWidth) throw new ArgumentOutOfRangeException(nameof(col), "must be in range [1 to innerWidth]");
            if(ch is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'",nameof(ch));
            for(int i = 1; i <= InnerHeight; i++){
                SetCursorPos(col, i);
                Console.Write(ch);
            }
            if(col == InnerWidth) SetCursorPos(1, 1);
            else SetCursorPos(col + 1, 1);
        }
        /// <summary>
        ///     clears a single row/line of the console by printing <paramref name="ch"/> repeatedly, excluding borders
        ///     <br/>if on last row/line set the cursor to [1, 1], else to the start of next row/line, afterwards
        ///     <br/>see also:
        ///     <br/><seealso cref="ClearCol"/>
        ///     <br/><seealso cref="ClearAll"/>
        ///     <br/><seealso cref="ClearAllAnim"/>
        ///     <br/><seealso cref="ClearConsoleAll"/>
        /// </summary>
        /// <param name="row">row index (left) of line to clear - [1 to <see cref="InnerHeight"/>]</param>
        /// <param name="ch">char to clear the line with - defaults to ' '</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1 to <see cref="InnerHeight"/>]</exception>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        public static void ClearRow(Int32 row, char ch = ' '){
            if(row < 1 || row > InnerHeight) throw new ArgumentOutOfRangeException(nameof(row), "must be in range [1 to innerHeight]");
            if(ch is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'",nameof(ch));
            SetCursorPos(1, row);
            Console.Write(new string(ch, InnerWidth));
            if(row == InnerHeight) SetCursorPos(1, 1);
            else SetCursorPos(1, row + 1);
        }
        /// <summary>used as directions for <see cref="ClearAllAnim"/></summary>
        [Flags]
        public enum AnimDir : byte{
            left  = 1,
            up    = 2,
            right = 4,
            down  = 8,
            row   = left | right, // 5    (0101)
            col   = up | down,    // 10   (1010)
        }
        // TODO → ClearAllAnim → add diagonal-lines ~
        /// <summary>
        ///     like <see cref="ClearAll"/> but animated in horizontal/vertical line
        ///     <br/>sets the cursor to [1, 1] afterwards
        ///     <br/>see also:
        ///     <br/><seealso cref="ClearAll"/>
        ///     <br/><seealso cref="ClearRow"/>
        ///     <br/><seealso cref="ClearCol"/>
        /// </summary>
        /// <param name="ch">char to clear the console with - defaults to ' '</param>
        /// <param name="ms">duration to wait in milliseconds - must be a positive integer - defaults to 20</param>
        /// <param name="timing">where to wait, one of [<see cref="AnimDir.col"/> | <see cref="AnimDir.row"/>] - defaults to <see cref="AnimDir.row"/></param>
        /// <param name="dir">direction in wich to print, one of [ <see cref="AnimDir.left"/> | <see cref="AnimDir.right"/> | <see cref="AnimDir.up"/> | <see cref="AnimDir.down"/> ] - defaults to <see cref="AnimDir.down"/></param>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentException">if <paramref name="timing"/> is not one of [<see cref="AnimDir.col"/> | <see cref="AnimDir.row"/>]</exception>
        /// <exception cref="ArgumentException">if <paramref name="dir"/> is not one of [ <see cref="AnimDir.left"/> | <see cref="AnimDir.right"/> | <see cref="AnimDir.up"/> | <see cref="AnimDir.down"/> ]</exception>
        /// <exception cref="InvalidOperationException">if the combination of <paramref name="timing"/> and <paramref name="dir"/> is not [<see cref="AnimDir.col"/> & [<see cref="AnimDir.left"/> | <see cref="AnimDir.right"/>]] or [<see cref="AnimDir.row"/> & [<see cref="AnimDir.up"/> | <see cref="AnimDir.down"/>]]</exception>
        public static void ClearAllAnim(char ch = ' ', int ms = 20, AnimDir timing = AnimDir.row, AnimDir dir = AnimDir.down){
            if(ch is '\r' or '\n') throw new ArgumentException(@"must not be '\n' or '\r'", nameof(ch));
            if(ms < 0) throw new ArgumentOutOfRangeException(nameof(ms), "must be a positive integer");
            switch(timing){
                case AnimDir.row:
                case AnimDir.col:
                break;
                default: throw new ArgumentException("value must be one of AnimDir[ col | row ]", nameof(timing));
            }
            switch(dir){
                case AnimDir.up:
                case AnimDir.down:
                case AnimDir.left:
                case AnimDir.right:
                break;
                default: throw new ArgumentException("value must be one of AnimDir[ left | right | up | down ]", nameof(dir));
            }
            switch(timing | dir){
                case AnimDir.col | AnimDir.left:  for(int i = InnerWidth; i > 0; i--){   ClearCol(i, ch); Wait(ms); } break;
                case AnimDir.col | AnimDir.right: for(int i = 1; i <= InnerWidth; i++){  ClearCol(i, ch); Wait(ms); } break;
                case AnimDir.row | AnimDir.up:    for(int i = InnerHeight; i > 0; i--){  ClearRow(i, ch); Wait(ms); } break;
                case AnimDir.row | AnimDir.down:  for(int i = 1; i <= InnerHeight; i++){ ClearRow(i, ch); Wait(ms); } break;
                default: throw new InvalidOperationException("combination of [timing | dir] is not AnimDir[col & left/right] or AnimDir[row & up/down]");
            }
            SetCursorPos(1, 1);
        }
        /// <summary>
        ///     clears console by printing <paramref name="ch"/> in every position in console, excluding borders
        ///     <br/>sets the cursor to [1, 1] afterwards
        ///     <br/>see also:
        ///     <br/><seealso cref="ClearAllAnim"/>
        ///     <br/><seealso cref="ClearRow"/>
        ///     <br/><seealso cref="ClearCol"/>
        ///     <br/><seealso cref="ClearConsoleAll"/>
        /// </summary>
        /// <param name="ch">char to clear the console with - defaults to ' '</param>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        public static void ClearAll(char ch = ' '){
            if(ch is '\r' or '\n') throw new ArgumentException(@"must not be '\r' or '\n'", nameof(ch));
            ClearAllAnim(ch, 0);
        }
        /// <summary>
        ///     clears all of console including borders
        ///     <br/>sets the cursor to [1, 1] afterwards
        ///     <br/>see also:
        ///     <br/><seealso cref="ClearAll"/>
        ///     <br/><seealso cref="ClearAllAnim"/>
        ///     <br/><seealso cref="ClearRow"/>
        ///     <br/><seealso cref="ClearCol"/>
        /// </summary>
        public static void ClearConsoleAll(){
            Console.Clear();
            SetCursorPos(1, 1);
        }
        /// <summary>
        ///     "press any key" message
        ///     <br/>(key that is pressed is not shown)
        ///     <br/>see also:
        ///     <br/><seealso cref="GetChar"/>
        /// </summary>
        /// <param name="add">added between "<< press any key" and " >>" as output - defualts to ""</param>
        /// <param name="col">column index (left) where to start printing - [1 to <see cref="InnerWidth"/>] - defaults to 0 → use current</param>
        /// <param name="row">row index (top) where to start printing - [1 to <see cref="InnerHeight"/>] - defaults to 0 → use current</param>
        /// <param name="toggleCursor">if true toggles the cursors visibility after "press any key" - defaults to false</param>
        /// <param name="nextLine">if true sets cursor to next line after "press any key" - defaults to false</param>
        /// <return>char that is pressed</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1 to <see cref="InnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1 to <see cref="InnerHeight"/>]</exception>
        public static char AnyKey(string add = "", int col = 0, int row = 0, bool toggleCursor = false, bool nextLine = false){
            if(col == 0) col = CursorX;
            if(row == 0) row = CursorY;
            if(col < 1 || col > InnerWidth) throw new ArgumentOutOfRangeException(nameof(col), "must be in range [1 to innerWidth]");
            if(row < 1 || row > InnerHeight) throw new ArgumentOutOfRangeException(nameof(row), "must be in range [1 to innerHeight]");
            Anim($"<< Press any key{add} >>{(nextLine?"\n":"")}", 5, '\0', true, col, row);
            if(toggleCursor) CursorShow = !CursorShow;
            ClearInput();
            return GetChar(true);
        }
        /// <summary>formatting a single number</summary>
        /// <param name="num">initial number</param>
        /// <param name="dec_dig">number of decimal places to return - defaults to 0 → also no decimal point</param>
        /// <param name="dec_sep">decimal point string - defaults to "."</param>
        /// <param name="grp_sep">group seperator string - defaults to " "</param>
        /// <param name="grp_siz">group sizes array - defaults to null → {0} (last entry loops)<br/>
        ///     Example:
        ///     <br/>{0}       →              1111111
        ///     <br/>{3}       →           33 333 333
        ///     <br/>{2, 3, 5} →      55 55555 333 22
        ///     <br/>{3, 4, 0} →  5555555555 4444 333
        /// </param>
        /// <return>the formatted number as string</return>
        public static string NumberFormatting(double num, Int32 dec_dig = 0, string dec_sep = ".", string grp_sep = " ", Int32[]? grp_siz = null){
            /*
                ~ https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#table
                $"{5000:N0}"        →   "5 000"
                $"{5000:N3}"        →   "5 000.000"
                $"{5000,10:N2}"     →   "  5 000.00"
                $"{5000:F3}"        →   "5000.000"
                $"{5:D4}"           →   "0005"
                $"{5000:E4}"        →   "5.0000E+003"
                $"{0.2345678:P4}"   →   "23.457%"
            */
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalDigits = dec_dig;
            nfi.NumberDecimalSeparator = dec_sep;
            nfi.NumberGroupSeparator = grp_sep;
            if(grp_siz == null){
                Int32[] no_grp = {0};
                nfi.NumberGroupSizes = no_grp;
            }else grp_siz.CopyTo(nfi.NumberGroupSizes, 0);
            nfi.NumberNegativePattern = 1;
            return num.ToString("N", nfi);
        }
    }
    /// <summary>a small game to show some of the capabilities of the ConsoleIO text-game-"engine"</summary>
    public class TestGame{
        /// <summary>[private] for the animation-text if it is skiped with the interrupt-key(' ') and speed up further animation-text</summary>
        private static bool LastSkiped = false;
        /// <summary>player name as set by the user in the program</summary>
        protected static string playerName = "";
        /// <summary>player age in years as set by the user in the program</summary>
        protected static uint playerAge;
        /// <summary>player height in meters as set by the user in the program</summary>
        protected static float playerHeight;
        /// <summary>todays date-time during runtime</summary>
        protected static DateTime today;
        /// <summary>random number for the find-key mini-game at the end</summary>
        protected static Random random = new Random();
        /// <summary> Start the test game </summary>
        public static void Start(){
            random = new Random();
            today = DateTime.Now;
            Run.SetTitle("MIGHTY CONSOLE - C#");
            // Run.SetConsoleSize(120, 50);
            Run.SetFullscreen();
            Run.CursorSize = 25;
            Run.CursorShow = false;
            Run.ClearSavedCursors();
            Run.PlayTone(1_000, 200, false);
            Run.ClearConsoleAll();
            Run.PlayTone(1_000, 100, false);
            Run.SetColor(Run.Colors.White, Run.Colors.Black);
            ClearBar(Run.InnerHeight>>2, Run.ScrollDirection.down, 2);
            Run.SetColor(Run.Colors.Green);
            Run.DrawBorder('-', '|', '+');
            // timing may be different for each direction, console, and OS ~
            ClearBar(2, Run.ScrollDirection.right, 0); // 3 - 0
            ClearBar(3, Run.ScrollDirection.left,  0); // 4 - 0
            ClearBar(3, Run.ScrollDirection.down,  4); // 2 - 1
            ClearBar(4, Run.ScrollDirection.up,    4); // 3 - 1
            PrintLogoAnim();
            LastSkiped = Run.Anim("\r\n\r\nMighty Console [Version 0.1.001]\r\nCopyright(c) 2021 MAZ.\r\n", 10);
            LastSkiped = Run.Anim("GREETINGS NEW USER!", LastSkiped ? 0 : 10);
            LastSkiped = Run.Anim("\r\nThank you for installing the ", LastSkiped ? 0 : 5);
            LastSkiped = Run.Anim("MIGHTY CONSOLE !!!", LastSkiped ? 0 : 50);
            LastSkiped = Run.Anim("\r\nTo Begin,", LastSkiped ? 0 : 10);
            Run.ScrollAnim(12, 200);
            GetPlayerName();
            GetPlayerAge();
            GetPlayerHeight();
            Run.ScrollAnim(Run.InnerWidth, 10, false, Run.ScrollDirection.left);
            Run.SetCursorPos(1, Run.InnerHeight>>2);
            Run.Anim("so...", 100, '\0');
            DeleteLast(5);
            Run.Anim("your name is ", 100, '\0');
            Run.SetColor(Run.Colors.Cyan);
            Run.Anim($"\"{playerName}\"\r\n", 50);
            Run.SetColor();
            if(playerName.Length > 15){
                Run.Anim("that's quite a dumb...", 100, '\0');
                DeleteLast(7);
                Run.Anim("long name\r\n", 100, '\0');
            }
            Run.Anim("and you are ", 100, '\0');
            Run.SetColor(Run.Colors.Cyan);
            Run.Anim($"{playerAge} Years", 50);
            Run.SetColor();
            Run.Anim($" old\r\nthat means you are born in the year {today.Year - playerAge}\r\n", 100, '\0');
            Run.Anim($"and {Run.NumberFormatting(playerHeight, 2)}m height, wich means in order to reach the moon\r\nyou would need {Run.NumberFormatting(384_400_000 / playerHeight, 2)} clones of yourself on top of each other.", 50);
            Run.Wait(5_000);
            ClearBar();
            Run.SetCursorPos(1, Run.InnerHeight>>2);
            Run.SaveCursor(0);
            byte _menuOption;
            do{
                Run.LoadCursor(0);
                PrintMenu(); // 1 find key | 2 secret | 3 OOO | 4 WIP
                LastSkiped = Run.Anim("\r\nchoose whisely.\r\n", 20, '\0', true, 1, Run.CursorY + 1);
                _menuOption = GetSingleNumStrict();
                switch(_menuOption){
                    case 1:
                        Run.Anim("you did choose wisely.");
                        Run.Wait(3_000);
                    break;
                    case 2:
                        Run.Anim("you did not choose wisely.");
                        Run.Wait(3_000);
                    break;
                    case 3:
                        Run.Anim("can't you read it ");
                        Run.SetColor(Run.Colors.Blue);
                        Run.Anim("clearly");
                        Run.SetColor();
                        Run.Anim(" says Out Of Order.");
                        Run.Wait(5_000);
                    break;
                    case 4:
                        Run.SetColor(Run.Colors.Yellow, Run.Colors.DarkRed);
                        Run.Anim("ERROR");
                        Run.SetColor();
                        Run.Anim(" option does not exist.\r\nour best specialists are on it.");
                        Run.Wait(5_000);
                    break;
                    default:
                        Run.Anim("really? as if it's that hard to pick one of the ");
                        Run.SetColor(Run.Colors.Yellow, Run.Colors.DarkRed);
                        Run.Anim("available");
                        Run.SetColor();
                        Run.Anim(" numbers.");
                        Run.Wait(5_000);
                    break;
                }
                Run.ClearAll();
            }while(_menuOption != 1);
            CursorGame(3);
            PrintFinalCode();
            ExitProgram();
        }
        /// <summary>play mini-game find the key</summary>
        protected static void CursorGame(int times = 3){
            Run.Anim("(use WASD to move)");
            int _c,
                _r,
                _i;
            for(int i = 0; i < times; i++){
                _c = random.Next(2, Run.InnerWidth);
                _r = random.Next(2, Run.InnerHeight);
                Run.SetColor(Run.Colors.Magenta);
                Run.Text("⚿", false, _c, _r); // unicode shows as ? (good enough)
                Run.SetCursorPos(random.Next(1, Run.InnerWidth + 1), random.Next(1, Run.InnerHeight + 1));
                Run.SetColor();
                Run.CursorSize = 100;
                Run.CursorShow = true;
                do{
                    switch(Run.GetChar(true)){
                        case 'w':
                            _i = Run.CursorY - 1;
                            if(_i < 1) _i = Run.InnerHeight;
                            Run.SetCursorPos(Run.CursorX, _i);
                        break;
                        case 'a':
                            _i = Run.CursorX - 1;
                            if(_i < 1) _i = Run.InnerWidth;
                            Run.SetCursorPos(_i, Run.CursorY);
                        break;
                        case 's':
                            _i = Run.CursorY + 1;
                            if(_i > Run.InnerHeight) _i = 1;
                            Run.SetCursorPos(Run.CursorX, _i);
                        break;
                        case 'd':
                            _i = Run.CursorX + 1;
                            if(_i > Run.InnerWidth) _i = 1;
                            Run.SetCursorPos(_i, Run.CursorY);
                        break;
                    }
                }while(Run.CursorX != _c || Run.CursorY != _r);
                Run.CursorShow = false;
                Run.Text(" ", false, _c, _r);
            }
            Run.SetColor();
            Run.CursorSize = 25;
            Run.CursorShow = false;
        }
        /// <summary>print final code on screen</summary>
        protected static void PrintFinalCode(){
            int _c = (Run.InnerWidth>>1) - 28,
                _r = (Run.InnerHeight>>1) - 6;
            Run.SetColor(Run.Colors.Blue);
            Run.Anim(@"    .----.                           _____              ", 1, '\0', true, _c, _r);
            Run.Anim(@"   / .--. \        .-''-.           /    /  ..-'''-.    ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"  ' '    ' '     .' .-.  )         /    /   \.-'''\ \   ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"  \ \    / /    / .'  / /         /    /           | |  ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"   `.`'--.'    (_/   / /         /    /         __/ /   ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"   / `'-. `.        / /         /    /  __     |_  '.   ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"  ' /    `. \      / /         /    /  |  |       `.  \ ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@" / /       \ '    . '         /    '   |  |         \ '.", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"| |         | |  / /    _.-')/    '----|  |---.      , |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"| |         | |.' '  _.'.-''/          |  |   |      | |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@" \ \       / //  /.-'_.'    '----------|  |---'     / ,'", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"  `.'-...-'.'/    _.'                  |  | -....--'  / ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"     `-...-'( _.-'                    /____\`.. __..-'  ", 1, '\0', true, _c, Run.CursorY + 1);
            Run.SetColor();
        }
        /// <summary>prints the main menu on screen as ascii-table</summary>
        protected static void PrintMenu(){
            LastSkiped = Run.Anim(@"+-------------------------+", 5, '\0');
            LastSkiped = Run.Anim(@"|    M A I N   M E N U    |",LastSkiped ? 1 : 5, '\0', true, 1, Run.CursorY + 1);
            LastSkiped = Run.Anim(@"+------------+------------+",LastSkiped ? 1 : 5, '\0', true, 1, Run.CursorY + 1);
            LastSkiped = Run.Anim(@"| 1 find key | 2 secret   |",LastSkiped ? 1 : 5, '\0', true, 1, Run.CursorY + 1);
            LastSkiped = Run.Anim(@"+------------+------------+",LastSkiped ? 1 : 5, '\0', true, 1, Run.CursorY + 1);
            LastSkiped = Run.Anim(@"| 3 OOO      | 4 WIP      |",LastSkiped ? 1 : 5, '\0', true, 1, Run.CursorY + 1);
            LastSkiped = Run.Anim(@"+------------+------------+",LastSkiped ? 1 : 5, '\0', true, 1, Run.CursorY + 1);
        }
        /// <summary>get a single number [0 to 9] and only returns when it receives one</summary>
        protected static byte GetSingleNumStrict(){
            byte _num;
            Run.ClearInput();
            Run.CursorSize = 100;
            Run.CursorShow = true;
            do{
                try{ _num = Run.GetSingleNum(true); }
                catch(FormatException){ _num = 10; }
            }while(_num == 10);
            Run.CursorSize = 25;
            Run.CursorShow = false;
            Run.SetColor(Run.Colors.Yellow);
            Run.Text($"{_num}\r\n");
            Run.SetColor();
            return _num;
        }
        /// <summary>final press-any-key and reset of console settings</summary>
        protected static void ExitProgram(){
            Run.SetCursorPos((Run.InnerWidth>>1) - 14, Run.InnerHeight>>1);
            Run.SetColor(Run.Colors.Black, Run.Colors.Gray);
            Run.AnyKey(" to exit", 0, 0);
            Run.ClearSavedCursors();
            Run.ResetColor();
            Run.ClearConsoleAll();
            Run.RevertFullscreen();
            Run.CursorShow = true;
        }
        /// <summary>gets the player name and sets it</summary>
        protected static void GetPlayerName(){
            LastSkiped = false;
            do{
                Run.SetColor();
                Run.CursorShow = false;
                if(Run.CursorY > Run.InnerHeight + (Run.InnerHeight>>2)) Run.ScrollAnim(Run.InnerHeight + (Run.InnerHeight>>2));
                LastSkiped = Run.Anim("\r\nplease enter your name below.\r\n>", LastSkiped ? 0 : 10);
                ushort _maxInput = (ushort)(Run.InnerWidth<<1);
                if(_maxInput <= 15) _maxInput = (ushort)100;
                Run.ClearInput();
                playerName = Run.GetInput(_maxInput);
                LastSkiped = Run.Anim("\r\nYou entered ");
                Run.SetColor(Run.Colors.Gray);
                LastSkiped = Run.Anim(playerName, LastSkiped ? 0 : 20);
                Run.SetColor();
                LastSkiped = Run.Anim(". Is that correct?\r\n", LastSkiped ? 0 : 20);
                Run.SetColor(Run.Colors.Red);
                LastSkiped = Run.Anim("WARNING: This can't be changed later.", LastSkiped ? 0 : 20);
                Run.SetColor();
                LastSkiped = Run.Anim("\r\n[y/n]\r\n>", LastSkiped ? 0 : 20);
                Run.SetColor(Run.Colors.Yellow);
            }while(!GetYNStrict());
            Run.SetColor();
        }
        /// <summary>gets the player age and sets it</summary>
        protected static void GetPlayerAge(){
            LastSkiped = false;
            do{
                Run.SetColor();
                Run.CursorShow = false;
                if(Run.CursorY > Run.InnerHeight + (Run.InnerHeight>>2)) Run.ScrollAnim(Run.InnerHeight + (Run.InnerHeight>>2));
                LastSkiped = Run.Anim("\r\nplease enter your age in years below.\r\n>", LastSkiped ? 0 : 10);
                Run.ClearInput();
                playerAge = GetSaveUInteger32();
                LastSkiped = Run.Anim("\r\nYou entered ");
                Run.SetColor(Run.Colors.Gray);
                LastSkiped = Run.Anim($"{playerAge}", LastSkiped ? 0 : 20);
                Run.SetColor();
                LastSkiped = Run.Anim(". Is that correct?\r\n", LastSkiped ? 0 : 20);
                Run.SetColor(Run.Colors.Red);
                LastSkiped = Run.Anim("WARNING: This can't be changed later.", LastSkiped ? 0 : 20);
                Run.SetColor();
                LastSkiped = Run.Anim("\r\n[y/n]\r\n>", LastSkiped ? 0 : 20);
                Run.SetColor(Run.Colors.Yellow);
            }while(!GetYNStrict());
            Run.SetColor();
        }
        /// <summary>gets the player height and sets it</summary>
        protected static void GetPlayerHeight(){
            LastSkiped = false;
            do{
                Run.SetColor();
                Run.CursorShow = false;
                if(Run.CursorY > Run.InnerHeight + (Run.InnerHeight>>2)) Run.ScrollAnim(Run.InnerHeight + (Run.InnerHeight>>2));
                LastSkiped = Run.Anim("\r\nplease enter your height in meters below.\r\n>", LastSkiped ? 0 : 10);
                Run.ClearInput();
                playerHeight = GetSaveUFloat();
                LastSkiped = Run.Anim("\r\nYou entered ");
                Run.SetColor(Run.Colors.Gray);
                LastSkiped = Run.Anim(Run.NumberFormatting(playerHeight, 2), LastSkiped ? 0 : 20);
                Run.SetColor();
                LastSkiped = Run.Anim(". Is that correct?\r\n", LastSkiped ? 0 : 20);
                Run.SetColor(Run.Colors.Red);
                LastSkiped = Run.Anim("WARNING: This can't be changed later.", LastSkiped ? 0 : 20);
                Run.SetColor();
                LastSkiped = Run.Anim("\r\n[y/n]\r\n>", LastSkiped ? 0 : 20);
                Run.SetColor(Run.Colors.Yellow);
            }while(!GetYNStrict());
            Run.SetColor();
        }
        /// <summary>prints the logo on screen with an animation</summary>
        protected static void PrintLogoAnim(){
            int _c = (Run.InnerWidth>>1) - 39,
                _r = (Run.InnerHeight>>1) - 4;
            Run.SetColor(Run.Colors.White);
            Run.Anim(@"+-----------------------------------------------------------------------+", 1, '\0', true, _c, _r);
            Run.Anim(@"| ___  ____       _     _           _____                       _       |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"| |  \/  (_)     | |   | |         /  __ \                     | |      |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"| | .  . |_  __ _| |__ | |_ _   _  | /  \/ ___  _ __  ___  ___ | | ___  |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"| | |\/| | |/ _` | '_ \| __| | | | | |    / _ \| '_ \/ __|/ _ \| |/ _ \ |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"| | |  | | | (_| | | | | |_| |_| | | \__/\ (_) | | | \__ \ (_) | |  __/ |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"| \_|  |_/_|\__, |_| |_|\__|\__, |  \____/\___/|_| |_|___/\___/|_|\___| |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"|            __/ |           __/ |                                      |", 1, '\0', true, _c, Run.CursorY + 1);
            Run.Anim(@"+---------- |___/ --------- |___/ --------------------------------------+", 1, '\0', true, _c, Run.CursorY + 1);
            Run.SetColor();
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.up);
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.right);
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.down);
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.down);
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.left);
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.left);
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.up);
            Run.ScrollAnim(2, 50, false, Run.ScrollDirection.right);
            Run.Scroll(_c - 1, true, Run.ScrollDirection.left);
            Run.Scroll(_r - 1, true, Run.ScrollDirection.up);
            Run.ClearInput();
        }
        /// <summary>read input and checks for a whole 32bit number and only returns if input is correct - print message if input is wrong and asks for another try</summary>
        public static UInt32 GetSaveUInteger32(){
            Int32 _tmp = -1;
            do{
                try{
                    Run.SetColor(Run.Colors.Yellow);
                    Run.CursorShow = true;
                    _tmp = Run.GetIntNum();
                    Run.SetColor();
                    Run.CursorShow = false;
                    if(_tmp < 0) Run.Anim("\r\nnegative numbers aren't allowed, try again: ", 1);
                }catch(FormatException){
                    Run.CursorShow = false;
                    Run.Anim("\r\nonly whole numbers may be entered, try again: ", 1);
                }catch(OverflowException){
                    Run.CursorShow = false;
                    Run.Anim("\r\nthe number is to long, try again: ", 1);
                }
            }while(_tmp < 0);
            return (UInt32)_tmp;
        }
        /// <summary>read input and checks for a positive float number and only returns if input is correct - print message if input is wrong and asks for another try</summary>
        public static float GetSaveUFloat(){
            float _tmp = -1F;
            Run.CursorSize = 25;
            do{
                try{
                    Run.SetColor(Run.Colors.Yellow);
                    Run.CursorShow = true;
                    _tmp = (float)Run.GetNum();
                    Run.SetColor();
                    Run.CursorShow = false;
                    if(_tmp < 0F) Run.Anim("\r\nnegative numbers aren't allowed, try again: ", 1);
                }catch(FormatException){
                    Run.SetColor();
                    Run.CursorShow = false;
                    Run.Anim("\r\nonly numbers may be entered, try again: ", 1);
                }catch(OverflowException){
                    Run.SetColor();
                    Run.CursorShow = false;
                    Run.Anim("\r\nthe number is to long, try again: ", 1);
                }
            }while(_tmp < 0F);
            return _tmp;
        }
        /// <summary>get either 'y' or 'n' and only returns when it receives one of them</summary>
        public static bool GetYNStrict(){
            char _yn;
            Run.ClearInput();
            Run.CursorSize = 100;
            Run.CursorShow = true;
            do _yn = Run.GetChar(true);
            while(!(_yn == 'y' || _yn == 'n'));
            Run.CursorSize = 25;
            Run.CursorShow = false;
            Run.SetColor(Run.Colors.Yellow);
            Run.Text($"{_yn}");
            Run.SetColor();
            return _yn == 'y';
        }
        /// <summary>slowly "deletes" the characters on console</summary>
        /// <param name="num">number of characters to delete/overide with ' ' - defaults to 1</param>
        /// <param name="time">delay while writing - defaults to 200</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void DeleteLast(int num = 1, int time = 200){
            String delete = "";
            for(; num > 0; num--) delete += "\b \b";
            Run.Anim(delete, time, '\0', false);
        }
        /// <summary>clears screen width a bar</summary>
        /// <param name="width">width of the bar - defaults to 3</param>
        /// <param name="dir">direction of bar - defaults to <see cref="Run.ScrollDirection.down"/></param>
        /// <param name="time">delay in ms - defaults to 3</param>
        /// <param name="ch">char of bar - defaults to '█' (unicode 0x2588 full block)</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void ClearBar(int width = 3, Run.ScrollDirection dir = Run.ScrollDirection.down, int time = 3, char ch = '█'){
            switch(dir){
                case Run.ScrollDirection.down:
                    for(int i = 1; i <= width; i++){
                        Run.ClearRow(i, ch);
                        Run.Wait(time);
                    }
                    for(int i = 1; i <= Run.InnerHeight; i++){
                        if(i < Run.InnerHeight - width) Run.ClearRow(i + width, ch);
                        Run.ClearRow(i, ' ');
                        Run.Wait(time);
                    }
                break;
                case Run.ScrollDirection.up:
                    for(int i = 0; i < width; i++){
                        Run.ClearRow(Run.InnerHeight - i, ch);
                        Run.Wait(time);
                    }
                    for(int i = Run.InnerHeight; i > 0; i--){
                        if(i > width) Run.ClearRow(i - width, ch);
                        Run.ClearRow(i, ' ');
                        Run.Wait(time);
                    }
                break;
                case Run.ScrollDirection.right:
                    for(int i = 1; i <= width; i++){
                        Run.ClearCol(i, ch);
                        Run.Wait(time);
                    }
                    for(int i = 1; i <= Run.InnerWidth; i++){
                        if(i < Run.InnerWidth - width) Run.ClearCol(i + width, ch);
                        Run.ClearCol(i, ' ');
                        Run.Wait(time);
                    }
                break;
                case Run.ScrollDirection.left:
                    for(int i = 0; i < width; i++){
                        Run.ClearCol(Run.InnerWidth - i, ch);
                        Run.Wait(time);
                    }
                    for(int i = Run.InnerWidth; i > 0; i--){
                        if(i > width) Run.ClearCol(i - width, ch);
                        Run.ClearCol(i, ' ');
                        Run.Wait(time);
                    }
                break;
            }
        }
    }
}

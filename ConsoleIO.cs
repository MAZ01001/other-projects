// TODO formatting and refactoring

using System;//for datatypes/namespaces, Console object, ConsoleColor, Convert object and exceptions (the basics)
using System.Threading;//for Sleep (Wait)
using System.Globalization;//for NumberFormatInfo (NumberFormatting)
using System.Collections.Generic;//for List<int[]> (cursor save slots)
using System.Runtime.InteropServices;//for DllImports (fullscreen)

namespace ConsoleIO{
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments#recommended-tags
    /// <summary>a lot of static methods for creating a console based text adventure (64bit win7+)</summary>
    public static class Run{
        /// <summary>
        ///     gets the cursor height in percent<br/>
        ///     see also:<br/>
        ///     <seealso cref="SetCursorSize"/><br/>
        ///     <seealso cref="GetCursorShow"/><br/>
        ///     <seealso cref="SetCursorShow"/><br/>
        ///     <seealso cref="ToggleCursorShow"/>
        /// </summary>
        /// <return>the height in percent - [1-100]</return>
        public static Int32 GetCursorSize(){return Console.CursorSize;}
        /// <summary>
        ///     sets the cursor height in percent<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetCursorSize"/><br/>
        ///     <seealso cref="SetCursorShow"/><br/>
        ///     <seealso cref="GetCursorShow"/>
        /// </summary>
        /// <param name="size">the height in percent - [1-100] - default [25]</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name=""/> is not in range [1-100]</exception>
        public static void SetCursorSize(Int32 size=25){
            if(size<1||size>100){throw new ArgumentOutOfRangeException(nameof(size),"must be in range [1-100]");}
            Console.CursorSize=size;
        }
        /// <summary>used as directions for <see cref="Scroll"/>, <see cref="ScrollAnim"/> and <see cref="ScrollOnce"/></summary>
        public enum ScrollDirection{
            up=0,
            down=1,
            left=2,
            right=3
        }
        /// <summary>for <see cref="Scroll"/> and <see cref="ScrollAnim"/></summary>
        private static void ScrollOnce(ScrollDirection dir=ScrollDirection.up,bool cm=true){
            switch(dir){
                case ScrollDirection.up:
                    if(cm){int _check=GetCurrentRow()-1;
                        if(_check<1){throw new ArgumentOutOfRangeException();}
                        else{SetCursorPos(GetCurrentCol(),_check);}
                    }Console.MoveBufferArea(
                        1,2,GetInnerWidth(),GetInnerHeight()-1,1,1,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
                case ScrollDirection.down:
                    if(cm){int _check=GetCurrentRow()+1;
                        if(_check>GetInnerHeight()){throw new ArgumentOutOfRangeException();}
                        else{SetCursorPos(GetCurrentCol(),_check);}
                    }Console.MoveBufferArea(
                        1,1,GetInnerWidth(),GetInnerHeight()-1,1,2,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
                case ScrollDirection.left:
                    if(cm){int _check=GetCurrentCol()-1;
                        if(_check<1){throw new ArgumentOutOfRangeException();}
                        else{SetCursorPos(_check,GetCurrentRow());}
                    }Console.MoveBufferArea(
                        2,1,GetInnerWidth()-1,GetInnerHeight(),1,1,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
                case ScrollDirection.right:
                    if(cm){int _check=GetCurrentCol()+1;
                        if(_check>GetInnerWidth()){throw new ArgumentOutOfRangeException();}
                        else{SetCursorPos(_check,GetCurrentRow());}
                    }Console.MoveBufferArea(
                        1,1,GetInnerWidth()-1,GetInnerHeight(),2,1,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
            }
        }
        /// <summary>scrolls the content inside borders by <paramref name="amount"/> in <see cref="ScrollDirection"/> with <paramref name="ms"/> delay<br/></summary>
        /// <param name="amount">how moch rows to scroll - [1-(<see cref="GetInnerHeight"/>-1)] - default [1]</param>
        /// <param name="ms">time in milliseconds for delay - must be positive integer</param>
        /// <param name="cursorMove">if true moves the cursor by same <paramref name="amount"/> in same <paramref name="direction"/> - default [true]</param>
        /// <param name="direction">in which direction to scroll - [<see cref="ScrollDirection"/>] - default [<see cref="ScrollDirection.up"/>]</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="amount"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="cursorMove"/> is true but can not move the cursor <paramref name="amount"/> in <paramref name="direction"/></exception>
        public static void ScrollAnim(int amount=1,int ms=10,bool cursorMove=true,ScrollDirection direction=ScrollDirection.up){
            if(amount<0){throw new ArgumentOutOfRangeException(nameof(amount),"must be a positive integer");}
            if(ms<0){throw new ArgumentOutOfRangeException(nameof(ms),"must be a positive integer");}
            try{
                if(ms==0){Scroll(amount,cursorMove,direction);return;}
                for(;amount>0;amount--){ScrollOnce(direction,cursorMove);Wait(ms);}
            }catch(ArgumentOutOfRangeException){throw new ArgumentOutOfRangeException(nameof(cursorMove),"can not move the cursor that much with [cursorMove] on");}
        }
        /// <summary>scrolls the content inside borders by <paramref name="amount"/> in <see cref="ScrollDirection"/><br/></summary>
        /// <param name="amount">how moch rows to scroll - [1-(<see cref="GetInnerHeight"/>-1)] - default [1]</param>
        /// <param name="cursorMove">if true moves the cursor by same <paramref name="amount"/> in same <paramref name="direction"/> - default [true]</param>
        /// <param name="direction">in which direction to scroll - [<see cref="ScrollDirection"/>] - default [<see cref="ScrollDirection.up"/>]</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="amount"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="cursorMove"/> is true but can not move the cursor <paramref name="amount"/> in <paramref name="direction"/></exception>
        public static void Scroll(int amount=1,bool cursorMove=true,ScrollDirection direction=ScrollDirection.up){
            if(amount<0){throw new ArgumentOutOfRangeException(nameof(amount),"must be a positive integer");}
            switch(direction){
                case ScrollDirection.up:
                    if(cursorMove){int _check=GetCurrentRow()-amount;
                        if(_check<1){throw new ArgumentOutOfRangeException(nameof(cursorMove),"can not move the cursor that much with [cursorMove] on");}
                        else{SetCursorPos(GetCurrentCol(),_check);}
                    }Console.MoveBufferArea(
                        1,1+amount,GetInnerWidth(),GetInnerHeight()-amount,1,1,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
                case ScrollDirection.down:
                    if(cursorMove){int _check=GetCurrentRow()+amount;
                        if(_check>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(cursorMove),"can not move the cursor that much with [cursorMove] on");}
                        else{SetCursorPos(GetCurrentCol(),_check);}
                    }Console.MoveBufferArea(
                        1,1,GetInnerWidth(),GetInnerHeight()-amount,1,1+amount,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
                case ScrollDirection.left:
                    if(cursorMove){int _check=GetCurrentCol()-amount;
                        if(_check<1){throw new ArgumentOutOfRangeException(nameof(cursorMove),"can not move the cursor that much with [cursorMove] on");}
                        else{SetCursorPos(_check,GetCurrentRow());}
                    }Console.MoveBufferArea(
                        1+amount,1,GetInnerWidth()-amount,GetInnerHeight(),1,1,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
                case ScrollDirection.right:
                    if(cursorMove){int _check=GetCurrentCol()+amount;
                        if(_check>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(cursorMove),"can not move the cursor that much with [cursorMove] on");}
                        else{SetCursorPos(_check,GetCurrentRow());}
                    }Console.MoveBufferArea(
                        1,1,GetInnerWidth()-amount,GetInnerHeight(),1+amount,1,
                        ' ',Console.ForegroundColor,Console.BackgroundColor
                    );break;
            }
        }
        /// <summary>plays sound with given frequency and duration (at system volume!)</summary>
        /// <param name="freq">the frequancy of the tone in Hertz - [37-32767] - default [800]</param>
        /// <param name="time">the duration of the tone in milliseconds - must be positive integer - default [200]</param>
        /// <param name="wait">if true waits for the tone to end - default [true]</param>
        /// <exception cref="freq">if <paramref name="freq"/> is not in range [37-32767]</exception>
        /// <exception cref="time">if <paramref name="time"/> is not a positive integer</exception>
        public static void PlayTone(Int32 freq=800,Int32 time=200,bool wait=true){
            if(freq<37||freq>32767){throw new ArgumentOutOfRangeException(nameof(freq),"must be in range [37-32767]");}
            if(time<0){throw new ArgumentOutOfRangeException(nameof(time),"must be a positive integer");}
            if(time==0){return;}
            Console.Beep(freq,time);//~ no volume option - better options with [new System.Media.SoundPlayer(@"c:\mywavfile.wav").Play();] but also neds soundfile ~ or byte stream !
            if(wait){Wait(time);}
        }
        /// <summary>
        ///     get the current cursor row(top) position<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetCurrentCol"/><br/>
        ///     <seealso cref="SetCursorPos"/><br/>
        ///     <seealso cref="GetInnerWidth"/><br/>
        ///     <seealso cref="GetInnerHeight"/>
        /// </summary>
        /// <return>the current cursor row(top) position</return>
        public static Int32 GetCurrentRow(){return Console.CursorTop;}
        /// <summary>
        ///     get the current cursor col(left) position<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetCurrentRow"/><br/>
        ///     <seealso cref="SetCursorPos"/><br/>
        ///     <seealso cref="GetInnerWidth"/><br/>
        ///     <seealso cref="GetInnerHeight"/>
        /// </summary>
        /// <return>the current cursor col(left) position</return>
        public static Int32 GetCurrentCol(){return Console.CursorLeft;}
        /// <summary>
        ///     get the width available inside the borders<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetInnerHeight"/><br/>
        ///     <seealso cref="GetLargestWidth"/><br/>
        ///     <seealso cref="GetLargestHeight"/><br/>
        ///     <seealso cref="SetConsoleSize"/><br/>
        ///     <seealso cref="GetCurrentCol"/><br/>
        ///     <seealso cref="GetCurrentRow"/><br/>
        ///     <seealso cref="SetCursorPos"/>
        /// </summary>
        /// <return>the current width available</return>
        public static Int32 GetInnerWidth(){return Console.WindowWidth-2;}
        /// <summary>
        ///     get the height available inside the borders<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetInnerWidth"/><br/>
        ///     <seealso cref="GetLargestHeight"/><br/>
        ///     <seealso cref="GetLargestWidth"/><br/>
        ///     <seealso cref="SetConsoleSize"/><br/>
        ///     <seealso cref="GetCurrentRow"/><br/>
        ///     <seealso cref="GetCurrentCol"/><br/>
        ///     <seealso cref="SetCursorPos"/>
        /// </summary>
        /// <return>the current height available</return>
        public static Int32 GetInnerHeight(){return Console.WindowHeight-2;}
        /// <summary>
        ///     waits for input and returns it<br/>
        ///     custom input mode, can read [A-Z], [0-9], [Numpad0-9], space, '_', delete(left/right), move cursor(left/right/start/end) and tabulator(auto 1-4 spaces)<br/>
        ///     input ends either if <see cref="MaxInputSize"/> is reached or if ESC or Enter is pressed<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetNum"/><br/>
        ///     <seealso cref="GetIntNum"/><br/>
        ///     <seealso cref="GetSingleNum"/><br/>
        ///     <seealso cref="GetChar"/><br/>
        ///     <seealso cref="ClearInput"/>
        /// </summary>
        /// <param name="MaxInputSize">maximum characters allowet to enter - default [10'000]</param>
        /// <return>the string inputed</return>
        public static string GetInput(ushort MaxInputSize=10000){
            int _c=GetCurrentCol(),_r=GetCurrentRow(),_i=0;
            string _s="";
            bool _end=false;
            ConsoleKeyInfo _last;
            do{
                ClearInput();
                SetCursorShow(true);
                _last=Console.ReadKey(true);
                SetCursorShow(false);
                switch(_last.Key){
                    case ConsoleKey.Backspace:
                        if(_i>0){
                            Text(new string(' ',_s.Length),true,_c,_r);
                            _s=_s.Substring(0,_i-1)+_s.Substring(_i);_i--;
                        }
                        break;
                    case ConsoleKey.Delete:
                        if(_i<_s.Length){
                            Text(new string(' ',_s.Length),true,_c,_r);
                            _s=_s.Substring(0,_i)+_s.Substring(_i+1);
                        }
                        break;
                    case ConsoleKey.LeftArrow:if(_i>0){_i--;}break;
                    case ConsoleKey.RightArrow:if(_i<_s.Length){_i++;}break;
                    case ConsoleKey.Home:case ConsoleKey.UpArrow:_i=0;break;
                    case ConsoleKey.End:case ConsoleKey.DownArrow:_i=_s.Length;break;
                    case ConsoleKey.Tab:
                        int _tmp_t=4-(((_c+_i)%GetInnerWidth()+1)%4);
                        _s=_s.Substring(0,_i)+new String((char)ConsoleKey.Spacebar,_tmp_t)+_s.Substring(_i);
                        _i+=_tmp_t;
                        break;
                    case ConsoleKey.Enter:case ConsoleKey.Escape:_end=true;break;
                    default:
                        if(
                            (_last.Key>=ConsoleKey.A&&_last.Key<=ConsoleKey.Z)
                            ||(_last.Key>=ConsoleKey.D0&&_last.Key<=ConsoleKey.D9)
                            ||(_last.Key>=ConsoleKey.NumPad0&&_last.Key<=ConsoleKey.NumPad9)
                            ||_last.Key==ConsoleKey.Spacebar||_last.KeyChar=='_'||_last.KeyChar=='-'||_last.KeyChar=='.'||_last.KeyChar==','||_last.KeyChar=='\''
                        ){_s=_s.Substring(0,_i)+_last.KeyChar+_s.Substring(_i);_i++;}
                        break;
                }
                SetColor(Colors.yellow);
                Text(_s,true,_c,_r);
                SetColor();
                SetCursorPos(
                    (((_c-1)+_i)%GetInnerWidth())+1,
                    (_r+(int)(((_c-1)+_i)/GetInnerWidth()))
                );
            }while(!_end&&_s.Length<MaxInputSize);
            ClearInput();
            return _s;
        }
        /// <summary>
        ///     waits for keypress and returns it<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetNum"/><br/>
        ///     <seealso cref="GetIntNum"/><br/>
        ///     <seealso cref="GetSingleNum"/><br/>
        ///     <seealso cref="GetInput"/><br/>
        ///     <seealso cref="ClearInput"/>
        /// </summary>
        /// <param name="hide">if true does not show pressed char in console - default [false] → shows char in console</param>
        /// <return>the key pressed</return>
        public static char GetChar(bool hide=false){
            ConsoleKeyInfo _tmp_c=Console.ReadKey(true);
            if(!hide&&_tmp_c.Key!=ConsoleKey.Enter){Text(""+_tmp_c.KeyChar);}
            return _tmp_c.KeyChar;
        }
        /// <summary>
        ///     waits for input, converts it and then returns the number<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetNum"/><br/>
        ///     <seealso cref="GetIntNum"/><br/>
        ///     <seealso cref="GetChar"/><br/>
        ///     <seealso cref="GetInput"/><br/>
        ///     <seealso cref="ClearInput"/>
        /// </summary>
        /// <param name="hide">if true does not show pressed char in console - default [false] → shows char in console (also on error)</param>
        /// <return>the number</return>
        /// <exception cref="FormatException">if the input is not a number</exception>
        public static byte GetSingleNum(bool hide=false){
            char _tmp=GetChar(hide);
            if(_tmp<'0'||_tmp>'9'){throw new FormatException("input is not a number");}
            return (byte)(_tmp-'0');
        }
        /// <summary>
        ///     waits for input, converts it and then returns the number<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetNum"/><br/>
        ///     <seealso cref="GetSingleNum"/><br/>
        ///     <seealso cref="GetChar"/><br/>
        ///     <seealso cref="GetInput"/><br/>
        ///     <seealso cref="ClearInput"/>
        /// </summary>
        /// <return>the number</return>
        /// <exception cref="FormatException">if the input is in wrong format</exception>
        /// <exception cref="OverflowException">if the input is too long to be converted</exception>
        public static int GetIntNum(){return Convert.ToInt32(GetInput());}
        /// <summary>
        ///     waits for input, converts it and then returns the number<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetIntNum"/><br/>
        ///     <seealso cref="GetSingleNum"/><br/>
        ///     <seealso cref="GetChar"/><br/>
        ///     <seealso cref="GetInput"/><br/>
        ///     <seealso cref="ClearInput"/>
        /// </summary>
        /// <return>the number</return>
        /// <exception cref="FormatException">if the input is in wrong format</exception>
        /// <exception cref="OverflowException">if the input is too long to be converted</exception>
        public static double GetNum(){return Convert.ToDouble(GetInput());}
        /// <summary>
        ///     clears the input stream<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetNum"/><br/>
        ///     <seealso cref="GetIntNum"/><br/>
        ///     <seealso cref="GetSingleNum"/><br/>
        ///     <seealso cref="GetChar"/><br/>
        ///     <seealso cref="GetInput"/>
        /// </summary>
        public static void ClearInput(){while(Console.KeyAvailable){GetChar(true);}}
        /// <summary>
        ///     get the maximum width the console could be set at<br/>
        ///     for <see cref="SetConsoleSize"/><br/>
        ///     see also:<br/>
        ///     <seealso cref="GetLargestHeight"/><br/>
        ///     <seealso cref="SetConsoleSize"/>
        /// </summary>
        /// <return>the maximum width the console could be set at</return>
        public static Int32 GetLargestWidth(){return Console.LargestWindowWidth;}
        /// <summary>
        ///     get the maximum height the console could be set at<br/>
        ///     for <see cref="SetConsoleSize"/><br/>
        ///     see also:<br/>
        ///     <seealso cref="GetLargestWidth"/><br/>
        ///     <seealso cref="SetConsoleSize"/>
        /// </summary>
        /// <return>the maximum height the console could be set at</return>
        public static Int32 GetLargestHeight(){return Console.LargestWindowHeight;}
        /// <summary>
        ///     sets the size of the console window<br/>
        ///     in character count<br/>
        ///     see also:<br/>
        ///     <seealso cref="SetFullscreen"/><br/>
        ///     <seealso cref="RevertFullscreen"/>
        /// </summary>
        /// <param name="width">the new width of the console (in character count) - [3-<see cref="GetLargestWidth"/>]</param>
        /// <param name="height">the new height of the console (in character count) - [3-<see cref="GetLargestHeight"/>]</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="width"/> is not in range [3-<see cref="GetLargestWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="height"/> is not in range [3-<see cref="GetLargestHeight"/>]</exception>
        public static void SetConsoleSize(Int32 width,Int32 height){
            if(width<3||width>Console.LargestWindowWidth){throw new ArgumentOutOfRangeException(nameof(width),"must be in range [3-GetLargestWidth]");}
            if(height<3||height>Console.LargestWindowHeight){throw new ArgumentOutOfRangeException(nameof(height),"must be in range [3-GetLargestHeight]");}
            Console.SetWindowSize(width,height);
        }
        [DllImport("user32.dll",CharSet=CharSet.Auto,SetLastError=true)]
        /// <summary>imported (private) function to set window size (maximize/minimize/restore/hidden/...)<br/>used for <see cref="SetFullscreen"/> and <see cref="RevertFullscreen"/></summary>
        private static extern bool ShowWindow(IntPtr hWnd,int nCmdShow);
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow#parameters
        private const int W_MAXIMIZE=3;
        private const int W_RESTORE=9;
        [DllImport("kernel32.dll",ExactSpelling=true)]
        /// <summary>imported (private) function to get the console window process handle<br/>used for <see cref="ShowWindow"/>'s first parameter</summary>
        private static extern IntPtr GetConsoleWindow();
        /// <summary>
        ///     make window fullscreen<br/>
        ///     see also:<br/>
        ///     <seealso cref="RevertFullscreen"/><br/>
        ///     <seealso cref="SetConsoleSize"/>
        /// </summary>
        public static void SetFullscreen(){ShowWindow(GetConsoleWindow(),W_MAXIMIZE);}
        /// <summary>
        ///     turn fullscreen off and restores the original window position and size<br/>
        ///     see also:<br/>
        ///     <seealso cref="SetFullscreen"/><br/>
        ///     <seealso cref="SetConsoleSize"/>
        /// </summary>
        public static void RevertFullscreen(){ShowWindow(GetConsoleWindow(),W_RESTORE);}
        /// <summary>
        ///     set visibility of the console cursor<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetCursorShow"/><br/>
        ///     <seealso cref="ToggleCursorShow"/><br/>
        ///     <seealso cref="SetCursorSize"/><br/>
        ///     <seealso cref="GetCursorSize"/>
        /// </summary>
        /// <param name="visible">if the cursor should be visible or not - default [true] → visible</param>
        public static void SetCursorShow(bool visible=true){Console.CursorVisible=visible;}
        /// <summary>
        ///     get visibility of the console cursor<br/>
        ///     see also:<br/>
        ///     <seealso cref="SetCursorShow"/><br/>
        ///     <seealso cref="ToggleCursorShow"/><br/>
        ///     <seealso cref="SetCursorSize"/><br/>
        ///     <seealso cref="GetCursorSize"/>
        /// </summary>
        /// <return>the cursor visibility - [true] → visible</return>
        public static bool GetCursorShow(){return Console.CursorVisible;}
        /// <summary>
        ///     toggles the visibility of the cursor bar<br/>
        ///     see also:<br/>
        ///     <seealso cref="SetCursorShow"/><br/>
        ///     <seealso cref="GetCursorShow"/><br/>
        ///     <seealso cref="SetCursorSize"/><br/>
        ///     <seealso cref="GetCursorSize"/>
        /// </summary>
        /// <return>the cursor visibility after toggle - [true] → visible</return>
        public static bool ToggleCursorShow(){Console.CursorVisible=!Console.CursorVisible;return Console.CursorVisible;}
        /// <summary>set the position of the cursor in the console</summary>
        /// <param name="col">column index (from left) - [1-<see cref="GetInnerWidth"/>]</param>
        /// <param name="row">row index (from top) - [1-<see cref="GetInnerHeight"/>]</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1-<see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1-<see cref="GetInnerHeight"/>]</exception>
        public static void SetCursorPos(Int32 col,Int32 row){
            if(col<1||col>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(col),"must be in range [1-innerWidth]");}
            if(row<1||row>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(row),"must be in range [1-innerHeight]");}
            Console.SetCursorPosition(col,row);
        }
        /// <summary>set the title of the console window</summary>
        /// <param name="title">title</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="title"/> is longer than 24500 characters</exception>
        public static void SetTitle(string title){
            if(title.Length>24500){throw new ArgumentOutOfRangeException(nameof(title),"must not be more than 24500 characters long");}
            Console.Title=title;
        }
        /// <summary>used as Colors for <see cref="SetColor"/></summary>
        public enum Colors{
            black=ConsoleColor.Black,               // 0
            darkBlue=ConsoleColor.DarkBlue,         // 1
            darkGreen=ConsoleColor.DarkGreen,       // 2
            darkCyan=ConsoleColor.DarkCyan,         // 3
            darkRed=ConsoleColor.DarkRed,           // 4
            darkMagenta=ConsoleColor.DarkMagenta,   // 5
            darkYellow=ConsoleColor.DarkYellow,     // 6
            gray=ConsoleColor.Gray,                 // 7
            darkGray=ConsoleColor.DarkGray,         // 8
            blue=ConsoleColor.Blue,                 // 9
            green=ConsoleColor.Green,               // A (10)
            cyan=ConsoleColor.Cyan,                 // B (11)
            red=ConsoleColor.Red,                   // C (12)
            magenta=ConsoleColor.Magenta,           // D (13)
            yellow=ConsoleColor.Yellow,             // E (14)
            white=ConsoleColor.White                // F (15)
        }
        /// <summary>
        ///     get the current console background color<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetCurrentForegroundColor"/><br/>
        ///     <seealso cref="Colors"/><br/>
        ///     <seealso cref="SetColor"/><br/>
        ///     <seealso cref="ResetColor"/>
        /// </summary>
        /// <return>the current background color</return>
        public static Colors GetCurrentBackgroundColor(){return (Colors)Console.BackgroundColor;}
        /// <summary>
        ///     get the current console foreground color<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetCurrentBackgroundColor"/><br/>
        ///     <seealso cref="Colors"/><br/>
        ///     <seealso cref="SetColor"/><br/>
        ///     <seealso cref="ResetColor"/>
        /// </summary>
        /// <return>the current foreground color</return>
        public static Colors GetCurrentForegroundColor(){return (Colors)Console.ForegroundColor;}
        /// <summary>
        ///     set the fore- and background color for the console<br/>
        ///     see also:<br/><seealso cref="ResetColor"/>
        /// </summary>
        /// <param name="fg">foreground color - default [<see cref="Colors.green"/>]</param>
        /// <param name="bg">background color - default [<see cref="Colors.black"/>]</param>
        public static void SetColor(Colors fg=Colors.green,Colors bg=Colors.black){Console.BackgroundColor=(ConsoleColor)bg;Console.ForegroundColor=(ConsoleColor)fg;}
        /// <summary>
        ///     resets the console color to original values<br/>
        ///     see also:<br/><seealso cref="SetColor"/>
        /// </summary>
        public static void ResetColor(){Console.ResetColor();}
        /// <summary>private cursor save slot list</summary>
        private static List<int[]> cursorSave=new List<int[]>(1);/* 1 (empty) inital entry */
        /// <summary>
        ///     saves current cursor position in cursor save slot list<br/>
        ///     see also:<br/>
        ///     <seealso cref="LoadCursor"/><br/>
        ///     <seealso cref="ClearSavedCursors"/>
        /// </summary>
        /// <param name="index">save slot index - overrides - default [-1] → auto add to end</param>
        /// <return>the index of the save slot used</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is not an available save slot</exception>
        public static int SaveCursor(int index=-1){
            if(index==-1||index==cursorSave.Count){
                cursorSave.Add(new int[2]{GetCurrentCol(),GetCurrentRow()});
                return cursorSave.Count-1;
            }else if(index<0||index>=cursorSave.Count){throw new ArgumentOutOfRangeException(nameof(index),"must be an index of a possible cursor save slot");}
            cursorSave[index]=new int[2]{GetCurrentCol(),GetCurrentRow()};
            return index;
        }
        /// <summary>
        ///     loads cursor save slot<br/>
        ///     see also:<br/>
        ///     <seealso cref="SaveCursor"/><br/>
        ///     <seealso cref="ClearSavedCursors"/>
        /// </summary>
        /// <param name="index">save slot index</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is not an existing save slot</exception>
        public static void LoadCursor(int index){
            if(index<0||index>=cursorSave.Count){throw new ArgumentOutOfRangeException(nameof(index),"must be an index of an existing cursor save slot");}
            SetCursorPos(cursorSave[index][0],cursorSave[index][1]);
        }
        /// <summary>
        ///     clear all cursor save slots<br/>
        ///     see also:<br/>
        ///     <seealso cref="SaveCursor"/><br/>
        ///     <seealso cref="LoadCursor"/>
        /// </summary>
        public static void ClearSavedCursors(){cursorSave.Clear();}
        /// <summary>holds the program for the given delay in milliseconds</summary>
        /// <param name="ms">time in milliseconds for delay - must be positive integer</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        public static void Wait(int ms){
            if(ms<0){throw new ArgumentOutOfRangeException(nameof(ms),"must be a positive integer");}
            if(ms>0){Thread.Sleep(ms);}
        }
        /// <summary>
        ///     draws the border in the console with custom chars<br/>
        ///     custom left/top/bottom/right-bars and topleft/bottomleft/topright/bottomright-corners in that order<br/>
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
        public static void DrawBorder(char left,char top,char bottom,char right,char topLeft,char bottomLeft,char topRight,char bottomRight){
            if(left=='\r'||left=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(left));}
            if(top=='\r'||top=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(top));}
            if(bottom=='\r'||bottom=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(bottom));}
            if(right=='\r'||right=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(right));}
            if(topLeft=='\r'||topLeft=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(topLeft));}
            if(bottomLeft=='\r'||bottomLeft=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(bottomLeft));}
            if(topRight=='\r'||topRight=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(topRight));}
            if(bottomRight=='\r'||bottomRight=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(bottomRight));}
            Console.MoveBufferArea(GetInnerWidth()+1,0,1,1,0,0,topRight,(ConsoleColor)GetCurrentForegroundColor(),(ConsoleColor)GetCurrentBackgroundColor());
            Console.MoveBufferArea(0,GetInnerHeight()+1,1,1,0,0,bottomLeft,(ConsoleColor)GetCurrentForegroundColor(),(ConsoleColor)GetCurrentBackgroundColor());
            Console.MoveBufferArea(GetInnerWidth()+1,GetInnerHeight()+1,1,1,0,0,bottomRight,(ConsoleColor)GetCurrentForegroundColor(),(ConsoleColor)GetCurrentBackgroundColor());
            Console.MoveBufferArea(1,GetInnerHeight()+1,GetInnerWidth(),1,1,0,bottom,(ConsoleColor)GetCurrentForegroundColor(),(ConsoleColor)GetCurrentBackgroundColor());
            Console.CursorLeft=1;Console.CursorTop=0;
            Console.Write(new String(top,GetInnerWidth()));
            Console.MoveBufferArea(GetInnerWidth()+1,1,1,GetInnerHeight(),0,1,right,(ConsoleColor)GetCurrentForegroundColor(),(ConsoleColor)GetCurrentBackgroundColor());
            for(int i=1;i<=GetInnerHeight();i++){
                Console.CursorLeft=0;Console.CursorTop=i;
                Console.Write(left);
            }
            Console.CursorLeft=0;Console.CursorTop=0;
            Console.Write(topLeft);
            SetCursorPos(1,1);
        }
        /// <summary>
        ///     draws the border in the console with custom chars<br/>
        ///     custom horizontal/vertical-bars and topleft/bottomleft/topright/bottomright-corners in that order<br/>
        /// </summary>
        /// <param name="horizontal">char for the horizontal bars (top/bottom) of the console excluding the corners</param>
        /// <param name="vertical">char for the vertical bars (left/right) of the console excluding the corners</param>
        /// <param name="topleft">char for the topleft corner of the console</param>
        /// <param name="bottomleft">char for the bottomleft corner of the console</param>
        /// <param name="topright">char for the topright corner of the console</param>
        /// <param name="bottomright">char for the bottomright corner of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r' or '\n' as value</exception>
        public static void DrawBorder(char horizontal,char vertical,char topLeft,char bottomLeft,char topRight,char bottomRight){
            if(horizontal=='\r'||horizontal=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(horizontal));}
            if(vertical=='\r'||vertical=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(vertical));}
            if(topLeft=='\r'||topLeft=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(topLeft));}
            if(bottomLeft=='\r'||bottomLeft=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(bottomLeft));}
            if(topRight=='\r'||topRight=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(topRight));}
            if(bottomRight=='\r'||bottomRight=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(bottomRight));}
            DrawBorder(vertical,horizontal,horizontal,vertical,topLeft,bottomLeft,topRight,bottomRight);
        }
        /// <summary>
        ///     draws the border in the console with custom chars<br/>
        ///     custom horizontal/vertical-bars and corners in that order<br/>
        /// </summary>
        /// <param name="horizontal">char for the horizontal bars (top/bottom) of the console excluding the corners</param>
        /// <param name="vertical">char for the vertical bars (left/right) of the console excluding the corners</param>
        /// <param name="corners">char for all corners of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r' or '\n' as value</exception>
        public static void DrawBorder(char horizontal,char vertical,char corners){
            if(horizontal=='\r'||horizontal=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(horizontal));}
            if(vertical=='\r'||vertical=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(vertical));}
            if(corners=='\r'||corners=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(corners));}
            DrawBorder(vertical,horizontal,horizontal,vertical,corners,corners,corners,corners);
        }
        /// <summary>
        ///     draws the border in the console with custom chars<br/>
        ///     custom bars and corners in that order<br/>
        /// </summary>
        /// <param name="left">char for all bars of the console excluding the corners</param>
        /// <param name="corners">char for all corners of the console</param>
        /// <exception cref="ArgumentException">if any parameter has '\r' or '\n' as value</exception>
        public static void DrawBorder(char bars,char corners){
            if(bars=='\r'||bars=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(bars));}
            if(corners=='\r'||corners=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(corners));}
            DrawBorder(bars,bars,bars,bars,corners,corners,corners,corners);
        }
        /// <summary>draws the border in the console with custom char</summary>
        /// <param name="frame">char for the border of the console</param>
        /// <exception cref="ArgumentException">if <paramref name="frame"/> has '\r' or '\n' as value</exception>
        public static void DrawBorder(char frame){
            if(frame=='\r'||frame=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(frame));}
            DrawBorder(frame,frame,frame,frame,frame,frame,frame,frame);
        }
        /// <summary>
        ///     prints text animated without finishing line break, auto bound to inner border<br/>
        ///     can read '\n', '\r', '\t' and '\b'<br/>
        ///     '\0' does not print anything nor move the cursor but still delays<br/>
        ///     clears input before and after printing and if <see cref="interrupt"/> is pressed during printing sets <see cref="ms"/> to 0 and returns true<br/>
        ///     see also:<br/>
        ///     <seealso cref="AnimHor"/><br/>
        ///     <seealso cref="Text"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="ms">time in milliseconds to wait each character while printing - must be a positive integer - default [20]</param>
        /// <param name="interrupt">if this character is pressed during printing sets <see cref="ms"/> to 0 - default [' ']</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - default [true]</param>
        /// <param name="col">column index (left) where to start printing - [1-<see cref="GetInnerWidth"/>] - default [0] → use current</param>
        /// <param name="row">row index (top) where to start printing - [1-<see cref="GetInnerHeight"/>] - default [0] → use current</param>
        /// <return>true if printing was interrupted by pressing the <see cref="interrupt"/> key or <see cref="ms"/> is 0</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1-<see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1-<see cref="GetInnerHeight"/>]</exception>
        public static bool Anim(string outpt,int ms=20,char interrupt=' ',bool scrolling=true,int col=0,int row=0){
            if(ms<0){throw new ArgumentOutOfRangeException(nameof(ms),"must be a positive integer");}
            if(col==0){col=GetCurrentCol();}
            if(row==0){row=GetCurrentRow();}
            if(col<1||col>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(col),"must be in range [1-innerWidth]");}
            if(row<1||row>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(row),"must be in range [1-innerHeight]");}
            SetCursorPos(col,row);
            ClearInput();
            //// Array.ForEach<string>(outpt.Split('\n'),delegate(string line){});
            foreach(char key in outpt){
                if(key=='\n'){
                    //// SetCursorPos(GetCurrentCol(),(GetCurrentRow()+1)%GetInnerHeight());
                    if(GetCurrentRow()>=GetInnerHeight()){
                        if(scrolling){ScrollAnim((GetInnerHeight()-GetCurrentRow())+1);}
                        else{SetCursorPos(GetCurrentCol(),1);}
                    }else{SetCursorPos(GetCurrentCol(),GetCurrentRow()+1);}
                }else if(key=='\r'){SetCursorPos(1,GetCurrentRow());}
                else if(key=='\t'){
                    int _tmp_c=GetCurrentCol()+(4-((GetCurrentCol()+1)%4));
                    if(_tmp_c>GetInnerWidth()){SetCursorPos(_tmp_c-GetInnerWidth(),GetCurrentRow()+1);}
                    else{SetCursorPos(_tmp_c,GetCurrentRow());}
                }else if(key=='\b'){
                    int _tmp_b=GetCurrentCol()-1;
                    if(_tmp_b<1){SetCursorPos(GetInnerWidth(),GetCurrentRow()-1);}
                    else{SetCursorPos(_tmp_b,GetCurrentRow());}
                }else{
                    if(key!='\0'){
                        if(GetCurrentCol()>=GetInnerWidth()){
                            Console.Write(key);
                            if(GetCurrentRow()>=GetInnerHeight()){
                                if(scrolling){ScrollAnim((GetInnerHeight()-GetCurrentRow())+1);}
                                else{SetCursorPos(1,1);}
                            }else{SetCursorPos(1,GetCurrentRow()+1);}
                        }else{Console.Write(key);}
                    }
                    if(ms>0){
                        Wait(ms);
                        if(Console.KeyAvailable){if(GetChar(true)==interrupt){ms=0;}}
                    }
                }
            }
            ClearInput();
            return ms==0;
        }
        /// <summary>
        ///     prints text animated from top to bottom without finishing line break, auto bound to inner border<br/>
        ///     auto converts '\n', '\r', '\t' and '\b' to be on the vertical axis<br/>
        ///     '\0' does not print anything nor move the cursor but still delays<br/>
        ///     clears input before and after printing and if <see cref="interrupt"/> is pressed during printing sets <see cref="ms"/> to 0 and returns true<br/>
        ///     see also:<br/>
        ///     <seealso cref="Anim"/><br/>
        ///     <seealso cref="Text"/><br/>
        ///     <seealso cref="TextHor"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="ms">time in milliseconds to wait each character while printing - must be a positive integer - default [20]</param>
        /// <param name="interrupt">if this character is pressed during printing sets <see cref="ms"/> to 0 - default [' ']</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - default [true]</param>
        /// <param name="col">column index (left) where to start printing - [1-<see cref="GetInnerWidth"/>] - default [0] → use current</param>
        /// <param name="row">row index (top) where to start printing - [1-<see cref="GetInnerHeight"/>] - default [0] → use current</param>
        /// <return>true if printing was interrupted by pressing the <see cref="interrupt"/> key or <see cref="ms"/> is 0</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1-<see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1-<see cref="GetInnerHeight"/>]</exception>
        public static bool AnimHor(string outpt,int ms=20,char interrupt=' ',bool scrolling=true,int col=0,int row=0){
            if(ms<0){throw new ArgumentOutOfRangeException(nameof(ms),"must be a positive integer");}
            if(col==0){col=GetCurrentCol();}
            if(row==0){row=GetCurrentRow();}
            if(col<1||col>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(col),"must be in range [1-innerWidth]");}
            if(row<1||row>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(row),"must be in range [1-innerHeight]");}
            SetCursorPos(col,row);
            ClearInput();
            foreach(char key in outpt){
                if(key=='\n'){
                    if(GetCurrentCol()>=GetInnerWidth()){
                        if(scrolling){ScrollAnim((GetInnerWidth()-GetCurrentCol())+1);}
                        else{SetCursorPos(1,GetCurrentRow());}
                    }else{SetCursorPos(GetCurrentCol()+1,GetCurrentRow());}
                }else if(key=='\r'){SetCursorPos(GetCurrentCol(),1);}
                else if(key=='\t'){
                    int _tmp_r=GetCurrentRow()+(4-((GetCurrentRow()+1)%4));
                    if(_tmp_r>GetInnerHeight()){SetCursorPos(GetCurrentCol()+1,_tmp_r-GetInnerHeight());}
                    else{SetCursorPos(GetCurrentCol(),_tmp_r);}
                }else if(key=='\b'){
                    int _tmp_b=GetCurrentRow()-1;
                    if(_tmp_b<1){SetCursorPos(GetCurrentCol()-1,GetInnerHeight());}
                    else{SetCursorPos(GetCurrentCol(),_tmp_b);}
                }else{
                    if(key!='\0'){
                        if(GetCurrentRow()>=GetInnerHeight()){
                            Console.Write(key);
                            if(GetCurrentCol()>=GetInnerWidth()){
                                if(scrolling){ScrollAnim((GetInnerWidth()-GetCurrentCol())+1);}
                                else{SetCursorPos(1,1);}
                            }else{SetCursorPos(GetCurrentCol(),1);}
                        }else{
                            Console.Write(key);
                            SetCursorPos(GetCurrentCol()-1,GetCurrentRow()+1);
                        }
                    }
                    if(ms>0){
                        Wait(ms);
                        if(Console.KeyAvailable){if(GetChar(true)==interrupt){ms=0;}}
                    }
                }
            }
            ClearInput();
            return ms==0;
        }
        /// <summary>
        ///     prints text without finishing line break, auto bound to inner border<br/>
        ///     `\0` does not print anything nor move the cursor but still delays<br/>
        ///     see also:<br/>
        ///     <seealso cref="Anim"/><br/>
        ///     <seealso cref="TextHor"/><br/>
        ///     <seealso cref="AnimHor"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - default [true]</param>
        /// <param name="col">column index (left) where to start printing - [1-<see cref="GetInnerWidth"/>] - default [0] → use current</param>
        /// <param name="row">row index (top) where to start printing - [1-<see cref="GetInnerHeight"/>] - default [0] → use current</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1-<see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1-<see cref="GetInnerHeight"/>]</exception>
        public static void Text(string outpt,bool scrolling=true,int col=0,int row=0){
            if(col==0){col=GetCurrentCol();}
            if(row==0){row=GetCurrentRow();}
            if(col<1||col>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(col),"must be in range [1-innerWidth]");}
            if(row<1||row>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(row),"must be in range [1-innerHeight]");}
            Anim(outpt,0,' ',scrolling,col,row);
        }
        /// <summary>
        ///     prints text from top to bottom without finishing line break, auto bound to inner border<br/>,
        ///     auto converts '\n' and '\r' to be on the vertical axis<br/>
        ///     `\0` does not print anything nor move the cursor but still delays<br/>
        ///     see also:<br/>
        ///     <seealso cref="AnimHor"/><br/>
        ///     <seealso cref="Text"/><br/>
        ///     <seealso cref="Anim"/>
        /// </summary>
        /// <param name="outpt">output string</param>
        /// <param name="scrolling">if it should scroll up if the text is to long for screen height - default [true]</param>
        /// <param name="col">column index (left) where to start printing - [1-<see cref="GetInnerWidth"/>] - default [0] → use current</param>
        /// <param name="row">row index (top) where to start printing - [1-<see cref="GetInnerHeight"/>] - default [0] → use current</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1-<see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1-<see cref="GetInnerHeight"/>]</exception>
        public static void TextHor(string outpt,bool scrolling=true,int col=0,int row=0){
            if(col==0){col=GetCurrentCol();}
            if(row==0){row=GetCurrentRow();}
            if(col<1||col>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(col),"must be in range [1-innerWidth]");}
            if(row<1||row>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(row),"must be in range [1-innerHeight]");}
            AnimHor(outpt,0,' ',scrolling,col,row);
        }
        /// <summary>
        ///     clears a single column of the console by printing <paramref name="ch"/> repeadedly, excluding borders</br>
        ///     if on last column set the cursor to [1,1], else to the start of next column, afterwards</br>
        ///     see also:<br/>
        ///     <seealso cref="ClearRow"/><br/>
        ///     <seealso cref="ClearAll"/><br/>
        ///     <seealso cref="ClearAllAnim"/><br/>
        ///     <seealso cref="ClearConsoleAll"/>
        /// </summary>
        /// <param name="col">column index (top) of line to clear - [1-<see cref="GetInnerWidth"/>]</param>
        /// <param name="ch">char to clear the line with</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1-<see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        public static void ClearCol(Int32 col,char ch=' '){
            if(col<1||col>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(col),"must be in range [1-innerWidth]");}
            if(ch=='\r'||ch=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(ch));}
            for(int i=1;i<=GetInnerHeight();i++){
                SetCursorPos(col,i);
                Console.Write(ch);
            }
            if(col==GetInnerWidth()){SetCursorPos(1,1);}
            else{SetCursorPos(col+1,1);}
        }
        /// <summary>
        ///     clears a single row/line of the console by printing <paramref name="ch"/> repeatedly, excluding borders</br>
        ///     if on last row/line set the cursor to [1,1], else to the start of next row/line, afterwards</br>
        ///     see also:<br/>
        ///     <seealso cref="ClearCol"/><br/>
        ///     <seealso cref="ClearAll"/><br/>
        ///     <seealso cref="ClearAllAnim"/><br/>
        ///     <seealso cref="ClearConsoleAll"/>
        /// </summary>
        /// <param name="row">row index (left) of line to clear - [1-<see cref="GetInnerHeight"/>]</param>
        /// <param name="ch">char to clear the line with</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1-<see cref="GetInnerHeight"/>]</exception>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        public static void ClearRow(Int32 row,char ch=' '){
            if(row<1||row>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(row),"must be in range [1-innerHeight]");}
            if(ch=='\r'||ch=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(ch));}
            SetCursorPos(1,row);
            Console.Write(new string(ch,GetInnerWidth()));
            if(row==GetInnerHeight()){SetCursorPos(1,1);}
            else{SetCursorPos(1,row+1);}
        }
        /// <summary>used as directions for <see cref="ClearAllAnim"/></summary>
        [Flags] public enum AnimDir:byte{
            left=   1,
            up=     2,
            right=  4,
            down=   8,
            row=    left|right, // 5    (0101)
            col=    up|down,    // 10   (1010)
        }
        // TODO → ClearAllAnim → add diagonal-lines ~
        /// <summary>
        ///     like <see cref="ClearAll"/> but animated in horizontal/vertical lines<br/>
        ///     sets the cursor to [1,1] afterwards</br>
        ///     see also:<br/>
        ///     <seealso cref="ClearAll"/><br/>
        ///     <seealso cref="ClearRow"/><br/>
        ///     <seealso cref="ClearCol"/>
        /// </summary>
        /// <param name="ch">char to clear the console with</param>
        /// <param name="ms">duration to wait in milliseconds - must be a positive integer - default [20]</param>
        /// <param name="timing">where to wait, one of <c><see cref="AnimDir"/>[ col | row ]</c></param>
        /// <param name="dir">direction in wich to print, one of <c><see cref="AnimDir"/>[ left | right | up | down ]</c></param>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="ms"/> is not a positive integer</exception>
        /// <exception cref="ArgumentException">if <paramref name="timing"/> is not one of <c><see cref="AnimDir"/>[ col | row ]</c></exception>
        /// <exception cref="ArgumentException">if <paramref name="dir"/> is not one of <c><see cref="AnimDir"/>[ left | right | up | down ]</c></exception>
        /// <exception cref="InvalidOperationException">if the combination of <paramref name="timing"/> and <paramref name="dir"/> is not <c><see cref="AnimDir"/>[col + left/right] or <see cref="AnimDir"/>[row + up/down]</c></exception>
        public static void ClearAllAnim(char ch=' ',int ms=20,AnimDir timing=AnimDir.row,AnimDir dir=AnimDir.down){
            if(ch=='\r'||ch=='\n'){throw new ArgumentException("must not be '\\n' or '\\r'",nameof(ch));}
            if(ms<0){throw new ArgumentOutOfRangeException(nameof(ms),"must be a positive integer");}
            switch(timing){
                case AnimDir.row:case AnimDir.col:break;
                default:throw new ArgumentException("value must be one of AnimDir[ col | row ]",nameof(timing));
            }
            switch(dir){
                case AnimDir.up:case AnimDir.down:case AnimDir.left:case AnimDir.right:break;
                default:throw new ArgumentException("value must be one of AnimDir[ left | right | up | down ]",nameof(dir));
            }
            switch(timing|dir){
                case AnimDir.col|AnimDir.left:for(int i=GetInnerWidth();i>0;i--){ClearCol(i,ch);Wait(ms);}break;
                case AnimDir.col|AnimDir.right:for(int i=1;i<=GetInnerWidth();i++){ClearCol(i,ch);Wait(ms);}break;
                case AnimDir.row|AnimDir.up:for(int i=GetInnerHeight();i>0;i--){ClearRow(i,ch);Wait(ms);}break;
                case AnimDir.row|AnimDir.down:for(int i=1;i<=GetInnerHeight();i++){ClearRow(i,ch);Wait(ms);}break;
                default:throw new InvalidOperationException("combination of [timing | dir] is not AnimDir[col & left/right] or AnimDir[row & up/down]");
            }
            SetCursorPos(1,1);
        }
        /// <summary>
        ///     clears console by printing <paramref name="ch"/> in every position in console, excluding borders</br>
        ///     sets the cursor to [1,1] afterwards<br/>
        ///     see also:<br/>
        ///     <seealso cref="ClearAllAnim"/><br/>
        ///     <seealso cref="ClearRow"/><br/>
        ///     <seealso cref="ClearCol"/><br/>
        ///     <seealso cref="ClearConsoleAll"/>
        /// </summary>
        /// <param name="ch">char to clear the console with</param>
        /// <exception cref="ArgumentException">if <paramref name="ch"/> is '\r' or '\n'</exception>
        public static void ClearAll(char ch=' '){
            if(ch=='\r'||ch=='\n'){throw new ArgumentException("must not be '\\r' or '\\n'",nameof(ch));}
            ClearAllAnim(ch,0);
        }
        /// <summary>
        ///     clears all of console including borders</br>
        ///     sets the cursor to [1,1] afterwards<br/>
        ///     see also:<br/>
        ///     <seealso cref="ClearAll"/><br/>
        ///     <seealso cref="ClearAllAnim"/><br/>
        ///     <seealso cref="ClearRow"/><br/>
        ///     <seealso cref="ClearCol"/>
        /// </summary>
        public static void ClearConsoleAll(){
            Console.Clear();
            SetCursorPos(1,1);
        }
        /// <summary>
        ///     "press any key"-message<br/>
        ///     (key that is pressed is not shown)<br/>
        ///     see also:<br/>
        ///     <seealso cref="GetChar"/>
        /// </summary>
        /// <param name="add">added between "<< press any key" and " >>" as output</param>
        /// <param name="col">column index (left) where to start printing - [1-<see cref="GetInnerWidth"/>] - default [0] → use current</param>
        /// <param name="row">row index (top) where to start printing - [1-<see cref="GetInnerHeight"/>] - default [0] → use current</param>
        /// <param name="toggleCursor">if true toggles the cursors visibility after "press any key"</param>
        /// <param name="nextLine">cursor to next line after "press any key"</param>
        /// <return>char that is pressed</return>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="col"/> is not in range [1-<see cref="GetInnerWidth"/>]</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="row"/> is not in range [1-<see cref="GetInnerHeight"/>]</exception>
        public static char AnyKey(string add="",int col=0,int row=0,bool toggleCursor=false,bool nextLine=false){
            if(col==0){col=GetCurrentCol();}
            if(row==0){row=GetCurrentRow();}
            if(col<1||col>GetInnerWidth()){throw new ArgumentOutOfRangeException(nameof(col),"must be in range [1-innerWidth]");}
            if(row<1||row>GetInnerHeight()){throw new ArgumentOutOfRangeException(nameof(row),"must be in range [1-innerHeight]");}
            Anim("<< Press any key"+add+" >>"+(nextLine?"\n":""),5,'\0',true,col,row);
            if(toggleCursor){ToggleCursorShow();}
            ClearInput();
            return GetChar(true);
        }
        /// <summary>formatting a single number</summary>
        /// <param name="num">initial number</param>
        /// <param name="dec_dig">number of decimal places to return - default [0]→also no decimal point</param>
        /// <param name="dec_sep">decimal point string - default ["."]</param>
        /// <param name="grp_sep">group seperator string - default [" "]</param>
        /// <param name="grp_siz">group sizes array - default [null]→{0} (last entry loops)<br/>
        ///     Example:<br/>
        ///     {0}     →              1111111<br/>
        ///     {3}     →           33 333 333<br/>
        ///     {2,3,5} →      55 55555 333 22<br/>
        ///     {3,4,0} →  5555555555 4444 333<br/>
        /// </param>
        /// <return>the formatted number as string</return>
        public static string NumberFormatting(double num,Int32 dec_dig=0,string dec_sep=".",string grp_sep=" ",Int32[] grp_siz=null){
            /*
                https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#table
                $"{5000:N0}"        →   "5 000"
                $"{5000:N3}"        →   "5 000.000"
                $"{5000,10:N2}"     →   "  5 000.00"
                $"{5000:F3}"        →   "5000.000"
                $"{5:D4}"           →   "0005"
                $"{5000:E4}"        →   "5.0000E+003"
                $"{0.2345678:P4}"   →   "23.457%"
            */
            NumberFormatInfo nfi=new CultureInfo("en-US",false).NumberFormat;
            nfi.NumberDecimalDigits=dec_dig;
            nfi.NumberDecimalSeparator=dec_sep;
            nfi.NumberGroupSeparator=grp_sep;
            if(grp_siz==null){
                Int32[] no_grp={0};
                nfi.NumberGroupSizes=no_grp;
            }else{grp_siz.CopyTo(nfi.NumberGroupSizes,0);}
            nfi.NumberNegativePattern=1;
            return num.ToString("N",nfi);
        }
    }
    /// <summary>a small game to show some of the capabilities of the ConsoleIO text-game-"engine"</summary>
    public static class TestGame{
        /// <summary>for the animation-text if it is skiped with the interrupt-key(' ') and speed up further animation-text</summary>
        private static bool LastSkiped=false;
        /// <summary>player name as set by the user in the program</summary>
        protected static string playerName;
        /// <summary>player age in years as set by the user in the program</summary>
        protected static uint playerAge;
        /// <summary>player height in meters as set by the user in the program</summary>
        protected static float playerHeight;
        /// <summary>todays date-time during runtime</summary>
        protected static DateTime today;
        /// <summary>random number for the find-key mini-game at the end</summary>
        protected static Random random;
        /// <summary> Start the test game </summary>
        public static void Start(){
            random=new Random();
            today=DateTime.Now;
            Run.SetTitle("MIGHTY CONSOLE - C#");
            // Run.SetConsoleSize(120,50);
            Run.SetFullscreen();
            Run.SetCursorSize(25);
            Run.SetCursorShow(false);
            Run.ClearSavedCursors();
            Run.PlayTone(1000,200,false);
            Run.ClearConsoleAll();
            Run.PlayTone(1000,100,false);
            Run.SetColor(Run.Colors.white,Run.Colors.black);
            ClearBar(Run.GetInnerHeight()>>2,Run.ScrollDirection.down,2);
            Run.SetColor(Run.Colors.green);
            Run.DrawBorder('-','|','+');
            // timing may be different for each direction, console, and OS ~
            ClearBar(2,Run.ScrollDirection.right,0);// 3 - 0
            ClearBar(3,Run.ScrollDirection.left,0); // 4 - 0
            ClearBar(3,Run.ScrollDirection.down,4); // 2 - 1
            ClearBar(4,Run.ScrollDirection.up,4);   // 3 - 1
            PrintLogoAnim();
            LastSkiped=Run.Anim("\r\n\r\nMighty Console [Version 0.1.001]\r\nCopyright(c) 2021 MAZ.\r\n",10);
            LastSkiped=Run.Anim("GREETINGS NEW USER!",LastSkiped?0:10);
            LastSkiped=Run.Anim("\r\nThank you for installing the ",LastSkiped?0:5);
            LastSkiped=Run.Anim("MIGHTY CONSOLE !!!",LastSkiped?0:50);
            LastSkiped=Run.Anim("\r\nTo Begin,",LastSkiped?0:10);
            Run.ScrollAnim(12,200);
            GetPlayerName();
            GetPlayerAge();
            GetPlayerHeight();
            Run.ScrollAnim(Run.GetInnerWidth(),10,false,Run.ScrollDirection.left);
            Run.SetCursorPos(1,Run.GetInnerHeight()>>2);
            Run.Anim("so...",100,'\0');
            DeleteLast(5);
            Run.Anim("your name is ",100,'\0');
            Run.SetColor(Run.Colors.cyan);Run.Anim($"\"{playerName}\"\r\n",50);Run.SetColor();
            if(playerName.Length>15){Run.Anim("that's quite a dumb...",100,'\0');DeleteLast(7);Run.Anim("long name\r\n",100,'\0');}
            Run.Anim("and you are ",100,'\0');
            Run.SetColor(Run.Colors.cyan);Run.Anim($"{playerAge}Years",50);Run.SetColor();
            Run.Anim($" old\r\nthat means you are born in the year {today.Year-playerAge}\r\n",100,'\0');
            Run.Anim($"and {Run.NumberFormatting(playerHeight,2)}m height, wich means in order to reach the moon\r\nyou would need {Run.NumberFormatting(384400000/playerHeight,2)} clones of yourself on top of each other.",50);
            Run.Wait((int)5e3);
            ClearBar();
            Run.SetCursorPos(1,Run.GetInnerHeight()>>2);
            Run.SaveCursor(0);
            byte _menuOption;
            do{
                Run.LoadCursor(0);
                PrintMenu();// 1 find key | 2 secret | 3 OOO | 4 WIP
                LastSkiped=Run.Anim("\r\nchoose whisely.\r\n",20,'\0',true,1,Run.GetCurrentRow()+1);
                _menuOption=GetSingleNumStrict();
                switch(_menuOption){
                    case 1:Run.Anim("you did choose wisely.");Run.Wait((int)3e3);break;
                    case 2:Run.Anim("you did not choose wisely.");Run.Wait((int)3e3);break;
                    case 3:
                        Run.Anim("can't you read it ");
                        Run.SetColor(Run.Colors.blue);Run.Anim("clearly");Run.SetColor();
                        Run.Anim(" says Out Of Order.");
                        Run.Wait((int)5e3);
                        break;
                    case 4:
                        Run.SetColor(Run.Colors.yellow,Run.Colors.darkRed);Run.Anim("ERROR");Run.SetColor();
                        Run.Anim(" option does not exist.\r\nour best specialists are on it.");
                        Run.Wait((int)5e3);
                        break;
                    default:
                        Run.Anim("really? as if it's that hard to pick one of the ");
                        Run.SetColor(Run.Colors.yellow,Run.Colors.darkRed);Run.Anim("available");Run.SetColor();
                        Run.Anim(" numbers.");
                        Run.Wait((int)5e3);
                        break;
                }
                Run.ClearAll();
            }while(_menuOption!=1);
            CursorGame(3);
            PrintFinalCode();
            ExitProgram();
        }
        /// <summary>play mini-game find the key</summary>
        protected static void CursorGame(int times=3){
            Run.Anim("(use WASD to move)");
            int _c,_r,_i;
            for(int i=0;i<times;i++){
                _c=random.Next(2,Run.GetInnerWidth());
                _r=random.Next(2,Run.GetInnerHeight());
                Run.SetColor(Run.Colors.magenta);
                Run.Text("⚿",false,_c,_r);//unicode shows as ? (good enough)
                Run.SetCursorPos(random.Next(1,Run.GetInnerWidth()+1),random.Next(1,Run.GetInnerHeight()+1));
                Run.SetColor();Run.SetCursorSize(100);Run.SetCursorShow(true);
                do{
                    switch(Run.GetChar(true)){
                        case 'w':
                            _i=Run.GetCurrentRow()-1;
                            if(_i<1){_i=Run.GetInnerHeight();}
                            Run.SetCursorPos(Run.GetCurrentCol(),_i);
                            break;
                        case 'a':
                            _i=Run.GetCurrentCol()-1;
                            if(_i<1){_i=Run.GetInnerWidth();}
                            Run.SetCursorPos(_i,Run.GetCurrentRow());
                            break;
                        case 's':
                            _i=Run.GetCurrentRow()+1;
                            if(_i>Run.GetInnerHeight()){_i=1;}
                            Run.SetCursorPos(Run.GetCurrentCol(),_i);
                            break;
                        case 'd':
                            _i=Run.GetCurrentCol()+1;
                            if(_i>Run.GetInnerWidth()){_i=1;}
                            Run.SetCursorPos(_i,Run.GetCurrentRow());
                            break;
                    }
                }while(Run.GetCurrentCol()!=_c||Run.GetCurrentRow()!=_r);
                Run.SetCursorShow(false);
                Run.Text(" ",false,_c,_r);
            }
            Run.SetColor();Run.SetCursorSize(25);Run.SetCursorShow(false);
        }
        /// <summary>print final code on screen</summary>
        protected static void PrintFinalCode(){
            int _c=(Run.GetInnerWidth()>>1)-28,_r=(Run.GetInnerHeight()>>1)-6;
            Run.SetColor(Run.Colors.blue);
            Run.Anim(@"    .----.                           _____              ",1,'\0',true,_c,_r);
            Run.Anim(@"   / .--. \        .-''-.           /    /  ..-'''-.    ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"  ' '    ' '     .' .-.  )         /    /   \.-'''\ \   ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"  \ \    / /    / .'  / /         /    /           | |  ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"   `.`'--.'    (_/   / /         /    /         __/ /   ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"   / `'-. `.        / /         /    /  __     |_  '.   ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"  ' /    `. \      / /         /    /  |  |       `.  \ ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@" / /       \ '    . '         /    '   |  |         \ '.",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"| |         | |  / /    _.-')/    '----|  |---.      , |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"| |         | |.' '  _.'.-''/          |  |   |      | |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@" \ \       / //  /.-'_.'    '----------|  |---'     / ,'",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"  `.'-...-'.'/    _.'                  |  | -....--'  / ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"     `-...-'( _.-'                    /____\`.. __..-'  ",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.SetColor();
        }
        /// <summary>prints the main menu on screen as ascii-table</summary>
        protected static void PrintMenu(){
            LastSkiped=Run.Anim(@"+-------------------------+",5,'\0');
            LastSkiped=Run.Anim(@"|    M A I N   M E N U    |",LastSkiped?1:5,'\0',true,1,Run.GetCurrentRow()+1);
            LastSkiped=Run.Anim(@"+------------+------------+",LastSkiped?1:5,'\0',true,1,Run.GetCurrentRow()+1);
            LastSkiped=Run.Anim(@"| 1 find key | 2 secret   |",LastSkiped?1:5,'\0',true,1,Run.GetCurrentRow()+1);
            LastSkiped=Run.Anim(@"+------------+------------+",LastSkiped?1:5,'\0',true,1,Run.GetCurrentRow()+1);
            LastSkiped=Run.Anim(@"| 3 OOO      | 4 WIP      |",LastSkiped?1:5,'\0',true,1,Run.GetCurrentRow()+1);
            LastSkiped=Run.Anim(@"+------------+------------+",LastSkiped?1:5,'\0',true,1,Run.GetCurrentRow()+1);
        }
        /// <summary>get a single number [0-9] and only returns when it receives one</summary>
        protected static byte GetSingleNumStrict(){
            byte _num;Run.ClearInput();
            Run.SetCursorSize(100);Run.SetCursorShow(true);
            do{
                try{_num=Run.GetSingleNum(true);}
                catch(FormatException){_num=10;}
            }while(_num==10);
            Run.SetCursorSize(25);Run.SetCursorShow(false);
            Run.SetColor(Run.Colors.yellow);
            Run.Text(_num+"\r\n");
            Run.SetColor();
            return _num;
        }
        /// <summary>final press-any-key and reset of console settings</summary>
        protected static void ExitProgram(){
            Run.SetCursorPos((Run.GetInnerWidth()>>1)-14,Run.GetInnerHeight()>>1);
            Run.SetColor(Run.Colors.black,Run.Colors.gray);
            Run.AnyKey(" to exit",0,0);
            Run.ClearSavedCursors();
            Run.ResetColor();
            Run.ClearConsoleAll();
            Run.RevertFullscreen();
            Run.SetCursorShow(true);
        }
        /// <summary>gets the player name and sets it</summary>
        protected static void GetPlayerName(){
            LastSkiped=false;
            do{
                Run.SetColor();Run.SetCursorShow(false);
                if(Run.GetCurrentRow()>Run.GetInnerHeight()+(Run.GetInnerHeight()>>2)){Run.ScrollAnim(Run.GetInnerHeight()+(Run.GetInnerHeight()>>2));}
                LastSkiped=Run.Anim("\r\nplease enter your name below.\r\n>",LastSkiped?0:10);
                ushort _maxInput=(ushort)(Run.GetInnerWidth()<<1);
                if(_maxInput<=15){_maxInput=(ushort)100;}
                Run.ClearInput();playerName=Run.GetInput(_maxInput);
                LastSkiped=Run.Anim("\r\nYou entered ");
                Run.SetColor(Run.Colors.gray);LastSkiped=Run.Anim(playerName,LastSkiped?0:20);Run.SetColor();
                LastSkiped=Run.Anim(". Is that correct?\r\n",LastSkiped?0:20);
                Run.SetColor(Run.Colors.red);LastSkiped=Run.Anim("WARNING: This can't be changed later.",LastSkiped?0:20);Run.SetColor();
                LastSkiped=Run.Anim("\r\n[y/n]\r\n>",LastSkiped?0:20);
                Run.SetColor(Run.Colors.yellow);
            }while(!GetYNStrict());
            Run.SetColor();
        }
        /// <summary>gets the player age and sets it</summary>
        protected static void GetPlayerAge(){
            LastSkiped=false;
            do{
                Run.SetColor();Run.SetCursorShow(false);
                if(Run.GetCurrentRow()>Run.GetInnerHeight()+(Run.GetInnerHeight()>>2)){Run.ScrollAnim(Run.GetInnerHeight()+(Run.GetInnerHeight()>>2));}
                LastSkiped=Run.Anim("\r\nplease enter your age in years below.\r\n>",LastSkiped?0:10);
                Run.ClearInput();playerAge=GetSaveUInteger32();
                LastSkiped=Run.Anim("\r\nYou entered ");
                Run.SetColor(Run.Colors.gray);LastSkiped=Run.Anim(""+playerAge,LastSkiped?0:20);Run.SetColor();
                LastSkiped=Run.Anim(". Is that correct?\r\n",LastSkiped?0:20);
                Run.SetColor(Run.Colors.red);LastSkiped=Run.Anim("WARNING: This can't be changed later.",LastSkiped?0:20);Run.SetColor();
                LastSkiped=Run.Anim("\r\n[y/n]\r\n>",LastSkiped?0:20);
                Run.SetColor(Run.Colors.yellow);
            }while(!GetYNStrict());
            Run.SetColor();
        }
        /// <summary>gets the player height and sets it</summary>
        protected static void GetPlayerHeight(){
            LastSkiped=false;
            do{
                Run.SetColor();Run.SetCursorShow(false);
                if(Run.GetCurrentRow()>Run.GetInnerHeight()+(Run.GetInnerHeight()>>2)){Run.ScrollAnim(Run.GetInnerHeight()+(Run.GetInnerHeight()>>2));}
                LastSkiped=Run.Anim("\r\nplease enter your height in meters below.\r\n>",LastSkiped?0:10);
                Run.ClearInput();playerHeight=GetSaveUFloat();
                LastSkiped=Run.Anim("\r\nYou entered ");
                Run.SetColor(Run.Colors.gray);LastSkiped=Run.Anim(Run.NumberFormatting(playerHeight,2),LastSkiped?0:20);Run.SetColor();
                LastSkiped=Run.Anim(". Is that correct?\r\n",LastSkiped?0:20);
                Run.SetColor(Run.Colors.red);LastSkiped=Run.Anim("WARNING: This can't be changed later.",LastSkiped?0:20);Run.SetColor();
                LastSkiped=Run.Anim("\r\n[y/n]\r\n>",LastSkiped?0:20);
                Run.SetColor(Run.Colors.yellow);
            }while(!GetYNStrict());
            Run.SetColor();
        }
        /// <summary>read input and checks for a whole 32bit number and only returns if input is correct - print message if input is wrong and asks for another try</summary>
        public static UInt32 GetSaveUInteger32(){
            Int32 _tmp=-1;
            do{
                try{
                    Run.SetColor(Run.Colors.yellow);Run.SetCursorShow(true);
                    _tmp=Run.GetIntNum();
                    Run.SetColor();Run.SetCursorShow(false);
                    if(_tmp<0){Run.Anim("\r\nnegative numbers aren't allowed, try again: ",1);}
                }
                catch(FormatException){Run.SetCursorShow(false);Run.Anim("\r\nonly whole numbers may be entered, try again: ",1);}
                catch(OverflowException){Run.SetCursorShow(false);Run.Anim("\r\nthe number is to long, try again: ",1);}
            }while(_tmp<0);
            return (UInt32)_tmp;
        }
        /// <summary>read input and checks for a positive float number and only returns if input is correct - print message if input is wrong and asks for another try</summary>
        public static float GetSaveUFloat(){
            float _tmp=-1F;
            Run.SetCursorSize(25);
            do{
                try{
                    Run.SetColor(Run.Colors.yellow);Run.SetCursorShow(true);
                    _tmp=(float)Run.GetNum();
                    Run.SetColor();Run.SetCursorShow(false);
                    if(_tmp<0F){Run.Anim("\r\nnegative numbers aren't allowed, try again: ",1);}
                }
                catch(FormatException){Run.SetColor();Run.SetCursorShow(false);Run.Anim("\r\nonly numbers may be entered, try again: ",1);}
                catch(OverflowException){Run.SetColor();Run.SetCursorShow(false);Run.Anim("\r\nthe number is to long, try again: ",1);}
            }while(_tmp<0F);
            return _tmp;
        }
        /// <summary>prints the logo on screen with an animation</summary>
        protected static void PrintLogoAnim(){
            int _c=(Run.GetInnerWidth()>>1)-39,_r=(Run.GetInnerHeight()>>1)-4;
            Run.SetColor(Run.Colors.white);
            Run.Anim(@"+-----------------------------------------------------------------------+",1,'\0',true,_c,_r);
            Run.Anim(@"| ___  ____       _     _           _____                       _       |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"| |  \/  (_)     | |   | |         /  __ \                     | |      |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"| | .  . |_  __ _| |__ | |_ _   _  | /  \/ ___  _ __  ___  ___ | | ___  |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"| | |\/| | |/ _` | '_ \| __| | | | | |    / _ \| '_ \/ __|/ _ \| |/ _ \ |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"| | |  | | | (_| | | | | |_| |_| | | \__/\ (_) | | | \__ \ (_) | |  __/ |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"| \_|  |_/_|\__, |_| |_|\__|\__, |  \____/\___/|_| |_|___/\___/|_|\___| |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"|            __/ |           __/ |                                      |",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.Anim(@"+---------- |___/ --------- |___/ --------------------------------------+",1,'\0',true,_c,Run.GetCurrentRow()+1);
            Run.SetColor();
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.up);
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.right);
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.down);
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.down);
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.left);
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.left);
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.up);
            Run.ScrollAnim(2,50,false,Run.ScrollDirection.right);
            Run.Scroll(_c-1,true,Run.ScrollDirection.left);
            Run.Scroll(_r-1,true,Run.ScrollDirection.up);
            Run.ClearInput();
        }
        /// <summary>get either 'y' or 'n' and only returns when it receives one of them</summary>
        public static bool GetYNStrict(){
            char _yn;Run.ClearInput();
            Run.SetCursorSize(100);Run.SetCursorShow(true);
            do{_yn=Run.GetChar(true);}
            while(!(_yn=='y'||_yn=='n'));
            Run.SetCursorSize(25);Run.SetCursorShow(false);
            Run.SetColor(Run.Colors.yellow);
            Run.Text(""+_yn);
            Run.SetColor();
            return _yn=='y';
        }
        /// <summary>slowly "deletes" the characters on console</summary>
        /// <param name="num">number of characters to delete/overide with ' '</param>
        /// <param name="time">delay while writing</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void DeleteLast(int num=1,int time=200){
            String delete="";
            for(;num>0;num--){delete+="\b \b";}
            Run.Anim(delete,time,'\0',false);
        }
        /// <summary>clears screen width a bar</summary>
        /// <param name="width">width of the bar</param>
        /// <param name="dir">direction of bar</param>
        /// <param name="time">delay in ms</param>
        /// <param name="ch">char of bar</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected static void ClearBar(int width=3,Run.ScrollDirection dir=Run.ScrollDirection.down,int time=3,char ch='█'){
            switch(dir){
                case Run.ScrollDirection.down:
                    for(int i=1;i<=width;i++){Run.ClearRow(i,ch);Run.Wait(time);}
                    for(int i=1;i<=Run.GetInnerHeight();i++){if(i<Run.GetInnerHeight()-width){Run.ClearRow(i+width,ch);}Run.ClearRow(i,' ');Run.Wait(time);}
                    break;
                case Run.ScrollDirection.up:
                    for(int i=0;i<width;i++){Run.ClearRow(Run.GetInnerHeight()-i,ch);Run.Wait(time);}
                    for(int i=Run.GetInnerHeight();i>0;i--){if(i>width){Run.ClearRow(i-width,ch);}Run.ClearRow(i,' ');Run.Wait(time);}
                    break;
                case Run.ScrollDirection.right:
                    for(int i=1;i<=width;i++){Run.ClearCol(i,ch);Run.Wait(time);}
                    for(int i=1;i<=Run.GetInnerWidth();i++){if(i<Run.GetInnerWidth()-width){Run.ClearCol(i+width,ch);}Run.ClearCol(i,' ');Run.Wait(time);}
                    break;
                case Run.ScrollDirection.left:
                    for(int i=0;i<width;i++){Run.ClearCol(Run.GetInnerWidth()-i,ch);Run.Wait(time);}
                    for(int i=Run.GetInnerWidth();i>0;i--){if(i>width){Run.ClearCol(i-width,ch);}Run.ClearCol(i,' ');Run.Wait(time);}
                    break;
            }
        }
    }
}

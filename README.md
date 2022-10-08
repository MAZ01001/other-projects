
# ["other-projects"](https://github.com/MAZ01001/other-projects)

## other small projects that aren't on [my-GitHub-page](https://maz01001.github.io)

( other languages than javascript :o )

- [[snake_cmd-game.cpp]](#snake_cmd-gamecpp)
- [[useful.js]](#usefuljs)
- [[black-green.css]](#black-greencss)
- [[ffmpeg.md]](#ffmpegmd)
- [[better_video_controls.user.js]](#better_video_controlsuserjs)

----
>
> ## [snake_cmd-game.cpp](https://github.com/MAZ01001/other-projects/blob/main/snake_cmd-game.cpp)
>
>     +---------------+
>     |       +--->   |
>     |   +---+       |
>     |   |      [F]  |
>     +---------------+
>
> A Windows console Snake game which's written in C++
>
> - Compile (with MinGW) → `g++ .\snake_cmd-game.cpp -o .\run`
> - Start (in Windows-cmd) → `.\run.exe -t 200 -p`
>   - Extra flags:
>     - `-t 100` ← Sets the millisecond delay between each frame/calculation. Default is 200.
>     - `-p` ← Will enable "portal walls" which makes the snake reappear on the other side instead of game over.
>   - Other keys and what they do, like `[wasd] move` are on-screen underneath the game.
>   - The playable field is default 30*30 cells big. Wich is only changeable before compiling.
>
----
>
> ## [useful.js](https://github.com/MAZ01001/other-projects/blob/main/useful.cpp)
>
> some useful JavaScript functions
>
> - `_string`
>   - `_insert(str,i=0,r='',d=0)` insert string in string at index and delete some characters
>   - `_charStats(str,chars='')` analyses string of how much each character appears
> - _(moved `_number_*` to [`Math-Js/functions.js`](https://github.com/MAZ01001/Math-Js#functionsjs))_
>
----
>
> ## [black-green.css](https://github.com/MAZ01001/other-projects/blob/main/black-green.css)
>
> some style rules I find useful and nice-looking
>
----
>
> ## [ffmpeg.md](https://github.com/MAZ01001/other-projects/blob/main/ffmpeg.md)
>
> a more or less detailed list of some useful FFmpeg commands
>
----
>
> ## [better_video_controls.user.js](https://github.com/MAZ01001/other-projects/blob/main/better_video_controls.user.js)
>
> A [tampermonkey](https://www.tampermonkey.net/) userscript to control html video elements with the keyboard.
> Oriented on [YouTube](https://www.youtube.com/) keyboard shortcuts.
>
> Keeps track of the last video element that was moused over, to control it when a key is pressed.
> It shows a popup for 2 sec on what action was performed (the text is selectable and stays while the mouse is over it).
>
> __Note__: The default behavior of key presses won't be prevented.
> That means when there's already control for fast forwarding, it'll trigger double, skipping more than intended.
>
> The following table will also be logged to the console, including a function to toggle the event listeners on/off.
> <details open><summary>keyboard controls</summary>
>
>      Keyboard (intended for QWERTZ) | Function
>     --------------------------------+------------------------------------------------------------------------
>      [0] - [9]                      |  skip to []% of total duration (ie. key [8] skips to 80% of the video)
>      [.]                            |  (while paused) next frame (1/60 sec)
>      [,]                            |  (while paused) previous frame (1/60 sec)
>      [:] ( [shift] [.] )            |  decrease playback speed by 10%
>      [;] ( [shift] [,] )            |  increase playback speed by 10%
>     --------------------------------+------------------------------------------------------------------------
>      [j] / [ArrowLeft]              |  rewind 5 seconds
>      [l] / [ArrowRight]             |  fast forward 5 seconds
>      [J] ( [shift] [j] )            |  rewind 30 seconds
>      [l] ( [shift] [l] )            |  fast forward 30 seconds
>      [k]                            |  pause / play video
>     --------------------------------+------------------------------------------------------------------------
>      [+] / [ArrowUp]                |  increase volume by 10%
>      [-] / [ArrowDown]              |  lower volume by 10%
>      [m]                            |  mute / unmute video
>     --------------------------------+------------------------------------------------------------------------
>      [f]                            |  toggle fullscreen mode
>      [p]                            |  toggle picture-in-picture mode
>      [t]                            |  displays exact time and duration
>      [u]                            |  displays current source url
>
> </details>
>

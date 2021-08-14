
# "other-projects"

## other small projects that aren't on [my-GitHub-page](https://maz01001.github.io)

( other languages than javascript :o )

----
>
> ### snake_cmd-game.cpp
>
>     +---------------+
>     |       +--->   |
>     |   +---+       |
>     |   |      [F]  |
>     +---------------+
>
> A Windows-console Snake game written in C++
>
> + Compile (with MinGW) → `g++ .\snake_cmd-game.cpp -o .\run`
> + Start (in Windows-cmd) → `.\run.exe -t 200 -p`
>   + Extra flags:
>     + `-t 100` ← Sets the millisecond delay between each frame/calculation. Default is 200.
>     + `-p` ← Will enable "portal walls" which makes the snake reappear on the other side instead of game over.
>   + Other keys and what they do, like `[wasd] move` are on-screen underneath the game.
>   + The playable field is default 30*30 cells big. Wich is only changeable before compiling.
>
----
>
> ## useful.js
>
> some useful JavaScript functions
>
> + `_string`
>   + `_insert(str,i=0,r='',d=0)` insert string in string at index and delete some characters
>   + `_charStats(str,chars='')` analyses string of how much each character appears
> + `_number`
>   + `_mapRange(n,x,y,x2,y2,limit=false);` map number from one range to another
>   + `_roundDecimal(n,dec=0);` rounds number to decimal place
>   + `_toPercent(n,x,y);` calculates percent of number in range ("progress")
>   + `_deg2rad(deg);` DEG to RAD
>   + `_rad2deg(rad);` RAD to DEG
>   + `_gcd(A,B);` calculates greatest common divisor
>   + `_dec2frac(dec,loop_last=0,max_den=0,max_iter=1e6);` estimates a decimal number with a fraction
>   + `_padNum(n,first=0,last=0);` pad number in respect to the decimal point
>

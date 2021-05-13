
# "other-projects"

## other small projects that aren't on [my-GitHub-page](https://maz01001.github.io)

( other languages than javascript :o )

----
>
> ## snake-game
>
> A simple game where you control a snake on a 2d plane.
>
> Your goal is to collect fruits and avoid colliding with yourself or the walls.
>
> With each fruit you eat, you gain length, and the game will speed up.
>
> >
> > ### snake_cmd-game.cpp
> >
> >     +---------------+
> >     |       +--->   |
> >     |   +---+       |
> >     |   |      [F]  |
> >     +---------------+
> >
> > A Windows-console Snake game written in C++
> >
> > + Compile (with MinGW) → `g++ .\snake_cmd-game.cpp -o .\run`
> > + Start (in Windows-cmd) → `.\run.exe -t 200 -p`
> >   + Extra flags:
> >     + `-t 100` ← Sets the millisecond delay between each frame/calculation. Default is 200.
> >     + `-p` ← Will enable "portal walls" wich makes the snake reappear on the other side instead of game over.
> >   + Other keys and what they do, like `[wasd] move` are on-screen underneath the game.
> >   + The playable field is default 30*30 cells big. Wich is only changable before compiling.
> >
>

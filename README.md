# Random projects

>
> other "small" projects that aren't on [my-GitHub-page](https://maz01001.github.io)
> nor strongly related to [math](https://github.com/MAZ01001/Math-Js)
>
> other languages than javascript :o
>

- [snake_cmd-game.cpp](#snake_cmd-gamecpp)
- [useful.js](#usefuljs)
- [black-green.css](#black-greencss)
- [ffmpeg.md](#ffmpegmd)
- [ConsoleIO.cs](#consoleiocs)

----

## [snake_cmd-game.cpp](./snake_cmd-game.cpp)

    +---------------+
    |       +--->   |
    |   +---+       |
    |   |      [F]  |
    +---------------+

A Windows console Snake game which's written in C++

- Compile (with MinGW) → `g++ .\snake_cmd-game.cpp -o .\run`
- Start (in Windows-cmd) → `.\run.exe -t 200 -p`
  - Extra flags:
    - `-t 100` ← Sets the millisecond delay between each frame/calculation. Default is 200.
    - `-p` ← Will enable "portal walls" which makes the snake reappear on the other side instead of game over.
  - Other keys and what they do, like `[wasd] move` are on-screen underneath the game.
  - The playable field is default 30*30 cells big. Wich is only changeable before compiling.

## [useful.js](./useful.js)

some useful JavaScript functions

### 1. String

#### __strInsert__

- `function strInsert(str: string, i?: number, r?: string, d?: number): string`
- insert string in string at index and delete some characters

#### __strCharStats__

- `function strCharStats(str: string, chars?: string): Readonly<{ [string]: number; other: number; }>`
- analyses string of how much each character appears

### 2. Date

#### __formatDate__

- `function formatDate(dt: Date | null, utc: boolean | null, separators: string | string[] | null): string`
- format date with custom separators

#### __getUTC0__

- `function getUTC0(highResMonotonicClock: boolean): BigInt`
- get the current timestamp UTC from year 0

### 2. Number

see [`Math-Js/functions.js`](https://github.com/MAZ01001/Math-Js#functionsjs)

### 3. Array

#### __hasArrayHoles__

- `function hasArrayHoles(arr: any[]): boolean`
- checks the array for empty entries / holes

### 4. HTML / DOM

#### __getTextDimensions__

- `function getTextDimensions(text: string, element?: Element, pseudoElt?: string): Readonly<{ width: number; height: number; lineHeight: number; }>`
- measures the dimensions of a given text in pixels (sub-pixel accurate)
- uses an element to account for text styling

#### __copyToClipboard__

- `function copyToClipboard(data: string | Blob): Promise<any>`
- copies the given text or rich-content to clipboard

#### __getMousePos__

- `function getMousePos(offsetElement?: Element | null): Readonly<{ pageX: number; pageY: number; clientX: number; clientY: number; offsetX: number; offsetY: number; screenX: number; screenY: number; movementX: number; movementY: number; }>`
- gets the current cursor position and also relative to the previous position, screen space, the browser window, the HTML page, and a given (HTML) element

## [black-green.css](./black-green.css)

some style rules I find useful and nice-looking

## [ffmpeg.md](./ffmpeg.md)

a more or less detailed list of some useful FFmpeg commands

## [ConsoleIO.cs](./ConsoleIO.cs)

A text-based game engine for (windows) console.

It was made for a school project to learn C# with a console game, but as you can see, I went overboard with this.

Features include (but are not limited to)

- changing text color
- moving the cursor and toggle visibility (also save cursor positions to load later)
- writing text horizontally or vertically (auto wraps to window) with optional delay for each character (looks animated)
- writing text can include `'\n'`, `'\r'`, `'\t'` and `'\b'` (also `'\0'` which just delays)
- creating a border with custom characters for all sides and corners
- scrolling the text on the screen (screen buffer), can also be "animated"
- clearing the screen with an animation
- make a noise with given frequency and duration (allways full volume !)
- change window size and toggle fullscreen
- get user imput (type-checked not sanitized)
- pauses the program for given milliseconds
- "press any key"
- custom number formatting

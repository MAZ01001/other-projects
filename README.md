# Random projects

Other _small_ projects that don't have their own repo (yet?)
or aren't strongly related to [Math-JS](https://github.com/MAZ01001/Math-Js "My Math-js repo")

also...other languages than javascript ?! :o

- [snake_cmd-game.cpp](#snake_cmd-gamecpp)
- [useful.js](#usefuljs)
- [black-green.css](#black-greencss)
- [ConsoleIO.cs](#consoleiocs)

_Moved `ffmpeg.md` to <https://github.com/MAZ01001/FFmpeg-resource>._

----

## [snake_cmd-game.cpp](./snake_cmd-game.cpp)

```text
+---------------+
|       +--->   |
|   +---+       |
|   |      [F]  |
+---------------+
```

A Windows console Snake game which's written in C++

- Compile (with MinGW) → `g++ .\snake_cmd-game.cpp -o .\run`
- Start (in Windows-cmd) → `.\run.exe -t 200 -p`
  - Extra flags:
    - `-t 100` ← Sets the millisecond delay between each frame/calculation. Default is 200.
    - `-p` ← Will enable "portal walls" which makes the snake reappear on the other side instead of game over.
  - Other keys and what they do, like `[wasd] move` are on-screen underneath the game.
  - The playable field is default 30*30 cells big. Wich is only changeable before compiling.

Scroll [TOP](#random-projects)

## [useful.js](./useful.js)

some useful JavaScript functions

also see [`Math-Js/functions.js`](https://github.com/MAZ01001/Math-Js#functionsjs)

```typescript
// String

/** insert string in string at index and delete some characters */
function strInsert(str: string, i?: number, r?: string, d?: number): string

/** analyses string of how much each character appears */
function strCharStats(str: string, chars?: string): Readonly<{
  [string]: number;
  other: number;
}>

/**
 * Create ANSI codes to set terminal color
 * for browser dev-console use `console.log("%cCSS", "background-color: #000; color: #F90");` instead
 */
function ansi(c?: number | [number, number, number] | null | undefined, bg?: number | undefined): string

// Date

/** format date with custom separators */
function formatDate(dt: Date | null, utc: boolean | null, separators: string | string[] | null): string

/** number of milliseconds from `0000-01-01 00:00` to `1970-01-01 00:00` (UTC) */
const UTC_OFFSET = 62125920000000;

// Array

/** checks the array for empty entries / holes */
const hasArrayHoles: (arr: any[]) => boolean

/** binary search in ascending sorted array (with dynamic typed elements) for index (or next smaller index) */
function binarySearch(arr: any[], e: any): number

// HTML / DOM

/**
 * measures the dimensions of a given text in pixels (sub-pixel accurate)
 * uses an element to account for text styling
 */
function getTextDimensions(text: string, element?: Element, pseudoElt?: string): Readonly<{
  width: number;
  height: number;
  lineHeight: number;
}>

/** copies the given text or rich-content to clipboard */
function copyToClipboard(data: string | Blob): Promise<any>

/** gets the current cursor position and also relative to the previous position, screen space, the browser window, the HTML page, and a given (HTML) element */
function getMousePos(offsetElement?: Element | null): Readonly<{
  pageX: number;     pageY: number;
  clientX: number;   clientY: number;
  offsetX: number;   offsetY: number;
  screenX: number;   screenY: number;
  movementX: number; movementY: number;
}>

/** pads element when it's overflowing (pad left/top the same as the width/height of `-webkit-scrollbar` if element is overflowing (per axis)) */
function PadOverflowFor(el: HTMLElement): void

/** shows gradients at the edges of a given element when it overflows to visualize that it's scrollable */
function StyleOverflowFor(el: HTMLElement, offset: number | [number, number], size: string | [string, string], color: string, alphaMax: number, background?: string | undefined): () => void

/** convert image to base64 data URL - for offline viewing (asynchronous) */
function LoadIMG(src: string): Promise<string | null>
```

Scroll [UP](#usefuljs) | [TOP](#random-projects)

## [black-green.css](./black-green.css)

some style rules I find useful and nice-looking

Scroll [TOP](#random-projects)

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

Scroll [TOP](#random-projects)

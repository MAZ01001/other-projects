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

_locate function in `useful.js` for more documentation via JSDoc, like parameter/return description_

<dl>
<dt>String</dt>
<dd>
<details><summary><code>strRem</code></summary>

__remove part of a string at a specific index and optionally inserts another string__

string equivalent of `Array.splice`

```typescript
function strSplice(
    txt: string,
    i: number,
    rem: number,
    add?: string | undefined
): string
```

```javascript
strSplice("Hello#World!",  5,  1);      //=> "HelloWorld!"
strSplice("Hello#World!", -7,  1, ", ");//=> "Hello, World!"
strSplice("Hello#World!",  6, -1, ", ");//=> "Hello#, #World!"
```

</details>
<details><summary><code>strCharStats</code></summary>

__object of how much each character appears in the string__

or for only the given characters

```typescript
function strCharStats(
    str: string,
    locale?: Intl.LocalesArgument | null,
    chars?: string
): Map<string, number> & Map<"other", number>
```

```javascript
strCharStats("abzaacdd");              //~ Map{"other" => 0, "a" => 3, "b" => 1, "z" => 1, "c" => 1, "d" => 2}
strCharStats("abzaacdd", null, "abce");//~ Map{"other" => 3, "a" => 3, "b" => 1, "c" => 1, "e" => 0}
```

</details>
<details><summary><code>ansi</code></summary>

__Create ANSI codes to set terminal color__

sets output terminal fore/background colors \
! remember to output the reset code before end of script or terminal colors stay this way \
for browser dev-console use `console.log("%cCSS", "background-color: #000; color: #f90");` instead \
! keep in mind that if the terminal doesn't support ansi-codes it will output them as plain text

```typescript
function ansi(
    c?: number | [number, number, number] | null | undefined,
    bg?: number | undefined
): string
```

```javascript
console.log("TEST%sTEST%sTEST",ansi(0xff9900),ansi());
// <=> console.log("TEST%cTEST%cTEST","color:#f90","");
```

</details>
<details><summary><code>strCompare</code></summary>

__get [Damerau-Levenshtein distance](https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance "Wikipedia: Damerau-Levenshtein distance") of two strings__

```typescript
function strCompare(
    a: string,
    b: string,
    locale?: Intl.LocalesArgument | null
): number
```

```javascript
strCompare("ca","abc");//=> 2 (flip 'ca' and add 'b')
```

</details>
<details><summary><code>strCompareLite</code></summary>

__get [Levenshtein distance](https://en.wikipedia.org/wiki/Levenshtein_distance "Wikipedia: Levenshtein distance") of two strings__

is always greater or equal to `strCompare` (Damerau-Levenshtein distance), but a bit faster for longer strings

```typescript
function strCompareLite(
    a: string,
    b: string,
    locale?: Intl.LocalesArgument | null
): number
```

```javascript
strCompareLite("ca","abc");//=> 3 (delete 'c', add 'b', and add 'c')
```

<details><summary>performance difference</summary>

> node.js `v22.12.0` on intel `i7-10700K`

```text
┌─────────┬────┬────┬────────┬────────┬──────────┬────────────────────────────────────────────────────┐
│ (index) │ A  │ B  │ Atime  │ Btime  │ improved │ parameters                                         │
├─────────┼────┼────┼────────┼────────┼──────────┼────────────────────────────────────────────────────┤
│ 0       │ 3  │ 3  │ 0.0465 │ 0.0326 │ '29.89%' │ [ 'sitting', 'kitten' ]                            │
│ 1       │ 3  │ 3  │ 0.0359 │ 0.0325 │ '9.69%'  │ [ 'sunday', 'saturday' ]                           │
│ 2       │ 1  │ 2  │ 0.0261 │ 0.0244 │ '6.61%'  │ [ 'test', 'tset' ]                                 │
│ 3       │ 2  │ 3  │ 0.0211 │ 0.0219 │ '-3.47%' │ [ 'ca', 'abc' ]                                    │
│ 4       │ 0  │ 0  │ 0.0075 │ 0.0075 │ '-0.42%' │ [ 'abc', 'abc' ]                                   │
│ 5       │ 2  │ 2  │ 0.1003 │ 0.0876 │ '12.65%' │ [ 'some more text to...', 'some more text to...' ] │
│ 6       │ 51 │ 52 │ 0.1933 │ 0.1228 │ '36.47%' │ [ 'Lorem ipsum dolor...', 'completely differ...' ] │
│ 7       │ 10 │ 11 │ 0.237  │ 0.1433 │ '39.55%' │ [ 'too sennteces tha...', 'two sentences tha...' ] │
│ 8       │ 4  │ 6  │ 0.162  │ 0.1041 │ '35.75%' │ [ 'two sentneces tha...', 'two sentences tha...' ] │
│ 9       │ 1  │ 1  │ 0.2382 │ 0.1449 │ '39.17%' │ [ 'one difference. L...', 'one difference. L...' ] │
│ 10      │ 1  │ 2  │ 0.2427 │ 0.1501 │ '38.16%' │ [ 'one difference. L...', 'one difference. L...' ] │
└─────────┴────┴────┴────────┴────────┴──────────┴────────────────────────────────────────────────────┘
```

```javascript
strCompare("a","b");strCompareLite("a","b");//~ warmup
strCompare("b","c");strCompareLite("b","d");//~ warmup
strCompare("c","d");strCompareLite("c","d");//~ warmup
/**@type {[string,string][]}*/const V=[
    ["sitting","kitten"],
    ["sunday","saturday"],
    ["test","tset"],
    ["ca","abc"],
    ["abc","abc"],
    ["some more text too compare to eachother","some more text to compare to each other"],
    ["Lorem ipsum dolor sit amet, consectetuer adipiscing elit.","completely different text that may have some characters in common"],
    ["too sennteces that're long but not all tht muc diffreent to eachothe","two sentences that are long but not all that much different to each other"],
    ["two sentneces that are long but allmots thet exact same","two sentences that are long but all most the exact same"],
    ["one difference. Lorem ipsum dolor sit amet, consectetuer adipiscing elit.","one difference. Lorem ipsum door sit amet, consectetuer adipiscing elit."],
    ["one difference. Lorem ipsum dolor sit amet, consectetuer adipiscing elit.","one difference. Lorem ipsum doolr sit amet, consectetuer adipiscing elit."],
];
console.table(V.map((v,i)=>{
    let A,B,a=0,b=0,t;
    for(let i=0;i<100;++i){t=performance.now();A=strCompare(v[0],v[1],"en");a+=(performance.now()-t)**-1;}
    for(let i=0;i<100;++i){t=performance.now();B=strCompareLite(v[0],v[1],"en");b+=(performance.now()-t)**-1;}
    return{
        A,B,
        Atime:Number((a=100/a).toFixed(4)),
        Btime:Number((b=100/b).toFixed(4)),
        improved:`${Number((100-b*100/a).toFixed(2))}%`,
        parameters:V[i].map(w=>w.length>20?w.substring(0,17)+"...":w)
    };
}));
```

</details>
</details>
</dd>
<dt>Number</dt>
<dd>

see <https://github.com/MAZ01001/Math-Js#functionsjs>

</dd>
<dt>Color</dt>
<dd>
<details><summary><code>HSVtoRGB</code></summary>

__convert HSV color to RGB__

! notice that `H` input is in range `[0,6]` so to convert from `[0,360]` (degrees) divide by `60`; or multiply with `6` if coming from `[0,1]` (like `S`/`V` input)

```typescript
function HSVtoRGB(
    H: number,
    S: number,
    V: number
): [number, number, number]
```

</details>
<details><summary><code>RGBtoHSV</code></summary>

__convert RGB color to HSV__

! notice that hue output is in range `[0,6]` so multiply with `60` to get `[0,360]` (degrees); or divide by `6` for `[0,1]` (like saturation/value output)

```typescript
function RGBtoHSV(
    R: number,
    G: number,
    B: number
): [number, number, number]
```

</details>
<details><summary><code>colorHexRound</code></summary>

__round hex color from 6/8 digits to 3/4 digits__

rounded componentwise to nearest hex-double like `F5` → `E` = `EE`

```typescript
function colorHexRound(
    color: string
): string
```

</details>
<details><summary><code>RGBtoCMYK</code></summary>

__convert RGB to CMY(K)__

```typescript
function RGBtoCMYK(
    R: number,
    G: number,
    B: number,
    excludeK?: boolean | undefined
): [number, number, number, number]
```

</details>
<details><summary><code>CMYKtoRGB</code></summary>

__convert CMY(K) to RGB__

```typescript
function CMYKtoRGB(
    C: number,
    M: number,
    Y: number,
    K?: number | undefined
): [number, number, number]
```

</details>
</dd>
<dt>Array</dt>
<dd>
<details><summary><code>hasArrayHoles</code></summary>

__checks if the given array has empty slots__

most iterator functions skip empty entries, like `Array.every` and `Array.some`, so they might bypass checks and lead to undefined behavior \
their value is `undefined` but they're treated differently from an actual `undefined` in the array \
but the length attribute does include them since they do contribute to the total length of the array

```typescript
const hasArrayHoles: (
    arr: any[]
) => boolean
```

```javascript
hasArrayHoles(["",0,undefined,,,,null,()=>{},[],{}]); //=> true
hasArrayHoles(["",0,undefined,null,()=>{},[],{}]);    //=> false
```

</details>
<details><summary><code>binarySearch</code></summary>

__Binary search in `arr` (ascending sorted array) for `e`__

using `!=` and `<` (supports dynamic types), `arr` can have duplicate elements

```typescript
function binarySearch(
    arr: any[],
    e: any
): number
```

</details>
</dd>
<dt>HTML / DOM</dt>
<dd>
<details><summary><code>getTextDimensions</code></summary>

__measures the dimensions of a given `text` in pixels (sub-pixel accurate)__

[!] only works in the context of HTML ie. a browser [!]

for using an elements font use `CSSStyleDeclaration.font` of `window.getComputedStyle` ie `window.getComputedStyle(element, pseudoElementOrNull).font` \
if using `"initial"`, `"revert"`, or any similar or invalid value as font, it seems to use `"10px sans-serif"` (default of `OffscreenCanvasRenderingContext2D.font`)

```typescript
function getTextDimensions(
    text: string,
    fontCSS?: string | undefined
): number
```

</details>
<details><summary><code>getMousePos</code></summary>

__gets current mouse position (optionally relative to an element)__

[!] only works in the context of HTML ie. a browser [!]

__WARNING__: \
Browsers may use different units for movementX and screenX than what the specification defines. \
The movementX units can be physical, logical, or CSS pixels, depending on the browser and operating system. \
_See [this issue on GitHub](https://github.com/w3c/pointerlock/issues/42) for more information on the topic._

```typescript
function getMousePos(
    offsetElement?: Element | null | undefined
): [
    Readonly<{
        page: [number, number];
        client: [number, number];
        offset: [number, number];
        screen: [number, number];
        movement: [number, number];
    }>,
    AbortController
]
```

```javascript
const [mousePos, mouseSignal] = getMousePos();
const log = setInterval(() => console.log(JSON.stringify(mousePos)), 1000);
// mouseSignal.abort();
// clearInterval(log);
```

</details>
<details><summary><code>styleOverflowFor</code></summary>

__Shows gradients at the edges of `el` when it overflows and becommes scrollable__

[!] only works in the context of HTML ie. a browser [!]

Overrides the CSS `background` property (use `background` to add any additional value/s for CSS)

```typescript
function styleOverflowFor(
    el: HTMLElement,
    offset: number | [number, number],
    size: string | [string, string],
    color: string,
    alphaMax: number,
    background?: string | null | undefined
): () => void
```

```javascript
const box = document.getElementById("box");
const boxOverflowUpdate = StyleOverflowFor(
    box,
    0xC8,
    [
        "clamp(1rem, 5vw, 2rem)",
        "clamp(1rem, 5vh, 2rem)"
    ],
    "#CCCC00$x",
    2/3,
    "#0008"
);
boxOverflowUpdate();
box.addEventListener("scroll", boxOverflowUpdate, {passive: true});
window.addEventListener("resize", boxOverflowUpdate, {passive: true});
```

</details>
</dd>
<dt>Other</dt>
<dd>
<details><summary><code>LoadIMG</code></summary>

__fetch image from URL and convert it to (base64) data-URL__

```typescript
LoadIMG.IMG():             (src: string) => Promise<string | null>
LoadIMG.FetchFileReader(): (src: string) => Promise<string | null>
LoadIMG.FetchManual():     (src: string) => Promise<string | null>
```

```javascript
// FetchManual and FetchFileReader can also "fetch" other data-URLs and convert them to base64
LoadIMG.FetchManual()("data:text/plain;,Hello, World!").then(console.log);
//=> data:text/plain;base64,SGVsbG8sIFdvcmxkIQ==
//=> Hello, World!

// sadly, Unicode is not supported
LoadIMG.FetchManual()("data:text/plain;,test ⣿ unicode ↔ string").then(console.log);
//=> data:text/plain;base64,dGVzdCDio78gdW5pY29kZSDihpQgc3RyaW5n
//=> test â£¿ unicode â string

// but for images this doesn't matter
LoadIMG.FetchManual()("https://picsum.photos/id/40/50.webp").then(console.log);
// ↓
// data:image/webp;base64,UklGRqIDAABXRUJQVlA4WAoAAAAIAAAAMQAAMQAAVlA4IKACAABwDgCdASoyAD
// IAPo06lUelIyIhLguIoBGJZQDGOGnpTc74alUQ7JcfLH3CAqEkT5HdCKzmc/LsNcRvnkXL9d9PGgIIdLCND7P
// XIPXnISPidGgN9vEI84604t47K0wn+Kt2DUHYgLCbQYDR16nTdE8pFKRerZlcgWG19yAAAP7Zux4ZwtVnCo5D
// w6Q/Ds87nvXL02B6iSiaT/bRfyuGyiCOCnDImLAv6BcfEOKth2Eipd658UZ9NgvpReW2voVZOkpm2iRdNh3HH
// lI0Z6MWDOMszRbznpCPWHitNcAQLHYmFyWlu7/psWWar76ChuR3R8N9CR7kTeva2v+//Vk4UCReeaqI5I8TnD
// eCcr4KLyKrJ52utlg/OlIkWDn1NZWAq+RJTr20EPvB/KiyMJJTkrso2DJPTthuFWkEnBeVRAoQxc/PaHTgzT3
// 4LX4orXUPaCFRyAud+aTYJmENUj/qGijXe7qTrBy5zSbowno9GEPNrPG+mi7vLhyszCjC/ajLGGmtwK/ohCw3
// /yVZQ5H3ms+C8d8P8P3y5EYHyPunL4UVJbC8QwHthDU1FrrE4JzZGuniHBuRDmEqmCx5BJU9ifFIQnHHd5B5T
// vn4CKgHETsxpafDqiAwsw6HdFkZdzInd9C02StE6v70dENPlx1/AidSEoZFeWKwsXQNkpbe+0ej5HZxj2XB5q
// utls4eMY4YuTT26/zwWxcVwMZ5Rr7GPmC3h/B1XRb9i1Sa2Zlb5eVCj3sccFKPHJouE/gs+sLsrXKl4oNKkX4
// kNkUoLJRV1TLIoguOyhgh7iHLQi23FA0EiF4rwfB0ABYG1U5P0BAAiqSJHoxMsIdcspL7vteYRvvml/tujDFk
// r8TJBGA3acbJhJcJ9IGwrMBItrMdw8/MAABFWElG3AAAAEV4aWYAAElJKgAIAAAABgASAQMAAQAAAAEAAAAaA
// QUAAQAAAFYAAAAbAQUAAQAAAF4AAAAoAQMAAQAAAAIAAAATAgMAAQAAAAEAAABphwQAAQAAAGYAAAAAAAAASA
// AAAAEAAABIAAAAAQAAAAcAAJAHAAQAAAAwMjEwAZEHAAQAAAABAgMAhpIHABUAAADAAAAAAKAHAAQAAAAwMTA
// wAaADAAEAAAD//wAAAqAEAAEAAAAyAAAAA6AEAAEAAAAyAAAAAAAAAEFTQ0lJAAAAUGljc3VtIElEOiA0MAA=
// ↓
```

<img src="data:image/webp;base64,UklGRqIDAABXRUJQVlA4WAoAAAAIAAAAMQAAMQAAVlA4IKACAABwDgCdASoyADIAPo06lUelIyIhLguIoBGJZQDGOGnpTc74alUQ7JcfLH3CAqEkT5HdCKzmc/LsNcRvnkXL9d9PGgIIdLCND7PXIPXnISPidGgN9vEI84604t47K0wn+Kt2DUHYgLCbQYDR16nTdE8pFKRerZlcgWG19yAAAP7Zux4ZwtVnCo5Dw6Q/Ds87nvXL02B6iSiaT/bRfyuGyiCOCnDImLAv6BcfEOKth2Eipd658UZ9NgvpReW2voVZOkpm2iRdNh3HHlI0Z6MWDOMszRbznpCPWHitNcAQLHYmFyWlu7/psWWar76ChuR3R8N9CR7kTeva2v+//Vk4UCReeaqI5I8TnDeCcr4KLyKrJ52utlg/OlIkWDn1NZWAq+RJTr20EPvB/KiyMJJTkrso2DJPTthuFWkEnBeVRAoQxc/PaHTgzT34LX4orXUPaCFRyAud+aTYJmENUj/qGijXe7qTrBy5zSbowno9GEPNrPG+mi7vLhyszCjC/ajLGGmtwK/ohCw3/yVZQ5H3ms+C8d8P8P3y5EYHyPunL4UVJbC8QwHthDU1FrrE4JzZGuniHBuRDmEqmCx5BJU9ifFIQnHHd5B5Tvn4CKgHETsxpafDqiAwsw6HdFkZdzInd9C02StE6v70dENPlx1/AidSEoZFeWKwsXQNkpbe+0ej5HZxj2XB5qutls4eMY4YuTT26/zwWxcVwMZ5Rr7GPmC3h/B1XRb9i1Sa2Zlb5eVCj3sccFKPHJouE/gs+sLsrXKl4oNKkX4kNkUoLJRV1TLIoguOyhgh7iHLQi23FA0EiF4rwfB0ABYG1U5P0BAAiqSJHoxMsIdcspL7vteYRvvml/tujDFkr8TJBGA3acbJhJcJ9IGwrMBItrMdw8/MAABFWElG3AAAAEV4aWYAAElJKgAIAAAABgASAQMAAQAAAAEAAAAaAQUAAQAAAFYAAAAbAQUAAQAAAF4AAAAoAQMAAQAAAAIAAAATAgMAAQAAAAEAAABphwQAAQAAAGYAAAAAAAAASAAAAAEAAABIAAAAAQAAAAcAAJAHAAQAAAAwMjEwAZEHAAQAAAABAgMAhpIHABUAAADAAAAAAKAHAAQAAAAwMTAwAaADAAEAAAD//wAAAqAEAAEAAAAyAAAAA6AEAAEAAAAyAAAAAAAAAEFTQ0lJAAAAUGljc3VtIElEOiA0MAA=" alt="example image" title="Image by Ryan Mcguire (unsplash.com)">

</details>
</dd>
</dl>

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

//@ts-check
"use strict";

//MARK: string

/**
 * ## remove part of a string at a specific index and optionally inserts another string
 * string equivalent of {@linkcode Array.splice}
 * @param {string} txt - initial string
 * @param {number} i - (zero-based) index of {@linkcode txt} (from the end if negative)
 * @param {number} rem - delete count of characters in {@linkcode txt} at {@linkcode i} (duplicates characters if negative)
 * @param {string} [add] - [optional] replacement string to be inserted in {@linkcode txt} at {@linkcode i} (after removing) - default `""` (none)
 * @returns {string} (new) modified string
 * @throws {TypeError} if {@linkcode txt} or {@linkcode add} (if given) are not strings
 * @throws {TypeError} if {@linkcode i} or {@linkcode rem} are not safe integers (`]-2^53..2^53[`)
 * @example
 * strSplice("Hello#World!",  5,  1);      //=> "HelloWorld!"
 * strSplice("Hello#World!", -7,  1, ", ");//=> "Hello, World!"
 * strSplice("Hello#World!",  6, -1, ", ");//=> "Hello#, #World!"
 */
function strSplice(txt,i,rem,add){
    "use strict";
    if(typeof txt!=="string")throw new TypeError("[strSplice] txt is not a string");
    if(!Number.isSafeInteger(i))throw new TypeError("[strSplice] i is not a safe integer");
    if(!Number.isSafeInteger(rem))throw new TypeError("[strSplice] rem is not a safe integer");
    if(add!=null&&typeof add!=="string")throw new TypeError("[strSplice] add (given) is not a string");
    if(i<0)i=txt.length+i;
    return txt.substring(0,i)+(add??"")+txt.substring(i+rem);
}
/**
 * ## object of how much each character appears in the string
 * or for only the given characters
 * @param {string} str - the string for analysis
 * @param {Intl.LocalesArgument|null} [locale] - [optional] locale for deciding what a character is (language/unicode support) - default `null` (current system locale)
 * @param {string} [chars] - [optional] searches only the amount for these characters - default `""` (all)
 * @returns {Map<string,number>&Map<"other",number>} map with amount of apperance (in order of appearance in {@linkcode chars} or (if not given) {@linkcode str})
 * @example
 * strCharStats("abzaacdd");              //=> Map{"other" => 0, "a" => 3, "b" => 1, "z" => 1, "c" => 1, "d" => 2}
 * strCharStats("abzaacdd", null, "abce");//=> Map{"other" => 3, "a" => 3, "b" => 1, "c" => 1, "e" => 0}
 */
function strCharStats(str,locale,chars){
    "use strict";
    if(typeof str!=="string")throw new TypeError("[strCharStats] str is not a string");
    if(chars!=null&&typeof chars!=="string")throw new TypeError("[strCharStats] chars (given) is not a string");
    /**@type {Map<string,number>&Map<"other",number>}*/
    const obj=new Map();
    obj.set("other",0);
    const seg=new Intl.Segmenter(locale??undefined);
    if(chars==null||chars==='')
        for(const{segment:char}of seg.segment(str))obj.set(char,(obj.get(char)??0)+1);
    else{
        for(const{segment:char}of seg.segment(chars))obj.set(char,0);
        for(const{segment:char}of seg.segment(str))
            //@ts-ignore `char` does exist in `obj` within if block
            if(obj.has(char))obj.set(char,obj.get(char)+1);
            //@ts-ignore "other" does exist in `obj`
            else obj.set("other",obj.get("other")+1);
    }
    return obj;
}
/**
 * ## Create ANSI codes to set terminal color
 * sets output terminal fore/background colors \
 * ! remember to output the reset code before end of script or terminal colors stay this way \
 * for browser dev-console use `console.log("%cCSS", "background-color: #000; color: #f90");` instead \
 * ! keep in mind that if the terminal doesn't support ansi-codes it will output them as plain text
 * @param {null|number|[number,number,number]} [c] - set color by type (default `null`):
 * | type        | return                                        |
 * |:----------- |:--------------------------------------------- |
 * | `null`      | ANSI reset code                               |
 * | `number`    | color as 3 * 8bit RGB ~ 0x112233              |
 * | `number[3]` | color as `[R, G, B]` (truncated to 8bit each) |
 * @param {number} [bg] - `<0` for background color, `>0` for foreground color, or `0` for both - default: foreground color
 * @returns {string} ANSI code for re/setting fore/background color of output terminal
 * @throws {TypeError} when {@linkcode c} is not one of the documented types or {@linkcode bg} is given and not a number
 * @example console.log("TEST%sTEST%sTEST",ansi(0xff9900),ansi());// <=> console.log("TEST%cTEST%cTEST","color:#f90","");
 */
function ansi(c,bg){
    "use strict";
    if(c==null)return"\x1b[0m";
    let color="";
    if(typeof c==="number")color=`${(c&0xff0000)>>>16};${(c&0xff00)>>>8};${c&255}`;
    else if(Array.isArray(c)&&c.length===3&&c.every(v=>typeof v==="number"))color=`${c[0]&255};${c[1]&255};${c[2]&255}`;
    else throw new TypeError("[ansi] c is given but not a number nor an array of 3 numbers");
    if(bg==null)return`\x1b[38;2;${color}m`;
    if(typeof bg==="number")
        switch(Math.sign(bg)){
            case 1:return`\x1b[38;2;${color}m`;
            case 0:return`\x1b[38;2;${color};48;2;${color}m`;
            case-1:return`\x1b[48;2;${color}m`;
        }
    throw new TypeError("[ansi] bg is given but not a number");
}
/**
 * ## get Damerau-Levenshtein distance of two strings
 * @param {string} a - string A
 * @param {string} b - string B
 * @param {Intl.LocalesArgument|null} [locale] - [optional] locale for splitting {@linkcode a} and {@linkcode b} into grapheme clusters (characters/symbols/emoji) - default `null` = current system locale
 * @returns {number} Damerau-Levenshtein distance of {@linkcode a} and {@linkcode b} (number of needed substitution, insertion, deletion, transposition)
 * @throws {TypeError} if {@linkcode a} or {@linkcode b} are not strings
 * @example strCompare("ca","abc");//=> 2 (flip 'ca' and add 'b')
 * @link https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
 */
function strCompare(a,b,locale){
    "use strict";
    if(typeof a!=="string")throw new TypeError("[strCompare] a is not a string");
    if(typeof b!=="string")throw new TypeError("[strCompare] b is not a string");
    /**@type {Intl.Segmenter}*/let segmenter;
    try{segmenter=new Intl.Segmenter(locale??undefined,{granularity:"grapheme"});}
    catch(err){
        if(err instanceof TypeError||err instanceof RangeError)throw new TypeError(`[strCompare] locale is invalid: ${err.message}`);
        throw err;
    }
    if(a===b)return 0;
    let A=Object.freeze(Array.from(segmenter.segment(a),v=>v.segment));
    let B=Object.freeze(Array.from(segmenter.segment(b),v=>v.segment));
    if(A.length>B.length)[A,B]=[B,A];//~ |B| >= |A|
    /**@type {number[][]} `[A.length][B.length]` empty fields are set before use*/
    const d=Array.from({length:A.length},()=>new Array(B.length));
    /**@type {Map<string,number>} use `0` if not found*/
    const da=new Map();
    const maxDist=A.length+B.length;
    /**@type {(i:number,j:number)=>number} read {@linkcode d}`[i][j]`*/
    const dR=(i,j)=>{
        "use strict";
        if(i>1&&j>1)return d[i-2][j-2];
        if(i===0||j===0)return maxDist;
        return(j===1?i:j)-1;
    };
    for(let i=0,Ai=A[0],j,Bj,db,k,l,cost;i<A.length;Ai=A[++i]){
        db=0;
        for(j=0,Bj=B[0];j<B.length;Bj=B[++j]){
            k=da.get(Bj)??0;
            l=db;
            if(Ai===Bj){
                cost=0;
                db=j+1;
            }else cost=1;
            d[i][j]=Math.min(//~ substitution, insertion, deletion, transposition
                dR(i+1,j+1)+cost,
                dR(i+2,j+1)+1,
                dR(i+1,j+2)+1,
                dR(k,l)+(i-k)+1+(j-l)
            );
        }
        da.set(Ai,i+1);
    }
    // const table={},keys=[" ↓ "];
    // for(let i=0;i<=A.length;++i){
    //     table[i===0?" → ":" "+i]={};
    //     for(let j=0;j<=B.length;++j)
    //         if(i===0&&j===0)table[" → "][" ↓ "]=maxDist;
    //         else if(j===0)table[" "+i][" ↓ "]=A[i-1];
    //         else if(i===0)table[" → "][" "+j]=B[j-1],keys.push(" "+j);
    //         else table[" "+i][" "+j]=d[i-1][j-1];
    // }
    // console.table(table,keys);
    return d[A.length-1][B.length-1];
}
/**
 * ## get Levenshtein distance of two strings
 * is always greater or equal to {@linkcode strCompare} (Damerau-Levenshtein distance), but a bit faster for longer strings
 * @param {string} a - string A
 * @param {string} b - string B
 * @param {Intl.LocalesArgument|null} [locale] - [optional] locale for splitting {@linkcode a} and {@linkcode b} into grapheme clusters (characters/symbols/emoji) - default `null` = current system locale
 * @returns {number} Levenshtein distance of {@linkcode a} and {@linkcode b} (number of needed substitution, insertion, deletion)
 * @throws {TypeError} if {@linkcode a} or {@linkcode b} are not strings
 * @example strCompareLite("ca","abc");//=> 3 (delete 'c', add 'b', and add 'c')
 * @link https://en.wikipedia.org/wiki/Levenshtein_distance
 */
function strCompareLite(a,b,locale){
    "use strict";
    if(typeof a!=="string")throw new TypeError("[strCompareFast] a is not a string");
    if(typeof b!=="string")throw new TypeError("[strCompareFast] b is not a string");
    /**@type {Intl.Segmenter}*/let segmenter;
    try{segmenter=new Intl.Segmenter(locale??undefined,{granularity:"grapheme"});}
    catch(err){
        if(err instanceof TypeError||err instanceof RangeError)throw new TypeError(`[strCompareFast] locale is invalid: ${err.message}`);
        throw err;
    }
    if(a===b)return 0;
    let A=Object.freeze(Array.from(segmenter.segment(a),v=>v.segment));
    let B=Object.freeze(Array.from(segmenter.segment(b),v=>v.segment));
    if(A.length<B.length)[A,B]=[B,A];//~ |A| >= |B|
    if(B.length>4294967294)throw new RangeError("[strCompareFast] smaller text has too many grapheme clusters (can not create aux array)");
    let prev=Array.from({length:B.length+1},(_,i)=>i);
    let curr=Array(B.length+1);
    for(let i=0,Ai=A[0];i<A.length;[prev,curr]=[curr,prev],Ai=A[++i]){
        curr[0]=i+1;
        for(let j=0;j<B.length;++j)curr[j+1]=Math.min(//~ deletion, insertion, substitution
            prev[j+1]+1,
            curr[j]+1,
            Ai===B[j]?prev[j]:prev[j]+1
        );
    }
    return prev[B.length];
}

//MARK: number

// https://github.com/MAZ01001/Math-Js#functionsjs
// https://github.com/MAZ01001/Math-Js/blob/main/functions.js

//MARK: color

/**
 * ## convert HSV color to RGB
 * ! notice that {@linkcode H} input is in range `[0,6]` so to convert from `[0,360]` (degrees) divide by `60`; or multiply with `6` if coming from `[0,1]` (like {@linkcode S}/{@linkcode V} input)
 * @param {number} H - hue in range `[0,6]` (angle) where 6=360deg (0 red-yellow-green-cyan-blue-magenta-red 6)
 * @param {number} S - saturation in range `[0,1]` (percentage) where 0=black/white 1=color
 * @param {number} V - value in range `[0,1]` (percentage) where 0=black and 1=white/color
 * @returns {[number,number,number]} `[red, green, blue]` with each in range `[0,1]`
 * @throws {TypeError} if {@linkcode H}, {@linkcode S}, or {@linkcode V} are not numbers withing their documented ranges
 */
function HSVtoRGB(H,S,V){
    "use strict";
    if(Number.isNaN(H)||H<0||H>6)throw new TypeError("[HSVtoRGB] H is not a number in range 0 to 6 inclusive");
    if(Number.isNaN(S)||S<0||S>1)throw new TypeError("[HSVtoRGB] S is not a number in range 0 to 1 inclusive");
    if(Number.isNaN(V)||V<0||V>1)throw new TypeError("[HSVtoRGB] V is not a number in range 0 to 1 inclusive");
    const fac=Math.trunc(H);
    switch(fac){
      case 0://! fall through
      case 6:return[V,V*(1-S*(1-(H-fac))),V*(1-S)];
      case 1:return[V*(1-S*(H-fac)),V,V*(1-S)];
      case 2:return[V*(1-S),V,V*(1-S*(1-(H-fac)))];
      case 3:return[V*(1-S),V*(1-S*(H-fac)),V];
      case 4:return[V*(1-S*(1-(H-fac))),V*(1-S),V];
      case 5:return[V,V*(1-S),V*(1-S*(H-fac))];
    }
    return[0,0,0];
}
/**
 * ## convert RGB color to HSV
 * ! notice that hue output is in range `[0,6]` so multiply with `60` to get `[0,360]` (degrees); or divide by `6` for `[0,1]` (like saturation/value output)
 * @param {number} R - red in range `[0,1]` (percentage of red)
 * @param {number} G - green in range `[0,1]` (percentage of green)
 * @param {number} B - blue in range `[0,1]` (percentage of blue)
 * @returns {[number,number,number]} `[hue, saturation, value]` with hue in range `[0,6]` and the other two in range `[0,1]`
 * @throws {TypeError} if {@linkcode R}, {@linkcode G}, or {@linkcode B} are not numbers in range 0 to 1 inclusive
 */
function RGBtoHSV(R,G,B){
    "use strict";
    if(Number.isNaN(R)||R<0||R>1)throw new TypeError("[RGBtoHSV] R is not a number in range 0 to 1 inclusive");
    if(Number.isNaN(G)||G<0||G>1)throw new TypeError("[RGBtoHSV] G is not a number in range 0 to 1 inclusive");
    if(Number.isNaN(B)||B<0||B>1)throw new TypeError("[RGBtoHSV] B is not a number in range 0 to 1 inclusive");
    const max=Math.max(R,G,B);
    const min=Math.min(R,G,B);
    switch(max){
        case R:return[max===min?0:  (G-B)/(max-min),max===0?0:(max-min)/max,max];
        case G:return[max===min?0:2+(B-R)/(max-min),max===0?0:(max-min)/max,max];
        case B:return[max===min?0:4+(R-G)/(max-min),max===0?0:(max-min)/max,max];
    }
    return[0,0,0];
}
/**
 * ## round hex color from 6/8 digits to 3/4 digits
 * rounded componentwise to nearest hex-double like `F5`→`E`=`EE`
 * @param {string} color - a 6/8 digit hex color (`#RRGGBB` or `#RRGGBBAA` case-insensitive)
 * @returns {string} 3/4 digit hex color (`#RGB` or `#RGBA` lowercase)
 * @throws {TypeError} if {@linkcode color} is not a string
 * @throws {SyntaxError} if {@linkcode color} is not a 6/8 digit hex color
 */
function colorHexRound(color){
    "use strict";
    if(typeof color!=="string")throw new TypeError("[colorHexRound] color is not a string");
    const[_,r,g,b,a]=color.match(/^#([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})?$/i)??[null];
    if(_==null)throw new SyntaxError("[colorHexRound] color is not a 6/8 digit hex color");
    let out="#";
    out+=Math.round(Number.parseInt(r,16)/17).toString(16);
    out+=Math.round(Number.parseInt(g,16)/17).toString(16);
    out+=Math.round(Number.parseInt(b,16)/17).toString(16);
    return a==null?out:(out+Math.round(Number.parseInt(a,16)/17).toString(16));
}
/**
 * ## convert RGB to CMY(K)
 * @param {number} R - red in range `[0,1]` (percentage of red)
 * @param {number} G - green in range `[0,1]` (percentage of green)
 * @param {number} B - blue in range `[0,1]` (percentage of blue)
 * @param {boolean} [excludeK] - [optional] if `true` does not calculate `Key/black` output and gives `0` for that entry - default `false`
 * @returns {[number,number,number,number]} - `[cyan, magenta, yellow, key/black]` with each in range `[0,1]`
 * @throws {TypeError} if {@linkcode R}, {@linkcode G}, or {@linkcode B} are not numbers in range 0 to 1 inclusive
 * @throws {TypeError} if {@linkcode excludeK} is given but not a boolean
 */
function RGBtoCMYK(R,G,B,excludeK){
    "use strict";
    if(typeof R!=="number"||Number.isNaN(R)||R<0||R>1)throw new TypeError("[RGBtoCMYK] R is not a number in range 0 to 1 inclusive");
    if(typeof G!=="number"||Number.isNaN(G)||G<0||G>1)throw new TypeError("[RGBtoCMYK] G is not a number in range 0 to 1 inclusive");
    if(typeof B!=="number"||Number.isNaN(B)||B<0||B>1)throw new TypeError("[RGBtoCMYK] B is not a number in range 0 to 1 inclusive");
    if(excludeK!=null&&typeof excludeK!=="boolean")throw new TypeError("[RGBtoCMYK] excludeK (given) is not a boolean");
    if(excludeK??false)return[1-R,1-G,1-B,0];
    const Ck=1-R;
    const Mk=1-G;
    const Yk=1-B;
    const K=Math.min(Ck,Mk,Yk);
    return[Ck-K,Mk-K,Yk-K,K];
}
/**
 * ## convert CMY(K) to RGB
 * @param {number} C - cyan in range `[0,1]` (percentage of cyan)
 * @param {number} M - magenta in range `[0,1]` (percentage of magenta)
 * @param {number} Y - yellow in range `[0,1]` (percentage of yellow)
 * @param {number} [K] - [optional] key/black in range `[0,1]` (percentage of black) - default `0`
 * @returns {[number,number,number]} `[red, green, blue]` with each in range `[0,1]`
 * @throws {TypeError} if {@linkcode C}, {@linkcode M}, or {@linkcode Y} are not numbers in range 0 to 1 inclusive
 * @throws {TypeError} if {@linkcode K} is given but not a number in range 0 to 1 inclusive
 * @throws {RangeError} if {@linkcode K} (is given and) combined with {@linkcode C}, {@linkcode M}, or {@linkcode Y} results in a number larger than `1`
 */
function CMYKtoRGB(C,M,Y,K){
    "use strict";
    if(typeof C!=="number"||Number.isNaN(C)||C<0||C>1)throw new TypeError("[CMYKtoRGB] C is not a number in range 0 to 1 inclusive");
    if(typeof M!=="number"||Number.isNaN(M)||M<0||M>1)throw new TypeError("[CMYKtoRGB] M is not a number in range 0 to 1 inclusive");
    if(typeof Y!=="number"||Number.isNaN(Y)||Y<0||Y>1)throw new TypeError("[CMYKtoRGB] Y is not a number in range 0 to 1 inclusive");
    if(K==null)return[1-C,1-M,1-Y];
    if(typeof K!=="number"||Number.isNaN(K)||K<0||K>1)throw new TypeError("[CMYKtoRGB] K (given) is not a number in range 0 to 1 inclusive");
    if(Math.max(C,M,Y)+K>1)throw new RangeError("[CMYKtoRGB] combination with K is out of range");
    return[1-(C+K),1-(M+K),1-(Y+K)];
}

//MARK: array

/**
 * ## checks if the given array has empty slots
 * most iterator functions skip empty entries, like {@linkcode Array.every} and {@linkcode Array.some}, so they might bypass checks and lead to undefined behavior \
 * their value is `undefined` but they're treated differently from an actual `undefined` in the array \
 * but the length attribute does include them since they do contribute to the total length of the array
 * @param {any[]} arr - an array of items (any items, incl. `undefined` are allowed)
 * @returns {boolean} true if the array has empty slots and false otherwise
 * @example
 * hasArrayHoles(["",0,undefined,,,,null,()=>{},[],{}]); //=> true
 * hasArrayHoles(["",0,undefined,null,()=>{},[],{}]);    //=> false
 */
const hasArrayHoles=arr=>arr.length>arr.reduce(s=>s+1,0);
/**
 * ## Binary search in {@linkcode arr} (ascending sorted array) for {@linkcode e}
 * using `!=` and `<` (supports dynamic types), {@linkcode arr} can have duplicate elements
 * @param {any[]} arr - list of [compareable](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Less_than#description "MDN:JS < operator (description)") elements (sorted in ascending order)
 * @param {any} e - element in {@linkcode arr} (presumably)
 * @returns {number} index of {@linkcode e} in {@linkcode arr} or index of next smaller element (`-1` when smaller than first element)
 * @throws {TypeError} if {@linkcode arr} is not an array
 */
function binarySearch(arr,e){
    "use strict";
    if(!Array.isArray(arr))throw new TypeError("[binarySearch] arr is not an array");
    let l=0,r=arr.length-1,i=Math.trunc((r-l)*.5);
    for(;arr[i]!=e;i=Math.trunc(l+(r-l)*.5)){
        if(arr[i]<e)l=i+1;
        else r=i-1;
        if(l>r)return r;
    }
    return i;
}

//MARK: HTML / DOM

/**
 * ## measures the dimensions of a given {@linkcode text} in pixels (sub-pixel accurate)
 * [!] only works in the context of HTML ie. a browser [!] \
 * for using an elements font use {@linkcode CSSStyleDeclaration.font} of {@linkcode window.getComputedStyle} ie `window.getComputedStyle(element, pseudoElementOrNull).font` \
 * if using `"initial"`, `"revert"`, or any similar or invalid value as font, it seems to use `"10px sans-serif"` (default of {@linkcode OffscreenCanvasRenderingContext2D.font})
 * @param {string} text - the string to calculate the dimensions of in pixels
 * @param {string} [fontCSS] - [optional] CSS font - default uses computed font of {@linkcode document.body}
 * @returns {number} the width of the {@linkcode text} in CSS pixels (sub-pixel accurate)
 * @throws {Error} if {@linkcode Window} or {@linkcode Document} are not defined (not in HTML context)
 * @throws {TypeError} if {@linkcode text} or {@linkcode fontCSS} (if given) are not strings
 * @throws {ReferenceError} (internal error) if (offscreen) canvas 2d context could not be created/linked
 */
function getTextDimensions(text,fontCSS){
    "use strict";
    if(
        (typeof Window==="undefined")
        ||(typeof Document==="undefined")
    )throw new Error("[getTextDimensions] called outside the context of HTML (Window and Document are not defined)");
    if(typeof text!=="string")throw new TypeError("[getTextDimensions] text is not a string");
    if(fontCSS!=null&&typeof fontCSS!=="string")throw new TypeError("[getTextDimensions] fontCSS (given) is not a string");
    if(this.cnv2d==null)
        if((this.cnv2d=new OffscreenCanvas(0,0).getContext("2d"))==null)throw new ReferenceError("[getTextDimensions] (internal error) could not get (offscreen) canvas 2d context");
    this.cnv2d.font=fontCSS??getComputedStyle(document.body).font;
    return this.cnv2d.measureText(text).width;
    // ((txt,font)=>Object.assign(new OffscreenCanvas(0,0).getContext("2d"),{font}).measureText(txt).width)("Hello, World!","16px consolas,monospace");
}
/**
 * ## gets current mouse position (optionally relative to an element)
 * [!] only works in the context of HTML ie. a browser [!] \
 * __WARNING__: \
 * Browsers may use different units for movementX and screenX than what the specification defines. \
 * The movementX units can be physical, logical, or CSS pixels, depending on the browser and operating system. \
 * _See [this issue on GitHub](https://github.com/w3c/pointerlock/issues/42) for more information on the topic._
 * @param {Element|null} [offsetElement] - [optional] HTML element for calculating relative / offset mouse position - default `null` (none)
 * @returns {[Readonly<{page:[number,number];client:[number,number];offset:[number,number];screen:[number,number];movement:[number,number]}>,AbortController]} mouse positions (reference to sealed object) and abort controller for `mousemove` event listener on `document` \
 * mouse `[X,Y]` positions (sealed arrays) automatically updated (`offset` will be (0,0) when {@linkcode offsetElement} is `null`)
 * | attribute  | description                                                           |
 * | ---------- | --------------------------------------------------------------------- |
 * | `page`     | position on the rendered page (actual page size >= browser window)    |
 * | `client`   | position on the visible portion on the rendered page (browser window) |
 * | `offset`   | relative position on the rendered page (to an elements position)      |
 * | `screen`   | position on screen (from top left of all monitors)                    |
 * | `movement` | distance moved from previous `screen` position                        |
 * @throws {Error} if {@linkcode Window} or {@linkcode Document} are not defined (not in HTML context)
 * @throws {TypeError} if {@linkcode offsetElement} is set but is not an (HTML) element
 * @example
 * const [mousePos, mouseSignal] = getMousePos();
 * const log = setInterval(() => console.log(JSON.stringify(mousePos)), 1000);
 * // mouseSignal.abort();
 * // clearInterval(log);
 */
function getMousePos(offsetElement){
    "use strict";
    if(
        typeof Window==="undefined"
        ||typeof Document==="undefined"
    )throw new Error("[getMousePos] called outside the context of HTML (Window and Document are not defined)");
    if(offsetElement!=null&&!(offsetElement instanceof Element))throw new TypeError("[getMousePos] offsetElement (given) is not an (HTML) element");
    /**@type {Element|null|undefined} - current/last (optional) offset element*/
    this.offsetElement=offsetElement;
    if(this.ctrl?.signal.aborted??true){
        if(this.obj==null)
            /**
             * @type {Readonly<{page:[number,number];client:[number,number];offset:[number,number];screen:[number,number];movement:[number,number]}>} - various mouse positions (readonly object) \
             * mouse `[X,Y]` positions (sealed arrays)
             * | attribute  | description                                                           |
             * | ---------- | --------------------------------------------------------------------- |
             * | `page`     | position on the rendered page (actual page size >= browser window)    |
             * | `client`   | position on the visible portion on the rendered page (browser window) |
             * | `offset`   | relative position on the rendered page (to an elements position)      |
             * | `screen`   | position on screen (from top left of all monitors)                    |
             * | `movement` | distance moved from previous `screen` position                        |
             */
            this.obj=Object.freeze({
                /**@type {[number,number]}`[X,Y]`*/page:Object.seal([0,0]),
                /**@type {[number,number]}`[X,Y]`*/client:Object.seal([0,0]),
                /**@type {[number,number]}`[X,Y]`*/offset:Object.seal([0,0]),
                /**@type {[number,number]}`[X,Y]`*/screen:Object.seal([0,0]),
                /**@type {[number,number]}`[X,Y]`*/movement:Object.seal([0,0])
            });
        if(this.listener==null)
            /**
             * @type {(ev:MouseEvent)=>void} - set instance variables with current mouse position
             * @param {MouseEvent} ev - `mousemove` event
             */
            this.listener=ev=>{
                "use strict";
                this.obj.page[0]=ev.pageX;        this.obj.page[1]=ev.pageY;
                this.obj.client[0]=ev.clientX;    this.obj.client[1]=ev.clientY;
                this.obj.screen[0]=ev.screenX;    this.obj.screen[1]=ev.screenY;
                this.obj.movement[0]=ev.movementX;this.obj.movement[1]=ev.movementY;
                if(this.offsetElement==null)this.obj.offset[1]=this.obj.offset[0]=0;
                else{
                    const bcr=this.offsetElement.getBoundingClientRect();
                    this.obj.offset[0]=this.obj.page[0]-bcr.x;
                    this.obj.offset[1]=this.obj.page[1]-bcr.y;
                }
            };
        /**@type {AbortController} - abort controller for `mousemove` event listener on `document`*/
        this.ctrl=new AbortController();
        document.addEventListener("mousemove",this.listener,{passive:true,signal:this.ctrl.signal});
    }
    return[this.obj,this.ctrl];
}
/**
 * ## Shows gradients at the edges of {@linkcode el} when it overflows and becommes scrollable
 * [!] only works in the context of HTML ie. a browser [!] \
 * Overrides the CSS `background` property (use {@linkcode background} to add any additional value/s for CSS)
 * @param {HTMLElement} el - The HTML element for styling
 * @param {number|[number,number]} offset - The minimum scroll distance (in pixels - positive and non-zero) at which to start blending out the alpha of {@linkcode color} (from {@linkcode alphaMax} to 0) - use a two numbers as `[x, y]` to set horizontal and vertical offset individually
 * @param {string|[string,string]} size - The size of the gradients as CSS length value (`1rem` / `16px` / `clamp(1rem, 5%, 2rem)`) - use a two values as `[width, height]` to set width and height individually
 * @param {string} color - A CSS color like `rgb(255 153 0 / $f)`, `#FF9900$x`, or `oklch(77.2% .17 64.55 / $%)` for the color of each gradient
 * - replaces all `$f` with floatingpoint numbers from 0 to 1 (inclusive)
 * - replaces all `$i` with integers from 0 to 255 (inclusive)
 * - replaces all `$x` with double digit hex numbers from 00 to FF (inclusive) excluding any prefix
 * - replaces all `$%` with percentages from 0 to 100 (inclusive) including the % sign
 * - use \ to escape $
 * @param {number} alphaMax - max value for the alpha of {@linkcode color} (0 to 1 inclusive)
 * @param {string|null} [background] - [optional] additional value/s for the CSS `background` property (excluding trailing semicolon)
 * @returns {()=>void} call this function to update the styles with the current scroll position and offset
 * @throws {Error} if {@linkcode Window} or {@linkcode Document} are not defined (not in HTML context)
 * @throws {TypeError} if {@linkcode el}, {@linkcode offset}, {@linkcode size}, {@linkcode color}, {@linkcode alphaMax}, or {@linkcode background} (if given) are not one of the defined types
 * @example
 * const box = document.getElementById("box"),
 *     boxOverflowUpdate = StyleOverflowFor(box, 0xC8, ["clamp(1rem, 5vw, 2rem)", "clamp(1rem, 5vh, 2rem)"], "#CCCC00$x", 2/3, "#0008");
 * boxOverflowUpdate();
 * box.addEventListener("scroll", boxOverflowUpdate, {passive: true});
 * window.addEventListener("resize", boxOverflowUpdate, {passive: true});
 */
function styleOverflowFor(el,offset,size,color,alphaMax,background){
    "use strict";
    if(
        typeof Window==="undefined"
        ||typeof Document==="undefined"
    )throw new Error("[styleOverflowFor] called outside the context of HTML (Window and Document are not defined)");
    if(!(el instanceof HTMLElement))throw new TypeError("[styleOverflowFor] el is not an HTML element");
    if(Array.isArray(offset)){
        if(offset.length!==2)throw new TypeError("[styleOverflowFor] offset is not an array with two entries");
        if(offset.some(v=>typeof v!=="number"||v<=0))throw new TypeError("[styleOverflowFor] offset is not an array with two non-zero positive numbers");
    }else if(typeof offset!=="number"||offset<=0)throw new TypeError("[styleOverflowFor] offset is not a non-zero positive number");
    else{offset=[offset,offset];}
    if(Array.isArray(size)){
        if(size.length!==2)throw new TypeError("[styleOverflowFor] size is not an array with two entries");
        if(size.some(v=>typeof v!=="string"))throw new TypeError("[styleOverflowFor] size is not an array with two strings");
    }else if(typeof size!=="string")throw new TypeError("[styleOverflowFor] size is not a string");
    else{size=[size,size];}
    if(typeof color!=="string")throw new TypeError("[styleOverflowFor] color is not a string");
    if(typeof alphaMax!=="number"||alphaMax<0||alphaMax>1)throw new TypeError("[styleOverflowFor] alphaMax is not a number between 0 and 1");
    if(background!=null&&typeof background!=="string")throw new TypeError("[styleOverflowFor] background is given but is not a string");
    /**@type {(n:number)=>number} - clamps {@linkcode n} to 0-1*/
    const Clamp01=n=>n>1?1:n<0?0:n;
    return()=>{
        "use strict";
        const[top,left,bottom,right]=[
            alphaMax*Clamp01(el.scrollTop/offset[1]),
            alphaMax*Clamp01(el.scrollLeft/offset[0]),
            alphaMax*Clamp01((el.scrollHeight-(el.scrollTop+el.clientHeight))/offset[1]),
            alphaMax*Clamp01((el.scrollWidth-(el.scrollLeft+el.clientWidth))/offset[0])
        ].map(v=>color
            .replace(/(?<!\\)\$f/g,String(v))
            .replace(/(?<!\\)\$i/g,(v*255).toFixed(0))
            .replace(/(?<!\\)\$x/g,(v*255).toString(16).padStart(2,"0"))
            .replace(/(?<!\\)\$%/g,`${v*100}%`)
        );
        el.style.background=`
            scroll linear-gradient(to top,    transparent, ${top})    center top    / 100% ${size[1]}      no-repeat,
            scroll linear-gradient(to left,   transparent, ${left})   center left   /      ${size[0]} 100% no-repeat,
            scroll linear-gradient(to bottom, transparent, ${bottom}) center bottom / 100% ${size[1]}      no-repeat,
            scroll linear-gradient(to right,  transparent, ${right})  center right  /      ${size[0]} 100% no-repeat
            ${background==null?"":`, ${background}`}
        `;
    };
}

//MARK: other

/**## fetch image from URL and convert it to (base64) data-URL*/
const LoadIMG=class LoadIMG{
    /**
     * ### fetch via `<img>` and convert via `<canvas>`
     * [!] only works in the context of HTML ie. a browser [!] \
     * for offline viewing (asynchronous) \
     * high chance of being blocked by CORS when it's not called on the page (HTML context) where the image is displayed
     * @returns {(src:string)=>Promise<string|null>} async function (for repeated calls) to get data-URL (`null` when image can't be loaded)
     * @throws {Error} if {@linkcode Window} or {@linkcode Document} are not defined (not in HTML context)
     * @throws {TypeError} [by returned function] if {@linkcode src} is not a string
     * @throws {ReferenceError} (internal error) if canvas 2d context could not be created/linked
     */
    static IMG(){
        if(
            typeof Window==="undefined"
            ||typeof Document==="undefined"
        )throw new Error("[LoadIMG:IMG] called outside the context of HTML (Window and Document are not defined)");
        /**@type {HTMLCanvasElement}*/
        const cnv=document.createElement("canvas");//~ use the same canvas object for all calls to save some resources
        /**@type {CanvasRenderingContext2D}*///@ts-ignore checked for null right after
        const cnx=cnv.getContext("2d");
        if(cnx==null)throw new ReferenceError("[LoadIMG:IMG] (internal error) could not get 2d canvas context");
        /**@type {HTMLImageElement}*/
        const img=new Image();
        img.loading="eager";
        img.crossOrigin="anonymous";//~ make sure canvas does not taint and can still convert itself to data URL
        /**
         * #### get IMG as data-URL
         * for offline viewing (asynchronous) \
         * high chance of being blocked by CORS when it's not called on the page (HTML context) where the image is displayed
         * @param {string} src - image URL
         * @returns {Promise<string|null>} data-URL or `null` when image can't be loaded
         * @throws {TypeError} if {@linkcode src} is not a string
         */
        return async src=>{
            if(typeof src!=="string")throw new TypeError("[LoadIMG:IMG] src is not a string");
            const load=new Promise(res=>{
                img.onload=()=>res(false);
                img.onerror=()=>res(true);
            });
            img.src=src;
            if(await load)return null;
            cnv.width=img.naturalWidth;
            cnv.height=img.naturalHeight;
            cnx.drawImage(img,0,0);
            return cnv.toDataURL();
        };
    }
    /**
     * ### fetch via {@linkcode fetch} and convert via {@linkcode FileReader}
     * [!] only works in the context of HTML ie. a browser [!] \
     * for offline viewing (asynchronous)
     * @returns {(src:string)=>Promise<string|null>} async function (for repeated calls) to get data-URL (`null` when image can't be loaded)
     * @throws {Error} if {@linkcode Window} or {@linkcode Document} are not defined (not in HTML context)
     * @throws {TypeError} [by returned function] if {@linkcode src} is not a string
     */
    static FetchFileReader(){
        if(
            typeof Window==="undefined"
            ||typeof Document==="undefined"
        )throw new Error("[LoadIMG:FetchFileReader] called outside the context of HTML (Window and Document are not defined)");
        const fr=new FileReader();
        /**
         * #### get IMG as data-URL
         * for offline viewing (asynchronous)
         * @param {string} src - image URL
         * @returns {Promise<string|null>} data-URL or `null` when image can't be loaded
         * @throws {TypeError} if {@linkcode src} is not a string
         */
        return async src=>{
            if(typeof src!=="string")throw new TypeError("[LoadIMG:FetchFileReader] src is not a string");
            const load=new Promise(res=>{
                fr.onload=()=>res(true);
                fr.onerror=()=>res(false);
                fr.onabort=()=>res(false);
            });
            const res=await fetch(src).catch(()=>null);
            if(res==null||!res.ok)return null;
            const blob=await res.blob().catch(()=>null);
            if(blob==null)return null;
            fr.readAsDataURL(blob);
            return(await load)?String(fr.result):null;
        };
    }
    /**
     * ### fetch via {@linkcode fetch} and convert manually to base64
     * for offline viewing (asynchronous)
     * @returns {(src:string)=>Promise<string|null>} async function (for repeated calls) to get data-URL (`null` when image can't be loaded)
     * @throws {TypeError} [by returned function] if {@linkcode src} is not a string
     */
    static FetchManual(){
        /**@type {(bytes:Uint8Array<ArrayBuffer>)=>string} <https://en.wikipedia.org/wiki/Base64>*/
        const b64=bytes=>{
            const b64="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            let res="";
            for(let bitmap,i=0;i<bytes.length;i+=3){
                bitmap=(bytes[i]<<16)|(bytes[i+1]<<8)|bytes[i+2];
                res+=b64.charAt(bitmap>>18&63)
                    +b64.charAt(bitmap>>12&63)
                    +b64.charAt(bitmap>>6&63)
                    +b64.charAt(bitmap&63);
            }
            const rest=bytes.length%3;
            return rest!==0?res.slice(0,rest-3)+"===".substring(rest):res;
        };
        /**
         * #### get IMG as data-URL
         * for offline viewing (asynchronous)
         * @param {string} src - image URL
         * @returns {Promise<string|null>} data-URL or `null` when image can't be loaded
         * @throws {TypeError} if {@linkcode src} is not a string
         */
        return async src=>{
            if(typeof src!=="string")throw new TypeError("[LoadIMG:FetchManual] src is not a string");
            const fetchRes=await fetch(src).catch(()=>null);
            if(fetchRes==null||!fetchRes.ok)return null;
            const blob=await fetchRes.blob().catch(()=>null);
            if(blob==null)return null;
            return`data:${blob.type};base64,${b64(await blob.bytes())}`;
        };
    }
};

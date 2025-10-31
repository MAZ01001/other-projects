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

//MARK: number

// https://github.com/MAZ01001/Math-Js#functionsjs
// https://github.com/MAZ01001/Math-Js/blob/main/functions.js

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

/* some useful js functions */

//~ string
/**
 * __inserts a string at a specific index__
 * @param {string} str - initial string
 * @param {number} i - (zero-based) index of `str` - can be negative
 * @param {string} r - replacement string to be inserted in `str` at `i`
 * @param {number} d - delete count of characters in `str` at `i`
 * @returns {string} modified string
 * @throws {TypeError} if `i` or `d` are not whole numbers
 * @example strInsert('Hello#World!',-6,', ',-1);//=> 'Hello, World!'
 */
function strInsert(str,i=0,r='',d=0){
    i=Number(i);if(!Number.isInteger(i)){throw new TypeError('[i] is not a whole number.');}
    d=Number(d);if(!Number.isInteger(d)){throw new TypeError('[d] is not a whole number.');}
    str=String(str);
    r=String(r);
    if(i<0)i=str.length+i;
    return str.substring(0,i)+r+str.substring(i+d);
}
/**
 * __object of how much each character appears in the string__\
 * _or for only the given characters_
 * @param {string} str - the string for analysis
 * @param {string} chars - if given, searches only the amount for these characters - _default `''` = all_
 * @returns {Readonly<{[string]:number;other:number;}>} object with amount of apperance (`'a':8, 'b':2, ..., other:0`)
 * @example
 * strCharStats('abzaacdd');       //~ Readonly<{'a':3, 'b':1, 'z':1, 'c':1, 'd':2}>
 * strCharStats('abzaacdd','abce');//~ Readonly<{'a':3, 'b':1, 'c':1, 'e':0, 'other':3}>
 */
function strCharStats(str,chars=''){
    //~ getUnique >>> str.split('').sort().join('').replace(/([\s\S])\1+/,'$1').replace(/(([\s\S])\2*)/g,(m,a,z)=>`${z} - ${a.length}\n`);
    str=String(str);
    chars=String(chars);
    /** @type {{string:number;}} */
    let obj={};
    if(chars==='')for(const char of str)obj[char]=(obj[char]??0)+1;
    else{
        obj.other=0;
        for(const char of chars)obj[char]=0;
        for(const char of str){
            if(typeof obj[char]==='undefined')obj.other++;
            else obj[char]++;
        }
    }
    return Object.freeze(obj);
}
/**
 * ## Create ANSI codes to set terminal color
 * sets output terminal fore/background colors \
 * ! remember to output the reset code before end of script or terminal colors stay this way \
 * for browser dev-console use `console.log("%cCSS", "background-color: #000; color: #F90");` instead \
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
 * @example console.log(`TEST${ansi(0xff9900)}TEST${ansi()}TEST`);
 */
function ansi(c,bg){
    "use strict";
    if(c==null)return"\x1b[0m";
    if(typeof c==="number")c=`${(c&0xff0000)>>>16};${(c&0xff00)>>>8};${c&0xff}`;
    else{
        if(!Array.isArray(c)||c.length!==3||c.some(v=>typeof v!=="number"))throw new TypeError("[c] c must be either null, a (24bit) number, or an array of 3 (8bit) numbers.");
        c=`${c[0]&=0xff};${c[1]&=0xff};${c[2]&=0xff}`;
    }
    if(bg==null)return`\x1b[38;2;${c}m`;
    if(typeof bg!=="number"||Number.isNaN(bg))throw new TypeError("[c] bg is given but not a number.");
    switch(Math.sign(bg)){
        case 1:return`\x1b[38;2;${c}m`;
        case 0:return`\x1b[38;2;${c};48;2;${c}m`;
        case-1:return`\x1b[48;2;${c}m`;
    }
}

//~ date (string|number)
/**
 * __format date with custom separators__
 * @param {Date|null} dt - a valid date - default current date
 * @param {boolean|null} utc - if `true` uses UTC otherwise local time - default `false`
 * @param {string|string[]|null} separators - the separators between each block (from left) - 6 possible separators - default `"__-__."`
 * @returns {string} date in format (with default {@linkcode separators}) `YYYY_MM_dd-HH_mm_ss.ms` (zero padded to fit this format - ms is 3 digits)
 * @example
 * getDate(null,null,[,," - ",,,"."]); //=> "YYYYMMdd - HHmmss.ms"
 * getDate(null,null,""); //=> "YYYYMMddHHmmssms"
 */
function formatDate(dt,utc,separators){
    "use strict";
    if(dt==null)dt=new Date();
    if(utc==null)utc=false;
    if(separators==null)separators="__-__.";
    return[
        (utc?dt.getUTCFullYear():dt.getFullYear()).toString().padStart(4,"0"),
        ((utc?dt.getUTCMonth():dt.getMonth())+1).toString().padStart(2,"0"),
        (utc?dt.getUTCDate():dt.getDate()).toString().padStart(2,"0"),
        (utc?dt.getUTCHours():dt.getHours()).toString().padStart(2,"0"),
        (utc?dt.getUTCMinutes():dt.getMinutes()).toString().padStart(2,"0"),
        (utc?dt.getUTCSeconds():dt.getSeconds()).toString().padStart(2,"0"),
        (utc?dt.getUTCMilliseconds():dt.getMilliseconds()).toString().padStart(3,"0")
    ].reduce((o,v,i)=>o+String(separators[i-1]??"")+v);
}
/**## Number of milliseconds from `0000-01-01 00:00` to `1970-01-01 00:00` (UTC)*/
const UTC_OFFSET=62125920000000;
//// console.log(UTC_OFFSET+Date.now());

//~ number
//// see https://github.com/MAZ01001/Math-Js/blob/main/functions.js

//~ array
/**
 * __checks if the given array has empty slots__ \
 * most iterator functions skip empty entries, like `every` and `some`, so they might bypass checks and lead to undefined behavior \
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
    while(arr[i]!=e){
        if(arr[i]<e)l=i+1;
        else r=i-1;
        if(l>r)return r;
        i=Math.trunc(l+(r-l)*.5);
    }
    return i;
}

//~ HTML / DOM
/**
 * __measures the dimensions of a given `text` in pixels (sub-pixel accurate)__ \
 * [!] only works in the context of `HTML` ie. a browser [!]
 * @param {string} text - the string to calculate the dimensions of in pixels
 * @param {Element} element - (HTML) element to get the font informations from - _default `document.body`_
 * @param {string} pseudoElt - if set get the pseudo-element of `element`, for example `':after'` - _default `null` (no pseudo element, `element` itself)_
 * @returns {Readonly<{width:number;height:number;lineHeight:number;}>} the dimensions of the text in pixels (sub-pixel accurate)
 * @throws {Error} if `Window` or `Document` are not defined (not in HTML context)
 * @throws {TypeError} if `element` is not an (HTML) `Element`
 */
function getTextDimensions(text,element=document.body,pseudoElt=null){
    if(
        (typeof Window==='undefined')
        ||(typeof Document==='undefined')
    )throw new Error('[getTextDimensions] called outside the context of HTML (Window and Document are not defined)');
    if(!(element instanceof Element))throw new TypeError('[getTextDimensions] element is not an (HTML) element');
    text=String(text);
    if(typeof this.cnv2d==='undefined')this.cnv2d=document.createElement('canvas').getContext('2d');
    const elementCSS=getComputedStyle(element,pseudoElt);
    this.cnv2d.font=elementCSS.font;
    return Object.freeze({
        width:this.cnv2d.measureText(text).width,
        height:elementCSS.fontSize,
        lineHeight:elementCSS.lineHeight
    });
}
/**
 * __copy data to clipboard__ \
 * [!] only works in the context of `HTML` ie. a browser [!] \
 * _might not work for non-chromium-browsers, because of the permission check_
 * @param {string|Blob} data - a text or rich-content in the form of a `Blob`
 * @returns {Promise} resolves the promise if the copying of `data` to the clipboard was successful and rejects the promise when:
 * - `Document` is not defined (not in HTML context)
 * - `Document` is not in focus
 * - `navigator.permissions.query` is not defined (can not check for permissions to write to the clipboard)
 * - has no permission to write to the clipboard (or in a non-chromium-browser)
 * @example
 *   setTimeout(
 *       ()=>copyToClipboard('Hello, World!').then(
 *           ()=>console.log('success'),
 *           reason=>console.log('error: %O',reason)
 *       ),3000
 *   ); //~ with 3 seconds to focus on the document
 */
function copyToClipboard(data){
    return new Promise((resolve,reject)=>{
        //~ reject instead of throw
        if(typeof Document==='undefined'){
            reject('[copyToClipboard] called outside the context of HTML (Document is not defined)');
            return;
        }
        if(!document.hasFocus()){
            reject('[copyToClipboard] HTML document must be in focus');
            return;
        }
        if(
            typeof Navigator===undefined
            ||typeof Permissions===undefined
            ||typeof(navigator?.permissions?.query??undefined)!=='function'
        ){
            reject('[copyToClipboard] `navigator.permissions.query` is not defined');
            return;
        }
        navigator.permissions.query(Object.freeze({name:'clipboard-write'})).then(result=>{
            if(result.state==='granted'){
                if(data instanceof Blob)
                    navigator.clipboard.write([new ClipboardItem(Object.freeze({[data.type]:data}))])
                    .then(()=>resolve(),reason=>reject(reason));
                else
                    navigator.clipboard.writeText(String(data))
                    .then(()=>resolve(),reason=>reject(reason));
            }else reject('[copyToClipboard] no permission to write to the clipboard (or in a non-chromium-browser)');
        },reason=>reject(reason));
    });
}
/**
 * __gets different mouse positions__ \
 * [!] only works in the context of `HTML` ie. a browser [!]
 * @description __Warning__:
 * Browsers use different units for movementX and screenX than what the specification defines.
 * Depending on the browser and operating system, the movementX units
 * may be a physical pixel, a logical pixel, or a CSS pixel. \
 * _(see [this issue on GitHub](https://github.com/w3c/pointerlock/issues/42) for more information on the topic)_
 * @param {Element?} offsetElement - HTML element for calculating relative / offset mouse position - _default `null`_
 * @returns {Readonly<{pageX:number;pageY:number;clientX:number;clientY:number;offsetX:number;offsetY:number;screenX:number;screenY:number;movementX:number;movementY:number;}>} mouse X and Y positions (read only)
 * - `page`       : position on the rendered page (actual page size >= browser window)
 * - `client`     : position on the visible portion on the rendered page (browser window)
 * - `offset`     : relative position on the rendered page (to an elements position)
 * - `screen`     : position on screen (from top left of all monitors)
 * - `movement`   : distance moved from previous `screen` position
 * @throws {Error} if `Window` or `Document` are not defined (not in HTML context)
 * @throws {TypeError} if `offsetElement` is set but is not an (HTML) `Element`
 * @example
 * const log = setInterval( () => console.log( JSON.stringify( getMousePos() ) ), 1000 );
 * // clearInterval( log );
 */
function getMousePos(offsetElement=null){
    if(
        typeof Window==='undefined'
        ||typeof Document==='undefined'
    )throw new Error('[getMousePos] called outside the context of HTML (Window and Document are not defined)');
    if(this.obj==null){//~ ==null checks for null and undefined
        /**
         * @type {{page:number[];client:number[];offset:number[];screen:number[];movement:number[];}} - various mouse positions (sealed object)
         * @description mouse `[X,Y]` positions (sealed arrays)
         * - `page`       : position on the rendered page (actual page size >= browser window)
         * - `client`     : position on the visible portion on the rendered page (browser window)
         * - `offset`     : relative position on the rendered page (to an elements position)
         * - `screen`     : position on screen (from top left of all monitors)
         * - `movement`   : distance moved from previous `screen` position
         */
        this.obj=Object.seal({
            page:Object.seal([0,0]),
            client:Object.seal([0,0]),
            offset:Object.seal([0,0]),
            screen:Object.seal([0,0]),
            movement:Object.seal([0,0]),
        });
        /**
         * __set instance variables with current mouse position__
         * @param {MouseEvent} e - the mouse event from a `mousemove` event listener
         */
        this.listener=e=>{
            this.obj.page=[e.pageX,e.pageY];
            this.obj.client=[e.clientX,e.clientY];
            this.obj.screen=[e.screenX,e.screenY];
            this.obj.movement=[e.movementX,e.movementY];
        };
        document.addEventListener('mousemove',this.listener,{passive:true});
        /**
         * __deletes the current instance__ \
         * also removes the `mousemove` event listener from the `document`
         */
        this.deleteInstance=function(){
            document.removeEventListener('mousemove',this.listener,{passive:true});
            delete this.listener;
            delete this.obj;
            delete this.deleteInstance;
        };
    }
    if(offsetElement===null)this.obj.offset=[0,0];
    else{
        if(!(offsetElement instanceof Element))throw new TypeError('[getMousePos] offsetElement is not an (HTML) element');
        const offsetElementBCR=offsetElement.getBoundingClientRect();
        this.obj.offset[0]=this.obj.page[0]-offsetElementBCR.x;
        this.obj.offset[1]=this.obj.page[1]-offsetElementBCR.y;
    }
    return Object.freeze({
        pageX:this.obj.page[0],
        pageY:this.obj.page[1],
        clientX:this.obj.client[0],
        clientY:this.obj.client[1],
        screenX:this.obj.screen[0],
        screenY:this.obj.screen[1],
        movementX:this.obj.movement[0],
        movementY:this.obj.movement[1]
    });
}
/**
 * ## pads overflow of {@linkcode el} (pad left/top the same as the width/height of `-webkit-scrollbar`) \
 * if {@linkcode el} is not an {@linkcode HTMLElement} ends silently (without doing anything)
 * @param {HTMLElement} el - the element to pad (according to overflow and sizes of `-webkit-scrollbar`)
 * @throws {Error} if `Window` is not defined (not in HTML context)
 * @deprecated use [CSS `scrollbar-gutter: stable both-edges;`](https://developer.mozilla.org/en-US/docs/Web/CSS/scrollbar-gutter) instead
 * @example
 * //~ to automatically update element "ID" when document loads and on every resize of the browser window
 * window.addEventListener("resize",()=>PadOverflowFor(document.getElementById("ID")),{passive:true});
 * window.addEventListener("DOMContentLoaded",()=>PadOverflowFor(document.getElementById("ID")),{passive:true,once:true});
 */
function PadOverflowFor(el){
    "use strict";
    if(window==null)throw new Error("[PadOverflowFor] called outside the context of HTML (Window is not defined)");
    if(!(el instanceof HTMLElement))return;
    const[x,y]=(({clientWidth,clientHeight,scrollWidth,scrollHeight})=>[scrollWidth>clientWidth,scrollHeight>clientHeight])(el),
        {width:scW,height:scH}=window.getComputedStyle(el,"-webkit-scrollbar"),
        {paddingLeft:pl,paddingTop:pt}=window.getComputedStyle(el);
    if(el.dataset.ox==="1"){
        if(!x){
            //~ horizontal scrollbar disappears (no X-overflow)
            el.style.paddingTop=`calc(${pt} - ${scH})`;
            el.dataset.ox="0";
        }
    }else if(x){
        //~ horizontal scrollbar appears (X-overflow)
        el.style.paddingTop=`calc(${pt} + ${scH})`;
        el.dataset.ox="1";
    }
    if(el.dataset.oy==="1"){
        if(!y){
            //~ vertical scrollbar disappears (no Y-overflow)
            el.style.paddingLeft=`calc(${pl} - ${scW})`;
            el.dataset.oy="0";
        }
    }else if(y){
        //~ vertical scrollbar appears (Y-overflow)
        el.style.paddingLeft=`calc(${pl} + ${scW})`;
        el.dataset.oy="1";
    }
}
/**
 * ## Shows gradients at the edges of {@linkcode el} when it overflows and becommes scrollable
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
 * @param {string} [background] - [optional] additional value/s for the CSS `background` property (excluding trailing semicolon)
 * @returns {()=>void} call this function to update the styles with the current scroll position and offset
 * @example
 * const box = document.getElementById("box"),
 *     boxOverflowUpdate = StyleOverflowFor(box, 0xC8, ["clamp(1rem, 5vw, 2rem)", "clamp(1rem, 5vh, 2rem)"], "#CCCC00$x", 2/3, "#0008");
 * boxOverflowUpdate();
 * box.addEventListener("scroll", boxOverflowUpdate, {passive: true});
 * window.addEventListener("resize", boxOverflowUpdate, {passive: true});
 */
function StyleOverflowFor(el,offset,size,color,alphaMax,background){
    "use strict";
    if(typeof HTMLElement==="undefined")throw new Error('[styleOverflowFor] called outside the context of HTML (HTMLElement is not defined)');
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
    /**
     * ## Clamps {@linkcode n} between 0 and 1
     * @param {number} n - a number
     * @returns {number} the clamped number
     */
    const Clamp01=n=>n>1?1:n<0?0:n;
    return()=>{
        "use strict";
        const[top,left,bottom,right]=[
            alphaMax*Clamp01(el.scrollTop/offset[1]),
            alphaMax*Clamp01(el.scrollLeft/offset[0]),
            alphaMax*Clamp01((el.scrollHeight-(el.scrollTop+el.clientHeight))/offset[1]),
            alphaMax*Clamp01((el.scrollWidth-(el.scrollLeft+el.clientWidth))/offset[0])
        ].map(v=>color
            .replace(/(?<!\\)\$f/g,v)
            .replace(/(?<!\\)\$i/g,v*0xFF)
            .replace(/(?<!\\)\$x/g,(v*0xFF).toString(0x10).padStart(2,'0'))
            .replace(/(?<!\\)\$%/g,`${v*0x64}%`)
        );
        el.style.background=`
            scroll linear-gradient(to top,    transparent, ${top})    center top    / 100% ${size[1]}      no-repeat,
            scroll linear-gradient(to left,   transparent, ${left})   center left   /      ${size[0]} 100% no-repeat,
            scroll linear-gradient(to bottom, transparent, ${bottom}) center bottom / 100% ${size[1]}      no-repeat,
            scroll linear-gradient(to right,  transparent, ${right})  center right  /      ${size[0]} 100% no-repeat
            ${background==null?'':`, ${background}`}
        `;
    };
}
/**
 * ## Convert image to base64 data URL
 * for offline viewing (asynchronous) \
 * high chance of being blocked by CORS when it's not called on the page (HTML context) where the image is displayed
 * @param {string} src - image source URL
 * @returns {Promise<string|null>} (async) data URL or `null` when image could not be loaded
 * @throws {TypeError} if {@linkcode src} is not a string
 */
async function LoadIMG(src){
    "use strict";
    if(typeof this.cnv==="undefined"){
        this.cnv=document.createElement("canvas");//~ use the same canvas object for all calls to save some resources
        this.cnx=this.cnv.getContext("2d");
        this.img=new Image();
        this.img.loading="eager";
        this.img.crossOrigin="anonymous";//~ make sure canvas does not taint and can still convert itself to data URL
    }
    if(typeof src!=="string")throw new TypeError("[LoadIMG] src is not a string");
    const load=new Promise(res=>{
        this.img.onload=()=>res(false);
        this.img.onerror=()=>res(true);
    });
    this.img.src=src;
    if(await load)return null;
    this.cnv.width=this.img.naturalWidth;
    this.cnv.height=this.img.naturalHeight;
    this.cnx.drawImage(this.img,0,0);
    return this.cnv.toDataURL();
}

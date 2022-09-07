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

//~ number
//// see https://github.com/MAZ01001/Math-Js/blob/main/functions.js

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
            (typeof Navigator)===undefined
            ||(typeof Permissions)===undefined
            ||(typeof (navigator?.permissions?.query??undefined))!=='function'
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
 * @returns {(offsetElement?:Element)=>Readonly<{page:number[];client:number[];offset:number[];screen:number[];movement:number[];}>} a function: \
 * [__param__] `offsetElement` (optional) of type `Element` - HTML element for calculating relative / offset mouse position - _default `null`_ \
 * [__returns__] a read-only object with the following attributes, stored as sealed arrays (`[X,Y]`):
 * - `page`       : the mouse position on the rendered page (actual page size >= browser window)
 * - `client`     : the mouse position on the visible portion on the rendered page (browser window)
 * - `offset`     : the mouse position on the rendered page relative to an elements position
 * - `screen`     : the mouse position on the screen (all monitors together form one continuous screen)
 * - `movement`   : the distance the mouse moved from the previous `screen` position
 *
 * [__throws__] `TypeError` if `offsetElement` is set but is not an (HTML) `Element`
 * @throws {Error} if `Window` or `Document` are not defined (not in HTML context)
 * @example
 * const mousePos = getMousePos(),
 *     log = setInterval( () => console.log( JSON.stringify( mousePos() ) ), 1000 );
 * // clearInterval( log );
 */
function getMousePos(){
    if(
        (typeof Window==='undefined')
        ||(typeof Document==='undefined')
    )throw new Error('[getMousePos] called outside the context of HTML (Window and Document are not defined)');
    if(typeof this.obj==='undefined'){
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
        document.addEventListener('mousemove',e=>{
            this.obj.page=[e.pageX,e.pageY];
            this.obj.client=[e.clientX,e.clientY];
            this.obj.screen=[e.screenX,e.screenY];
            this.obj.movement=[e.movementX,e.movementY];
        },{passive:true});
    }
    /**
     * @param {Element} offsetElement - HTML element for calculating relative / offset mouse position - _default `null`_
     * @returns {Readonly<{page:number[];client:number[];offset:number[];screen:number[];movement:number[];}>} mouse `[X,Y]` positions (sealed array)
     * - `page`       : position on the rendered page (actual page size >= browser window)
     * - `client`     : position on the visible portion on the rendered page (browser window)
     * - `offset`     : relative position on the rendered page (to an elements position)
     * - `screen`     : position on screen (from top left of all monitors)
     * - `movement`   : distance moved from previous `screen` position
     * @throws {TypeError} if `offsetElement` is set but is not an (HTML) `Element`
     */
    return(offsetElement=null)=>{
        if(offsetElement===null)this.obj.offset=[0,0];
        else{
            if(!(offsetElement instanceof Element))throw new TypeError('[getMousePos][function] offsetElement is not an (HTML) element');
            const offsetElementBCR=offsetElement.getBoundingClientRect();
            this.obj.offset[0]=this.obj.page[0]-offsetElementBCR.x;
            this.obj.offset[1]=this.obj.page[1]-offsetElementBCR.y;
        }
        return Object.freeze({...this.obj});//~ a static read-only copy of obj in its current state
    };
}

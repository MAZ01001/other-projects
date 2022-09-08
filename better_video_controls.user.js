// ==UserScript==
// @name         better video controls
// @version      0.99.2
// @description  various keyboard controls (see console after page load) for html video element (checks for `video:hover` element on every `keydown`)
// @author       MAZ / MAZ01001
// @source       https://github.com/MAZ01001/other-projects#better_video_controlsuserjs
// @updateURL    https://github.com/MAZ01001/other-projects/raw/main/better_video_controls.user.js
// @downloadURL  https://github.com/MAZ01001/other-projects/raw/main/better_video_controls.user.js
// @include      *
// @match        /^[^:/#?]*:\/\/([^#?/]*\.)?.*\..*(:[0-9]{1,5})?\/.*$/
// @match        /^file:\/\/\/.*\..*$/
// @exclude      /^[^:/#?]*:\/\/([^#?/]*\.)?youtube\.com(:[0-9]{1,5})?\/.*$/
// @run-at       document-end
// @noframes     true
// ==/UserScript==

//~ set some (local) variables
/** @type {HTMLDivElement} - the base for the `_bvc_hint_text` and `_bvc_css` element */
const _bvc_hint=document.createElement('div'),
    /** @type {HTMLStyleElement} - the element for showing the action done on keypress */
    _bvc_hint_text=document.createElement('span'),
    /** @type {HTMLStyleElement} - css style for the `_bvc_hint` element */
    _bvc_css=document.createElement('style'),
    /** @type {number[]} - current mouse x and y position on page (sealed array) */
    _bvc_mouse=Object.seal([0,0]);
/** @type {boolean} - `true` if the event listener is on and `false` if off */
let _bvc_state=false;
//~ set a name and ""some"" styling for the hint element
_bvc_hint.dataset.name="better-video-controls hint";
_bvc_css.innerText=`
    div[data-name="better-video-controls hint"]{
        position: fixed;
        transform: translate(-50%,-50%);
        border-radius: .5rem;
        border: 1px solid #333;
        background-color: #000;
        color: #0f0;
        font-family: consolas,monospace;
        font-size: large;
        text-align: center;
        width: max-content;
        padding: 2px 8px;
        /* white-space: nowrap; */
        line-break: anywhere;
        z-index: 1000000;
        opacity: 0;
        visibility: hidden;
        transition: opacity 500ms ease-in 500ms,
            visibility 0s linear 1s;
    }
    div[data-name="better-video-controls hint"].visible{
        opacity: 1;
        visibility: visible;
        transition: opacity 0s linear 0s,
            visibility 0s linear 0s;
    }
`.replaceAll(/\n\s*/g,' ');
//~ main functions
/**
 * __track mouse position on page__
 * @param {MouseEvent} ev - mouse event `mousemove`
 */
function bvc_mousemove_event_listener(ev){
    "use strict";
    _bvc_mouse[0]=ev.clientX;
    _bvc_mouse[1]=ev.clientY;
}
/**
 * __test if the mouse is over given element__
 * @param {Element} el - the element given
 * @returns {boolean} `true` if mouse is over `el` bounds, `false` otherwise
 */
function bvc_mouse_over_element(el){
    "use strict";
    const{top,bottom,left,right}=el.getBoundingClientRect();
    return _bvc_mouse[0]>=left&&_bvc_mouse[0]<=right&&
           _bvc_mouse[1]>=top&& _bvc_mouse[1]<=bottom;
}
/**
 * __keyboard controls for video element__ \
 * `keypress` eventlistener on document \
 * _(controls for video element with mouse hover ie. css:`video:hover`)_
 * @param {KeyboardEvent} ev - keyboard event `keypress`
 * @description __Keyboard controls with `{key}` of `KeyboardEvent`__
 * - `0` - `9`          → skip to ` `% of total duration (ie. key `8` skips to 80% of the video)
 * - `.`                → (while paused) next frame (1/60 sec)
 * - `,`                → (while paused) previous frame (1/60 sec)
 * - `:` (`shift` `.`)  → decrease playback speed by 10%
 * - `;` (`shift` `,`)  → increase playback speed by 10%
 * - `j` / `ArrowLeft`  → rewind 5 seconds
 * - `l` / `ArrowRight` → fast forward 5 seconds
 * - `j` (`shift` `j`)  → rewind 30 seconds
 * - `l` (`shift` `l`)  → fast forward 30 seconds
 * - `k`                → pause / play video
 * - `+` / `ArrowUp`    → increase volume by 10%
 * - `-` / `ArrowDown`  → lower volume by 10%
 * - `m`                → mute / unmute video
 * - `f`                → toggle fullscreen mode
 * - `p`                → toggle picture-in-picture mode
 * - `t`                → displays exact time and duration
 * - `u`                → displays current source url
 */
function bvc_keyboard_event_listener(ev){
    "use strict";
    /** @type {HTMLVideoElement} - html video element that has the mouse ~hovering~ over it */
    const _video_=(()=>{
        for(const vid of document.body.getElementsByTagName("video")){
            if(bvc_mouse_over_element(vid))return vid;
        }
        return null;
    })();
    if(_video_!==null){
        let text="";
        switch(ev.key){
            case'0':case'1':case'2':case'3':case'4':case'5':case'6':case'7':case'8':case'9':
                _video_.currentTime=_video_.duration*Number(ev.key)*.1;
                text=`skiped video to ${Number(ev.key)*10}%`;
            break;
            //~ _video_.requestVideoFrameCallback((...[,{processingDuration}])=>console.log(processingDuration)); //=> fps ~ varies greatly
            case'.':
                if(_video_.paused){
                    _video_.currentTime+=1/60;
                    text="next frame (skiped 1/60 sec ahead)";
                }else text="pause video for frame-by-frame";
            break;
            case',':
                if(_video_.paused){
                    _video_.currentTime-=1/60;
                    text="previous frame (skiped 1/60 sec back)";
                }else text="pause video for frame-by-frame";
            break;
            case':':
                if(_video_.playbackRate<3){
                    _video_.playbackRate=Number.parseFloat((_video_.playbackRate+0.1).toFixed(4));
                    text=`speed increased to ${_video_.playbackRate*100} %`;
                }else text=`speed already max (${_video_.playbackRate*100} %)`;
            break;
            case';':
                if(_video_.playbackRate>0.1){
                    _video_.playbackRate=Number.parseFloat((_video_.playbackRate-0.1).toFixed(4));
                    text=`speed decreased to ${_video_.playbackRate*100} %`;
                }else text=`speed already min (${_video_.playbackRate*100} %)`;
            break;
            case'j':case'ArrowLeft':
                _video_.currentTime-=5;
                text="skiped back 5 sec";
            break;
            case'l':case'ArrowRight':
                _video_.currentTime+=5;
                text="skiped ahead 5 sec";
            break;
            case'J':
                _video_.currentTime-=30;
                text="skiped back 30 sec";
            break;
            case'L':
                _video_.currentTime+=30;
                text="skiped ahead 30 sec";
            break;
            case'k':
                if(_video_.paused)_video_.play();
                else _video_.pause();
                text=`video ${_video_.paused?"paused":"resumed"}`;
            break;
            case'+':case'ArrowUp':
                if(_video_.volume<1){
                    _video_.volume=Number.parseFloat((_video_.volume+0.1).toFixed(4));
                    text=`volume increased to ${_video_.volume*100} %`;
                }else text=`volume already max (${_video_.volume*100} %)`;
                _video_.muted=_video_.volume<=0;
            break;
            case'-':case'ArrowDown':
                if(_video_.volume>0){
                    _video_.volume=Number.parseFloat((_video_.volume-0.1).toFixed(4));
                    text=`volume decreased to ${_video_.volume*100} %`;
                }else text=`volume already min (${_video_.volume*100} %)`;
                _video_.muted=_video_.volume<=0;
            break;
            case'm':
                if(_video_.muted=!_video_.muted)text="volume muted";
                else text="volume unmuted";
            break;
            case'f':
                if(document.fullscreenEnabled){
                    if(!document.fullscreenElement)_video_.requestFullscreen({navigationUI:'hide'});
                    else if(document.fullscreenElement===_video_)document.exitFullscreen();
                }else text="fullscreen not supported";
            break;
            case'p':
                if(document.pictureInPictureEnabled){
                    if(!document.pictureInPictureElement)_video_.requestPictureInPicture();
                    else if(document.pictureInPictureElement===_video_)document.exitPictureInPicture();
                }else text="picture-in-picture not supported";
            break;
            case't':
                text=`time: ${_video_.currentTime.toFixed(6)} / `;
                if(_video_.duration===Infinity)text+="live";
                else if(Number.isNaN(_video_.duration))text+="???";
                else text+=`${_video_.duration.toFixed(6)} -${(_video_.duration-_video_.currentTime).toFixed(6)}`;
                text+=" (seconds)";
            break;
            case'u':text=`url: ${_video_.currentSrc}`;break;
        }
        if(text!==""){
            _bvc_hint_text.innerText=text;
            const{top,left,height,width}=_video_.getBoundingClientRect();
            _bvc_hint.style.top=`${top+Math.floor(height*.5)}px`;
            _bvc_hint.style.left=`${left+Math.floor(width*.5)}px`;
            _bvc_hint.style.maxWidth=`${width}px`;
            bvc_hint_visible(true);
            if(!bvc_mouse_over_element(_bvc_hint))bvc_hint_visible(false);
        }
    }
}
/**
 * __set/remove visible class of `_bvc_hint` to show/hide the element__
 * @param {boolean} state - `true` to show the element and `false` to fade out
 */
function bvc_hint_visible(state){
    "use strict";
    if(state)_bvc_hint.classList.add('visible');
    else _bvc_hint.classList.remove('visible');
}
/**
 * __toggle the better video controls keyboard event listener on/off__
 * @param {?boolean} force_state if set forces the state to on on `true` and off on `false`
 * @returns {boolean} `true` if currently on and `false` if turned off
 */
function bvc_toggle_eventlistener(force_state){
    "use strict";
    if(
        (force_state===undefined||force_state===null)
        ||(Boolean(force_state)!==_bvc_state)
    ){
        if(_bvc_state=!_bvc_state){
            document.body.appendChild(_bvc_hint);
            document.addEventListener('mousemove',bvc_mousemove_event_listener,{passive:true});
            document.addEventListener('keydown',bvc_keyboard_event_listener,{passive:true});
            document.body.addEventListener('resize',()=>bvc_hint_visible(false),{passive:true});
        }else{
            document.body.removeEventListener('resize',()=>bvc_hint_visible(false),{passive:true});
            document.removeEventListener('keydown',bvc_keyboard_event_listener,{passive:true});
            document.removeEventListener('mousemove',bvc_mousemove_event_listener,{passive:true});
            document.body.removeChild(_bvc_hint);
        }
    }
    return _bvc_state;
}
//~ append elements and eventlisteners to base element (stay even when element is removed and re-appended to document body)
_bvc_hint.appendChild(_bvc_css);
_bvc_hint.appendChild(_bvc_hint_text);
_bvc_hint.addEventListener('mouseleave',()=>bvc_hint_visible(false),{passive:true});
_bvc_hint.addEventListener('mouseover',()=>bvc_hint_visible(true),{passive:true});
//~ append hint element, turn on bvc and log controls and toggle function
bvc_toggle_eventlistener(true);
console.groupCollapsed("Better Video Controls - Script via Tampermonkey by MAZ01001");
console.log(
    "%ccontrols:\n%c%s",
    "background-color:#000;color:#fff;",
    "background-color:#000;color:#0a0;font-family:consolas,monospace;",
    [
        " Keyboard (intended for QWERTZ) | Function                                                               ",
        "--------------------------------+------------------------------------------------------------------------",
        " [0] - [9]                      |  skip to []% of total duration (ie. key [8] skips to 80% of the video) ",
        " [.]                            |  (while paused) next frame (1/60 sec)                                  ",
        " [,]                            |  (while paused) previous frame (1/60 sec)                              ",
        " [:] ( [shift] [.] )            |  decrease playback speed by 10%                                        ",
        " [;] ( [shift] [,] )            |  increase playback speed by 10%                                        ",
        "--------------------------------+------------------------------------------------------------------------",
        " [j] / [ArrowLeft]              |  rewind 5 seconds                                                      ",
        " [l] / [ArrowRight]             |  fast forward 5 seconds                                                ",
        " [J] ( [shift] [j] )            |  rewind 30 seconds                                                     ",
        " [l] ( [shift] [l] )            |  fast forward 30 seconds                                               ",
        " [k]                            |  pause / play video                                                    ",
        "--------------------------------+------------------------------------------------------------------------",
        " [+] / [ArrowUp]                |  increase volume by 10%                                                ",
        " [-] / [ArrowDown]              |  lower volume by 10%                                                   ",
        " [m]                            |  mute / unmute video                                                   ",
        "--------------------------------+------------------------------------------------------------------------",
        " [f]                            |  toggle fullscreen mode                                                ",
        " [p]                            |  toggle picture-in-picture mode                                        ",
        " [t]                            |  displays exact time and duration                                      ",
        " [u]                            |  displays current source url                                           ",
    ].join('\n')
);
console.log(
    "%cfunction for on/off toggle: %O",
    "background-color:#000;color:#fff;",
    bvc_toggle_eventlistener,
);
console.log(
    "%cRight-click on the above function and select \"%cStore function as global variable%c\".\nThen you can call it with that variable like %ctemp1();",
    "background-color:#000;color:#fff;",
    "background-color:#000;color:#0a0;",
    "background-color:#000;color:#fff;",
    "background-color:#000;color:#0a0;font-family:consolas,monospace;"
);
console.groupEnd();

// ==UserScript==
// @name         better video controls
// @version      0.99.81
// @description  various keyboard controls for html video elements, see console after page loads for keyboard shortcuts (uses the last video element that was moused over).
// @author       MAZ / MAZ01001
// @source       https://github.com/MAZ01001/other-projects#better_video_controlsuserjs
// @updateURL    https://github.com/MAZ01001/other-projects/raw/main/better_video_controls.user.js
// @downloadURL  https://github.com/MAZ01001/other-projects/raw/main/better_video_controls.user.js
// @include      *
// @match        /^[^:/#?]*:\/\/([^#?/]*\.)?.*\..*(:[0-9]{1,5})?\/.*$/
// @match        /^file:\/\/\/.*\..*$/
// @exclude      /^[^:/#?]*:\/\/([^#?/]*\.)?youtube\.com(:[0-9]{1,5})?\/.*$/
// @run-at       document-start
// @noframes     false
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
/** @type {boolean} - if `false` ignores video controls and does not call `preventDefault` and `stopImmediatePropagation` for keypressed on video elements */
let _bvc_state=true,
    /** @type {HTMLVideoElement|null} - the last video element that the mouse was over */
    _bvc_last_video=null;
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
//~ append elements and eventlisteners to base element (stay even when element is removed and re-appended to document body)
_bvc_hint.appendChild(_bvc_css);
_bvc_hint.appendChild(_bvc_hint_text);
_bvc_hint.addEventListener('mouseleave',()=>bvc_hint_visible(false),{passive:true});
_bvc_hint.addEventListener('mouseover',()=>bvc_hint_visible(true),{passive:true});
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
 * __set `_bvc_last_video` to video hovering or `null` when clicking some where else__
 * @param {MouseEvent?} ev - mouse event `click` - _unused_
 */
function bvc_click_event(ev=null){
    "use strict";
    for(const vid of document.body.getElementsByTagName("video")){
        if(bvc_mouse_over_element(vid)){
            _bvc_last_video=vid;
            return;
        }
    }
    _bvc_last_video=null;
}
/**
 * __test if the mouse is over given element__
 * @param {Element} el - the element given
 * @returns {boolean} `true` if mouse is over `el` bounds, `false` otherwise
 */
function bvc_mouse_over_element(el){
    "use strict";
    const{top,bottom,left,right}=el.getBoundingClientRect();
    return _bvc_mouse[0]>=left
        && _bvc_mouse[0]<=right
        && _bvc_mouse[1]>=top
        && _bvc_mouse[1]<=bottom;
}
/**
 * __keyboard controls for video element__ \
 * `keypress` eventlistener on document \
 * _(controls for last hovered video element)_
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
 * - `r`                → toggle loop mode
 * - `f`                → toggle fullscreen mode
 * - `p`                → toggle picture-in-picture mode
 * - `t`                → displays exact time and duration
 * - `u`                → displays current source url
 *
 * __NOTE__ calls `preventDefault` and `stopImmediatePropagation`
 */
function bvc_keyboard_event_listener(ev){
    "use strict";
    if(_bvc_last_video==null)return;
    if(!_bvc_state)return;
    let text="";
    switch(ev.key){
        case'0':case'1':case'2':case'3':case'4':case'5':case'6':case'7':case'8':case'9':
            _bvc_last_video.currentTime=_bvc_last_video.duration*Number(ev.key)*.1;
            text=`skiped video to ${Number(ev.key)*10}%`;
        break;
        //~ _bvc_last_video.requestVideoFrameCallback((...[,{processingDuration}])=>console.log(processingDuration)); //=> fps ~ varies greatly
        case'.':
            if(!_bvc_last_video.paused)_bvc_last_video.pause();
            _bvc_last_video.currentTime+=0.0166666666666666666; //~ 1/60
            text=`next frame (if 60 fps) to ${_bvc_last_video.currentTime.toFixed(6)}`;
        break;
        case',':
            if(!_bvc_last_video.paused)_bvc_last_video.pause();
            _bvc_last_video.currentTime-=0.0166666666666666666; //~ 1/60
            text=`previous frame (if 60 fps) to ${_bvc_last_video.currentTime.toFixed(6)}`;
        break;
        case':':
            if(_bvc_last_video.playbackRate<4){
                _bvc_last_video.playbackRate=Number.parseFloat((_bvc_last_video.playbackRate+0.1).toFixed(4));
                text=`speed increased to ${Math.floor(_bvc_last_video.playbackRate*100)} %`;
            }else{
                _bvc_last_video.playbackRate=4
                text="speed already max (400 %)";
            }
        break;
        case';':
            if(_bvc_last_video.playbackRate>0.1){
                _bvc_last_video.playbackRate=Number.parseFloat((_bvc_last_video.playbackRate-0.1).toFixed(4));
                text=`speed decreased to ${Math.floor(_bvc_last_video.playbackRate*100)} %`;
            }else{
                _bvc_last_video.playbackRate=0.1
                text="speed already min (10 %)";
            }
        break;
        case'j':case"ArrowLeft":
            _bvc_last_video.currentTime-=5;
            text=`skiped back 5 sec to ${_bvc_last_video.currentTime.toFixed(6)}`;
        break;
        case'l':case"ArrowRight":
            _bvc_last_video.currentTime+=5;
            text=`skiped ahead 5 sec to ${_bvc_last_video.currentTime.toFixed(6)}`;
        break;
        case'J':
            _bvc_last_video.currentTime-=30;
            text=`skiped back 30 sec to ${_bvc_last_video.currentTime.toFixed(6)}`;
        break;
        case'L':
            _bvc_last_video.currentTime+=30;
            text=`skiped ahead 30 sec to ${_bvc_last_video.currentTime.toFixed(6)}`;
        break;
        case'k':
            if(_bvc_last_video.paused)_bvc_last_video.play();
            else _bvc_last_video.pause();
            text=`video ${_bvc_last_video.paused?"paused":"resumed"} at ${_bvc_last_video.currentTime.toFixed(6)}`;
        break;
        case'+':case"ArrowUp":
            _bvc_last_video.volume=(vol=>vol>1?1:vol)(Number.parseFloat((_bvc_last_video.volume+0.1).toFixed(4)));
            text=`volume increased to ${Math.floor(_bvc_last_video.volume*100)} %`;
            _bvc_last_video.muted=_bvc_last_video.volume<=0;
        break;
        case'-':case"ArrowDown":
            _bvc_last_video.volume=(vol=>vol<0?0:vol)(Number.parseFloat((_bvc_last_video.volume-0.1).toFixed(4)));
            text=`volume decreased to ${Math.floor(_bvc_last_video.volume*100)} %`;
            _bvc_last_video.muted=_bvc_last_video.volume<=0;
        break;
        case'm':text=`volume ${(_bvc_last_video.muted=!_bvc_last_video.muted)?"muted":"unmuted"}`;break;
        case'r':text=(_bvc_last_video.loop=!_bvc_last_video.loop)?"looping":"not looping";break;
        case'f':
            if(document.fullscreenEnabled){
                if(!document.fullscreenElement)_bvc_last_video.requestFullscreen({navigationUI:'hide'});
                else if(document.fullscreenElement===_bvc_last_video)document.exitFullscreen();
            }else text="fullscreen not supported";
        break;
        case'p':
            if(document.pictureInPictureEnabled){
                if(!document.pictureInPictureElement)_bvc_last_video.requestPictureInPicture();
                else if(document.pictureInPictureElement===_bvc_last_video)document.exitPictureInPicture();
            }else text="picture-in-picture not supported";
        break;
        case't':
            text=`time: ${_bvc_last_video.currentTime.toFixed(6)} / `;
            if(_bvc_last_video.duration===Infinity)text+="live";
            else if(Number.isNaN(_bvc_last_video.duration))text+="???";
            else text+=`${_bvc_last_video.duration.toFixed(6)} -${(_bvc_last_video.duration-_bvc_last_video.currentTime).toFixed(6)}`;
            text+=" (seconds)";
        break;
        case'u':text=`url: ${_bvc_last_video.currentSrc}`;break;
    }
    if(text===""){
        if(ev.key==="Control")bvc_click_event();
        return;
    }
    ev.preventDefault();
    ev.stopImmediatePropagation();
    _bvc_hint_text.innerText=text;
    const{top,left,height,width}=_bvc_last_video.getBoundingClientRect();
    _bvc_hint.style.top=`${top+Math.floor(height*.5)}px`;
    _bvc_hint.style.left=`${left+Math.floor(width*.5)}px`;
    _bvc_hint.style.maxWidth=`${width}px`;
    bvc_hint_visible(true);
    if(!bvc_mouse_over_element(_bvc_hint))bvc_hint_visible(false);
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
 * __toggle the better video controls on/off__
 * @param {?boolean} force_state if set forces the state to on on `true` and off on `false`
 * @returns {boolean} `true` if currently on and `false` if turned off
 */
function bvc_toggle_controls(force_state){
    "use strict";
    //~ `== null` to check for `=== undefined || === null`
    if(
        force_state==null
        ||(Boolean(force_state)!==_bvc_state)
    )_bvc_state=!_bvc_state;
    return _bvc_state;
}
//~ append hint element, turn on bvc, and log controls, toggle function, and credits as a collapsed group
document.addEventListener('keydown',bvc_keyboard_event_listener,{passive:false});
document.addEventListener('mousemove',bvc_mousemove_event_listener,{passive:true});
document.addEventListener('click',bvc_click_event,{passive:true});
window.addEventListener('load',()=>{
    document.body.appendChild(_bvc_hint);
    document.body.addEventListener('resize',()=>bvc_hint_visible(false),{passive:true});
},{passive:true,once:true});
console.groupCollapsed("Better Video Controls - Script via Tampermonkey by MAZ01001");
    console.info(
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
            " [r]                            |  toggle loop mode                                                      ",
            " [f]                            |  toggle fullscreen mode                                                ",
            " [p]                            |  toggle picture-in-picture mode                                        ",
            "--------------------------------+------------------------------------------------------------------------",
            " [t]                            |  displays exact time and duration                                      ",
            " [u]                            |  displays current source url                                           ",
        ].join('\n')
    );
    console.info(
        "%cfunction for on/off toggle: %O",
        "background-color:#000;color:#fff;",
        bvc_toggle_controls,
    );
    console.info(
        "%cRight-click on the above function and select \"%cStore function as global variable%c\".\nThen you can call it with that variable like %ctemp1();",
        "background-color:#000;color:#fff;",
        "background-color:#000;color:#0a0;",
        "background-color:#000;color:#fff;",
        "background-color:#000;color:#0a0;font-family:consolas,monospace;"
    );
    console.info("Credits: MAZ https://maz01001.github.io/ \nDocumentation: https://github.com/MAZ01001/other-projects#better_video_controlsuserjs \nSource code: https://github.com/MAZ01001/other-projects/blob/main/better_video_controls.user.js");
console.groupEnd();

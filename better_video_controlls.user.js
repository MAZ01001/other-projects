// ==UserScript==
// @name         better video controls
// @version      0.98.2
// @description  various keyboard controls (see console after page load) for html video element (checks for `video:hover` element on every `keydown`)
// @author       MAZ / MAZ01001
// @source       https://github.com/MAZ01001/other-projects#better_video_controlsuserjs
// @updateURL    https://github.com/MAZ01001/other-projects/blob/main/better_video_controls.user.js
// @downloadURL  https://github.com/MAZ01001/other-projects/blob/main/better_video_controls.user.js
// @include      *
// @match        /^[^:/#?]*:\/\/([^#?/]*\.)?.*\..*(:[0-9]{1,5})?\/.*$/
// @match        /^file:\/\/\/.*\..*$/
// @exclude      /^[^:/#?]*:\/\/([^#?/]*\.)?youtube\.com(:[0-9]{1,5})?\/.*$/
// @run-at       document-end
// @noframes     true
// ==/UserScript==

//~ set some (local) variables
/** @type {HTMLDivElement} - the `_bvc_hint` element for showing the action done on keypress */
const _bvc_hint=document.createElement('div');
/** @type {number|null} - timeout for visibility of `_bvc_hint` */
let _bvc_hint_timeout=null,
    /** @type {boolean} - `true` if the event listener is on and `false` if off */
    _bvc_state=false;
//~ set a name and some styling for the hint element
_bvc_hint.dataset.name="better-video-controls hint";
_bvc_hint.style.position="fixed";
_bvc_hint.style.visibility="none";
_bvc_hint.style.borderRadius=".5rem";
_bvc_hint.style.backgroundColor="#000";
_bvc_hint.style.color="#0f0";
_bvc_hint.style.fontSize="x-large";
_bvc_hint.style.pointerEvents="none";
//~ main functions
/**
 * __keyboard controls for video element__ \
 * `keypress` eventlistener on document \
 * _(controls for video element with mouse hover ie. css:`video:hover`)_
 * @param {KeyboardEvent} ev - keyboard event `keypress`
 * @description __Keyboard controls with `{key}` of `KeyboardEvent`__
 * - `0` - `9`          → skip to ` `% of total duration (ie. key `8` skips to 80% of the video)
 * - `.`                → (while paused) next frame (1/60 sec)
 * - `,`                → (while paused) previous frame (1/60 sec)
 * - `:` (`shift` `.`)  → decrease playback speed by 0.1
 * - `;` (`shift` `,`)  → increase playback speed by 0.1
 * - `M` (`shift` `m`)  → reset playback speed
 * - `j` / `ArrowLeft`  → rewind 10 seconds
 * - `l` / `ArrowRight` → fast forward 10 seconds
 * - `k`                → pause / play video
 * - `+` / `ArrowUp`    → increase volume by 10%
 * - `-` / `ArrowDown`  → lower volume by 10%
 * - `m`                → mute / unmute video
 * - `f`                → toggle fullscreen mode
 * - `p`                → toggle picture-in-picture mode
 * - `t`                → displays exact time and duration
 */
function bvc_keyboard_event_listener(ev){
    'use strict';
    /** @type {HTMLVideoElement} - html video element that has the mouse hovering over it */
    const _video_=document.body.querySelector('video:hover');
    if(_video_){
        let changed=true;
        switch(ev.key){
            case'0':case'1':case'2':case'3':case'4':case'5':case'6':case'7':case'8':case'9':
                _video_.currentTime=_video_.duration*Number(ev.key)*.1;
                _bvc_hint.innerText=`skiped video to ${Number(ev.key)*10}%`;
            break;
            //~ _video_.requestVideoFrameCallback((...[,{processingDuration}])=>console.log(processingDuration)); //=> fps ~ varies greatly
            case'.':
                if(_video_.paused){
                    _video_.currentTime+=1/60;
                    _bvc_hint.innerText="next frame (skiped 1/60 sec ahead)";
                }else{changed=false;}
            break;
            case',':
                if(_video_.paused){
                    _video_.currentTime-=1/60;
                    _bvc_hint.innerText="previous frame (skiped 1/60 sec back)";
                }else{changed=false;}
            break;
            case':':
                if(_video_.playbackRate<3){
                    _video_.playbackRate=Number.parseFloat((_video_.playbackRate+0.1).toFixed(4));
                    _bvc_hint.innerText=`speed increased to ${_video_.playbackRate}`;
                }else{changed=false;}
            break;
            case';':
                if(_video_.playbackRate>0.1){
                    _video_.playbackRate=Number.parseFloat((_video_.playbackRate-0.1).toFixed(4));
                    _bvc_hint.innerText=`speed decreased to ${_video_.playbackRate}`;
                }else{changed=false;}
            break;
            case'M':
                _video_.playbackRate=_video_.defaultPlaybackRate;
                _bvc_hint.innerText=`reset speed to ${_video_.playbackRate}`;
            break;
            case'j':case'ArrowLeft':
                _video_.currentTime-=5;
                _bvc_hint.innerText="skiped back 5 sec";
            break;
            case'l':case'ArrowRight':
                _video_.currentTime+=5;
                _bvc_hint.innerText="skiped ahead 5 sec";
            break;
            case'k':
                if(_video_.paused){_video_.play();}
                else{_video_.pause();}
                _bvc_hint.innerText=`video ${_video_.paused?"paused":"resumed"}`;
            break;
            case'+':case'ArrowUp':
                if(_video_.volume<1){
                    _video_.volume=Number.parseFloat((_video_.volume+0.1).toFixed(4));
                    _bvc_hint.innerText=`volume increased to ${_video_.volume}`;
                }else{changed=false;}
            break;
            case'-':case'ArrowDown':
                if(_video_.volume>0){
                    _video_.volume=Number.parseFloat((_video_.volume-0.1).toFixed(4));
                    _bvc_hint.innerText=`volume decreased to ${_video_.volume}`;
                }else{changed=false;}
            break;
            case'm':
                if(_video_.volume>0){
                    _video_.volume=0;
                    _bvc_hint.innerText="volume muted";
                }else{
                    _video_.volume=1;
                    _bvc_hint.innerText="volume unmuted";
                }
            break;
            case'f':
                if(document.fullscreenEnabled){
                    if(!document.fullscreenElement){_video_.requestFullscreen({navigationUI:'hide'});}
                    else if(document.fullscreenElement===_video_){document.exitFullscreen();}
                }else{changed=false;}
            break;
            case'p':
                if(document.pictureInPictureEnabled){
                    if(!document.pictureInPictureElement){_video_.requestPictureInPicture();}
                    else if(document.pictureInPictureElement===_video_){document.exitPictureInPicture();}
                }else{changed=false;}
            break;
            case't':_bvc_hint.innerText=`time: ${_video_.currentTime} / ${_video_.duration} seconds`;break;
            default:changed=false;break;
        }
        if(changed){
            if(_bvc_hint_timeout!==null){clearTimeout(_bvc_hint_timeout);}
            const _bb_video_=_video_.getBoundingClientRect();
            _bvc_hint.style.top=`${_bb_video_.top+Math.floor(_bb_video_.height*.5)}px`;
            _bvc_hint.style.left=`${_bb_video_.left+Math.floor(_bb_video_.width*.5)}px`;
            _bvc_hint.style.visibility="visible";
            _bvc_hint_timeout=setTimeout(()=>{
                _bvc_hint.style.visibility="hidden";
                _bvc_hint_timeout=null;
            },2000);
        }
    }
}
/**
 * __toggle the better video controls keyboard event listener on/off__
 * @param {?boolean} force_state if set forces the state to on on `true` and off on `false`
 * @returns {boolean} `true` if currently on and `false` if turned off
 */
function bvc_toggle_eventlistener(force_state){
    'use strict';
    if(
        (force_state===undefined||force_state===null)
        ||(Boolean(force_state)!==_bvc_state)
    ){
        if(_bvc_state){document.removeEventListener('keydown',bvc_keyboard_event_listener,{passive:true});}
        else{document.addEventListener('keydown',bvc_keyboard_event_listener,{passive:true});}
        _bvc_state=!_bvc_state;
    }
    return _bvc_state;
}
//~ append hint element, turn on bvc and log controls and toggle function
document.body.appendChild(_bvc_hint);
bvc_toggle_eventlistener(true);
console.log(
    "%cbetter-video-controls loaded\n%ccontrols:\n%c%s\n%cfunction for toggle on/off: %O",
    "background-color:#000;color:#0f0;font-size:larger;",
    "background-color:#000;color:#fff;",
    "background-color:#000;color:#0a0;font-family:consolas,monospace;",
    [
        "   `0` - `9`          → skip to ` `% of total duration (ie. key `8` skips to 80% of the video) ",
        "   `.`                → (while paused) next frame (1/60 sec)                                   ",
        "   `,`                → (while paused) previous frame (1/60 sec)                               ",
        "   `:` (`shift` `.`)  → decrease playback speed by 0.1                                         ",
        "   `;` (`shift` `,`)  → increase playback speed by 0.1                                         ",
        "   `M` (`shift` `m`)  → reset playback speed                                                   ",
        "   `j` / `ArrowLeft`  → rewind 10 seconds                                                      ",
        "   `l` / `ArrowRight` → fast forward 10 seconds                                                ",
        "   `k`                → pause / play video                                                     ",
        "   `+` / `ArrowUp`    → increase volume by 10%                                                 ",
        "   `-` / `ArrowDown`  → lower volume by 10%                                                    ",
        "   `m`                → mute / unmute video                                                    ",
        "   `f`                → toggle fullscreen mode                                                 ",
        "   `p`                → toggle picture-in-picture mode                                         ",
        "   `t`                → displays exact time and duration                                       ",
    ].join('\n'),
    "background-color:#000;color:#fff;",
    bvc_toggle_eventlistener,
);

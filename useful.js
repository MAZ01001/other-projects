/* some useful js functions */

/**
 * __inserts a string at a specific index__
 * @param {string} str - initial string
 * @param {number} i - zero-based index of `str`
 * @param {string} r - string to be inserted in `str` at `i`
 * @param {number} d - delete count of characters in `str` at `i`
 * @returns {string} modified string
 * @throws {TypeError} if `i` or `d` are not whole numbers
 * @example _string_insert('Hello#World!',-6,', ',-1);//=> 'Hello, World!'
 */
function _string_insert(str,i=0,r='',d=0){
    str=String(str);
    i=Math.abs(Number(i));
    if(!Number.isInteger(i)){throw new TypeError('[i] is not a whole number');}
    r=String(r);
    d=Number(d);
    if(!Number.isInteger(d)){throw new TypeError('[d] is not a whole number');}
    if(i<0){i=str.length+i;}
    if(d<0){return str.substring(0,i+d)+r+str.substring(i);}
    return str.substring(0,i)+r+str.substring(i+d);
}
/**
 * __calculates new bounds/scale for given number `n`__ \
 * _(number can be out of bounds)_
 * @param {number} n initial number
 * @param {number} x initial lower bound
 * @param {number} y initial upper bound
 * @param {number} x2 new lower bound
 * @param {number} y2 new upper bound
 * @param {boolean} limit if `true` limits output to min `x2` and max `y2` - _default `false`_
 * @returns {number} calculated number
 * @throws {TypeError} if `n`, `x`, `y`, `x2` or `y2` are not numbers
 * @description copy of [P5.js map function](https://github.com/processing/p5.js/blob/main/src/math/calculation.js#:~:text=p5.prototype.map)
 * @example _number_mapRange(0.5,0,1,0,100);//=> 50
 */
function _number_mapRange(n,x,y,x2,y2,limit=false){
    n=Number(n);
    if(Number.isNaN(n)){throw new TypeError('[n] is not a number.');}
    x=Number(x);
    if(Number.isNaN(x)){throw new TypeError('[x] is not a number.');}
    y=Number(y);
    if(Number.isNaN(y)){throw new TypeError('[y] is not a number.');}
    x2=Number(x2);
    if(Number.isNaN(x2)){throw new TypeError('[x2] is not a number.');}
    y2=Number(y2);
    if(Number.isNaN(y2)){throw new TypeError('[y2] is not a number.');}
    limit=!!limit;
    let o=((n-x)/(y-x))*(y2-x2)+x2;
    return limit?(
        x2<y2?
        Math.max(Math.min(o,y2),x2):
        Math.max(Math.min(o,x2),y2)
    ):o;
}
/**
 * __rounds given number to given decimal place__
 * @param {number} n - initial number
 * @param {number} dec - number of decimal places to round to - _default `0`_
 * @returns {number} rounded number
 * @throws {TypeError} if `n` is not a number or `dec` is not a whole finite number
 * @description copy of [P5.js round function](https://github.com/processing/p5.js/blob/main/src/math/calculation.js#:~:text=p5.prototype.round)
 */
function _number_roundDecimal(n,dec=0){
    n=Number(n);
    if(Number.isNaN(n)){throw new TypeError('[n] is not a number.');}
    dec=Math.abs(Number(dec));
    if(!Number.isFinite(dec)||!Number.isInteger(dec)){throw new TypeError('[dec] is not a whole finite number.');}
    return Number(Math.round(n+'e'+dec)+'e-'+dec);
}
/**
 * __calculates percentage of a number within bounds__
 * @param {number} n initial number
 * @param {number} x lower bound
 * @param {number} y upper bound
 * @returns {number} percent as decimal number between 0 and 1
 * @throws {TypeError} if `n`, `x` or `y` are not numbers
 * @example _number_toPercent(150,100,200);//=> 0.5
 * @description _same as `_number_mapRange(n,x,y,0,1);`
 */
function _number_toPercent(n,x,y){
    n=Number(n);
    if(Number.isNaN(n)){throw new TypeError('[n] is not a number.');}
    x=Number(x);
    if(Number.isNaN(x)){throw new TypeError('[x] is not a number.');}
    y=Number(y);
    if(Number.isNaN(y)){throw new TypeError('[y] is not a number.');}
    if(y>x){[x,y]=[y,x];}
    return (n-x)/(y-x);
}
/**
 * __converts angle from DEG to RAD__
 * @param {number} deg - angle in degrees
 * @returns {number} angle in radians
 * @throws {TypeError} if `deg` is not a number
 */
function _number_deg2rad(deg){
    deg=Number(deg);
    if(Number.isNaN(deg)){throw new TypeError('[deg] is not a number.');}
    return deg*(180/Math.PI);
}
/**
 * __converts angle from RAD to DEG__
 * @param {number} rad - angle in radians
 * @returns {number} angle in degrees
 * @throws {TypeError} if `rad` is not a number
 */
function _number_rad2deg(rad){
    rad=Number(rad);
    if(Number.isNaN(rad)){throw new TypeError('[rad] is not a number.');}
    return rad*(Math.PI/180);
}
/**
 * __computes the greatest-common-divisor of two whole numbers__
 * @param {number} A integer
 * @param {number} B integer
 * @returns {number} greatest-common-divisor (integer)
 * @throws {TypeError} if `A` or `B` are not whole numbers
 * @example _number_gcd(45,100);//=> 5 | (/5)>> 45/100 == 9/20
 * @description used to shorten fractions (see example)
 */
function _number_gcd(A,B){
    A=Number(A);
    if(Number.isInteger(A)){throw new TypeError('[A] is not a whole number.');}
    B=Number(B);
    if(Number.isInteger(B)){throw new TypeError('[B] is not a whole number.');}
    for([A,B]=A<B?[B,A]:[A,B];A%B>0;[A,B]=[B,A%B]);
    return B;
}
/**
 * __converts a decimal number to an improper-fraction (rough estimation)__
 * @param {number} dec - decimal number
 * @param {number} loop_last - if `>0` repeat the last `loop_last` decimal numbers of `dec` - _default `0`_
 * @param {number} max_den - max number for denominator - _default `0` (no limit)_
 * @param {number} max_iter - max iteration count - _default `1e6`_
 * @returns {{a:number,b:number,c:number,n:number,s:string}}
 * + a : whole number part
 * + b : numerator
 * + c : denominator
 * + n : iteration count
 * + s : reason of exit (`'precision'`|`'infinity'`|`'max_den'`|`'max_iter'`)
 * + `dec = a + ( b / c )`
 * @throws {TypeError} if `dec` is not a finite number or `loop_last`, `max_den` or `max_iter` are not whole numbers
 * @example _number_dectofrac(.12,2);//=> a:0 b:4 c:33 = 4/33 = .121212121212...
 */
function _number_dec2frac(dec,loop_last=0,max_den=0,max_iter=1e6){
    dec=Number(dec);
    if(!Number.isFinite(dec)){throw new TypeError('[dec] is not a finite number.');}
    if(Number.isInteger(dec)){return{a:dec,b:0,c:1,n:0,s:'precision'};}
    loop_last=Math.abs(Number(loop_last));
    if(!Number.isInteger(loop_last)){throw new TypeError('[loop_last] is not a whole number.');}
    max_den=Math.abs(Number(max_den));
    if(!Number.isInteger(max_den)){throw new TypeError('[max_den] is not a whole number.');}
    max_iter=Math.abs(Number(max_iter));
    if(!Number.isInteger(max_iter)){throw new TypeError('[max_iter] is not a whole number.');}
    let sign=(dec<0?-1:1),
        nint,ndec=Math.abs(dec),
        num,pnum=1,ppnum=0,
        den,pden=0,ppden=1,
        iter=0;
    /**
     * __shorten and return fraction__
     * @param {number} si sign
     * @param {number} W whole part
     * @param {number} N nominator
     * @param {number} D denominator
     * @param {number} I iteration
     * @param {string} S string
     * @returns {a:number;b:number;c:number;n:number;s:string} fraction
     */
    const __end=(si,W,N,D,I,S)=>{
        if(N===D){return{a:si*(W+1),b:0,c:1,n:I,s:S};}
        if(N>D){
            const _t=(N/D);
            if(_t===Math.floor(_t)){return{a:si*(W+_t),b:0,c:1,n:I,s:S};}
            W+=Math.floor(_t);
            N-=Math.floor(_t)*D;
        }
        const _gcd=((A,B)=>{for([A,B]=A<B?[B,A]:[A,B];A%B>0;[A,B]=[B,A%B]);return B;})(N,D);
        N/=_gcd;
        D/=_gcd;
        return{a:si*W,b:N,c:D,n:I,s:S};
    };
    if(loop_last>0&&!/e/.test(ndec.toString())){
        if(max_den===0){
            [,nint,ndec]=[...ndec.toString().match(/^([0-9]+)\.([0-9]+)$/)];
            nint=parseInt(nint);
            if(loop_last>ndec.length){loop_last=ndec.length;}
            const _l=10**(ndec.length-loop_last),
                _r=parseInt(''.padEnd(loop_last,'9')+''.padEnd(ndec.length-loop_last,'0'));
            num=(Number(ndec.slice(-loop_last))*_l)
                +(Number(ndec.slice(0,-loop_last))*_r);
            den=_l*_r;
            if(!Number.isFinite(nint+(num/den))){return __end(sign,nint,num,den,iter,'infinity');}
            return __end(sign,nint,num,den,iter,'precision');
        }
        const _l=dec.toString().match(/^[0-9]+\.([0-9]+)$/)[1];
        if(loop_last>_l.length){loop_last=_l.length;}
        dec=Number(dec.toString()+''.padEnd(22,_l.substr(-loop_last)));
        ndec=Math.abs(dec);
    }
    do{
        nint=Math.floor(ndec);
        num=ppnum+nint*pnum;
        den=ppden+nint*pden;
        if(max_den>0&&(ppden+(nint*pden))>max_den){return __end(sign,0,pnum,pden,--iter,'max_den');}
        if(!isFinite(ppnum+(nint*pnum))){return __end(sign,0,num,den,iter,'infinity');}
        // console.log(
        //     "<[%d]>\n%s\n%s\n%s\n%s",
        //     iter,
        //     ` ${((sign>0?'+':'-')+num).padEnd(21,' ')} ${ppnum} + ${nint} * ${pnum} * ${sign}`,
        //     `  ${den.toString().padEnd(20,' ')} ${ppden} + ${nint} * ${pden}`,
        //     ` =${sign*(num/den)}`,
        //     ` (${dec})`
        // );
        ppnum=pnum;
        ppden=pden;
        pnum=num;
        pden=den;
        ndec=1.0/(ndec-nint);
        if(Number.EPSILON>Math.abs((sign*(num/den))-dec)){return __end(sign,0,num,den,iter,'precision');}
    }while(iter++<max_iter);
    return __end(sign,0,num,den,iter,'max_iter');
}
/**
 * __convert number to string with padding__
 * @param {number} n - number
 * @param {number} first - padding to length before decimal point
 * @param {number} last - padding to length after decimal point
 * @returns {string} padded number as string
 * @throws {TypeError} if `n` is not a number or `first` or `last` are not whole numbers
 * @example _number_padNum(1.23,3,5);//=> '+  1.23000'
 * @description
 * format:`[sign] [padded start ' '] [.] [padded end '0'] [e ~]`
*/
function _number_padNum(n,first=0,last=0){
    n=Number(n);
    if(Number.isNaN(n)){throw new TypeError('[n] is not a number.')}
    first=Math.abs(Number(first));
    if(Number.isNaN(first)){throw new TypeError('[first] is not a whole number.');}
    last=Math.abs(Number(last));
    if(Number.isNaN(last)){throw new TypeError('[last] is not a whole number.');}
    if(/[eE]/.test(n.toString())){
        let [,s,i,d,x]=[...n.toString().match(/^([+-]?)([0-9]+)(?:\.([0-9]+))?([eE][+-]?[0-9]+)$/)];
        if(!d){d='0';}
        if(!s){s='+';}
        return s+i.padStart(first,' ')+'.'+d.padEnd(last,'0')+x;
    }
    let [,s,i,d]=[...n.toString().match(/^([+-]?)([0-9]+)(?:\.([0-9]+))?$/)];
    if(!d){d='0';}
    if(!s){s='+';}
    return s+i.padStart(first,' ')+'.'+d.padEnd(last,'0');
}

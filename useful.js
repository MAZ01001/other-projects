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
    str=String(str);
    i=Number(i);
    if(!Number.isInteger(i)){throw new TypeError('[i] is not a whole number.');}
    r=String(r);
    d=Number(d);
    if(!Number.isInteger(d)){throw new TypeError('[d] is not a whole number.');}
    if(i<0){i=str.length+i;}
    return str.substring(0,i)+r+str.substring(i+d);
}
/**
 * __object of how much each character appears in the string__\
 * _or for only the given characters_
 * @param {string} str - the string for analysis
 * @param {string} chars - if given, searches only the amount for these characters - _default `''` = all_
 * @returns {{string:number;string:number;string:number}} object with amount of apperance (`'a':8,'b':2,...`)
 * @example strCharStats('abzaacdd','abce');
 * // {
 * //   'a':3,
 * //   'b':1,
 * //   'c':1,
 * //   'e':0,
 * //   'other':3
 * // }
 */
function strCharStats(str,chars=''){
    ////getUnique=> str.split('').sort().join('').replace(/([\s\S])\1+/,'$1').replace(/(([\s\S])\2*)/g,(m,a,z)=>`${z} - ${a.length}\n`);
    str=String(str);
    chars=String(chars);
    let obj={};
    if(chars===''){for(const char of str){obj[char]=(obj[char]+1)||1;}}
    else{
        for(const char of chars){obj[char]=0;}
        for(const char of str){
            if(chars.includes(char)){obj[char]++;}
            else{obj['other']=(obj['other']+1)||1;}
        }
    }
    return obj;
}

//~ number
//// see https://github.com/MAZ01001/Math-Js/blob/main/functions.js


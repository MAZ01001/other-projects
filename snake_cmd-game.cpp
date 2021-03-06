#include <windows.h>
#include <iostream>
#include <stdlib.h>
#include <conio.h>
bool gameOver;
const int width=30,height=30;
const int widthField=((width*2)+1);
const int maxSizeField=((widthField+3)*(height+2)),
    maxSizeSnake=(width*height);
bool _debugInfo=false,portalWalls=false;
double delayTime,delayTimeOg;
int x,y,
    fruitX,fruitY,
    score,scoreStep,
    fieldCount,nTail,
    tailX[maxSizeSnake],
    tailY[maxSizeSnake];
char field[maxSizeField],
    tailC[maxSizeSnake];
enum eDirection{STOP=0,LEFT,UP,RIGHT,DOWN};
eDirection dir,pdir;
bool screenClear(bool full){//~ https://www.cplusplus.com/articles/4z18T05o/#Windows
    HANDLE hStdOut;
    CONSOLE_SCREEN_BUFFER_INFO csbi;
    DWORD count,cellCount;
    COORD homeCoords={0,0};
    hStdOut=GetStdHandle(STD_OUTPUT_HANDLE);
    if(hStdOut==INVALID_HANDLE_VALUE){return false;}
    if(full){
        //~ Get the number of cells in the current buffer
        if(!GetConsoleScreenBufferInfo(hStdOut,&csbi)){return false;}
        cellCount=csbi.dwSize.X*csbi.dwSize.Y;
        //~ Fill the entire buffer with spaces
        if(!FillConsoleOutputCharacter(
            hStdOut,
            (TCHAR)' ',
            cellCount,
            homeCoords,
            &count
        )){return false;}
        //~ Fill the entire buffer with the current colors and attributes
        if(!FillConsoleOutputAttribute(
            hStdOut,
            csbi.wAttributes,
            cellCount,
            homeCoords,
            &count
        )){return false;}
    }
    //~ Move the cursor home
    SetConsoleCursorPosition(hStdOut,homeCoords);
    return true;
}
void Setup(){
    gameOver=false;
    dir=STOP;
    x=width*.5;
    y=height*.5;
    fruitX=rand()%width;
    fruitY=rand()%height;
    score=0;
    scoreStep=0;
    fieldCount=0;
    field[fieldCount]='\0';
    tailX[0]=x;
    tailY[0]=y;
    nTail=0;
    delayTime=delayTimeOg;
    if(!screenClear(true)){exit(EXIT_FAILURE);}
}
void Draw(){
    fieldCount=0;
    field[fieldCount++]='+';
    for(int i=0;i<widthField;i++){field[fieldCount++]='-';}//~ border top
    field[fieldCount++]='+';
    field[fieldCount++]='\n';
    for(int i=0;i<height;i++){//~ draw map row
        field[fieldCount++]='|';//~ border left
        if(_debugInfo){//~ draw map col
            if(i==y){for(int j=0;j<widthField;j++){field[fieldCount++]='.';}}
            else{for(int j=0;j<widthField;j++){
                if(j==((x*2)+1)){field[fieldCount++]='.';}
                else{field[fieldCount++]=' ';}
            }}
        }else{for(int j=0;j<widthField;j++){field[fieldCount++]=' ';}}
        field[fieldCount++]='|';//~ border right
        field[fieldCount++]='\n';
    }
    field[fieldCount++]='+';
    for(int i=0;i<widthField;i++){field[fieldCount++]='-';}//~ border bottom
    field[fieldCount++]='+';
    field[fieldCount]='\0';
    field[(((widthField+3)*(fruitY+1))+((fruitX*2)+2))]='F';
    if(nTail>0){//~ snake tail
        int _tailpos=(((widthField+3)*(tailY[0]+1))+((tailX[0]*2)+2));
        field[_tailpos]=tailC[0];
        if(tailC[0]=='-'&&tailX[0]!=x){
            field[_tailpos+1]='-';
            field[_tailpos-1]='-';
        }
        if(portalWalls){
            if(dir==LEFT&&x==width-1){field[(((widthField+3)*(y+1))+((x*2)+2))+1]='-';}
            else if(dir==RIGHT&&x==0){field[(((widthField+3)*(y+1))+2)-1]='-';}
        }
        for(int i=1;i<nTail-1;i++){
            _tailpos=(((widthField+3)*(tailY[i]+1))+((tailX[i]*2)+2));
            field[_tailpos]=tailC[i];
            if(tailC[i]=='-'){
                field[_tailpos+1]='-';
                field[_tailpos-1]='-';
            }
        }
        if(nTail>1){
            int _tailpos=(((widthField+3)*(tailY[nTail-1]+1))+((tailX[nTail-1]*2)+2));
            field[_tailpos]=tailC[nTail-1];
            if(tailC[nTail-1]=='-'){
                if(tailX[nTail-1]==width-1||tailX[nTail-1]<tailX[nTail-2]){field[_tailpos+1]='-';}
                else if(tailX[nTail-1]==0||tailX[nTail-1]>tailX[nTail-2]){field[_tailpos-1]='-';}
            }
        }
    }
    switch(dir){//~ snake head
        case LEFT:field[(((widthField+3)*(y+1))+((x*2)+2))]='<';break;
        case UP:field[(((widthField+3)*(y+1))+((x*2)+2))]='A';break;
        case RIGHT:field[(((widthField+3)*(y+1))+((x*2)+2))]='>';break;
        case DOWN:field[(((widthField+3)*(y+1))+((x*2)+2))]='V';break;
        default:field[(((widthField+3)*(y+1))+((x*2)+2))]='O';break;
    };
    field[(((widthField+3)*(fruitY+1))+((fruitX*2)+2))-1]='[';
    field[(((widthField+3)*(fruitY+1))+((fruitX*2)+2))+1]=']';
    if(!screenClear(false)){exit(EXIT_FAILURE);}//~ clear screen
    if(_debugInfo){//~ print info under game
        std::cout
            <<field
            <<"\nidea:[https://youtu.be/E_-lMZDi7Uw]                                "
            <<"\n[ESC/q] Exit  [r] DelScore  [p] Pause  [wasd] Move  [b] DebugInfo  "
            <<"\nscore: "<<score<<"                                                 "
            <<"\n-------------------------------------------------------------------"
            <<"\nscorestep: "<<scoreStep<<"                                         "
            <<"\ndelayTime: "<<delayTime<<"                                         "
            <<"\nnTail: "<<nTail<<"                                                 "
            <<"\ndir: "<<dir<<"                                                     "
            <<"\nX/Y: "<<x<<'/'<<y<<"                                               "
            <<"\nfruitX/Y: "<<fruitX<<'/'<<fruitY<<"                                "
            <<"\nportalWalls: "<<portalWalls<<"                                     "
            <<"\n-------------------------------------------------------------------"
            <<"\n.                                                                  "
            <<std::endl;
    }else{
        std::cout
            <<field
            <<"\nidea:[https://youtu.be/E_-lMZDi7Uw]                                "
            <<"\n[ESC/q] Exit  [r] DelScore  [p] Pause  [wasd] Move  [b] DebugInfo  "
            <<"\nscore: "<<score<<"                                                 "
            <<"\n-------------------------------------------------------------------"
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<"\n.                                                                  "
            <<std::endl;
    }
}
void Input(){
    pdir=dir;
    if(_kbhit()){
        switch(_getch()){
            // wasd - HKPM (arrow_keys for _getch()) delay!
            case 'w':if(nTail==0||dir!=DOWN){dir=UP;}break;
            case 'a':if(nTail==0||dir!=RIGHT){dir=LEFT;}break;
            case 's':if(nTail==0||dir!=UP){dir=DOWN;}break;
            case 'd':if(nTail==0||dir!=LEFT){dir=RIGHT;}break;
            case 'b':_debugInfo=!_debugInfo;break;
            case 'p':
                if(!screenClear(true)){exit(EXIT_FAILURE);}
                if(_debugInfo){//~ print info under game
                    std::cout
                        <<field
                        <<"\nidea:[https://youtu.be/E_-lMZDi7Uw]                                "
                        <<"\n[ESC/q] Exit  [r] DelScore  [p] Pause  [wasd] Move  [b] DebugInfo  "
                        <<"\nscore: "<<score<<"                                                 "
                        <<"\n-------------------------------------------------------------------"
                        <<"\nscorestep: "<<scoreStep<<"                                         "
                        <<"\ndelayTime: "<<delayTime<<"                                         "
                        <<"\nnTail: "<<nTail<<"                                                 "
                        <<"\ndir: "<<dir<<"                                                     "
                        <<"\nX/Y: "<<x<<'/'<<y<<"                                               "
                        <<"\nfruitX/Y: "<<fruitX<<'/'<<fruitY<<"                                "
                        <<"\nportalWalls: "<<portalWalls<<"                                     "
                        <<"\n-------------------------------------------------------------------"
                        <<"\n.          ##  PAUSED  ##  press any key to continue  ##           "
                        <<std::endl;
                }else{
                    std::cout
                        <<field
                        <<"\nidea:[https://youtu.be/E_-lMZDi7Uw]                                "
                        <<"\n[ESC/q] Exit  [r] DelScore  [p] Pause  [wasd] Move  [b] DebugInfo  "
                        <<"\nscore: "<<score<<"                                                 "
                        <<"\n-------------------------------------------------------------------"
                        <<"\n.                                                                  "
                        <<"\n.                                                                  "
                        <<"\n.                                                                  "
                        <<"\n.                                                                  "
                        <<"\n.          ##  PAUSED  ##  press any key to continue  ##           "
                        <<"\n.                                                                  "
                        <<"\n.                                                                  "
                        <<"\n.                                                                  "
                        <<"\n.                                                                  "
                        <<std::endl;
                }
                _getch();
                break;
            case 'r':
                fruitX=rand()%width;
                fruitY=rand()%height;
                score=0;
                scoreStep=0;
                tailX[0]=x;
                tailY[0]=y;
                nTail=0;
                break;
            case '':
            case 'q':
                exit(EXIT_SUCCESS);
                break;
            default:break;
        }
    }
}
void Logic(){
    if(nTail>0){//~ snake tail
        for(int i=nTail-1;i>0;i--){//~ tail movement
            tailX[i]=tailX[i-1];
            tailY[i]=tailY[i-1];
            tailC[i]=tailC[i-1];
            if(tailX[i]==x&&tailY[i]==y){gameOver=true;return;}//~ on tail
        }
        tailX[0]=x;
        tailY[0]=y;
        if(pdir==dir){//~tail character
            if(dir==LEFT||dir==RIGHT){tailC[0]='-';}
            else if(dir==UP||dir==DOWN){tailC[0]='|';}
            else{tailC[0]='+';}
        }else{tailC[0]='+';}
    }
    switch(dir){//~ snake movement
        case LEFT:x--;break;
        case RIGHT:x++;break;
        case UP:y--;break;
        case DOWN:y++;break;
        default:break;
    }
    if(portalWalls){//~ no-walls ~ loop-screen
        if(x>=width){x=0;}
        else if(x<0){x=width-1;}
        if(y>=height){y=0;}
        else if(y<0){y=height-1;}
    }else if(x>=width||y>=height||x<0||y<0){Setup();gameOver=true;return;}//~ out of bounds
    if(dir>0){//~ survive bonus
        if(++scoreStep%100==0){score+=2;}
        if(scoreStep>=1000){scoreStep=0;score+=2;}
    }
    if(x==fruitX&&y==fruitY){//~ collect fruit and delay decrease
        score+=10;
        if(delayTime>1e-20){delayTime*=.99;}
        else{delayTime=0;}
        nTail++;
        if(nTail>1){
            tailX[nTail-1]=tailX[nTail-2];
            tailY[nTail-1]=tailY[nTail-2];
            tailC[nTail-1]=tailC[nTail-2];
        }else{
            tailX[0]=x;
            tailY[0]=y;
            if(pdir==dir){//~tail character
                if(dir==LEFT||dir==RIGHT){tailC[0]='-';}
                else if(dir==UP||dir==DOWN){tailC[0]='|';}
                else{tailC[0]='+';}
            }else{tailC[0]='+';}
        }
        fruitX=(rand()%(width-2))+1;
        fruitY=(rand()%(height-2))+1;
    }
}
int toNum(char const*num){
    int a=0;
    for(int i=0,t;num[i]!='\0';i++){
        t=num[i]-'0';
        if(t>=0&&t<=9){a*=10;a+=t;}else{return -1;}
    }
    return a;
}
int main(int argc, char const *argv[]){
    delayTimeOg=200;
    for(int i=1;i<argc;i++){//~ run.exe -t 100 -p -b
        if(argv[i][0]=='-'){
            switch(argv[i][1]){
                case 't':
                    delayTimeOg=toNum(argv[i+1]);
                    if(delayTimeOg<0){delayTimeOg=0;}
                    else if(delayTimeOg>60000){delayTimeOg=60000;}
                    break;
                case 'p':portalWalls=true;break;
                case 'b':_debugInfo=true;break;
                default:break;
            }
        }
    }
    Setup();
    while(!gameOver){
        Draw();
        if(delayTime>0){Sleep(delayTime);}
        Input();
        Logic();
        if(gameOver){
            if(!screenClear(true)){return EXIT_FAILURE;}
            if(_debugInfo){//~ print info under game
                std::cout
                    <<field
                    <<"\nidea:[https://youtu.be/E_-lMZDi7Uw]                                "
                    <<"\n[ESC/q] Exit  [r] DelScore  [p] Pause  [wasd] Move  [b] DebugInfo  "
                    <<"\nscore: "<<score<<"                                                 "
                    <<"\n-------------------------------------------------------------------"
                    <<"\nscorestep: "<<scoreStep<<"                                         "
                    <<"\ndelayTime: "<<delayTime<<"                                         "
                    <<"\nnTail: "<<nTail<<"                                                 "
                    <<"\ndir: "<<dir<<"                                                     "
                    <<"\nX/Y: "<<x<<'/'<<y<<"                                               "
                    <<"\nfruitX/Y: "<<fruitX<<'/'<<fruitY<<"                                "
                    <<"\nportalWalls: "<<portalWalls<<"                                     "
                    <<"\n-------------------------------------------------------------------"
                    <<"\n.          ##  G a m e - O v e r  ##  again ?  [y/n]  ##           "
                    <<std::endl;
            }else{
                std::cout
                    <<field
                    <<"\nidea:[https://youtu.be/E_-lMZDi7Uw]                                "
                    <<"\n[ESC/q] Exit  [r] DelScore  [p] Pause  [wasd] Move  [b] DebugInfo  "
                    <<"\nscore: "<<score<<"                                                 "
                    <<"\n-------------------------------------------------------------------"
                    <<"\n.                                                                  "
                    <<"\n.                                                                  "
                    <<"\n.                                                                  "
                    <<"\n.                                                                  "
                    <<"\n.          ##  G a m e - O v e r  ##  again ?  [y/n]  ##           "
                    <<"\n.                                                                  "
                    <<"\n.                                                                  "
                    <<"\n.                                                                  "
                    <<"\n.                                                                  "
                    <<std::endl;
            }
            int _repeat=_getch();
            while(_repeat!='n'){
                if(_repeat=='y'||_repeat=='r'){Setup();break;}
                else if(_repeat==0x1b||_repeat=='q'){return EXIT_SUCCESS;}
                else{_repeat=_getch();}
            }
        }
    }
    return EXIT_SUCCESS;
}

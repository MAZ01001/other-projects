# Some useful [FFmpeg](https://ffmpeg.org/) commands

- [official documentation](#official-documentation)
- [Hide all but stats when running a command](#hide-all-but-stats-when-running-a-command)
- [Convert MP4 to M4A (video to audio)](#convert-mp4-to-m4a-video-to-audio)
- [Extracting metadata to file](#extracting-metadata-to-file)
  - [Edit metadata file](#edit-metadata-file)
  - [Reinserting (edited) metadata](#reinserting-edited-metadata)
- [Add thumbnail](#add-thumbnail)
- [Add subtitles](#add-subtitles)
  - [subtitle file format](#subtitle-file-format)
- [Extract frames](#extract-frames)
  - [Create video from frames](#create-video-from-frames)
    - [Watch short video as a loop](#watch-short-video-as-a-loop)
- [crop video](#crop-video)
- [compress video](#compress-video)
- [cut video](#cut-video)
- [loop video](#loop-video)
- [reverse video](#reverse-video)
- [concat videos](#concat-videos)
- [create/download video with m3u8 playlist](#createdownload-video-with-m3u8-playlist)
- [find silence parts in video](#find-silence-parts-in-video)
- [my current FFmpeg version](#my-current-ffmpeg-version)

## official documentation

- [FFmpeg - all](https://ffmpeg.org/ffmpeg-all.html)
- [FFplay - all](https://ffmpeg.org/ffplay-all.html)
- [FFprobe - all](https://ffmpeg.org/ffprobe-all.html)

## Hide all but stats when running a command

    ffmpeg [-hide_banner] [-v {[level+]&[quiet|error|warning|info|verbose]}] -stats [...]

- [-hide_banner] - hides the big version/configuration block in the beginning
- [-v|-loglevel]
  - [level] - show what log_level each log is
    - (optional) use first after `-v` and then with a `+` the log_level after it
      - `-v error` → `-v level+error`
    - shows the log_level in "[]" before each logged message
  - [quiet] - show nothing
  - [error] - only show errors _(incl. recoverable errors)_
  - [warning] - only show warnings and errors _(I usually use this one 'cause it also hides the big "config-banner")_
  - [info] - show above plus informative messages like file metadata _(default)_
  - [verbose] - just like info but more verbose
- [-stats] - show stats like how far encoding is live
  - [-nostats] - for no live stats

## Convert MP4 to M4A (video to audio)

- no video = smaller file size
- metadata and subtitles are preserved
- no conversion to MP3, uses original audio codec from MP4 file
- same audio codec =  can be played by anything that can play MP4 (audio)

      ffmpeg -i ".\INPUT.mp4" -c copy -map 0:a -map 0:s? ".\OUTPUT.m4a"

## Extracting metadata to file

    ffmpeg -i ".\INPUT.mp4" -f ffmetadata ".\FFMETADATAFILE.txt"

### Edit metadata file

    ;FFMETADATA1
     [...]
    title=Video Title
    artist=Artist Name
    description=Text\
    Line two\
    \
    \
    Line five\
    Line with Û̕͝͡n̊̑̓̊i͚͚ͬ́c̗͕̈́̀o̵̯ͣ͊ḑ̴̱̐ḛ̯̓̒\
     [...]
    Line twenty

- adding chapters _(order does not matter so easiest is append to end of file)_

      [CHAPTER]
      TIMEBASE=1/1000
      START=0
      END=10000
      title=0 to 10sec of the video
      [CHAPTER]
      TIMEBASE=1/1000
      START=10000
      END=20000
      title=10sec to 20sec of the video

  - TIMEBASE - 1/1000 _(of a sec)_ = milliseconds for setting start/end
    - the actual point is set to the next nearest frame that exists
    - only on re-encoding _(during reinsertion)_ it will be exact after that amount of time
  - START - start of chapter in milliseconds _(or according to TIMEBASE)_
  - END - end of chapter in milliseconds _(or according to TIMEBASE)_
  - title - title of this chapter

### Reinserting (edited) metadata

    ffmpeg -i ".\INPUT.mp4" -i ".\FFMETADATAFILE.txt" -map_metadata 1 -codec copy ".\OUTPUT.mp4"

- empty lines in metadata file will be ignored
  and order doesn't matter except for the ";FFMETADATA1" in the first line

## Add thumbnail

    ffmpeg -i ".\INPUT.mp4" -i ".\IMAGE.png" -map 0 -map 1 -c copy -c:v:1 png - disposition:v:1 attached_pic ".\OUTPUT.mp4"

## Add subtitles

- adding subtitles as an extra stream so they can be turned on and off
  - must use a player that supports this like [VLC](https://www.videolan.org/vlc/index.en_GB.html)
- ... for mp4 output

      ffmpeg -i ".\INPUT.mp4" -i ".\SUB.srt" -c copy -c:s mov_text ".\OUTPUT.mp4"

- ... for mkv output

      ffmpeg -i ".\INPUT.mp4" -i ".\SUB.srt" -c copy ".\OUTPUT.mkv"

- multiple subtitle files

      ffmpeg -i ".\INPUT.mp4" -i ".\SUB_ENG.srt" -i ".\SUB_GER.srt" -map 0:0 -map 1:0 -map 2:0 -c copy -c:s mov_text ".\OUTPUT.mp4"

  - with language codes

        ffmpeg -i ".\INPUT.mp4" -i ".\SUB_ENG.srt" -i ".\SUB_GER.srt" -map 0:0 -map 1:0 -map 2:0 -c copy -c:s mov_text -metadata:s:s:0 language=eng -metadata:s:s:1 language=ger ".\OUTPUT.mp4"

### subtitle file format

    1
    00:00:00,000 --> 00:00:03,000
    hello there

    2
    00:00:04,000 --> 00:00:08,000
    general kenobi

    3
    00:00:10,000 --> 00:01:00,000
    multi
    line
    subtitles

- Unicode can be used - tested with z̧̢̛̻̱̝͖ͤͯͪ̏ͤ̀ͩ̂̅͒̕͞ͅa̵̸̡̯̼̠͑̑ͫ̔̉̈̉͊ͥ̍̿̂͝͝l̵̥̮̳̖̟̗ͧ̆ͣ͋͋̐̌͊ͩ̇̋̀̚g̴̴͉̲͖͉̱̪̙̣͙̩̪͈̈́ͬ̎̄̈͠ó̵̰̼͈͗̔͌ͩ̽̌̒̓ͨ̕͝ text and it "pushed" the subtitles of screen - big line height
- displayed like in file
  - new line in SRT = new line in video

## Extract frames

    ffmpeg -i ".\INPUT.mp4" ".\_dump\frame%03d.png"

- "frame%03d.png" = frame000.png / frame001.png / frame050.png / frame1000.png
- jpeg slow - bmp large
- _(subfolder must be created first)_
- every x frames x pictures

      ffmpeg [-r 1] -i ".\INPUT.mp4" [-r 1] ".\_dump\frame%03d.png"

  - _(first and second "-r 1")_
  - if only first is omitted then every 1/x sec a pic
  - if both are omitted then all frames
  - or use -ss -t to give the timeframe for extraction

### Create video from frames

    ffmpeg -framerate 24 -i [".\INPUT%03d.jpeg"|".\INPUT*.png"] ".\OUTPUT.mp4"

#### Watch short video as a loop

    ffplay -loop -1 ".\INPUT.mp4"

- a window will show the video looping infinitly
  - [controls](https://ffmpeg.org/ffplay.html#While-playing)

## crop video

    ffmpeg -i ".\INPUT.mp4" -vf "crop=WIDTH_PX:HEIGHT_PX:POSX_PX:POSY_PX" ".\OUTPUT.mp4"

- `WIDTH_PX` - the width of the croped window
- `HEIGHT_PX` - the height of the croped window
- `POSX_PX` - the X position of the croped window (can be omitted = auto center)
- `POSY_PX` - the Y position of the croped window (can be omitted = auto center)
- all in pixels, but just the integer number no "px" after it !
- for more flags, info, and examples [see the documentation](https://ffmpeg.org/ffmpeg-filters.html#crop "official ffmpeg documentation for the crop filter")

## compress video

- need re-encoding for compression _(when in doubt, choose the same video-codec as input video)_
- [-crf] - lower is better because more bitrate but also a higher file size
- h.264 18-23 _(very good quality)_

      ffmpeg -i ".\INPUT.mp4" -vcodec libx264 -crf 18 ".\OUTPUT.mp4"

- h.265 24-30 _(very good quality)_

      ffmpeg -i ".\INPUT.mp4" -vcodec libx265 -crf 24 ".\OUTPUT.mp4"

## cut video

- start at 1sec and stop at 10sec

      ffmpeg -ss 0:0:1 -to 0:0:10 -i ".\INPUT.mp4" -c copy ".\OUTPUT.mp4"

- start at 10sec and stop after 10sec

      ffmpeg -ss 0:0:10 -t 0:0:10 -i ".\INPUT.mp4" -c copy ".\OUTPUT.mp4"

> NOTE \
> with `-ss`, `-to`, and `-t` ffmpeg picks the nearest frame and not at the exact timestamp \
> if exact time is needed add `-vcodec libx264` or `-c:v libx264` after `-c copy` to recompile the video to the exact timestamps _(takes longer)_ \
> also the timestamp is in format `[[H:]m:]s[.ms]` so default is seconds

- limit output to 30sec total

      ffmpeg -i ".\INPUT.mp4" -c copy -t 30 ".\OUTPUT.mp4"

## loop video

- loop video infinitely but stop after 30sec _(loop up to 30sec)_

      ffmpeg -stream_loop -1 -i ".\INPUT.mp4" -c copy -t 30 ".\OUTPUT.mp4"

- loop video to length of audio

      ffmpeg  -stream_loop -1 -i ".\INPUT.mp4" -i ".\INPUT.mp3" -shortest -map 0:v -map 1:a ".\OUTPUT.mp4"

- loop audio to length of video

      ffmpeg  -i ".\INPUT.mp4" -stream_loop -1 -i ".\INPUT.mp3" -shortest -map 0:v -map 1:a ".\OUTPUT.mp4"

## reverse video

- video

      ffmpeg -i ".\INPUT.mp4" -vf reverse ".\OUTPUT.mp4"

- audio

      ffmpeg -i ".\INPUT.mp4" -af areverse ".\OUTPUT.mp4"

## concat videos

- using filter complex

      ffmpeg -i ".\INPUT_0.mp4" -i ".\INPUT_1.mp4" -filter_complex "[0:v] [0:a] [1:v] [1:a] concat=n=2:v=1:a=1 [v] [a]" -map "[v]" -map "[a]" ".\OUTPUT.mp4"

- using demuxer
  - video files listed in `".\VIDEO_LIST.txt"` like this

        file '.\INPUT_0.mp4'
        file '.\INPUT_1.mp4'

  - command

        ffmpeg -safe 0 -f concat -i ".\VIDEO_LIST.txt" -c copy ".\OUTPUT.mp4"

## create/download video with m3u8 playlist

    ffmpeg -protocol_whitelist file,http,https,tcp,tls,crypto -i [".\INPUT.m3u8"|"https://INPUT.m3u8"] -c copy ".\OUTPUT.mp4"

## find silence parts in video

    ffmpeg -i ".\INPUT.mp4" -af silencedetect=noise=-70dB:d=240 -f null - 2> ".\LOG.txt"

- [noise=-70dB] = -70dB or quieter
- [d=240] = 240sec/4min minimum silence duration for detect
- look for "[silencedetect*" lines in log file like:

      [silencedetect @ 0000000000******] silence_start: 01:00:02.500
      [silencedetect @ 0000000000******] silence_end: 01:10:02.500 | silence_duration: 00:09:59.989
      [silencedetect @ 000000000*******] silence_start: 02:00:02.500
      [silencedetect @ 000000000*******] silence_end: 02:10:02.500 | silence_duration: 00:09:59.989
      [...]

## my current FFmpeg version

    ffmpeg -version

- 2021-12-00
  - ffmpeg version 4.2.2 Copyright (c) 2000-2019 the FFmpeg developers
  - built with gcc 9.2.1 (GCC) 20200122
- 2022-10-00
  - ffmpeg version 5.0-full_build-www.gyan.dev Copyright (c) 2000-2022 the FFmpeg developers
  - built with gcc 11.2.0 (Rev5, Built by MSYS2 project)

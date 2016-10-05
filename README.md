![](https://pbs.twimg.com/client_application_images/54927/shelltwit.png)
shelltwit
=========

[![Join the chat at https://gitter.im/sebagomez/shelltwit](https://badges.gitter.im/sebagomez/shelltwit.svg)](https://gitter.im/sebagomez/shelltwit?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/1m0mqeskgew1ry4o?svg=true)](https://ci.appveyor.com/project/sebagomez/shelltwit)

shelltwit allows you to update your twitter status via command line.   
It's a good example of twitter API calls with xAuth. It also has bit.ly integration for url shortening.

![ScreenShot](http://farm9.staticflickr.com/8363/8319299202_9282a7e6ed.jpg)


Little more (?) info about it at the original [blog post](http://sgomez.blogspot.com/2010/06/introducing-shelltwit.html)

Implemented Twitter APIs
------------------------
- [Status Update](https://dev.twitter.com/rest/reference/post/statuses/update)
- [Status Mentions](https://dev.twitter.com/rest/reference/get/statuses/mentions_timeline)
- [Status Home Timeline](https://dev.twitter.com/rest/reference/get/statuses/home_timeline)
- [Status User Timeline](https://dev.twitter.com/rest/reference/get/statuses/user_timeline)
- [Search](https://dev.twitter.com/rest/public/search)
- [Favorites List](https://dev.twitter.com/rest/reference/get/favorites/list)

Build
-----
After downloading the source files build the sln with Visual Studio 2015 (framework 4.6)
There's also a .net core branch. It uses the same files from the framework 4.6 projects, use the build.bat to copy the files into the project.

```
Command line Twitter client running on Microsoft Windows 10.0.14393
Copyright c 2010-2016 v4.0.0.25009

Usage: twit /q <query>|/c|/tl|/m|/l|/u <user>|/?|<status> [<mediaPath>]

/c              : clears user stored credentials
/tl             : show user's timeline (default)
/q              : query twits containing words
/m              : show user's mentions
/u user         : show another user's timeline
/l              : user's likes (fka favorites)
/?              : show this help
status          : status to update at twitter.com
mediaPath       : full path, between brackets, to the media files (up to four) to upload.
```
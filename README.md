![](https://pbs.twimg.com/client_application_images/54927/shelltwit.png)

shelltwit
=========

[![Join the chat at https://gitter.im/sebagomez/shelltwit](https://badges.gitter.im/sebagomez/shelltwit.svg)](https://gitter.im/sebagomez/shelltwit?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Travis Build Status](https://travis-ci.org/sebagomez/shelltwit.svg?branch=master)](https://travis-ci.org/sebagomez/shelltwit)
[![](https://images.microbadger.com/badges/image/sebagomez/shelltwit.svg)](https://microbadger.com/images/sebagomez/shelltwit)
[![](https://images.microbadger.com/badges/version/sebagomez/shelltwit.svg)](https://microbadger.com/images/sebagomez/shelltwit)

shelltwit allows you to update your twitter status via command line. It also has bit.ly integration for url shortening.
~~It's a good example of twitter API calls with xAuth.~~ As of Jun-05-2017 it uses [PIN-based](https://dev.twitter.com/oauth/pin-based) authorization

<img src="res//Windows.png" alt="shelltwit on windows" width="700">
<img src="res//Ubuntu.png" alt="shelltwit on ubuntu" width="700">

Little more (?) info about it at the original [blog post](http://sgomez.blogspot.com/2010/06/introducing-shelltwit.html)

Implemented Twitter APIs
------------------------
- [Status Update](https://dev.twitter.com/rest/reference/post/statuses/update)
- [Status Mentions](https://dev.twitter.com/rest/reference/get/statuses/mentions_timeline)
- [Status Home Timeline](https://dev.twitter.com/rest/reference/get/statuses/home_timeline)
- [Status User Timeline](https://dev.twitter.com/rest/reference/get/statuses/user_timeline)
- [Search](https://dev.twitter.com/rest/public/search)
- [Favorites List](https://dev.twitter.com/rest/reference/get/favorites/list)
- [Streaming statuses](https://dev.twitter.com/streaming/reference/post/statuses/filter)
- [Streaming user](https://dev.twitter.com/streaming/userstreams)

Build
-----
As of Sep 10th 2017 there's a single solution in .net Core 2.0. The old .net framework 4.6 was removed since there was no reason to keep both of them.
After downloading the repo just build the sln with Visual Studio 2017 (15.3)

This build has been tested on Ubuntu [WSL](https://en.wikipedia.org/wiki/Windows_Subsystem_for_Linux), and it [works on my machine](https://blog.codinghorror.com/the-works-on-my-machine-certification-program/) <img src="res//works on my machine.png" alt="Works on my machine" height="50">.

For my next trick I'll create a docker image so it will definetely works on yours too.

```
Sebagomez.Shelltwit version 7.3.1.0 for Microsoft Windows 10.0.17115
Copyright (C) @sebagomez. All rights reserved.

Usage: twit [options] | <status> [<mediaPath>]

Options:
        -c|--clear      clears user stored credentials
        -tl|--timeline  show user's timeline
        -q|--query      query twits containing words
        -m|--mentions   show user's mentions
        -u|--user       show another user's timeline
        -t|--track      live status with a specific track
        -s|--streamed   streamed user timeline
        -l|--likes      user's likes (fka favorites)
        -h|--help       show this help

status:
        status to update at twitter.com

mediaPath:
        full path, between brackets, to the media files (up to four) to upload.

```

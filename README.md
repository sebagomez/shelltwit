![](res/shelltwit.png?raw=true)

# shelltwit

[![Join the chat at https://gitter.im/sebagomez/shelltwit](https://badges.gitter.im/sebagomez/shelltwit.svg)](https://gitter.im/sebagomez/shelltwit?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Travis Build Status](https://travis-ci.org/sebagomez/shelltwit.svg?branch=master)](https://travis-ci.org/sebagomez/shelltwit)
[![](https://images.microbadger.com/badges/image/sebagomez/shelltwit.svg)](https://microbadger.com/images/sebagomez/shelltwit)
[![](https://images.microbadger.com/badges/version/sebagomez/shelltwit.svg)](https://microbadger.com/images/sebagomez/shelltwit)
[![Build Status](https://sebagomez.visualstudio.com/shelltwit/_apis/build/status/sebagomez.shelltwit)](https://sebagomez.visualstudio.com/shelltwit/_build/latest?definitionId=4)

shelltwit updates your twitter status while at the command line. It also has [bit.ly](http://bit.ly) integration for url shortening.
~~It's a good example of twitter API calls with xAuth.~~ As of Jun-05-2017 it uses [PIN-based](https://dev.twitter.com/oauth/pin-based) authorization

shelltwit running on Windows
![](res/Windows.gif?raw=true)

shelltwit running on Linux (WSL)
![](res/Ubuntu.gif?raw=true)


Little more (?) info about it at the original [blog post](http://sgomez.blogspot.com/2010/06/introducing-shelltwit.html)

## Implemented Twitter APIs

- [Status Update](https://dev.twitter.com/rest/reference/post/statuses/update)
- [Status Mentions](https://dev.twitter.com/rest/reference/get/statuses/mentions_timeline)
- [Status Home Timeline](https://dev.twitter.com/rest/reference/get/statuses/home_timeline)
- [Status User Timeline](https://dev.twitter.com/rest/reference/get/statuses/user_timeline)
- [Search](https://dev.twitter.com/rest/public/search)
- [Favorites List](https://dev.twitter.com/rest/reference/get/favorites/list)
- [Streaming statuses](https://dev.twitter.com/streaming/reference/post/statuses/filter)
- [Streaming user](https://dev.twitter.com/streaming/userstreams)

## Build

As of April 29th 2019 there's a single .NET Core 2.2 solution, which reference newly created NuGet packages.
After downloading the repo just build the sln with Visual Studio 2017 (15.9+).

This build has been tested on Ubuntu [WSL](https://en.wikipedia.org/wiki/Windows_Subsystem_for_Linux), and it [works on my machine](https://blog.codinghorror.com/the-works-on-my-machine-certification-program/) 

but it can now work on your machine thanks to [Docker](https://docker.com)  

## Docker

In order to build your own container you must run the following command

``` docker
docker run -e "TWIT_KEY=<Your Twitter Key>" -e "TWIT_SECRET=<Your Twitter Secret>" -it --name twit sebagomez/shelltwit 
```

![](res/PINAuthorization.png?raw=true)

Copy and paste the provided URL in your favorite browser, authorize the app to access twitter on your behalf and copy and paste the provided PIN in the command line waiting for it.

![](res/TwitterPIN.png?raw=true)

Now you have a container with the needed credentials to access the Twitter API.

We'll now commit those changes into a new image:

``` docker
docker commit twit mytwit
```

And that's it, you can now call commands inside the newly created image as follows:

``` docker
docker run --rm mytwit --help
```

```
🐤Sebagomez.Shelltwit version 8.2.2.0-20181004.1 running on Linux 4.9.93-linuxkit-aufs #1 SMP Wed Jun 6 16:55:56 UTC 2018
Copyright (C) @sebagomez. All rights reserved.

Usage: twit [options] | <status> [<mediaPath>]

Options:
        -c|--clear              clears user stored credentials
        -t|--timeline [count]   show user's timeline, optionally set how many twits to display (up to 200)
        -q|--query <query>      query twits containing words
        -m|--mentions           show user's mentions
        -u|--user <handle>      show another user's timeline
        -k|--track <track>      live status with a specific track
        -s|--streamed <handle>  streamed user timeline
        -l|--likes              user's likes (fka favorites)
        -h|--help               show this help

status:
        status to update at twitter.com

mediaPath:
        full path, between brackets, to the media files (up to four) to upload.
```

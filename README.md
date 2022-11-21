[![.NET Build](https://github.com/sebagomez/shelltwit/actions/workflows/build.yaml/badge.svg?branch=master)](https://github.com/sebagomez/shelltwit/actions/workflows/build.yaml)
[![Docker Build](https://github.com/sebagomez/shelltwit/actions/workflows/docker.yaml/badge.svg?branch=master)](https://github.com/sebagomez/shelltwit/actions/workflows/docker.yaml)  
[![Join the chat at https://gitter.im/sebagomez/shelltwit](https://badges.gitter.im/sebagomez/shelltwit.svg)](https://gitter.im/sebagomez/shelltwit?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

![](https://github.com/sebagomez/shelltwit/blob/master/res/shelltwit.png?raw=true)

# shelltwit

shelltwit updates your twitter status while at the command line. It also has [bit.ly](http://bit.ly) integration for url shortening.
~~It's a good example of twitter API calls with xAuth.~~ As of Jun-05-2017 it uses [PIN-based](https://dev.twitter.com/oauth/pin-based) authorization

shelltwit running on Windows
![](https://github.com/sebagomez/shelltwit/blob/master/res/Windows.gif?raw=true)

shelltwit running on Linux (WSL2)
![](https://github.com/sebagomez/shelltwit/blob/master/res/Ubuntu.gif?raw=true)


Little more (?) info about it at the original [blog post](http://sgomez.blogspot.com/2010/06/introducing-shelltwit.html)

## Build

The repo contains a single .NET solution, which reference newly created [twitterlib](https://github.com/sebagomez/twitterlib) [NuGet package](https://www.nuget.org/packages/Sebagomez.TwitterLib/).
After downloading the repo just build the sln with Visual Studio 2019.

This build has been tested on Ubuntu [WSL](https://en.wikipedia.org/wiki/Windows_Subsystem_for_Linux), and it [works on my machine](https://blog.codinghorror.com/the-works-on-my-machine-certification-program/). It can now work on your machine thanks to [Docker](https://docker.com)  

For obvious security reasons I don't have the app Key and Secrets embeded in the code. If you want to run the app with your own crdentials you can setup two Environment variables named `TWIT_KEY` and `TWIT_SECRET`. With these, shelltwit will run as your registered twitter app. 

You can also build the app with the provided [build.cmd](./build.cmd) file if you're running on Windows or [build.sh](./build.sh) script if you're on Mac or Linux.

After that just open an terminal at the bin folder and run the application:  
```dotnet
dotnet Sebagomez.Shelltwit.dll
```

## Docker

In order to build your own container you must run the following command

``` docker
docker run -e "TWIT_KEY=<Your Twitter Key>" -e "TWIT_SECRET=<Your Twitter Secret>" -it --name twit sebagomez/shelltwit 
```

![](https://github.com/sebagomez/shelltwit/blob/master/res/PINAuthorization.png?raw=true)

Copy and paste the provided URL in your favorite browser, authorize the app to access twitter on your behalf and copy and paste the provided PIN in the command line waiting for it.

![](https://github.com/sebagomez/shelltwit/blob/master/res/TwitterPIN.png?raw=true)

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
🐤 Sebagomez.Shelltwit version 9.0.0.0                                                                                                                  
Copyright 2022 @SebaGomez                                                                                                                               
                                                                                                                                                        
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
        -d|--dm <handle> <message>      direct message to user
        -h|--help               show this help

status:
        status to update at twitter.com

mediaPath:
        full path, between brackets, to the media files (up to four) to upload.
```

_Edit: You can also use the following command to copy your existing credentials into your docker container_  
```
docker cp ./last.usr twit:/twit
docker cp ./sebagomez.data twit:/twit
```

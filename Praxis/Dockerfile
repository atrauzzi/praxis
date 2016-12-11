FROM microsoft/dotnet:1.1.0-sdk-projectjson
MAINTAINER Alexander Trauzzi <atrauzzi@gmail.com>

WORKDIR /Praxis

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN echo "deb http://download.mono-project.com/repo/debian wheezy main" | tee /etc/apt/sources.list.d/mono-xamarin.list
RUN echo "deb http://download.mono-project.com/repo/debian wheezy-libjpeg62-compat main" | tee -a /etc/apt/sources.list.d/mono-xamarin.list
RUN apt-get update
RUN apt-get -y upgrade
RUN apt-get -y install mono-complete

RUN wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -O /usr/local/bin/nuget
RUN chmod +x /usr/local/bin/nuget

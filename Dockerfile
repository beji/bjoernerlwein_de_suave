FROM geoder101/fsharp-extended
RUN apt-get install -y nuget
RUN mkdir /app
WORKDIR /app
ADD . /app

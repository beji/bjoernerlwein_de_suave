FROM geoder101/fsharp-extended
RUN mkdir /app
WORKDIR /app
ADD . /app

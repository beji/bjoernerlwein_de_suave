FROM geoder101/fsharp-extended
RUN npm install -g less
RUN mkdir /app
WORKDIR /app
ADD . /app

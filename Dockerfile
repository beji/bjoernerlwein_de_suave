FROM frolvlad/alpine-mono
RUN apk -U add libuv
RUN mkdir /app
WORKDIR /app
ADD ./build /app
CMD mono bjoernerlwein_de.exe production

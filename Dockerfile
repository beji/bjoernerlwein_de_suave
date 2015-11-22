FROM alpine
RUN echo "@testing http://nl.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories
RUN apk --update add mono@testing
RUN mkdir /app
WORKDIR /app
ADD ./build /app
CMD mono bjoernerlwein_de.exe production
